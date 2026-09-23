namespace SymbolDB
{
    partial class SearchForMediaByName
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            dgv1 = new System.Windows.Forms.DataGridView();
            tbDirectoryPath = new System.Windows.Forms.TextBox();
            btClose = new System.Windows.Forms.Button();
            tbMainOrSubWindow = new System.Windows.Forms.TextBox();
            btSearchWin2 = new System.Windows.Forms.Button();
            tbFound2Folder = new System.Windows.Forms.TextBox();
            tbFoundFileFolder = new System.Windows.Forms.TextBox();
            tbFolderName = new System.Windows.Forms.TextBox();
            tbWin2FileSizeMatchSearch = new System.Windows.Forms.TextBox();
            tbFileNameSelected = new System.Windows.Forms.TextBox();
            cbSearchPartial = new System.Windows.Forms.CheckBox();
            cbSearchForLenMatch = new System.Windows.Forms.CheckBox();
            cbLoadedAndReady = new System.Windows.Forms.CheckBox();
            tbMovieFpath = new System.Windows.Forms.TextBox();
            tbStatusFoundInWin2 = new System.Windows.Forms.TextBox();
            tbCount = new System.Windows.Forms.TextBox();
            tbRowId = new System.Windows.Forms.TextBox();
            tbFileLength = new System.Windows.Forms.TextBox();
            btTraversing = new System.Windows.Forms.Button();
            btCancel = new System.Windows.Forms.Button();
            tbCopyFileName = new System.Windows.Forms.TextBox();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            btVideos = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            btValidate = new System.Windows.Forms.Button();
            btImages = new System.Windows.Forms.Button();
            tbFoundSeachList2b = new System.Windows.Forms.TextBox();
            tbFoundSeachList2 = new System.Windows.Forms.TextBox();
            tbTotalCountFound = new System.Windows.Forms.TextBox();
            tbTotalSpace = new System.Windows.Forms.TextBox();
            btGetCount = new System.Windows.Forms.Button();
            btView = new System.Windows.Forms.Button();
            btViewEnding = new System.Windows.Forms.Button();
            tbDuration = new System.Windows.Forms.TextBox();
            tbPlayerState = new System.Windows.Forms.TextBox();
            tbPosition = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)dgv1).BeginInit();
            SuspendLayout();
            // 
            // dgv1
            // 
            dgv1.AllowUserToOrderColumns = true;
            dgv1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgv1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dgv1.DefaultCellStyle = dataGridViewCellStyle2;
            dgv1.Location = new System.Drawing.Point(0, 146);
            dgv1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            dgv1.MultiSelect = false;
            dgv1.Name = "dgv1";
            dgv1.RowHeadersVisible = false;
            dgv1.RowHeadersWidth = 51;
            dgv1.RowTemplate.Height = 24;
            dgv1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgv1.Size = new System.Drawing.Size(2058, 194);
            dgv1.TabIndex = 59;
            dgv1.SelectionChanged += dgv1_SelectionChanged;
            // 
            // tbDirectoryPath
            // 
            tbDirectoryPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbDirectoryPath.Location = new System.Drawing.Point(156, 16);
            tbDirectoryPath.Margin = new System.Windows.Forms.Padding(4);
            tbDirectoryPath.Name = "tbDirectoryPath";
            tbDirectoryPath.Size = new System.Drawing.Size(490, 23);
            tbDirectoryPath.TabIndex = 98;
            // 
            // btClose
            // 
            btClose.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            btClose.Location = new System.Drawing.Point(1912, 10);
            btClose.Margin = new System.Windows.Forms.Padding(4);
            btClose.Name = "btClose";
            btClose.Size = new System.Drawing.Size(66, 26);
            btClose.TabIndex = 287;
            btClose.Text = "CLOSE";
            btClose.UseVisualStyleBackColor = false;
            btClose.Click += btClose_Click;
            // 
            // tbMainOrSubWindow
            // 
            tbMainOrSubWindow.Location = new System.Drawing.Point(756, 13);
            tbMainOrSubWindow.Margin = new System.Windows.Forms.Padding(4);
            tbMainOrSubWindow.Name = "tbMainOrSubWindow";
            tbMainOrSubWindow.Size = new System.Drawing.Size(137, 23);
            tbMainOrSubWindow.TabIndex = 286;
            // 
            // btSearchWin2
            // 
            btSearchWin2.Location = new System.Drawing.Point(26, 40);
            btSearchWin2.Margin = new System.Windows.Forms.Padding(4);
            btSearchWin2.Name = "btSearchWin2";
            btSearchWin2.Size = new System.Drawing.Size(138, 30);
            btSearchWin2.TabIndex = 285;
            btSearchWin2.Text = "Search1";
            btSearchWin2.UseVisualStyleBackColor = true;
            btSearchWin2.Click += btSearchWin2_Click;
            // 
            // tbFound2Folder
            // 
            tbFound2Folder.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbFound2Folder.Location = new System.Drawing.Point(507, 75);
            tbFound2Folder.Margin = new System.Windows.Forms.Padding(4);
            tbFound2Folder.Name = "tbFound2Folder";
            tbFound2Folder.Size = new System.Drawing.Size(490, 23);
            tbFound2Folder.TabIndex = 288;
            // 
            // tbFoundFileFolder
            // 
            tbFoundFileFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbFoundFileFolder.Location = new System.Drawing.Point(324, 103);
            tbFoundFileFolder.Margin = new System.Windows.Forms.Padding(4);
            tbFoundFileFolder.Name = "tbFoundFileFolder";
            tbFoundFileFolder.Size = new System.Drawing.Size(738, 23);
            tbFoundFileFolder.TabIndex = 289;
            // 
            // tbFolderName
            // 
            tbFolderName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbFolderName.Location = new System.Drawing.Point(1005, 71);
            tbFolderName.Margin = new System.Windows.Forms.Padding(4);
            tbFolderName.Name = "tbFolderName";
            tbFolderName.Size = new System.Drawing.Size(70, 26);
            tbFolderName.TabIndex = 290;
            // 
            // tbWin2FileSizeMatchSearch
            // 
            tbWin2FileSizeMatchSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbWin2FileSizeMatchSearch.Location = new System.Drawing.Point(156, 103);
            tbWin2FileSizeMatchSearch.Margin = new System.Windows.Forms.Padding(4);
            tbWin2FileSizeMatchSearch.Name = "tbWin2FileSizeMatchSearch";
            tbWin2FileSizeMatchSearch.Size = new System.Drawing.Size(160, 26);
            tbWin2FileSizeMatchSearch.TabIndex = 291;
            // 
            // tbFileNameSelected
            // 
            tbFileNameSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbFileNameSelected.Location = new System.Drawing.Point(1095, 103);
            tbFileNameSelected.Margin = new System.Windows.Forms.Padding(4);
            tbFileNameSelected.Name = "tbFileNameSelected";
            tbFileNameSelected.Size = new System.Drawing.Size(322, 26);
            tbFileNameSelected.TabIndex = 292;
            // 
            // cbSearchPartial
            // 
            cbSearchPartial.AutoSize = true;
            cbSearchPartial.Location = new System.Drawing.Point(12, 80);
            cbSearchPartial.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            cbSearchPartial.Name = "cbSearchPartial";
            cbSearchPartial.Size = new System.Drawing.Size(97, 19);
            cbSearchPartial.TabIndex = 293;
            cbSearchPartial.Text = "Search Partial";
            cbSearchPartial.UseVisualStyleBackColor = true;
            // 
            // cbSearchForLenMatch
            // 
            cbSearchForLenMatch.AutoSize = true;
            cbSearchForLenMatch.Location = new System.Drawing.Point(12, 103);
            cbSearchForLenMatch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            cbSearchForLenMatch.Name = "cbSearchForLenMatch";
            cbSearchForLenMatch.Size = new System.Drawing.Size(47, 19);
            cbSearchForLenMatch.TabIndex = 294;
            cbSearchForLenMatch.Text = "LEN";
            cbSearchForLenMatch.UseVisualStyleBackColor = true;
            // 
            // cbLoadedAndReady
            // 
            cbLoadedAndReady.AutoSize = true;
            cbLoadedAndReady.Location = new System.Drawing.Point(12, 124);
            cbLoadedAndReady.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            cbLoadedAndReady.Name = "cbLoadedAndReady";
            cbLoadedAndReady.Size = new System.Drawing.Size(123, 19);
            cbLoadedAndReady.TabIndex = 356;
            cbLoadedAndReady.Text = "Loaded and Ready";
            cbLoadedAndReady.UseVisualStyleBackColor = true;
            // 
            // tbMovieFpath
            // 
            tbMovieFpath.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            tbMovieFpath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbMovieFpath.Location = new System.Drawing.Point(516, 46);
            tbMovieFpath.Margin = new System.Windows.Forms.Padding(4);
            tbMovieFpath.Name = "tbMovieFpath";
            tbMovieFpath.Size = new System.Drawing.Size(862, 22);
            tbMovieFpath.TabIndex = 355;
            // 
            // tbStatusFoundInWin2
            // 
            tbStatusFoundInWin2.Location = new System.Drawing.Point(198, 47);
            tbStatusFoundInWin2.Margin = new System.Windows.Forms.Padding(4);
            tbStatusFoundInWin2.Name = "tbStatusFoundInWin2";
            tbStatusFoundInWin2.Size = new System.Drawing.Size(287, 23);
            tbStatusFoundInWin2.TabIndex = 357;
            // 
            // tbCount
            // 
            tbCount.Location = new System.Drawing.Point(198, 72);
            tbCount.Margin = new System.Windows.Forms.Padding(4);
            tbCount.Name = "tbCount";
            tbCount.Size = new System.Drawing.Size(137, 23);
            tbCount.TabIndex = 358;
            // 
            // tbRowId
            // 
            tbRowId.Location = new System.Drawing.Point(348, 76);
            tbRowId.Margin = new System.Windows.Forms.Padding(4);
            tbRowId.Name = "tbRowId";
            tbRowId.Size = new System.Drawing.Size(137, 23);
            tbRowId.TabIndex = 359;
            // 
            // tbFileLength
            // 
            tbFileLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbFileLength.Location = new System.Drawing.Point(1437, 103);
            tbFileLength.Margin = new System.Windows.Forms.Padding(4);
            tbFileLength.Name = "tbFileLength";
            tbFileLength.Size = new System.Drawing.Size(160, 26);
            tbFileLength.TabIndex = 360;
            // 
            // btTraversing
            // 
            btTraversing.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            btTraversing.Location = new System.Drawing.Point(1399, 1);
            btTraversing.Margin = new System.Windows.Forms.Padding(4);
            btTraversing.Name = "btTraversing";
            btTraversing.Size = new System.Drawing.Size(162, 50);
            btTraversing.TabIndex = 361;
            btTraversing.Text = "Traversing";
            btTraversing.UseVisualStyleBackColor = false;
            // 
            // btCancel
            // 
            btCancel.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            btCancel.Location = new System.Drawing.Point(1569, 15);
            btCancel.Margin = new System.Windows.Forms.Padding(4);
            btCancel.Name = "btCancel";
            btCancel.Size = new System.Drawing.Size(66, 26);
            btCancel.TabIndex = 362;
            btCancel.Text = "Cancel";
            btCancel.UseVisualStyleBackColor = false;
            // 
            // tbCopyFileName
            // 
            tbCopyFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbCopyFileName.Location = new System.Drawing.Point(1471, 68);
            tbCopyFileName.Margin = new System.Windows.Forms.Padding(4);
            tbCopyFileName.Name = "tbCopyFileName";
            tbCopyFileName.Size = new System.Drawing.Size(160, 26);
            tbCopyFileName.TabIndex = 363;
            // 
            // progressBar1
            // 
            progressBar1.Location = new System.Drawing.Point(1656, 80);
            progressBar1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(142, 22);
            progressBar1.TabIndex = 376;
            // 
            // btVideos
            // 
            btVideos.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            btVideos.Location = new System.Drawing.Point(1656, 44);
            btVideos.Margin = new System.Windows.Forms.Padding(4);
            btVideos.Name = "btVideos";
            btVideos.Size = new System.Drawing.Size(66, 26);
            btVideos.TabIndex = 377;
            btVideos.Text = "videos";
            btVideos.UseVisualStyleBackColor = false;
            btVideos.Click += btVideos_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(69, 21);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(60, 15);
            label1.TabIndex = 378;
            label1.Text = "Base Foler";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(1399, 71);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(60, 15);
            label2.TabIndex = 379;
            label2.Text = "File Name";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(255, 158);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(0, 15);
            label3.TabIndex = 380;
            // 
            // btValidate
            // 
            btValidate.Location = new System.Drawing.Point(0, 0);
            btValidate.Name = "btValidate";
            btValidate.Size = new System.Drawing.Size(75, 23);
            btValidate.TabIndex = 1;
            // 
            // btImages
            // 
            btImages.Location = new System.Drawing.Point(0, 0);
            btImages.Name = "btImages";
            btImages.Size = new System.Drawing.Size(75, 23);
            btImages.TabIndex = 0;
            // 
            // tbFoundSeachList2b
            // 
            tbFoundSeachList2b.Location = new System.Drawing.Point(1095, 18);
            tbFoundSeachList2b.Name = "tbFoundSeachList2b";
            tbFoundSeachList2b.Size = new System.Drawing.Size(100, 23);
            tbFoundSeachList2b.TabIndex = 381;
            // 
            // tbFoundSeachList2
            // 
            tbFoundSeachList2.Location = new System.Drawing.Point(1223, 16);
            tbFoundSeachList2.Name = "tbFoundSeachList2";
            tbFoundSeachList2.Size = new System.Drawing.Size(100, 23);
            tbFoundSeachList2.TabIndex = 382;
            // 
            // tbTotalCountFound
            // 
            tbTotalCountFound.Location = new System.Drawing.Point(1841, 71);
            tbTotalCountFound.Margin = new System.Windows.Forms.Padding(4);
            tbTotalCountFound.Name = "tbTotalCountFound";
            tbTotalCountFound.Size = new System.Drawing.Size(137, 23);
            tbTotalCountFound.TabIndex = 383;
            // 
            // tbTotalSpace
            // 
            tbTotalSpace.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbTotalSpace.Location = new System.Drawing.Point(1841, 114);
            tbTotalSpace.Margin = new System.Windows.Forms.Padding(4);
            tbTotalSpace.Name = "tbTotalSpace";
            tbTotalSpace.Size = new System.Drawing.Size(160, 26);
            tbTotalSpace.TabIndex = 384;
            // 
            // btGetCount
            // 
            btGetCount.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            btGetCount.Location = new System.Drawing.Point(1730, 44);
            btGetCount.Margin = new System.Windows.Forms.Padding(4);
            btGetCount.Name = "btGetCount";
            btGetCount.Size = new System.Drawing.Size(102, 26);
            btGetCount.TabIndex = 385;
            btGetCount.Text = "Get Count";
            btGetCount.UseVisualStyleBackColor = false;
            btGetCount.Click += btGetCount_Click;
            // 
            // btView
            // 
            btView.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            btView.Location = new System.Drawing.Point(1674, 10);
            btView.Margin = new System.Windows.Forms.Padding(4);
            btView.Name = "btView";
            btView.Size = new System.Drawing.Size(66, 26);
            btView.TabIndex = 386;
            btView.Text = "View";
            btView.UseVisualStyleBackColor = false;
            btView.Click += btView_Click;
            // 
            // btViewEnding
            // 
            btViewEnding.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            btViewEnding.Location = new System.Drawing.Point(1766, 6);
            btViewEnding.Margin = new System.Windows.Forms.Padding(4);
            btViewEnding.Name = "btViewEnding";
            btViewEnding.Size = new System.Drawing.Size(66, 26);
            btViewEnding.TabIndex = 387;
            btViewEnding.Text = "End";
            btViewEnding.UseVisualStyleBackColor = false;
            btViewEnding.Click += btViewEnding_Click;
            // 
            // tbDuration
            // 
            tbDuration.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbDuration.Location = new System.Drawing.Point(1312, 72);
            tbDuration.Margin = new System.Windows.Forms.Padding(4);
            tbDuration.Name = "tbDuration";
            tbDuration.Size = new System.Drawing.Size(70, 26);
            tbDuration.TabIndex = 388;
            tbDuration.Text = "dur";
            // 
            // tbPlayerState
            // 
            tbPlayerState.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbPlayerState.Location = new System.Drawing.Point(1135, 72);
            tbPlayerState.Margin = new System.Windows.Forms.Padding(4);
            tbPlayerState.Name = "tbPlayerState";
            tbPlayerState.Size = new System.Drawing.Size(84, 26);
            tbPlayerState.TabIndex = 389;
            tbPlayerState.Text = "state";
            // 
            // tbPosition
            // 
            tbPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbPosition.Location = new System.Drawing.Point(1229, 72);
            tbPosition.Margin = new System.Windows.Forms.Padding(4);
            tbPosition.Name = "tbPosition";
            tbPosition.Size = new System.Drawing.Size(70, 26);
            tbPosition.TabIndex = 390;
            tbPosition.Text = "time";
            // 
            // SearchForMediaByName
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(2063, 339);
            Controls.Add(tbPosition);
            Controls.Add(tbPlayerState);
            Controls.Add(tbDuration);
            Controls.Add(btViewEnding);
            Controls.Add(btView);
            Controls.Add(btGetCount);
            Controls.Add(tbTotalSpace);
            Controls.Add(tbTotalCountFound);
            Controls.Add(tbFoundSeachList2);
            Controls.Add(tbFoundSeachList2b);
            Controls.Add(btImages);
            Controls.Add(btValidate);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btVideos);
            Controls.Add(progressBar1);
            Controls.Add(tbCopyFileName);
            Controls.Add(btCancel);
            Controls.Add(btTraversing);
            Controls.Add(tbFileLength);
            Controls.Add(tbRowId);
            Controls.Add(tbCount);
            Controls.Add(tbStatusFoundInWin2);
            Controls.Add(cbLoadedAndReady);
            Controls.Add(tbMovieFpath);
            Controls.Add(cbSearchForLenMatch);
            Controls.Add(cbSearchPartial);
            Controls.Add(tbFileNameSelected);
            Controls.Add(tbWin2FileSizeMatchSearch);
            Controls.Add(tbFolderName);
            Controls.Add(tbFoundFileFolder);
            Controls.Add(tbFound2Folder);
            Controls.Add(btClose);
            Controls.Add(tbMainOrSubWindow);
            Controls.Add(btSearchWin2);
            Controls.Add(tbDirectoryPath);
            Controls.Add(dgv1);
            Name = "SearchForMediaByName";
            Text = "SearchForMediaByName";
            FormClosing += SearchForMediaByName_FormClosing;
            ((System.ComponentModel.ISupportInitialize)dgv1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.TextBox tbDirectoryPath;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.TextBox tbMainOrSubWindow;
        private System.Windows.Forms.Button btSearchWin2;
        private System.Windows.Forms.TextBox tbCount;
        private System.Windows.Forms.TextBox tbFound2Folder;
        private System.Windows.Forms.TextBox tbFolderName;
        private System.Windows.Forms.TextBox tbFoundFileFolder;
        private System.Windows.Forms.TextBox tbWin2FileSizeMatchSearch;
        private System.Windows.Forms.TextBox tbFileNameSelected;
        private System.Windows.Forms.CheckBox cbSearchPartial;
        private System.Windows.Forms.CheckBox cbSearchForLenMatch;
        private System.Windows.Forms.CheckBox cbLoadedAndReady;
        private System.Windows.Forms.TextBox tbMovieFpath;
        private System.Windows.Forms.TextBox tbStatusFoundInWin2;
        private System.Windows.Forms.TextBox tbRowId;
        private System.Windows.Forms.TextBox tbFileLength;
        private System.Windows.Forms.Button btTraversing;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.TextBox tbCopyFileName;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btVideos;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btValidate;
        private System.Windows.Forms.Button btImages;
        private System.Windows.Forms.TextBox tbFoundSeachList2xb;
        private System.Windows.Forms.TextBox tbFoundSeachList2;
        private System.Windows.Forms.TextBox tbTotalCountFound;
        private System.Windows.Forms.TextBox tbTotalSpace;
        private System.Windows.Forms.Button btGetCount;
        private System.Windows.Forms.Button btView;
        private System.Windows.Forms.Button btViewEnding;
        private System.Windows.Forms.TextBox tbDuration;
        private System.Windows.Forms.TextBox tbPlayerState;
        private System.Windows.Forms.TextBox tbPosition;
    }
}