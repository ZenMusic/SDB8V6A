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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv1 = new System.Windows.Forms.DataGridView();
            this.btVideos = new System.Windows.Forms.Button();
            this.btImages = new System.Windows.Forms.Button();
            this.tbCount = new System.Windows.Forms.TextBox();
            this.cbTopMost = new System.Windows.Forms.CheckBox();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.btSaveDeleteList = new System.Windows.Forms.Button();
            this.tbRowId = new System.Windows.Forms.TextBox();
            this.tbLoad = new System.Windows.Forms.Button();
            this.btClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv1
            // 
            this.dgv1.AllowUserToOrderColumns = true;
            this.dgv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgv1.Location = new System.Drawing.Point(11, 11);
            this.dgv1.Margin = new System.Windows.Forms.Padding(2);
            this.dgv1.MultiSelect = false;
            this.dgv1.Name = "dgv1";
            this.dgv1.RowHeadersVisible = false;
            this.dgv1.RowHeadersWidth = 51;
            this.dgv1.RowTemplate.Height = 24;
            this.dgv1.Size = new System.Drawing.Size(1143, 547);
            this.dgv1.TabIndex = 59;
            this.dgv1.SelectionChanged += new System.EventHandler(this.dgv1_SelectionChanged);
            // 
            // btVideos
            // 
            this.btVideos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btVideos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btVideos.Location = new System.Drawing.Point(224, 586);
            this.btVideos.Name = "btVideos";
            this.btVideos.Size = new System.Drawing.Size(46, 23);
            this.btVideos.TabIndex = 138;
            this.btVideos.Text = "Video";
            this.btVideos.UseVisualStyleBackColor = false;
            // 
            // btImages
            // 
            this.btImages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btImages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btImages.Location = new System.Drawing.Point(292, 586);
            this.btImages.Name = "btImages";
            this.btImages.Size = new System.Drawing.Size(46, 23);
            this.btImages.TabIndex = 143;
            this.btImages.Text = "Images";
            this.btImages.UseVisualStyleBackColor = false;
            // 
            // tbCount
            // 
            this.tbCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbCount.Location = new System.Drawing.Point(364, 588);
            this.tbCount.Name = "tbCount";
            this.tbCount.Size = new System.Drawing.Size(60, 20);
            this.tbCount.TabIndex = 144;
            // 
            // cbTopMost
            // 
            this.cbTopMost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbTopMost.AutoSize = true;
            this.cbTopMost.Location = new System.Drawing.Point(103, 586);
            this.cbTopMost.Name = "cbTopMost";
            this.cbTopMost.Size = new System.Drawing.Size(71, 17);
            this.cbTopMost.TabIndex = 145;
            this.cbTopMost.Text = "Top Most";
            this.cbTopMost.UseVisualStyleBackColor = true;
            this.cbTopMost.CheckedChanged += new System.EventHandler(this.cbTopMost_CheckedChanged);
            // 
            // tbFileName
            // 
            this.tbFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbFileName.Location = new System.Drawing.Point(72, 624);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(533, 20);
            this.tbFileName.TabIndex = 146;
            // 
            // btSaveDeleteList
            // 
            this.btSaveDeleteList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btSaveDeleteList.Location = new System.Drawing.Point(511, 581);
            this.btSaveDeleteList.Margin = new System.Windows.Forms.Padding(2);
            this.btSaveDeleteList.Name = "btSaveDeleteList";
            this.btSaveDeleteList.Size = new System.Drawing.Size(71, 22);
            this.btSaveDeleteList.TabIndex = 254;
            this.btSaveDeleteList.Text = "save D list";
            this.btSaveDeleteList.UseVisualStyleBackColor = true;
            this.btSaveDeleteList.Click += new System.EventHandler(this.btSave_Click);
            // 
            // tbRowId
            // 
            this.tbRowId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbRowId.Location = new System.Drawing.Point(664, 583);
            this.tbRowId.Name = "tbRowId";
            this.tbRowId.Size = new System.Drawing.Size(60, 20);
            this.tbRowId.TabIndex = 255;
            // 
            // tbLoad
            // 
            this.tbLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbLoad.Location = new System.Drawing.Point(921, 587);
            this.tbLoad.Margin = new System.Windows.Forms.Padding(2);
            this.tbLoad.Name = "tbLoad";
            this.tbLoad.Size = new System.Drawing.Size(71, 22);
            this.tbLoad.TabIndex = 256;
            this.tbLoad.Text = "load";
            this.tbLoad.UseVisualStyleBackColor = true;
            this.tbLoad.Click += new System.EventHandler(this.tbLoad_Click);
            // 
            // btClose
            // 
            this.btClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btClose.Location = new System.Drawing.Point(1024, 588);
            this.btClose.Margin = new System.Windows.Forms.Padding(2);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(71, 22);
            this.btClose.TabIndex = 257;
            this.btClose.Text = "close";
            this.btClose.UseVisualStyleBackColor = true;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // FileList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1165, 645);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.tbLoad);
            this.Controls.Add(this.tbRowId);
            this.Controls.Add(this.btSaveDeleteList);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.cbTopMost);
            this.Controls.Add(this.tbCount);
            this.Controls.Add(this.btImages);
            this.Controls.Add(this.btVideos);
            this.Controls.Add(this.dgv1);
            this.Name = "FileList";
            this.Text = "FileList";
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}