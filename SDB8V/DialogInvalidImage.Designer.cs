namespace SymbolDB
{
    partial class DialogInvalidImage
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
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            button3 = new System.Windows.Forms.Button();
            tbFilePath = new System.Windows.Forms.TextBox();
            textBox2 = new System.Windows.Forms.TextBox();
            tbRowNumber = new System.Windows.Forms.TextBox();
            tbTotal = new System.Windows.Forms.TextBox();
            numNextImageId = new System.Windows.Forms.NumericUpDown();
            tbCount = new System.Windows.Forms.TextBox();
            btClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)numNextImageId).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(355, 328);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(100, 42);
            button1.TabIndex = 0;
            button1.Text = "Continue";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btContinue_Click;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(492, 328);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(100, 42);
            button2.TabIndex = 1;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            button2.Click += btCancel_Click;
            // 
            // button3
            // 
            button3.Location = new System.Drawing.Point(640, 328);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(100, 42);
            button3.TabIndex = 2;
            button3.Text = "Stop";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btStop_Click;
            // 
            // tbFilePath
            // 
            tbFilePath.Location = new System.Drawing.Point(2, 60);
            tbFilePath.Name = "tbFilePath";
            tbFilePath.Size = new System.Drawing.Size(786, 23);
            tbFilePath.TabIndex = 3;
            // 
            // textBox2
            // 
            textBox2.Location = new System.Drawing.Point(7, 214);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(786, 23);
            textBox2.TabIndex = 4;
            // 
            // tbRowNumber
            // 
            tbRowNumber.Location = new System.Drawing.Point(211, 112);
            tbRowNumber.Name = "tbRowNumber";
            tbRowNumber.Size = new System.Drawing.Size(106, 23);
            tbRowNumber.TabIndex = 5;
            // 
            // tbTotal
            // 
            tbTotal.Location = new System.Drawing.Point(349, 112);
            tbTotal.Name = "tbTotal";
            tbTotal.Size = new System.Drawing.Size(106, 23);
            tbTotal.TabIndex = 6;
            // 
            // numNextImageId
            // 
            numNextImageId.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            numNextImageId.Location = new System.Drawing.Point(215, 162);
            numNextImageId.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numNextImageId.Name = "numNextImageId";
            numNextImageId.Size = new System.Drawing.Size(108, 29);
            numNextImageId.TabIndex = 7;
            // 
            // tbCount
            // 
            tbCount.Location = new System.Drawing.Point(682, 12);
            tbCount.Name = "tbCount";
            tbCount.Size = new System.Drawing.Size(106, 23);
            tbCount.TabIndex = 8;
            // 
            // btClose
            // 
            btClose.Location = new System.Drawing.Point(640, 396);
            btClose.Name = "btClose";
            btClose.Size = new System.Drawing.Size(100, 42);
            btClose.TabIndex = 9;
            btClose.Text = "Close";
            btClose.UseVisualStyleBackColor = true;
            btClose.Click += btClose_Click;
            // 
            // DialogInvalidImage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(btClose);
            Controls.Add(tbCount);
            Controls.Add(numNextImageId);
            Controls.Add(tbTotal);
            Controls.Add(tbRowNumber);
            Controls.Add(textBox2);
            Controls.Add(tbFilePath);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "DialogInvalidImage";
            Text = "DialogInvalidImage";
            ((System.ComponentModel.ISupportInitialize)numNextImageId).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox tbFilePath;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox tbRowNumber;
        private System.Windows.Forms.TextBox tbTotal;
        private System.Windows.Forms.NumericUpDown numNextImageId;
        private System.Windows.Forms.TextBox tbCount;
        private System.Windows.Forms.Button btClose;
    }
}