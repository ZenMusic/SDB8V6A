namespace SymbolDB
{
    partial class DialogDeleteDup
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
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tbCounter = new System.Windows.Forms.TextBox();
            this.numCounter = new System.Windows.Forms.NumericUpDown();
            this.btStop = new System.Windows.Forms.Button();
            this.btContinue = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCounter)).BeginInit();
            this.SuspendLayout();
            // 
            // numCount
            // 
            this.numCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numCount.Location = new System.Drawing.Point(249, 23);
            this.numCount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numCount.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numCount.Name = "numCount";
            this.numCount.Size = new System.Drawing.Size(53, 22);
            this.numCount.TabIndex = 11;
            this.numCount.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(199, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "seconds to continue";
            // 
            // tbCounter
            // 
            this.tbCounter.Enabled = false;
            this.tbCounter.Location = new System.Drawing.Point(40, 65);
            this.tbCounter.Name = "tbCounter";
            this.tbCounter.Size = new System.Drawing.Size(72, 20);
            this.tbCounter.TabIndex = 9;
            // 
            // numCounter
            // 
            this.numCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numCounter.Location = new System.Drawing.Point(140, 65);
            this.numCounter.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numCounter.Name = "numCounter";
            this.numCounter.Size = new System.Drawing.Size(53, 22);
            this.numCounter.TabIndex = 8;
            // 
            // btStop
            // 
            this.btStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btStop.Location = new System.Drawing.Point(102, 23);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(75, 23);
            this.btStop.TabIndex = 7;
            this.btStop.Text = "Stop";
            this.btStop.UseVisualStyleBackColor = true;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // btContinue
            // 
            this.btContinue.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btContinue.Location = new System.Drawing.Point(21, 23);
            this.btContinue.Name = "btContinue";
            this.btContinue.Size = new System.Drawing.Size(75, 23);
            this.btContinue.TabIndex = 6;
            this.btContinue.Text = "Delete";
            this.btContinue.UseVisualStyleBackColor = true;
            this.btContinue.Click += new System.EventHandler(this.btContinue_Click);
            // 
            // DialogDeleteDup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 114);
            this.Controls.Add(this.numCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbCounter);
            this.Controls.Add(this.numCounter);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.btContinue);
            this.Name = "DialogDeleteDup";
            this.Text = "DIalogDeleteDup";
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCounter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbCounter;
        private System.Windows.Forms.NumericUpDown numCounter;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Button btContinue;
    }
}