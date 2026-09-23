namespace SymbolDB
{
    partial class Database
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
            tbMessage = new System.Windows.Forms.TextBox();
            tbFpath = new System.Windows.Forms.TextBox();
            btBackupDB = new System.Windows.Forms.Button();
            dgvDatabase = new System.Windows.Forms.DataGridView();
            btLoadFileInfoItem = new System.Windows.Forms.Button();
            btAddRow = new System.Windows.Forms.Button();
            btDeleteRow = new System.Windows.Forms.Button();
            btApplyUpdates = new System.Windows.Forms.Button();
            btClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)dgvDatabase).BeginInit();
            SuspendLayout();
            // 
            // tbMessage
            // 
            tbMessage.Location = new System.Drawing.Point(0, 2);
            tbMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tbMessage.Name = "tbMessage";
            tbMessage.Size = new System.Drawing.Size(798, 23);
            tbMessage.TabIndex = 183;
            // 
            // tbFpath
            // 
            tbFpath.Location = new System.Drawing.Point(0, 29);
            tbFpath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tbFpath.Name = "tbFpath";
            tbFpath.Size = new System.Drawing.Size(788, 23);
            tbFpath.TabIndex = 182;
            tbFpath.TextChanged += tbFpath_TextChanged;
            // 
            // btBackupDB
            // 
            btBackupDB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btBackupDB.BackColor = System.Drawing.Color.FromArgb(255, 128, 128);
            btBackupDB.Location = new System.Drawing.Point(695, 478);
            btBackupDB.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btBackupDB.Name = "btBackupDB";
            btBackupDB.Size = new System.Drawing.Size(93, 28);
            btBackupDB.TabIndex = 327;
            btBackupDB.Text = "BKUP DB";
            btBackupDB.UseVisualStyleBackColor = false;
            btBackupDB.Click += btBackupDB_Click;
            // 
            // dgvDatabase
            // 
            dgvDatabase.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgvDatabase.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDatabase.Location = new System.Drawing.Point(12, 105);
            dgvDatabase.Name = "dgvDatabase";
            dgvDatabase.Size = new System.Drawing.Size(776, 324);
            dgvDatabase.TabIndex = 328;
            // 
            // btLoadFileInfoItem
            // 
            btLoadFileInfoItem.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btLoadFileInfoItem.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            btLoadFileInfoItem.Location = new System.Drawing.Point(30, 59);
            btLoadFileInfoItem.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btLoadFileInfoItem.Name = "btLoadFileInfoItem";
            btLoadFileInfoItem.Size = new System.Drawing.Size(151, 28);
            btLoadFileInfoItem.TabIndex = 329;
            btLoadFileInfoItem.Text = "Load FileInfoItem";
            btLoadFileInfoItem.UseVisualStyleBackColor = false;
            btLoadFileInfoItem.Click += btLoadFileInfoItem_Click;
            // 
            // btAddRow
            // 
            btAddRow.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btAddRow.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
            btAddRow.Location = new System.Drawing.Point(199, 59);
            btAddRow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btAddRow.Name = "btAddRow";
            btAddRow.Size = new System.Drawing.Size(93, 28);
            btAddRow.TabIndex = 330;
            btAddRow.Text = "Add Row";
            btAddRow.UseVisualStyleBackColor = false;
            btAddRow.Click += btAddRow_Click;
            // 
            // btDeleteRow
            // 
            btDeleteRow.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btDeleteRow.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
            btDeleteRow.Location = new System.Drawing.Point(326, 59);
            btDeleteRow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btDeleteRow.Name = "btDeleteRow";
            btDeleteRow.Size = new System.Drawing.Size(93, 28);
            btDeleteRow.TabIndex = 331;
            btDeleteRow.Text = "Delete";
            btDeleteRow.UseVisualStyleBackColor = false;
            btDeleteRow.Click += btDeleteRow_Click;
            // 
            // btApplyUpdates
            // 
            btApplyUpdates.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btApplyUpdates.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
            btApplyUpdates.Location = new System.Drawing.Point(443, 59);
            btApplyUpdates.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btApplyUpdates.Name = "btApplyUpdates";
            btApplyUpdates.Size = new System.Drawing.Size(93, 28);
            btApplyUpdates.TabIndex = 332;
            btApplyUpdates.Text = "Update";
            btApplyUpdates.UseVisualStyleBackColor = false;
            btApplyUpdates.Click += btApplyUpdates_Click;
            // 
            // btClose
            // 
            btClose.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btClose.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
            btClose.Location = new System.Drawing.Point(686, 61);
            btClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            btClose.Name = "btClose";
            btClose.Size = new System.Drawing.Size(93, 28);
            btClose.TabIndex = 333;
            btClose.Text = "Close";
            btClose.UseVisualStyleBackColor = false;
            btClose.Click += btClose_Click;
            // 
            // Database
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 517);
            Controls.Add(btClose);
            Controls.Add(btApplyUpdates);
            Controls.Add(btDeleteRow);
            Controls.Add(btAddRow);
            Controls.Add(btLoadFileInfoItem);
            Controls.Add(dgvDatabase);
            Controls.Add(btBackupDB);
            Controls.Add(tbMessage);
            Controls.Add(tbFpath);
            Name = "Database";
            Text = "Database";
            FormClosing += Database_FormClosing;
            Load += Database_Load;
            ((System.ComponentModel.ISupportInitialize)dgvDatabase).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox tbMessage;
        private System.Windows.Forms.TextBox tbFpath;
        private System.Windows.Forms.Button btBackupDB;
        private System.Windows.Forms.DataGridView dgvDatabase;
        private System.Windows.Forms.Button btLoadFileInfoItem;
        private System.Windows.Forms.Button btAddRow;
        private System.Windows.Forms.Button btDeleteRow;
        private System.Windows.Forms.Button btApplyUpdates;
        private System.Windows.Forms.Button btClose;
    }
}