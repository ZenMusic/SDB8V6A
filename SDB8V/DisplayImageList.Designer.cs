namespace SymbolDB
{
    partial class DisplayImageList
    {
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pbImage = new System.Windows.Forms.PictureBox();
            tbFilePath = new System.Windows.Forms.TextBox();
            btPrevious = new System.Windows.Forms.Button();
            btNext = new System.Windows.Forms.Button();
            btClose = new System.Windows.Forms.Button();
            btSaveList = new System.Windows.Forms.Button();
            gbListSelection = new System.Windows.Forms.GroupBox();
            rbImageFileList1 = new System.Windows.Forms.RadioButton();
            rbImageFileList2 = new System.Windows.Forms.RadioButton();
            rbImageFileErrorList = new System.Windows.Forms.RadioButton();
            rbImageFileListMatches = new System.Windows.Forms.RadioButton();
            dgvFileInfo = new System.Windows.Forms.DataGridView();
            colProperty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            lblStatus = new System.Windows.Forms.Label();
            lblCurrentIndex = new System.Windows.Forms.Label();
            tbGoToIndex = new System.Windows.Forms.TextBox();
            btGoToIndex = new System.Windows.Forms.Button();
            btCopyFIleList = new System.Windows.Forms.Button();
            cbMoveFiles = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)pbImage).BeginInit();
            gbListSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvFileInfo).BeginInit();
            SuspendLayout();
            // 
            // pbImage
            // 
            pbImage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pbImage.Location = new System.Drawing.Point(12, 120);
            pbImage.Name = "pbImage";
            pbImage.Size = new System.Drawing.Size(800, 500);
            pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pbImage.TabIndex = 0;
            pbImage.TabStop = false;
            // 
            // tbFilePath
            // 
            tbFilePath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbFilePath.Location = new System.Drawing.Point(12, 90);
            tbFilePath.Name = "tbFilePath";
            tbFilePath.ReadOnly = true;
            tbFilePath.Size = new System.Drawing.Size(1160, 23);
            tbFilePath.TabIndex = 1;
            // 
            // btPrevious
            // 
            btPrevious.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btPrevious.Location = new System.Drawing.Point(12, 630);
            btPrevious.Name = "btPrevious";
            btPrevious.Size = new System.Drawing.Size(100, 30);
            btPrevious.TabIndex = 2;
            btPrevious.Text = "<< Previous";
            btPrevious.UseVisualStyleBackColor = true;
            btPrevious.Click += btPrevious_Click;
            // 
            // btNext
            // 
            btNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btNext.Location = new System.Drawing.Point(118, 630);
            btNext.Name = "btNext";
            btNext.Size = new System.Drawing.Size(100, 30);
            btNext.TabIndex = 3;
            btNext.Text = "Next >>";
            btNext.UseVisualStyleBackColor = true;
            btNext.Click += btNext_Click;
            // 
            // btClose
            // 
            btClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btClose.Location = new System.Drawing.Point(1072, 630);
            btClose.Name = "btClose";
            btClose.Size = new System.Drawing.Size(100, 30);
            btClose.TabIndex = 4;
            btClose.Text = "Close";
            btClose.UseVisualStyleBackColor = true;
            btClose.Click += btClose_Click;
            // 
            // btSaveList
            // 
            btSaveList.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btSaveList.Location = new System.Drawing.Point(966, 630);
            btSaveList.Name = "btSaveList";
            btSaveList.Size = new System.Drawing.Size(100, 30);
            btSaveList.TabIndex = 5;
            btSaveList.Text = "Save List";
            btSaveList.UseVisualStyleBackColor = true;
            btSaveList.Click += btSaveList_Click;
            // 
            // gbListSelection
            // 
            gbListSelection.Controls.Add(rbImageFileList1);
            gbListSelection.Controls.Add(rbImageFileList2);
            gbListSelection.Controls.Add(rbImageFileErrorList);
            gbListSelection.Controls.Add(rbImageFileListMatches);
            gbListSelection.Location = new System.Drawing.Point(12, 12);
            gbListSelection.Name = "gbListSelection";
            gbListSelection.Size = new System.Drawing.Size(600, 70);
            gbListSelection.TabIndex = 6;
            gbListSelection.TabStop = false;
            gbListSelection.Text = "Select Image List";
            // 
            // rbImageFileList1
            // 
            rbImageFileList1.AutoSize = true;
            rbImageFileList1.Checked = true;
            rbImageFileList1.Location = new System.Drawing.Point(15, 25);
            rbImageFileList1.Name = "rbImageFileList1";
            rbImageFileList1.Size = new System.Drawing.Size(109, 19);
            rbImageFileList1.TabIndex = 0;
            rbImageFileList1.TabStop = true;
            rbImageFileList1.Text = "Image File List 1";
            rbImageFileList1.UseVisualStyleBackColor = true;
            rbImageFileList1.CheckedChanged += rbListSelection_CheckedChanged;
            // 
            // rbImageFileList2
            // 
            rbImageFileList2.AutoSize = true;
            rbImageFileList2.Location = new System.Drawing.Point(150, 25);
            rbImageFileList2.Name = "rbImageFileList2";
            rbImageFileList2.Size = new System.Drawing.Size(109, 19);
            rbImageFileList2.TabIndex = 1;
            rbImageFileList2.Text = "Image File List 2";
            rbImageFileList2.UseVisualStyleBackColor = true;
            rbImageFileList2.CheckedChanged += rbListSelection_CheckedChanged;
            // 
            // rbImageFileErrorList
            // 
            rbImageFileErrorList.AutoSize = true;
            rbImageFileErrorList.Location = new System.Drawing.Point(285, 25);
            rbImageFileErrorList.Name = "rbImageFileErrorList";
            rbImageFileErrorList.Size = new System.Drawing.Size(107, 19);
            rbImageFileErrorList.TabIndex = 2;
            rbImageFileErrorList.Text = "Image Error List";
            rbImageFileErrorList.UseVisualStyleBackColor = true;
            rbImageFileErrorList.CheckedChanged += rbListSelection_CheckedChanged;
            // 
            // rbImageFileListMatches
            // 
            rbImageFileListMatches.AutoSize = true;
            rbImageFileListMatches.Location = new System.Drawing.Point(15, 45);
            rbImageFileListMatches.Name = "rbImageFileListMatches";
            rbImageFileListMatches.Size = new System.Drawing.Size(127, 19);
            rbImageFileListMatches.TabIndex = 3;
            rbImageFileListMatches.Text = "Image Matches List";
            rbImageFileListMatches.UseVisualStyleBackColor = true;
            rbImageFileListMatches.CheckedChanged += rbListSelection_CheckedChanged;
            // 
            // dgvFileInfo
            // 
            dgvFileInfo.AllowUserToAddRows = false;
            dgvFileInfo.AllowUserToDeleteRows = false;
            dgvFileInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            dgvFileInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFileInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { colProperty, colValue });
            dgvFileInfo.Location = new System.Drawing.Point(820, 120);
            dgvFileInfo.Name = "dgvFileInfo";
            dgvFileInfo.ReadOnly = true;
            dgvFileInfo.RowHeadersVisible = false;
            dgvFileInfo.Size = new System.Drawing.Size(352, 500);
            dgvFileInfo.TabIndex = 7;
            // 
            // colProperty
            // 
            colProperty.HeaderText = "Property";
            colProperty.Name = "colProperty";
            colProperty.ReadOnly = true;
            colProperty.Width = 120;
            // 
            // colValue
            // 
            colValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            colValue.HeaderText = "Value";
            colValue.Name = "colValue";
            colValue.ReadOnly = true;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(224, 638);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(62, 15);
            lblStatus.TabIndex = 8;
            lblStatus.Text = "0 / 0 items";
            // 
            // lblCurrentIndex
            // 
            lblCurrentIndex.AutoSize = true;
            lblCurrentIndex.Location = new System.Drawing.Point(625, 18);
            lblCurrentIndex.Name = "lblCurrentIndex";
            lblCurrentIndex.Size = new System.Drawing.Size(49, 15);
            lblCurrentIndex.TabIndex = 9;
            lblCurrentIndex.Text = "Go to #:";
            // 
            // tbGoToIndex
            // 
            tbGoToIndex.Location = new System.Drawing.Point(625, 40);
            tbGoToIndex.Name = "tbGoToIndex";
            tbGoToIndex.Size = new System.Drawing.Size(100, 23);
            tbGoToIndex.TabIndex = 10;
            // 
            // btGoToIndex
            // 
            btGoToIndex.Location = new System.Drawing.Point(731, 38);
            btGoToIndex.Name = "btGoToIndex";
            btGoToIndex.Size = new System.Drawing.Size(75, 27);
            btGoToIndex.TabIndex = 11;
            btGoToIndex.Text = "Go";
            btGoToIndex.UseVisualStyleBackColor = true;
            btGoToIndex.Click += btGoToIndex_Click;
            // 
            // btCopyFIleList
            // 
            btCopyFIleList.Location = new System.Drawing.Point(873, 40);
            btCopyFIleList.Name = "btCopyFIleList";
            btCopyFIleList.Size = new System.Drawing.Size(75, 27);
            btCopyFIleList.TabIndex = 12;
            btCopyFIleList.Text = "Copy File List";
            btCopyFIleList.UseVisualStyleBackColor = true;
            btCopyFIleList.Click += btCopyFIleList_Click;
            // 
            // cbMoveFiles
            // 
            cbMoveFiles.AutoSize = true;
            cbMoveFiles.Location = new System.Drawing.Point(966, 44);
            cbMoveFiles.Name = "cbMoveFiles";
            cbMoveFiles.Size = new System.Drawing.Size(80, 19);
            cbMoveFiles.TabIndex = 13;
            cbMoveFiles.Text = "Move files";
            cbMoveFiles.UseVisualStyleBackColor = true;
            // 
            // DisplayImageList
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1184, 672);
            Controls.Add(cbMoveFiles);
            Controls.Add(btCopyFIleList);
            Controls.Add(btGoToIndex);
            Controls.Add(tbGoToIndex);
            Controls.Add(lblCurrentIndex);
            Controls.Add(lblStatus);
            Controls.Add(dgvFileInfo);
            Controls.Add(gbListSelection);
            Controls.Add(btSaveList);
            Controls.Add(btClose);
            Controls.Add(btNext);
            Controls.Add(btPrevious);
            Controls.Add(tbFilePath);
            Controls.Add(pbImage);
            Name = "DisplayImageList";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Display Image List";
            KeyDown += DisplayImageList_KeyDown;
            ((System.ComponentModel.ISupportInitialize)pbImage).EndInit();
            gbListSelection.ResumeLayout(false);
            gbListSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvFileInfo).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.TextBox tbFilePath;
        private System.Windows.Forms.Button btPrevious;
        private System.Windows.Forms.Button btNext;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.Button btSaveList;
        private System.Windows.Forms.GroupBox gbListSelection;
        private System.Windows.Forms.RadioButton rbImageFileList1;
        private System.Windows.Forms.RadioButton rbImageFileList2;
        private System.Windows.Forms.RadioButton rbImageFileErrorList;
        private System.Windows.Forms.RadioButton rbImageFileListMatches;
        private System.Windows.Forms.DataGridView dgvFileInfo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProperty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblCurrentIndex;
        private System.Windows.Forms.TextBox tbGoToIndex;
        private System.Windows.Forms.Button btGoToIndex;
        private System.Windows.Forms.Button btCopyFIleList;
        private System.Windows.Forms.CheckBox cbMoveFiles;
    }
}