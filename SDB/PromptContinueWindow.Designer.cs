
namespace SymbolDB
{
    partial class PromptContinueWindow
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
            this.btContinue = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.numCounter = new System.Windows.Forms.NumericUpDown();
            this.tbCounter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.btCancel = new System.Windows.Forms.Button();
            this.bt1000 = new System.Windows.Forms.Button();
            this.btSet = new System.Windows.Forms.Button();
            this.cbPause = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numCounter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            this.SuspendLayout();
            // 
            // btContinue
            // 
            this.btContinue.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btContinue.Location = new System.Drawing.Point(12, 12);
            this.btContinue.Name = "btContinue";
            this.btContinue.Size = new System.Drawing.Size(75, 23);
            this.btContinue.TabIndex = 0;
            this.btContinue.Text = "Continue";
            this.btContinue.UseVisualStyleBackColor = true;
            this.btContinue.Click += new System.EventHandler(this.btContinue_Click);
            // 
            // btStop
            // 
            this.btStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btStop.Location = new System.Drawing.Point(93, 12);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(75, 23);
            this.btStop.TabIndex = 1;
            this.btStop.Text = "Stop";
            this.btStop.UseVisualStyleBackColor = true;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // numCounter
            // 
            this.numCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numCounter.Location = new System.Drawing.Point(131, 54);
            this.numCounter.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numCounter.Name = "numCounter";
            this.numCounter.Size = new System.Drawing.Size(53, 22);
            this.numCounter.TabIndex = 2;
            this.numCounter.ValueChanged += new System.EventHandler(this.numCounter_ValueChanged);
            // 
            // tbCounter
            // 
            this.tbCounter.Enabled = false;
            this.tbCounter.Location = new System.Drawing.Point(31, 54);
            this.tbCounter.Name = "tbCounter";
            this.tbCounter.Size = new System.Drawing.Size(72, 20);
            this.tbCounter.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(190, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "seconds to continue";
            // 
            // numCount
            // 
            this.numCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numCount.Location = new System.Drawing.Point(256, 12);
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
            this.numCount.Size = new System.Drawing.Size(63, 22);
            this.numCount.TabIndex = 5;
            this.numCount.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numCount.ValueChanged += new System.EventHandler(this.numCount_ValueChanged);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(174, 11);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 6;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // bt1000
            // 
            this.bt1000.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt1000.Location = new System.Drawing.Point(335, 12);
            this.bt1000.Name = "bt1000";
            this.bt1000.Size = new System.Drawing.Size(49, 23);
            this.bt1000.TabIndex = 7;
            this.bt1000.Text = "1000";
            this.bt1000.UseVisualStyleBackColor = true;
            this.bt1000.Click += new System.EventHandler(this.bt1000_Click);
            // 
            // btSet
            // 
            this.btSet.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btSet.Location = new System.Drawing.Point(335, 80);
            this.btSet.Name = "btSet";
            this.btSet.Size = new System.Drawing.Size(34, 23);
            this.btSet.TabIndex = 8;
            this.btSet.Text = "Set";
            this.btSet.UseVisualStyleBackColor = true;
            // 
            // cbPause
            // 
            this.cbPause.AutoSize = true;
            this.cbPause.Location = new System.Drawing.Point(322, 53);
            this.cbPause.Margin = new System.Windows.Forms.Padding(2);
            this.cbPause.Name = "cbPause";
            this.cbPause.Size = new System.Drawing.Size(56, 17);
            this.cbPause.TabIndex = 288;
            this.cbPause.Text = "Pause";
            this.cbPause.UseVisualStyleBackColor = true;
            // 
            // PromptContinueWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 115);
            this.Controls.Add(this.cbPause);
            this.Controls.Add(this.btSet);
            this.Controls.Add(this.bt1000);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.numCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbCounter);
            this.Controls.Add(this.numCounter);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.btContinue);
            this.Name = "PromptContinueWindow";
            this.Text = "PromptContinueWindow";
            this.Activated += new System.EventHandler(this.PromptContinueWindow_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.numCounter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btContinue;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.NumericUpDown numCounter;
        private System.Windows.Forms.TextBox tbCounter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button bt1000;
        private System.Windows.Forms.Button btSet;
        private System.Windows.Forms.CheckBox cbPause;
    }
}