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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.visibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transparentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbPosition = new System.Windows.Forms.TextBox();
            this.tbDuration = new System.Windows.Forms.TextBox();
            this.cbTopMost = new System.Windows.Forms.CheckBox();
            this.tbFound = new System.Windows.Forms.TextBox();
            this.tbFolder = new System.Windows.Forms.TextBox();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.cbActive = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.visibleToolStripMenuItem,
            this.transparentToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(154, 52);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // visibleToolStripMenuItem
            // 
            this.visibleToolStripMenuItem.Name = "visibleToolStripMenuItem";
            this.visibleToolStripMenuItem.Size = new System.Drawing.Size(153, 24);
            this.visibleToolStripMenuItem.Text = "visible";
            this.visibleToolStripMenuItem.Click += new System.EventHandler(this.visibleToolStripMenuItem_Click);
            // 
            // transparentToolStripMenuItem
            // 
            this.transparentToolStripMenuItem.Name = "transparentToolStripMenuItem";
            this.transparentToolStripMenuItem.Size = new System.Drawing.Size(153, 24);
            this.transparentToolStripMenuItem.Text = "transparent";
            this.transparentToolStripMenuItem.Click += new System.EventHandler(this.transparentToolStripMenuItem_Click);
            // 
            // tbPosition
            // 
            this.tbPosition.Location = new System.Drawing.Point(12, 12);
            this.tbPosition.Name = "tbPosition";
            this.tbPosition.Size = new System.Drawing.Size(75, 22);
            this.tbPosition.TabIndex = 1;
            // 
            // tbDuration
            // 
            this.tbDuration.Location = new System.Drawing.Point(111, 10);
            this.tbDuration.Name = "tbDuration";
            this.tbDuration.Size = new System.Drawing.Size(75, 22);
            this.tbDuration.TabIndex = 2;
            // 
            // cbTopMost
            // 
            this.cbTopMost.AutoSize = true;
            this.cbTopMost.Location = new System.Drawing.Point(216, 12);
            this.cbTopMost.Name = "cbTopMost";
            this.cbTopMost.Size = new System.Drawing.Size(48, 20);
            this.cbTopMost.TabIndex = 3;
            this.cbTopMost.Text = "top";
            this.cbTopMost.UseVisualStyleBackColor = true;
            this.cbTopMost.CheckedChanged += new System.EventHandler(this.cbTopMost_CheckedChanged);
            // 
            // tbFound
            // 
            this.tbFound.Location = new System.Drawing.Point(12, 96);
            this.tbFound.Name = "tbFound";
            this.tbFound.Size = new System.Drawing.Size(301, 22);
            this.tbFound.TabIndex = 4;
            // 
            // tbFolder
            // 
            this.tbFolder.Location = new System.Drawing.Point(12, 68);
            this.tbFolder.Name = "tbFolder";
            this.tbFolder.Size = new System.Drawing.Size(301, 22);
            this.tbFolder.TabIndex = 5;
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(12, 40);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(301, 22);
            this.tbFileName.TabIndex = 6;
            // 
            // cbActive
            // 
            this.cbActive.AutoSize = true;
            this.cbActive.Location = new System.Drawing.Point(265, 12);
            this.cbActive.Name = "cbActive";
            this.cbActive.Size = new System.Drawing.Size(48, 20);
            this.cbActive.TabIndex = 7;
            this.cbActive.Text = "Act";
            this.cbActive.UseVisualStyleBackColor = true;
            // 
            // TransparentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 128);
            this.Controls.Add(this.cbActive);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.tbFolder);
            this.Controls.Add(this.tbFound);
            this.Controls.Add(this.cbTopMost);
            this.Controls.Add(this.tbDuration);
            this.Controls.Add(this.tbPosition);
            this.Name = "TransparentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "VVVVVVVVVvvvvvv--";
            this.Load += new System.EventHandler(this.TransparentForm_Load);
            this.DoubleClick += new System.EventHandler(this.TransparentForm_DoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TransparentForm_MouseDown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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