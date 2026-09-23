namespace SymbolDB
{
    partial class ShowDisplay
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btWidth = new System.Windows.Forms.Button();
            this.btHeight = new System.Windows.Forms.Button();
            this.btTopLeftY = new System.Windows.Forms.Button();
            this.btTopLeftX = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(359, 177);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(210, 187);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "1";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // btWidth
            // 
            this.btWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btWidth.Location = new System.Drawing.Point(207, 386);
            this.btWidth.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btWidth.Name = "btWidth";
            this.btWidth.Size = new System.Drawing.Size(119, 78);
            this.btWidth.TabIndex = 291;
            this.btWidth.Text = "size";
            this.btWidth.UseVisualStyleBackColor = true;
            // 
            // btHeight
            // 
            this.btHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btHeight.Location = new System.Drawing.Point(346, 386);
            this.btHeight.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btHeight.Name = "btHeight";
            this.btHeight.Size = new System.Drawing.Size(119, 78);
            this.btHeight.TabIndex = 292;
            this.btHeight.Text = "size";
            this.btHeight.UseVisualStyleBackColor = true;
            // 
            // btTopLeftY
            // 
            this.btTopLeftY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btTopLeftY.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTopLeftY.Location = new System.Drawing.Point(346, 468);
            this.btTopLeftY.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btTopLeftY.Name = "btTopLeftY";
            this.btTopLeftY.Size = new System.Drawing.Size(119, 78);
            this.btTopLeftY.TabIndex = 294;
            this.btTopLeftY.Text = "size";
            this.btTopLeftY.UseVisualStyleBackColor = true;
            // 
            // btTopLeftX
            // 
            this.btTopLeftX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btTopLeftX.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTopLeftX.Location = new System.Drawing.Point(207, 468);
            this.btTopLeftX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btTopLeftX.Name = "btTopLeftX";
            this.btTopLeftX.Size = new System.Drawing.Size(119, 78);
            this.btTopLeftX.TabIndex = 293;
            this.btTopLeftX.Text = "size";
            this.btTopLeftX.UseVisualStyleBackColor = true;
            // 
            // ShowDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 564);
            this.Controls.Add(this.btTopLeftY);
            this.Controls.Add(this.btTopLeftX);
            this.Controls.Add(this.btHeight);
            this.Controls.Add(this.btWidth);
            this.Controls.Add(this.richTextBox1);
            this.Name = "ShowDisplay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "davids_new";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btWidth;
        private System.Windows.Forms.Button btHeight;
        private System.Windows.Forms.Button btTopLeftY;
        private System.Windows.Forms.Button btTopLeftX;
    }
}