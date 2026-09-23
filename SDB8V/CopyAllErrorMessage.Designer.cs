#nullable disable
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SymbolDB
{
    partial class CopyAllErrorMessage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        private Label lblErrorMessage;
        private PictureBox pbCurrentImage;
        private Label lblCurrentImage;
        private PictureBox pbNextImage;
        private Label lblNextImage;
        private Button btnContinue;
        private Button btnSkip1;
        private Button btnSkip2;
        private Button btnCancel;
        private TableLayoutPanel mainLayout;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
            this.lblErrorMessage = new Label();
            this.pbCurrentImage = new PictureBox();
            this.lblCurrentImage = new Label();
            this.pbNextImage = new PictureBox();
            this.lblNextImage = new Label();
            this.btnContinue = new Button();
            this.btnSkip1 = new Button();
            this.btnSkip2 = new Button();
            this.btnCancel = new Button();
            this.mainLayout = new TableLayoutPanel();
            ((ISupportInitialize)(this.pbCurrentImage)).BeginInit();
            ((ISupportInitialize)(this.pbNextImage)).BeginInit();
            this.mainLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblErrorMessage
            // 
            this.lblErrorMessage.AutoSize = true;
            this.lblErrorMessage.Dock = DockStyle.Top;
            this.lblErrorMessage.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblErrorMessage.ForeColor = Color.Red;
            this.lblErrorMessage.Location = new Point(10, 10);
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Padding = new Padding(0, 0, 0, 10);
            this.lblErrorMessage.Size = new Size(694, 31);
            this.lblErrorMessage.TabIndex = 0;
            this.lblErrorMessage.Text = "Image Error";
            this.lblErrorMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 2;
            this.mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.mainLayout.Controls.Add(this.lblCurrentImage, 0, 0);
            this.mainLayout.Controls.Add(this.pbCurrentImage, 0, 1);
            this.mainLayout.Controls.Add(this.lblNextImage, 1, 0);
            this.mainLayout.Controls.Add(this.pbNextImage, 1, 1);
            this.mainLayout.Dock = DockStyle.Top;
            this.mainLayout.Location = new Point(10, 41);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.RowCount = 2;
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 300F));
            this.mainLayout.Size = new Size(700, 325);
            this.mainLayout.TabIndex = 1;
            // 
            // lblCurrentImage
            // 
            this.lblCurrentImage.AutoSize = true;
            this.lblCurrentImage.Dock = DockStyle.Fill;
            this.lblCurrentImage.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.lblCurrentImage.Location = new Point(3, 0);
            this.lblCurrentImage.Name = "lblCurrentImage";
            this.lblCurrentImage.Size = new Size(344, 25);
            this.lblCurrentImage.TabIndex = 0;
            this.lblCurrentImage.Text = "Current Image:";
            this.lblCurrentImage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pbCurrentImage
            // 
            this.pbCurrentImage.BorderStyle = BorderStyle.FixedSingle;
            this.pbCurrentImage.Dock = DockStyle.Fill;
            this.pbCurrentImage.Location = new Point(3, 28);
            this.pbCurrentImage.Name = "pbCurrentImage";
            this.pbCurrentImage.Size = new Size(344, 294);
            this.pbCurrentImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.pbCurrentImage.TabIndex = 1;
            this.pbCurrentImage.TabStop = false;
            // 
            // lblNextImage
            // 
            this.lblNextImage.AutoSize = true;
            this.lblNextImage.Dock = DockStyle.Fill;
            this.lblNextImage.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.lblNextImage.Location = new Point(353, 0);
            this.lblNextImage.Name = "lblNextImage";
            this.lblNextImage.Size = new Size(344, 25);
            this.lblNextImage.TabIndex = 2;
            this.lblNextImage.Text = "Next Image:";
            this.lblNextImage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pbNextImage
            // 
            this.pbNextImage.BorderStyle = BorderStyle.FixedSingle;
            this.pbNextImage.Dock = DockStyle.Fill;
            this.pbNextImage.Location = new Point(353, 28);
            this.pbNextImage.Name = "pbNextImage";
            this.pbNextImage.Size = new Size(344, 294);
            this.pbNextImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.pbNextImage.TabIndex = 3;
            this.pbNextImage.TabStop = false;
            // 
            // btnContinue
            // 
            this.btnContinue.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.btnContinue.Location = new Point(13, 376);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new Size(165, 35);
            this.btnContinue.TabIndex = 2;
            this.btnContinue.Text = "Continue (C)";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new EventHandler(this.btnContinue_Click);
            // 
            // btnSkip1
            // 
            this.btnSkip1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.btnSkip1.Location = new Point(184, 376);
            this.btnSkip1.Name = "btnSkip1";
            this.btnSkip1.Size = new Size(165, 35);
            this.btnSkip1.TabIndex = 3;
            this.btnSkip1.Text = "Skip 1 (S)";
            this.btnSkip1.UseVisualStyleBackColor = true;
            this.btnSkip1.Click += new EventHandler(this.btnSkip1_Click);
            // 
            // btnSkip2
            // 
            this.btnSkip2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.btnSkip2.Location = new Point(355, 376);
            this.btnSkip2.Name = "btnSkip2";
            this.btnSkip2.Size = new Size(165, 35);
            this.btnSkip2.TabIndex = 4;
            this.btnSkip2.Text = "Skip 2 (2)";
            this.btnSkip2.UseVisualStyleBackColor = true;
            this.btnSkip2.Click += new EventHandler(this.btnSkip2_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.btnCancel.Location = new Point(526, 376);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(165, 35);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel (Esc)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            // 
            // CopyAllErrorMessage
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new Size(720, 425);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSkip2);
            this.Controls.Add(this.btnSkip1);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.mainLayout);
            this.Controls.Add(this.lblErrorMessage);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CopyAllErrorMessage";
            this.Padding = new Padding(10);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Copy All - Image Error";
            ((ISupportInitialize)(this.pbCurrentImage)).EndInit();
            ((ISupportInitialize)(this.pbNextImage)).EndInit();
            this.mainLayout.ResumeLayout(false);
            this.mainLayout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}