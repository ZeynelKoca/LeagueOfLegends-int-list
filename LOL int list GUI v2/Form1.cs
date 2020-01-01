using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOL_int_list_GUI_v2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void SetColors()
        {
            this.BackColor = Color.FromArgb(37, 37, 37);
            btnAdd.BackColor = Color.FromArgb(80, 80, 80);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtbxSummonerName_Enter(object sender, EventArgs e)
        {
            if(txtbxSummonerName.Text == "Summoner name")
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

        }
    }
}
