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
            components = new System.ComponentModel.Container();
            pb1 = new System.Windows.Forms.PictureBox();
            colorDialog1 = new System.Windows.Forms.ColorDialog();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            tagpb1 = new System.Windows.Forms.PictureBox();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            miAnnotation = new System.Windows.Forms.ToolStripMenuItem();
            miSlideShowMode = new System.Windows.Forms.ToolStripMenuItem();
            miRedraw = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            miResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pb2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            closeThisWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            catalogImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            fullScreenModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            showPreviousPb2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            miShowAllTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            monitorFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            holdThisSlideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)pb1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tagpb1).BeginInit();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // pb1
            // 
            pb1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pb1.BackColor = System.Drawing.Color.Black;
            pb1.Location = new System.Drawing.Point(0, 0);
            pb1.Margin = new System.Windows.Forms.Padding(0);
            pb1.MinimumSize = new System.Drawing.Size(518, 512);
            pb1.Name = "pb1";
            pb1.Size = new System.Drawing.Size(1653, 967);
            pb1.TabIndex = 0;
            pb1.TabStop = false;
            pb1.SizeChanged += pb1_SizeChanged;
            pb1.Click += pb_Click;
            pb1.DoubleClick += pb_DoubleClick;
            pb1.MouseDoubleClick += pb_MouseDoubleClick;
            
            pb1.MouseDown += pb_MouseDown;
            pb1.MouseEnter += pb1_MouseEnter;
            pb1.MouseLeave += pb1_MouseLeave;
            //pb1.MouseMove += pb_MouseMove;
            //pb1.MouseUp += pb_MouseUp;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // tagpb1
            // 
            tagpb1.Location = new System.Drawing.Point(969, 37);
            tagpb1.Margin = new System.Windows.Forms.Padding(4);
            tagpb1.Name = "tagpb1";
            tagpb1.Size = new System.Drawing.Size(116, 58);
            tagpb1.TabIndex = 1;
            tagpb1.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { miAnnotation, miSlideShowMode, miRedraw, toolStripMenuItem1, miResetToolStripMenuItem, pb2ToolStripMenuItem, closeThisWindowToolStripMenuItem, exitToolStripMenuItem, catalogImagesToolStripMenuItem, fullScreenModeToolStripMenuItem, showPreviousPb2ToolStripMenuItem, miShowAllTagsToolStripMenuItem, monitorFolderToolStripMenuItem, holdThisSlideToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(192, 334);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // miAnnotation
            // 
            miAnnotation.Name = "miAnnotation";
            miAnnotation.Size = new System.Drawing.Size(191, 22);
            miAnnotation.Text = "Annotation Mode";
            miAnnotation.Click += miAnnotation_Click;
            // 
            // miSlideShowMode
            // 
            miSlideShowMode.Name = "miSlideShowMode";
            miSlideShowMode.Size = new System.Drawing.Size(191, 22);
            miSlideShowMode.Text = "SlideShow Mode";
            miSlideShowMode.Click += miSlideShowMode_Click;
            // 
            // miRedraw
            // 
            miRedraw.Name = "miRedraw";
            miRedraw.Size = new System.Drawing.Size(191, 22);
            miRedraw.Text = "reset";
            miRedraw.Click += miRedraw_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(191, 22);
            toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // miResetToolStripMenuItem
            // 
            miResetToolStripMenuItem.Name = "miResetToolStripMenuItem";
            miResetToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            miResetToolStripMenuItem.Text = "Redraw";
            miResetToolStripMenuItem.Click += miResetToolStripMenuItem_Click;
            // 
            // pb2ToolStripMenuItem
            // 
            pb2ToolStripMenuItem.Name = "pb2ToolStripMenuItem";
            pb2ToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            pb2ToolStripMenuItem.Text = "pb2";
            pb2ToolStripMenuItem.Click += pb2ToolStripMenuItem_Click;
            // 
            // closeThisWindowToolStripMenuItem
            // 
            closeThisWindowToolStripMenuItem.Name = "closeThisWindowToolStripMenuItem";
            closeThisWindowToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            closeThisWindowToolStripMenuItem.Text = "Close this window";
            closeThisWindowToolStripMenuItem.Click += closeThisWindowToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // catalogImagesToolStripMenuItem
            // 
            catalogImagesToolStripMenuItem.Name = "catalogImagesToolStripMenuItem";
            catalogImagesToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            catalogImagesToolStripMenuItem.Text = "Catalog Images";
            catalogImagesToolStripMenuItem.Click += miCatalogImagesToolStripMenuItem_Click;
            // 
            // fullScreenModeToolStripMenuItem
            // 
            fullScreenModeToolStripMenuItem.Name = "fullScreenModeToolStripMenuItem";
            fullScreenModeToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            fullScreenModeToolStripMenuItem.Text = "Full Screen Mode";
            fullScreenModeToolStripMenuItem.Click += miFullScreenModeToolStripMenuItem_Click;
            // 
            // showPreviousPb2ToolStripMenuItem
            // 
            showPreviousPb2ToolStripMenuItem.Name = "showPreviousPb2ToolStripMenuItem";
            showPreviousPb2ToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            showPreviousPb2ToolStripMenuItem.Text = "Resizable with title bar";
            showPreviousPb2ToolStripMenuItem.Click += showResizableWithTitleBar;
            // 
            // miShowAllTagsToolStripMenuItem
            // 
            miShowAllTagsToolStripMenuItem.Name = "miShowAllTagsToolStripMenuItem";
            miShowAllTagsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            miShowAllTagsToolStripMenuItem.Text = "Show All Tags";
            miShowAllTagsToolStripMenuItem.Click += miShowAllTagsToolStripMenuItem_Click;
            // 
            // monitorFolderToolStripMenuItem
            // 
            monitorFolderToolStripMenuItem.Name = "monitorFolderToolStripMenuItem";
            monitorFolderToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            monitorFolderToolStripMenuItem.Text = "monitor folder";
            monitorFolderToolStripMenuItem.Click += monitorFolderToolStripMenuItem_Click;
            // 
            // holdThisSlideToolStripMenuItem
            // 
            holdThisSlideToolStripMenuItem.Name = "holdThisSlideToolStripMenuItem";
            holdThisSlideToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            holdThisSlideToolStripMenuItem.Text = "Hold This Slide";
            holdThisSlideToolStripMenuItem.Click += holdThisSlideToolStripMenuItem_Click;
            // 
            // Display1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1661, 966);
            Controls.Add(tagpb1);
            Controls.Add(pb1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "Display1";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Display1";
            FormClosing += Display1_FormClosing;
            Load += Display1_Load;
            Scroll += Display1_Scroll;
            KeyPress += Display1_KeyPress;
            KeyUp += Display1_KeyUp;
            ((System.ComponentModel.ISupportInitialize)pb1).EndInit();
            ((System.ComponentModel.ISupportInitialize)tagpb1).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);

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