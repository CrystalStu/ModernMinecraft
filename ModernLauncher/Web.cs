using System;
using System.Net;
using System.Text;

namespace ModernLauncher
{
    public class Web
    {
        public static bool CheckFile(string fileUrl)
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(fileUrl);
                res = (HttpWebResponse)req.GetResponse();
                if (res.ContentLength != 0) return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (req != null)
                {
                    req.Abort();
                }
                if (res != null)
                {
                    res.Close();
                }
            }
            return false;
        }

        public static string DownloadText(string url)
        {
            WebClient client = new WebClient();
            byte[] buffer = client.DownloadData(url);
            return Encoding.ASCII.GetString(buffer);
        }
    }
}
