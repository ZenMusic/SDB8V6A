
namespace SymbolDB
{
    partial class DialogFolderAssignments
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
            this.dataGridViewConfig = new System.Windows.Forms.DataGridView();
            this.btDirectoryDialog = new System.Windows.Forms.Button();
            this.tbDirectoryPath = new System.Windows.Forms.TextBox();
            this.tbFolderName = new System.Windows.Forms.TextBox();
            this.btDisplayData = new System.Windows.Forms.Button();
            this.tbDesc = new System.Windows.Forms.TextBox();
            this.cbTraverseThisFolder = new System.Windows.Forms.CheckBox();
            this.cbTraverse = new System.Windows.Forms.CheckBox();
            this.tbTargetFolder = new System.Windows.Forms.TextBox();
            this.btClose = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.Folder = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbRowIdTemp = new System.Windows.Forms.TextBox();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.btEnterData = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConfig)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewConfig
            // 
            this.dataGridViewConfig.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewConfig.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridViewConfig.Location = new System.Drawing.Point(59, 11);
            this.dataGridViewConfig.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridViewConfig.Name = "dataGridViewConfig";
            this.dataGridViewConfig.RowHeadersWidth = 51;
            this.dataGridViewConfig.RowTemplate.Height = 24;
            this.dataGridViewConfig.Size = new System.Drawing.Size(1193, 859);
            this.dataGridViewConfig.TabIndex = 131;
            this.dataGridViewConfig.SelectionChanged += new System.EventHandler(this.dgvFolderAssignments_SelectionChanged);
            // 
            // btDirectoryDialog
            // 
            this.btDirectoryDialog.AutoSize = true;
            this.btDirectoryDialog.Location = new System.Drawing.Point(1359, 103);
            this.btDirectoryDialog.Name = "btDirectoryDialog";
            this.btDirectoryDialog.Size = new System.Drawing.Size(112, 23);
            this.btDirectoryDialog.TabIndex = 132;
            this.btDirectoryDialog.Text = "Select Directory";
            this.btDirectoryDialog.UseVisualStyleBackColor = true;
            this.btDirectoryDialog.Click += new System.EventHandler(this.btDirectoryDialog_Click);
            // 
            // tbDirectoryPath
            // 
            this.tbDirectoryPath.Location = new System.Drawing.Point(1285, 146);
            this.tbDirectoryPath.Margin = new System.Windows.Forms.Padding(2);
            this.tbDirectoryPath.Name = "tbDirectoryPath";
            this.tbDirectoryPath.Size = new System.Drawing.Size(286, 20);
            this.tbDirectoryPath.TabIndex = 179;
            // 
            // tbFolderName
            // 
            this.tbFolderName.Location = new System.Drawing.Point(1372, 53);
            this.tbFolderName.Margin = new System.Windows.Forms.Padding(2);
            this.tbFolderName.Name = "tbFolderName";
            this.tbFolderName.Size = new System.Drawing.Size(81, 20);
            this.tbFolderName.TabIndex = 180;
            // 
            // btDisplayData
            // 
            this.btDisplayData.AutoSize = true;
            this.btDisplayData.Location = new System.Drawing.Point(1359, 184);
            this.btDisplayData.Name = "btDisplayData";
            this.btDisplayData.Size = new System.Drawing.Size(112, 23);
            this.btDisplayData.TabIndex = 181;
            this.btDisplayData.Text = "Display Data";
            this.btDisplayData.UseVisualStyleBackColor = true;
            this.btDisplayData.Click += new System.EventHandler(this.btEnterData_Click);
            // 
            // tbDesc
            // 
            this.tbDesc.Location = new System.Drawing.Point(1285, 250);
            this.tbDesc.Margin = new System.Windows.Forms.Padding(2);
            this.tbDesc.Name = "tbDesc";
            this.tbDesc.Size = new System.Drawing.Size(286, 20);
            this.tbDesc.TabIndex = 182;
            // 
            // cbTraverseThisFolder
            // 
            this.cbTraverseThisFolder.AutoSize = true;
            this.cbTraverseThisFolder.Location = new System.Drawing.Point(1319, 496);
            this.cbTraverseThisFolder.Name = "cbTraverseThisFolder";
            this.cbTraverseThisFolder.Size = new System.Drawing.Size(116, 17);
            this.cbTraverseThisFolder.TabIndex = 185;
            this.cbTraverseThisFolder.Text = "Traverse this folder";
            this.cbTraverseThisFolder.UseVisualStyleBackColor = true;
            // 
            // cbTraverse
            // 
            this.cbTraverse.AutoSize = true;
            this.cbTraverse.Checked = true;
            this.cbTraverse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTraverse.Location = new System.Drawing.Point(1319, 460);
            this.cbTraverse.Name = "cbTraverse";
            this.cbTraverse.Size = new System.Drawing.Size(119, 17);
            this.cbTraverse.TabIndex = 184;
            this.cbTraverse.Text = "Traverse subfolders";
            this.cbTraverse.UseVisualStyleBackColor = true;
            // 
            // tbTargetFolder
            // 
            this.tbTargetFolder.Location = new System.Drawing.Point(1285, 394);
            this.tbTargetFolder.Margin = new System.Windows.Forms.Padding(2);
            this.tbTargetFolder.Name = "tbTargetFolder";
            this.tbTargetFolder.Size = new System.Drawing.Size(286, 20);
            this.tbTargetFolder.TabIndex = 186;
            // 
            // btClose
            // 
            this.btClose.Location = new System.Drawing.Point(1391, 599);
            this.btClose.Margin = new System.Windows.Forms.Padding(2);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(62, 30);
            this.btClose.TabIndex = 187;
            this.btClose.Text = "Close";
            this.btClose.UseVisualStyleBackColor = true;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1291, 369);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 216;
            this.label2.Text = "Description";
            // 
            // Folder
            // 
            this.Folder.AutoSize = true;
            this.Folder.Location = new System.Drawing.Point(1291, 216);
            this.Folder.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Folder.Name = "Folder";
            this.Folder.Size = new System.Drawing.Size(36, 13);
            this.Folder.TabIndex = 217;
            this.Folder.Text = "Folder";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1282, 122);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 218;
            this.label1.Text = "Base";
            // 
            // tbRowIdTemp
            // 
            this.tbRowIdTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRowIdTemp.Location = new System.Drawing.Point(1319, 649);
            this.tbRowIdTemp.Name = "tbRowIdTemp";
            this.tbRowIdTemp.Size = new System.Drawing.Size(59, 23);
            this.tbRowIdTemp.TabIndex = 219;
            // 
            // tbPath
            // 
            this.tbPath.Location = new System.Drawing.Point(834, 894);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(737, 20);
            this.tbPath.TabIndex = 220;
            // 
            // btEnterData
            // 
            this.btEnterData.AutoSize = true;
            this.btEnterData.Location = new System.Drawing.Point(1359, 298);
            this.btEnterData.Name = "btEnterData";
            this.btEnterData.Size = new System.Drawing.Size(112, 23);
            this.btEnterData.TabIndex = 221;
            this.btEnterData.Text = "Enter Data";
            this.btEnterData.UseVisualStyleBackColor = true;
            // 
            // DialogFolderAssignments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1639, 949);
            this.Controls.Add(this.btEnterData);
            this.Controls.Add(this.tbPath);
            this.Controls.Add(this.tbRowIdTemp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Folder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.tbTargetFolder);
            this.Controls.Add(this.cbTraverseThisFolder);
            this.Controls.Add(this.cbTraverse);
            this.Controls.Add(this.tbDesc);
            this.Controls.Add(this.btDisplayData);
            this.Controls.Add(this.tbFolderName);
            this.Controls.Add(this.tbDirectoryPath);
            this.Controls.Add(this.btDirectoryDialog);
            this.Controls.Add(this.dataGridViewConfig);
            this.Name = "DialogFolderAssignments";
            this.Text = "FolderAssignments";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConfig)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewConfig;
        private System.Windows.Forms.Button btDirectoryDialog;
        private System.Windows.Forms.TextBox tbDirectoryPath;
        private System.Windows.Forms.TextBox tbFolderName;
        private System.Windows.Forms.Button btDisplayData;
        private System.Windows.Forms.TextBox tbDesc;
        private System.Windows.Forms.CheckBox cbTraverseThisFolder;
        private System.Windows.Forms.CheckBox cbTraverse;
        private System.Windows.Forms.TextBox tbTargetFolder;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label Folder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbRowIdTemp;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Button btEnterData;
    }
}