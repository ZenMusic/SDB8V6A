namespace SymbolDB
{
    partial class TraverserDialog
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
            this.btClose = new System.Windows.Forms.Button();
            this.LVcs_ImageList = new System.Windows.Forms.ListView();
            this.tbCount = new System.Windows.Forms.TextBox();
            this.tbFullPath = new System.Windows.Forms.TextBox();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.tbDirectoryPath = new System.Windows.Forms.TextBox();
            this.btCancel = new System.Windows.Forms.Button();
            this.btTraverse = new System.Windows.Forms.Button();
            this.btDirectoryDialog = new System.Windows.Forms.Button();
            this.tbLoadedCount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.labelFileCount = new System.Windows.Forms.Label();
            this.labelLoadedCount = new System.Windows.Forms.Label();
            this.tbInfo = new System.Windows.Forms.TextBox();
            this.dtPicker1 = new System.Windows.Forms.DateTimePicker();
            this.btAcceptSort = new System.Windows.Forms.Button();
            this.btDisplayList = new System.Windows.Forms.Button();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.cbDisplayListView = new System.Windows.Forms.CheckBox();
            this.btSave = new System.Windows.Forms.Button();
            this.btLoad = new System.Windows.Forms.Button();
            this.cbTraverse = new System.Windows.Forms.CheckBox();
            this.bTraversing = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.gbFolder = new System.Windows.Forms.GroupBox();
            this.rbTarot = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cbProgress = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbMax = new System.Windows.Forms.TextBox();
            this.gbFolder.SuspendLayout();
            this.SuspendLayout();
            // 
            // btClose
            // 
            this.btClose.Location = new System.Drawing.Point(827, 56);
            this.btClose.Margin = new System.Windows.Forms.Padding(4);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(201, 37);
            this.btClose.TabIndex = 26;
            this.btClose.Text = "Close and Return to Main";
            this.btClose.UseVisualStyleBackColor = true;
            this.btClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // listView1
            // 
            this.LVcs_ImageList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LVcs_ImageList.Location = new System.Drawing.Point(5, 256);
            this.LVcs_ImageList.Margin = new System.Windows.Forms.Padding(4);
            this.LVcs_ImageList.MinimumSize = new System.Drawing.Size(439, 405);
            this.LVcs_ImageList.Name = "listView1";
            this.LVcs_ImageList.Size = new System.Drawing.Size(1585, 622);
            this.LVcs_ImageList.TabIndex = 25;
            this.LVcs_ImageList.UseCompatibleStateImageBehavior = false;
            this.LVcs_ImageList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.LVcs_ImageList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
            this.LVcs_ImageList.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged_1);
            // 
            // tbCount
            // 
            this.tbCount.Location = new System.Drawing.Point(113, 203);
            this.tbCount.Margin = new System.Windows.Forms.Padding(4);
            this.tbCount.Name = "tbCount";
            this.tbCount.Size = new System.Drawing.Size(132, 22);
            this.tbCount.TabIndex = 24;
            // 
            // tbFullPath
            // 
            this.tbFullPath.Location = new System.Drawing.Point(201, 133);
            this.tbFullPath.Margin = new System.Windows.Forms.Padding(4);
            this.tbFullPath.Name = "tbFullPath";
            this.tbFullPath.Size = new System.Drawing.Size(258, 22);
            this.tbFullPath.TabIndex = 22;
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(1223, 54);
            this.tbFileName.Margin = new System.Windows.Forms.Padding(4);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(363, 22);
            this.tbFileName.TabIndex = 19;
            // 
            // tbResult
            // 
            this.tbResult.Location = new System.Drawing.Point(113, 232);
            this.tbResult.Margin = new System.Windows.Forms.Padding(4);
            this.tbResult.Name = "tbResult";
            this.tbResult.Size = new System.Drawing.Size(981, 22);
            this.tbResult.TabIndex = 18;
            // 
            // tbDirectoryPath
            // 
            this.tbDirectoryPath.Location = new System.Drawing.Point(481, 133);
            this.tbDirectoryPath.Margin = new System.Windows.Forms.Padding(4);
            this.tbDirectoryPath.Name = "tbDirectoryPath";
            this.tbDirectoryPath.Size = new System.Drawing.Size(298, 22);
            this.tbDirectoryPath.TabIndex = 17;
            // 
            // btCancel
            // 
            this.btCancel.AutoSize = true;
            this.btCancel.Location = new System.Drawing.Point(521, 13);
            this.btCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(96, 28);
            this.btCancel.TabIndex = 16;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.buttonCancel_Click_1);
            // 
            // btTraverse
            // 
            this.btTraverse.AutoSize = true;
            this.btTraverse.Location = new System.Drawing.Point(400, 13);
            this.btTraverse.Margin = new System.Windows.Forms.Padding(4);
            this.btTraverse.Name = "btTraverse";
            this.btTraverse.Size = new System.Drawing.Size(113, 28);
            this.btTraverse.TabIndex = 15;
            this.btTraverse.Text = "Traverse";
            this.btTraverse.UseVisualStyleBackColor = true;
            this.btTraverse.Click += new System.EventHandler(this.buttonTraverse_Click_1);
            // 
            // btDirectoryDialog
            // 
            this.btDirectoryDialog.AutoSize = true;
            this.btDirectoryDialog.Location = new System.Drawing.Point(201, 13);
            this.btDirectoryDialog.Margin = new System.Windows.Forms.Padding(4);
            this.btDirectoryDialog.Name = "btDirectoryDialog";
            this.btDirectoryDialog.Size = new System.Drawing.Size(149, 28);
            this.btDirectoryDialog.TabIndex = 14;
            this.btDirectoryDialog.Text = "Select Directory";
            this.btDirectoryDialog.UseVisualStyleBackColor = true;
            this.btDirectoryDialog.Click += new System.EventHandler(this.btDirectoryDialog_Click);
            // 
            // tbLoadedCount
            // 
            this.tbLoadedCount.Location = new System.Drawing.Point(375, 206);
            this.tbLoadedCount.Margin = new System.Windows.Forms.Padding(4);
            this.tbLoadedCount.Name = "tbLoadedCount";
            this.tbLoadedCount.Size = new System.Drawing.Size(136, 22);
            this.tbLoadedCount.TabIndex = 27;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 232);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 17);
            this.label2.TabIndex = 29;
            this.label2.Text = "result";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1150, 57);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 17);
            this.label3.TabIndex = 30;
            this.label3.Text = "file name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(944, 206);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 17);
            this.label5.TabIndex = 32;
            this.label5.Text = "type";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1262, 98);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 17);
            this.label7.TabIndex = 34;
            this.label7.Text = "length";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // labelFileCount
            // 
            this.labelFileCount.AutoSize = true;
            this.labelFileCount.Location = new System.Drawing.Point(40, 204);
            this.labelFileCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFileCount.Name = "labelFileCount";
            this.labelFileCount.Size = new System.Drawing.Size(65, 17);
            this.labelFileCount.TabIndex = 35;
            this.labelFileCount.Text = "file count";
            // 
            // labelLoadedCount
            // 
            this.labelLoadedCount.AutoSize = true;
            this.labelLoadedCount.Location = new System.Drawing.Point(315, 206);
            this.labelLoadedCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelLoadedCount.Name = "labelLoadedCount";
            this.labelLoadedCount.Size = new System.Drawing.Size(51, 17);
            this.labelLoadedCount.TabIndex = 36;
            this.labelLoadedCount.Text = "loaded";
            // 
            // tbInfo
            // 
            this.tbInfo.Location = new System.Drawing.Point(1346, 93);
            this.tbInfo.Margin = new System.Windows.Forms.Padding(4);
            this.tbInfo.Name = "tbInfo";
            this.tbInfo.Size = new System.Drawing.Size(240, 22);
            this.tbInfo.TabIndex = 37;
            this.tbInfo.Text = "info";
            // 
            // dtPicker1
            // 
            this.dtPicker1.Location = new System.Drawing.Point(789, 14);
            this.dtPicker1.Margin = new System.Windows.Forms.Padding(4);
            this.dtPicker1.Name = "dtPicker1";
            this.dtPicker1.Size = new System.Drawing.Size(339, 22);
            this.dtPicker1.TabIndex = 39;
            // 
            // btAcceptSort
            // 
            this.btAcceptSort.Location = new System.Drawing.Point(643, 13);
            this.btAcceptSort.Margin = new System.Windows.Forms.Padding(4);
            this.btAcceptSort.Name = "btAcceptSort";
            this.btAcceptSort.Size = new System.Drawing.Size(100, 28);
            this.btAcceptSort.TabIndex = 40;
            this.btAcceptSort.Text = "Accept Sort";
            this.btAcceptSort.UseVisualStyleBackColor = true;
            this.btAcceptSort.Click += new System.EventHandler(this.AcceptSort_Click_1);
            // 
            // btDisplayList
            // 
            this.btDisplayList.Location = new System.Drawing.Point(1482, 129);
            this.btDisplayList.Margin = new System.Windows.Forms.Padding(4);
            this.btDisplayList.Name = "btDisplayList";
            this.btDisplayList.Size = new System.Drawing.Size(79, 28);
            this.btDisplayList.TabIndex = 42;
            this.btDisplayList.Text = "refresh";
            this.btDisplayList.UseVisualStyleBackColor = true;
            this.btDisplayList.Click += new System.EventHandler(this.btDisplayList_Click);
            // 
            // cbDisplayListView
            // 
            this.cbDisplayListView.AutoSize = true;
            this.cbDisplayListView.Checked = true;
            this.cbDisplayListView.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDisplayListView.Location = new System.Drawing.Point(1342, 233);
            this.cbDisplayListView.Margin = new System.Windows.Forms.Padding(4);
            this.cbDisplayListView.Name = "cbDisplayListView";
            this.cbDisplayListView.Size = new System.Drawing.Size(102, 21);
            this.cbDisplayListView.TabIndex = 43;
            this.cbDisplayListView.Text = "Display List";
            this.cbDisplayListView.UseVisualStyleBackColor = true;
            this.cbDisplayListView.CheckedChanged += new System.EventHandler(this.cbDone_CheckedChanged);
            this.cbDisplayListView.CheckStateChanged += new System.EventHandler(this.cbDone_CheckStateChanged);
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(1197, 13);
            this.btSave.Margin = new System.Windows.Forms.Padding(4);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(157, 28);
            this.btSave.TabIndex = 44;
            this.btSave.Text = "Save Image List File";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btLoad
            // 
            this.btLoad.Location = new System.Drawing.Point(1374, 13);
            this.btLoad.Margin = new System.Windows.Forms.Padding(4);
            this.btLoad.Name = "btLoad";
            this.btLoad.Size = new System.Drawing.Size(187, 28);
            this.btLoad.TabIndex = 45;
            this.btLoad.Text = "Load Image List File";
            this.btLoad.UseVisualStyleBackColor = true;
            this.btLoad.Click += new System.EventHandler(this.btLoad_Click);
            // 
            // cbTraverse
            // 
            this.cbTraverse.AutoSize = true;
            this.cbTraverse.Checked = true;
            this.cbTraverse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTraverse.Location = new System.Drawing.Point(201, 56);
            this.cbTraverse.Margin = new System.Windows.Forms.Padding(4);
            this.cbTraverse.Name = "cbTraverse";
            this.cbTraverse.Size = new System.Drawing.Size(187, 21);
            this.cbTraverse.TabIndex = 47;
            this.cbTraverse.Text = "Traverse subfolders also";
            this.cbTraverse.UseVisualStyleBackColor = true;
            this.cbTraverse.CheckedChanged += new System.EventHandler(this.cbTraverse_CheckedChanged);
            // 
            // bTraversing
            // 
            this.bTraversing.Location = new System.Drawing.Point(530, 392);
            this.bTraversing.Name = "bTraversing";
            this.bTraversing.Size = new System.Drawing.Size(231, 93);
            this.bTraversing.TabIndex = 48;
            this.bTraversing.Text = "traversing ...";
            this.bTraversing.UseVisualStyleBackColor = true;
            this.bTraversing.Visible = false;
            this.bTraversing.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(17, 21);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(108, 21);
            this.radioButton1.TabIndex = 49;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "MyComputer";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(17, 48);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(73, 21);
            this.radioButton2.TabIndex = 50;
            this.radioButton2.Text = "MyPics";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(17, 72);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(79, 21);
            this.radioButton3.TabIndex = 51;
            this.radioButton3.Text = "MyDocs";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // gbFolder
            // 
            this.gbFolder.Controls.Add(this.rbTarot);
            this.gbFolder.Controls.Add(this.radioButton5);
            this.gbFolder.Controls.Add(this.radioButton4);
            this.gbFolder.Controls.Add(this.radioButton1);
            this.gbFolder.Controls.Add(this.radioButton3);
            this.gbFolder.Controls.Add(this.radioButton2);
            this.gbFolder.Location = new System.Drawing.Point(26, 7);
            this.gbFolder.Name = "gbFolder";
            this.gbFolder.Size = new System.Drawing.Size(146, 180);
            this.gbFolder.TabIndex = 52;
            this.gbFolder.TabStop = false;
            this.gbFolder.Text = "Start selection at:";
            // 
            // rbTarot
            // 
            this.rbTarot.AutoSize = true;
            this.rbTarot.Location = new System.Drawing.Point(17, 153);
            this.rbTarot.Name = "rbTarot";
            this.rbTarot.Size = new System.Drawing.Size(58, 21);
            this.rbTarot.TabIndex = 54;
            this.rbTarot.Text = "tarot";
            this.rbTarot.UseVisualStyleBackColor = true;
            this.rbTarot.CheckedChanged += new System.EventHandler(this.rbTarot_CheckedChanged_1);
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Location = new System.Drawing.Point(17, 126);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(81, 21);
            this.radioButton5.TabIndex = 53;
            this.radioButton5.Text = "Desktop";
            this.radioButton5.UseVisualStyleBackColor = true;
            this.radioButton5.CheckedChanged += new System.EventHandler(this.radioButton5_CheckedChanged);
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(17, 99);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(42, 21);
            this.radioButton4.TabIndex = 52;
            this.radioButton4.Text = "C:";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(400, 55);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(144, 23);
            this.progressBar1.TabIndex = 53;
            // 
            // cbProgress
            // 
            this.cbProgress.AutoSize = true;
            this.cbProgress.Location = new System.Drawing.Point(400, 87);
            this.cbProgress.Name = "cbProgress";
            this.cbProgress.Size = new System.Drawing.Size(136, 21);
            this.cbProgress.TabIndex = 54;
            this.cbProgress.Text = "Show progress...";
            this.cbProgress.UseVisualStyleBackColor = true;
            this.cbProgress.CheckedChanged += new System.EventHandler(this.cbProgress_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(538, 206);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 17);
            this.label8.TabIndex = 55;
            this.label8.Text = "max";
            // 
            // tbMax
            // 
            this.tbMax.Enabled = false;
            this.tbMax.Location = new System.Drawing.Point(580, 206);
            this.tbMax.Margin = new System.Windows.Forms.Padding(4);
            this.tbMax.Name = "tbMax";
            this.tbMax.Size = new System.Drawing.Size(136, 22);
            this.tbMax.TabIndex = 56;
            // 
            // TraverserDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1603, 891);
            this.Controls.Add(this.tbMax);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cbProgress);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.gbFolder);
            this.Controls.Add(this.bTraversing);
            this.Controls.Add(this.cbTraverse);
            this.Controls.Add(this.btLoad);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.cbDisplayListView);
            this.Controls.Add(this.btDisplayList);
            this.Controls.Add(this.btAcceptSort);
            this.Controls.Add(this.dtPicker1);
            this.Controls.Add(this.tbInfo);
            this.Controls.Add(this.labelLoadedCount);
            this.Controls.Add(this.labelFileCount);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbLoadedCount);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.LVcs_ImageList);
            this.Controls.Add(this.tbCount);
            this.Controls.Add(this.tbFullPath);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.tbDirectoryPath);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btTraverse);
            this.Controls.Add(this.btDirectoryDialog);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TraverserDialog";
            this.Text = "Select a folder containing Images to be viewed or managed and then  click Travers" +
    "e to build the Image List";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormTraverser2_FormClosed);
            this.Resize += new System.EventHandler(this.FormTraverser2_Resize);
            this.gbFolder.ResumeLayout(false);
            this.gbFolder.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.ListView LVcs_ImageList;
        private System.Windows.Forms.TextBox tbCount;
        private System.Windows.Forms.TextBox tbFullPath;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.TextBox tbDirectoryPath;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btTraverse;
        private System.Windows.Forms.Button btDirectoryDialog;
        private System.Windows.Forms.TextBox tbLoadedCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelFileCount;
        private System.Windows.Forms.Label labelLoadedCount;
        private System.Windows.Forms.TextBox tbInfo;
        private System.Windows.Forms.DateTimePicker dtPicker1;
        private System.Windows.Forms.Button btAcceptSort;
        private System.Windows.Forms.Button btDisplayList;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.CheckBox cbDisplayListView;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.CheckBox cbTraverse;
        private System.Windows.Forms.Button bTraversing;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.GroupBox gbFolder;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton rbTarot;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox cbProgress;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbMax;
    }
}