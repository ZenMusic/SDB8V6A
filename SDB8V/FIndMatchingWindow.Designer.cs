namespace SymbolDB
{
    partial class FindMatchingWindow
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pbMatch = new System.Windows.Forms.PictureBox();
            btFindMatchingImage = new System.Windows.Forms.Button();
            lbMatches = new System.Windows.Forms.ListBox();
            btCancelFind = new System.Windows.Forms.Button();
            btDeleteMatchingImages = new System.Windows.Forms.Button();
            cbUseSameFolder = new System.Windows.Forms.CheckBox();
            tbSourceImageNumber = new System.Windows.Forms.TextBox();
            cbUseSize = new System.Windows.Forms.CheckBox();
            cbShowMatchPercentage = new System.Windows.Forms.CheckBox();
            cbToNewList = new System.Windows.Forms.CheckBox();
            cbSwapNewList = new System.Windows.Forms.CheckBox();
            tbSearchResult = new System.Windows.Forms.TextBox();
            tbFileName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)pbMatch).BeginInit();
            SuspendLayout();
            // 
            // pbMatch
            // 
            pbMatch.BackColor = System.Drawing.SystemColors.ControlDark;
            pbMatch.Location = new System.Drawing.Point(6, 0);
            pbMatch.MinimumSize = new System.Drawing.Size(18, 19);
            pbMatch.Name = "pbMatch";
            pbMatch.Size = new System.Drawing.Size(212, 143);
            pbMatch.TabIndex = 0;
            pbMatch.TabStop = false;
            pbMatch.Click += pbMatch_Click;
            // 
            // btFindMatchingImage
            // 
            btFindMatchingImage.Location = new System.Drawing.Point(12, 146);
            btFindMatchingImage.Name = "btFindMatchingImage";
            btFindMatchingImage.Size = new System.Drawing.Size(109, 26);
            btFindMatchingImage.TabIndex = 1;
            btFindMatchingImage.Text = "Find Matching";
            btFindMatchingImage.UseVisualStyleBackColor = true;
            btFindMatchingImage.Click += btFindMatchingImage_Click;
            // 
            // lbMatches
            // 
            lbMatches.FormattingEnabled = true;
            lbMatches.ItemHeight = 15;
            lbMatches.Location = new System.Drawing.Point(222, 0);
            lbMatches.Name = "lbMatches";
            lbMatches.Size = new System.Drawing.Size(528, 124);
            lbMatches.TabIndex = 2;
            lbMatches.SelectedIndexChanged += lbMatches_SelectedIndexChanged;
            lbMatches.MouseDoubleClick += lbMatches_MouseDoubleClick;
            // 
            // btCancelFind
            // 
            btCancelFind.Location = new System.Drawing.Point(248, 128);
            btCancelFind.Name = "btCancelFind";
            btCancelFind.Size = new System.Drawing.Size(54, 26);
            btCancelFind.TabIndex = 3;
            btCancelFind.Text = "Cancel";
            btCancelFind.UseVisualStyleBackColor = true;
            btCancelFind.Click += btCancelFind_Click;
            // 
            // btDeleteMatchingImages
            // 
            btDeleteMatchingImages.Location = new System.Drawing.Point(387, 128);
            btDeleteMatchingImages.Name = "btDeleteMatchingImages";
            btDeleteMatchingImages.Size = new System.Drawing.Size(130, 26);
            btDeleteMatchingImages.TabIndex = 4;
            btDeleteMatchingImages.Text = "Delete Matching";
            btDeleteMatchingImages.UseVisualStyleBackColor = true;
            btDeleteMatchingImages.Click += btDeleteMatchingImages_Click;
            // 
            // cbUseSameFolder
            // 
            cbUseSameFolder.AutoSize = true;
            cbUseSameFolder.Location = new System.Drawing.Point(127, 149);
            cbUseSameFolder.Name = "cbUseSameFolder";
            cbUseSameFolder.Size = new System.Drawing.Size(78, 19);
            cbUseSameFolder.TabIndex = 5;
            cbUseSameFolder.Text = "use same ";
            cbUseSameFolder.UseVisualStyleBackColor = true;
            // 
            // tbSourceImageNumber
            // 
            tbSourceImageNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
            tbSourceImageNumber.Location = new System.Drawing.Point(222, 158);
            tbSourceImageNumber.Name = "tbSourceImageNumber";
            tbSourceImageNumber.Size = new System.Drawing.Size(300, 19);
            tbSourceImageNumber.TabIndex = 6;
            // 
            // cbUseSize
            // 
            cbUseSize.AutoSize = true;
            cbUseSize.Location = new System.Drawing.Point(127, 168);
            cbUseSize.Name = "cbUseSize";
            cbUseSize.Size = new System.Drawing.Size(66, 19);
            cbUseSize.TabIndex = 7;
            cbUseSize.Text = "use size";
            cbUseSize.UseVisualStyleBackColor = true;
            // 
            // cbShowMatchPercentage
            // 
            cbShowMatchPercentage.AutoSize = true;
            cbShowMatchPercentage.Location = new System.Drawing.Point(311, 131);
            cbShowMatchPercentage.Name = "cbShowMatchPercentage";
            cbShowMatchPercentage.Size = new System.Drawing.Size(70, 19);
            cbShowMatchPercentage.TabIndex = 8;
            cbShowMatchPercentage.Text = "match%";
            cbShowMatchPercentage.UseVisualStyleBackColor = true;
            cbShowMatchPercentage.CheckedChanged += cbShowMatchPercentage_CheckedChanged;
            // 
            // cbToNewList
            // 
            cbToNewList.AutoSize = true;
            cbToNewList.BackColor = System.Drawing.SystemColors.Control;
            cbToNewList.Location = new System.Drawing.Point(33, 174);
            cbToNewList.Name = "cbToNewList";
            cbToNewList.Size = new System.Drawing.Size(82, 19);
            cbToNewList.TabIndex = 9;
            cbToNewList.Text = "to New Lst";
            cbToNewList.UseVisualStyleBackColor = false;
            // 
            // cbSwapNewList
            // 
            cbSwapNewList.AutoSize = true;
            cbSwapNewList.Enabled = false;
            cbSwapNewList.Location = new System.Drawing.Point(12, 196);
            cbSwapNewList.Name = "cbSwapNewList";
            cbSwapNewList.Size = new System.Drawing.Size(110, 19);
            cbSwapNewList.TabIndex = 10;
            cbSwapNewList.Text = "Swap Result List";
            cbSwapNewList.UseVisualStyleBackColor = true;
            cbSwapNewList.CheckedChanged += cbSwapNewList_CheckedChanged;
            // 
            // tbSearchResult
            // 
            tbSearchResult.Location = new System.Drawing.Point(127, 192);
            tbSearchResult.Name = "tbSearchResult";
            tbSearchResult.Size = new System.Drawing.Size(395, 23);
            tbSearchResult.TabIndex = 11;
            tbSearchResult.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbFileName
            // 
            tbFileName.Location = new System.Drawing.Point(33, 221);
            tbFileName.Name = "tbFileName";
            tbFileName.Size = new System.Drawing.Size(673, 23);
            tbFileName.TabIndex = 12;
            tbFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FindMatchingWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(752, 247);
            Controls.Add(tbFileName);
            Controls.Add(tbSearchResult);
            Controls.Add(cbSwapNewList);
            Controls.Add(cbToNewList);
            Controls.Add(cbShowMatchPercentage);
            Controls.Add(cbUseSize);
            Controls.Add(tbSourceImageNumber);
            Controls.Add(cbUseSameFolder);
            Controls.Add(btDeleteMatchingImages);
            Controls.Add(btCancelFind);
            Controls.Add(lbMatches);
            Controls.Add(btFindMatchingImage);
            Controls.Add(pbMatch);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Name = "FindMatchingWindow";
            Text = "Find Matching Images";
            ((System.ComponentModel.ISupportInitialize)pbMatch).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox pbMatch;
        private System.Windows.Forms.Button btFindMatchingImage;
        private System.Windows.Forms.ListBox lbMatches;
        private System.Windows.Forms.Button btCancelFind;
        private System.Windows.Forms.Button btDeleteMatchingImages;
        private System.Windows.Forms.CheckBox cbUseSameFolder;
        private System.Windows.Forms.TextBox tbSourceImageNumber;
        private System.Windows.Forms.CheckBox cbUseSize;
        private System.Windows.Forms.CheckBox cbShowMatchPercentage;
        private System.Windows.Forms.CheckBox cbToNewList;
        private System.Windows.Forms.CheckBox cbSwapNewList;
        private System.Windows.Forms.TextBox tbSearchResult;
        private System.Windows.Forms.TextBox tbFileName;
    }
}