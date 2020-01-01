using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOL_int_list_GUI_v2
{
    public partial class Form1 : Form
    {

        private static int Port;
        private static string Password;
        private const string LockfilePath = @"C:\Riot Games\League of Legends\lockfile";

        private static bool IsOnline = false;

        public Form1()
        {
            InitializeComponent();
            InitColors();
            ClientLoop();
        }

        public async void ClientLoop()
        {
            while (true)
            {
                try
                {
                    using (Stream s = new FileStream(LockfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        StreamReader sr = new StreamReader(s);
                        var text = sr.ReadToEnd();
                        string[] splittedText = text.Split(':');
                        Int32.TryParse(splittedText[2], out Port);
                        Password = splittedText[3];
                        IsOnline = true;
                    }
                }
                catch (FileNotFoundException e)
                {
                    //Console.WriteLine("The lockfile could not be found. Either league isn't opened or the directory could not be found\n");
                    //Console.WriteLine(e);
                    IsOnline = false;
                }
                string status = IsOnline ? "Online" : "Offline";
                lblClientStatus.Text = status;

                if (IsOnline)
                    lblClientStatus.ForeColor = Color.Green;
                else
                    lblClientStatus.ForeColor = Color.Red;

                if (IsOnline && IsInChampSelect())
                {
                    using (var context = new SummonerContext())
                    {
                        var summoners = GetLobbyMembers();
                        foreach (var s in summoners)
                        {
                            bool isInList = context.Summoners.Any(summoner => summoner.summonerName.ToLower() == s.summonerName.ToLower());
                            if (isInList)
                                Console.WriteLine($"{s.summonerName} is on your int list.");
                        }
                    }
                }

                await Task.Delay(100);
            }
        }

        private WebResponse GetEndpointResponse(string endPoint)
        {
            string url = "https://127.0.0.1:" + Port + endPoint;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Credentials = new NetworkCredential("riot", Password);
            request.ServerCertificateValidationCallback = delegate { return true; };

            return request.GetResponse();
        }

        private void AddToIntList(string name)
        {
            using (var context = new SummonerContext())
            {
                try
                {
                    context.Add(new Summoner { summonerName = name });
                    context.SaveChanges();
                    lblAddedMessage.Text = $"Summoner {name} has been added to your int list.";
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e) when (e.InnerException is Microsoft.Data.SqlClient.SqlException)
                {
                    lblAddedMessage.Text = $"Summoner {name} could not be added to your int list because they are already in it.";
                }
            }
        }

        private List<Summoner> GetLobbyMembers()
        {
            using (WebResponse response = GetEndpointResponse("/lol-lobby/v2/lobby/members"))
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string jsonString = reader.ReadToEnd();
                    List<Summoner> summonerList = JsonConvert.DeserializeObject<List<Summoner>>(jsonString);

                    return summonerList;
                }
            }
        }

        private bool IsInChampSelect()
        {
            using (WebResponse response = GetEndpointResponse("/lol-chat/v1/conversations"))
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


        private void InitColors()
        {
            this.BackColor = Color.FromArgb(37, 37, 37);
            btnAdd.BackColor = Color.FromArgb(80, 80, 80);
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
            AddToIntList(txtbxSummonerName.Text);
        }
    }
}
