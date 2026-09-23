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
            btOpenDialog = new System.Windows.Forms.Button();
            btClose = new System.Windows.Forms.Button();
            tbDirPath2 = new System.Windows.Forms.TextBox();
            rbRoot = new System.Windows.Forms.RadioButton();
            rbMyPics = new System.Windows.Forms.RadioButton();
            rbMyDocs = new System.Windows.Forms.RadioButton();
            rbMyComputer = new System.Windows.Forms.RadioButton();
            rbC = new System.Windows.Forms.RadioButton();
            rbDesktop = new System.Windows.Forms.RadioButton();
            rbTarot = new System.Windows.Forms.RadioButton();
            groupBox = new System.Windows.Forms.GroupBox();
            gbSaveFolderAs = new System.Windows.Forms.GroupBox();
            rbWatchFolder3 = new System.Windows.Forms.RadioButton();
            rbSource4 = new System.Windows.Forms.RadioButton();
            rbWatchFolder2 = new System.Windows.Forms.RadioButton();
            rbAlarmWav = new System.Windows.Forms.RadioButton();
            rbSource5 = new System.Windows.Forms.RadioButton();
            rbTargetDir1 = new System.Windows.Forms.RadioButton();
            rbWatchFolder = new System.Windows.Forms.RadioButton();
            rbTargetDir2 = new System.Windows.Forms.RadioButton();
            tbBaseDirectory = new System.Windows.Forms.TextBox();
            cbUseBaseDirectory = new System.Windows.Forms.CheckBox();
            btSubFolders = new System.Windows.Forms.Button();
            btSetTargetFolder = new System.Windows.Forms.Button();
            btSetFolder = new System.Windows.Forms.Button();
            btVerifyFolder = new System.Windows.Forms.Button();
            tbStatus = new System.Windows.Forms.TextBox();
            btSetWatchFolder = new System.Windows.Forms.Button();
            tbTargetAssign = new System.Windows.Forms.TextBox();
            btResetTarget1 = new System.Windows.Forms.Button();
            comboTargetHistory = new System.Windows.Forms.ComboBox();
            groupBox.SuspendLayout();
            gbSaveFolderAs.SuspendLayout();
            SuspendLayout();
            // 
            // btOpenDialog
            // 
            btOpenDialog.Location = new System.Drawing.Point(197, 26);
            btOpenDialog.Margin = new System.Windows.Forms.Padding(2);
            btOpenDialog.Name = "btOpenDialog";
            btOpenDialog.Size = new System.Drawing.Size(105, 30);
            btOpenDialog.TabIndex = 54;
            btOpenDialog.Text = "Select Folder";
            btOpenDialog.UseVisualStyleBackColor = true;
            btOpenDialog.Click += btOpen_Click;
            // 
            // btClose
            // 
            btClose.Location = new System.Drawing.Point(164, 60);
            btClose.Margin = new System.Windows.Forms.Padding(2);
            btClose.Name = "btClose";
            btClose.Size = new System.Drawing.Size(170, 30);
            btClose.TabIndex = 55;
            btClose.Text = "Close and Set Target Folder";
            btClose.UseVisualStyleBackColor = true;
            btClose.Click += btClose_Click;
            // 
            // tbDirPath2
            // 
            tbDirPath2.Location = new System.Drawing.Point(26, 254);
            tbDirPath2.Margin = new System.Windows.Forms.Padding(2);
            tbDirPath2.Name = "tbDirPath2";
            tbDirPath2.Size = new System.Drawing.Size(438, 23);
            tbDirPath2.TabIndex = 57;
            // 
            // rbRoot
            // 
            rbRoot.AutoSize = true;
            rbRoot.Location = new System.Drawing.Point(15, 168);
            rbRoot.Margin = new System.Windows.Forms.Padding(2);
            rbRoot.Name = "rbRoot";
            rbRoot.Size = new System.Drawing.Size(107, 19);
            rbRoot.TabIndex = 55;
            rbRoot.Text = "Desktop (Root )";
            rbRoot.UseVisualStyleBackColor = true;
            rbRoot.CheckedChanged += rbRoot_CheckedChanged;
            // 
            // rbMyPics
            // 
            rbMyPics.AutoSize = true;
            rbMyPics.Location = new System.Drawing.Point(15, 45);
            rbMyPics.Margin = new System.Windows.Forms.Padding(2);
            rbMyPics.Name = "rbMyPics";
            rbMyPics.Size = new System.Drawing.Size(63, 19);
            rbMyPics.TabIndex = 50;
            rbMyPics.Text = "MyPics";
            rbMyPics.UseVisualStyleBackColor = true;
            rbMyPics.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // rbMyDocs
            // 
            rbMyDocs.AutoSize = true;
            rbMyDocs.Location = new System.Drawing.Point(15, 67);
            rbMyDocs.Margin = new System.Windows.Forms.Padding(2);
            rbMyDocs.Name = "rbMyDocs";
            rbMyDocs.Size = new System.Drawing.Size(68, 19);
            rbMyDocs.TabIndex = 51;
            rbMyDocs.Text = "MyDocs";
            rbMyDocs.UseVisualStyleBackColor = true;
            rbMyDocs.CheckedChanged += rbMyDocs_CheckedChanged;
            // 
            // rbMyComputer
            // 
            rbMyComputer.AutoSize = true;
            rbMyComputer.Checked = true;
            rbMyComputer.Location = new System.Drawing.Point(15, 20);
            rbMyComputer.Margin = new System.Windows.Forms.Padding(2);
            rbMyComputer.Name = "rbMyComputer";
            rbMyComputer.Size = new System.Drawing.Size(96, 19);
            rbMyComputer.TabIndex = 49;
            rbMyComputer.TabStop = true;
            rbMyComputer.Text = "MyComputer";
            rbMyComputer.UseVisualStyleBackColor = true;
            rbMyComputer.CheckedChanged += rbMyComputer_CheckedChanged;
            // 
            // rbC
            // 
            rbC.AutoSize = true;
            rbC.Location = new System.Drawing.Point(15, 92);
            rbC.Margin = new System.Windows.Forms.Padding(2);
            rbC.Name = "rbC";
            rbC.Size = new System.Drawing.Size(36, 19);
            rbC.TabIndex = 52;
            rbC.Text = "C:";
            rbC.UseVisualStyleBackColor = true;
            rbC.CheckedChanged += rbC_CheckedChanged;
            // 
            // rbDesktop
            // 
            rbDesktop.AutoSize = true;
            rbDesktop.Location = new System.Drawing.Point(15, 118);
            rbDesktop.Margin = new System.Windows.Forms.Padding(2);
            rbDesktop.Name = "rbDesktop";
            rbDesktop.Size = new System.Drawing.Size(68, 19);
            rbDesktop.TabIndex = 53;
            rbDesktop.Text = "Desktop";
            rbDesktop.UseVisualStyleBackColor = true;
            rbDesktop.CheckedChanged += rbDesktop_CheckedChanged;
            // 
            // rbTarot
            // 
            rbTarot.AutoSize = true;
            rbTarot.Location = new System.Drawing.Point(15, 143);
            rbTarot.Margin = new System.Windows.Forms.Padding(2);
            rbTarot.Name = "rbTarot";
            rbTarot.Size = new System.Drawing.Size(50, 19);
            rbTarot.TabIndex = 54;
            rbTarot.Text = "tarot";
            rbTarot.UseVisualStyleBackColor = true;
            rbTarot.CheckedChanged += rbTarot_CheckedChanged;
            // 
            // groupBox
            // 
            groupBox.Controls.Add(rbRoot);
            groupBox.Controls.Add(rbTarot);
            groupBox.Controls.Add(rbDesktop);
            groupBox.Controls.Add(rbC);
            groupBox.Controls.Add(rbMyComputer);
            groupBox.Controls.Add(rbMyDocs);
            groupBox.Controls.Add(rbMyPics);
            groupBox.Location = new System.Drawing.Point(10, 12);
            groupBox.Margin = new System.Windows.Forms.Padding(2);
            groupBox.Name = "groupBox";
            groupBox.Padding = new System.Windows.Forms.Padding(2);
            groupBox.Size = new System.Drawing.Size(128, 207);
            groupBox.TabIndex = 53;
            groupBox.TabStop = false;
            groupBox.Text = "Start selection at:";
            // 
            // gbSaveFolderAs
            // 
            gbSaveFolderAs.Controls.Add(rbWatchFolder3);
            gbSaveFolderAs.Controls.Add(rbSource4);
            gbSaveFolderAs.Controls.Add(rbWatchFolder2);
            gbSaveFolderAs.Controls.Add(rbAlarmWav);
            gbSaveFolderAs.Controls.Add(rbSource5);
            gbSaveFolderAs.Controls.Add(rbTargetDir1);
            gbSaveFolderAs.Controls.Add(rbWatchFolder);
            gbSaveFolderAs.Controls.Add(rbTargetDir2);
            gbSaveFolderAs.Location = new System.Drawing.Point(358, 12);
            gbSaveFolderAs.Margin = new System.Windows.Forms.Padding(2);
            gbSaveFolderAs.Name = "gbSaveFolderAs";
            gbSaveFolderAs.Padding = new System.Windows.Forms.Padding(2);
            gbSaveFolderAs.Size = new System.Drawing.Size(128, 219);
            gbSaveFolderAs.TabIndex = 56;
            gbSaveFolderAs.TabStop = false;
            gbSaveFolderAs.Text = "Save As:";
            // 
            // rbWatchFolder3
            // 
            rbWatchFolder3.AutoSize = true;
            rbWatchFolder3.Location = new System.Drawing.Point(16, 191);
            rbWatchFolder3.Margin = new System.Windows.Forms.Padding(2);
            rbWatchFolder3.Name = "rbWatchFolder3";
            rbWatchFolder3.Size = new System.Drawing.Size(68, 19);
            rbWatchFolder3.TabIndex = 56;
            rbWatchFolder3.Text = "Watch 3";
            rbWatchFolder3.UseVisualStyleBackColor = true;
            // 
            // rbSource4
            // 
            rbSource4.AutoSize = true;
            rbSource4.Location = new System.Drawing.Point(16, 168);
            rbSource4.Margin = new System.Windows.Forms.Padding(2);
            rbSource4.Name = "rbSource4";
            rbSource4.Size = new System.Drawing.Size(70, 19);
            rbSource4.TabIndex = 55;
            rbSource4.Text = "Source 4";
            rbSource4.UseVisualStyleBackColor = true;
            // 
            // rbWatchFolder2
            // 
            rbWatchFolder2.AutoSize = true;
            rbWatchFolder2.Location = new System.Drawing.Point(16, 143);
            rbWatchFolder2.Margin = new System.Windows.Forms.Padding(2);
            rbWatchFolder2.Name = "rbWatchFolder2";
            rbWatchFolder2.Size = new System.Drawing.Size(68, 19);
            rbWatchFolder2.TabIndex = 54;
            rbWatchFolder2.Text = "Watch 2";
            rbWatchFolder2.UseVisualStyleBackColor = true;
            // 
            // rbAlarmWav
            // 
            rbAlarmWav.AutoSize = true;
            rbAlarmWav.Location = new System.Drawing.Point(16, 118);
            rbAlarmWav.Margin = new System.Windows.Forms.Padding(2);
            rbAlarmWav.Name = "rbAlarmWav";
            rbAlarmWav.Size = new System.Drawing.Size(83, 19);
            rbAlarmWav.TabIndex = 53;
            rbAlarmWav.Text = "Alarm Wav";
            rbAlarmWav.UseVisualStyleBackColor = true;
            // 
            // rbSource5
            // 
            rbSource5.AutoSize = true;
            rbSource5.Location = new System.Drawing.Point(16, 92);
            rbSource5.Margin = new System.Windows.Forms.Padding(2);
            rbSource5.Name = "rbSource5";
            rbSource5.Size = new System.Drawing.Size(70, 19);
            rbSource5.TabIndex = 52;
            rbSource5.Text = "Source 5";
            rbSource5.UseVisualStyleBackColor = true;
            // 
            // rbTargetDir1
            // 
            rbTargetDir1.AutoSize = true;
            rbTargetDir1.Checked = true;
            rbTargetDir1.Location = new System.Drawing.Point(16, 20);
            rbTargetDir1.Margin = new System.Windows.Forms.Padding(2);
            rbTargetDir1.Name = "rbTargetDir1";
            rbTargetDir1.Size = new System.Drawing.Size(82, 19);
            rbTargetDir1.TabIndex = 49;
            rbTargetDir1.TabStop = true;
            rbTargetDir1.Text = "target dir 1";
            rbTargetDir1.UseVisualStyleBackColor = true;
            // 
            // rbWatchFolder
            // 
            rbWatchFolder.AutoSize = true;
            rbWatchFolder.Location = new System.Drawing.Point(16, 67);
            rbWatchFolder.Margin = new System.Windows.Forms.Padding(2);
            rbWatchFolder.Name = "rbWatchFolder";
            rbWatchFolder.Size = new System.Drawing.Size(95, 19);
            rbWatchFolder.TabIndex = 51;
            rbWatchFolder.Text = "Watch Folder";
            rbWatchFolder.UseVisualStyleBackColor = true;
            // 
            // rbTargetDir2
            // 
            rbTargetDir2.AutoSize = true;
            rbTargetDir2.Location = new System.Drawing.Point(16, 45);
            rbTargetDir2.Margin = new System.Windows.Forms.Padding(2);
            rbTargetDir2.Name = "rbTargetDir2";
            rbTargetDir2.Size = new System.Drawing.Size(82, 19);
            rbTargetDir2.TabIndex = 50;
            rbTargetDir2.Text = "target dir 2";
            rbTargetDir2.UseVisualStyleBackColor = true;
            // 
            // tbBaseDirectory
            // 
            tbBaseDirectory.Location = new System.Drawing.Point(26, 316);
            tbBaseDirectory.Margin = new System.Windows.Forms.Padding(2);
            tbBaseDirectory.Name = "tbBaseDirectory";
            tbBaseDirectory.Size = new System.Drawing.Size(438, 23);
            tbBaseDirectory.TabIndex = 58;
            // 
            // cbUseBaseDirectory
            // 
            cbUseBaseDirectory.AutoSize = true;
            cbUseBaseDirectory.Location = new System.Drawing.Point(42, 292);
            cbUseBaseDirectory.Margin = new System.Windows.Forms.Padding(2);
            cbUseBaseDirectory.Name = "cbUseBaseDirectory";
            cbUseBaseDirectory.Size = new System.Drawing.Size(85, 19);
            cbUseBaseDirectory.TabIndex = 194;
            cbUseBaseDirectory.Text = "Set As Base";
            cbUseBaseDirectory.UseVisualStyleBackColor = true;
            cbUseBaseDirectory.CheckedChanged += cbUseBaseDirectory_CheckedChanged;
            // 
            // btSubFolders
            // 
            btSubFolders.Location = new System.Drawing.Point(32, 343);
            btSubFolders.Margin = new System.Windows.Forms.Padding(2);
            btSubFolders.Name = "btSubFolders";
            btSubFolders.Size = new System.Drawing.Size(170, 30);
            btSubFolders.TabIndex = 195;
            btSubFolders.Text = "Get Subfolders";
            btSubFolders.UseVisualStyleBackColor = true;
            btSubFolders.Click += btSubFolders_Click;
            // 
            // btSetTargetFolder
            // 
            btSetTargetFolder.Location = new System.Drawing.Point(164, 104);
            btSetTargetFolder.Margin = new System.Windows.Forms.Padding(2);
            btSetTargetFolder.Name = "btSetTargetFolder";
            btSetTargetFolder.Size = new System.Drawing.Size(170, 30);
            btSetTargetFolder.TabIndex = 196;
            btSetTargetFolder.Text = "Set Target Folder";
            btSetTargetFolder.UseVisualStyleBackColor = true;
            btSetTargetFolder.Click += btSetTargetFolder_Click;
            // 
            // btSetFolder
            // 
            btSetFolder.Location = new System.Drawing.Point(29, 223);
            btSetFolder.Margin = new System.Windows.Forms.Padding(2);
            btSetFolder.Name = "btSetFolder";
            btSetFolder.Size = new System.Drawing.Size(173, 22);
            btSetFolder.TabIndex = 197;
            btSetFolder.Text = "Set Folder tbDirPath2";
            btSetFolder.UseVisualStyleBackColor = true;
            btSetFolder.Click += btSetFolder_Click;
            // 
            // btVerifyFolder
            // 
            btVerifyFolder.Location = new System.Drawing.Point(197, 283);
            btVerifyFolder.Margin = new System.Windows.Forms.Padding(2);
            btVerifyFolder.Name = "btVerifyFolder";
            btVerifyFolder.Size = new System.Drawing.Size(105, 28);
            btVerifyFolder.TabIndex = 198;
            btVerifyFolder.Text = "Verify Folder";
            btVerifyFolder.UseVisualStyleBackColor = true;
            btVerifyFolder.Click += btVerifyFolder_Click;
            // 
            // tbStatus
            // 
            tbStatus.Location = new System.Drawing.Point(310, 286);
            tbStatus.Margin = new System.Windows.Forms.Padding(2);
            tbStatus.Name = "tbStatus";
            tbStatus.Size = new System.Drawing.Size(172, 23);
            tbStatus.TabIndex = 199;
            // 
            // btSetWatchFolder
            // 
            btSetWatchFolder.Location = new System.Drawing.Point(164, 155);
            btSetWatchFolder.Margin = new System.Windows.Forms.Padding(2);
            btSetWatchFolder.Name = "btSetWatchFolder";
            btSetWatchFolder.Size = new System.Drawing.Size(170, 30);
            btSetWatchFolder.TabIndex = 200;
            btSetWatchFolder.Text = "Set Watch Folder1";
            btSetWatchFolder.UseVisualStyleBackColor = true;
            btSetWatchFolder.Click += btSetWatchFolder_Click;
            // 
            // tbTargetAssign
            // 
            tbTargetAssign.Location = new System.Drawing.Point(31, 400);
            tbTargetAssign.Margin = new System.Windows.Forms.Padding(2);
            tbTargetAssign.Name = "tbTargetAssign";
            tbTargetAssign.Size = new System.Drawing.Size(438, 23);
            tbTargetAssign.TabIndex = 201;
            // 
            // btResetTarget1
            // 
            btResetTarget1.Location = new System.Drawing.Point(291, 364);
            btResetTarget1.Margin = new System.Windows.Forms.Padding(2);
            btResetTarget1.Name = "btResetTarget1";
            btResetTarget1.Size = new System.Drawing.Size(173, 22);
            btResetTarget1.TabIndex = 202;
            btResetTarget1.Text = "reset as target1";
            btResetTarget1.UseVisualStyleBackColor = true;
            btResetTarget1.Click += btResetTarget1_Click;
            // 
            // comboTargetHistory
            // 
            comboTargetHistory.FormattingEnabled = true;
            comboTargetHistory.Location = new System.Drawing.Point(46, 429);
            comboTargetHistory.Name = "comboTargetHistory";
            comboTargetHistory.Size = new System.Drawing.Size(396, 23);
            comboTargetHistory.TabIndex = 203;
            comboTargetHistory.SelectedIndexChanged += comboTargetHistory_SelectedIndexChanged;
            // 
            // DialogFolderSelection
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(496, 530);
            Controls.Add(comboTargetHistory);
            Controls.Add(btResetTarget1);
            Controls.Add(tbTargetAssign);
            Controls.Add(btSetWatchFolder);
            Controls.Add(tbStatus);
            Controls.Add(btVerifyFolder);
            Controls.Add(btSetFolder);
            Controls.Add(btSetTargetFolder);
            Controls.Add(btSubFolders);
            Controls.Add(cbUseBaseDirectory);
            Controls.Add(tbBaseDirectory);
            Controls.Add(gbSaveFolderAs);
            Controls.Add(tbDirPath2);
            Controls.Add(btClose);
            Controls.Add(btOpenDialog);
            Controls.Add(groupBox);
            Margin = new System.Windows.Forms.Padding(2);
            Name = "DialogFolderSelection";
            Text = "DialogFolderSelection";
            Load += DialogFolderSelection_Load;
            groupBox.ResumeLayout(false);
            groupBox.PerformLayout();
            gbSaveFolderAs.ResumeLayout(false);
            gbSaveFolderAs.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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
        private System.Windows.Forms.Button btSetWatchFolder;
        private System.Windows.Forms.RadioButton rbWatchFolder3;
        private System.Windows.Forms.TextBox tbTargetAssign;
        private System.Windows.Forms.Button btResetTarget1;
        private System.Windows.Forms.ComboBox comboTargetHistory;
    }
}