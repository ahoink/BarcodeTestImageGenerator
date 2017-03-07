namespace BarcodeTestImageCreator
{
    partial class Settings
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
            this.rotateAngle = new System.Windows.Forms.TextBox();
            this.noiseLevel = new System.Windows.Forms.TextBox();
            this.skewAngle = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.blurLevel = new System.Windows.Forms.TextBox();
            this.contrastAmt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.saveBtn = new System.Windows.Forms.Button();
            this.resetBtn = new System.Windows.Forms.Button();
            this.rotRand = new System.Windows.Forms.CheckBox();
            this.skewRand = new System.Windows.Forms.CheckBox();
            this.noiseRand = new System.Windows.Forms.CheckBox();
            this.blurRand = new System.Windows.Forms.CheckBox();
            this.contRand = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Barcode Rotation";
            // 
            // rotateAngle
            // 
            this.rotateAngle.Location = new System.Drawing.Point(145, 27);
            this.rotateAngle.Name = "rotateAngle";
            this.rotateAngle.Size = new System.Drawing.Size(50, 20);
            this.rotateAngle.TabIndex = 1;
            // 
            // noiseLevel
            // 
            this.noiseLevel.Location = new System.Drawing.Point(145, 79);
            this.noiseLevel.Name = "noiseLevel";
            this.noiseLevel.Size = new System.Drawing.Size(50, 20);
            this.noiseLevel.TabIndex = 3;
            // 
            // skewAngle
            // 
            this.skewAngle.Location = new System.Drawing.Point(145, 53);
            this.skewAngle.Name = "skewAngle";
            this.skewAngle.Size = new System.Drawing.Size(50, 20);
            this.skewAngle.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Barcode Skew";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Image Noise";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(49, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Image Blur";
            // 
            // blurLevel
            // 
            this.blurLevel.Location = new System.Drawing.Point(145, 105);
            this.blurLevel.Name = "blurLevel";
            this.blurLevel.Size = new System.Drawing.Size(50, 20);
            this.blurLevel.TabIndex = 4;
            // 
            // contrastAmt
            // 
            this.contrastAmt.Location = new System.Drawing.Point(145, 131);
            this.contrastAmt.Name = "contrastAmt";
            this.contrastAmt.Size = new System.Drawing.Size(50, 20);
            this.contrastAmt.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(49, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Image Contrast";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(201, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Degrees";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(201, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Degrees (± 90)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(201, 82);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Std Dev";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(201, 108);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Level (0-5)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(201, 134);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Amount (± 255)";
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(76, 160);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.TabIndex = 15;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // resetBtn
            // 
            this.resetBtn.Location = new System.Drawing.Point(157, 160);
            this.resetBtn.Name = "resetBtn";
            this.resetBtn.Size = new System.Drawing.Size(75, 23);
            this.resetBtn.TabIndex = 16;
            this.resetBtn.Text = "Reset";
            this.resetBtn.UseVisualStyleBackColor = true;
            this.resetBtn.Click += new System.EventHandler(this.resetBtn_Click);
            // 
            // rotRand
            // 
            this.rotRand.AutoSize = true;
            this.rotRand.Location = new System.Drawing.Point(28, 30);
            this.rotRand.Name = "rotRand";
            this.rotRand.Size = new System.Drawing.Size(15, 14);
            this.rotRand.TabIndex = 17;
            this.rotRand.UseVisualStyleBackColor = true;
            this.rotRand.CheckedChanged += new System.EventHandler(this.rotRand_CheckedChanged);
            // 
            // skewRand
            // 
            this.skewRand.AutoSize = true;
            this.skewRand.Location = new System.Drawing.Point(28, 56);
            this.skewRand.Name = "skewRand";
            this.skewRand.Size = new System.Drawing.Size(15, 14);
            this.skewRand.TabIndex = 18;
            this.skewRand.UseVisualStyleBackColor = true;
            this.skewRand.CheckedChanged += new System.EventHandler(this.skewRand_CheckedChanged);
            // 
            // noiseRand
            // 
            this.noiseRand.AutoSize = true;
            this.noiseRand.Location = new System.Drawing.Point(28, 81);
            this.noiseRand.Name = "noiseRand";
            this.noiseRand.Size = new System.Drawing.Size(15, 14);
            this.noiseRand.TabIndex = 19;
            this.noiseRand.UseVisualStyleBackColor = true;
            this.noiseRand.CheckedChanged += new System.EventHandler(this.noiseRand_CheckedChanged);
            // 
            // blurRand
            // 
            this.blurRand.AutoSize = true;
            this.blurRand.Location = new System.Drawing.Point(28, 108);
            this.blurRand.Name = "blurRand";
            this.blurRand.Size = new System.Drawing.Size(15, 14);
            this.blurRand.TabIndex = 20;
            this.blurRand.UseVisualStyleBackColor = true;
            this.blurRand.CheckedChanged += new System.EventHandler(this.blurRand_CheckedChanged);
            // 
            // contRand
            // 
            this.contRand.AutoSize = true;
            this.contRand.Location = new System.Drawing.Point(28, 134);
            this.contRand.Name = "contRand";
            this.contRand.Size = new System.Drawing.Size(15, 14);
            this.contRand.TabIndex = 21;
            this.contRand.UseVisualStyleBackColor = true;
            this.contRand.CheckedChanged += new System.EventHandler(this.contRand_CheckedChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Random";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 211);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.contRand);
            this.Controls.Add(this.blurRand);
            this.Controls.Add(this.noiseRand);
            this.Controls.Add(this.skewRand);
            this.Controls.Add(this.rotRand);
            this.Controls.Add(this.resetBtn);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.contrastAmt);
            this.Controls.Add(this.blurLevel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.skewAngle);
            this.Controls.Add(this.noiseLevel);
            this.Controls.Add(this.rotateAngle);
            this.Controls.Add(this.label1);
            this.Name = "Settings";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox rotateAngle;
        private System.Windows.Forms.TextBox noiseLevel;
        private System.Windows.Forms.TextBox skewAngle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox blurLevel;
        private System.Windows.Forms.TextBox contrastAmt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button resetBtn;
        private System.Windows.Forms.CheckBox rotRand;
        private System.Windows.Forms.CheckBox skewRand;
        private System.Windows.Forms.CheckBox noiseRand;
        private System.Windows.Forms.CheckBox blurRand;
        private System.Windows.Forms.CheckBox contRand;
        private System.Windows.Forms.Label label11;
    }
}