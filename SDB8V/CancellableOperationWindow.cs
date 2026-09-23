using System;
using System.Threading;
using System.Windows.Forms;

public class CancellableOperationWindow : Form
{
    private Button btnCancel;
    private Label lblStatus;
    private ProgressBar progressBar;
    private CancellationTokenSource _cts;

    public CancellableOperationWindow(string operationName)
    {
        InitializeComponent(operationName);
        this.TopMost = true;  // Always on top
        this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        this.StartPosition = FormStartPosition.CenterScreen;
    }

    private void InitializeComponent(string operationName)
    {
        this.Size = new System.Drawing.Size(400, 150);
        this.Text = operationName;

        lblStatus = new Label
        {
            Location = new System.Drawing.Point(12, 12),
            Size = new System.Drawing.Size(360, 20),
            Text = "Processing..."
        };

        progressBar = new ProgressBar
        {
            Location = new System.Drawing.Point(12, 40),
            Size = new System.Drawing.Size(360, 23),
            Style = ProgressBarStyle.Marquee
        };

        btnCancel = new Button
        {
            Location = new System.Drawing.Point(150, 75),
            Size = new System.Drawing.Size(100, 30),
            Text = "Cancel",
            BackColor = System.Drawing.Color.OrangeRed
        };
        btnCancel.Click += (s, e) => Cancel();

        this.Controls.Add(lblStatus);
        this.Controls.Add(progressBar);
        this.Controls.Add(btnCancel);
    }

    public void SetCancellationTokenSource(CancellationTokenSource cts)
    {
        _cts = cts;
    }

    public void UpdateStatus(string message)
    {
        if (lblStatus.InvokeRequired)
            lblStatus.Invoke(new Action(() => lblStatus.Text = message));
        else
            lblStatus.Text = message;
    }

    private void Cancel()
    {
        _cts?.Cancel();
        btnCancel.Enabled = false;
        btnCancel.Text = "Cancelling...";
        UpdateStatus("Cancellation requested...");
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            Cancel();
            e.Cancel = true; // Keep window open until operation completes
        }
        base.OnFormClosing(e);
    }
}