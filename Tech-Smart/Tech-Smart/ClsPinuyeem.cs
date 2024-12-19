using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;


namespace Tech_Smart
{


    public class ClsPinuyeem
    {

        const string PINUYEEM_PREFIX = "P_";

        string currDir; string op; string code; string charsType; string user; string callDateTime; string custName; string siteName; string address; string contactName; string message; string quantity; string unloadingSite; string car; string driver; string invType; string desc;
        string serverFileName;

        public ClsPinuyeem(string serverFileName, string currDir, string op, string code, string charsType, string user, string callDateTime, string custName, string siteName, string address, string contactName, string message, string quantity, string unloadingSite, string car, string driver, string invType, string desc)
        {
            this.currDir = currDir; this.op = op; this.code = code; this.charsType = charsType; this.user = user; this.callDateTime = callDateTime; this.custName = custName; this.siteName = siteName; this.address = address; this.contactName = contactName; this.message = message; this.quantity = quantity; this.unloadingSite = unloadingSite; this.car = car; this.driver = driver; this.invType = invType; this.desc = desc;
            this.serverFileName = serverFileName;
        }

        public void Start()
        {
            if (!String.IsNullOrEmpty(callDateTime))
                callDateTime = GlobalFuncs.DateStrToUnixTimeStamp(callDateTime).ToString();

            char[] sep = { '~' };
            
            string[] contactArr = !String.IsNullOrEmpty(contactName) ? contactName.Split(sep) : new string[3];
            

            if (op == "insCall")
            {
                var qryStr = String.Format("op={0}&invNum={1}&userCode={2}&dt={3}&custName={4}&siteName={5}&address={6}&contactName={7}&contactPhone={8}&contactMobile={9}&message={10}&quantity={11}&dischargeArea={12}&carNum={13}&driverName={14}&invType={15}&invDesc={16}", "insInvitation", code, user, callDateTime, custName, siteName, address, contactArr[0], contactArr[1], contactArr[2], message, quantity, unloadingSite, car, driver, invType, desc);
                qryStr = qryStr.Replace("'", "''");
                var strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + serverFileName, qryStr);

                System.IO.File.WriteAllText("XXX.txt", strResult);
            }

            if (op == "delCall")
            {
                var qryStr = String.Format("op={0}&invNum={1}", "delInv", code);
                var strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + serverFileName, qryStr);
            }
            
            if (op == "getCallRep")
            {



                var qryStr = String.Format("op={0}&invNum={1}", "invByNum", code);
                var strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + serverFileName, qryStr);
                strResult = strResult.Replace(",false", String.Empty);
                List<ClsInv> invRep = JsonConvert.DeserializeObject<List<ClsInv>>(strResult);
                Encoding enc = Encoding.GetEncoding(1255, new EncoderReplacementFallback(" "), new DecoderReplacementFallback(" "));
                StreamWriter sw = new StreamWriter(Path.Combine(currDir, "Tec-Phone-Report.txt"), false, enc);
                sw.WriteLine("Report");

                if (invRep.Count > 0)
                {


                    sw.WriteLine("Chr Set~Oem/Ansii");
                    sw.WriteLine("Application Type~3");
                    sw.WriteLine("ID~" + code);
                    sw.WriteLine("Date~" + GlobalFuncs.UnixTimeStampToDateTime(invRep[0].dt));


                    //sw.WriteLine("siteName~" + invRep[0].siteName);
                    //sw.WriteLine("address~" + invRep[0].address);
                    //sw.WriteLine("custName~" + invRep[0].custName);
                    //sw.WriteLine("dischargeArea~" + invRep[0].dischargeArea);
                    //sw.WriteLine("contactName~" + invRep[0].contactName);
                    //sw.WriteLine("contactPhone~" + invRep[0].contactPhone);
                    //sw.WriteLine("contactMobile~" + invRep[0].contactMobile);

                    //sw.WriteLine("remarks~" + invRep[0].remarks);
                    sw.WriteLine("signerName~" + invRep[0].signerName);
                    sw.WriteLine("Role~" + invRep[0].signerRole);
                    sw.WriteLine("Customer-Mail~" + invRep[0].signEmail);
                    sw.WriteLine("Signature-Comments~" + invRep[0].signRemarks);
                    if (invRep[0].signPic != null && invRep[0].signPic.Length > 0)
                        sw.WriteLine("Signature~" + PINUYEEM_PREFIX + "sign_" + code + ".jpg");
                    sw.WriteLine("ID1~" + invRep[0].numExecRep);

                    //sw.WriteLine("weigeCertNum~" + invRep[0].weigeCertNum);
                    //sw.WriteLine("weigeRemarks~" + invRep[0].weigeRemarks);
                    //sw.WriteLine("weight~" + invRep[0].weight);

                    var picStr = "";
                    if (invRep[0].numImages_erPic > 0)
                    {
                        for (var i = 0; i < invRep[0].numImages_erPic; i++)
                        {
                            picStr += PINUYEEM_PREFIX + code + "_erPic_" + i.ToString() + ".jpg" + ",";
                        }
                        //sw.WriteLine(picStr);
                    }
                    if (invRep[0].numImages_erSnap > 0)
                    {
                        for (var i = 0; i < invRep[0].numImages_erSnap; i++)
                        {
                            picStr += PINUYEEM_PREFIX + code + "_erSnap_" + i.ToString() + ".jpg" + ",";
                        }
                        //sw.WriteLine(picStr);
                    }

                    if (invRep[0].numImages_wgPic > 0)
                    {
                        for (var i = 0; i < invRep[0].numImages_wgPic; i++)
                        {
                            picStr += PINUYEEM_PREFIX + code + "_wgPic_" + i.ToString() + ".jpg" + ",";
                        }
                        //sw.WriteLine(picStr);
                    }

                    if (invRep[0].numImages_wgSnap > 0)
                    {
                        for (var i = 0; i < invRep[0].numImages_wgSnap; i++)
                        {
                            picStr += PINUYEEM_PREFIX + code + "_wgSnap_" + i.ToString() + ".jpg" + ",";
                        }
                        //sw.WriteLine(picStr);
                    }

                    if (picStr.Length > 0)
                        sw.WriteLine("Image~" + picStr);

                    sw.WriteLine("Quantity~" + invRep[0].quantity + "~" + invRep[0].remarks);
                    sw.WriteLine("Unloading site~" + invRep[0].dischargeArea + "~" +  invRep[0].weigeCertNum + "~" + invRep[0].weight + "~" + invRep[0].weigeRemarks);

                    sw.Close();
                }
            }
        }
    }
 }
