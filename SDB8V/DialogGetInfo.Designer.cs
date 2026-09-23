namespace SymbolDB
{
    partial class DialogGetInfo
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
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            tbX = new System.Windows.Forms.TextBox();
            tbY = new System.Windows.Forms.TextBox();
            tbWidth = new System.Windows.Forms.TextBox();
            tbHeight = new System.Windows.Forms.TextBox();
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            tbName = new System.Windows.Forms.TextBox();
            tbDescription = new System.Windows.Forms.TextBox();
            tbKeys = new System.Windows.Forms.TextBox();
            btReset = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            rtbImageFullPath = new System.Windows.Forms.RichTextBox();
            btClose = new System.Windows.Forms.Button();
            pb1 = new System.Windows.Forms.PictureBox();
            btSave = new System.Windows.Forms.Button();
            btDisplayTag = new System.Windows.Forms.Button();
            pbx1 = new ZoomablePictureBox();
            btPbxShow = new System.Windows.Forms.Button();
            btMaxSize = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)pb1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbx1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(43, 15);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(14, 15);
            label1.TabIndex = 0;
            label1.Text = "X";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(43, 45);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(14, 15);
            label2.TabIndex = 1;
            label2.Text = "Y";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(43, 79);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(39, 15);
            label3.TabIndex = 2;
            label3.Text = "Width";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(43, 113);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(43, 15);
            label4.TabIndex = 3;
            label4.Text = "Height";
            // 
            // tbX
            // 
            tbX.Location = new System.Drawing.Point(127, 15);
            tbX.Margin = new System.Windows.Forms.Padding(4);
            tbX.Name = "tbX";
            tbX.Size = new System.Drawing.Size(116, 23);
            tbX.TabIndex = 4;
            // 
            // tbY
            // 
            tbY.Location = new System.Drawing.Point(127, 45);
            tbY.Margin = new System.Windows.Forms.Padding(4);
            tbY.Name = "tbY";
            tbY.Size = new System.Drawing.Size(116, 23);
            tbY.TabIndex = 5;
            // 
            // tbWidth
            // 
            tbWidth.Location = new System.Drawing.Point(127, 75);
            tbWidth.Margin = new System.Windows.Forms.Padding(4);
            tbWidth.Name = "tbWidth";
            tbWidth.Size = new System.Drawing.Size(116, 23);
            tbWidth.TabIndex = 6;
            // 
            // tbHeight
            // 
            tbHeight.Location = new System.Drawing.Point(127, 105);
            tbHeight.Margin = new System.Windows.Forms.Padding(4);
            tbHeight.Name = "tbHeight";
            tbHeight.Size = new System.Drawing.Size(116, 23);
            tbHeight.TabIndex = 7;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new System.Drawing.Point(-1, 465);
            richTextBox1.Margin = new System.Windows.Forms.Padding(4);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new System.Drawing.Size(1125, 190);
            richTextBox1.TabIndex = 8;
            richTextBox1.Text = "";
            // 
            // tbName
            // 
            tbName.Location = new System.Drawing.Point(54, 262);
            tbName.Margin = new System.Windows.Forms.Padding(4);
            tbName.Name = "tbName";
            tbName.Size = new System.Drawing.Size(501, 23);
            tbName.TabIndex = 9;
            // 
            // tbDescription
            // 
            tbDescription.Location = new System.Drawing.Point(75, 336);
            tbDescription.Margin = new System.Windows.Forms.Padding(4);
            tbDescription.Name = "tbDescription";
            tbDescription.Size = new System.Drawing.Size(1005, 23);
            tbDescription.TabIndex = 10;
            // 
            // tbKeys
            // 
            tbKeys.Location = new System.Drawing.Point(48, 222);
            tbKeys.Margin = new System.Windows.Forms.Padding(4);
            tbKeys.Name = "tbKeys";
            tbKeys.Size = new System.Drawing.Size(504, 23);
            tbKeys.TabIndex = 11;
            // 
            // btReset
            // 
            btReset.Location = new System.Drawing.Point(209, 662);
            btReset.Margin = new System.Windows.Forms.Padding(4);
            btReset.Name = "btReset";
            btReset.Size = new System.Drawing.Size(88, 26);
            btReset.TabIndex = 12;
            btReset.Text = "move tag";
            btReset.UseVisualStyleBackColor = true;
            btReset.Click += btReset_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(456, 662);
            button1.Margin = new System.Windows.Forms.Padding(4);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(88, 26);
            button1.TabIndex = 13;
            button1.Text = "Exit program";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(8, 265);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(39, 15);
            label5.TabIndex = 15;
            label5.Text = "Name";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(11, 335);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(32, 15);
            label6.TabIndex = 16;
            label6.Text = "Desc";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(11, 222);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(26, 15);
            label7.TabIndex = 17;
            label7.Text = "Key";
            // 
            // rtbImageFullPath
            // 
            rtbImageFullPath.Location = new System.Drawing.Point(-1, 366);
            rtbImageFullPath.Margin = new System.Windows.Forms.Padding(4);
            rtbImageFullPath.Name = "rtbImageFullPath";
            rtbImageFullPath.Size = new System.Drawing.Size(1114, 94);
            rtbImageFullPath.TabIndex = 18;
            rtbImageFullPath.Text = "";
            // 
            // btClose
            // 
            btClose.Location = new System.Drawing.Point(659, 662);
            btClose.Margin = new System.Windows.Forms.Padding(4);
            btClose.Name = "btClose";
            btClose.Size = new System.Drawing.Size(88, 26);
            btClose.TabIndex = 19;
            btClose.Text = "Close";
            btClose.UseVisualStyleBackColor = true;
            btClose.Click += btClose_Click_1;
            // 
            // pb1
            // 
            pb1.Location = new System.Drawing.Point(562, 4);
            pb1.Name = "pb1";
            pb1.Size = new System.Drawing.Size(570, 325);
            pb1.TabIndex = 20;
            pb1.TabStop = false;
            // 
            // btSave
            // 
            btSave.Location = new System.Drawing.Point(884, 662);
            btSave.Name = "btSave";
            btSave.Size = new System.Drawing.Size(66, 22);
            btSave.TabIndex = 21;
            btSave.Text = "Save";
            btSave.UseVisualStyleBackColor = true;
            btSave.Click += btSave_Click;
            // 
            // btDisplayTag
            // 
            btDisplayTag.Location = new System.Drawing.Point(326, 662);
            btDisplayTag.Margin = new System.Windows.Forms.Padding(4);
            btDisplayTag.Name = "btDisplayTag";
            btDisplayTag.Size = new System.Drawing.Size(88, 26);
            btDisplayTag.TabIndex = 22;
            btDisplayTag.Text = "Display Tag";
            btDisplayTag.UseVisualStyleBackColor = true;
            btDisplayTag.Click += bDisplayTag_Click;
            // 
            // pbx1
            // 
            pbx1.Location = new System.Drawing.Point(559, 4);
            pbx1.Name = "pbx1";
            pbx1.Size = new System.Drawing.Size(563, 316);
            pbx1.TabIndex = 23;
            pbx1.TabStop = false;
            pbx1.DoubleClick += pbx1_DoubleClick;
            // 
            // btPbxShow
            // 
            btPbxShow.Location = new System.Drawing.Point(306, 43);
            btPbxShow.Name = "btPbxShow";
            btPbxShow.Size = new System.Drawing.Size(68, 32);
            btPbxShow.TabIndex = 24;
            btPbxShow.Text = "pbx";
            btPbxShow.UseVisualStyleBackColor = true;
            btPbxShow.Click += btPbxShow_Click;
            // 
            // btMaxSize
            // 
            btMaxSize.Location = new System.Drawing.Point(306, 96);
            btMaxSize.Name = "btMaxSize";
            btMaxSize.Size = new System.Drawing.Size(68, 32);
            btMaxSize.TabIndex = 25;
            btMaxSize.Text = "Max Size";
            btMaxSize.UseVisualStyleBackColor = true;
            btMaxSize.Click += btMaxSize_Click;
            // 
            // DialogGetInfo
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1135, 692);
            Controls.Add(btMaxSize);
            Controls.Add(btPbxShow);
            Controls.Add(pbx1);
            Controls.Add(btDisplayTag);
            Controls.Add(btSave);
            Controls.Add(btClose);
            Controls.Add(rtbImageFullPath);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(button1);
            Controls.Add(btReset);
            Controls.Add(tbKeys);
            Controls.Add(tbDescription);
            Controls.Add(tbName);
            Controls.Add(richTextBox1);
            Controls.Add(tbHeight);
            Controls.Add(tbWidth);
            Controls.Add(tbY);
            Controls.Add(tbX);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pb1);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "DialogGetInfo";
            Text = "C:\\_ws\\DavidMcClanahan";
            FormClosing += DialogGetInfo_FormClosing;
            Load += DialogGetInfo_Load;
            Move += DialogGetInfo_Move;
            ((System.ComponentModel.ISupportInitialize)pb1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbx1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbX;
        private System.Windows.Forms.TextBox tbY;
        private System.Windows.Forms.TextBox tbWidth;
        private System.Windows.Forms.TextBox tbHeight;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.TextBox tbKeys;
        private System.Windows.Forms.Button btReset;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RichTextBox rtbImageFullPath;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.PictureBox pb1;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btDisplayTag;
        private SymbolDB.ZoomablePictureBox pbx1;
        private System.Windows.Forms.Button btPbxShow;
        private System.Windows.Forms.Button btMaxSize;
    }
}