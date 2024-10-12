using System.Net;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;

namespace IntList
{
    public partial class Form1 : Form
    {
        private readonly List<string> _intListSummoners = new();

        private readonly string _dbListPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IntList/DbList.xml");
        private readonly string _leagueLocationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IntList/leagueloc");
        private readonly string _intListFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IntList");
        private readonly LockFile _lockFile = new();

        private static int _port;
        private static string? _lockfilePw;

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
            if (!File.Exists(_leagueLocationPath))
            {
                return;
            }

            using var reader = new StreamReader(_leagueLocationPath);
            _lockFile.FilePath = $@"{reader.ReadToEnd()}\lockfile";
        }

        private void UpdateLeagueLocationFile(string path)
        {
            using var writer = new StreamWriter(_leagueLocationPath);
            writer.Write(path);
        }

        private void CreateDbListFile()
        {
            if (File.Exists(_dbListPath))
            {
                return;
            }

            Directory.CreateDirectory(_intListFolderPath);

            using var writer = new XmlTextWriter(_dbListPath, null);
            writer.Formatting = Formatting.Indented;

            var doc = new XmlDocument();
            doc.LoadXml("<ArrayOfSummoner></ArrayOfSummoner>");
            doc.Save(writer);
        }

        private void InitColors()
        {
            BackColor = Color.FromArgb(37, 37, 37);
            btnAdd.BackColor = Color.FromArgb(80, 80, 80);
            btnSelectFolder.BackColor = Color.FromArgb(80, 80, 80);
            pnlHeader.BackColor = Color.FromArgb(50, 50, 50);
        }

        private async Task IntListCheck()
        {
            try
            {
                if (await IsInGame())
                {
                    var gameSummonerNames = await GetSummonerNames();

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void PlayPopSound()
        {
            Stream str = Properties.Resources.pop;
            var snd = new System.Media.SoundPlayer(str);
            snd.Play();
        }

        private async static Task<IEnumerable<string>> GetSummonerNames()
        {
            var summonerIds = await GetSummonerIds();

            var summonerNames = new List<string>();
            foreach (var id in summonerIds)
            {
                using var response = await GetEndpointResponse($"/lol-summoner/v1/summoners/{id}");

                var jsonString = await response.Content.ReadAsStringAsync();
                dynamic data = JObject.Parse(jsonString);

                summonerNames.Add((string)data.gameName);
            }

            return summonerNames;
        }

        private async static Task<IEnumerable<long>> GetSummonerIds()
        {
            using var response = await GetEndpointResponse("/lol-gameflow/v1/session");
            var summonerIds = new List<long>();

            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(jsonString);

            foreach (var obj in data.gameData.teamOne)
            {
                summonerIds.Add((long)obj.summonerId);
            }
            foreach (var obj in data.gameData.teamTwo)
            {
                summonerIds.Add((long)obj.summonerId);
            }

            return summonerIds;
        }

        private async void OnlineCheckLoop()
        {
            while (true)
            {
                try
                {
                    using Stream s = new FileStream(_lockFile.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    var sr = new StreamReader(s);
                    var text = await sr.ReadToEndAsync();
                    var splitText = text.Split(':');
                    _ = int.TryParse(splitText[2], out _port);
                    _lockfilePw = splitText[3];
                    _isOnline = true;
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
                    await IntListCheck();
                }
                else
                {
                    lblClientStatus.ForeColor = Color.Red;
                }

                await Task.Delay(2000);
            }
        }

        private void UpdateIntList()
        {
            var summonersList = _intListSummoners
                .Select(summonerName => new Summoner { SummonerName = summonerName })
                .ToList();

            var serializer = new XmlSerializer(typeof(List<Summoner>));
            using var writer = new StreamWriter(_dbListPath);
            serializer.Serialize(writer, summonersList);

            txtbxSummonerName.Text = "";
        }

        private static async Task<HttpResponseMessage> GetEndpointResponse(string endPoint)
        {
            try
            {
                var url = $"https://127.0.0.1:{_port}{endPoint}";
                using var handler = new HttpClientHandler();
                handler.Credentials = new NetworkCredential("riot", _lockfilePw);
                handler.ServerCertificateCustomValidationCallback = delegate { return true; };

                using var httpClient = new HttpClient(handler);
                return await httpClient.GetAsync(url);
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }

        private void AddOrDeleteFromIntList(string name)
        {
            if (string.IsNullOrWhiteSpace(txtbxSummonerName.Text))
            {
                lblAddedMessage.Text = @"Enter a summoner name.";
                return;
            }

            if (name[0] == '-')
            {
                DeleteFromIntList(name[1..]);
                return;
            }

            var summoner = _intListSummoners.FirstOrDefault(s => string.Equals(s, name, StringComparison.CurrentCultureIgnoreCase));
            if (summoner != null)
            {
                lblAddedMessage.Text = $@"Summoner '{name}' could not be added to your int list because they are already in it.";
                return;
            }

            _intListSummoners.Add(name);
            UpdateIntList();

            lblAddedMessage.Text = $@"Summoner '{name}' has been added to your int list.";
        }

        private void DeleteFromIntList(string name)
        {
            var summoner = _intListSummoners.FirstOrDefault(s => string.Equals(s, name, StringComparison.CurrentCultureIgnoreCase));
            if (summoner != null)
            {
                _intListSummoners.Remove(summoner);
                UpdateIntList();

                lblAddedMessage.Text = $@"Summoner '{name}' has been removed from your int list.";
            }
            else
            {
                lblAddedMessage.Text = $@"Summoner '{name}' doesn't exist on your int list.";
            }
        }

        private static async Task<bool> IsInGame()
        {
            using var response = await GetEndpointResponse("/lol-gameflow/v1/session");
            if (response == null || response.Content == null)
            {
                return false;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(jsonString);

            return data.phase == "InProgress";
        }

        internal void BtnAdd_Click(object sender, EventArgs e)
        {
            AddOrDeleteFromIntList(txtbxSummonerName.Text);
        }

        internal void BtnSelectFolder_Click(object sender, EventArgs e)
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

        internal void PnlHeader_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _dragCursor = Cursor.Position;
            _dragForm = Location;
        }

        internal void PnlHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;
            var dif = Point.Subtract(Cursor.Position, new Size(_dragCursor));
            Location = Point.Add(_dragForm, new Size(dif));
        }

        internal void PnlHeader_MouseUp(object sender, MouseEventArgs e)
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

        internal void TxtbxSummonerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AddOrDeleteFromIntList(txtbxSummonerName.Text);
            }
        }
    }
}
