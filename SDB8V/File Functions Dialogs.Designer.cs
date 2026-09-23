namespace SymbolDB
{
    partial class FileFunctionsDialog
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
            this.tbSourceFpath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbInfo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbTargetDir = new System.Windows.Forms.TextBox();
            this.btCopy = new System.Windows.Forms.Button();
            this.btMove = new System.Windows.Forms.Button();
            this.btRename = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btCopyPathFname = new System.Windows.Forms.Button();
            this.tbSourceDir = new System.Windows.Forms.TextBox();
            this.btClose = new System.Windows.Forms.Button();
            this.btCopyFolderItems = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btDelete = new System.Windows.Forms.Button();
            this.btCreateDir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbSourceFpath
            // 
            this.tbSourceFpath.Location = new System.Drawing.Point(136, 54);
            this.tbSourceFpath.Name = "tbSourceFpath";
            this.tbSourceFpath.Size = new System.Drawing.Size(888, 20);
            this.tbSourceFpath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Source file /dir";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Target file / dir";
            // 
            // tbInfo
            // 
            this.tbInfo.Location = new System.Drawing.Point(119, 249);
            this.tbInfo.Name = "tbInfo";
            this.tbInfo.Size = new System.Drawing.Size(888, 20);
            this.tbInfo.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 249);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "info ---";
            // 
            // tbTargetDir
            // 
            this.tbTargetDir.Location = new System.Drawing.Point(136, 134);
            this.tbTargetDir.Name = "tbTargetDir";
            this.tbTargetDir.Size = new System.Drawing.Size(888, 20);
            this.tbTargetDir.TabIndex = 4;
            // 
            // btCopy
            // 
            this.btCopy.Location = new System.Drawing.Point(27, 181);
            this.btCopy.Name = "btCopy";
            this.btCopy.Size = new System.Drawing.Size(75, 23);
            this.btCopy.TabIndex = 6;
            this.btCopy.Text = "Copy";
            this.btCopy.UseVisualStyleBackColor = true;
            this.btCopy.Click += new System.EventHandler(this.btCopy_Click);
            // 
            // btMove
            // 
            this.btMove.Location = new System.Drawing.Point(392, 181);
            this.btMove.Name = "btMove";
            this.btMove.Size = new System.Drawing.Size(75, 23);
            this.btMove.TabIndex = 7;
            this.btMove.Text = "Move";
            this.btMove.UseVisualStyleBackColor = true;
            this.btMove.Click += new System.EventHandler(this.btMove_Click);
            // 
            // btRename
            // 
            this.btRename.Location = new System.Drawing.Point(585, 181);
            this.btRename.Name = "btRename";
            this.btRename.Size = new System.Drawing.Size(99, 23);
            this.btRename.TabIndex = 8;
            this.btRename.Text = "RenameFile";
            this.btRename.UseVisualStyleBackColor = true;
            this.btRename.Click += new System.EventHandler(this.btRename_Click);
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(38, 322);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(969, 375);
            this.listView1.TabIndex = 9;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(136, 90);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(274, 20);
            this.tbFileName.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "filename";
            // 
            // btCopyPathFname
            // 
            this.btCopyPathFname.Location = new System.Drawing.Point(119, 181);
            this.btCopyPathFname.Name = "btCopyPathFname";
            this.btCopyPathFname.Size = new System.Drawing.Size(152, 23);
            this.btCopyPathFname.TabIndex = 12;
            this.btCopyPathFname.Text = "Copy path and fname";
            this.btCopyPathFname.UseVisualStyleBackColor = true;
            this.btCopyPathFname.Click += new System.EventHandler(this.btCopyPathFname_Click);
            // 
            // tbSourceDir
            // 
            this.tbSourceDir.Location = new System.Drawing.Point(487, 89);
            this.tbSourceDir.Name = "tbSourceDir";
            this.tbSourceDir.Size = new System.Drawing.Size(537, 20);
            this.tbSourceDir.TabIndex = 13;
            // 
            // btClose
            // 
            this.btClose.Location = new System.Drawing.Point(950, 180);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(75, 23);
            this.btClose.TabIndex = 14;
            this.btClose.Text = "close";
            this.btClose.UseVisualStyleBackColor = true;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // btCopyFolderItems
            // 
            this.btCopyFolderItems.Location = new System.Drawing.Point(296, 181);
            this.btCopyFolderItems.Name = "btCopyFolderItems";
            this.btCopyFolderItems.Size = new System.Drawing.Size(75, 23);
            this.btCopyFolderItems.TabIndex = 15;
            this.btCopyFolderItems.Text = "copy folder items";
            this.btCopyFolderItems.UseVisualStyleBackColor = true;
            this.btCopyFolderItems.Click += new System.EventHandler(this.btCopyFolderItems_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(487, 181);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Move Folder";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(441, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "DIR";
            // 
            // btDelete
            // 
            this.btDelete.Location = new System.Drawing.Point(706, 181);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(75, 23);
            this.btDelete.TabIndex = 18;
            this.btDelete.Text = "Delete";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btCreateDir
            // 
            this.btCreateDir.Location = new System.Drawing.Point(809, 180);
            this.btCreateDir.Name = "btCreateDir";
            this.btCreateDir.Size = new System.Drawing.Size(75, 23);
            this.btCreateDir.TabIndex = 19;
            this.btCreateDir.Text = "Create Dir";
            this.btCreateDir.UseVisualStyleBackColor = true;
            this.btCreateDir.Click += new System.EventHandler(this.btCreateDir_Click);
            // 
            // FileFunctionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1036, 709);
            this.Controls.Add(this.btCreateDir);
            this.Controls.Add(this.btDelete);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btCopyFolderItems);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.tbSourceDir);
            this.Controls.Add(this.btCopyPathFname);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.btRename);
            this.Controls.Add(this.btMove);
            this.Controls.Add(this.btCopy);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbTargetDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSourceFpath);
            this.Name = "FileFunctionsDialog";
            this.Text = "File Functions";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSourceFpath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTargetDir;
        private System.Windows.Forms.Button btCopy;
        private System.Windows.Forms.Button btMove;
        private System.Windows.Forms.Button btRename;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btCopyPathFname;
        private System.Windows.Forms.TextBox tbSourceDir;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.Button btCopyFolderItems;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btCreateDir;
    }
}