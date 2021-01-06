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
        private readonly List<Summoner> _intListSummoners = new List<Summoner>();

        private readonly string _dbListPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DbList.xml");
        private LockFile _lockFile;

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
                    _intListSummoners.Add(new Summoner { SummonerName = name });
                }
            }
        }

        private void CreateDbListFile()
        {
            if (File.Exists(_dbListPath)) return;
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
            if (IsInChampSelect())
            {
                var lobbySummonerNames = GetSummonerIds().Select(GetSummonerName).ToList();

                foreach (var name in lobbySummonerNames)
                {
                    var isInList = _intListSummoners.Any(summoner =>
                        string.Equals(summoner.SummonerName, name, StringComparison.CurrentCultureIgnoreCase));
                    if (isInList)
                    {
                        lblIntListText.Text = @"The following people are on your int list";
                        if (lblIntList.Text.Contains(name)) continue;
                        PlayPopSound();
                        lblIntList.Text += $"- {name}\r\n";
                        WindowState = FormWindowState.Normal;
                        Activate();
                    }
                    else
                    {
                        lblIntListText.Text = @"No summoners found on your int list. GLHF";
                    }
                }
            }
            else
            {
                lblIntList.Text = "";
                lblIntListText.Text = @"Currently not in champ select";
            }
        }

        private static void PlayPopSound()
        {
            Stream str = Properties.Resources.pop1;
            var snd = new System.Media.SoundPlayer(str);
            snd.Play();
        }

        private IEnumerable<long> GetSummonerIds()
        {
            using (var response = GetEndpointResponse("/lol-champ-select/v1/session"))
            {
                var summonerIds = new List<long>();
                if (response == null)
                {
                    return summonerIds;
                }

                using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    var jsonString = reader.ReadToEnd();
                    dynamic data = JObject.Parse(jsonString);
                    foreach (var obj in data.myTeam)
                    {
                        summonerIds.Add((long)obj.summonerId);
                    }

                    return summonerIds;
                }
            }
        }

        private string GetSummonerName(long summonerId)
        {
            using (var response = GetEndpointResponse("/lol-summoner/v1/summoners/" + summonerId))
            {
                using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    var jsonString = reader.ReadToEnd();
                    dynamic data = JObject.Parse(jsonString);

                    return (string)data.displayName;
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
                    string.Equals(s.SummonerName, name, StringComparison.CurrentCultureIgnoreCase));
                if (summoner != null)
                {
                    lblAddedMessage.Text = $@"Summoner '{name}' could not be added to your int list because they are already in it.";
                }
                else
                {
                    _intListSummoners.Add(new Summoner { SummonerName = name });
                    var serializer = new XmlSerializer(typeof(List<Summoner>));
                    using (TextWriter writer = new StreamWriter(_dbListPath))
                    {
                        serializer.Serialize(writer, _intListSummoners);
                    }

                    lblAddedMessage.Text = $@"Summoner '{name}' has been added to your int list.";
                }
            }
        }

        private void DeleteFromIntList(string name)
        {
            var summoner = _intListSummoners.FirstOrDefault(s =>
                string.Equals(s.SummonerName, name, StringComparison.CurrentCultureIgnoreCase));
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

        private static bool IsInChampSelect()
        {
            using (var response = GetEndpointResponse("/lol-chat/v1/conversations"))
            {
                if (response == null)
                {
                    return false;
                }
                else
                {
                    using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                    {
                        var jsonString = reader.ReadToEnd();
                        var conversations = JsonConvert.DeserializeObject<List<Conversations>>(jsonString);

                        return conversations.Any(c => c.Type == "championSelect");
                    }
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
                _lockFile = new LockFile { FilePath = $@"{fbd.SelectedPath}\lockfile" };
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
