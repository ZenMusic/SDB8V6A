namespace SymbolDB
{
    partial class ErrorStop
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
            this.btExit = new System.Windows.Forms.Button();
            this.btCloseThisOneOnly = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btExit
            // 
            this.btExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btExit.Location = new System.Drawing.Point(375, 253);
            this.btExit.Margin = new System.Windows.Forms.Padding(2);
            this.btExit.Name = "btExit";
            this.btExit.Size = new System.Drawing.Size(60, 27);
            this.btExit.TabIndex = 88;
            this.btExit.Text = "EXIT";
            this.btExit.UseVisualStyleBackColor = false;
            this.btExit.Click += new System.EventHandler(this.btExit_Click);
            // 
            // btCloseThisOneOnly
            // 
            this.btCloseThisOneOnly.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btCloseThisOneOnly.Location = new System.Drawing.Point(372, 214);
            this.btCloseThisOneOnly.Name = "btCloseThisOneOnly";
            this.btCloseThisOneOnly.Size = new System.Drawing.Size(118, 23);
            this.btCloseThisOneOnly.TabIndex = 285;
            this.btCloseThisOneOnly.Text = "Continue";
            this.btCloseThisOneOnly.UseVisualStyleBackColor = false;
            this.btCloseThisOneOnly.Click += new System.EventHandler(this.btCloseThisOneOnly_Click);
            // 
            // ErrorStop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btCloseThisOneOnly);
            this.Controls.Add(this.btExit);
            this.Name = "ErrorStop";
            this.Text = "ErrorStop";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btExit;
        private System.Windows.Forms.Button btCloseThisOneOnly;
    }
}