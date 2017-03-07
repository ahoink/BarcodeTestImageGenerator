namespace BarcodeTestImageCreator
{
    partial class MainForm
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
            this.folderSelect1 = new System.Windows.Forms.Button();
            this.folderSelect2 = new System.Windows.Forms.Button();
            this.folderPath1 = new System.Windows.Forms.TextBox();
            this.folderPath2 = new System.Windows.Forms.TextBox();
            this.modeSelect = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.otoRadio = new System.Windows.Forms.RadioButton();
            this.allPossRadio = new System.Windows.Forms.RadioButton();
            this.genBtn = new System.Windows.Forms.Button();
            this.randPlaceChkBox = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.settingsBtn = new System.Windows.Forms.Button();
            this.specRadio = new System.Windows.Forms.RadioButton();
            this.specAmt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.folderPath3 = new System.Windows.Forms.TextBox();
            this.folderSelect3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // folderSelect1
            // 
            this.folderSelect1.Location = new System.Drawing.Point(272, 83);
            this.folderSelect1.Name = "folderSelect1";
            this.folderSelect1.Size = new System.Drawing.Size(75, 23);
            this.folderSelect1.TabIndex = 5;
            this.folderSelect1.Text = "Browse";
            this.folderSelect1.UseVisualStyleBackColor = true;
            this.folderSelect1.Click += new System.EventHandler(this.folderSelect1_Click);
            // 
            // folderSelect2
            // 
            this.folderSelect2.Location = new System.Drawing.Point(272, 132);
            this.folderSelect2.Name = "folderSelect2";
            this.folderSelect2.Size = new System.Drawing.Size(75, 23);
            this.folderSelect2.TabIndex = 5;
            this.folderSelect2.Text = "Browse";
            this.folderSelect2.UseVisualStyleBackColor = true;
            this.folderSelect2.Click += new System.EventHandler(this.folderSelect2_Click);
            // 
            // folderPath1
            // 
            this.folderPath1.Location = new System.Drawing.Point(12, 85);
            this.folderPath1.Name = "folderPath1";
            this.folderPath1.Size = new System.Drawing.Size(254, 20);
            this.folderPath1.TabIndex = 1;
            // 
            // folderPath2
            // 
            this.folderPath2.Location = new System.Drawing.Point(12, 134);
            this.folderPath2.Name = "folderPath2";
            this.folderPath2.Size = new System.Drawing.Size(254, 20);
            this.folderPath2.TabIndex = 2;
            // 
            // modeSelect
            // 
            this.modeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeSelect.FormattingEnabled = true;
            this.modeSelect.Items.AddRange(new object[] {
            "Snippet",
            "Stitch"});
            this.modeSelect.Location = new System.Drawing.Point(140, 12);
            this.modeSelect.Name = "modeSelect";
            this.modeSelect.Size = new System.Drawing.Size(121, 21);
            this.modeSelect.TabIndex = 0;
            this.modeSelect.SelectedValueChanged += new System.EventHandler(this.modeSelect_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(97, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Mode:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Folder of barcode snippets";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Folder of real-world images:";
            // 
            // otoRadio
            // 
            this.otoRadio.AutoSize = true;
            this.otoRadio.Checked = true;
            this.otoRadio.Location = new System.Drawing.Point(21, 230);
            this.otoRadio.Name = "otoRadio";
            this.otoRadio.Size = new System.Drawing.Size(80, 17);
            this.otoRadio.TabIndex = 5;
            this.otoRadio.TabStop = true;
            this.otoRadio.Text = "One-to-One";
            this.otoRadio.UseVisualStyleBackColor = true;
            // 
            // allPossRadio
            // 
            this.allPossRadio.AutoSize = true;
            this.allPossRadio.Location = new System.Drawing.Point(21, 253);
            this.allPossRadio.Name = "allPossRadio";
            this.allPossRadio.Size = new System.Drawing.Size(144, 17);
            this.allPossRadio.TabIndex = 5;
            this.allPossRadio.TabStop = true;
            this.allPossRadio.Text = "All Possible Combinations";
            this.allPossRadio.UseVisualStyleBackColor = true;
            // 
            // genBtn
            // 
            this.genBtn.Location = new System.Drawing.Point(104, 305);
            this.genBtn.Name = "genBtn";
            this.genBtn.Size = new System.Drawing.Size(150, 30);
            this.genBtn.TabIndex = 3;
            this.genBtn.Text = "Generate";
            this.genBtn.UseVisualStyleBackColor = true;
            this.genBtn.Click += new System.EventHandler(this.genBtn_Click);
            // 
            // randPlaceChkBox
            // 
            this.randPlaceChkBox.AutoSize = true;
            this.randPlaceChkBox.Location = new System.Drawing.Point(175, 231);
            this.randPlaceChkBox.Name = "randPlaceChkBox";
            this.randPlaceChkBox.Size = new System.Drawing.Size(162, 17);
            this.randPlaceChkBox.TabIndex = 5;
            this.randPlaceChkBox.Text = "Random Barcode Placement";
            this.randPlaceChkBox.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 351);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(335, 23);
            this.progressBar1.TabIndex = 12;
            this.progressBar1.Visible = false;
            // 
            // settingsBtn
            // 
            this.settingsBtn.Location = new System.Drawing.Point(175, 254);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(162, 25);
            this.settingsBtn.TabIndex = 13;
            this.settingsBtn.Text = "Additional Settings";
            this.settingsBtn.UseVisualStyleBackColor = true;
            this.settingsBtn.Click += new System.EventHandler(this.settingsBtn_Click);
            // 
            // specRadio
            // 
            this.specRadio.AutoSize = true;
            this.specRadio.Location = new System.Drawing.Point(21, 276);
            this.specRadio.Name = "specRadio";
            this.specRadio.Size = new System.Drawing.Size(14, 13);
            this.specRadio.TabIndex = 14;
            this.specRadio.TabStop = true;
            this.specRadio.UseVisualStyleBackColor = true;
            // 
            // specAmt
            // 
            this.specAmt.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.specAmt.Location = new System.Drawing.Point(41, 273);
            this.specAmt.Name = "specAmt";
            this.specAmt.Size = new System.Drawing.Size(48, 20);
            this.specAmt.TabIndex = 15;
            this.specAmt.Text = "Specify";
            this.specAmt.MouseClick += new System.Windows.Forms.MouseEventHandler(this.specAmt_MouseClick);
            this.specAmt.TextChanged += new System.EventHandler(this.specAmt_TextChanged);
            this.specAmt.Leave += new System.EventHandler(this.specAmt_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 166);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Output folder (optional):";
            // 
            // folderPath3
            // 
            this.folderPath3.Location = new System.Drawing.Point(12, 182);
            this.folderPath3.Name = "folderPath3";
            this.folderPath3.Size = new System.Drawing.Size(254, 20);
            this.folderPath3.TabIndex = 17;
            // 
            // folderSelect3
            // 
            this.folderSelect3.Location = new System.Drawing.Point(272, 180);
            this.folderSelect3.Name = "folderSelect3";
            this.folderSelect3.Size = new System.Drawing.Size(75, 23);
            this.folderSelect3.TabIndex = 18;
            this.folderSelect3.Text = "Browse";
            this.folderSelect3.UseVisualStyleBackColor = true;
            this.folderSelect3.Click += new System.EventHandler(this.folderSelect3_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 386);
            this.Controls.Add(this.folderSelect3);
            this.Controls.Add(this.folderPath3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.specAmt);
            this.Controls.Add(this.specRadio);
            this.Controls.Add(this.settingsBtn);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.randPlaceChkBox);
            this.Controls.Add(this.genBtn);
            this.Controls.Add(this.allPossRadio);
            this.Controls.Add(this.otoRadio);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.modeSelect);
            this.Controls.Add(this.folderPath2);
            this.Controls.Add(this.folderPath1);
            this.Controls.Add(this.folderSelect2);
            this.Controls.Add(this.folderSelect1);
            this.Name = "MainForm";
            this.Text = "Barcode Test Image Creator 1.0";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button folderSelect1;
        private System.Windows.Forms.Button folderSelect2;
        private System.Windows.Forms.TextBox folderPath1;
        private System.Windows.Forms.TextBox folderPath2;
        private System.Windows.Forms.ComboBox modeSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton otoRadio;
        private System.Windows.Forms.RadioButton allPossRadio;
        private System.Windows.Forms.Button genBtn;
        private System.Windows.Forms.CheckBox randPlaceChkBox;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button settingsBtn;
        private System.Windows.Forms.RadioButton specRadio;
        private System.Windows.Forms.TextBox specAmt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox folderPath3;
        private System.Windows.Forms.Button folderSelect3;
    }
}

