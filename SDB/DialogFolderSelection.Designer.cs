namespace SymbolDB
{
    partial class DialogFolderSelection
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
            this.btOpenDialog = new System.Windows.Forms.Button();
            this.btClose = new System.Windows.Forms.Button();
            this.tbDirPath2 = new System.Windows.Forms.TextBox();
            this.rbRoot = new System.Windows.Forms.RadioButton();
            this.rbMyPics = new System.Windows.Forms.RadioButton();
            this.rbMyDocs = new System.Windows.Forms.RadioButton();
            this.rbMyComputer = new System.Windows.Forms.RadioButton();
            this.rbC = new System.Windows.Forms.RadioButton();
            this.rbDesktop = new System.Windows.Forms.RadioButton();
            this.rbTarot = new System.Windows.Forms.RadioButton();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.gbSaveFolderAs = new System.Windows.Forms.GroupBox();
            this.rbSource4 = new System.Windows.Forms.RadioButton();
            this.rbWatchFolder2 = new System.Windows.Forms.RadioButton();
            this.rbAlarmWav = new System.Windows.Forms.RadioButton();
            this.rbSource5 = new System.Windows.Forms.RadioButton();
            this.rbTargetDir1 = new System.Windows.Forms.RadioButton();
            this.rbWatchFolder = new System.Windows.Forms.RadioButton();
            this.rbTargetDir2 = new System.Windows.Forms.RadioButton();
            this.tbBaseDirectory = new System.Windows.Forms.TextBox();
            this.cbUseBaseDirectory = new System.Windows.Forms.CheckBox();
            this.btSubFolders = new System.Windows.Forms.Button();
            this.btSetTargetFolder = new System.Windows.Forms.Button();
            this.btSetFolder = new System.Windows.Forms.Button();
            this.btVerifyFolder = new System.Windows.Forms.Button();
            this.tbStatus = new System.Windows.Forms.TextBox();
            this.groupBox.SuspendLayout();
            this.gbSaveFolderAs.SuspendLayout();
            this.SuspendLayout();
            // 
            // btOpenDialog
            // 
            this.btOpenDialog.Location = new System.Drawing.Point(153, 63);
            this.btOpenDialog.Margin = new System.Windows.Forms.Padding(2);
            this.btOpenDialog.Name = "btOpenDialog";
            this.btOpenDialog.Size = new System.Drawing.Size(90, 26);
            this.btOpenDialog.TabIndex = 54;
            this.btOpenDialog.Text = "Select Folder";
            this.btOpenDialog.UseVisualStyleBackColor = true;
            this.btOpenDialog.Click += new System.EventHandler(this.btOpen_Click);
            // 
            // btClose
            // 
            this.btClose.Location = new System.Drawing.Point(153, 107);
            this.btClose.Margin = new System.Windows.Forms.Padding(2);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(146, 26);
            this.btClose.TabIndex = 55;
            this.btClose.Text = "Close and Set Target Folder";
            this.btClose.UseVisualStyleBackColor = true;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // tbDirPath2
            // 
            this.tbDirPath2.Location = new System.Drawing.Point(22, 220);
            this.tbDirPath2.Margin = new System.Windows.Forms.Padding(2);
            this.tbDirPath2.Name = "tbDirPath2";
            this.tbDirPath2.Size = new System.Drawing.Size(376, 20);
            this.tbDirPath2.TabIndex = 57;
            // 
            // rbRoot
            // 
            this.rbRoot.AutoSize = true;
            this.rbRoot.Location = new System.Drawing.Point(13, 146);
            this.rbRoot.Margin = new System.Windows.Forms.Padding(2);
            this.rbRoot.Name = "rbRoot";
            this.rbRoot.Size = new System.Drawing.Size(100, 17);
            this.rbRoot.TabIndex = 55;
            this.rbRoot.Text = "Desktop (Root )";
            this.rbRoot.UseVisualStyleBackColor = true;
            this.rbRoot.CheckedChanged += new System.EventHandler(this.rbRoot_CheckedChanged);
            // 
            // rbMyPics
            // 
            this.rbMyPics.AutoSize = true;
            this.rbMyPics.Location = new System.Drawing.Point(13, 39);
            this.rbMyPics.Margin = new System.Windows.Forms.Padding(2);
            this.rbMyPics.Name = "rbMyPics";
            this.rbMyPics.Size = new System.Drawing.Size(59, 17);
            this.rbMyPics.TabIndex = 50;
            this.rbMyPics.Text = "MyPics";
            this.rbMyPics.UseVisualStyleBackColor = true;
            this.rbMyPics.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // rbMyDocs
            // 
            this.rbMyDocs.AutoSize = true;
            this.rbMyDocs.Location = new System.Drawing.Point(13, 58);
            this.rbMyDocs.Margin = new System.Windows.Forms.Padding(2);
            this.rbMyDocs.Name = "rbMyDocs";
            this.rbMyDocs.Size = new System.Drawing.Size(64, 17);
            this.rbMyDocs.TabIndex = 51;
            this.rbMyDocs.Text = "MyDocs";
            this.rbMyDocs.UseVisualStyleBackColor = true;
            this.rbMyDocs.CheckedChanged += new System.EventHandler(this.rbMyDocs_CheckedChanged);
            // 
            // rbMyComputer
            // 
            this.rbMyComputer.AutoSize = true;
            this.rbMyComputer.Checked = true;
            this.rbMyComputer.Location = new System.Drawing.Point(13, 17);
            this.rbMyComputer.Margin = new System.Windows.Forms.Padding(2);
            this.rbMyComputer.Name = "rbMyComputer";
            this.rbMyComputer.Size = new System.Drawing.Size(84, 17);
            this.rbMyComputer.TabIndex = 49;
            this.rbMyComputer.TabStop = true;
            this.rbMyComputer.Text = "MyComputer";
            this.rbMyComputer.UseVisualStyleBackColor = true;
            this.rbMyComputer.CheckedChanged += new System.EventHandler(this.rbMyComputer_CheckedChanged);
            // 
            // rbC
            // 
            this.rbC.AutoSize = true;
            this.rbC.Location = new System.Drawing.Point(13, 80);
            this.rbC.Margin = new System.Windows.Forms.Padding(2);
            this.rbC.Name = "rbC";
            this.rbC.Size = new System.Drawing.Size(35, 17);
            this.rbC.TabIndex = 52;
            this.rbC.Text = "C:";
            this.rbC.UseVisualStyleBackColor = true;
            this.rbC.CheckedChanged += new System.EventHandler(this.rbC_CheckedChanged);
            // 
            // rbDesktop
            // 
            this.rbDesktop.AutoSize = true;
            this.rbDesktop.Location = new System.Drawing.Point(13, 102);
            this.rbDesktop.Margin = new System.Windows.Forms.Padding(2);
            this.rbDesktop.Name = "rbDesktop";
            this.rbDesktop.Size = new System.Drawing.Size(65, 17);
            this.rbDesktop.TabIndex = 53;
            this.rbDesktop.Text = "Desktop";
            this.rbDesktop.UseVisualStyleBackColor = true;
            this.rbDesktop.CheckedChanged += new System.EventHandler(this.rbDesktop_CheckedChanged);
            // 
            // rbTarot
            // 
            this.rbTarot.AutoSize = true;
            this.rbTarot.Location = new System.Drawing.Point(13, 124);
            this.rbTarot.Margin = new System.Windows.Forms.Padding(2);
            this.rbTarot.Name = "rbTarot";
            this.rbTarot.Size = new System.Drawing.Size(46, 17);
            this.rbTarot.TabIndex = 54;
            this.rbTarot.Text = "tarot";
            this.rbTarot.UseVisualStyleBackColor = true;
            this.rbTarot.CheckedChanged += new System.EventHandler(this.rbTarot_CheckedChanged);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.rbRoot);
            this.groupBox.Controls.Add(this.rbTarot);
            this.groupBox.Controls.Add(this.rbDesktop);
            this.groupBox.Controls.Add(this.rbC);
            this.groupBox.Controls.Add(this.rbMyComputer);
            this.groupBox.Controls.Add(this.rbMyDocs);
            this.groupBox.Controls.Add(this.rbMyPics);
            this.groupBox.Location = new System.Drawing.Point(9, 10);
            this.groupBox.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox.Name = "groupBox";
            this.groupBox.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox.Size = new System.Drawing.Size(110, 179);
            this.groupBox.TabIndex = 53;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Start selection at:";
            // 
            // gbSaveFolderAs
            // 
            this.gbSaveFolderAs.Controls.Add(this.rbSource4);
            this.gbSaveFolderAs.Controls.Add(this.rbWatchFolder2);
            this.gbSaveFolderAs.Controls.Add(this.rbAlarmWav);
            this.gbSaveFolderAs.Controls.Add(this.rbSource5);
            this.gbSaveFolderAs.Controls.Add(this.rbTargetDir1);
            this.gbSaveFolderAs.Controls.Add(this.rbWatchFolder);
            this.gbSaveFolderAs.Controls.Add(this.rbTargetDir2);
            this.gbSaveFolderAs.Location = new System.Drawing.Point(307, 10);
            this.gbSaveFolderAs.Margin = new System.Windows.Forms.Padding(2);
            this.gbSaveFolderAs.Name = "gbSaveFolderAs";
            this.gbSaveFolderAs.Padding = new System.Windows.Forms.Padding(2);
            this.gbSaveFolderAs.Size = new System.Drawing.Size(110, 171);
            this.gbSaveFolderAs.TabIndex = 56;
            this.gbSaveFolderAs.TabStop = false;
            this.gbSaveFolderAs.Text = "Save As:";
            // 
            // rbSource4
            // 
            this.rbSource4.AutoSize = true;
            this.rbSource4.Location = new System.Drawing.Point(14, 146);
            this.rbSource4.Margin = new System.Windows.Forms.Padding(2);
            this.rbSource4.Name = "rbSource4";
            this.rbSource4.Size = new System.Drawing.Size(68, 17);
            this.rbSource4.TabIndex = 55;
            this.rbSource4.Text = "Source 4";
            this.rbSource4.UseVisualStyleBackColor = true;
            // 
            // rbWatchFolder2
            // 
            this.rbWatchFolder2.AutoSize = true;
            this.rbWatchFolder2.Location = new System.Drawing.Point(14, 124);
            this.rbWatchFolder2.Margin = new System.Windows.Forms.Padding(2);
            this.rbWatchFolder2.Name = "rbWatchFolder2";
            this.rbWatchFolder2.Size = new System.Drawing.Size(66, 17);
            this.rbWatchFolder2.TabIndex = 54;
            this.rbWatchFolder2.Text = "Watch 2";
            this.rbWatchFolder2.UseVisualStyleBackColor = true;
            // 
            // rbAlarmWav
            // 
            this.rbAlarmWav.AutoSize = true;
            this.rbAlarmWav.Location = new System.Drawing.Point(14, 102);
            this.rbAlarmWav.Margin = new System.Windows.Forms.Padding(2);
            this.rbAlarmWav.Name = "rbAlarmWav";
            this.rbAlarmWav.Size = new System.Drawing.Size(77, 17);
            this.rbAlarmWav.TabIndex = 53;
            this.rbAlarmWav.Text = "Alarm Wav";
            this.rbAlarmWav.UseVisualStyleBackColor = true;
            // 
            // rbSource5
            // 
            this.rbSource5.AutoSize = true;
            this.rbSource5.Location = new System.Drawing.Point(14, 80);
            this.rbSource5.Margin = new System.Windows.Forms.Padding(2);
            this.rbSource5.Name = "rbSource5";
            this.rbSource5.Size = new System.Drawing.Size(68, 17);
            this.rbSource5.TabIndex = 52;
            this.rbSource5.Text = "Source 5";
            this.rbSource5.UseVisualStyleBackColor = true;
            // 
            // rbTargetDir1
            // 
            this.rbTargetDir1.AutoSize = true;
            this.rbTargetDir1.Checked = true;
            this.rbTargetDir1.Location = new System.Drawing.Point(14, 17);
            this.rbTargetDir1.Margin = new System.Windows.Forms.Padding(2);
            this.rbTargetDir1.Name = "rbTargetDir1";
            this.rbTargetDir1.Size = new System.Drawing.Size(75, 17);
            this.rbTargetDir1.TabIndex = 49;
            this.rbTargetDir1.TabStop = true;
            this.rbTargetDir1.Text = "target dir 1";
            this.rbTargetDir1.UseVisualStyleBackColor = true;
            // 
            // rbWatchFolder
            // 
            this.rbWatchFolder.AutoSize = true;
            this.rbWatchFolder.Location = new System.Drawing.Point(14, 58);
            this.rbWatchFolder.Margin = new System.Windows.Forms.Padding(2);
            this.rbWatchFolder.Name = "rbWatchFolder";
            this.rbWatchFolder.Size = new System.Drawing.Size(89, 17);
            this.rbWatchFolder.TabIndex = 51;
            this.rbWatchFolder.Text = "Watch Folder";
            this.rbWatchFolder.UseVisualStyleBackColor = true;
            // 
            // rbTargetDir2
            // 
            this.rbTargetDir2.AutoSize = true;
            this.rbTargetDir2.Location = new System.Drawing.Point(14, 39);
            this.rbTargetDir2.Margin = new System.Windows.Forms.Padding(2);
            this.rbTargetDir2.Name = "rbTargetDir2";
            this.rbTargetDir2.Size = new System.Drawing.Size(75, 17);
            this.rbTargetDir2.TabIndex = 50;
            this.rbTargetDir2.Text = "target dir 2";
            this.rbTargetDir2.UseVisualStyleBackColor = true;
            // 
            // tbBaseDirectory
            // 
            this.tbBaseDirectory.Location = new System.Drawing.Point(22, 274);
            this.tbBaseDirectory.Margin = new System.Windows.Forms.Padding(2);
            this.tbBaseDirectory.Name = "tbBaseDirectory";
            this.tbBaseDirectory.Size = new System.Drawing.Size(376, 20);
            this.tbBaseDirectory.TabIndex = 58;
            // 
            // cbUseBaseDirectory
            // 
            this.cbUseBaseDirectory.AutoSize = true;
            this.cbUseBaseDirectory.Location = new System.Drawing.Point(36, 253);
            this.cbUseBaseDirectory.Margin = new System.Windows.Forms.Padding(2);
            this.cbUseBaseDirectory.Name = "cbUseBaseDirectory";
            this.cbUseBaseDirectory.Size = new System.Drawing.Size(84, 17);
            this.cbUseBaseDirectory.TabIndex = 194;
            this.cbUseBaseDirectory.Text = "Set As Base";
            this.cbUseBaseDirectory.UseVisualStyleBackColor = true;
            this.cbUseBaseDirectory.CheckedChanged += new System.EventHandler(this.cbUseBaseDirectory_CheckedChanged);
            // 
            // btSubFolders
            // 
            this.btSubFolders.Location = new System.Drawing.Point(153, 321);
            this.btSubFolders.Margin = new System.Windows.Forms.Padding(2);
            this.btSubFolders.Name = "btSubFolders";
            this.btSubFolders.Size = new System.Drawing.Size(146, 26);
            this.btSubFolders.TabIndex = 195;
            this.btSubFolders.Text = "Get Subfolders";
            this.btSubFolders.UseVisualStyleBackColor = true;
            this.btSubFolders.Click += new System.EventHandler(this.btSubFolders_Click);
            // 
            // btSetTargetFolder
            // 
            this.btSetTargetFolder.Location = new System.Drawing.Point(153, 147);
            this.btSetTargetFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btSetTargetFolder.Name = "btSetTargetFolder";
            this.btSetTargetFolder.Size = new System.Drawing.Size(146, 26);
            this.btSetTargetFolder.TabIndex = 196;
            this.btSetTargetFolder.Text = "Set Target Folder";
            this.btSetTargetFolder.UseVisualStyleBackColor = true;
            this.btSetTargetFolder.Click += new System.EventHandler(this.btSetTargetFolder_Click);
            // 
            // btSetFolder
            // 
            this.btSetFolder.Location = new System.Drawing.Point(25, 193);
            this.btSetFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btSetFolder.Name = "btSetFolder";
            this.btSetFolder.Size = new System.Drawing.Size(148, 19);
            this.btSetFolder.TabIndex = 197;
            this.btSetFolder.Text = "Set Folder tbDirPath2";
            this.btSetFolder.UseVisualStyleBackColor = true;
            this.btSetFolder.Click += new System.EventHandler(this.btSetFolder_Click);
            // 
            // btVerifyFolder
            // 
            this.btVerifyFolder.Location = new System.Drawing.Point(169, 245);
            this.btVerifyFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btVerifyFolder.Name = "btVerifyFolder";
            this.btVerifyFolder.Size = new System.Drawing.Size(90, 24);
            this.btVerifyFolder.TabIndex = 198;
            this.btVerifyFolder.Text = "Verify Folder";
            this.btVerifyFolder.UseVisualStyleBackColor = true;
            this.btVerifyFolder.Click += new System.EventHandler(this.btVerifyFolder_Click);
            // 
            // tbStatus
            // 
            this.tbStatus.Location = new System.Drawing.Point(266, 248);
            this.tbStatus.Margin = new System.Windows.Forms.Padding(2);
            this.tbStatus.Name = "tbStatus";
            this.tbStatus.Size = new System.Drawing.Size(148, 20);
            this.tbStatus.TabIndex = 199;
            // 
            // DialogFolderSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 408);
            this.Controls.Add(this.tbStatus);
            this.Controls.Add(this.btVerifyFolder);
            this.Controls.Add(this.btSetFolder);
            this.Controls.Add(this.btSetTargetFolder);
            this.Controls.Add(this.btSubFolders);
            this.Controls.Add(this.cbUseBaseDirectory);
            this.Controls.Add(this.tbBaseDirectory);
            this.Controls.Add(this.gbSaveFolderAs);
            this.Controls.Add(this.tbDirPath2);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.btOpenDialog);
            this.Controls.Add(this.groupBox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DialogFolderSelection";
            this.Text = "DialogFolderSelection";
            this.Load += new System.EventHandler(this.DialogFolderSelection_Load);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.gbSaveFolderAs.ResumeLayout(false);
            this.gbSaveFolderAs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btOpenDialog;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.TextBox tbDirPath2;
        private System.Windows.Forms.RadioButton rbRoot;
        private System.Windows.Forms.RadioButton rbMyPics;
        private System.Windows.Forms.RadioButton rbMyDocs;
        private System.Windows.Forms.RadioButton rbMyComputer;
        private System.Windows.Forms.RadioButton rbC;
        private System.Windows.Forms.RadioButton rbDesktop;
        private System.Windows.Forms.RadioButton rbTarot;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.GroupBox gbSaveFolderAs;
        private System.Windows.Forms.RadioButton rbAlarmWav;
        private System.Windows.Forms.RadioButton rbSource5;
        private System.Windows.Forms.RadioButton rbTargetDir1;
        private System.Windows.Forms.RadioButton rbWatchFolder;
        private System.Windows.Forms.RadioButton rbTargetDir2;
        private System.Windows.Forms.RadioButton rbWatchFolder2;
        private System.Windows.Forms.RadioButton rbSource4;
        private System.Windows.Forms.TextBox tbBaseDirectory;
        private System.Windows.Forms.CheckBox cbUseBaseDirectory;
        private System.Windows.Forms.Button btSubFolders;
        private System.Windows.Forms.Button btSetTargetFolder;
        private System.Windows.Forms.Button btSetFolder;
        private System.Windows.Forms.Button btVerifyFolder;
        private System.Windows.Forms.TextBox tbStatus;
    }
}