namespace SymbolDB
{
    partial class TagPB
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
            this.tbX = new System.Windows.Forms.TextBox();
            this.tbY = new System.Windows.Forms.TextBox();
            this.tbW = new System.Windows.Forms.TextBox();
            this.tbH = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbX
            // 
            this.tbX.Location = new System.Drawing.Point(0, -1);
            this.tbX.Name = "tbX";
            this.tbX.Size = new System.Drawing.Size(48, 20);
            this.tbX.TabIndex = 0;
            // 
            // tbY
            // 
            this.tbY.Location = new System.Drawing.Point(54, -1);
            this.tbY.Name = "tbY";
            this.tbY.Size = new System.Drawing.Size(41, 20);
            this.tbY.TabIndex = 1;
            // 
            // tbW
            // 
            this.tbW.Location = new System.Drawing.Point(0, 25);
            this.tbW.Name = "tbW";
            this.tbW.Size = new System.Drawing.Size(48, 20);
            this.tbW.TabIndex = 2;
            // 
            // tbH
            // 
            this.tbH.Location = new System.Drawing.Point(54, 25);
            this.tbH.Name = "tbH";
            this.tbH.Size = new System.Drawing.Size(41, 20);
            this.tbH.TabIndex = 3;
            // 
            // Tag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(153, 80);
            this.Controls.Add(this.tbH);
            this.Controls.Add(this.tbW);
            this.Controls.Add(this.tbY);
            this.Controls.Add(this.tbX);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(10, 10);
            this.Name = "Tag";
            this.Text = "Tag";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Tag_Load);
            this.SizeChanged += new System.EventHandler(this.Tag_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbX;
        private System.Windows.Forms.TextBox tbY;
        private System.Windows.Forms.TextBox tbW;
        private System.Windows.Forms.TextBox tbH;
    }
}