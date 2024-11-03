using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;


namespace Tech_Smart
{
    class Program
    {
        enum EAppType
        {
         Tech = 1,
         Pinuyeem = 3
        }

        const string VERSION = "1.46";
        
        const string CALL_TYPE = "1";
        const string PART_TYPE = "2";
        const string EXEC_REP_TYPE = "3";

        static string applicationType = String.Empty;
        static string code=String.Empty;
        static string charsType = String.Empty;
        static string user = String.Empty;
        static string pass = String.Empty;
        static string callType = String.Empty;
        static string callDateTime = String.Empty;
        static string isContinue = String.Empty;
        static string inService = String.Empty;
        static string custName = String.Empty;
        static string siteName = String.Empty;
        static string projName = String.Empty;
        static string address = String.Empty;
        static string id = String.Empty;
        static string location = String.Empty;
        static string recieverServiceName = String.Empty;
        static string callDesc = String.Empty;
        static string callStatus = "2"; //String.Empty;
        static string op = "insCall";
        static string recordType = CALL_TYPE;
        static string currDir = String.Empty;
        static List<ClsInvParts> lstInvParts = new List<ClsInvParts>();
        static List<ClsParts> lstParts = new List<ClsParts>();
        static List<ClsUnit> lstUnits = new List<ClsUnit>();
        static List<ClsUnitHandling> lstUnitHandling = new List<ClsUnitHandling>();
        
        static string execRepFileName = String.Empty;

        /*
        Chr Set ~ Oem/Ansii 
User & Pass~ tec1~123456 
ID ~ 35431511 
Type ~ שוטפת
Date ~ 13/07/2015 
Time ~ 16:55 
Cont ~ המשך
Customer Name~ פולגת רשת חנויות
Site Name ~ גרנד קניון חיפה
Project ~ 
Addr. ~ דרך שמחה גולן 54 חיפה
Identification ~ זיהוי
Location ~ צפון
Being Served ~ משה כהן
Description ~ לפי דוח ביצוע 301424, החלפת 4 גמישים ב- 0 היחידות
*/
        /************************************************************************/
        private static string ConvToUtf8Encoding(string line,Encoding encoding )
        {
            /*
            byte[] encBytes = encoding.GetBytes(line);
            byte[] utf8Bytes = Encoding.Convert(encoding, Encoding.UTF8, encBytes);
            return Encoding.UTF8.GetString(utf8Bytes);
            */
            return line;     
        }

        /************************************************************************/
        private static int DateStrToUnixTimeStamp(string dateStr)
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

        private static string GetServerIP(){

            return "ec2-54-67-7-60.us-west-1.compute.amazonaws.com"; //"ec2-52-37-115-198.us-west-2.compute.amazonaws.com";
        }    
            
        
        
        /************************************************************************/
        private static string GetCallTypeCode(string val){
          string typeCode="0";
          val = System.Web.HttpUtility.UrlDecode(val);
          switch(val){
              case "שוטפת":
              typeCode = "1";
              break; 
                case "מונעת":
              typeCode = "2";
              break;
                  case "פרויקט":
              typeCode = "3";
              break;
              default:
               break; 
          }   
            return typeCode;
        }

        /************************************************************************/
        private static void AddUnit(string[] arrKeyVal)
        {

            ClsUnit clsUnit = new ClsUnit();
            clsUnit.unitCode = Convert.ToInt32(arrKeyVal[1].Trim());
            clsUnit.unitDesc = System.Web.HttpUtility.UrlEncode(arrKeyVal[2].Trim());
            clsUnit.building = System.Web.HttpUtility.UrlEncode(arrKeyVal[3].Trim());
            clsUnit.floor = System.Web.HttpUtility.UrlEncode(arrKeyVal[4].Trim());
            clsUnit.room = System.Web.HttpUtility.UrlEncode(arrKeyVal[5].Trim());
            clsUnit.manufacturer = System.Web.HttpUtility.UrlEncode(arrKeyVal[6].Trim());
            clsUnit.model = System.Web.HttpUtility.UrlEncode(arrKeyVal[7].Trim());
            clsUnit.outputTon = Convert.ToInt32(arrKeyVal[8].Trim());
            if (arrKeyVal.Length > 9)
                clsUnit.remarks = System.Web.HttpUtility.UrlEncode(arrKeyVal[9].Trim());
            lstUnits.Add(clsUnit);

        }

        /************************************************************************/
        private static void AddUnitHandling(string[] arrKeyVal)
        {

            ClsUnitHandling clsUnitHandling = new ClsUnitHandling();
            clsUnitHandling.unitCode = Convert.ToInt32(arrKeyVal[1].Trim());
            clsUnitHandling.handlingCode = Convert.ToInt32(arrKeyVal[2].Trim());
            clsUnitHandling.handlingDesc = System.Web.HttpUtility.UrlEncode(arrKeyVal[3].Trim());
            clsUnitHandling.isDone = 0;
            if (arrKeyVal.Length > 4 && arrKeyVal[4].Trim() != String.Empty)
                clsUnitHandling.isDone = Convert.ToInt16(arrKeyVal[4].Trim());
            if (arrKeyVal.Length > 5)
                clsUnitHandling.remarks = System.Web.HttpUtility.UrlEncode(arrKeyVal[5].Trim());
            lstUnitHandling.Add(clsUnitHandling);
        }
                    


        /************************************************************************/
        /*
        private static string GetStatusStrByCode(int statusCode){
        
        string status
        

        }
        */

        /************************************************************************/
        private static void SetValues(string line)
        {
            char[] sep = { '~' };
            string[] arrKeyVal = line.Split(sep);
            if (arrKeyVal.Length >= 2)
            {
                string key = arrKeyVal[0].Trim();
                string val = arrKeyVal[1].Trim();
                if(key!="Date" && key!="Time")
                  val = System.Web.HttpUtility.UrlEncode(val);
                switch (key)
                {
                    case "ID":
                    code = val;
                    break;
                    case "Chr Set":
                    charsType = val;
                    break;
                    case "Application Type":
                    applicationType = val;
                    break;
                    case "User and Pass":
                    user = val;
                    pass = arrKeyVal[2].Trim();
                    break;
                    case "Type":
                    callType = GetCallTypeCode(val);
                    break;
                    case "Date":
                    case "Time":
                    callDateTime+= " " + val;
                    break;
                    case "Cont":
                    isContinue = (val == System.Web.HttpUtility.UrlEncode("המשך") ? "1" :"0");
                    break;
                    case "in service":
                    inService = (val == System.Web.HttpUtility.UrlEncode("בשירות") ? "1" : "0");
                    break;
                    case "Customer Name":
                    custName = val;
                    break;
                    case "Site Name":
                    siteName = val;
                    break;
                    case "Project":
                    projName = val;
                    break;
                    case "Addr.":
                    address = val;
                    break;
                    case "Identification":
                    id = val;
                    break;
                    case "Location":
                    location = val;
                    break;
                    case "Being Served":
                    recieverServiceName = val; 
                    break;
                    case "Description":
                    callDesc = val;
                    break;
                    case "Record-Type":
                    recordType = val;
                    break;
                    case "File-Name":
                    execRepFileName = val;
                    break;
                    case "Unit":
                    AddUnit(arrKeyVal);
                    break;
                    case "Handling":
                    AddUnitHandling(arrKeyVal);
                    break;
                    default:
                    break;
                    
         
                }
            }

            if (line.ToUpper().Contains("PART"))
            {
                if (recordType==CALL_TYPE && arrKeyVal.Length == 5) //fixed invited part line
                {
                    ClsInvParts clsInvParts = new ClsInvParts();
                    clsInvParts.callCode = code;
                    clsInvParts.partCode = arrKeyVal[1].Trim();
                    clsInvParts.partDesc = System.Web.HttpUtility.UrlEncode(arrKeyVal[2].Trim());
                    clsInvParts.partUnit = System.Web.HttpUtility.UrlEncode(arrKeyVal[3].Trim());
                    clsInvParts.partQuantity = arrKeyVal[4].Trim();
                    lstInvParts.Add(clsInvParts);
                }

                if (recordType == PART_TYPE && arrKeyVal.Length == 7) //fixed part line
                {
                    ClsParts clsParts = new ClsParts();
                    clsParts.op = arrKeyVal[1].Trim();
                    clsParts.code = System.Web.HttpUtility.UrlEncode(arrKeyVal[2].Trim());
                    clsParts.desc_ = System.Web.HttpUtility.UrlEncode(arrKeyVal[3].Trim());
                    clsParts.unit = System.Web.HttpUtility.UrlEncode(arrKeyVal[4].Trim());
                    clsParts.barcode = System.Web.HttpUtility.UrlEncode(arrKeyVal[5].Trim());
                    clsParts.status = System.Web.HttpUtility.UrlEncode(arrKeyVal[6].Trim());
                    clsParts.source = "1";
                    lstParts.Add(clsParts);
                }
            
            }
        
        }

        private static bool IsUserExist(){
            

            string qryStr = ("op=login&userName=" + user + "&pass=" + pass);
            qryStr = qryStr.Replace("'", "''");
            var strResult = WebReq.DoRequest("http://" + GetServerIP() + "/tech_post.php", qryStr);
            return strResult.Contains("perm");
        }		
        
        
        /************************************************************************/
        static void Main(string[] args)
        {
         
            try
            {
                
                if (args.Length > 0 && Directory.Exists(args[0]))
                    currDir = args[0];
                else
                    currDir = Directory.GetCurrentDirectory();

                File.Delete(Path.Combine(currDir, "Tec-Phone-Log.txt"));

                StreamReader sr = new StreamReader(Path.Combine(currDir, "Tec-Phone-Get.txt"), Encoding.Default);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (sr.CurrentEncoding != Encoding.UTF8)
                        line = ConvToUtf8Encoding(line, sr.CurrentEncoding);

                    if (line.Trim().ToUpper() == "DELETE")
                        op = "delCall";
                    if (line.Trim().ToUpper() == "REPORT")
                        op = "getCallRep";
                    
                    SetValues(line);
                }
                sr.Close();

                if (!String.IsNullOrEmpty(callDateTime))
                    callDateTime = DateStrToUnixTimeStamp(callDateTime).ToString();

                
                if(!IsUserExist()){
                    throw new Exception("user does not exists");
                }
           
                if (recordType == PART_TYPE)
                {
                    new ClsPartsManager(op,currDir,lstParts);
                    return;
                }

                if (recordType == EXEC_REP_TYPE)
                {
                    CookieContainer cookies = new CookieContainer();
                    //add or use cookies
                    NameValueCollection querystring = new NameValueCollection();
                    querystring["callCode"] = code; 
                    querystring["execRepFileName"] = code + "_rep" + Path.GetExtension(execRepFileName); 
                    string uploadfile = Path.Combine(currDir, execRepFileName);

                    //everything except upload file and url can be left blank if needed
                    var strResult2 = ClsUploadFile.UploadFileEx(uploadfile, "http://" + GetServerIP() + "/tech_post.php", String.Empty, String.Empty, querystring, cookies);
                    Console.WriteLine(strResult2);
                    return;
                }


                string qryStr = String.Format("code={0}&callCode={0}&charsType={1}&callType={2}&callDateTime={3}&isContinue={4}&inService={5}&custName={6}&siteName={7}&projName={8}&address={9}&id={10}&location={11}&recieverServiceName={12}&callDesc={13}&callStatus={14}&userCode={15}", code, charsType, callType, callDateTime, isContinue,inService, custName, siteName, projName, address, id, location, recieverServiceName, callDesc, callStatus, user);
                qryStr = "op=" + op + "&" + qryStr;
                qryStr = qryStr.Replace("'", "''");
                var strResult = WebReq.DoRequest("http://" + GetServerIP() + "/tech_post.php", qryStr);

                if (op == "insCall") //insert invited parts
                {

                    qryStr = String.Format("op={0}&callCode={1}", "delInvParts", code);
                    strResult = WebReq.DoRequest("http://" + GetServerIP() + "/tech_post.php", qryStr);
                                        
                    foreach(var clsInvParts  in lstInvParts){
                      qryStr = String.Format("op={0}&code={1}&callCode={2}&partDesc={3}&partUnit={4}&partQuantity={5}", "insInvPart", clsInvParts.partCode, clsInvParts.callCode, clsInvParts.partDesc, clsInvParts.partUnit, clsInvParts.partQuantity);
                      qryStr = qryStr.Replace("'", "''");
                      Console.WriteLine(qryStr);
                      strResult = WebReq.DoRequest("http://" + GetServerIP() + "/tech_post.php", qryStr);
                      Console.WriteLine(strResult);
                  }  
                
                  //units
                  foreach (var clsUnit in lstUnits)
                  {
                      qryStr = String.Format("op={0}&unitCode={1}&callCode={2}&unitDesc={3}&building={4}&floor={5}&room={6}&manufacturer={7}&model={8}&outputTon={9}&remarks={10}", "insCallUnit", clsUnit.unitCode, code, clsUnit.unitDesc,clsUnit.building, clsUnit.floor, clsUnit.room, clsUnit.manufacturer,clsUnit.model,clsUnit.outputTon,clsUnit.remarks);
                      qryStr = qryStr.Replace("'", "''");
                      Console.WriteLine(qryStr);
                      strResult = WebReq.DoRequest("http://" + GetServerIP() + "/tech_post.php", qryStr);
                      Console.WriteLine(strResult);
                  }

                  //unit handling
                  foreach (var clsUnitHandling in lstUnitHandling)
                  {
                      qryStr = String.Format("op={0}&unitCode={1}&handlingCode={2}&handlingDesc={3}&isDone={4}&remarks={5}&callCode={6}", "insCallUnitHandling", clsUnitHandling.unitCode, clsUnitHandling.handlingCode, clsUnitHandling.handlingDesc, clsUnitHandling.isDone, clsUnitHandling.remarks, code);
                      qryStr = qryStr.Replace("'", "''");
                      Console.WriteLine(qryStr);
                      strResult = WebReq.DoRequest("http://" + GetServerIP() + "/tech_post.php", qryStr);
                      Console.WriteLine(strResult);
                  }

                }
                
                
                if (op == "getCallRep")
                {
                    if (strResult.Contains("callCode"))
                    {
                        strResult = strResult.Replace(",false", String.Empty);
                        List<ClsRepData> repData = null;
                        List<ClsUnit> unitsData = null;
                        List<ClsSupParts> supPartsData = null;
                        List<ClsUnitHandling> unitHandlingData = null;

                        List<CallRep> callRep = JsonConvert.DeserializeObject<List<CallRep>>(strResult);

                        for (int i = 0; i < callRep.Count; i++)
                        {
                            if (callRep[i].repData != null)
                            {
                                repData = JsonConvert.DeserializeObject<List<ClsRepData>>(callRep[i].repData);
                            }
                            if (callRep[i].unitsData != null && callRep[i].unitsData != "[false]")
                            {
                                 unitsData = JsonConvert.DeserializeObject<List<ClsUnit>>(callRep[i].unitsData);
                            }
                            if (callRep[i].supPartsData != null &&	callRep[i].supPartsData!="[false]")
                            {
                                supPartsData = JsonConvert.DeserializeObject<List<ClsSupParts>>(callRep[i].supPartsData);
                            }
                            if (callRep[i].unitHandlingData != null && callRep[i].unitHandlingData != "[false]")
                            {
                                unitHandlingData = JsonConvert.DeserializeObject<List<ClsUnitHandling>>(callRep[i].unitHandlingData);
                            }
                        }
                        
                        Encoding enc = Encoding.GetEncoding(1255, new EncoderReplacementFallback(" "), new DecoderReplacementFallback(" "));
                        StreamWriter sw = new StreamWriter(Path.Combine(currDir, "Tec-Phone-Report.txt"), false, enc);
                        sw.WriteLine("Report");

                        for (int i = 0; i < repData.Count; i++)
                        {
                            if (repData[i].unitCode == 0)
                            {
                                sw.WriteLine("CallID~" + repData[i].callCode);
                                sw.WriteLine("ID1~" + repData[i].numExecRep);
                                sw.WriteLine("isJobDone~" + (repData[i].isEnded == -1 || repData[i].isEnded == 0 ? "1" : "2"));
                                if (repData[i].dtStartRide != 0)
                                    sw.WriteLine("Start-Driving~" + UnixTimeStampToDateTime(repData[i].dtStartRide));
                                if (repData[i].dtStartWork != 0)
                                    sw.WriteLine("Start-Working~" + UnixTimeStampToDateTime(repData[i].dtStartWork));
                                if (repData[i].dtEndWork != 0)
                                    sw.WriteLine("Task-End~" + UnixTimeStampToDateTime(repData[i].dtEndWork));
                                if (!String.IsNullOrEmpty(repData[i].recommendation))
                                    sw.WriteLine("Recommendations~" + repData[i].recommendation.Replace("\r\n", "..").Replace("\n", "..").Replace("\r", ".."));
                                else
                                    sw.WriteLine("Recommendations~");
                                if (repData[i].status > 30)
                                    repData[i].status = 3;
                                sw.WriteLine("Status~" + repData[i].status);
                                if (!String.IsNullOrEmpty(repData[i].summary))
                                    sw.WriteLine("Summary~" + repData[i].summary.Replace("\r\n", "..").Replace("\n", "..").Replace("\r", ".."));
                                else
                                    sw.WriteLine("Summary~");
                                if (repData[i].signPicFileName != null && repData[i].signPicFileName.Length > 0)
                                    sw.WriteLine("Signature~" + "sign_" + repData[i].callCode + ".jpg");
                                sw.WriteLine("SignerName~" + repData[i].signerName);
                                sw.WriteLine("Role~" + repData[i].signerRole);
                                sw.WriteLine("Customer-Mail~" + repData[i].signerMail);
                                if (!String.IsNullOrEmpty(repData[i].signComments))
                                    sw.WriteLine("Signature-Comments~" + repData[i].signComments.Replace("\r\n", "..").Replace("\n", "..").Replace("\r", ".."));
                                else
                                    sw.WriteLine("Signature-Comments~");
                                //sw.WriteLine("Audio~");
                                if (repData[i].numImages > 0)
                                {
                                    var imgStr = "Image~";
                                    for (int j = 0; j < repData[i].numImages; j++){
                                        imgStr +=   "pic_" + repData[i].callCode + "_" + j + ".jpg";
                                        if (j < repData[i].numImages - 1)
                                            imgStr += ",";
                                    }
                                    sw.WriteLine(imgStr);
                                }
                            }
                            else
                            {
                                Console.Write(repData[i].unitCode);
                                if (!String.IsNullOrEmpty(repData[i].recommendation))
                                    sw.WriteLine("Recommendations~" + repData[i].recommendation.Replace("\r\n", "..").Replace("\n", "..").Replace("\r", "..") + "~" + repData[i].unitCode);
                                if (!String.IsNullOrEmpty(repData[i].summary))
                                    sw.WriteLine("Summary~" + repData[i].summary.Replace("\r\n", "..").Replace("\n", "..").Replace("\r", "..") + "~" + repData[i].unitCode);
                                if (repData[i].numImages > 0)
                                {
                                    var imgStr = "Image~";
                                    for (int j = 0; j < repData[i].numImages; j++)
                                    {
                                        imgStr +=  "pic_" + repData[i].callCode + "_" + repData[i].unitCode + "_" + j + ".jpg";
                                        if (j < repData[i].numImages - 1)
                                            imgStr += ",";
                                    }
                                    sw.WriteLine(imgStr + "~" + repData[i].unitCode);
                                }    
                            }
                        }            
                        
                        //supplied parts
                        if (supPartsData != null)
                        {
                            foreach (var supPart in supPartsData)
                            {
                                if (Convert.ToInt16(supPart.quantity) > 0)
                                    if (supPart.unitCode == 0)
                                        sw.WriteLine("Part~" + supPart.desc_ + "~" + supPart.code + "~" + supPart.unit + "~" + supPart.quantity);
                                    else
                                        sw.WriteLine("Part~" + supPart.desc_ + "~" + supPart.code + "~" + supPart.unit + "~" + supPart.quantity + "~" + supPart.unitCode);
                            }
                        }
                        //units
                        if (unitsData != null)
                        {
                            foreach (var unit in unitsData)
                                sw.WriteLine("Unit~" + unit.unitCode + "~" + unit.unitDesc + "~" + unit.building + "~" + unit.floor + "~" + unit.room + "~" + unit.manufacturer + "~" + unit.model + "~" + unit.outputTon + "~" + unit.remarks);
                        }
                        //handlings
                        if (unitHandlingData != null)
                        {
                            foreach (var handling in unitHandlingData)
                                sw.WriteLine("Handling~" + handling.unitCode + "~" + handling.handlingCode + "~" + handling.handlingDesc + "~" + handling.isDone + "~" + handling.remarks);
                        }
                       
                        sw.Close();
                        Console.WriteLine("created Tec-Phone-Report.txt");
                    }
                    else
                        Console.WriteLine(strResult);
                }
                else
                {
                    Console.WriteLine(strResult);
                }

            }
            catch (Exception exp)
            {
                StreamWriter swErr = new StreamWriter(Path.Combine(currDir, "Tec-Phone-Log.txt"), false, Encoding.ASCII);
                swErr.WriteLine(exp.Message);
                swErr.Close();
                Console.WriteLine("Error occured , see Tec-Phone-Log.txt for details");
            }
        }
        
    }
}
