namespace SymbolDB
{
    partial class DisplayParms
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
            btCreateInitFile = new System.Windows.Forms.Button();
            btSaveInitFile = new System.Windows.Forms.Button();
            btVerifyIniFile = new System.Windows.Forms.Button();
            btClose = new System.Windows.Forms.Button();
            btMyDocs = new System.Windows.Forms.Button();
            dgvInitParms = new System.Windows.Forms.DataGridView();
            tbDirectoryPath = new System.Windows.Forms.TextBox();
            btVerifyMyDocs = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            tbMyDocumentsFolder = new System.Windows.Forms.TextBox();
            tbInitFolderPath = new System.Windows.Forms.TextBox();
            tbEnvironmnet = new System.Windows.Forms.TextBox();
            tbInitFileName = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            tbDotNetVersion = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvInitParms).BeginInit();
            SuspendLayout();
            // 
            // btCreateInitFile
            // 
            btCreateInitFile.Location = new System.Drawing.Point(55, 246);
            btCreateInitFile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btCreateInitFile.Name = "btCreateInitFile";
            btCreateInitFile.Size = new System.Drawing.Size(101, 32);
            btCreateInitFile.TabIndex = 114;
            btCreateInitFile.Text = "Create Init File";
            btCreateInitFile.UseVisualStyleBackColor = true;
            btCreateInitFile.Click += btCreateInitFile_Click;
            // 
            // btSaveInitFile
            // 
            btSaveInitFile.Location = new System.Drawing.Point(181, 246);
            btSaveInitFile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btSaveInitFile.Name = "btSaveInitFile";
            btSaveInitFile.Size = new System.Drawing.Size(101, 32);
            btSaveInitFile.TabIndex = 120;
            btSaveInitFile.Text = "Save Init File";
            btSaveInitFile.UseVisualStyleBackColor = true;
            btSaveInitFile.Click += btSaveInitFile_Click;
            // 
            // btVerifyIniFile
            // 
            btVerifyIniFile.Location = new System.Drawing.Point(176, 164);
            btVerifyIniFile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btVerifyIniFile.Name = "btVerifyIniFile";
            btVerifyIniFile.Size = new System.Drawing.Size(130, 35);
            btVerifyIniFile.TabIndex = 122;
            btVerifyIniFile.Text = "verify ini file";
            btVerifyIniFile.UseVisualStyleBackColor = true;
            btVerifyIniFile.Click += btVerifyIniFile_Click;
            // 
            // btClose
            // 
            btClose.Location = new System.Drawing.Point(1128, 143);
            btClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btClose.Name = "btClose";
            btClose.Size = new System.Drawing.Size(73, 35);
            btClose.TabIndex = 126;
            btClose.Text = "Close";
            btClose.UseVisualStyleBackColor = true;
            btClose.Click += btClose_Click;
            // 
            // btMyDocs
            // 
            btMyDocs.Location = new System.Drawing.Point(37, 122);
            btMyDocs.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btMyDocs.Name = "btMyDocs";
            btMyDocs.Size = new System.Drawing.Size(101, 32);
            btMyDocs.TabIndex = 129;
            btMyDocs.Text = "MyDocs";
            btMyDocs.UseVisualStyleBackColor = true;
            btMyDocs.Click += btMyDocs_Click;
            // 
            // dgvInitParms
            // 
            dgvInitParms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvInitParms.Location = new System.Drawing.Point(8, 34);
            dgvInitParms.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            dgvInitParms.Name = "dgvInitParms";
            dgvInitParms.RowHeadersWidth = 51;
            dgvInitParms.RowTemplate.Height = 24;
            dgvInitParms.Size = new System.Drawing.Size(1242, 74);
            dgvInitParms.TabIndex = 130;
            // 
            // tbDirectoryPath
            // 
            tbDirectoryPath.Location = new System.Drawing.Point(756, 320);
            tbDirectoryPath.Margin = new System.Windows.Forms.Padding(4);
            tbDirectoryPath.Name = "tbDirectoryPath";
            tbDirectoryPath.Size = new System.Drawing.Size(494, 23);
            tbDirectoryPath.TabIndex = 174;
            // 
            // btVerifyMyDocs
            // 
            btVerifyMyDocs.Location = new System.Drawing.Point(155, 122);
            btVerifyMyDocs.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btVerifyMyDocs.Name = "btVerifyMyDocs";
            btVerifyMyDocs.Size = new System.Drawing.Size(108, 35);
            btVerifyMyDocs.TabIndex = 176;
            btVerifyMyDocs.Text = "verify  MyDocs";
            btVerifyMyDocs.UseVisualStyleBackColor = true;
            btVerifyMyDocs.Click += btVerifyMyDocs_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(35, 164);
            button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(130, 35);
            button1.TabIndex = 177;
            button1.Text = "verify dazen folder";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // tbMyDocumentsFolder
            // 
            tbMyDocumentsFolder.Location = new System.Drawing.Point(37, 208);
            tbMyDocumentsFolder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tbMyDocumentsFolder.Name = "tbMyDocumentsFolder";
            tbMyDocumentsFolder.Size = new System.Drawing.Size(425, 23);
            tbMyDocumentsFolder.TabIndex = 178;
            // 
            // tbInitFolderPath
            // 
            tbInitFolderPath.Location = new System.Drawing.Point(37, 334);
            tbInitFolderPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tbInitFolderPath.Name = "tbInitFolderPath";
            tbInitFolderPath.Size = new System.Drawing.Size(427, 23);
            tbInitFolderPath.TabIndex = 179;
            tbInitFolderPath.TextChanged += tbInitFolderPath_TextChanged;
            // 
            // tbEnvironmnet
            // 
            tbEnvironmnet.Location = new System.Drawing.Point(297, 122);
            tbEnvironmnet.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tbEnvironmnet.Name = "tbEnvironmnet";
            tbEnvironmnet.Size = new System.Drawing.Size(266, 23);
            tbEnvironmnet.TabIndex = 397;
            tbEnvironmnet.Text = "ENVIRONMENT running";
            // 
            // tbInitFileName
            // 
            tbInitFileName.Location = new System.Drawing.Point(37, 297);
            tbInitFileName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tbInitFileName.Name = "tbInitFileName";
            tbInitFileName.Size = new System.Drawing.Size(427, 23);
            tbInitFileName.TabIndex = 398;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(675, 295);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(70, 15);
            label1.TabIndex = 400;
            label1.Text = ".Net Version";
            // 
            // tbDotNetVersion
            // 
            tbDotNetVersion.Location = new System.Drawing.Point(756, 291);
            tbDotNetVersion.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tbDotNetVersion.Name = "tbDotNetVersion";
            tbDotNetVersion.Size = new System.Drawing.Size(470, 23);
            tbDotNetVersion.TabIndex = 399;
            // 
            // DisplayParms
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1285, 380);
            Controls.Add(label1);
            Controls.Add(tbDotNetVersion);
            Controls.Add(tbInitFileName);
            Controls.Add(tbEnvironmnet);
            Controls.Add(tbInitFolderPath);
            Controls.Add(tbMyDocumentsFolder);
            Controls.Add(button1);
            Controls.Add(btVerifyMyDocs);
            Controls.Add(tbDirectoryPath);
            Controls.Add(dgvInitParms);
            Controls.Add(btMyDocs);
            Controls.Add(btClose);
            Controls.Add(btVerifyIniFile);
            Controls.Add(btSaveInitFile);
            Controls.Add(btCreateInitFile);
            Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Name = "DisplayParms";
            Text = "DisplayParms";
            ((System.ComponentModel.ISupportInitialize)dgvInitParms).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btCreateInitFile;
        private System.Windows.Forms.Button btSaveInitFile;
        private System.Windows.Forms.Button btVerifyIniFile;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.Button btMyDocs;
        private System.Windows.Forms.DataGridView dgvInitParms;
        private System.Windows.Forms.TextBox tbDirectoryPath;
        private System.Windows.Forms.Button btVerifyMyDocs;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbMyDocumentsFolder;
        private System.Windows.Forms.TextBox tbInitFolderPath;
        private System.Windows.Forms.TextBox tbEnvironmnet;
        private System.Windows.Forms.TextBox tbInitFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbDotNetVersion;
    }
}