namespace SymbolDB
{
    partial class TopMost
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cbTopMost = new System.Windows.Forms.CheckBox();
            this.cbContinue = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(30, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(131, 23);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // cbTopMost
            // 
            this.cbTopMost.AutoSize = true;
            this.cbTopMost.Checked = true;
            this.cbTopMost.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTopMost.Location = new System.Drawing.Point(43, 134);
            this.cbTopMost.Name = "cbTopMost";
            this.cbTopMost.Size = new System.Drawing.Size(71, 17);
            this.cbTopMost.TabIndex = 2;
            this.cbTopMost.Text = "Top Most";
            this.cbTopMost.UseVisualStyleBackColor = true;
            // 
            // cbContinue
            // 
            this.cbContinue.AutoSize = true;
            this.cbContinue.Checked = true;
            this.cbContinue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbContinue.Location = new System.Drawing.Point(181, 74);
            this.cbContinue.Name = "cbContinue";
            this.cbContinue.Size = new System.Drawing.Size(68, 17);
            this.cbContinue.TabIndex = 3;
            this.cbContinue.Text = "Continue";
            this.cbContinue.UseVisualStyleBackColor = true;
            this.cbContinue.CheckedChanged += new System.EventHandler(this.cbContinue_CheckedChanged);
            // 
            // TopMost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 170);
            this.Controls.Add(this.cbContinue);
            this.Controls.Add(this.cbTopMost);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "TopMost";
            this.Text = "TopMost";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox cbTopMost;
        private System.Windows.Forms.CheckBox cbContinue;
    }
}