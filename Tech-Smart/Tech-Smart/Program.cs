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

        const string VERSION = "4.09";
        const string CALL_TYPE = "1";
        const string PART_TYPE = "2";
        
        static string applicationType = "1";
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
        static string op = "insCall";
        static string callStatus = "2";
        static string recordType = CALL_TYPE;
        static string currDir = String.Empty;
        static List<ClsInvParts> lstInvParts = new List<ClsInvParts>();
        static List<ClsParts> lstParts = new List<ClsParts>();
        static List<ClsUnit> lstUnits = new List<ClsUnit>();
        static List<ClsUnitHandling> lstUnitHandling = new List<ClsUnitHandling>();
        static List<ClsUnitType> lstUnitType = new List<ClsUnitType>();
        
        static string execRepFileName = String.Empty;

        static string contactName = String.Empty;
        static string message = String.Empty;
        static string quantity = String.Empty;
        static string unloadingSite = String.Empty;
        static string car = String.Empty;
        static string driver = String.Empty;
        

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
                typeCode = val;
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
            if (arrKeyVal.Length > 10)
                clsUnit.creationYear = Convert.ToInt32(arrKeyVal[10].Trim());
            if (arrKeyVal.Length > 11)
                clsUnit.serialNum = System.Web.HttpUtility.UrlEncode(arrKeyVal[11].Trim());
            if (arrKeyVal.Length > 12)
                clsUnit.unitType = Convert.ToInt32(arrKeyVal[12].Trim());
            
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
        private static void AddUnitType(string[] arrKeyVal)
        {
            ClsUnitType clsUnitType = new ClsUnitType();
            clsUnitType.code = Convert.ToInt32(arrKeyVal[1].Trim());
            clsUnitType.txt =  System.Web.HttpUtility.UrlEncode(arrKeyVal[2].Trim());
            lstUnitType.Add(clsUnitType);
        }
    
        /************************************************************************/
        private static void GetCarAndDriver(string[] arrKeyVal)
        {
            car = arrKeyVal[1];
            driver = System.Web.HttpUtility.UrlEncode(arrKeyVal[2]);
        }


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
                    case "Tabel":
                    AddUnitType(arrKeyVal);
                    break;
                    case "Contact Name":
                    if (arrKeyVal.Length == 4)
                      contactName = System.Web.HttpUtility.UrlEncode(arrKeyVal[1].Trim()) + "~" + System.Web.HttpUtility.UrlEncode(arrKeyVal[2].Trim()) + "~" + System.Web.HttpUtility.UrlEncode(arrKeyVal[3].Trim());
                    break;
                    case "Message":
                    message = val;
                    break;
                    case "Quantity":
                    quantity = val;
                    break;
                    case "Unloading site":
                    unloadingSite = val;
                    break;
                    case "Call-Status":
                    callStatus = val;
                    break;
                    case "Car & Driver":
                    GetCarAndDriver(arrKeyVal);
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

        /************************************************************************/
        private static bool IsUserExist(ref int perm){
            
            string qryStr = ("op=login&userName=" + user + "&pass=" + pass);
            qryStr = qryStr.Replace("'", "''");
            Console.Write(qryStr);
            var strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", qryStr);
            if (strResult.Contains("perm"))
            {
                var res = JsonConvert.DeserializeObject<List<Permission>>(strResult);
                perm = res[0].perm;
                return true;
            }
            else
            {
                return false;
            }   
        }		
        
        
        /************************************************************************/
        static void Main(string[] args)
        {
            try
            {
                if (args.Length>0 && args[0].ToUpper() == "-V")
                {
                    Console.Write(VERSION);
                    return;
                }
                
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
                    if (line.Trim().ToUpper() == "UPDATE")
                        op = "updCallStatus";
                    if (line.Trim().ToUpper() == "REPORT")
                        op = "getCallRep";

                    SetValues(line);
                }
                sr.Close();

                int perm = -1;
                if (!IsUserExist(ref perm))
                {
                    throw new Exception("user does not exists");
                }

                switch (Convert.ToInt16(applicationType))
                {
                    case (int)EAppType.Tech:
                        var techPhone = new ClsTechPhone(currDir, op, code,charsType, user, callType, callDateTime, isContinue, inService, custName, siteName, projName, address, id, location, recieverServiceName, callDesc, recordType, execRepFileName, lstInvParts, lstParts, lstUnits,lstUnitHandling,callStatus,lstUnitType);
                        techPhone.Start();
                        break;
                    case (int)EAppType.Pinuyeem:
                        string serverFileName = "/drivers_post.php";
                        if (perm > 2)
                        {
                            serverFileName = "/drivers" + (perm - 1).ToString() + "_post.php";  //perm=3 db = dbdrivers2 , perm=4 db = dbdrivers3  and so on...   
                        }

                        var pinuyeem = new ClsPinuyeem(serverFileName, currDir, op, code, charsType, user, callDateTime, custName, siteName, address, contactName, message, quantity, unloadingSite, car, driver, callType, callDesc);
                        pinuyeem.Start();
                        break;
                 
                }
                // finally , delete Tec-Phone-Get.txt
                File.Delete(Path.Combine(currDir, "Tec-Phone-Get.txt"));
            }
            catch (Exception exp)
            {
                StreamWriter swErr = new StreamWriter(Path.Combine(currDir, "Tec-Phone-Log.txt"), false, Encoding.UTF8);
                swErr.WriteLine(exp.Message);
                swErr.Close();
                Console.WriteLine("Error occured , see Tec-Phone-Log.txt for details");
            }
        }
        
    }

    public class Permission
    {
        public int perm { get; set; }
    }
}
