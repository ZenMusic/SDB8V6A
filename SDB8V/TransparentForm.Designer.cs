namespace SymbolDB
{
    partial class TransparentForm
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
            components = new System.ComponentModel.Container();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            visibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            transparentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tbPosition = new System.Windows.Forms.TextBox();
            tbDuration = new System.Windows.Forms.TextBox();
            cbTopMost = new System.Windows.Forms.CheckBox();
            tbFound = new System.Windows.Forms.TextBox();
            tbFolder = new System.Windows.Forms.TextBox();
            tbFileName = new System.Windows.Forms.TextBox();
            cbActive = new System.Windows.Forms.CheckBox();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { visibleToolStripMenuItem, transparentToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(135, 48);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // visibleToolStripMenuItem
            // 
            visibleToolStripMenuItem.Name = "visibleToolStripMenuItem";
            visibleToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            visibleToolStripMenuItem.Text = "visible";
            visibleToolStripMenuItem.Click += visibleToolStripMenuItem_Click;
            // 
            // transparentToolStripMenuItem
            // 
            transparentToolStripMenuItem.Name = "transparentToolStripMenuItem";
            transparentToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            transparentToolStripMenuItem.Text = "transparent";
            transparentToolStripMenuItem.Click += transparentToolStripMenuItem_Click;
            // 
            // tbPosition
            // 
            tbPosition.Font = new System.Drawing.Font("Segoe UI", 12F);
            tbPosition.Location = new System.Drawing.Point(10, 8);
            tbPosition.Name = "tbPosition";
            tbPosition.Size = new System.Drawing.Size(66, 29);
            tbPosition.TabIndex = 1;
            // 
            // tbDuration
            // 
            tbDuration.Font = new System.Drawing.Font("Segoe UI", 12F);
            tbDuration.Location = new System.Drawing.Point(97, 8);
            tbDuration.Name = "tbDuration";
            tbDuration.Size = new System.Drawing.Size(66, 29);
            tbDuration.TabIndex = 2;
            // 
            // cbTopMost
            // 
            cbTopMost.AutoSize = true;
            cbTopMost.Checked = true;
            cbTopMost.CheckState = System.Windows.Forms.CheckState.Checked;
            cbTopMost.Location = new System.Drawing.Point(189, 11);
            cbTopMost.Name = "cbTopMost";
            cbTopMost.Size = new System.Drawing.Size(44, 19);
            cbTopMost.TabIndex = 3;
            cbTopMost.Text = "top";
            cbTopMost.UseVisualStyleBackColor = true;
            cbTopMost.CheckedChanged += cbTopMost_CheckedChanged;
            // 
            // tbFound
            // 
            tbFound.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbFound.Location = new System.Drawing.Point(10, 94);
            tbFound.Name = "tbFound";
            tbFound.Size = new System.Drawing.Size(637, 23);
            tbFound.TabIndex = 4;
            // 
            // tbFolder
            // 
            tbFolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbFolder.Location = new System.Drawing.Point(10, 69);
            tbFolder.Name = "tbFolder";
            tbFolder.Size = new System.Drawing.Size(637, 23);
            tbFolder.TabIndex = 5;
            // 
            // tbFileName
            // 
            tbFileName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbFileName.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbFileName.Location = new System.Drawing.Point(10, 38);
            tbFileName.Name = "tbFileName";
            tbFileName.Size = new System.Drawing.Size(637, 26);
            tbFileName.TabIndex = 6;
            // 
            // cbActive
            // 
            cbActive.AutoSize = true;
            cbActive.Location = new System.Drawing.Point(232, 11);
            cbActive.Name = "cbActive";
            cbActive.Size = new System.Drawing.Size(44, 19);
            cbActive.TabIndex = 7;
            cbActive.Text = "Act";
            cbActive.UseVisualStyleBackColor = true;
            // 
            // TransparentForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(654, 120);
            Controls.Add(cbActive);
            Controls.Add(tbFileName);
            Controls.Add(tbFolder);
            Controls.Add(tbFound);
            Controls.Add(cbTopMost);
            Controls.Add(tbDuration);
            Controls.Add(tbPosition);
            Name = "TransparentForm";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "VVVVVVVVVvvvvvv--";
            FormClosing += TransparentForm_FormClosing;
            Load += TransparentForm_Load;
            DoubleClick += TransparentForm_DoubleClick;
            MouseDown += TransparentForm_MouseDown;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem visibleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem transparentToolStripMenuItem;
        private System.Windows.Forms.TextBox tbPosition;
        private System.Windows.Forms.TextBox tbDuration;
        private System.Windows.Forms.CheckBox cbTopMost;
        private System.Windows.Forms.TextBox tbFound;
        private System.Windows.Forms.TextBox tbFolder;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.CheckBox cbActive;
    }
}