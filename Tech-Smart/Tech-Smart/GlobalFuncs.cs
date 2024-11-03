using System;
using System.Collections.Generic;
using System.Text;

namespace Tech_Smart
{
    public class GlobalFuncs
    {

        /************************************************************************/
        public static int DateStrToUnixTimeStamp(string dateStr)
        {
            char[] dtHourSep = { ' ' };
            string[] dtHour = dateStr.Trim().Split(dtHourSep);
            char[] dtSep = { '/' };

            string[] dtParts = dtHour[0].Trim().Split(dtSep);
            dateStr = dtParts[2] + "-" + dtParts[1] + "-" + dtParts[0] + " " + dtHour[1];

            DateTime dt = DateTime.Parse(dateStr);

            DateTime unixEpoch = new DateTime(1970, 1, 1);
            int unixTimeStamp = (int)(dt.Subtract(unixEpoch)).TotalSeconds;
            return unixTimeStamp;
        }

        /************************************************************************/
        public static string UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime.Hour.ToString() + ":" + dtDateTime.Minute.ToString("00") + ":" + dtDateTime.Second.ToString("00") + " " + dtDateTime.Year.ToString() + "-" + dtDateTime.Month.ToString("00") + "-" + dtDateTime.Day.ToString("00");
        }

        /************************************************************************/
        public static string GetServerIP()
        {
            return "http://magos.co.il"; //"ec2-18-217-253-195.us-east-2.compute.amazonaws.com";     //"ec2-52-37-115-198.us-west-2.compute.amazonaws.com";
        }
    }
}
