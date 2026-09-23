using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace SymbolDB
{
    public partial class DialogSelectTargetFolder : Form
    {
        private TextBox tbTargetFolder;
        private Label lblPrompt;
        private Button btOK;
        private Button btCancel;
        private Button btOpenInExplorer;
        private FlowLayoutPanel panelButtons;
        private TextBox tbFullPath;
        private string baseTargetFolder;
        
        public string SelectedFolder { get; private set; }

        public DialogSelectTargetFolder(string targetFolderBase = "")
        {
            baseTargetFolder = targetFolderBase;
            InitializeComponent();
            SetupControls();
            CreateButtons();
        }

        private void InitializeComponent()
        {
            tbFullPath = new TextBox();
            SuspendLayout();
            // 
            // tbFullPath
            // 
            tbFullPath.Location = new Point(12, 334);
            tbFullPath.Name = "tbFullPath";
            tbFullPath.Size = new Size(426, 23);
            tbFullPath.TabIndex = 16;
            // 
            // DialogSelectTargetFolder
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(450, 380);
            Controls.Add(tbFullPath);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DialogSelectTargetFolder";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Target Folder";
            ResumeLayout(false);
            PerformLayout();
        }

        private void SetupControls()
        {
            // Label
            lblPrompt = new Label
            {
                Text = "Select the target folder:",
                Location = new Point(20, 20),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular)
            };
            this.Controls.Add(lblPrompt);

            // TextBox
            tbTargetFolder = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ReadOnly = true,
                BackColor = Color.White
            };
            this.Controls.Add(tbTargetFolder);

            // FlowLayoutPanel for buttons
            panelButtons = new FlowLayoutPanel
            {
                Location = new Point(20, 90),
                Size = new Size(400, 180),
                AutoScroll = false,
                WrapContents = true
            };
            this.Controls.Add(panelButtons);

            // Open in Explorer Button
            btOpenInExplorer = new Button
            {
                Text = "Open in Explorer",
                Location = new Point(20, 280),
                Size = new Size(120, 30),
                Enabled = false
            };
            btOpenInExplorer.Click += BtOpenInExplorer_Click;
            this.Controls.Add(btOpenInExplorer);

            // OK Button
            btOK = new Button
            {
                Text = "OK",
                Location = new Point(250, 280),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };
            btOK.Click += BtOK_Click;
            this.Controls.Add(btOK);

            // Cancel Button
            btCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(340, 280),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btCancel);

            this.AcceptButton = btOK;
            this.CancelButton = btCancel;
        }

        private void CreateButtons()
        {
            // Create buttons for A-Z
            for (char c = 'A'; c <= 'Z'; c++)
            {
                CreateFolderButton(c.ToString());
            }

            // Create buttons for 0-9
            for (int i = 0; i <= 9; i++)
            {
                CreateFolderButton(i.ToString());
            }
        }

        private void CreateFolderButton(string text)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(35, 35),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(2),
                BackColor = Color.LightBlue
            };
            
            btn.Click += FolderButton_Click;
            panelButtons.Controls.Add(btn);
        }

        private void FolderButton_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                tbTargetFolder.Text = btn.Text;
                SelectedFolder = btn.Text;
                
                // Enable the Open in Explorer button
                btOpenInExplorer.Enabled = !string.IsNullOrWhiteSpace(baseTargetFolder);
                
                // Highlight the selected button
                foreach (Control ctrl in panelButtons.Controls)
                {
                    if (ctrl is Button b)
                    {
                        b.BackColor = (b == btn) ? Color.LightGreen : Color.LightBlue;
                    }
                }
            }
        }

        private void BtOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbTargetFolder.Text))
            {
                MessageBox.Show("Please select a folder by clicking one of the buttons.", 
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
            }
            else
            {
                SelectedFolder = tbTargetFolder.Text;
            }
        }

        private void BtOpenInExplorer_Click(object sender, EventArgs e)
        {
            string fullPath = BuildFullTargetPath();
            if (!string.IsNullOrWhiteSpace(fullPath))
            {
                OpenFolderInExplorer(fullPath);
            }
        }

        /// <summary>
        /// Builds the full target path by concatenating the base target folder with the selected subfolder
        /// </summary>
        /// <returns>The full path to the target folder</returns>
        private string BuildFullTargetPath()
        {
            if (string.IsNullOrWhiteSpace(baseTargetFolder))
            {
                MessageBox.Show("Base target folder is not set.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(tbTargetFolder.Text))
            {
                MessageBox.Show("Please select a subfolder first.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return string.Empty;
            }

            // Ensure the base path ends with a directory separator
            string normalizedBase = baseTargetFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            
            // Combine the paths
            string fullPath = Path.Combine(normalizedBase, tbTargetFolder.Text);
            
            return fullPath;
        }

        /// <summary>
        /// Opens the specified folder in Windows File Explorer
        /// </summary>
        /// <param name="folderPath">The full path to the folder to open</param>
        public static void OpenFolderInExplorer(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                MessageBox.Show("Invalid folder path.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Create the directory if it doesn't exist
                if (!Directory.Exists(folderPath))
                {
                    var result = MessageBox.Show(
                        $"Folder does not exist:\n{folderPath}\n\nDo you want to create it?",
                        "Folder Not Found",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    else
                    {
                        return;
                    }
                }

                // Launch Windows Explorer
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{folderPath}\"",
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to open folder in Explorer:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}