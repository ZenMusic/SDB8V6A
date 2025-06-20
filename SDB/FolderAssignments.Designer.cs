
namespace SymbolDB
{
    partial class FolderAssignments
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
            this.dgvFolderAssignments = new System.Windows.Forms.DataGridView();
            this.btDirectoryDialog = new System.Windows.Forms.Button();
            this.tbDirectoryPath = new System.Windows.Forms.TextBox();
            this.tbFolderName = new System.Windows.Forms.TextBox();
            this.btEnterData = new System.Windows.Forms.Button();
            this.tbDesc = new System.Windows.Forms.TextBox();
            this.cbTraverseThisFolder = new System.Windows.Forms.CheckBox();
            this.cbTraverse = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolderAssignments)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvFolderAssignments
            // 
            this.dgvFolderAssignments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFolderAssignments.Location = new System.Drawing.Point(59, 11);
            this.dgvFolderAssignments.Margin = new System.Windows.Forms.Padding(2);
            this.dgvFolderAssignments.Name = "dgvFolderAssignments";
            this.dgvFolderAssignments.RowHeadersWidth = 51;
            this.dgvFolderAssignments.RowTemplate.Height = 24;
            this.dgvFolderAssignments.Size = new System.Drawing.Size(1193, 927);
            this.dgvFolderAssignments.TabIndex = 131;
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
            // btEnterData
            // 
            this.btEnterData.AutoSize = true;
            this.btEnterData.Location = new System.Drawing.Point(1359, 312);
            this.btEnterData.Name = "btEnterData";
            this.btEnterData.Size = new System.Drawing.Size(112, 23);
            this.btEnterData.TabIndex = 181;
            this.btEnterData.Text = "Enter Data";
            this.btEnterData.UseVisualStyleBackColor = true;
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
            // FolderAssignments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1639, 949);
            this.Controls.Add(this.cbTraverseThisFolder);
            this.Controls.Add(this.cbTraverse);
            this.Controls.Add(this.tbDesc);
            this.Controls.Add(this.btEnterData);
            this.Controls.Add(this.tbFolderName);
            this.Controls.Add(this.tbDirectoryPath);
            this.Controls.Add(this.btDirectoryDialog);
            this.Controls.Add(this.dgvFolderAssignments);
            this.Name = "FolderAssignments";
            this.Text = "FolderAssignments";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolderAssignments)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvFolderAssignments;
        private System.Windows.Forms.Button btDirectoryDialog;
        private System.Windows.Forms.TextBox tbDirectoryPath;
        private System.Windows.Forms.TextBox tbFolderName;
        private System.Windows.Forms.Button btEnterData;
        private System.Windows.Forms.TextBox tbDesc;
        private System.Windows.Forms.CheckBox cbTraverseThisFolder;
        private System.Windows.Forms.CheckBox cbTraverse;
    }
}