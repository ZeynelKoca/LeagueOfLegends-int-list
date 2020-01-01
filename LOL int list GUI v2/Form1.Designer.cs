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
            this.label1 = new System.Windows.Forms.Label();
            this.lblClientStatus = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtbxSummonerName = new System.Windows.Forms.TextBox();
            this.lblAddedMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Nirmala UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(3, 483);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "League client status:";
            // 
            // lblClientStatus
            // 
            this.lblClientStatus.AutoSize = true;
            this.lblClientStatus.Font = new System.Drawing.Font("Nirmala UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientStatus.ForeColor = System.Drawing.SystemColors.Control;
            this.lblClientStatus.Location = new System.Drawing.Point(144, 483);
            this.lblClientStatus.Name = "lblClientStatus";
            this.lblClientStatus.Size = new System.Drawing.Size(49, 19);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(408, 506);
            this.Controls.Add(this.lblAddedMessage);
            this.Controls.Add(this.txtbxSummonerName);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblClientStatus);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblClientStatus;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtbxSummonerName;
        private System.Windows.Forms.Label lblAddedMessage;
    }
}

