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

namespace LOL_int_list_GUI_v2
{
    public partial class Form1 : Form
    {

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
            OnlineCheckLoop();
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

                using (var context = new IntListContext())
                {

                    List<string> summoners = new List<string>();
                    foreach (var id in GetSummonerIds())
                    {
                        summoners.Add(GetSummonerName(id));
                    }
                    foreach (var name in summoners)
                    {
                        bool isInList = context.Summoners.Any(summoner => summoner.summonerName.ToLower() == name.ToLower());
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
            }
            else
            {
                lblIntList.Text = "";
                lblIntListText.Text = "Currently not in champ select";

            }
        }

        private void PlayPopSound()
        {
            Stream str = Properties.Resources.pop1;
            System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
            snd.Play();
        }

        private List<int> GetSummonerIds()
        {
            using (WebResponse response = GetEndpointResponse("/lol-champ-select-legacy/v1/session"))
            {
                List<int> summonerIds = new List<int>();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string jsonString = reader.ReadToEnd();
                    dynamic data = JObject.Parse(jsonString);
                    foreach (var obj in data.myTeam)
                    {
                        summonerIds.Add((int)obj.summonerId);
                    }
                    //summonerIds = JsonConvert.DeserializeObject<List<SummonerId>>(jsonString);

                    return summonerIds;
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

                    //summonerIds = JsonConvert.DeserializeObject<List<SummonerId>>(jsonString);

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
                    using (var context = new IntListContext())
                    {
                        using (Stream s = new FileStream(context.LockFileLocation.FirstOrDefault().FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            StreamReader sr = new StreamReader(s);
                            var text = sr.ReadToEnd();
                            string[] splittedText = text.Split(':');
                            Int32.TryParse(splittedText[2], out Port);
                            Password = splittedText[3];
                            IsOnline = true;
                        }
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
                DeleteFromIntList(name.Substring(1));
            else
            {
                using (var context = new IntListContext())
                {
                    try
                    {
                        context.Add(new Summoner { summonerName = name });
                        context.SaveChanges();
                        lblAddedMessage.Text = $"Summoner '{name}' has been added to your int list.";
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException e) when (e.InnerException is Microsoft.Data.SqlClient.SqlException)
                    {
                        lblAddedMessage.Text = $"Summoner '{name}' could not be added to your int list because they are already in it.";
                    }
                }
            }
        }

        private void DeleteFromIntList(string name)
        {
            using (var context = new IntListContext())
            {
                Summoner summoner = context.Summoners.Where(s => s.summonerName.ToLower() == name.ToLower()).FirstOrDefault();
                if (summoner != null)
                {
                    context.Remove(summoner);
                    context.SaveChanges();
                }
                lblAddedMessage.Text = $"Summoner '{name}' has been removed from your int list.";
            }
        }

        private List<Summoner> GetLobbyMembers()
        {
            using (WebResponse response = GetEndpointResponse("/lol-lobby/v2/lobby/members"))
            {
                List<Summoner> summonerList = new List<Summoner>();
                if (response == null)
                {
                    return summonerList;
                }
                else
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonString = reader.ReadToEnd();
                        summonerList = JsonConvert.DeserializeObject<List<Summoner>>(jsonString);

                        return summonerList;
                    }
                }
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



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

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
            using (var context = new IntListContext())
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.RootFolder = Environment.SpecialFolder.Desktop;
                fbd.Description = " Select your league of legends folder";
                fbd.ShowNewFolderButton = false;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    if (context.LockFileLocation.Any())
                        context.LockFileLocation.Remove(context.LockFileLocation.FirstOrDefault());
                    context.LockFileLocation.Add(new LockFile { FilePath = $@"{fbd.SelectedPath}\lockfile" });
                    context.SaveChanges();
                }
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
