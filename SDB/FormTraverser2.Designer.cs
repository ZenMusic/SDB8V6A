namespace SymbolDB
{
    partial class FormTraverser2
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
            this.tbLength = new System.Windows.Forms.TextBox();
            this.tbFullPath = new System.Windows.Forms.TextBox();
            this.tbLevel = new System.Windows.Forms.TextBox();
            this.tbType = new System.Windows.Forms.TextBox();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.textBoxResult = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonTraverse = new System.Windows.Forms.Button();
            this.buttonFileDialog = new System.Windows.Forms.Button();
            this.tbLoadedCount = new System.Windows.Forms.TextBox();
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
            this.listView1.Location = new System.Drawing.Point(1, 160);
            this.listView1.MinimumSize = new System.Drawing.Size(330, 330);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1192, 467);
            this.listView1.TabIndex = 25;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // tbCount
            // 
            this.tbCount.Location = new System.Drawing.Point(45, 7);
            this.tbCount.Name = "tbCount";
            this.tbCount.Size = new System.Drawing.Size(100, 20);
            this.tbCount.TabIndex = 24;
            // 
            // tbLength
            // 
            this.tbLength.Location = new System.Drawing.Point(13, 134);
            this.tbLength.Name = "tbLength";
            this.tbLength.Size = new System.Drawing.Size(273, 20);
            this.tbLength.TabIndex = 23;
            // 
            // tbFullPath
            // 
            this.tbFullPath.Location = new System.Drawing.Point(550, 105);
            this.tbFullPath.Name = "tbFullPath";
            this.tbFullPath.Size = new System.Drawing.Size(519, 20);
            this.tbFullPath.TabIndex = 22;
            // 
            // tbLevel
            // 
            this.tbLevel.Location = new System.Drawing.Point(416, 106);
            this.tbLevel.Name = "tbLevel";
            this.tbLevel.Size = new System.Drawing.Size(108, 20);
            this.tbLevel.TabIndex = 21;
            // 
            // tbType
            // 
            this.tbType.Location = new System.Drawing.Point(310, 106);
            this.tbType.Name = "tbType";
            this.tbType.Size = new System.Drawing.Size(100, 20);
            this.tbType.TabIndex = 20;
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(13, 106);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(273, 20);
            this.tbFileName.TabIndex = 19;
            // 
            // textBoxResult
            // 
            this.textBoxResult.Location = new System.Drawing.Point(13, 79);
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.Size = new System.Drawing.Size(932, 20);
            this.textBoxResult.TabIndex = 18;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 53);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(955, 20);
            this.textBox1.TabIndex = 17;
            // 
            // buttonCancel
            // 
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.Location = new System.Drawing.Point(726, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(138, 23);
            this.buttonCancel.TabIndex = 16;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click_1);
            // 
            // buttonTraverse
            // 
            this.buttonTraverse.AutoSize = true;
            this.buttonTraverse.Location = new System.Drawing.Point(536, 7);
            this.buttonTraverse.Name = "buttonTraverse";
            this.buttonTraverse.Size = new System.Drawing.Size(138, 23);
            this.buttonTraverse.TabIndex = 15;
            this.buttonTraverse.Text = "Traverse";
            this.buttonTraverse.UseVisualStyleBackColor = true;
            this.buttonTraverse.Click += new System.EventHandler(this.buttonTraverse_Click_1);
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
            // tbLoadedCount
            // 
            this.tbLoadedCount.Location = new System.Drawing.Point(168, 9);
            this.tbLoadedCount.Name = "tbLoadedCount";
            this.tbLoadedCount.Size = new System.Drawing.Size(144, 20);
            this.tbLoadedCount.TabIndex = 27;
            // 
            // FormTraverser2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1205, 713);
            this.Controls.Add(this.tbLoadedCount);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.tbCount);
            this.Controls.Add(this.tbLength);
            this.Controls.Add(this.tbFullPath);
            this.Controls.Add(this.tbLevel);
            this.Controls.Add(this.tbType);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.textBoxResult);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonTraverse);
            this.Controls.Add(this.buttonFileDialog);
            this.Name = "FormTraverser2";
            this.Text = "FormTraverser2";
            this.Resize += new System.EventHandler(this.FormTraverser2_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TextBox tbCount;
        private System.Windows.Forms.TextBox tbLength;
        private System.Windows.Forms.TextBox tbFullPath;
        private System.Windows.Forms.TextBox tbLevel;
        private System.Windows.Forms.TextBox tbType;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonTraverse;
        private System.Windows.Forms.Button buttonFileDialog;
        private System.Windows.Forms.TextBox tbLoadedCount;
    }
}