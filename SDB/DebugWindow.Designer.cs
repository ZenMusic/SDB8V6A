namespace SymbolDB
{
    partial class DebugWindow
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.pb1 = new System.Windows.Forms.PictureBox();
            this.relocate_button = new System.Windows.Forms.Button();
            this.cbDisplayImage = new System.Windows.Forms.CheckBox();
            this.cbDisplay1 = new System.Windows.Forms.CheckBox();
            this.cbPosition1 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.btParms = new System.Windows.Forms.Button();
            this.btMain = new System.Windows.Forms.Button();
            this.btReset = new System.Windows.Forms.Button();
            this.cbDisplaySave = new System.Windows.Forms.CheckBox();
            this.cbDisplayInfo = new System.Windows.Forms.CheckBox();
            this.cbMonitor = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.cbDisplayNumber = new System.Windows.Forms.CheckBox();
            this.btHideDebug2 = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.Location = new System.Drawing.Point(-1, 12);
            this.listBox1.MaximumSize = new System.Drawing.Size(1441, 878);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(816, 771);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(854, 446);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Exit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pb1
            // 
            this.pb1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb1.Location = new System.Drawing.Point(819, 11);
            this.pb1.Name = "pb1";
            this.pb1.Size = new System.Drawing.Size(275, 377);
            this.pb1.TabIndex = 2;
            this.pb1.TabStop = false;
            // 
            // relocate_button
            // 
            this.relocate_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.relocate_button.Location = new System.Drawing.Point(987, 446);
            this.relocate_button.Name = "relocate_button";
            this.relocate_button.Size = new System.Drawing.Size(75, 23);
            this.relocate_button.TabIndex = 3;
            this.relocate_button.Text = "replace";
            this.relocate_button.UseVisualStyleBackColor = true;
            this.relocate_button.Click += new System.EventHandler(this.relocate_button_Click);
            // 
            // cbDisplayImage
            // 
            this.cbDisplayImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDisplayImage.AutoSize = true;
            this.cbDisplayImage.Location = new System.Drawing.Point(849, 535);
            this.cbDisplayImage.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbDisplayImage.Name = "cbDisplayImage";
            this.cbDisplayImage.Size = new System.Drawing.Size(92, 17);
            this.cbDisplayImage.TabIndex = 4;
            this.cbDisplayImage.Text = "Display Image";
            this.cbDisplayImage.UseVisualStyleBackColor = true;
            this.cbDisplayImage.CheckedChanged += new System.EventHandler(this.cbDisplayImage_CheckedChanged);
            // 
            // cbDisplay1
            // 
            this.cbDisplay1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDisplay1.AutoSize = true;
            this.cbDisplay1.Location = new System.Drawing.Point(946, 521);
            this.cbDisplay1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbDisplay1.Name = "cbDisplay1";
            this.cbDisplay1.Size = new System.Drawing.Size(66, 17);
            this.cbDisplay1.TabIndex = 5;
            this.cbDisplay1.Text = "Display1";
            this.cbDisplay1.UseVisualStyleBackColor = true;
            this.cbDisplay1.CheckedChanged += new System.EventHandler(this.cbDisplay1_CheckedChanged);
            // 
            // cbPosition1
            // 
            this.cbPosition1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbPosition1.AutoSize = true;
            this.cbPosition1.Location = new System.Drawing.Point(946, 554);
            this.cbPosition1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbPosition1.Name = "cbPosition1";
            this.cbPosition1.Size = new System.Drawing.Size(72, 17);
            this.cbPosition1.TabIndex = 6;
            this.cbPosition1.Text = "Position 1";
            this.cbPosition1.UseVisualStyleBackColor = true;
            this.cbPosition1.CheckedChanged += new System.EventHandler(this.cbPosition1_CheckedChanged);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(838, 408);
            this.button2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 22);
            this.button2.TabIndex = 7;
            this.button2.Text = "close display2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btStop
            // 
            this.btStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btStop.Location = new System.Drawing.Point(1029, 402);
            this.btStop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(56, 32);
            this.btStop.TabIndex = 8;
            this.btStop.Text = "stop";
            this.btStop.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(824, 780);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(66, 17);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "Display2";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(899, 780);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(66, 17);
            this.checkBox2.TabIndex = 10;
            this.checkBox2.Text = "Display2";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(980, 780);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(66, 17);
            this.checkBox3.TabIndex = 11;
            this.checkBox3.Text = "Display2";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // btParms
            // 
            this.btParms.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btParms.Location = new System.Drawing.Point(987, 475);
            this.btParms.Name = "btParms";
            this.btParms.Size = new System.Drawing.Size(98, 23);
            this.btParms.TabIndex = 12;
            this.btParms.Text = "browser parms";
            this.btParms.UseVisualStyleBackColor = true;
            // 
            // btMain
            // 
            this.btMain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btMain.Location = new System.Drawing.Point(980, 577);
            this.btMain.Name = "btMain";
            this.btMain.Size = new System.Drawing.Size(98, 23);
            this.btMain.TabIndex = 13;
            this.btMain.Text = "Main Window";
            this.btMain.UseVisualStyleBackColor = true;
            this.btMain.Click += new System.EventHandler(this.btMain_Click);
            // 
            // btReset
            // 
            this.btReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btReset.Location = new System.Drawing.Point(843, 577);
            this.btReset.Name = "btReset";
            this.btReset.Size = new System.Drawing.Size(98, 23);
            this.btReset.TabIndex = 14;
            this.btReset.Text = "reset PBx";
            this.btReset.UseVisualStyleBackColor = true;
            this.btReset.Click += new System.EventHandler(this.btReset_Click);
            // 
            // cbDisplaySave
            // 
            this.cbDisplaySave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDisplaySave.AutoSize = true;
            this.cbDisplaySave.Location = new System.Drawing.Point(1013, 535);
            this.cbDisplaySave.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbDisplaySave.Name = "cbDisplaySave";
            this.cbDisplaySave.Size = new System.Drawing.Size(88, 17);
            this.cbDisplaySave.TabIndex = 15;
            this.cbDisplaySave.Text = "displaySaves";
            this.cbDisplaySave.UseVisualStyleBackColor = true;
            this.cbDisplaySave.CheckedChanged += new System.EventHandler(this.cbDisplaySave_CheckedChanged);
            // 
            // cbDisplayInfo
            // 
            this.cbDisplayInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDisplayInfo.AutoSize = true;
            this.cbDisplayInfo.Checked = true;
            this.cbDisplayInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDisplayInfo.Location = new System.Drawing.Point(967, 760);
            this.cbDisplayInfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbDisplayInfo.Name = "cbDisplayInfo";
            this.cbDisplayInfo.Size = new System.Drawing.Size(116, 17);
            this.cbDisplayInfo.TabIndex = 16;
            this.cbDisplayInfo.Text = "Display Debug Info";
            this.cbDisplayInfo.UseVisualStyleBackColor = true;
            this.cbDisplayInfo.CheckedChanged += new System.EventHandler(this.cbDisplayInfo_CheckedChanged);
            // 
            // cbMonitor
            // 
            this.cbMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMonitor.AutoSize = true;
            this.cbMonitor.Location = new System.Drawing.Point(853, 500);
            this.cbMonitor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbMonitor.Name = "cbMonitor";
            this.cbMonitor.Size = new System.Drawing.Size(74, 17);
            this.cbMonitor.TabIndex = 17;
            this.cbMonitor.Text = "monitor dir";
            this.cbMonitor.UseVisualStyleBackColor = true;
            this.cbMonitor.CheckedChanged += new System.EventHandler(this.cbMonitor_CheckedChanged);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(1006, 609);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(74, 23);
            this.button3.TabIndex = 18;
            this.button3.Text = "Hide";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // cbDisplayNumber
            // 
            this.cbDisplayNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDisplayNumber.AutoSize = true;
            this.cbDisplayNumber.Location = new System.Drawing.Point(934, 729);
            this.cbDisplayNumber.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbDisplayNumber.Name = "cbDisplayNumber";
            this.cbDisplayNumber.Size = new System.Drawing.Size(68, 17);
            this.cbDisplayNumber.TabIndex = 19;
            this.cbDisplayNumber.Text = "display #";
            this.cbDisplayNumber.UseVisualStyleBackColor = true;
            this.cbDisplayNumber.CheckedChanged += new System.EventHandler(this.cbDisplayNumber_CheckedChanged);
            // 
            // btHideDebug2
            // 
            this.btHideDebug2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btHideDebug2.Location = new System.Drawing.Point(843, 609);
            this.btHideDebug2.Name = "btHideDebug2";
            this.btHideDebug2.Size = new System.Drawing.Size(98, 23);
            this.btHideDebug2.TabIndex = 20;
            this.btHideDebug2.Text = "hide Debug";
            this.btHideDebug2.UseVisualStyleBackColor = true;
            this.btHideDebug2.Click += new System.EventHandler(this.btHideDebug_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1.Location = new System.Drawing.Point(864, 647);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(53, 23);
            this.numericUpDown1.TabIndex = 21;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(983, 659);
            this.button4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(72, 27);
            this.button4.TabIndex = 22;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // DebugWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1104, 807);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.btHideDebug2);
            this.Controls.Add(this.cbDisplayNumber);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.cbMonitor);
            this.Controls.Add(this.cbDisplayInfo);
            this.Controls.Add(this.cbDisplaySave);
            this.Controls.Add(this.btReset);
            this.Controls.Add(this.btMain);
            this.Controls.Add(this.btParms);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cbPosition1);
            this.Controls.Add(this.cbDisplay1);
            this.Controls.Add(this.cbDisplayImage);
            this.Controls.Add(this.relocate_button);
            this.Controls.Add(this.pb1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox1);
            this.Location = new System.Drawing.Point(100, 100);
            this.Name = "DebugWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "debug info";
            this.Activated += new System.EventHandler(this.DebugWindow_Activated);
            this.Load += new System.EventHandler(this.winDebug_Load);
            this.Resize += new System.EventHandler(this.DebugWindow_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pb1;
        private System.Windows.Forms.Button relocate_button;
        private System.Windows.Forms.CheckBox cbDisplayImage;
        private System.Windows.Forms.CheckBox cbDisplay1;
        private System.Windows.Forms.CheckBox cbPosition1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Button btParms;
        private System.Windows.Forms.Button btMain;
        private System.Windows.Forms.Button btReset;
        private System.Windows.Forms.CheckBox cbDisplaySave;
        private System.Windows.Forms.CheckBox cbDisplayInfo;
        private System.Windows.Forms.CheckBox cbMonitor;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox cbDisplayNumber;
        private System.Windows.Forms.Button btHideDebug2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button button4;
    }
}