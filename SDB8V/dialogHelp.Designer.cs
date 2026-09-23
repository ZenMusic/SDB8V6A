namespace SymbolDB
{
    partial class dialogHelp
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
            this.lvHelp = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // lvHelp
            // 
            this.lvHelp.Location = new System.Drawing.Point(39, 188);
            this.lvHelp.Name = "lvHelp";
            this.lvHelp.Size = new System.Drawing.Size(1260, 885);
            this.lvHelp.TabIndex = 0;
            this.lvHelp.UseCompatibleStateImageBehavior = false;
            // 
            // dialogHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1752, 1234);
            this.Controls.Add(this.lvHelp);
            this.Name = "dialogHelp";
            this.Text = "dialogHelp";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvHelp;
    }
}