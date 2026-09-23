namespace SymbolDB
{
    partial class EditNotes
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
        private void InitializeComponent2b()
        {
            this.rtbNotes = new System.Windows.Forms.RichTextBox();
            this.gbFont = new System.Windows.Forms.GroupBox();
            this.btTahoma = new System.Windows.Forms.RadioButton();
            this.rbSegoeUI = new System.Windows.Forms.RadioButton();
            this.rbArial = new System.Windows.Forms.RadioButton();
            this.btSave = new System.Windows.Forms.Button();
            this.btClose = new System.Windows.Forms.Button();
            this.dgvHoldings = new System.Windows.Forms.DataGridView();
            this.btOpenHoldingsFile = new System.Windows.Forms.Button();
            this.gbFont.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoldings)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbTradingNotes
            // 
            this.rtbNotes.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbNotes.Location = new System.Drawing.Point(11, 4);
            this.rtbNotes.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rtbNotes.Name = "rtbTradingNotes";
            this.rtbNotes.Size = new System.Drawing.Size(1110, 548);
            this.rtbNotes.TabIndex = 1;
            this.rtbNotes.Text = "";
            // 
            // gbFont
            // 
            this.gbFont.Controls.Add(this.btTahoma);
            this.gbFont.Controls.Add(this.rbSegoeUI);
            this.gbFont.Controls.Add(this.rbArial);
            this.gbFont.Location = new System.Drawing.Point(1125, 273);
            this.gbFont.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gbFont.Name = "gbFont";
            this.gbFont.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gbFont.Size = new System.Drawing.Size(68, 109);
            this.gbFont.TabIndex = 7;
            this.gbFont.TabStop = false;
            this.gbFont.Text = "Font";
            // 
            // btTahoma
            // 
            this.btTahoma.AutoSize = true;
            this.btTahoma.Location = new System.Drawing.Point(3, 78);
            this.btTahoma.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btTahoma.Name = "btTahoma";
            this.btTahoma.Size = new System.Drawing.Size(64, 17);
            this.btTahoma.TabIndex = 2;
            this.btTahoma.Text = "Tahoma";
            this.btTahoma.UseVisualStyleBackColor = true;
            this.btTahoma.Click += new System.EventHandler(this.btTahoma_CheckedChanged);
            // 
            // rbSegoeUI
            // 
            this.rbSegoeUI.AutoSize = true;
            this.rbSegoeUI.Checked = true;
            this.rbSegoeUI.Location = new System.Drawing.Point(3, 54);
            this.rbSegoeUI.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbSegoeUI.Name = "rbSegoeUI";
            this.rbSegoeUI.Size = new System.Drawing.Size(56, 17);
            this.rbSegoeUI.TabIndex = 1;
            this.rbSegoeUI.TabStop = true;
            this.rbSegoeUI.Text = "Segoe";
            this.rbSegoeUI.UseVisualStyleBackColor = true;
            this.rbSegoeUI.Click += new System.EventHandler(this.rbSegoeUI_CheckedChanged);
            // 
            // rbArial
            // 
            this.rbArial.AutoSize = true;
            this.rbArial.Location = new System.Drawing.Point(3, 26);
            this.rbArial.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbArial.Name = "rbArial";
            this.rbArial.Size = new System.Drawing.Size(45, 17);
            this.rbArial.TabIndex = 0;
            this.rbArial.Text = "Arial";
            this.rbArial.UseVisualStyleBackColor = true;
            this.rbArial.Click += new System.EventHandler(this.rbArial_CheckedChanged);
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(1137, 78);
            this.btSave.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(56, 36);
            this.btSave.TabIndex = 6;
            this.btSave.Text = "Save";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btClose
            // 
            this.btClose.Location = new System.Drawing.Point(1137, 145);
            this.btClose.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(56, 36);
            this.btClose.TabIndex = 8;
            this.btClose.Text = "Close";
            this.btClose.UseVisualStyleBackColor = true;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // dgvHoldings
            // 
            this.dgvHoldings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHoldings.Location = new System.Drawing.Point(2, 557);
            this.dgvHoldings.Name = "dgvHoldings";
            this.dgvHoldings.Size = new System.Drawing.Size(1314, 187);
            this.dgvHoldings.TabIndex = 9;
            // 
            // btOpenHoldingsFile
            // 
            this.btOpenHoldingsFile.Location = new System.Drawing.Point(1147, 425);
            this.btOpenHoldingsFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btOpenHoldingsFile.Name = "btOpenHoldingsFile";
            this.btOpenHoldingsFile.Size = new System.Drawing.Size(113, 36);
            this.btOpenHoldingsFile.TabIndex = 10;
            this.btOpenHoldingsFile.Text = "Open Holdings";
            this.btOpenHoldingsFile.UseVisualStyleBackColor = true;
            this.btOpenHoldingsFile.Click += new System.EventHandler(this.btOpenHoldingsFile_Click);
            // 
            // EditTradingNotes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1322, 750);
            this.Controls.Add(this.btOpenHoldingsFile);
            this.Controls.Add(this.dgvHoldings);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.gbFont);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.rtbNotes);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "EditTradingNotes";
            this.Text = "SPY.txt (in RTF control)";
            this.gbFont.ResumeLayout(false);
            this.gbFont.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoldings)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

    }
}

