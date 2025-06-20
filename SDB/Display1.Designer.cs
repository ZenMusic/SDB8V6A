namespace SymbolDB
{
    partial class Display1
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
            if (this.MyImage != null)
                this.MyImage.Dispose();
            if (this.captured != null)
                this.captured.Dispose();
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
            this.pb1 = new System.Windows.Forms.PictureBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tagpb1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miAnnotation = new System.Windows.Forms.ToolStripMenuItem();
            this.miSlideShowMode = new System.Windows.Forms.ToolStripMenuItem();
            this.miRedraw = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.miResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pb2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeThisWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.catalogImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullScreenModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPreviousPb2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miShowAllTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.holdThisSlideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tagpb1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pb1
            // 
            this.pb1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb1.BackColor = System.Drawing.Color.Black;
            this.pb1.Location = new System.Drawing.Point(0, 0);
            this.pb1.Margin = new System.Windows.Forms.Padding(0);
            this.pb1.MinimumSize = new System.Drawing.Size(592, 546);
            this.pb1.Name = "pb1";
            this.pb1.Size = new System.Drawing.Size(1889, 1031);
            this.pb1.TabIndex = 0;
            this.pb1.TabStop = false;
            this.pb1.SizeChanged += new System.EventHandler(this.pb1_SizeChanged);
            this.pb1.Click += new System.EventHandler(this.pb_Click);
            this.pb1.DoubleClick += new System.EventHandler(this.pb_DoubleClick);
            this.pb1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pb_MouseDoubleClick);
            this.pb1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pb_MouseDown);
            this.pb1.MouseEnter += new System.EventHandler(this.pb1_MouseEnter);
            this.pb1.MouseLeave += new System.EventHandler(this.pb1_MouseLeave);
            this.pb1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pb_MouseMove);
            this.pb1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pb_MouseUp);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tagpb1
            // 
            this.tagpb1.Location = new System.Drawing.Point(1107, 39);
            this.tagpb1.Margin = new System.Windows.Forms.Padding(4);
            this.tagpb1.Name = "tagpb1";
            this.tagpb1.Size = new System.Drawing.Size(133, 62);
            this.tagpb1.TabIndex = 1;
            this.tagpb1.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAnnotation,
            this.miSlideShowMode,
            this.miRedraw,
            this.toolStripMenuItem1,
            this.miResetToolStripMenuItem,
            this.pb2ToolStripMenuItem,
            this.closeThisWindowToolStripMenuItem,
            this.exitToolStripMenuItem,
            this.catalogImagesToolStripMenuItem,
            this.fullScreenModeToolStripMenuItem,
            this.showPreviousPb2ToolStripMenuItem,
            this.miShowAllTagsToolStripMenuItem,
            this.monitorFolderToolStripMenuItem,
            this.holdThisSlideToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(230, 368);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // miAnnotation
            // 
            this.miAnnotation.Name = "miAnnotation";
            this.miAnnotation.Size = new System.Drawing.Size(229, 24);
            this.miAnnotation.Text = "Annotation Mode";
            this.miAnnotation.Click += new System.EventHandler(this.miAnnotation_Click);
            // 
            // miSlideShowMode
            // 
            this.miSlideShowMode.Name = "miSlideShowMode";
            this.miSlideShowMode.Size = new System.Drawing.Size(229, 24);
            this.miSlideShowMode.Text = "SlideShow Mode";
            this.miSlideShowMode.Click += new System.EventHandler(this.miSlideShowMode_Click);
            // 
            // miRedraw
            // 
            this.miRedraw.Name = "miRedraw";
            this.miRedraw.Size = new System.Drawing.Size(229, 24);
            this.miRedraw.Text = "reset";
            this.miRedraw.Click += new System.EventHandler(this.miRedraw_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(229, 24);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // miResetToolStripMenuItem
            // 
            this.miResetToolStripMenuItem.Name = "miResetToolStripMenuItem";
            this.miResetToolStripMenuItem.Size = new System.Drawing.Size(229, 24);
            this.miResetToolStripMenuItem.Text = "Redraw";
            this.miResetToolStripMenuItem.Click += new System.EventHandler(this.miResetToolStripMenuItem_Click);
            // 
            // pb2ToolStripMenuItem
            // 
            this.pb2ToolStripMenuItem.Name = "pb2ToolStripMenuItem";
            this.pb2ToolStripMenuItem.Size = new System.Drawing.Size(229, 24);
            this.pb2ToolStripMenuItem.Text = "pb2";
            this.pb2ToolStripMenuItem.Click += new System.EventHandler(this.pb2ToolStripMenuItem_Click);
            // 
            // closeThisWindowToolStripMenuItem
            // 
            this.closeThisWindowToolStripMenuItem.Name = "closeThisWindowToolStripMenuItem";
            this.closeThisWindowToolStripMenuItem.Size = new System.Drawing.Size(229, 24);
            this.closeThisWindowToolStripMenuItem.Text = "Close this window";
            this.closeThisWindowToolStripMenuItem.Click += new System.EventHandler(this.closeThisWindowToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(229, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // catalogImagesToolStripMenuItem
            // 
            this.catalogImagesToolStripMenuItem.Name = "catalogImagesToolStripMenuItem";
            this.catalogImagesToolStripMenuItem.Size = new System.Drawing.Size(229, 24);
            this.catalogImagesToolStripMenuItem.Text = "Catalog Images";
            this.catalogImagesToolStripMenuItem.Click += new System.EventHandler(this.miCatalogImagesToolStripMenuItem_Click);
            // 
            // fullScreenModeToolStripMenuItem
            // 
            this.fullScreenModeToolStripMenuItem.Name = "fullScreenModeToolStripMenuItem";
            this.fullScreenModeToolStripMenuItem.Size = new System.Drawing.Size(229, 24);
            this.fullScreenModeToolStripMenuItem.Text = "Full Screen Mode";
            this.fullScreenModeToolStripMenuItem.Click += new System.EventHandler(this.miFullScreenModeToolStripMenuItem_Click);
            // 
            // showPreviousPb2ToolStripMenuItem
            // 
            this.showPreviousPb2ToolStripMenuItem.Name = "showPreviousPb2ToolStripMenuItem";
            this.showPreviousPb2ToolStripMenuItem.Size = new System.Drawing.Size(229, 24);
            this.showPreviousPb2ToolStripMenuItem.Text = "Resizable with title bar";
            this.showPreviousPb2ToolStripMenuItem.Click += new System.EventHandler(this.showResizableWithTitleBar);
            // 
            // miShowAllTagsToolStripMenuItem
            // 
            this.miShowAllTagsToolStripMenuItem.Name = "miShowAllTagsToolStripMenuItem";
            this.miShowAllTagsToolStripMenuItem.Size = new System.Drawing.Size(229, 24);
            this.miShowAllTagsToolStripMenuItem.Text = "Show All Tags";
            this.miShowAllTagsToolStripMenuItem.Click += new System.EventHandler(this.miShowAllTagsToolStripMenuItem_Click);
            // 
            // monitorFolderToolStripMenuItem
            // 
            this.monitorFolderToolStripMenuItem.Name = "monitorFolderToolStripMenuItem";
            this.monitorFolderToolStripMenuItem.Size = new System.Drawing.Size(229, 24);
            this.monitorFolderToolStripMenuItem.Text = "monitor folder";
            this.monitorFolderToolStripMenuItem.Click += new System.EventHandler(this.monitorFolderToolStripMenuItem_Click);
            // 
            // holdThisSlideToolStripMenuItem
            // 
            this.holdThisSlideToolStripMenuItem.Name = "holdThisSlideToolStripMenuItem";
            this.holdThisSlideToolStripMenuItem.Size = new System.Drawing.Size(229, 24);
            this.holdThisSlideToolStripMenuItem.Text = "Hold This Slide";
            this.holdThisSlideToolStripMenuItem.Click += new System.EventHandler(this.holdThisSlideToolStripMenuItem_Click);
            // 
            // Display1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1898, 1030);
            this.Controls.Add(this.tagpb1);
            this.Controls.Add(this.pb1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Display1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Display1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Display1_FormClosing);
            this.Load += new System.EventHandler(this.Display1_Load);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Display1_Scroll);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Display1_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Display1_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tagpb1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pb1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PictureBox tagpb1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem miAnnotation;
        private System.Windows.Forms.ToolStripMenuItem miSlideShowMode;
        private System.Windows.Forms.ToolStripMenuItem miRedraw;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miResetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pb2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeThisWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem catalogImagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullScreenModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showPreviousPb2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miShowAllTagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem monitorFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem holdThisSlideToolStripMenuItem;
    }
}