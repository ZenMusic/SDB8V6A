namespace SymbolDB
{
    partial class Movies
    {
       // public AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        /// <summary>
        /// Required designer variable.
        /// </summary>
       // private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Movies));
            this.cbMute = new System.Windows.Forms.CheckBox();
            this.cbSetFocusOnParent = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.numIntTimerSeconds = new System.Windows.Forms.NumericUpDown();
            this.currentStateLabel = new System.Windows.Forms.TextBox();
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
          //  this.axWindowsMediaPlayer1.CreateControl();
            ((System.ComponentModel.ISupportInitialize)(this.numIntTimerSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.SuspendLayout();
            // 
            // cbMute
            // 
            this.cbMute.AutoSize = true;
            this.cbMute.Location = new System.Drawing.Point(92, 17);
            this.cbMute.Name = "cbMute";
            this.cbMute.Size = new System.Drawing.Size(61, 21);
            this.cbMute.TabIndex = 9;
            this.cbMute.Text = "mute";
            this.cbMute.UseVisualStyleBackColor = true;
            this.cbMute.CheckedChanged += new System.EventHandler(this.cbMute_CheckedChanged);
            // 
            // cbSetFocusOnParent
            // 
            this.cbSetFocusOnParent.AutoSize = true;
            this.cbSetFocusOnParent.Location = new System.Drawing.Point(159, 17);
            this.cbSetFocusOnParent.Name = "cbSetFocusOnParent";
            this.cbSetFocusOnParent.Size = new System.Drawing.Size(64, 21);
            this.cbSetFocusOnParent.TabIndex = 12;
            this.cbSetFocusOnParent.Text = "focus";
            this.cbSetFocusOnParent.UseVisualStyleBackColor = true;
            this.cbSetFocusOnParent.CheckedChanged += new System.EventHandler(this.cbSetFocusOnParent_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // numIntTimerSeconds
            // 
            this.numIntTimerSeconds.Location = new System.Drawing.Point(21, 16);
            this.numIntTimerSeconds.Name = "numIntTimerSeconds";
            this.numIntTimerSeconds.Size = new System.Drawing.Size(53, 22);
            this.numIntTimerSeconds.TabIndex = 14;
            this.numIntTimerSeconds.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // currentStateLabel
            // 
            this.currentStateLabel.Location = new System.Drawing.Point(244, 16);
            this.currentStateLabel.Name = "currentStateLabel";
            this.currentStateLabel.Size = new System.Drawing.Size(100, 22);
            this.currentStateLabel.TabIndex = 17;
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.axWindowsMediaPlayer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(-12, 2);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(1998, 944);
            this.axWindowsMediaPlayer1.TabIndex = 3;
            this.axWindowsMediaPlayer1.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.axWindowsMediaPlayer1_PlayStateChange);
            this.axWindowsMediaPlayer1.NewStream += new System.EventHandler(this.axWindowsMediaPlayer1_NewStream);
            this.axWindowsMediaPlayer1.EndOfStream += new AxWMPLib._WMPOCXEvents_EndOfStreamEventHandler(this.AxWindowsMediaPlayer1_EndOfStream);
            this.axWindowsMediaPlayer1.MediaError += new AxWMPLib._WMPOCXEvents_MediaErrorEventHandler(this.axWindowsMediaPlayer1_MediaError);
            this.axWindowsMediaPlayer1.KeyDownEvent += new AxWMPLib._WMPOCXEvents_KeyDownEventHandler(this.axWindowsMediaPlayer1_KeyDownEvent);
            this.axWindowsMediaPlayer1.KeyPressEvent += new AxWMPLib._WMPOCXEvents_KeyPressEventHandler(this.axWindowsMediaPlayer1_KeyPressEvent);
            // 
            // Movies
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1998, 944);
            this.Controls.Add(this.currentStateLabel);
            this.Controls.Add(this.numIntTimerSeconds);
            this.Controls.Add(this.cbSetFocusOnParent);
            this.Controls.Add(this.cbMute);
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.Name = "Movies";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "movies";
            this.Activated += new System.EventHandler(this.Movies_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Movies_FormClosed);
            this.Load += new System.EventHandler(this.Movies_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Movies_KeyPress);
            this.Resize += new System.EventHandler(this.Movies_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.numIntTimerSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.CheckBox cbMute;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.CheckBox cbSetFocusOnParent;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.NumericUpDown numIntTimerSeconds;
        private System.Windows.Forms.TextBox currentStateLabel;
    }
}