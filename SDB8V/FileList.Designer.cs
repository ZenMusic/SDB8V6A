namespace SymbolDB
{
    partial class FileList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            dgv1 = new System.Windows.Forms.DataGridView();
            btVideos = new System.Windows.Forms.Button();
            btImages = new System.Windows.Forms.Button();
            tbCount = new System.Windows.Forms.TextBox();
            cbTopMost = new System.Windows.Forms.CheckBox();
            tbFileName = new System.Windows.Forms.TextBox();
            btSaveDeleteList = new System.Windows.Forms.Button();
            tbRowId = new System.Windows.Forms.TextBox();
            tbLoad = new System.Windows.Forms.Button();
            btClose = new System.Windows.Forms.Button();
            btCreateNewFileList = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)dgv1).BeginInit();
            SuspendLayout();
            // 
            // dgv1
            // 
            dgv1.AllowUserToOrderColumns = true;
            dgv1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dgv1.DefaultCellStyle = dataGridViewCellStyle3;
            dgv1.Location = new System.Drawing.Point(13, 13);
            dgv1.Margin = new System.Windows.Forms.Padding(2);
            dgv1.MultiSelect = false;
            dgv1.Name = "dgv1";
            dgv1.RowHeadersVisible = false;
            dgv1.RowHeadersWidth = 51;
            dgv1.RowTemplate.Height = 24;
            dgv1.Size = new System.Drawing.Size(1334, 631);
            dgv1.TabIndex = 59;
            dgv1.SelectionChanged += dgv1_SelectionChanged;
            // 
            // btVideos
            // 
            btVideos.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btVideos.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            btVideos.Location = new System.Drawing.Point(261, 676);
            btVideos.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btVideos.Name = "btVideos";
            btVideos.Size = new System.Drawing.Size(54, 27);
            btVideos.TabIndex = 138;
            btVideos.Text = "Video";
            btVideos.UseVisualStyleBackColor = false;
            // 
            // btImages
            // 
            btImages.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btImages.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            btImages.Location = new System.Drawing.Point(341, 676);
            btImages.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btImages.Name = "btImages";
            btImages.Size = new System.Drawing.Size(54, 27);
            btImages.TabIndex = 143;
            btImages.Text = "Images";
            btImages.UseVisualStyleBackColor = false;
            // 
            // tbCount
            // 
            tbCount.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            tbCount.Location = new System.Drawing.Point(425, 678);
            tbCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tbCount.Name = "tbCount";
            tbCount.Size = new System.Drawing.Size(69, 23);
            tbCount.TabIndex = 144;
            // 
            // cbTopMost
            // 
            cbTopMost.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            cbTopMost.AutoSize = true;
            cbTopMost.Location = new System.Drawing.Point(120, 677);
            cbTopMost.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbTopMost.Name = "cbTopMost";
            cbTopMost.Size = new System.Drawing.Size(76, 19);
            cbTopMost.TabIndex = 145;
            cbTopMost.Text = "Top Most";
            cbTopMost.UseVisualStyleBackColor = true;
            cbTopMost.CheckedChanged += cbTopMost_CheckedChanged;
            // 
            // tbFileName
            // 
            tbFileName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            tbFileName.Location = new System.Drawing.Point(84, 720);
            tbFileName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tbFileName.Name = "tbFileName";
            tbFileName.Size = new System.Drawing.Size(621, 23);
            tbFileName.TabIndex = 146;
            // 
            // btSaveDeleteList
            // 
            btSaveDeleteList.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btSaveDeleteList.Location = new System.Drawing.Point(596, 670);
            btSaveDeleteList.Margin = new System.Windows.Forms.Padding(2);
            btSaveDeleteList.Name = "btSaveDeleteList";
            btSaveDeleteList.Size = new System.Drawing.Size(83, 25);
            btSaveDeleteList.TabIndex = 254;
            btSaveDeleteList.Text = "save D list";
            btSaveDeleteList.UseVisualStyleBackColor = true;
            btSaveDeleteList.Click += btSave_Click;
            // 
            // tbRowId
            // 
            tbRowId.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            tbRowId.Location = new System.Drawing.Point(775, 673);
            tbRowId.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tbRowId.Name = "tbRowId";
            tbRowId.Size = new System.Drawing.Size(69, 23);
            tbRowId.TabIndex = 255;
            // 
            // tbLoad
            // 
            tbLoad.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            tbLoad.Location = new System.Drawing.Point(1085, 677);
            tbLoad.Margin = new System.Windows.Forms.Padding(2);
            tbLoad.Name = "tbLoad";
            tbLoad.Size = new System.Drawing.Size(83, 25);
            tbLoad.TabIndex = 256;
            tbLoad.Text = "load";
            tbLoad.UseVisualStyleBackColor = true;
            tbLoad.Click += tbLoad_Click;
            // 
            // btClose
            // 
            btClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btClose.Location = new System.Drawing.Point(1195, 678);
            btClose.Margin = new System.Windows.Forms.Padding(2);
            btClose.Name = "btClose";
            btClose.Size = new System.Drawing.Size(83, 25);
            btClose.TabIndex = 257;
            btClose.Text = "close";
            btClose.UseVisualStyleBackColor = true;
            btClose.Click += btClose_Click;
            // 
            // btCreateNewFileList
            // 
            btCreateNewFileList.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btCreateNewFileList.Location = new System.Drawing.Point(935, 678);
            btCreateNewFileList.Margin = new System.Windows.Forms.Padding(2);
            btCreateNewFileList.Name = "btCreateNewFileList";
            btCreateNewFileList.Size = new System.Drawing.Size(105, 25);
            btCreateNewFileList.TabIndex = 258;
            btCreateNewFileList.Text = "create new";
            btCreateNewFileList.UseVisualStyleBackColor = true;
            btCreateNewFileList.Click += btCreateNewFileList_Click;
            // 
            // FileList
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1359, 744);
            Controls.Add(btCreateNewFileList);
            Controls.Add(btClose);
            Controls.Add(tbLoad);
            Controls.Add(tbRowId);
            Controls.Add(btSaveDeleteList);
            Controls.Add(tbFileName);
            Controls.Add(cbTopMost);
            Controls.Add(tbCount);
            Controls.Add(btImages);
            Controls.Add(btVideos);
            Controls.Add(dgv1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "FileList";
            Text = "FileList";
            ((System.ComponentModel.ISupportInitialize)dgv1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.Button btVideos;
        private System.Windows.Forms.Button btImages;
        private System.Windows.Forms.TextBox tbCount;
        private System.Windows.Forms.CheckBox cbTopMost;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.Button btSaveDeleteList;
        private System.Windows.Forms.TextBox tbRowId;
        private System.Windows.Forms.Button tbLoad;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.Button btCreateNewFileList;
    }
}