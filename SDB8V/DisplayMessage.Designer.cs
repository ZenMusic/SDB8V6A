namespace SymbolDB
{
    partial class DisplayMessage
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
            cbTopMost = new System.Windows.Forms.CheckBox();
            tbMessage = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            tbCopy2 = new System.Windows.Forms.TextBox();
            btHideMovie = new System.Windows.Forms.Button();
            tbCopy3 = new System.Windows.Forms.TextBox();
            lblAAA = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            tbAAA = new System.Windows.Forms.TextBox();
            tbFileName = new System.Windows.Forms.TextBox();
            tbFolder = new System.Windows.Forms.TextBox();
            btDisplayFolder = new System.Windows.Forms.Button();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            tbSourceFolder = new System.Windows.Forms.TextBox();
            tbSpaceAvailable = new System.Windows.Forms.TextBox();
            lblSpaceAvailable = new System.Windows.Forms.Label();
            tbFileSize = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // cbTopMost
            // 
            cbTopMost.AutoSize = true;
            cbTopMost.Checked = true;
            cbTopMost.CheckState = System.Windows.Forms.CheckState.Checked;
            cbTopMost.Location = new System.Drawing.Point(620, 207);
            cbTopMost.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            cbTopMost.Name = "cbTopMost";
            cbTopMost.Size = new System.Drawing.Size(73, 19);
            cbTopMost.TabIndex = 286;
            cbTopMost.Text = "TopMost";
            cbTopMost.UseVisualStyleBackColor = true;
            // 
            // tbMessage
            // 
            tbMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbMessage.Location = new System.Drawing.Point(4, 166);
            tbMessage.Margin = new System.Windows.Forms.Padding(4);
            tbMessage.Name = "tbMessage";
            tbMessage.Size = new System.Drawing.Size(797, 26);
            tbMessage.TabIndex = 287;
            tbMessage.Text = "copy";
            tbMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(19, 11);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(69, 20);
            label1.TabIndex = 288;
            label1.Text = "COPY ...";
            // 
            // tbCopy2
            // 
            tbCopy2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbCopy2.Location = new System.Drawing.Point(4, 42);
            tbCopy2.Margin = new System.Windows.Forms.Padding(4);
            tbCopy2.Name = "tbCopy2";
            tbCopy2.Size = new System.Drawing.Size(791, 26);
            tbCopy2.TabIndex = 289;
            tbCopy2.Text = "copy";
            tbCopy2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btHideMovie
            // 
            btHideMovie.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btHideMovie.Location = new System.Drawing.Point(479, 200);
            btHideMovie.Margin = new System.Windows.Forms.Padding(4);
            btHideMovie.Name = "btHideMovie";
            btHideMovie.Size = new System.Drawing.Size(70, 31);
            btHideMovie.TabIndex = 303;
            btHideMovie.Text = "Hide M";
            btHideMovie.UseVisualStyleBackColor = true;
            btHideMovie.Click += btHideMovie_Click;
            // 
            // tbCopy3
            // 
            tbCopy3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbCopy3.Location = new System.Drawing.Point(11, 200);
            tbCopy3.Margin = new System.Windows.Forms.Padding(4);
            tbCopy3.Name = "tbCopy3";
            tbCopy3.Size = new System.Drawing.Size(405, 22);
            tbCopy3.TabIndex = 304;
            tbCopy3.Text = "copy";
            tbCopy3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblAAA
            // 
            lblAAA.AutoSize = true;
            lblAAA.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblAAA.Location = new System.Drawing.Point(307, 128);
            lblAAA.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAAA.Name = "lblAAA";
            lblAAA.Size = new System.Drawing.Size(62, 20);
            lblAAA.TabIndex = 305;
            lblAAA.Text = "special ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label2.Location = new System.Drawing.Point(11, 126);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(147, 20);
            label2.TabIndex = 306;
            label2.Text = "Copy TO Subfolder:";
            // 
            // tbAAA
            // 
            tbAAA.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbAAA.Location = new System.Drawing.Point(377, 126);
            tbAAA.Margin = new System.Windows.Forms.Padding(4);
            tbAAA.Name = "tbAAA";
            tbAAA.Size = new System.Drawing.Size(101, 22);
            tbAAA.TabIndex = 307;
            tbAAA.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbFileName
            // 
            tbFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbFileName.Location = new System.Drawing.Point(104, 8);
            tbFileName.Margin = new System.Windows.Forms.Padding(4);
            tbFileName.Name = "tbFileName";
            tbFileName.Size = new System.Drawing.Size(691, 26);
            tbFileName.TabIndex = 308;
            tbFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbFolder
            // 
            tbFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbFolder.Location = new System.Drawing.Point(156, 123);
            tbFolder.Margin = new System.Windows.Forms.Padding(4);
            tbFolder.Name = "tbFolder";
            tbFolder.Size = new System.Drawing.Size(108, 28);
            tbFolder.TabIndex = 309;
            tbFolder.Text = "folder";
            tbFolder.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btDisplayFolder
            // 
            btDisplayFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btDisplayFolder.Location = new System.Drawing.Point(599, 126);
            btDisplayFolder.Margin = new System.Windows.Forms.Padding(4);
            btDisplayFolder.Name = "btDisplayFolder";
            btDisplayFolder.Size = new System.Drawing.Size(194, 31);
            btDisplayFolder.TabIndex = 310;
            btDisplayFolder.Text = "Display Folder";
            btDisplayFolder.UseVisualStyleBackColor = true;
            btDisplayFolder.Click += btDisplayFolder_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new System.Drawing.Point(326, 82);
            progressBar1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(112, 22);
            progressBar1.TabIndex = 311;
            // 
            // tbSourceFolder
            // 
            tbSourceFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbSourceFolder.Location = new System.Drawing.Point(4, 76);
            tbSourceFolder.Margin = new System.Windows.Forms.Padding(4);
            tbSourceFolder.Name = "tbSourceFolder";
            tbSourceFolder.Size = new System.Drawing.Size(284, 28);
            tbSourceFolder.TabIndex = 312;
            tbSourceFolder.Text = "folder";
            tbSourceFolder.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbSpaceAvailable
            // 
            tbSpaceAvailable.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbSpaceAvailable.Location = new System.Drawing.Point(692, 81);
            tbSpaceAvailable.Margin = new System.Windows.Forms.Padding(4);
            tbSpaceAvailable.Name = "tbSpaceAvailable";
            tbSpaceAvailable.Size = new System.Drawing.Size(101, 22);
            tbSpaceAvailable.TabIndex = 313;
            tbSpaceAvailable.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblSpaceAvailable
            // 
            lblSpaceAvailable.AutoSize = true;
            lblSpaceAvailable.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblSpaceAvailable.Location = new System.Drawing.Point(557, 81);
            lblSpaceAvailable.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSpaceAvailable.Name = "lblSpaceAvailable";
            lblSpaceAvailable.Size = new System.Drawing.Size(117, 20);
            lblSpaceAvailable.TabIndex = 314;
            lblSpaceAvailable.Text = "space available";
            // 
            // tbFileSize
            // 
            tbFileSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbFileSize.Location = new System.Drawing.Point(469, 81);
            tbFileSize.Margin = new System.Windows.Forms.Padding(4);
            tbFileSize.Name = "tbFileSize";
            tbFileSize.Size = new System.Drawing.Size(80, 22);
            tbFileSize.TabIndex = 315;
            tbFileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DisplayMessage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(806, 236);
            Controls.Add(tbFileSize);
            Controls.Add(lblSpaceAvailable);
            Controls.Add(tbSpaceAvailable);
            Controls.Add(tbSourceFolder);
            Controls.Add(progressBar1);
            Controls.Add(btDisplayFolder);
            Controls.Add(tbFolder);
            Controls.Add(tbFileName);
            Controls.Add(tbAAA);
            Controls.Add(label2);
            Controls.Add(lblAAA);
            Controls.Add(tbCopy3);
            Controls.Add(btHideMovie);
            Controls.Add(tbCopy2);
            Controls.Add(label1);
            Controls.Add(tbMessage);
            Controls.Add(cbTopMost);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "DisplayMessage";
            Text = "DisplayMessage COPY";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbTopMost;
        private System.Windows.Forms.TextBox tbMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbCopy2;
        private System.Windows.Forms.Button btHideMovie;
        private System.Windows.Forms.TextBox tbCopy3;
        private System.Windows.Forms.Label lblAAA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbAAA;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.TextBox tbFolder;
        private System.Windows.Forms.Button btDisplayFolder;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox tbSourceFolder;
        private System.Windows.Forms.TextBox tbSpaceAvailable;
        private System.Windows.Forms.Label lblSpaceAvailable;
        private System.Windows.Forms.TextBox tbFileSize;
    }
}