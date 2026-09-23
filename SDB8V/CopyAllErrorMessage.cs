#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class CopyAllErrorMessage : Form
    {
        public enum ErrorAction
        {
            Continue,
            Skip1,
            Skip2,
            Cancel
        }

        public ErrorAction SelectedAction { get; private set; }
        private readonly int _imageIndex;
        private readonly GlobalVars gv;

        public CopyAllErrorMessage(int imageIdx, Image currentImage, Image nextImage, GlobalVars g)
        {
            InitializeComponent();

            _imageIndex = imageIdx;
            gv = g;
            SelectedAction = ErrorAction.Cancel; // Default to Cancel

            // Set the error message with the image index
            lblErrorMessage.Text = $"Image Error on image #{imageIdx:N0}";

            // Display the images safely (create copies to avoid disposal issues)
            if (currentImage != null)
            {
                try
                {
                    pbCurrentImage.Image = new Bitmap(currentImage);
                }
                catch (Exception ex)
                {
                    lblCurrentImage.Text = $"Current Image: [Error loading - {ex.Message}]";
                    lblCurrentImage.ForeColor = Color.Red;
                }
            }
            else
            {
                lblCurrentImage.Text = "Current Image: [Not available]";
                lblCurrentImage.ForeColor = Color.Gray;
            }

            if (nextImage != null)
            {
                try
                {
                    pbNextImage.Image = new Bitmap(nextImage);
                }
                catch (Exception ex)
                {
                    lblNextImage.Text = $"Next Image: [Error loading - {ex.Message}]";
                    lblNextImage.ForeColor = Color.Red;
                }
            }
            else
            {
                lblNextImage.Text = "Next Image: [Not available]";
                lblNextImage.ForeColor = Color.Gray;
            }

            // Set keyboard shortcuts
            btnContinue.Text = "Continue (C)";
            btnSkip1.Text = "Skip 1 (S)";
            btnSkip2.Text = "Skip 2 (2)";
            btnCancel.Text = "Cancel (Esc)";

            // Enable key preview for keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += CopyAllErrorMessage_KeyDown;
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            SelectedAction = ErrorAction.Continue;
            gv.copyAllReturnCode = 0; // OK = 0
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnSkip1_Click(object sender, EventArgs e)
        {
            SelectedAction = ErrorAction.Skip1;
            gv.copyAllReturnCode = 1; // No = 1 (Skip 1)
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void btnSkip2_Click(object sender, EventArgs e)
        {
            SelectedAction = ErrorAction.Skip2;
            gv.copyAllReturnCode = 2; // Yes = 2 (Skip 2)
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectedAction = ErrorAction.Cancel;
            gv.copyAllReturnCode = -1; // Cancel = -1
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void CopyAllErrorMessage_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.C:
                    btnContinue_Click(sender, e);
                    break;
                case Keys.S:
                    btnSkip1_Click(sender, e);
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    btnSkip2_Click(sender, e);
                    break;
                case Keys.Escape:
                    btnCancel_Click(sender, e);
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up image resources
                if (pbCurrentImage?.Image != null)
                {
                    pbCurrentImage.Image.Dispose();
                    pbCurrentImage.Image = null;
                }

                if (pbNextImage?.Image != null)
                {
                    pbNextImage.Image.Dispose();
                    pbNextImage.Image = null;
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}