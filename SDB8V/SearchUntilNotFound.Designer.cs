namespace SymbolDB
{
    partial class SearchUntilNotFound
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
            components = new System.ComponentModel.Container();
            button1 = new System.Windows.Forms.Button();
            cbSearch = new System.Windows.Forms.CheckBox();
            btStart = new System.Windows.Forms.Button();
            searchTimer = new System.Windows.Forms.Timer(components);
            btSkipToNext = new System.Windows.Forms.Button();
            tbFileName = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(301, 182);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(8, 8);
            button1.TabIndex = 1;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // cbSearch
            // 
            cbSearch.AutoSize = true;
            cbSearch.Location = new System.Drawing.Point(12, 12);
            cbSearch.Name = "cbSearch";
            cbSearch.Size = new System.Drawing.Size(61, 19);
            cbSearch.TabIndex = 2;
            cbSearch.Text = "Search";
            cbSearch.UseVisualStyleBackColor = true;
            cbSearch.CheckedChanged += cbSearch_CheckedChanged;
            // 
            // btStart
            // 
            btStart.Location = new System.Drawing.Point(99, 3);
            btStart.Name = "btStart";
            btStart.Size = new System.Drawing.Size(226, 65);
            btStart.TabIndex = 3;
            btStart.Text = "start";
            btStart.UseVisualStyleBackColor = true;
            btStart.Click += btStart_Click;
            // 
            // searchTimer
            // 
            searchTimer.Enabled = true;
            searchTimer.Interval = 500;
            searchTimer.Tick += searchTimer_Tick;
            // 
            // btSkipToNext
            // 
            btSkipToNext.Location = new System.Drawing.Point(99, 74);
            btSkipToNext.Name = "btSkipToNext";
            btSkipToNext.Size = new System.Drawing.Size(226, 33);
            btSkipToNext.TabIndex = 4;
            btSkipToNext.Text = "skip to next row";
            btSkipToNext.UseVisualStyleBackColor = true;
            btSkipToNext.Click += btSkipToNext_Click;
            // 
            // tbFileName
            // 
            tbFileName.Location = new System.Drawing.Point(12, 164);
            tbFileName.Name = "tbFileName";
            tbFileName.Size = new System.Drawing.Size(324, 23);
            tbFileName.TabIndex = 5;
            // 
            // SearchUntilNotFound
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(348, 199);
            Controls.Add(tbFileName);
            Controls.Add(btSkipToNext);
            Controls.Add(btStart);
            Controls.Add(cbSearch);
            Controls.Add(button1);
            Name = "SearchUntilNotFound";
            Text = "SearchUntilNotFound";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox cbSearch;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Timer searchTimer;
        private System.Windows.Forms.Button btSkipToNext;
        private System.Windows.Forms.TextBox tbFileName;
    }
}