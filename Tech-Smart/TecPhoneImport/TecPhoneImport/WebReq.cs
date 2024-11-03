using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace TecPhoneImport
{
    public class WebReq
    {

        public static byte[] DoRequest(string strURL, string strPostData)
        {
            //wb.DocumentCompleted+=new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);
            string strResult = "";
            HttpWebRequest wbrq;
            HttpWebResponse wbrs;
            StreamWriter sw;
            StreamReader sr;

            byte[] lnBuffer;
            byte[] lnFile;

            /*
            //Create the web request   
            wbrq = (HttpWebRequest)WebRequest.Create(strURL);
            wbrq.Method = "POST";
            // We don't always need to set the Referer but in this case    
            // the page we are posting to will only issue a response if we do   
            wbrq.Referer = strURL;
            wbrq.ContentLength = strPostData.Length;
            wbrq.ContentType = "application/x-www-form-urlencoded";
            wbrq.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; .NET CLR 1.1.4322)";
            //wbrq.SendChunked = true;
            //wbrq.TransferEncoding = "UTF-8";
            */

            wbrq = (HttpWebRequest)WebRequest.Create(strURL);
            wbrq.Method = "POST";
            wbrq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7";
            wbrq.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
            wbrq.Headers.Add("Accept-Language", "he-IL,he;q=0.9,en-US;q=0.8,en;q=0.7");
            wbrq.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            wbrq.KeepAlive = true;
            //wbrq.Headers.Add("Connection", "keep-alive");
            wbrq.Host = "magos.co.il";
            wbrq.ContentLength = strPostData.Length;
            wbrq.ContentType = "application/x-www-form-urlencoded";

            wbrq.Headers.Add("Sec-CH-UA", "\"Chromium\";v=\"128\", \"Not;A=Brand\";v=\"24\", \"Google Chrome\";v=\"128\"");
            wbrq.Headers.Add("Sec-CH-UA-Mobile", "?0");
            wbrq.Headers.Add("Sec-CH-UA-Platform", "\"Windows\"");
            wbrq.Headers.Add("Sec-Fetch-Dest", "document");
            wbrq.Headers.Add("Sec-Fetch-Mode", "navigate");
            wbrq.Headers.Add("Sec-Fetch-Site", "none");
            wbrq.Headers.Add("Sec-Fetch-User", "?1");
            wbrq.Headers.Add("Upgrade-Insecure-Requests", "1");
            wbrq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36";
            wbrq.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            // Handle cookies
            var cookieContainer = new CookieContainer();
            wbrq.CookieContainer = cookieContainer;

            // Post the data   
            sw = new StreamWriter(wbrq.GetRequestStream(), Encoding.Default);
            sw.Write(strPostData);
            sw.Close();

            // Read the returned data    
            wbrs = (HttpWebResponse)wbrq.GetResponse();

            using (BinaryReader lxBR = new BinaryReader(wbrs.GetResponseStream()))
            {
                using (MemoryStream lxMS = new MemoryStream())
                {
                    lnBuffer = lxBR.ReadBytes(1024);
                    while (lnBuffer.Length > 0)
                    {
                        lxMS.Write(lnBuffer, 0, lnBuffer.Length);
                        lnBuffer = lxBR.ReadBytes(1024);
                    }
                    lnFile = new byte[(int)lxMS.Length];
                    lxMS.Position = 0;
                    lxMS.Read(lnFile, 0, lnFile.Length);
                }
            }


            return lnFile;


        }
     

    }
}
