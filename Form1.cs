using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;

namespace PicasaDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        List<string> linksToAccess;
        List<string> linksToImages;
        private void button1_Click(object sender, EventArgs e)
        {
            string link = Misc.ParseUrl(textBox1.Text);
            link = Regex.Replace(link, @"\?&firstFileToShow=([0-9]+)", "");
            string linkNext = link + "?&firstFileToShow=";


            Match mId = Regex.Match(link, @"all-images/([0-9a-zA-Z]+)/");
            string id = mId.Groups[1].Value.ToString();


            label2.Text = "Started download";
            foreach (Control c in this.Controls)
                c.Enabled = false;

            Thread tr = new Thread(() =>
            {
                string content = Misc.GetHtml(link);
                MatchCollection mcLinks = Regex.Matches(content, @"javascript:viewAllNavigate\(([0-9]+)\);");

                linksToAccess = new List<string>();
                linksToAccess.Add(link);
                foreach (Match m in mcLinks)
                    if (!linksToAccess.Contains(linkNext + m.Groups[1].Value.ToString()))
                        linksToAccess.Add(linkNext + m.Groups[1].Value.ToString());

                Invoke(new MethodInvoker(delegate() { label2.Text = "Getting all images from all pages"; }));
                linksToImages = new List<string>();
                foreach (string l in linksToAccess)
                {
                    string cLink = Misc.GetHtml(l);
                    MatchCollection mcPhotos = Regex.Matches(cLink, @"<img src=""(.*)""/>");

                    foreach (Match m in mcPhotos)
                        linksToImages.Add(m.Groups[1].Value.ToString());
                }

                Invoke(new MethodInvoker(delegate() { label2.Text = "Downloading " + linksToImages.Count.ToString() + " images from all pages"; }));
                string path = System.IO.Directory.GetCurrentDirectory().ToString() + @"\images\" + id.ToString() + @"\";

                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                foreach (string i in linksToImages)
                {
                    Invoke(new MethodInvoker(delegate() { label2.Text = "Downloading #" + linksToImages.IndexOf(i).ToString() + " from " + linksToImages.Count.ToString(); }));
                    Image im = Misc.GetImageFromUrl(i, path + linksToImages.IndexOf(i).ToString() + ".jpg");

                    if (im == null)
                        Invoke(new MethodInvoker(delegate() { richTextBox1.Text += "Error on #" + linksToImages.IndexOf(i.ToString()) + Environment.NewLine;   }));
                        
                }


                Invoke(new MethodInvoker(delegate() {
                    foreach (Control c in this.Controls)
                        c.Enabled = true;

                    label2.Text = "End!";
                }));
            });
            tr.IsBackground = true;
            tr.Start();

            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Image m = Misc.GetImageFromUrl(@"http://dc481.4shared.com/img/c55uplZ6/s7/14098f70bf0/DSC_0062", @"C:\a.jpg");
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        
    }
}
