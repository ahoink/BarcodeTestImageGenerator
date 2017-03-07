using System;
using System.Drawing;
using System.Windows.Forms;

namespace BarcodeTestImageCreator
{
    public partial class MainForm : Form
    {
        int mode;   // snippet = 1
                    // stitch  = 2

        public MainForm()
        {
            InitializeComponent();
        }

        private void folderSelect1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog selectFolder = new FolderBrowserDialog();
            selectFolder.Description = "Select a folder containing the input images";
            selectFolder.ShowDialog();
            
            folderPath1.Text = selectFolder.SelectedPath;
        }

        private void folderSelect2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog selectFolder = new FolderBrowserDialog();
            selectFolder.Description = "Select a folder containing the real world images";
            selectFolder.ShowDialog();

            folderPath2.Text = selectFolder.SelectedPath;
        }

        private void folderSelect3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog selectFolder = new FolderBrowserDialog();
            selectFolder.Description = "Select a folder to save the resulting images";
            selectFolder.ShowDialog();

            folderPath3.Text = selectFolder.SelectedPath;
        }

        private void modeSelect_SelectedValueChanged(object sender, EventArgs e)
        {
            if (modeSelect.Text == "Snippet")
            {
                mode = 1;
                label2.Text = "Folder of images to crop:";

                // Disabled all unneeded items
                label3.ForeColor = Color.Gray;
                folderPath2.Enabled = false;
                folderSelect2.Enabled = false;
                otoRadio.Enabled = false;
                allPossRadio.Enabled = false;
                specRadio.Enabled = false;
                specAmt.Enabled = false;
                randPlaceChkBox.Enabled = false;
                settingsBtn.Enabled = false;

            }
            else if (modeSelect.Text == "Stitch")
            {
                mode = 2;
                label2.Text = "Folder of barcode snippets:";

                // Reenable items that might've been disabled
                label3.ForeColor = Color.Black;
                folderPath2.Enabled = true;
                folderSelect2.Enabled = true;
                otoRadio.Enabled = true;
                allPossRadio.Enabled = true;
                specRadio.Enabled = true;
                specAmt.Enabled = true;
                randPlaceChkBox.Enabled = true;
                settingsBtn.Enabled = true;
            }

        }

        private void specAmt_TextChanged(object sender, EventArgs e)
        {
            // Placeholder
        }

        private void specAmt_MouseClick(object sender, MouseEventArgs e)
        {
            specAmt.Text = "";
            specAmt.ForeColor = Color.Black;
            specRadio.Checked = true;
        }

        private void specAmt_Leave(object sender, EventArgs e)
        {
            string text = specAmt.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                specAmt.Text = "Specify";
                specAmt.ForeColor = Color.LightGray;
                otoRadio.Checked = true;
            }
            else
            {
                int temp;
                if (int.TryParse(text, out temp))
                {
                    specAmt.ForeColor = Color.Black;
                }
                else
                {
                    MessageBox.Show("You must enter a positive integer.", "Invalid Entry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    specAmt.Text = "Specify";
                    specAmt.ForeColor = Color.LightGray;
                    otoRadio.Checked = true;
                }
            }
        }

        private void settingsBtn_Click(object sender, EventArgs e)
        {
            Settings settingsForm = new Settings();
            settingsForm.Show();
        }

        private void genBtn_Click(object sender, EventArgs e)
        {
            // Check if path1 or mode are empty. (path2 will be checked later)
            if (string.IsNullOrWhiteSpace(folderPath1.Text) ||
                string.IsNullOrWhiteSpace(modeSelect.Text))
            {
                MessageBox.Show("Not all entries have been completed!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            ImgProc processImages = new ImgProc();
            bool res = false;                                       // Result of task. True upon successful completion
            string output = "";
            progressBar1.Visible = true;
            processImages.Progress += updateProgress;               // MainForm subscribes to delegate in ImgProc

            if (this.mode == 1)                                     // Snippet mode
            {
                output = !string.IsNullOrWhiteSpace(folderPath3.Text) ? folderPath3.Text + "\\BarcodeSnippets\\" : folderPath1.Text + "\\BarcodeSnippets\\";
                res = processImages.snippet(folderPath1.Text + "\\", output);               
            }
            else if (this.mode == 2)                                // Stitch mode
            {
                if (string.IsNullOrWhiteSpace(folderPath2.Text))    // Only check path2 if stitch mode is selected
                {
                    MessageBox.Show("Not all entries have been completed!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int stitchMode = 1;
                if (otoRadio.Checked)
                    stitchMode = 1;
                else if (allPossRadio.Checked)
                    stitchMode = 2;
                else if (specRadio.Checked)
                    stitchMode = int.Parse(specAmt.Text);

                string[] insplit = folderPath1.Text.Split('\\');
                string[] outsplit = folderPath2.Text.Split('\\');
                if (!string.IsNullOrWhiteSpace(folderPath3.Text))
                {
                    output = folderPath3.Text + "\\" + outsplit[outsplit.Length - 1] + "_stitches\\";
                }
                else
                {
                    output = folderPath1.Text.Replace(insplit[insplit.Length - 1], outsplit[outsplit.Length - 1] + "_stitches\\");
                }

                res = processImages.stitch(folderPath1.Text + "\\", folderPath2.Text + "\\", output, stitchMode, randPlaceChkBox.Checked);
            }

            if (res)                                                // Display completion message upon completion. (displays even if there are errors during processing)
            {
                MessageBox.Show("Process Complete! New images are located at:\n" + output, "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                progressBar1.Value = 0;
            }

            progressBar1.Visible = false;

        }

        private void updateProgress(int value)
        {
            progressBar1.Value = value;
        }

        // ---------- Static Methods ---------- //

        // Display error message if a folder already exists when trying to create a new folder. User must confirm they want to delete the folder.
        public static bool dispDirExists(string path)
        {
            DialogResult ans = MessageBox.Show("A folder already exists at\n" + path + "\nDo you want to delete it and continue?", "Warning",
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (ans == DialogResult.Yes)
            {
                DialogResult ansDoubleCheck = MessageBox.Show("Are you sure you want to delete\n" + path + "\nand all of its contents?", "Warning",
                                                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                return ansDoubleCheck == DialogResult.Yes;
            }
            else return false;
        }

        // Display messagebox with an Error icon and a specifed message and title
        public static void dispError(string msg, string title)
        {
            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        
    }
}
