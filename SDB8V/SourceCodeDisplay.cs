#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;



namespace SymbolDB
{
    

    public partial class SourceCodeDisplay : Form
    {
        GlobalVars gv;
        int lastFind = 0;
        string searchText;
        string targetDirectory = "C:/tempi/";

        readonly HttpClient httpClient = new HttpClient();
        string Url = null;

        public SourceCodeDisplay(GlobalVars g, Point myLocation)
        {
            InitializeComponent();
            gv = g;
            this.Location = myLocation;
            if (true)//myLocation.Y > 100)
            {
                myLocation.Y = 200;
                this.Location = myLocation;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void insertText(string text, string url)
        {
            this.richTextBox1.Clear();
            this.listBox1.Items.Clear();
            this.pictureBox1.Image = null;
            this.pictureBox2.Image = null;
            if (text == null)
                return;

            this.richTextBox1.Text =  text;
            this.Text = url;
            this.listBox1.Text = url;
            parseToken();
            string title = url;
            string title2 = url;
           
            
            if (title2.Length < 255)
            {
                try
                {
                    // MessageBox.Show(Path.GetDirectoryName(this.textBox1.Text));
                    title2 = Path.GetDirectoryName(url);
                }
                catch (Exception)
                {

                   // title2 = url;
                    title2 = getBase(url);

                }
            }
            title = title2;
            if (title.Contains("http:\\\\"))
            {
                string temps = title.Substring(6);
                temps = "http://" + temps;
                this.Text = temps;
            }
            else if (title.Contains("http:\\"))
            {
                string temps = title.Substring(6);
                title = "http://" + temps;
              
            }
            title = title.Replace("\\", "/");
            this.listBox1.Text = title;
            this.Text = title;// +"  <<<<<< " + title2;

            gv.website = title;
        }

        public string getBase(string urltext) /////////////////////////////////  GET the website 
        {
            int idx = 0, jdx = 0;
            int level = 0;
            idx = urltext.IndexOf("http://"); // such as http://www.d.com/astart.htm
                if (idx >= 0)
                {
                    level = 0;
                    jdx = urltext.IndexOf("/", 7);
                    if (jdx > 0)
                        return urltext.Substring(idx, jdx);
                    else
                    {
                        jdx = urltext.IndexOf("\\", 7);
                        if (jdx >=0)
                            return urltext.Substring(idx, jdx);
                    }
                    return urltext;

                }
                else
                {
                    idx = urltext.IndexOf("http:");
                    if (level == 0)
                        level = 1;
                    if (idx >= 0)
                    {
                        jdx = urltext.IndexOf("/", 7);
                        if (jdx > 0)
                            return urltext.Substring(idx, jdx);
                        else
                        {
                            jdx = urltext.IndexOf("\\", 7);
                            if (jdx >=0)
                                return urltext.Substring(idx, jdx);
                        }
                        return urltext;
                    }
                }

                return urltext;
        }

        public string getText()
        {
            return this.richTextBox1.Text;
        }

        private void FormSource_Move(object sender, EventArgs e)
        {
            
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        public void setSearchText(string text)
        {
                searchText = text;
                findNext();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
          //  PromptNumber find = new PromptNumber(this);
         //   find.Activate();
         //   find.Visible = true;
            
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            findNext();
        }
        private void findNext()
        {
            lastFind = richTextBox1.Find(searchText, lastFind, 0);
            ++lastFind;
        }
        private void replace()
        {
           
        }

        private void replaceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            richTextBox1.Select(5, 10);
        }

        /*
             *   Remove javascript and CSS:

<(script|style).*?</\1> 
Remove tags

<.*?> */
        public int parseToken(string token)
        {
            Regex regex = new Regex(token);
            int icount = 0;    
            //regex.
            MatchCollection coll = regex.Matches(this.richTextBox1.Text);

            foreach (Match m in coll)
            {
                string test = m.Value;
                if (test.Contains("img") || test.Contains("IMG"))
                {
                    //listView1.Items.Add(new ListViewItem(new string[] { m.Value.Trim(), m.Index.ToString() }));
                    //dmc2022temp  this.listBox1.Items.Add(m.Value.Trim());
                    ++icount;
                }
            //    this.textBox2.Text = m.Index.ToString();

            }
            return icount;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        // "<img src.*?\" ");
        /// <a href="http://nudes2.hegre-art.com/20061107-alena/1000/013.jpg">
        /// <img src="http://nudes2.hegre-art.com/20061107-alena/150_150/013.jpg" width="150" height="150" border="0" /></a>
        ///token1
        // <a href= "http......">
        public int parseImage(string text, Boolean upperCase)
        {
            Regex regex;
            
            if (upperCase)
                regex = new Regex("<IMG.*?\" ");
            else
                regex = new Regex("<img.*?\" ");
            //Regex regex = new Regex(token);
            int icount = 0;
            string target, targetUrl;

        //    this.pictureBox1.Image = null;
            MatchCollection coll = regex.Matches(text);

            foreach (Match m in coll)
            {
                string test = m.Value;
                //test.Contains "<img"
                {
                    int hidx = test.IndexOf("http");
                    if (hidx > 0)
                    {
                        //++hidx;
                        target = test.Substring(hidx);
                        int jdx = target.IndexOf("\"");
                        targetUrl = target.Substring(0, jdx);
                        downloadFile(targetUrl);
                        downloadImage(targetUrl);
                    }
                    else
                    {
                        int len = test.Length;
                        int idx = test.IndexOf("\""); // find end of quote
                        if (idx > 0)
                        {
                            idx++;
                            string iname = test.Substring(idx, len - idx - 2);
                            string fpath;
                            if (iname.Contains("http"))
                                fpath = iname;
                            else
                                fpath = this.Text + "/" + iname;
                            //  MessageBox.Show(fpath);
                            downloadFile(fpath);
                            downloadImage(fpath);
                            ++icount;
                        }
                    }
                    
                }
                //    this.textBox2.Text = m.Index.ToString();

            }
            
            return icount;
        }
//        <a href="http://www.hyperlinkcode.com"><img src="http://hyperlinkcode.com/images/sample-image.gif"></a> 

        public int parseTargetImage(string text)
        {
            Regex regex = new Regex("href.*?>");
            //Regex regex = new Regex(token);
            int icount = 0;
            string target, targetUrl;

        //    this.pictureBox1.Image = null;
            MatchCollection coll = regex.Matches(text);

            foreach (Match m in coll)
            {
                string test = m.Value;
                //if (test.Contains("<a href="))
                {
                    int hidx = test.IndexOf("http");
                    if (hidx > 0)
                    {
                        //++hidx;
                        target = test.Substring(hidx);
                        int jdx = target.IndexOf("\"");
                        if (jdx > 5)
                            targetUrl = target.Substring(0, jdx);
                        else
                            targetUrl = target;
                       
                        int jpgindex = 0;
                        jpgindex = targetUrl.IndexOf(".jpg");
                        if (jpgindex > 0)
                        {
                            downloadFile(targetUrl);
                            downloadImage(targetUrl);
                        }
                        else // htm page 
                        {
                         //   gv.browser.addToListBox(targetUrl);
                        }

                    }
                    else
                    {
                        int len = test.Length;
                        int idx = test.IndexOf("\""); // find end of quote
                        if (idx > 0)
                        {
                            idx++;
                            string iname = test.Substring(idx, len - idx - 2);
                            string fpath;
                            
                            fpath = gv.website + "/" + iname;
                            //  MessageBox.Show(fpath);
                            downloadFile(fpath);
                            downloadImage(fpath);
                            
                            ++icount;
                        }
                    }

                }
                //    this.textBox2.Text = m.Index.ToString();

            }

            return icount;
        }
        //        <a href="http://www.hyperlinkcode.com"><img src="http://hyperlinkcode.com/images/sample-image.gif"></a> 

        public void searchHtml()
        {
        }
        public Image downloadImage(string url)
        {
            byte[] byteArray = null;
            try
            {
                // Synchronously wait for the async call (for compatibility with existing code)
                byteArray = httpClient.GetByteArrayAsync(url).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                MessageBox.Show("load image Exception " + url);
                Application.Exit();
            }

            ImageConverter ic = new ImageConverter();
            Image img = null;
            try
            {
                img = (Image)ic.ConvertFrom(byteArray);
            }
            catch (Exception)
            {
                return null;
            }

            this.pictureBox1.Image = img;
            this.pictureBox1.Visible = true;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.Update();
            return img;
        }
        public string downloadFile(string url)
        {
            DateTime timestamp = DateTime.Now;
            string ts = timestamp.ToOADate().ToString();
            ts = ts.Replace(".", "");
            bool rc = true;
            string filePath = targetDirectory + ts + ".jpg";
            try
            {
                // Download file bytes and write to disk
                var fileBytes = httpClient.GetByteArrayAsync(url).GetAwaiter().GetResult();
                File.WriteAllBytes(filePath, fileBytes);
            }
            catch (Exception)
            {
                rc = false;
                this.pictureBox2.Load(gv.errorImage);
                MessageBox.Show("download failed for " + url);
            }
            if (rc)
            {
                try
                {
                    this.pictureBox2.Load(filePath);
                }
                catch (Exception)
                {
                    gv.debug.w("reload and display failed for " + url);
                    gv.debug.w(gv.parseItem);
                    gv.debug.w(gv.website);
                    this.pictureBox2.Load(gv.errorImage);
                }
                this.pictureBox2.Visible = true;
                this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            }
            return null;
        }
        public void downloadImages(string text)
        {
            int targetCount = this.parseTargetImage(text);

            int count = parseImage(text, false);
            if (count == 0)
                parseImage(text, true);
            
        }

        private void downloadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            downloadImage(null);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void parseToken()
        {
            int icount = parseToken("<a.*?a>");
            if (icount < 1)
                icount = parseToken("<IMG.*>");
        }
        private void parseTokenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parseToken();
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbStatus.Text = " LOADING....... ";
            int idx = listBox1.SelectedIndex;
            string text = listBox1.Text;
            gv.parseItem = text;
          //  gv.browser.setText(text);
            this.downloadImages(text);
            tbStatus.Text = "done";
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void imagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //downloadImages();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        
    }
}
