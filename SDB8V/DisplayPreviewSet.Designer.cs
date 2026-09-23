using System;

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
            components = new System.ComponentModel.Container();
            tbLoading = new System.Windows.Forms.TextBox();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            display1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            renameSeqToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            renameItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            promptForSequenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toggleImageWidthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            btLoadingNextSet = new System.Windows.Forms.Button();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tbLoading
            // 
            tbLoading.Enabled = false;
            tbLoading.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            tbLoading.Location = new System.Drawing.Point(567, 357);
            tbLoading.Margin = new System.Windows.Forms.Padding(2);
            tbLoading.Name = "tbLoading";
            tbLoading.Size = new System.Drawing.Size(415, 32);
            tbLoading.TabIndex = 0;
            tbLoading.Text = "loading images, please wait....";
            tbLoading.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            tbLoading.TextChanged += textBox1_TextChanged;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { copyToolStripMenuItem, deleteToolStripMenuItem, toolStripMenuItem1, display1ToolStripMenuItem, renameSeqToolStripMenuItem, renameItemToolStripMenuItem, promptForSequenceToolStripMenuItem, toggleImageWidthToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(200, 180);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            copyToolStripMenuItem.Text = "Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            toolStripMenuItem1.Text = "Move to Top";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // display1ToolStripMenuItem
            // 
            display1ToolStripMenuItem.Name = "display1ToolStripMenuItem";
            display1ToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            display1ToolStripMenuItem.Text = "Display1";
            display1ToolStripMenuItem.Click += display1ToolStripMenuItem_Click;
            // 
            // renameSeqToolStripMenuItem
            // 
            renameSeqToolStripMenuItem.Name = "renameSeqToolStripMenuItem";
            renameSeqToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            renameSeqToolStripMenuItem.Text = "Copy and Rename";
            renameSeqToolStripMenuItem.Click += renameSeqToolStripMenuItem_Click;
            // 
            // renameItemToolStripMenuItem
            // 
            renameItemToolStripMenuItem.Name = "renameItemToolStripMenuItem";
            renameItemToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            renameItemToolStripMenuItem.Text = "Rename With Sequence";
            renameItemToolStripMenuItem.Click += renameItemToolStripMenuItem_Click;
            // 
            // promptForSequenceToolStripMenuItem
            // 
            promptForSequenceToolStripMenuItem.Name = "promptForSequenceToolStripMenuItem";
            promptForSequenceToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            promptForSequenceToolStripMenuItem.Text = "Prompt for sequence #";
            promptForSequenceToolStripMenuItem.Click += promptForSequenceToolStripMenuItem_Click;
            // 
            // toggleImageWidthToolStripMenuItem
            // 
            toggleImageWidthToolStripMenuItem.Name = "toggleImageWidthToolStripMenuItem";
            toggleImageWidthToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            toggleImageWidthToolStripMenuItem.Text = "Toggle Image Width";
            toggleImageWidthToolStripMenuItem.Click += toggleImageWidthToolStripMenuItem_Click;
            // 
            // btLoadingNextSet
            // 
            btLoadingNextSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btLoadingNextSet.Location = new System.Drawing.Point(197, 26);
            btLoadingNextSet.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btLoadingNextSet.Name = "btLoadingNextSet";
            btLoadingNextSet.Size = new System.Drawing.Size(311, 88);
            btLoadingNextSet.TabIndex = 49;
            btLoadingNextSet.Text = "Wait, loading ...";
            btLoadingNextSet.UseVisualStyleBackColor = true;
            btLoadingNextSet.Visible = false;
            // 
            // DisplayPreviewSet
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.SystemColors.AppWorkspace;
            ClientSize = new System.Drawing.Size(1664, 969);
            Controls.Add(btLoadingNextSet);
            Controls.Add(tbLoading);
            Margin = new System.Windows.Forms.Padding(2);
            Name = "DisplayPreviewSet";
            Text = "DisplaySet";
            DoubleClick += DisplayPreviewSet_DoubleClick;
            KeyPress += DisplaySet_KeyPress;
            MouseClick += DisplaySet_MouseClick;
            MouseDoubleClick += DisplayPreviewSet_MouseDoubleClick;
            MouseDown += DisplaySet_MouseDown;
            MouseUp += DisplaySet_MouseUp;
            Move += DisplaySet_Move;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }
        // Add this method to the DisplayPreviewSet class (in the code-behind file, e.g., DisplayPreviewSet.cs)
        private void DisplayPreviewSet_DoubleClick(object sender, EventArgs e)
        {
            // You can leave this empty or add logic as needed
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
        private System.Windows.Forms.Button btLoadingNextSet;
    }
}