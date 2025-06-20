using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class WebBrowser : Form
    {
        String Url = string.Empty;

        GlobalVars gv;
        Form parent;

        public WebBrowser()
        {
            InitializeComponent();
            Url = "http://www.msn.com";
            myBrowser();
        }
        public void WebBrowser2(GlobalVars g, Form p)
        {
            g = gv;
            parent = p;
        }
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void WebBrowser_Load(object sender, EventArgs e)
        {
            toolStripButtonPrevious.Enabled = false;
            toolStripButtonNext.Enabled = false;
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            webBrowser1.Refresh();
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void toolStripButtonPrevious_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void toolStripButtonHome_Click(object sender, EventArgs e)
        {
            webBrowser1.GoHome();
        }

        private void toolStripButtonPrint_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintPreviewDialog();
        }


        private void myBrowser()
        {
            if (toolStripComboBox1.Text != "")
                Url = toolStripComboBox1.Text;
            webBrowser1.Navigate(Url);
            webBrowser1.ProgressChanged +=
            new WebBrowserProgressChangedEventHandler(webpage_ProgressChanged);
            webBrowser1.DocumentTitleChanged +=
            new EventHandler(webpage_DocumentTitleChanged);
            webBrowser1.StatusTextChanged += new EventHandler(webpage_StatusTextChanged);
            webBrowser1.Navigated += new WebBrowserNavigatedEventHandler(webpage_Navigated);
            webBrowser1.DocumentCompleted +=
            new WebBrowserDocumentCompletedEventHandler(webpage_DocumentCompleted);
        }

        private void webpage_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.CanGoBack) toolStripButtonPrevious.Enabled = true;
            else toolStripButtonPrevious.Enabled = false;

            if (webBrowser1.CanGoForward) toolStripButtonNext.Enabled = true;
            else toolStripButtonNext.Enabled = false;
            toolStripStatusLabel1.Text = "Done";
        }

        private void webpage_DocumentTitleChanged(object sender, EventArgs e)
        {
            this.Text = webBrowser1.DocumentTitle.ToString();
        }

        private void webpage_StatusTextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = webBrowser1.StatusText;
        }

        private void webpage_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Maximum = (int)e.MaximumProgress;
            toolStripProgressBar1.Value = ((int)e.CurrentProgress < 0 ||
            (int)e.MaximumProgress < (int)e.CurrentProgress) ?
            (int)e.MaximumProgress : (int)e.CurrentProgress;
        }

        private void webpage_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            toolStripComboBox1.Text = webBrowser1.Url.ToString();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.CanGoBack) toolStripButtonPrevious.Enabled = true;
            else toolStripButtonPrevious.Enabled = false;

            if (webBrowser1.CanGoForward) toolStripButtonNext.Enabled = true;
            else toolStripButtonNext.Enabled = false;
            toolStripStatusLabel1.Text = "Done";
        }

        private void toolStripComboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
               // NavigateEventArgs()
            }
        }

        private void toolStripButton1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                webBrowser1.Navigate(toolStripTextBox1.Text);
            }
        }

        private void toolStripButtonGo_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonSearch_Click(object sender, EventArgs e)
        {
            Url = "https://www.bing.com/";
            webBrowser1.Navigate(Url);
            
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
            toolStripTextBox1.SelectAll();
        }
    }
}
