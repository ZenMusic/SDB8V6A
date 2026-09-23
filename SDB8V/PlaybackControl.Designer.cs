namespace SymbolDB
{
    partial class PlaybackControl
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tlpMain = new System.Windows.Forms.TableLayoutPanel();
            pnlRow1 = new System.Windows.Forms.FlowLayoutPanel();
            btZoom = new System.Windows.Forms.Button();
            cbSlow2 = new System.Windows.Forms.CheckBox();
            cbSlow = new System.Windows.Forms.CheckBox();
            cbPause = new System.Windows.Forms.CheckBox();
            btResume = new System.Windows.Forms.Button();
            cbTopMost = new System.Windows.Forms.CheckBox();
            pnlRow2Outer = new System.Windows.Forms.Panel();
            pnlRow2 = new System.Windows.Forms.FlowLayoutPanel();
            btREV = new System.Windows.Forms.Button();
            btSkipBackBig = new System.Windows.Forms.Button();
            btSkipBackMed = new System.Windows.Forms.Button();
            btSkipBack = new System.Windows.Forms.Button();
            btPlay = new System.Windows.Forms.Button();
            btSkipFwd = new System.Windows.Forms.Button();
            btSkipFwd1Min = new System.Windows.Forms.Button();
            btEnd = new System.Windows.Forms.Button();
            btGoToEnd = new System.Windows.Forms.Button();
            btSkipFwd2Min = new System.Windows.Forms.Button();
            btFF = new System.Windows.Forms.Button();
            pnlRow3 = new System.Windows.Forms.FlowLayoutPanel();
            cbR90 = new System.Windows.Forms.CheckBox();
            btSaveR = new System.Windows.Forms.Button();
            cbSI = new System.Windows.Forms.CheckBox();
            pnlExtra = new System.Windows.Forms.FlowLayoutPanel();
            btMute = new System.Windows.Forms.Button();
            btMoveMovie = new System.Windows.Forms.Button();
            btCloseMovie = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            tlpMain.SuspendLayout();
            pnlRow1.SuspendLayout();
            pnlRow2Outer.SuspendLayout();
            pnlRow2.SuspendLayout();
            pnlRow3.SuspendLayout();
            pnlExtra.SuspendLayout();
            SuspendLayout();
            // 
            // tlpMain
            // 
            tlpMain.AutoSize = true;
            tlpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tlpMain.ColumnCount = 1;
            tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tlpMain.Controls.Add(pnlRow1, 0, 0);
            tlpMain.Controls.Add(pnlRow2Outer, 0, 1);
            tlpMain.Controls.Add(pnlRow3, 0, 2);
            tlpMain.Controls.Add(pnlExtra, 0, 3);
            tlpMain.Dock = System.Windows.Forms.DockStyle.Top;
            tlpMain.Location = new System.Drawing.Point(0, 0);
            tlpMain.Margin = new System.Windows.Forms.Padding(0);
            tlpMain.Name = "tlpMain";
            tlpMain.RowCount = 4;
            tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tlpMain.Size = new System.Drawing.Size(0, 262);
            tlpMain.TabIndex = 0;
            // 
            // pnlRow1
            // 
            pnlRow1.AutoSize = true;
            pnlRow1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            pnlRow1.Controls.Add(btZoom);
            pnlRow1.Controls.Add(cbSlow2);
            pnlRow1.Controls.Add(cbSlow);
            pnlRow1.Controls.Add(cbPause);
            pnlRow1.Controls.Add(btResume);
            pnlRow1.Controls.Add(cbTopMost);
            pnlRow1.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlRow1.Location = new System.Drawing.Point(4, 2);
            pnlRow1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            pnlRow1.Name = "pnlRow1";
            pnlRow1.Padding = new System.Windows.Forms.Padding(4);
            pnlRow1.Size = new System.Drawing.Size(1, 58);
            pnlRow1.TabIndex = 0;
            pnlRow1.WrapContents = false;
            // 
            // btZoom
            // 
            btZoom.Location = new System.Drawing.Point(8, 6);
            btZoom.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btZoom.Name = "btZoom";
            btZoom.Size = new System.Drawing.Size(120, 46);
            btZoom.TabIndex = 0;
            btZoom.Text = "Zoom";
            btZoom.UseVisualStyleBackColor = true;
            btZoom.Click += btZoom_Click;
            // 
            // cbSlow2
            // 
            cbSlow2.AutoSize = true;
            cbSlow2.Location = new System.Drawing.Point(138, 12);
            cbSlow2.Margin = new System.Windows.Forms.Padding(6, 8, 6, 6);
            cbSlow2.Name = "cbSlow2";
            cbSlow2.Size = new System.Drawing.Size(96, 36);
            cbSlow2.TabIndex = 1;
            cbSlow2.Text = "Slow2";
            cbSlow2.UseVisualStyleBackColor = true;
            cbSlow2.CheckedChanged += cbSlow2_CheckedChanged;
            // 
            // cbSlow
            // 
            cbSlow.AutoSize = true;
            cbSlow.Location = new System.Drawing.Point(246, 12);
            cbSlow.Margin = new System.Windows.Forms.Padding(6, 8, 6, 6);
            cbSlow.Name = "cbSlow";
            cbSlow.Size = new System.Drawing.Size(83, 36);
            cbSlow.TabIndex = 2;
            cbSlow.Text = "Slow";
            cbSlow.UseVisualStyleBackColor = true;
            cbSlow.CheckedChanged += cbSlow_CheckedChanged;
            // 
            // cbPause
            // 
            cbPause.AutoSize = true;
            cbPause.Location = new System.Drawing.Point(341, 12);
            cbPause.Margin = new System.Windows.Forms.Padding(6, 8, 6, 6);
            cbPause.Name = "cbPause";
            cbPause.Size = new System.Drawing.Size(94, 36);
            cbPause.TabIndex = 3;
            cbPause.Text = "Pause";
            cbPause.UseVisualStyleBackColor = true;
            cbPause.CheckedChanged += cbPause_CheckedChanged;
            // 
            // btResume
            // 
            btResume.BackColor = System.Drawing.Color.LightGreen;
            btResume.Location = new System.Drawing.Point(445, 6);
            btResume.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btResume.Name = "btResume";
            btResume.Size = new System.Drawing.Size(132, 46);
            btResume.TabIndex = 4;
            btResume.Text = "Resume";
            btResume.UseVisualStyleBackColor = false;
            btResume.Click += btResume_Click;
            // 
            // cbTopMost
            // 
            cbTopMost.AutoSize = true;
            cbTopMost.Location = new System.Drawing.Point(587, 12);
            cbTopMost.Margin = new System.Windows.Forms.Padding(6, 8, 6, 6);
            cbTopMost.Name = "cbTopMost";
            cbTopMost.Size = new System.Drawing.Size(126, 36);
            cbTopMost.TabIndex = 5;
            cbTopMost.Text = "TopMost";
            toolTip1.SetToolTip(cbTopMost, "Keep this window on top of all others");
            cbTopMost.UseVisualStyleBackColor = true;
            cbTopMost.CheckedChanged += cbTopMost_CheckedChanged;
            // 
            // pnlRow2Outer
            // 
            pnlRow2Outer.AutoSize = true;
            pnlRow2Outer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            pnlRow2Outer.BackColor = System.Drawing.Color.FromArgb(180, 255, 255);
            pnlRow2Outer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlRow2Outer.Controls.Add(pnlRow2);
            pnlRow2Outer.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlRow2Outer.Location = new System.Drawing.Point(8, 66);
            pnlRow2Outer.Margin = new System.Windows.Forms.Padding(8, 4, 8, 4);
            pnlRow2Outer.Name = "pnlRow2Outer";
            pnlRow2Outer.Padding = new System.Windows.Forms.Padding(4);
            pnlRow2Outer.Size = new System.Drawing.Size(1, 68);
            pnlRow2Outer.TabIndex = 1;
            // 
            // pnlRow2
            // 
            pnlRow2.AutoSize = true;
            pnlRow2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            pnlRow2.BackColor = System.Drawing.Color.Transparent;
            pnlRow2.Controls.Add(btREV);
            pnlRow2.Controls.Add(btSkipBackBig);
            pnlRow2.Controls.Add(btSkipBackMed);
            pnlRow2.Controls.Add(btSkipBack);
            pnlRow2.Controls.Add(btPlay);
            pnlRow2.Controls.Add(btSkipFwd);
            pnlRow2.Controls.Add(btSkipFwd1Min);
            pnlRow2.Controls.Add(btEnd);
            pnlRow2.Controls.Add(btGoToEnd);
            pnlRow2.Controls.Add(btSkipFwd2Min);
            pnlRow2.Controls.Add(btFF);
            pnlRow2.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlRow2.Location = new System.Drawing.Point(4, 4);
            pnlRow2.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            pnlRow2.Name = "pnlRow2";
            pnlRow2.Padding = new System.Windows.Forms.Padding(4);
            pnlRow2.Size = new System.Drawing.Size(0, 58);
            pnlRow2.TabIndex = 0;
            pnlRow2.WrapContents = false;
            // 
            // btREV
            // 
            btREV.Location = new System.Drawing.Point(8, 6);
            btREV.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btREV.Name = "btREV";
            btREV.Size = new System.Drawing.Size(76, 46);
            btREV.TabIndex = 0;
            btREV.Text = "REV";
            toolTip1.SetToolTip(btREV, "Reverse playback direction");
            btREV.UseVisualStyleBackColor = true;
            btREV.Click += btREV_Click;
            // 
            // btSkipBackBig
            // 
            btSkipBackBig.Location = new System.Drawing.Point(92, 6);
            btSkipBackBig.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btSkipBackBig.Name = "btSkipBackBig";
            btSkipBackBig.Size = new System.Drawing.Size(60, 46);
            btSkipBackBig.TabIndex = 1;
            btSkipBackBig.Text = "<<";
            toolTip1.SetToolTip(btSkipBackBig, "Skip back 30 s");
            btSkipBackBig.UseVisualStyleBackColor = true;
            // 
            // btSkipBackMed
            // 
            btSkipBackMed.Location = new System.Drawing.Point(160, 6);
            btSkipBackMed.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btSkipBackMed.Name = "btSkipBackMed";
            btSkipBackMed.Size = new System.Drawing.Size(60, 46);
            btSkipBackMed.TabIndex = 2;
            btSkipBackMed.Text = "<<";
            toolTip1.SetToolTip(btSkipBackMed, "Skip back 10 s");
            btSkipBackMed.UseVisualStyleBackColor = true;
            btSkipBackMed.Click += btSkipBackMed_Click;
            // 
            // btSkipBack
            // 
            btSkipBack.Location = new System.Drawing.Point(228, 6);
            btSkipBack.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btSkipBack.Name = "btSkipBack";
            btSkipBack.Size = new System.Drawing.Size(50, 46);
            btSkipBack.TabIndex = 3;
            btSkipBack.Text = "<";
            toolTip1.SetToolTip(btSkipBack, "Skip back 5 s");
            btSkipBack.UseVisualStyleBackColor = true;
            btSkipBack.Click += btSkipBack_Click;
            // 
            // btPlay
            // 
            btPlay.BackColor = System.Drawing.Color.LightGreen;
            btPlay.Location = new System.Drawing.Point(286, 6);
            btPlay.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btPlay.Name = "btPlay";
            btPlay.Size = new System.Drawing.Size(110, 46);
            btPlay.TabIndex = 4;
            btPlay.Text = "play";
            toolTip1.SetToolTip(btPlay, "Play selected file");
            btPlay.UseVisualStyleBackColor = false;
            btPlay.Click += btPlay_Click;
            // 
            // btSkipFwd
            // 
            btSkipFwd.Location = new System.Drawing.Point(404, 6);
            btSkipFwd.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btSkipFwd.Name = "btSkipFwd";
            btSkipFwd.Size = new System.Drawing.Size(50, 46);
            btSkipFwd.TabIndex = 5;
            btSkipFwd.Text = ">";
            toolTip1.SetToolTip(btSkipFwd, "Skip forward 10 s");
            btSkipFwd.UseVisualStyleBackColor = true;
            btSkipFwd.Click += btSkipFwd_Click;
            // 
            // btSkipFwd1Min
            // 
            btSkipFwd1Min.Location = new System.Drawing.Point(462, 6);
            btSkipFwd1Min.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btSkipFwd1Min.Name = "btSkipFwd1Min";
            btSkipFwd1Min.Size = new System.Drawing.Size(60, 46);
            btSkipFwd1Min.TabIndex = 6;
            btSkipFwd1Min.Text = ">>";
            toolTip1.SetToolTip(btSkipFwd1Min, "Skip forward 1 min");
            btSkipFwd1Min.UseVisualStyleBackColor = true;
            btSkipFwd1Min.Click += btSkipFwd1Min_Click;
            // 
            // btEnd
            // 
            btEnd.Location = new System.Drawing.Point(530, 6);
            btEnd.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btEnd.Name = "btEnd";
            btEnd.Size = new System.Drawing.Size(50, 46);
            btEnd.TabIndex = 7;
            btEnd.Text = "E";
            toolTip1.SetToolTip(btEnd, "Go to end − 15 s");
            btEnd.UseVisualStyleBackColor = true;
            btEnd.Click += btEnd_Click;
            // 
            // btGoToEnd
            // 
            btGoToEnd.Location = new System.Drawing.Point(588, 6);
            btGoToEnd.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btGoToEnd.Name = "btGoToEnd";
            btGoToEnd.Size = new System.Drawing.Size(50, 46);
            btGoToEnd.TabIndex = 8;
            btGoToEnd.Text = "V";
            toolTip1.SetToolTip(btGoToEnd, "Go to end − 5 s");
            btGoToEnd.UseVisualStyleBackColor = true;
            btGoToEnd.Click += btGoToEnd_Click;
            // 
            // btSkipFwd2Min
            // 
            btSkipFwd2Min.Location = new System.Drawing.Point(646, 6);
            btSkipFwd2Min.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btSkipFwd2Min.Name = "btSkipFwd2Min";
            btSkipFwd2Min.Size = new System.Drawing.Size(60, 46);
            btSkipFwd2Min.TabIndex = 9;
            btSkipFwd2Min.Text = ">>";
            toolTip1.SetToolTip(btSkipFwd2Min, "Skip forward 2 min");
            btSkipFwd2Min.UseVisualStyleBackColor = true;
            btSkipFwd2Min.Click += btSkipFwd2Min_Click;
            // 
            // btFF
            // 
            btFF.Location = new System.Drawing.Point(714, 6);
            btFF.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btFF.Name = "btFF";
            btFF.Size = new System.Drawing.Size(60, 46);
            btFF.TabIndex = 10;
            btFF.Text = "FF";
            toolTip1.SetToolTip(btFF, "Fast-forward 3×");
            btFF.UseVisualStyleBackColor = true;
            btFF.Click += btFF_Click;
            // 
            // pnlRow3
            // 
            pnlRow3.AutoSize = true;
            pnlRow3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            pnlRow3.Controls.Add(cbR90);
            pnlRow3.Controls.Add(btSaveR);
            pnlRow3.Controls.Add(cbSI);
            pnlRow3.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlRow3.Location = new System.Drawing.Point(4, 140);
            pnlRow3.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            pnlRow3.Name = "pnlRow3";
            pnlRow3.Padding = new System.Windows.Forms.Padding(4);
            pnlRow3.Size = new System.Drawing.Size(1, 58);
            pnlRow3.TabIndex = 2;
            pnlRow3.WrapContents = false;
            // 
            // cbR90
            // 
            cbR90.AutoSize = true;
            cbR90.Location = new System.Drawing.Point(10, 12);
            cbR90.Margin = new System.Windows.Forms.Padding(6, 8, 6, 6);
            cbR90.Name = "cbR90";
            cbR90.Size = new System.Drawing.Size(73, 36);
            cbR90.TabIndex = 0;
            cbR90.Text = "R90";
            toolTip1.SetToolTip(cbR90, "Rotate video 90°");
            cbR90.UseVisualStyleBackColor = true;
            // 
            // btSaveR
            // 
            btSaveR.Location = new System.Drawing.Point(93, 6);
            btSaveR.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btSaveR.Name = "btSaveR";
            btSaveR.Size = new System.Drawing.Size(110, 46);
            btSaveR.TabIndex = 1;
            btSaveR.Text = "saveR";
            toolTip1.SetToolTip(btSaveR, "Apply rotation and resume");
            btSaveR.UseVisualStyleBackColor = true;
            // 
            // cbSI
            // 
            cbSI.AutoSize = true;
            cbSI.Location = new System.Drawing.Point(213, 12);
            cbSI.Margin = new System.Windows.Forms.Padding(6, 8, 6, 6);
            cbSI.Name = "cbSI";
            cbSI.Size = new System.Drawing.Size(53, 36);
            cbSI.TabIndex = 2;
            cbSI.Text = "S!";
            toolTip1.SetToolTip(cbSI, "Auto-save list after each action");
            cbSI.UseVisualStyleBackColor = true;
            // 
            // pnlExtra
            // 
            pnlExtra.AutoSize = true;
            pnlExtra.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            pnlExtra.Controls.Add(btMute);
            pnlExtra.Controls.Add(btMoveMovie);
            pnlExtra.Controls.Add(btCloseMovie);
            pnlExtra.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlExtra.Location = new System.Drawing.Point(4, 202);
            pnlExtra.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            pnlExtra.Name = "pnlExtra";
            pnlExtra.Padding = new System.Windows.Forms.Padding(4);
            pnlExtra.Size = new System.Drawing.Size(1, 58);
            pnlExtra.TabIndex = 3;
            pnlExtra.WrapContents = false;
            // 
            // btMute
            // 
            btMute.Location = new System.Drawing.Point(8, 6);
            btMute.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btMute.Name = "btMute";
            btMute.Size = new System.Drawing.Size(116, 46);
            btMute.TabIndex = 0;
            btMute.Text = "Mute";
            toolTip1.SetToolTip(btMute, "Toggle mute");
            btMute.UseVisualStyleBackColor = true;
            btMute.Click += btMute_Click;
            // 
            // btMoveMovie
            // 
            btMoveMovie.Location = new System.Drawing.Point(132, 6);
            btMoveMovie.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btMoveMovie.Name = "btMoveMovie";
            btMoveMovie.Size = new System.Drawing.Size(160, 46);
            btMoveMovie.TabIndex = 1;
            btMoveMovie.Text = "MoveMovie";
            toolTip1.SetToolTip(btMoveMovie, "Move player to next screen");
            btMoveMovie.UseVisualStyleBackColor = true;
            btMoveMovie.Click += btMoveMovie_Click;
            // 
            // btCloseMovie
            // 
            btCloseMovie.BackColor = System.Drawing.Color.LightCoral;
            btCloseMovie.Location = new System.Drawing.Point(300, 6);
            btCloseMovie.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            btCloseMovie.Name = "btCloseMovie";
            btCloseMovie.Size = new System.Drawing.Size(176, 46);
            btCloseMovie.TabIndex = 2;
            btCloseMovie.Text = "CloseMovie";
            toolTip1.SetToolTip(btCloseMovie, "Close and dispose player window");
            btCloseMovie.UseVisualStyleBackColor = false;
            btCloseMovie.Click += btCloseMovie_Click;
            // 
            // PlaybackControl
            // 
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            Controls.Add(tlpMain);
            Font = new System.Drawing.Font("Segoe UI", 18F);
            Name = "PlaybackControl";
            Size = new System.Drawing.Size(0, 262);
            tlpMain.ResumeLayout(false);
            tlpMain.PerformLayout();
            pnlRow1.ResumeLayout(false);
            pnlRow1.PerformLayout();
            pnlRow2Outer.ResumeLayout(false);
            pnlRow2Outer.PerformLayout();
            pnlRow2.ResumeLayout(false);
            pnlRow3.ResumeLayout(false);
            pnlRow3.PerformLayout();
            pnlExtra.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        // ── Row 1 ────────────────────────────────────────────────────────────────────
        private System.Windows.Forms.FlowLayoutPanel pnlRow1;
        private System.Windows.Forms.Button          btZoom;
        private System.Windows.Forms.CheckBox        cbSlow2;
        private System.Windows.Forms.CheckBox        cbSlow;
        private System.Windows.Forms.CheckBox        cbPause;
        private System.Windows.Forms.Button          btResume;
        private System.Windows.Forms.CheckBox        cbTopMost;

        // ── Row 2 (cyan transport strip) ─────────────────────────────────────────────
        private System.Windows.Forms.Panel           pnlRow2Outer;
        private System.Windows.Forms.FlowLayoutPanel pnlRow2;
        private System.Windows.Forms.Button          btREV;
        private System.Windows.Forms.Button          btSkipBackBig;
        private System.Windows.Forms.Button          btSkipBackMed;
        private System.Windows.Forms.Button          btSkipBack;
        private System.Windows.Forms.Button          btPlay;
        private System.Windows.Forms.Button          btSkipFwd;
        private System.Windows.Forms.Button          btSkipFwd1Min;
        private System.Windows.Forms.Button          btEnd;
        private System.Windows.Forms.Button          btGoToEnd;
        private System.Windows.Forms.Button          btSkipFwd2Min;
        private System.Windows.Forms.Button          btFF;

        // ── Row 3 ────────────────────────────────────────────────────────────────────
        private System.Windows.Forms.FlowLayoutPanel pnlRow3;
        private System.Windows.Forms.CheckBox        cbR90;
        private System.Windows.Forms.Button          btSaveR;
        private System.Windows.Forms.CheckBox        cbSI;

        // ── Extra row ────────────────────────────────────────────────────────────────
        private System.Windows.Forms.FlowLayoutPanel pnlExtra;
        private System.Windows.Forms.Button          btMute;
        private System.Windows.Forms.Button          btMoveMovie;
        private System.Windows.Forms.Button          btCloseMovie;

        // ── Outer container ──────────────────────────────────────────────────────────
        private System.Windows.Forms.TableLayoutPanel tlpMain;

        // ── ToolTip ──────────────────────────────────────────────────────────────────
        private System.Windows.Forms.ToolTip          toolTip1;
    }
}