using System.Windows.Forms;

namespace SymbolDB
{
    public partial class PlaybackControlForm : Form
    {
        private readonly PlaybackControl _control;

        // DialogTraverser passes itself here
        TransparentForm transparentForm; 
        public PlaybackControlForm(DialogTraverser owner)
        {
            InitializeComponent();
            _control = new PlaybackControl();
            _control.Dock = DockStyle.Fill;
            _control.AttachToParent(owner);
            Controls.Add(_control);

            // Stay on top of DialogTraverser, don't show in taskbar
            Owner = owner;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
        }
        public void SetTransparentForm(TransparentForm form)
        {
            transparentForm = form;
        }
        /// <summary>Call after player state changes to refresh button states.</summary>
        public void SyncState() => _control.SyncFromParent();

        private void PlaybackControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (transparentForm != null && !transparentForm.IsDisposed)
            {
                transparentForm.Close();
                transparentForm.Dispose();
                //transparentForm = null;
            }
        }
    }
}