namespace SymbolDB
{
    partial class DisplayPreviewSet
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
            this.tbLoading = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.display1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameSeqToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.promptForSequenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleImageWidthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbLoading
            // 
            this.tbLoading.Enabled = false;
            this.tbLoading.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLoading.Location = new System.Drawing.Point(486, 309);
            this.tbLoading.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbLoading.Name = "tbLoading";
            this.tbLoading.Size = new System.Drawing.Size(356, 32);
            this.tbLoading.TabIndex = 0;
            this.tbLoading.Text = "loading images, please wait....";
            this.tbLoading.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbLoading.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.display1ToolStripMenuItem,
            this.renameSeqToolStripMenuItem,
            this.renameItemToolStripMenuItem,
            this.promptForSequenceToolStripMenuItem,
            this.toggleImageWidthToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(200, 180);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            this.toolStripMenuItem1.Text = "Move to Top";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // display1ToolStripMenuItem
            // 
            this.display1ToolStripMenuItem.Name = "display1ToolStripMenuItem";
            this.display1ToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.display1ToolStripMenuItem.Text = "Display1";
            this.display1ToolStripMenuItem.Click += new System.EventHandler(this.display1ToolStripMenuItem_Click);
            // 
            // renameSeqToolStripMenuItem
            // 
            this.renameSeqToolStripMenuItem.Name = "renameSeqToolStripMenuItem";
            this.renameSeqToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.renameSeqToolStripMenuItem.Text = "Copy and Rename";
            this.renameSeqToolStripMenuItem.Click += new System.EventHandler(this.renameSeqToolStripMenuItem_Click);
            // 
            // renameItemToolStripMenuItem
            // 
            this.renameItemToolStripMenuItem.Name = "renameItemToolStripMenuItem";
            this.renameItemToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.renameItemToolStripMenuItem.Text = "Rename With Sequence";
            this.renameItemToolStripMenuItem.Click += new System.EventHandler(this.renameItemToolStripMenuItem_Click);
            // 
            // promptForSequenceToolStripMenuItem
            // 
            this.promptForSequenceToolStripMenuItem.Name = "promptForSequenceToolStripMenuItem";
            this.promptForSequenceToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.promptForSequenceToolStripMenuItem.Text = "Prompt for sequence #";
            this.promptForSequenceToolStripMenuItem.Click += new System.EventHandler(this.promptForSequenceToolStripMenuItem_Click);
            // 
            // toggleImageWidthToolStripMenuItem
            // 
            this.toggleImageWidthToolStripMenuItem.Name = "toggleImageWidthToolStripMenuItem";
            this.toggleImageWidthToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.toggleImageWidthToolStripMenuItem.Text = "Toggle Image Width";
            this.toggleImageWidthToolStripMenuItem.Click += new System.EventHandler(this.toggleImageWidthToolStripMenuItem_Click);
            // 
            // DisplayPreviewSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(1426, 840);
            this.Controls.Add(this.tbLoading);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "DisplayPreviewSet";
            this.Text = "DisplaySet";
            this.DoubleClick += new System.EventHandler(this.DisplayPreviewSet_DoubleClick);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DisplaySet_KeyPress);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DisplaySet_MouseClick);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.DisplayPreviewSet_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DisplaySet_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DisplaySet_MouseUp);
            this.Move += new System.EventHandler(this.DisplaySet_Move);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbLoading;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem display1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameSeqToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem promptForSequenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleImageWidthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;

    }
}