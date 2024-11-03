using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Tech_Smart
{
    public class ClsPartsManager
    {
        const string OP_INS_PART = "1";
        const string OP_UPD_PART = "2";
        const string OP_DEL_PART = "3";

        public ClsPartsManager(string op,string currDir,List<ClsParts> lstParts)
        {
            try
            {
                if (op == "insCall")
                {
                    foreach (var part in lstParts)
                    {
                        string op_ = "insPart";
                        if (part.op == OP_UPD_PART)
                            op_ = "updPart";
                        if (part.op == OP_DEL_PART)
                            op_ = "delPart";

                        var qryStr = "op=" + op_ + "&code=" + part.code + "&desc=" + part.desc_ + "&unit=" + part.unit + "&barcode=" + part.barcode + "&source=1" + "&status=" + part.status;
                        qryStr = qryStr.Replace("'", "''");
                        var strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", qryStr);
                    }
                }
                else
                {
                    Encoding enc = Encoding.GetEncoding(1255, new EncoderReplacementFallback(" "), new DecoderReplacementFallback(" "));
                    StreamWriter sw = new StreamWriter(Path.Combine(currDir, "Tec-Phone-Report.txt"), false, enc);

                    var qryStr = "op=getAllNewParts";
                    var strResult = WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", qryStr);
                    strResult = strResult.Replace(",false", String.Empty);
                    strResult = strResult.Replace("[false]", String.Empty);
                    sw.WriteLine("Report");
                    if (!String.IsNullOrEmpty(strResult))
                    {
                        List<ClsParts> lParts = JsonConvert.DeserializeObject<List<ClsParts>>(strResult);
                        foreach (var part in lParts)
                        {
                            sw.WriteLine("Part~" + part.code + "~" + part.desc_ + "~" + part.unit + "~" + part.barcode + "~" + part.source + "~" + part.status);
                        }
                    }
                    sw.Close();
                    Console.WriteLine("created Tec-Phone-Report.txt");
                    //mark isNew=0 in order not to display records again
                    WebReq.DoRequest(GlobalFuncs.GetServerIP() + "/tech_post.php", "op=makePartsDirty");

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
