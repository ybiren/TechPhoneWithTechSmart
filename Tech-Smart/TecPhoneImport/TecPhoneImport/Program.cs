using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace TecPhoneImport
{
    class Program
    {
        const string VERSION = "4.08";
        const string PINUY_PREFIX = "P_";
        const string SERVER_NAME = "http://magos.co.il";  //"http://ec2-18-217-253-195.us-east-2.compute.amazonaws.com";
        
        
        static void Main(string[] args)
        {
            string currDir = String.Empty;
            try
            {
                if (args.Length > 0)
                {
                    //sign_1478255312.jpg
                    //pic_17110808_0.jpg
                    string fileName = args[0];
                                                            
                    currDir = Directory.GetCurrentDirectory();
                    if (args.Length > 1 && Directory.Exists(args[1]))
                        currDir = args[1];

                    //"http://ec2-18-217-253-195.us-east-2.compute.amazonaws.com/pinuy_img.php?pic=" + fileName : "http://ec2-18-217-253-195.us-east-2.compute.amazonaws.com/tech_img.php?pic=" + fileName;
                    var url = SERVER_NAME + (fileName.StartsWith(PINUY_PREFIX) ? "/pinuy_img.php?pic=" + fileName : "/tech_img.php?pic=" + fileName);
                    
                    
                    byte[] fileDat = WebReq.DoRequest(url,String.Empty);

                    //origImage.Save( (Path.Combine(currDir, fileName));

                    using (System.IO.MemoryStream lxm = new MemoryStream())
                    {

                        lxm.Write(fileDat, 0, fileDat.Length);
                        Bitmap origImage = new Bitmap(lxm);
                        if (fileName.ToLower().Contains("sign"))
                        {
                            //change bpp from 32 to 24 according to Eli Shalit request
                            Bitmap clone = new Bitmap(origImage.Width, origImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                            Graphics g = Graphics.FromImage(clone);
                            g.DrawImage(origImage, new Point(0, 0));
                            clone.Save(Path.Combine(currDir, fileName));
                        }
                        else
                        {
                            FileInfo f = new FileInfo(Path.Combine(currDir, fileName));
                            string pdfPath = f.FullName.Replace(f.Extension, ".pdf");

                            Document document = new Document();
                            using (var stream = new FileStream(pdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                PdfWriter.GetInstance(document, stream);
                                document.Open();
                                using (var imageStream = lxm /*new FileStream("test.jpg", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)*/)
                                {
                                    imageStream.Seek(0, SeekOrigin.Begin);
                                    var image = iTextSharp.text.Image.GetInstance(imageStream);

                                    if (image.Height > image.Width)
                                    {
                                        //Maximum height is 800 pixels.
                                        float percentage = 0.0f;
                                        percentage = 700 / image.Height;
                                        image.ScalePercent(percentage * 100);
                                    }
                                    else
                                    {
                                        //Maximum width is 600 pixels.
                                        float percentage = 0.0f;
                                        percentage = 540 / image.Width;
                                        image.ScalePercent(percentage * 100);
                                    }




                                    document.Add(image);


                                }
                                document.Close();
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("missing file name param");
                }

            }
        
            catch(Exception exp){
                Console.WriteLine("exception occured ,see logError.txt");
                File.WriteAllText(Path.Combine(currDir,"logError.txt"), String.Format("{0} , arg={1}", exp.Message, args[0]));
            }
        }
    }
}
