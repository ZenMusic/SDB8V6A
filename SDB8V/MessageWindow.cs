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
    public partial class MessageWindow : Form
    {
        GlobalVars gv;
        public MessageWindow()
        {
            InitializeComponent();
        }
        public MessageWindow(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
            new Thread(SampleFunction).Start();
        }
        public void AppendTextBox(string txt)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { txt });
                return;
            }
            tbMessage.Text = txt;
            //if (img != null)
               // pb1.Image = img;
        }
        void SampleFunction()
        {
            AppendTextBox("hi.");
            Thread.Sleep(1000);
        }
    }
}
