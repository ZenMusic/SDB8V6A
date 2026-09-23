using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class StatusOfProcessing : Form
    {
        private readonly SynchronizationContext _syncContext;
        private CancellationTokenSource _cancellationTokenSource;

        public StatusOfProcessing(Point location, Size size)
        {
            InitializeComponent();

            // Capture the UI thread's synchronization context
            _syncContext = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();

            _cancellationTokenSource = new CancellationTokenSource();

            // Fix: Use the passed size parameter instead of Size.Width
            location.X = location.X + size.Width;
            this.Location = location;
            this.Size = size;
        }

        /// <summary>
        /// Gets the cancellation token for this operation
        /// </summary>
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        /// <summary>
        /// Shows the form on its own UI thread
        /// </summary>
        public static StatusOfProcessing ShowOnNewThread(Point location, Size size)
        {
            StatusOfProcessing form = null;
            var formReadyEvent = new ManualResetEventSlim(false);

            var thread = new Thread(() =>
            {
                form = new StatusOfProcessing(location, size);
                formReadyEvent.Set();
                Application.Run(form);
            })
            {
                IsBackground = true,
                Name = "StatusOfProcessing Thread"
            };

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            // Wait for form to be created
            formReadyEvent.Wait();
            return form;
        }

        public void UpdateStatus(int nextIdx, int maxIdx, int lastIdx, string status)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<int, int, int, string>(UpdateStatus), nextIdx, maxIdx, lastIdx, status);
            }
            else
            {
                tbNextIdx.Text = nextIdx.ToString();
                tbMinIdx.Text = maxIdx.ToString();
                tbLastIdxPreviewed.Text = lastIdx.ToString();
                tbLastFilePreviewed.Text = status;
            }
        }

        public void UpdateStatusHighest(int highestIdx)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<int>(UpdateStatusHighest), highestIdx);
            }
            else
            {
                tbMinIdx.Text = highestIdx.ToString();
            }
        }

        public void SafeClose()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(SafeClose));
            }
            else
            {
                _cancellationTokenSource?.Dispose();
                this.Close();
            }
        }

        private void btStatus_Click(object sender, EventArgs e)
        {
            // Signal cancellation
            _cancellationTokenSource?.Cancel();
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Ensure cancellation is signaled when form closes
            _cancellationTokenSource?.Cancel();
            base.OnFormClosing(e);
        }

        private void btCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
