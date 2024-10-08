using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Siskos_LOL_int_list
{
    public partial class Form1 : Form
    {
        private readonly List<string> _intListSummoners = new List<string>();

        private readonly string _dbListPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IntList/DbList.xml");
        private readonly string _leagueLocationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IntList/leagueloc");
        private readonly string _intListFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IntList");
        private readonly LockFile _lockFile = new LockFile();

        private static int _port;
        private static string _lockfilePw;

        private static bool _isOnline;

        private bool _dragging;
        private Point _dragCursor;
        private Point _dragForm;


        public Form1()
        {
            InitializeComponent();
            InitColors();
            LoadSummonersFromDb();
            SetLockFilePathFromSafe();
            OnlineCheckLoop();
        }

        private void LoadSummonersFromDb()
        {
            CreateDbListFile();

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(_dbListPath);

            foreach (XmlNode node in xmlDocument)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    var name = childNode.InnerText;
                    _intListSummoners.Add(name);
                }
            }
        }

        private void SetLockFilePathFromSafe()
        {
            if (!File.Exists(_leagueLocationPath)) return;
            using (var reader = new StreamReader(_leagueLocationPath))
            {
                _lockFile.FilePath = $@"{reader.ReadToEnd()}\lockfile";
            }
        }

        private void UpdateLeagueLocationFile(string path)
        {
            using (var writer = new StreamWriter(_leagueLocationPath))
            {
                writer.Write(path);
            }
        }

        private void CreateDbListFile()
        {
            if (File.Exists(_dbListPath)) return;
            if (!Directory.Exists(_intListFolderPath))
            {
                Directory.CreateDirectory(_intListFolderPath);
            }
            using (var writer = new XmlTextWriter(_dbListPath, null))
            {
                writer.Formatting = System.Xml.Formatting.Indented;

                var doc = new XmlDocument();
                doc.LoadXml("<ArrayOfSummoner></ArrayOfSummoner>");
                doc.Save(writer);
            }
        }

        private void InitColors()
        {
            BackColor = Color.FromArgb(37, 37, 37);
            btnAdd.BackColor = Color.FromArgb(80, 80, 80);
            txtbxSummonerName.ForeColor = Color.Silver;
            btnSelectFolder.BackColor = Color.FromArgb(80, 80, 80);
            pnlHeader.BackColor = Color.FromArgb(50, 50, 50);
        }

        private void IntListLoop()
        {
            if (IsInGame())
            {
                var gameSummonerNames = GetSummonerNames();

                var anyInIntList = _intListSummoners.Intersect(gameSummonerNames).Any();
                if (!anyInIntList)
                {
                    lblIntListText.Text = @"No summoners found on your int list. GLHF";
                    return;
                }

                lblIntListText.Text = @"The following people are on your int list";
                foreach (var name in gameSummonerNames)
                {
                    if (lblIntList.Text.Contains(name) || !_intListSummoners.Contains(name)) continue;
                    PlayPopSound();
                    lblIntList.Text += $"- {name}\r\n";
                    WindowState = FormWindowState.Normal;
                    Activate();
                }
            }
            else
            {
                lblIntList.Text = "";
                lblIntListText.Text = @"Currently not in game";
            }
        }

        private static void PlayPopSound()
        {
            Stream str = Properties.Resources.pop1;
            var snd = new System.Media.SoundPlayer(str);
            snd.Play();
        }

        private IEnumerable<string> GetSummonerNames()
        {
            using (var response = GetEndpointResponse("/lol-gameflow/v1/session"))
            {
                var summonerNames = new List<string>();

                using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    var jsonString = reader.ReadToEnd();
                    dynamic data = JObject.Parse(jsonString);
                    foreach (var obj in data.gameData.playerChampionSelections)
                    {
                        summonerNames.Add((string)obj.summonerInternalName);
                    }

                    return summonerNames;
                }
            }
        }

        private async void OnlineCheckLoop()
        {
            while (true)
            {
                try
                {
                    using (Stream s = new FileStream(_lockFile.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var sr = new StreamReader(s);
                        var text = await sr.ReadToEndAsync();
                        var splitText = text.Split(':');
                        int.TryParse(splitText[2], out _port);
                        _lockfilePw = splitText[3];
                        _isOnline = true;
                    }
                }
                catch (FileNotFoundException)
                {
                    _isOnline = false;
                }
                catch (NullReferenceException)
                {
                    lblAddedMessage.Text =
                        @"League of legends folder has not been set. Press the button on the bottom right and make sure your client is already running. If you did everything right, status should change to Online.";
                }

                var status = _isOnline ? "Online" : "Offline";
                lblClientStatus.Text = status;

                if (_isOnline)
                {
                    lblClientStatus.ForeColor = Color.Green;
                    if (lblAddedMessage.Text.Contains("League of legends folder has not been set"))
                    {
                        lblAddedMessage.Text = "";
                    }
                    IntListLoop();
                }
                else
                {
                    lblClientStatus.ForeColor = Color.Red;
                }

                await Task.Delay(2000);
            }
        }

        private static WebResponse GetEndpointResponse(string endPoint)
        {
            try
            {
                var url = "https://127.0.0.1:" + _port + endPoint;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential("riot", _lockfilePw);
                request.ServerCertificateValidationCallback = delegate { return true; };

                return request.GetResponse();
            }
            catch (WebException)
            {
                return null;
            }
        }

        private void AddToIntList(string name)
        {
            if (name[0] == '-')
            {
                DeleteFromIntList(name.Substring(1));
            }
            else
            {
                var summoner = _intListSummoners.FirstOrDefault(s =>
                    string.Equals(s, name, StringComparison.CurrentCultureIgnoreCase));
                if (summoner != null)
                {
                    lblAddedMessage.Text = $@"Summoner '{name}' could not be added to your int list because they are already in it.";
                }
                else
                {
                    _intListSummoners.Add(name);
                    var summonersList = new List<Summoner>();
                    foreach (var summonerName in _intListSummoners)
                    {
                        summonersList.Add(new Summoner { SummonerName = summonerName });
                    }

                    var serializer = new XmlSerializer(typeof(List<Summoner>));
                    using (TextWriter writer = new StreamWriter(_dbListPath))
                    {
                        serializer.Serialize(writer, summonersList);
                    }

                    lblAddedMessage.Text = $@"Summoner '{name}' has been added to your int list.";
                }
            }
        }

        private void DeleteFromIntList(string name)
        {
            var summoner = _intListSummoners.FirstOrDefault(s =>
                string.Equals(s, name, StringComparison.CurrentCultureIgnoreCase));
            if (summoner != null)
            {
                _intListSummoners.Remove(summoner);
                var serializer = new XmlSerializer(typeof(List<Summoner>));
                using (TextWriter writer = new StreamWriter(_dbListPath))
                {
                    serializer.Serialize(writer, _intListSummoners);
                }

                lblAddedMessage.Text = $@"Summoner '{name}' has been removed from your int list.";
            }
            else
            {
                lblAddedMessage.Text = $@"Summoner '{name}' doesn't exist on your int list.";
            }
        }

        private static bool IsInGame()
        {
            using (var response = GetEndpointResponse("/lol-gameflow/v1/session"))
            {
                if (response == null)
                {
                    return false;
                }

                using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    var jsonString = reader.ReadToEnd();
                    dynamic data = JObject.Parse(jsonString);

                    return data.phase == "InProgress";
                }
            }
        }

        private void txtbxSummonerName_Enter(object sender, EventArgs e)
        {
            if (txtbxSummonerName.Text != @"Summoner name") return;
            txtbxSummonerName.Text = "";
            txtbxSummonerName.ForeColor = Color.Black;
        }

        private void txtbxSummonerName_Leave(object sender, EventArgs e)
        {
            if (txtbxSummonerName.Text != "") return;
            txtbxSummonerName.Text = @"Summoner name";
            txtbxSummonerName.ForeColor = Color.Silver;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtbxSummonerName.Text == @"Summoner name" && txtbxSummonerName.ForeColor == Color.Silver)
            {
                lblAddedMessage.Text = @"Enter a summoner name.";
            }
            else
            {
                AddToIntList(txtbxSummonerName.Text);
                txtbxSummonerName.Text = @"Summoner name";
                txtbxSummonerName.ForeColor = Color.Silver;
            }
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop,
                Description = @" Select your league of legends folder",
                ShowNewFolderButton = false
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _lockFile.FilePath = $@"{fbd.SelectedPath}\lockfile";
                UpdateLeagueLocationFile(fbd.SelectedPath);
            }
        }

        private void pnlHeader_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _dragCursor = Cursor.Position;
            _dragForm = Location;
        }

        private void pnlHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;
            var dif = Point.Subtract(Cursor.Position, new Size(_dragCursor));
            Location = Point.Add(_dragForm, new Size(dif));
        }

        private void pnlHeader_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
    }
}
