using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Drawing;

namespace PicasaDownloader
{
    public static class Misc
    {
        public static bool CurrentlyWorkingHtml = false;
        public enum TorrentWebsite
        {
            ThePirateBay,
            Kickass,
        }

        public enum Category
        {
            All,
            Music,
            MoviesVideos,
            Applications,
            Games,
            Porn,
            Others,
        }

        public static string Subtitle_Path = "";

        public static bool IsValidUrl(string url)
        {
            Uri newUrl = null;
            return Uri.TryCreate(url, UriKind.Absolute, out newUrl) && newUrl.Scheme == Uri.UriSchemeHttp;
        }

        public static string ParseUrl(string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://"))
                return url;

            if (url.StartsWith("//"))
                return "http://" + url.Substring(2, url.Length - 2);

            if (url != "null")
                return "http://" + url;

            return "null";
        }

        public static string GetHtml(string url, bool needGzipDecompression = false)
        {
            try
            {
                if (!IsValidUrl(url))
                    return "null";


                switch (needGzipDecompression)
                {
                    case false:
                        using (WebClient wc = new WebClient())
                        {
                            wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                            return wc.DownloadString(url);
                        }

                    case true:
                        HttpWebRequest wr = WebRequest.CreateHttp(url);
                        var response = wr.GetResponse();

                        using (var z = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                        using (var reader = new StreamReader(z, Encoding.UTF8))
                            return reader.ReadToEnd();
                }

                return "null";
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("OI");
                return "null";
            }
        }

        public static bool IsSomethingPresentOnGrid(System.Windows.Forms.DataGridView dgv, string rowToCompare, string whatCompare)
        {
            try
            {
                foreach (System.Windows.Forms.DataGridViewRow r in dgv.Rows)
                    if (r.Cells[rowToCompare].Value.ToString() == whatCompare)
                        return true;

                return false;
            }
            catch
            {
                return false;
            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static Image GetImageFromUrl(string url, string toSave = "")
        {
            try
            {
                WebClient wc = new System.Net.WebClient();
                byte[] imageBytes = wc.DownloadData(url);


                MemoryStream ms = new MemoryStream(imageBytes);
                Image currImg = Image.FromStream(ms);

                if (toSave.Trim() != "")
                    currImg.Save(toSave);

                return currImg;
            }
            catch (Exception ex) { return null;  }
        }
    }
}
