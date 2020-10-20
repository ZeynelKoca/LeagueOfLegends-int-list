using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

namespace LoL_int_list
{
    public partial class Form1 : Form
    {
        private List<Summoner> _intListSummoners = new List<Summoner>();

        private string _dbListPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DbList.xml");
        private LockFile _lockFile;

        private static int Port;
        private static string Password;

        private static bool IsOnline = false;

        private bool dragging = false;
        private Point dragCursor;
        private Point dragForm;


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

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(_dbListPath);

            foreach (XmlNode node in xmlDocument)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    string name = childNode.InnerText;
                    _intListSummoners.Add(new Summoner { SummonerName = name });
                }
            }
        }

        private void CreateDbListFile()
        {
            if (!File.Exists(_dbListPath))
            {
                using (var writer = new XmlTextWriter(_dbListPath, null))
                {
                    writer.Formatting = System.Xml.Formatting.Indented;

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml("<ArrayOfSummoner></ArrayOfSummoner>");
                    doc.Save(writer);
                }
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

        public void IntListLoop()
        {
            if (IsInChampSelect())
            {
                List<string> lobbySummonerNames = new List<string>();
                foreach (var id in GetSummonerIds())
                {
                    lobbySummonerNames.Add(GetSummonerName(id));
                }

                foreach (var name in lobbySummonerNames)
                {
                    bool isInList = _intListSummoners.Any(summoner => summoner.SummonerName.ToLower() == name.ToLower());
                    if (isInList)
                    {
                        lblIntListText.Text = "The following people are on your int list";
                        if (!lblIntList.Text.Contains(name))
                        {
                            PlayPopSound();
                            lblIntList.Text += $"- {name}\r\n";
                            WindowState = FormWindowState.Normal;
                            Activate();
                        }
                    }
                    else
                    {
                        lblIntListText.Text = "No summoners found on your int list. GLHF";
                    }
                }
            }
            else
            {
                lblIntList.Text = "";
                lblIntListText.Text = "Currently not in champ select";
            }
        }

        private void PlayPopSound()
        {
            Stream str = Siskos_LOL_int_list.Properties.Resources.pop1;
            System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
            snd.Play();
        }

        private List<int> GetSummonerIds()
        {
            using (WebResponse response = GetEndpointResponse("/lol-champ-select/v1/session"))
            {
                List<int> summonerIds = new List<int>();
                if (response == null)
                {
                    return summonerIds;
                }
                else
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonString = reader.ReadToEnd();
                        dynamic data = JObject.Parse(jsonString);
                        foreach (var obj in data.myTeam)
                        {
                            summonerIds.Add((int)obj.summonerId);
                        }

                        return summonerIds;
                    }
                }
            }
        }

        private string GetSummonerName(int summonerId)
        {
            using (WebResponse response = GetEndpointResponse("/lol-summoner/v1/summoners/" + summonerId))
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string jsonString = reader.ReadToEnd();
                    dynamic data = JObject.Parse(jsonString);

                    return (string)data.displayName;
                }
            }

        }

        public async void OnlineCheckLoop()
        {
            while (true)
            {
                try
                {
                    using (Stream s = new FileStream(_lockFile.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        StreamReader sr = new StreamReader(s);
                        var text = sr.ReadToEnd();
                        string[] splittedText = text.Split(':');
                        int.TryParse(splittedText[2], out Port);
                        Password = splittedText[3];
                        IsOnline = true;
                    }
                }
                catch (FileNotFoundException)
                {
                    IsOnline = false;
                }
                catch (NullReferenceException)
                {
                    lblAddedMessage.Text = "League of legends folder has not been set. Press the button on the bottom right and make sure your client is already running. If you did everything right, status should change to Online.";
                }

                string status = IsOnline ? "Online" : "Offline";
                lblClientStatus.Text = status;

                if (IsOnline)
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

        private WebResponse GetEndpointResponse(string endPoint)
        {
            try
            {
                string url = "https://127.0.0.1:" + Port + endPoint;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential("riot", Password);
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
                var summoner = _intListSummoners.Where(s => s.SummonerName.ToLower() == name.ToLower()).FirstOrDefault();
                if (summoner != null)
                {
                    lblAddedMessage.Text = $"Summoner '{name}' could not be added to your int list because they are already in it.";
                }
                else
                {
                    _intListSummoners.Add(new Summoner { SummonerName = name });
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Summoner>));
                    using (TextWriter writer = new StreamWriter(_dbListPath))
                    {
                        serializer.Serialize(writer, _intListSummoners);
                    }

                    lblAddedMessage.Text = $"Summoner '{name}' has been added to your int list.";
                }
            }
        }

        private void DeleteFromIntList(string name)
        {
            Summoner summoner = _intListSummoners.Where(s => s.SummonerName.ToLower() == name.ToLower()).FirstOrDefault();
            if (summoner != null)
            {
                _intListSummoners.Remove(summoner);
                XmlSerializer serializer = new XmlSerializer(typeof(List<Summoner>));
                using (TextWriter writer = new StreamWriter(_dbListPath))
                {
                    serializer.Serialize(writer, _intListSummoners);
                }

                lblAddedMessage.Text = $"Summoner '{name}' has been removed from your int list.";
            }
            else
            {
                lblAddedMessage.Text = $"Summoner '{name}' doesn't exist on your int list.";
            }
        }

        private bool IsInChampSelect()
        {
            using (WebResponse response = GetEndpointResponse("/lol-chat/v1/conversations"))
            {
                if (response == null)
                {
                    return false;
                }
                else
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonString = reader.ReadToEnd();
                        List<Conversations> conversations = JsonConvert.DeserializeObject<List<Conversations>>(jsonString);

                        foreach (var c in conversations)
                        {
                            if (c.type == "championSelect")
                                return true;
                        }
                        return false;
                    }
                }
            }
        }

        private void txtbxSummonerName_Enter(object sender, EventArgs e)
        {
            if (txtbxSummonerName.Text == "Summoner name")
            {
                txtbxSummonerName.Text = "";
                txtbxSummonerName.ForeColor = Color.Black;
            }
        }

        private void txtbxSummonerName_Leave(object sender, EventArgs e)
        {
            if (txtbxSummonerName.Text == "")
            {
                txtbxSummonerName.Text = "Summoner name";
                txtbxSummonerName.ForeColor = Color.Silver;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtbxSummonerName.Text == "Summoner name" && txtbxSummonerName.ForeColor == Color.Silver)
            {
                lblAddedMessage.Text = "Enter a summoner name.";
            }
            else
            {
                AddToIntList(txtbxSummonerName.Text);
                txtbxSummonerName.Text = "Summoner name";
                txtbxSummonerName.ForeColor = Color.Silver;
            }
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop,
                Description = " Select your league of legends folder",
                ShowNewFolderButton = false
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _lockFile = new LockFile { FilePath = $@"{fbd.SelectedPath}\lockfile" };
            }
        }

        private void pnlHeader_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursor = Cursor.Position;
            dragForm = Location;
        }

        private void pnlHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursor));
                Location = Point.Add(dragForm, new Size(dif));
            }
        }

        private void pnlHeader_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
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
