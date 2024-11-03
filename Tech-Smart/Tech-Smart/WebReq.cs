using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Tech_Smart
{
    public class WebReq
    {

        public static string DoRequest(string strURL, string strPostData)
        {
            //wb.DocumentCompleted+=new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);
            string strResult = "";
            HttpWebRequest wbrq;
            HttpWebResponse wbrs;
            StreamWriter sw;
            StreamReader sr;
                        
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
            //wbrq.SendChunked = true;
            //wbrq.TransferEncoding = "UTF-8";

            // Handle cookies
            var cookieContainer = new CookieContainer();
            wbrq.CookieContainer = cookieContainer;

            // Post the data   
            sw = new StreamWriter(wbrq.GetRequestStream(), Encoding.Default);
            sw.Write(strPostData);
            sw.Close();


            // Read the returned data    
            wbrs = (HttpWebResponse)wbrq.GetResponse();
            sr = new StreamReader(wbrs.GetResponseStream(), Encoding.UTF8);
            strResult = sr.ReadToEnd().Trim();
            sr.Close();
            return strResult;
      
            /***
            //Create the web request   
            wbrq = (HttpWebRequest)WebRequest.Create(strURL);
            wbrq.Method = "POST";
            // We don't always need to set the Referer but in this case    
            // the page we are posting to will only issue a response if we do   
            wbrq.ContentLength = strPostData.Length;
            wbrq.ContentType = "application/x-www-form-urlencoded";
            //wbrq.Connection = "keep-alive";
            wbrq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng;q=0.8,application/signed-exchange;v=b3;q=0.7";
            wbrq.Host = "magos.co.il";
            wbrq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36";
            wbrq.KeepAlive = true;
            //wbrq.Headers["Connection"] = "keep-alive"; 

            //wbrq.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; .NET CLR 1.1.4322)";
            //wbrq.SendChunked = true;

            //wbrq.TransferEncoding = "UTF-8";
            ****/
                       

        }
    
    
        public static string RemoveJS(string htmlTXT){

            while (htmlTXT.Contains("<script"))
            {
                int len = htmlTXT.Length;
                int a = htmlTXT.IndexOf("<script", StringComparison.OrdinalIgnoreCase);
                int b = htmlTXT.IndexOf("</script>", StringComparison.OrdinalIgnoreCase);
                string s = htmlTXT.Substring(a, b + 9 - a);
                htmlTXT = htmlTXT.Replace(s, String.Empty);
            }
            htmlTXT = htmlTXT.Replace("onload", "onload22");

            return htmlTXT;
        }    
    

    }
}
