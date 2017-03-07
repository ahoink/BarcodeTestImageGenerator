using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace BarcodeTestImageCreator
{
    public partial class Settings : Form
    {
        Label savedMsg;
        public Settings()
        {
            InitializeComponent();
            autofill();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            int rotation = 0;
            int skew = 0;
            int noise = 0;
            int blur = 0;
            int contrast = 0;
            string msg = "";

            if (!int.TryParse(rotateAngle.Text, out rotation))
                msg += "Rotation Angle must be an integer between -360 and 360\n";
            if (!int.TryParse(skewAngle.Text, out skew) || (Math.Abs(skew) > 90 && skew != 999))
                msg += "Skew Angle must be an integer between -90 and 90\n";
            if (!int.TryParse(noiseLevel.Text, out noise) || noise < 0)
                msg += "Noise standard deviation must be a positive integer\n";
            if (!int.TryParse(blurLevel.Text, out blur) || (blur > 5 && blur != 999) || blur < 0)
                msg += "Blur level must be an integer from 0 to 5\n";
            if (!int.TryParse(contrastAmt.Text, out contrast) || (Math.Abs(contrast) > 255 && contrast != 999))
                msg += "Contrast must be an integer between -256 and 256\n";

            if (msg != "")
            {
                if (savedMsg != null)
                {
                    savedMsg.Text = "";
                }
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);         
            }
            else
            {
                updateSettings(rotation, skew, noise, blur, contrast);

                this.savedMsg = new Label();
                this.savedMsg.AutoSize = true;
                this.savedMsg.Name = "savedMsg";
                this.savedMsg.Text = "Settings successfully saved!";
                this.savedMsg.Font = new Font(this.savedMsg.Font, FontStyle.Bold);
                this.savedMsg.Location = new Point(this.Width / 2 - this.savedMsg.Width + 10, saveBtn.Location.Y + 30);
                this.Controls.Add(this.savedMsg);               
                this.savedMsg.Update();

                System.Threading.Thread.Sleep(1000);
                this.Close();
            }

        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            if (File.Exists("settings.conf"))
            {
                File.Delete("settings.conf");
            }

            rotateAngle.Text = "0";
            skewAngle.Text = "0";
            noiseLevel.Text = "0";
            blurLevel.Text = "0";
            contrastAmt.Text = "0";
        }

        private void autofill()
        {
            if (File.Exists("settings.conf"))
            {
                string[] lines;
                using (StreamReader f = new StreamReader("settings.conf"))
                {
                    lines = f.ReadToEnd().Split('\n');
                }
                rotateAngle.Text = lines[0].Split('=')[1];
                skewAngle.Text = lines[1].Split('=')[1];
                noiseLevel.Text = lines[2].Split('=')[1];
                blurLevel.Text = lines[3].Split('=')[1];
                contrastAmt.Text = lines[4].Split('=')[1];
            }
            else
            {
                rotateAngle.Text = "0";
                skewAngle.Text = "0";
                noiseLevel.Text = "0";
                blurLevel.Text = "0";
                contrastAmt.Text = "0";
            }
        }

        private void updateSettings(int rotation, int skew, int noise, int blur, int contrast)
        {
            using (StreamWriter f = new StreamWriter("settings.conf"))
            {
                f.WriteLine("Rotation Angle=" + rotation);
                f.WriteLine("Skew Angle=" + skew);
                f.WriteLine("Noise Std Dev=" + noise);
                f.WriteLine("Blur Level=" + blur);
                f.WriteLine("Contrast=" + contrast);
            }
        }

        private void rotRand_CheckedChanged(object sender, EventArgs e)
        {
            if (rotRand.Checked)
            {
                rotateAngle.Text = "999";
                rotateAngle.Enabled = false;
            }
            else
            {
                rotateAngle.Text = "0";
                rotateAngle.Enabled = true;
            }
        }

        private void skewRand_CheckedChanged(object sender, EventArgs e)
        {
            if (skewRand.Checked)
            {
                skewAngle.Text = "999";
                skewAngle.Enabled = false;
            }
            else
            {
                skewAngle.Text = "0";
                skewAngle.Enabled = true;
            }
        }

        private void noiseRand_CheckedChanged(object sender, EventArgs e)
        {
            if (noiseRand.Checked)
            {
                noiseLevel.Text = "999";
                noiseLevel.Enabled = false;
            }
            else
            {
                noiseLevel.Text = "0";
                noiseLevel.Enabled = true;
            }
        }

        private void blurRand_CheckedChanged(object sender, EventArgs e)
        {
            if (blurRand.Checked)
            {
                blurLevel.Text = "999";
                blurLevel.Enabled = false;
            }
            else
            {
                blurLevel.Text = "0";
                blurLevel.Enabled = true;
            }
        }

        private void contRand_CheckedChanged(object sender, EventArgs e)
        {
            if (contRand.Checked)
            {
                contrastAmt.Text = "999";
                contrastAmt.Enabled = false;
            }
            else
            {
                contrastAmt.Text = "0";
                contrastAmt.Enabled = true;
            }
        }
    }
}
