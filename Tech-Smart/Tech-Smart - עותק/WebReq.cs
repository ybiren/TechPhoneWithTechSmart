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


            //Create the web request   
            wbrq = (HttpWebRequest)WebRequest.Create(strURL);
            wbrq.Method = "POST";
            // We don't always need to set the Referer but in this case    
            // the page we are posting to will only issue a response if we do   
            //wbrq.Referer = strURL;
            //wbrq.ContentLength = strPostData.Length;
            wbrq.ContentType = "application/x-www-form-urlencoded";
            //wbrq.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; .NET CLR 1.1.4322)";
            //wbrq.SendChunked = true;

            //wbrq.TransferEncoding = "UTF-8";
    
            
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
