using System.Drawing;
using System.Windows.Forms;

namespace SymbolDB
{
    partial class EditFileInfo
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox tbFname;
        private TextBox tbExt;
        private TextBox tbDpath;
        private TextBox tbFpath;
        private TextBox tbType;
        private TextBox numLevel;
        private TextBox numLen;
        private TextBox tbStimestamp;
        private CheckBox cbBDelete;
        private CheckBox cbBInvalid;
        private TextBox tbRating;
        private TextBox tbSource;
        private TextBox numPlayTime;
        private TextBox numMinutes;
        private TextBox numSeconds;
        private TextBox numWidth;
        private TextBox numHeight;
        private TextBox tbComment;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support.
        /// </summary>
        private void InitializeComponent()
        {
            tbKeyPath = new TextBox();
            tbFname = new TextBox();
            lblFname = new Label();
            tbExt = new TextBox();
            lblExt = new Label();
            tbDpath = new TextBox();
            lblDpath = new Label();
            tbFpath = new TextBox();
            lblFpath = new Label();
            tbType = new TextBox();
            lblType = new Label();
            numLevel = new TextBox();
            lblLevel = new Label();
            numLen = new TextBox();
            lblLen = new Label();
            tbStimestamp = new TextBox();
            lblStimestamp = new Label();
            cbBDelete = new CheckBox();
            lblBDelete = new Label();
            cbBInvalid = new CheckBox();
            lblBInvalid = new Label();
            tbRating = new TextBox();
            lblRating = new Label();
            tbSource = new TextBox();
            lblSource = new Label();
            numPlayTime = new TextBox();
            lblPlayTime = new Label();
            numMinutes = new TextBox();
            lblMinutes = new Label();
            numSeconds = new TextBox();
            lblSeconds = new Label();
            numWidth = new TextBox();
            lblWidth = new Label();
            numHeight = new TextBox();
            lblHeight = new Label();
            tbComment = new TextBox();
            lblDesc = new Label();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            tbKeyPath = new TextBox();
            SuspendLayout();
            // 
            // tbFname
            // 
            tbFname.Location = new Point(100, 53);
            tbFname.Name = "tbFname";
            tbFname.Size = new Size(900, 23);
            tbFname.TabIndex = 0;
            // 
            // lblFname
            // 
            lblFname.AutoSize = true;
            lblFname.Location = new Point(12, 53);
            lblFname.Name = "lblFname";
            lblFname.Size = new Size(43, 15);
            lblFname.TabIndex = 0;
            lblFname.Text = "Fname";
            // 
            // tbExt
            // 
            tbExt.Location = new Point(100, 83);
            tbExt.Name = "tbExt";
            tbExt.Size = new Size(400, 23);
            tbExt.TabIndex = 1;
            // 
            // lblExt
            // 
            lblExt.AutoSize = true;
            lblExt.Location = new Point(12, 83);
            lblExt.Name = "lblExt";
            lblExt.Size = new Size(22, 15);
            lblExt.TabIndex = 1;
            lblExt.Text = "Ext";
            // 
            // tbDpath
            // 
            tbDpath.Location = new Point(100, 113);
            tbDpath.Name = "tbDpath";
            tbDpath.Size = new Size(900, 23);
            tbDpath.TabIndex = 2;
            // 
            // lblDpath
            // 
            lblDpath.AutoSize = true;
            lblDpath.Location = new Point(12, 113);
            lblDpath.Name = "lblDpath";
            lblDpath.Size = new Size(39, 15);
            lblDpath.TabIndex = 2;
            lblDpath.Text = "Dpath";
            // 
            // tbFpath
            // 
            tbFpath.Location = new Point(100, 143);
            tbFpath.Name = "tbFpath";
            tbFpath.Size = new Size(900, 23);
            tbFpath.TabIndex = 3;
            // 
            // lblFpath
            // 
            lblFpath.AutoSize = true;
            lblFpath.Location = new Point(12, 143);
            lblFpath.Name = "lblFpath";
            lblFpath.Size = new Size(37, 15);
            lblFpath.TabIndex = 3;
            lblFpath.Text = "Fpath";
            // 
            // tbType
            // 
            tbType.Location = new Point(100, 173);
            tbType.Name = "tbType";
            tbType.Size = new Size(200, 23);
            tbType.TabIndex = 4;
            // 
            // lblType
            // 
            lblType.AutoSize = true;
            lblType.Location = new Point(12, 173);
            lblType.Name = "lblType";
            lblType.Size = new Size(32, 15);
            lblType.TabIndex = 4;
            lblType.Text = "Type";
            // 
            // numLevel
            // 
            numLevel.Location = new Point(100, 203);
            numLevel.Name = "numLevel";
            numLevel.Size = new Size(200, 23);
            numLevel.TabIndex = 5;
            // 
            // lblLevel
            // 
            lblLevel.AutoSize = true;
            lblLevel.Location = new Point(12, 203);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(34, 15);
            lblLevel.TabIndex = 5;
            lblLevel.Text = "Level";
            // 
            // numLen
            // 
            numLen.Location = new Point(100, 233);
            numLen.Name = "numLen";
            numLen.Size = new Size(200, 23);
            numLen.TabIndex = 6;
            // 
            // lblLen
            // 
            lblLen.AutoSize = true;
            lblLen.Location = new Point(12, 233);
            lblLen.Name = "lblLen";
            lblLen.Size = new Size(26, 15);
            lblLen.TabIndex = 6;
            lblLen.Text = "Len";
            // 
            // tbStimestamp
            // 
            tbStimestamp.Location = new Point(100, 263);
            tbStimestamp.Name = "tbStimestamp";
            tbStimestamp.Size = new Size(200, 23);
            tbStimestamp.TabIndex = 7;
            // 
            // lblStimestamp
            // 
            lblStimestamp.AutoSize = true;
            lblStimestamp.Location = new Point(12, 263);
            lblStimestamp.Name = "lblStimestamp";
            lblStimestamp.Size = new Size(70, 15);
            lblStimestamp.TabIndex = 7;
            lblStimestamp.Text = "Stimestamp";
            // 
            // cbBDelete
            // 
            cbBDelete.Location = new Point(100, 293);
            cbBDelete.Name = "cbBDelete";
            cbBDelete.Size = new Size(200, 23);
            cbBDelete.TabIndex = 8;
            cbBDelete.Text = "Delete";
            // 
            // lblBDelete
            // 
            lblBDelete.AutoSize = true;
            lblBDelete.Location = new Point(12, 293);
            lblBDelete.Name = "lblBDelete";
            lblBDelete.Size = new Size(47, 15);
            lblBDelete.TabIndex = 8;
            lblBDelete.Text = "BDelete";
            // 
            // cbBInvalid
            // 
            cbBInvalid.Location = new Point(100, 323);
            cbBInvalid.Name = "cbBInvalid";
            cbBInvalid.Size = new Size(200, 23);
            cbBInvalid.TabIndex = 9;
            cbBInvalid.Text = "Invalid";
            // 
            // lblBInvalid
            // 
            lblBInvalid.AutoSize = true;
            lblBInvalid.Location = new Point(12, 323);
            lblBInvalid.Name = "lblBInvalid";
            lblBInvalid.Size = new Size(49, 15);
            lblBInvalid.TabIndex = 9;
            lblBInvalid.Text = "BInvalid";
            // 
            // tbRating
            // 
            tbRating.Location = new Point(100, 353);
            tbRating.MaxLength = 1;
            tbRating.Name = "tbRating";
            tbRating.Size = new Size(36, 23);
            tbRating.TabIndex = 10;
            tbRating.TextChanged += tbRating_TextChanged;
            tbRating.KeyPress += tbRating_KeyPress;
            // 
            // lblRating
            // 
            lblRating.AutoSize = true;
            lblRating.Location = new Point(12, 353);
            lblRating.Name = "lblRating";
            lblRating.Size = new Size(41, 15);
            lblRating.TabIndex = 10;
            lblRating.Text = "Rating";
            // 
            // tbSource
            // 
            tbSource.Location = new Point(100, 383);
            tbSource.Name = "tbSource";
            tbSource.Size = new Size(200, 23);
            tbSource.TabIndex = 11;
            // 
            // lblSource
            // 
            lblSource.AutoSize = true;
            lblSource.Location = new Point(12, 383);
            lblSource.Name = "lblSource";
            lblSource.Size = new Size(43, 15);
            lblSource.TabIndex = 11;
            lblSource.Text = "Source";
            // 
            // numPlayTime
            // 
            numPlayTime.Location = new Point(100, 413);
            numPlayTime.Name = "numPlayTime";
            numPlayTime.Size = new Size(200, 23);
            numPlayTime.TabIndex = 12;
            // 
            // lblPlayTime
            // 
            lblPlayTime.AutoSize = true;
            lblPlayTime.Location = new Point(12, 413);
            lblPlayTime.Name = "lblPlayTime";
            lblPlayTime.Size = new Size(56, 15);
            lblPlayTime.TabIndex = 12;
            lblPlayTime.Text = "PlayTime";
            // 
            // numMinutes
            // 
            numMinutes.Location = new Point(100, 443);
            numMinutes.Name = "numMinutes";
            numMinutes.Size = new Size(200, 23);
            numMinutes.TabIndex = 13;
            // 
            // lblMinutes
            // 
            lblMinutes.AutoSize = true;
            lblMinutes.Location = new Point(12, 443);
            lblMinutes.Name = "lblMinutes";
            lblMinutes.Size = new Size(50, 15);
            lblMinutes.TabIndex = 13;
            lblMinutes.Text = "Minutes";
            // 
            // numSeconds
            // 
            numSeconds.Location = new Point(100, 473);
            numSeconds.Name = "numSeconds";
            numSeconds.Size = new Size(200, 23);
            numSeconds.TabIndex = 14;
            // 
            // lblSeconds
            // 
            lblSeconds.AutoSize = true;
            lblSeconds.Location = new Point(12, 473);
            lblSeconds.Name = "lblSeconds";
            lblSeconds.Size = new Size(51, 15);
            lblSeconds.TabIndex = 14;
            lblSeconds.Text = "Seconds";
            // 
            // numWidth
            // 
            numWidth.Location = new Point(100, 503);
            numWidth.Name = "numWidth";
            numWidth.Size = new Size(200, 23);
            numWidth.TabIndex = 15;
            // 
            // lblWidth
            // 
            lblWidth.AutoSize = true;
            lblWidth.Location = new Point(12, 503);
            lblWidth.Name = "lblWidth";
            lblWidth.Size = new Size(39, 15);
            lblWidth.TabIndex = 15;
            lblWidth.Text = "Width";
            // 
            // numHeight
            // 
            numHeight.Location = new Point(100, 534);
            numHeight.Name = "numHeight";
            numHeight.Size = new Size(200, 23);
            numHeight.TabIndex = 16;
            // 
            // lblHeight
            // 
            lblHeight.AutoSize = true;
            lblHeight.Location = new Point(12, 534);
            lblHeight.Name = "lblHeight";
            lblHeight.Size = new Size(43, 15);
            lblHeight.TabIndex = 16;
            lblHeight.Text = "Height";
            // 
            // tbComment
            // 
            tbComment.Location = new Point(100, 563);
            tbComment.Name = "tbComment";
            tbComment.Size = new Size(200, 23);
            tbComment.TabIndex = 17;
            // 
            // lblDesc
            // 
            lblDesc.AutoSize = true;
            lblDesc.Location = new Point(12, 563);
            lblDesc.Name = "lblDesc";
            lblDesc.Size = new Size(61, 15);
            lblDesc.TabIndex = 17;
            lblDesc.Text = "Comment";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 53);
            label1.Name = "label1";
            label1.Size = new Size(50, 19);
            label1.TabIndex = 18;
            label1.Text = "Fname";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(12, 12);
            label2.Name = "label2";
            label2.Size = new Size(58, 19);
            label2.TabIndex = 21;
            label2.Text = "keyPath";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 12);
            label3.Name = "label3";
            label3.Size = new Size(43, 15);
            label3.TabIndex = 19;
            label3.Text = "Fname";
            // 
            // tbKeyPath
            // 
            tbKeyPath.Location = new Point(100, 12);
            tbKeyPath.Name = "tbKeyPath";
            tbKeyPath.Size = new Size(900, 23);
            tbKeyPath.TabIndex = 20;
            // 
            // EditFileInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 600);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(tbKeyPath);
            Controls.Add(label1);
            Controls.Add(lblFname);
            Controls.Add(tbFname);
            Controls.Add(lblExt);
            Controls.Add(tbExt);
            Controls.Add(lblDpath);
            Controls.Add(tbDpath);
            Controls.Add(lblFpath);
            Controls.Add(tbFpath);
            Controls.Add(lblType);
            Controls.Add(tbType);
            Controls.Add(lblLevel);
            Controls.Add(numLevel);
            Controls.Add(lblLen);
            Controls.Add(numLen);
            Controls.Add(lblStimestamp);
            Controls.Add(tbStimestamp);
            Controls.Add(lblBDelete);
            Controls.Add(cbBDelete);
            Controls.Add(lblBInvalid);
            Controls.Add(cbBInvalid);
            Controls.Add(lblRating);
            Controls.Add(tbRating);
            Controls.Add(lblSource);
            Controls.Add(tbSource);
            Controls.Add(lblPlayTime);
            Controls.Add(numPlayTime);
            Controls.Add(lblMinutes);
            Controls.Add(numMinutes);
            Controls.Add(lblSeconds);
            Controls.Add(numSeconds);
            Controls.Add(lblWidth);
            Controls.Add(numWidth);
            Controls.Add(lblHeight);
            Controls.Add(numHeight);
            Controls.Add(lblDesc);
            Controls.Add(tbComment);
            Name = "EditFileInfo";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Edit File Info";
            FormClosing += EditFileInfo_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }
        private Label lblFname;
        private Label lblExt;
        private Label lblDpath;
        private Label lblFpath;
        private Label lblType;
        private Label lblLevel;
        private Label lblLen;
        private Label lblStimestamp;
        private Label lblBDelete;
        private Label lblBInvalid;
        private Label lblRating;
        private Label lblSource;
        private Label lblPlayTime;
        private Label lblMinutes;
        private Label lblSeconds;
        private Label lblWidth;
        private Label lblHeight;
        private Label lblDesc;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox tbKeyPath;
    }
}
