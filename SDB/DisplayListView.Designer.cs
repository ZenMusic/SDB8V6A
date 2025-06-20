namespace SymbolDB
{
    partial class DisplayListView
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
            this.buttonClose = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.tbCount = new System.Windows.Forms.TextBox();
            this.buttonFileDialog = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(910, 7);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 26;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // listView1
            // 
            this.listView1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.listView1.Location = new System.Drawing.Point(1, 36);
            this.listView1.MinimumSize = new System.Drawing.Size(330, 330);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1206, 679);
            this.listView1.TabIndex = 25;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged_1);
            // 
            // tbCount
            // 
            this.tbCount.Location = new System.Drawing.Point(72, 10);
            this.tbCount.Name = "tbCount";
            this.tbCount.Size = new System.Drawing.Size(100, 20);
            this.tbCount.TabIndex = 24;
            // 
            // buttonFileDialog
            // 
            this.buttonFileDialog.AutoSize = true;
            this.buttonFileDialog.Location = new System.Drawing.Point(368, 7);
            this.buttonFileDialog.Name = "buttonFileDialog";
            this.buttonFileDialog.Size = new System.Drawing.Size(138, 23);
            this.buttonFileDialog.TabIndex = 14;
            this.buttonFileDialog.Text = "Open File Dialog";
            this.buttonFileDialog.UseVisualStyleBackColor = true;
            this.buttonFileDialog.Click += new System.EventHandler(this.buttonFileDialog_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 35;
            this.label8.Text = "file count";
            // 
            // DisplayList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1205, 713);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.tbCount);
            this.Controls.Add(this.buttonFileDialog);
            this.Name = "DisplayList";
            this.Text = "FormTraverser2";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormTraverser2_FormClosed);
            this.Resize += new System.EventHandler(this.FormTraverser2_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TextBox tbCount;
        private System.Windows.Forms.Button buttonFileDialog;
        private System.Windows.Forms.Label label8;
    }
}