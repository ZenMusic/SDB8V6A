namespace SymbolDB
{
    partial class SoundListForm
    {
        private System.ComponentModel.IContainer components = null;

        // Controls
        private System.Windows.Forms.ListBox lstSounds;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.Button btnPlayAll;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Panel pnlBottom;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lstSounds = new System.Windows.Forms.ListBox();
            lblCurrentFile = new System.Windows.Forms.Label();
            btnPlayAll = new System.Windows.Forms.Button();
            btnStop = new System.Windows.Forms.Button();
            lblHeader = new System.Windows.Forms.Label();
            pnlBottom = new System.Windows.Forms.Panel();
            numVolume = new System.Windows.Forms.NumericUpDown();
            btnCopyPath = new System.Windows.Forms.Button();
            pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numVolume).BeginInit();
            SuspendLayout();
            // 
            // lstSounds
            // 
            lstSounds.Dock = System.Windows.Forms.DockStyle.Fill;
            lstSounds.Font = new System.Drawing.Font("Consolas", 9.75F);
            lstSounds.HorizontalScrollbar = true;
            lstSounds.IntegralHeight = false;
            lstSounds.ItemHeight = 15;
            lstSounds.Location = new System.Drawing.Point(0, 28);
            lstSounds.Name = "lstSounds";
            lstSounds.ScrollAlwaysVisible = true;
            lstSounds.Size = new System.Drawing.Size(504, 367);
            lstSounds.TabIndex = 0;
            lstSounds.SelectedIndexChanged += lstSounds_SelectedIndexChanged;
            // 
            // lblCurrentFile
            // 
            lblCurrentFile.AutoEllipsis = true;
            lblCurrentFile.Dock = System.Windows.Forms.DockStyle.Bottom;
            lblCurrentFile.Font = new System.Drawing.Font("Consolas", 8.5F);
            lblCurrentFile.ForeColor = System.Drawing.Color.DarkBlue;
            lblCurrentFile.Location = new System.Drawing.Point(0, 395);
            lblCurrentFile.Name = "lblCurrentFile";
            lblCurrentFile.Size = new System.Drawing.Size(504, 36);
            lblCurrentFile.TabIndex = 1;
            lblCurrentFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnPlayAll
            // 
            btnPlayAll.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            btnPlayAll.Location = new System.Drawing.Point(6, 8);
            btnPlayAll.Name = "btnPlayAll";
            btnPlayAll.Size = new System.Drawing.Size(120, 36);
            btnPlayAll.TabIndex = 0;
            btnPlayAll.Text = "▶  Play All";
            btnPlayAll.Click += btnPlayAll_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            btnStop.Location = new System.Drawing.Point(134, 8);
            btnStop.Name = "btnStop";
            btnStop.Size = new System.Drawing.Size(90, 36);
            btnStop.TabIndex = 1;
            btnStop.Text = "■  Stop";
            btnStop.Click += btnStop_Click;
            // 
            // lblHeader
            // 
            lblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            lblHeader.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            lblHeader.Location = new System.Drawing.Point(0, 0);
            lblHeader.Name = "lblHeader";
            lblHeader.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            lblHeader.Size = new System.Drawing.Size(504, 28);
            lblHeader.TabIndex = 3;
            lblHeader.Text = "Sound Files  (click a row to play)";
            lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlBottom
            // 
            pnlBottom.Controls.Add(numVolume);
            pnlBottom.Controls.Add(btnCopyPath);
            pnlBottom.Controls.Add(btnPlayAll);
            pnlBottom.Controls.Add(btnStop);
            pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlBottom.Location = new System.Drawing.Point(0, 431);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new System.Windows.Forms.Padding(6);
            pnlBottom.Size = new System.Drawing.Size(504, 90);
            pnlBottom.TabIndex = 2;
            // 
            // numVolume
            // 
            numVolume.Location = new System.Drawing.Point(414, 9);
            numVolume.Name = "numVolume";
            numVolume.Size = new System.Drawing.Size(68, 23);
            numVolume.TabIndex = 3;
            numVolume.ValueChanged += numVolume_ValueChanged;
            // 
            // btnCopyPath
            // 
            btnCopyPath.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            btnCopyPath.Location = new System.Drawing.Point(252, 9);
            btnCopyPath.Name = "btnCopyPath";
            btnCopyPath.Size = new System.Drawing.Size(90, 36);
            btnCopyPath.TabIndex = 2;
            btnCopyPath.Text = "copy path";
            btnCopyPath.Click += btnCopyPath_Click;
            // 
            // SoundListForm
            // 
            ClientSize = new System.Drawing.Size(504, 521);
            Controls.Add(lstSounds);
            Controls.Add(lblCurrentFile);
            Controls.Add(pnlBottom);
            Controls.Add(lblHeader);
            Font = new System.Drawing.Font("Segoe UI", 9F);
            MinimumSize = new System.Drawing.Size(420, 400);
            Name = "SoundListForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Sound List";
            pnlBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numVolume).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnCopyPath;
        private System.Windows.Forms.NumericUpDown numVolume;
    }
}