namespace SymbolDB
{
    partial class XML_Editor
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
            this.lvFinfo = new System.Windows.Forms.ListView();
            this.tbSlideNumber = new System.Windows.Forms.TextBox();
            this.tagFileName = new System.Windows.Forms.TextBox();
            this.tbCount = new System.Windows.Forms.TextBox();
            this.pb1 = new System.Windows.Forms.PictureBox();
            this.tbFpath = new System.Windows.Forms.TextBox();
            this.btLoad = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.LVcs_ImageList = new System.Windows.Forms.ListView();
            this.tbLoadedCount = new System.Windows.Forms.TextBox();
            this.btLoadTarotInfo = new System.Windows.Forms.Button();
            this.lvTarotInfo = new System.Windows.Forms.ListView();
            this.btNext = new System.Windows.Forms.Button();
            this.tbIndex = new System.Windows.Forms.TextBox();
            this.btPrevious = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.btSetDescription = new System.Windows.Forms.Button();
            this.dgvFinfo = new System.Windows.Forms.DataGridView();
            this.Label = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFinfo)).BeginInit();
            this.SuspendLayout();
            // 
            // lvFinfo
            // 
            this.lvFinfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvFinfo.HideSelection = false;
            this.lvFinfo.Location = new System.Drawing.Point(12, 29);
            this.lvFinfo.Name = "lvFinfo";
            this.lvFinfo.Size = new System.Drawing.Size(542, 401);
            this.lvFinfo.TabIndex = 129;
            this.lvFinfo.UseCompatibleStateImageBehavior = false;
            // 
            // tbSlideNumber
            // 
            this.tbSlideNumber.Location = new System.Drawing.Point(1034, 56);
            this.tbSlideNumber.Name = "tbSlideNumber";
            this.tbSlideNumber.Size = new System.Drawing.Size(633, 22);
            this.tbSlideNumber.TabIndex = 130;
            // 
            // tagFileName
            // 
            this.tagFileName.Location = new System.Drawing.Point(1034, 104);
            this.tagFileName.Name = "tagFileName";
            this.tagFileName.Size = new System.Drawing.Size(633, 22);
            this.tagFileName.TabIndex = 131;
            // 
            // tbCount
            // 
            this.tbCount.Location = new System.Drawing.Point(1122, 264);
            this.tbCount.Name = "tbCount";
            this.tbCount.Size = new System.Drawing.Size(146, 22);
            this.tbCount.TabIndex = 132;
            // 
            // pb1
            // 
            this.pb1.Location = new System.Drawing.Point(1506, 238);
            this.pb1.Name = "pb1";
            this.pb1.Size = new System.Drawing.Size(161, 173);
            this.pb1.TabIndex = 133;
            this.pb1.TabStop = false;
            this.pb1.Click += new System.EventHandler(this.pb1_Click);
            // 
            // tbFpath
            // 
            this.tbFpath.Location = new System.Drawing.Point(1034, 150);
            this.tbFpath.Name = "tbFpath";
            this.tbFpath.Size = new System.Drawing.Size(633, 22);
            this.tbFpath.TabIndex = 134;
            // 
            // btLoad
            // 
            this.btLoad.Location = new System.Drawing.Point(1050, 329);
            this.btLoad.Name = "btLoad";
            this.btLoad.Size = new System.Drawing.Size(111, 30);
            this.btLoad.TabIndex = 135;
            this.btLoad.Text = "Load XML";
            this.btLoad.UseVisualStyleBackColor = true;
            this.btLoad.Click += new System.EventHandler(this.btLoad_Click);
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(1075, 390);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(126, 30);
            this.btSave.TabIndex = 136;
            this.btSave.Text = "Save XML";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(1034, 192);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(443, 22);
            this.tbFileName.TabIndex = 137;
            // 
            // LVcs_ImageList
            // 
            this.LVcs_ImageList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LVcs_ImageList.HideSelection = false;
            this.LVcs_ImageList.Location = new System.Drawing.Point(2, 612);
            this.LVcs_ImageList.Margin = new System.Windows.Forms.Padding(4);
            this.LVcs_ImageList.MinimumSize = new System.Drawing.Size(439, 405);
            this.LVcs_ImageList.Name = "LVcs_ImageList";
            this.LVcs_ImageList.Size = new System.Drawing.Size(1585, 435);
            this.LVcs_ImageList.TabIndex = 138;
            this.LVcs_ImageList.UseCompatibleStateImageBehavior = false;
            // 
            // tbLoadedCount
            // 
            this.tbLoadedCount.Location = new System.Drawing.Point(470, 1);
            this.tbLoadedCount.Name = "tbLoadedCount";
            this.tbLoadedCount.Size = new System.Drawing.Size(126, 22);
            this.tbLoadedCount.TabIndex = 139;
            // 
            // btLoadTarotInfo
            // 
            this.btLoadTarotInfo.Location = new System.Drawing.Point(1192, 329);
            this.btLoadTarotInfo.Name = "btLoadTarotInfo";
            this.btLoadTarotInfo.Size = new System.Drawing.Size(152, 30);
            this.btLoadTarotInfo.TabIndex = 140;
            this.btLoadTarotInfo.Text = "Load Tarot Info";
            this.btLoadTarotInfo.UseVisualStyleBackColor = true;
            this.btLoadTarotInfo.Click += new System.EventHandler(this.btLoadTarot_Click);
            // 
            // lvTarotInfo
            // 
            this.lvTarotInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTarotInfo.HideSelection = false;
            this.lvTarotInfo.Location = new System.Drawing.Point(115, 571);
            this.lvTarotInfo.Margin = new System.Windows.Forms.Padding(4);
            this.lvTarotInfo.MinimumSize = new System.Drawing.Size(439, 405);
            this.lvTarotInfo.Name = "lvTarotInfo";
            this.lvTarotInfo.Size = new System.Drawing.Size(1520, 405);
            this.lvTarotInfo.TabIndex = 141;
            this.lvTarotInfo.UseCompatibleStateImageBehavior = false;
            // 
            // btNext
            // 
            this.btNext.Location = new System.Drawing.Point(775, 494);
            this.btNext.Name = "btNext";
            this.btNext.Size = new System.Drawing.Size(75, 30);
            this.btNext.TabIndex = 142;
            this.btNext.Text = "Next";
            this.btNext.UseVisualStyleBackColor = true;
            this.btNext.Click += new System.EventHandler(this.btNext_Click);
            // 
            // tbIndex
            // 
            this.tbIndex.Location = new System.Drawing.Point(269, 1);
            this.tbIndex.Name = "tbIndex";
            this.tbIndex.Size = new System.Drawing.Size(126, 22);
            this.tbIndex.TabIndex = 143;
            // 
            // btPrevious
            // 
            this.btPrevious.Location = new System.Drawing.Point(876, 494);
            this.btPrevious.Name = "btPrevious";
            this.btPrevious.Size = new System.Drawing.Size(75, 30);
            this.btPrevious.TabIndex = 144;
            this.btPrevious.Text = "Previous";
            this.btPrevious.UseVisualStyleBackColor = true;
            this.btPrevious.Click += new System.EventHandler(this.btPrevious_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(416, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 16);
            this.label1.TabIndex = 145;
            this.label1.Text = "of";
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(143, 498);
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(497, 22);
            this.tbDescription.TabIndex = 146;
            // 
            // btSetDescription
            // 
            this.btSetDescription.Location = new System.Drawing.Point(667, 498);
            this.btSetDescription.Name = "btSetDescription";
            this.btSetDescription.Size = new System.Drawing.Size(75, 30);
            this.btSetDescription.TabIndex = 147;
            this.btSetDescription.Text = "Set Desc";
            this.btSetDescription.UseVisualStyleBackColor = true;
            this.btSetDescription.Click += new System.EventHandler(this.btSetDescription_Click);
            // 
            // dgvFinfo
            // 
            this.dgvFinfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFinfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Label,
            this.Value});
            this.dgvFinfo.Location = new System.Drawing.Point(569, 44);
            this.dgvFinfo.Name = "dgvFinfo";
            this.dgvFinfo.RowHeadersWidth = 51;
            this.dgvFinfo.RowTemplate.Height = 24;
            this.dgvFinfo.Size = new System.Drawing.Size(441, 386);
            this.dgvFinfo.TabIndex = 148;
            // 
            // Label
            // 
            this.Label.HeaderText = "Label";
            this.Label.MinimumWidth = 6;
            this.Label.Name = "Label";
            this.Label.Width = 125;
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.MinimumWidth = 6;
            this.Value.Name = "Value";
            this.Value.Width = 125;
            // 
            // XML_Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1782, 1047);
            this.Controls.Add(this.dgvFinfo);
            this.Controls.Add(this.btSetDescription);
            this.Controls.Add(this.tbDescription);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btPrevious);
            this.Controls.Add(this.tbIndex);
            this.Controls.Add(this.btNext);
            this.Controls.Add(this.lvTarotInfo);
            this.Controls.Add(this.btLoadTarotInfo);
            this.Controls.Add(this.tbLoadedCount);
            this.Controls.Add(this.LVcs_ImageList);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.btLoad);
            this.Controls.Add(this.tbFpath);
            this.Controls.Add(this.pb1);
            this.Controls.Add(this.tbCount);
            this.Controls.Add(this.tagFileName);
            this.Controls.Add(this.tbSlideNumber);
            this.Controls.Add(this.lvFinfo);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "XML_Editor";
            this.Text = "XML_Editor";
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFinfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvFinfo;
        private System.Windows.Forms.TextBox tbSlideNumber;
        private System.Windows.Forms.TextBox tagFileName;
        private System.Windows.Forms.TextBox tbCount;
        private System.Windows.Forms.PictureBox pb1;
        private System.Windows.Forms.TextBox tbFpath;
        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.ListView LVcs_ImageList;
        private System.Windows.Forms.TextBox tbLoadedCount;
        private System.Windows.Forms.Button btLoadTarotInfo;
        private System.Windows.Forms.ListView lvTarotInfo;
        private System.Windows.Forms.Button btNext;
        private System.Windows.Forms.TextBox tbIndex;
        private System.Windows.Forms.Button btPrevious;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.Button btSetDescription;
        private System.Windows.Forms.DataGridView dgvFinfo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Label;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
    }
}