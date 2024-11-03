using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using Newtonsoft.Json;


namespace Tech_Smart
{
    public class ClsTechPhone
    {

        const string CALL_TYPE = "1";
        const string PART_TYPE = "2";
        const string EXEC_REP_TYPE = "3";
        const string UNIT_TYPE = "4";

        string currDir; string op; string code; string charsType; string user; string callType; string callDateTime; string isContinue; string inService; string custName; string siteName; string projName; string address; string id; string location; string recieverServiceName; string callDesc; string recordType; string execRepFileName; List<ClsInvParts> lstInvParts; List<ClsParts> lstParts; List<ClsUnit> lstUnits; List<ClsUnitHandling> lstUnitHandling; string callStatus; List<ClsUnitType> lstUnitType;

        public ClsTechPhone(string currDir, string op, string code, string charsType, string user, string callType, string callDateTime, string isContinue, string inService, string custName, string siteName, string projName, string address, string id, string location, string recieverServiceName, string callDesc, string recordType, string execRepFileName, List<ClsInvParts> lstInvParts, List<ClsParts> lstParts, List<ClsUnit> lstUnits, List<ClsUnitHandling> lstUnitHandling, string callStatus, List<ClsUnitType> lstUnitType)
        {
            this.currDir = currDir; this.op = op; this.code = code; this.charsType = charsType; this.user = user; this.callType = callType; this.callDateTime = callDateTime; this.isContinue = isContinue; this.inService = inService; this.custName = custName; this.siteName = siteName; this.projName = projName; this.address = address; this.id = id; this.location = location; this.recieverServiceName = recieverServiceName; this.callDesc = callDesc; this.recordType = recordType; this.execRepFileName = execRepFileName; this.lstInvParts = lstInvParts; this.lstParts = lstParts; this.lstUnits = lstUnits; this.lstUnitHandling = lstUnitHandling; this.callStatus = callStatus; this.lstUnitType = lstUnitType;
        }


        public void Start()
        {
          if (!String.IsNullOrEmpty(callDateTime))
            callDateTime = GlobalFuncs.DateStrToUnixTimeStamp(callDateTime).ToString();

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
            var strResult2 = ClsUploadFile.UploadFileEx(uploadfile, GlobalFuncs.GetServerIP() + "/tech_post.php", String.Empty, String.Empty, querystring, cookies);
            Console.WriteLine(strResult2);
            return;
          }

          if (recordType == UNIT_TYPE)
          {
            var strResult2 = WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", "op=delUnitTypeTable");
            Console.WriteLine(strResult2);
            foreach (var clsUnitType in lstUnitType) {
                string qryStr2 = String.Format("op={0}&code={1}&txt={2}", "insUnitType", clsUnitType.code, clsUnitType.txt);
                qryStr2 = qryStr2.Replace("'", "''");
                Console.WriteLine(qryStr2);
                strResult2 = WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", qryStr2);
                Console.WriteLine(strResult2);
            }
            return;
          }

           string qryStr = String.Format("code={0}&callCode={0}&charsType={1}&callType={2}&callDateTime={3}&isContinue={4}&inService={5}&custName={6}&siteName={7}&projName={8}&address={9}&id={10}&location={11}&recieverServiceName={12}&callDesc={13}&callStatus={14}&userCode={15}", code, charsType, callType, callDateTime, isContinue,inService, custName, siteName, projName, address, id, location, recieverServiceName, callDesc, callStatus, user);
           qryStr = "op=" + op + "&" + qryStr;
           qryStr = qryStr.Replace("'", "''");
           var strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", qryStr);

           if (op == "insCall") //insert invited parts
           {

               // set isTechSmartInsCallCompleted=0  - an indicator that call insert\update is being updated  and can not be viewed by users
               qryStr = String.Format("op={0}&code={1}&isTechSmartInsCallCompleted=0", "updCallCompleted", code);
               Console.WriteLine(qryStr);
               strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", qryStr);
               Console.WriteLine(strResult);
               
               ////Console.WriteLine("sleeping...");
               /////System.Threading.Thread.Sleep(120000);
             qryStr = String.Format("op={0}&callCode={1}", "delInvParts", code);
             strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", qryStr);
                                        
             foreach(var clsInvParts  in lstInvParts){
               qryStr = String.Format("op={0}&code={1}&callCode={2}&partDesc={3}&partUnit={4}&partQuantity={5}", "insInvPart", clsInvParts.partCode, clsInvParts.callCode, clsInvParts.partDesc, clsInvParts.partUnit, clsInvParts.partQuantity);
               qryStr = qryStr.Replace("'", "''");
               Console.WriteLine(qryStr);
               strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", qryStr);
               Console.WriteLine(strResult);
             }  
                
             //units
             foreach (var clsUnit in lstUnits)
             {
               qryStr = String.Format("op={0}&unitCode={1}&callCode={2}&unitDesc={3}&building={4}&floor={5}&room={6}&manufacturer={7}&model={8}&outputTon={9}&remarks={10}&creationYear={11}&serialNum={12}&unitType={13}", "insCallUnit", clsUnit.unitCode, code, clsUnit.unitDesc, clsUnit.building, clsUnit.floor, clsUnit.room, clsUnit.manufacturer, clsUnit.model, clsUnit.outputTon, clsUnit.remarks, clsUnit.creationYear,clsUnit.serialNum,clsUnit.unitType);
               qryStr = qryStr.Replace("'", "''");
               Console.WriteLine(qryStr);
               strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", qryStr);
               Console.WriteLine(strResult);
             }

             //unit handling
             foreach (var clsUnitHandling in lstUnitHandling)
             {
               qryStr = String.Format("op={0}&unitCode={1}&handlingCode={2}&handlingDesc={3}&isDone={4}&remarks={5}&callCode={6}", "insCallUnitHandling", clsUnitHandling.unitCode, clsUnitHandling.handlingCode, clsUnitHandling.handlingDesc, clsUnitHandling.isDone, clsUnitHandling.remarks, code);
               qryStr = qryStr.Replace("'", "''");
               Console.WriteLine(qryStr);
               strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", qryStr);
               Console.WriteLine(strResult);
             }

             // set isTechSmartInsCallCompleted=1 - an indicator that call insert\update has been completed and can be viewed by users
             qryStr = String.Format("op={0}&code={1}&isTechSmartInsCallCompleted=1", "updCallCompleted", code);
             Console.WriteLine(qryStr);
             strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", qryStr);
             Console.WriteLine(strResult);  
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
                 if (callRep[i] == null)
                 {
                     continue;
                 }
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
                 if (repData[i] == null)
                 {
                     continue; ;
                 }
                 if (repData[i].unitCode == 0)
                 {
                   sw.WriteLine("CallID~" + repData[i].callCode);
                   sw.WriteLine("ID1~" + repData[i].numExecRep);
                   sw.WriteLine("isJobDone~" + (repData[i].isEnded == -1 || repData[i].isEnded == 0 ? "1" : "2"));
                   if (repData[i].dtStartRide != 0)
                     sw.WriteLine("Start-Driving~" + GlobalFuncs.UnixTimeStampToDateTime(repData[i].dtStartRide));
                   if (repData[i].dtStartWork != 0)
                     sw.WriteLine("Start-Working~" + GlobalFuncs.UnixTimeStampToDateTime(repData[i].dtStartWork));
                   if (repData[i].dtEndWork != 0)
                     sw.WriteLine("Task-End~" + GlobalFuncs.UnixTimeStampToDateTime(repData[i].dtEndWork));
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
                    if (supPart == null)
                    {
                        continue;
                    }
                    if (Convert.ToDouble(supPart.quantity) > 0)
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
                  {
                      if (unit == null)
                      {
                          continue;
                      }
                      sw.WriteLine("Unit~" + unit.unitCode + "~" + unit.unitDesc + "~" + unit.building + "~" + unit.floor + "~" + unit.room + "~" + unit.manufacturer + "~" + unit.model + "~" + unit.outputTon + "~" + unit.remarks + "~" + unit.creationYear + "~" + unit.serialNum + "~" + unit.unitType);
                  }
              }
              //handlings
              if (unitHandlingData != null)
              {
                  foreach (var handling in unitHandlingData)
                  {
                      if (handling == null)
                      {
                          continue;
                      }
                      if (handling.isDone == 1)
                          sw.WriteLine("Handling~" + handling.unitCode + "~" + handling.handlingCode + "~" + handling.handlingDesc + "~" + handling.isDone + "~" + handling.remarks);
                  }
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

     }
    
    }

