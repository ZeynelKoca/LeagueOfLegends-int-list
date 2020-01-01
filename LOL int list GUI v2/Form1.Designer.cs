namespace LOL_int_list_GUI_v2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.lblClientStatus = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtbxSummonerName = new System.Windows.Forms.TextBox();
            this.lblAddedMessage = new System.Windows.Forms.Label();
            this.lblIntList = new System.Windows.Forms.Label();
            this.lblIntListText = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Javanese Text", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(0, 479);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "League client status:";
            // 
            // lblClientStatus
            // 
            this.lblClientStatus.AutoSize = true;
            this.lblClientStatus.Font = new System.Drawing.Font("Nirmala UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientStatus.ForeColor = System.Drawing.SystemColors.Control;
            this.lblClientStatus.Location = new System.Drawing.Point(158, 484);
            this.lblClientStatus.Name = "lblClientStatus";
            this.lblClientStatus.Size = new System.Drawing.Size(46, 17);
            this.lblClientStatus.TabIndex = 0;
            this.lblClientStatus.Text = "Status";
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.DarkGray;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Microsoft New Tai Lue", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Location = new System.Drawing.Point(305, 34);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(55, 27);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtbxSummonerName
            // 
            this.txtbxSummonerName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtbxSummonerName.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtbxSummonerName.Location = new System.Drawing.Point(46, 34);
            this.txtbxSummonerName.Name = "txtbxSummonerName";
            this.txtbxSummonerName.Size = new System.Drawing.Size(253, 27);
            this.txtbxSummonerName.TabIndex = 2;
            this.txtbxSummonerName.Text = "Summoner name";
            this.txtbxSummonerName.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.txtbxSummonerName.Enter += new System.EventHandler(this.txtbxSummonerName_Enter);
            this.txtbxSummonerName.Leave += new System.EventHandler(this.txtbxSummonerName_Leave);
            // 
            // lblAddedMessage
            // 
            this.lblAddedMessage.AutoSize = true;
            this.lblAddedMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAddedMessage.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblAddedMessage.Location = new System.Drawing.Point(46, 66);
            this.lblAddedMessage.MaximumSize = new System.Drawing.Size(310, 0);
            this.lblAddedMessage.Name = "lblAddedMessage";
            this.lblAddedMessage.Size = new System.Drawing.Size(0, 15);
            this.lblAddedMessage.TabIndex = 3;
            // 
            // lblIntList
            // 
            this.lblIntList.AutoSize = true;
            this.lblIntList.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIntList.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblIntList.Location = new System.Drawing.Point(56, 208);
            this.lblIntList.Name = "lblIntList";
            this.lblIntList.Size = new System.Drawing.Size(0, 18);
            this.lblIntList.TabIndex = 4;
            // 
            // lblIntListText
            // 
            this.lblIntListText.AutoSize = true;
            this.lblIntListText.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIntListText.ForeColor = System.Drawing.SystemColors.Control;
            this.lblIntListText.Location = new System.Drawing.Point(43, 171);
            this.lblIntListText.Name = "lblIntListText";
            this.lblIntListText.Size = new System.Drawing.Size(292, 18);
            this.lblIntListText.TabIndex = 4;
            this.lblIntListText.Text = "The following people are on your int list";
            this.lblIntListText.Click += new System.EventHandler(this.lblIntListText_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(46, 197);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(310, 1);
            this.panel1.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Location = new System.Drawing.Point(45, 197);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1, 263);
            this.panel3.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(37, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(334, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Type \' - \' in front of the name to           them from your list";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.Location = new System.Drawing.Point(221, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "delete";
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.BackColor = System.Drawing.Color.DarkGray;
            this.btnSelectFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectFolder.Font = new System.Drawing.Font("Microsoft New Tai Lue", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectFolder.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSelectFolder.Location = new System.Drawing.Point(372, 477);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(31, 24);
            this.btnSelectFolder.TabIndex = 1;
            this.btnSelectFolder.Text = "...";
            this.btnSelectFolder.UseVisualStyleBackColor = false;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(408, 506);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblIntListText);
            this.Controls.Add(this.lblIntList);
            this.Controls.Add(this.lblAddedMessage);
            this.Controls.Add(this.txtbxSummonerName);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblClientStatus);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "My LOL int list";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblClientStatus;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtbxSummonerName;
        private System.Windows.Forms.Label lblAddedMessage;
        private System.Windows.Forms.Label lblIntList;
        private System.Windows.Forms.Label lblIntListText;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSelectFolder;
    }
}

