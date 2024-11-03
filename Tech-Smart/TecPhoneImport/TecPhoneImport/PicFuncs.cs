using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.IO;
using System.Drawing.Drawing2D;

namespace TecPhoneImport
{
    public class PicFuncs
    {
        public static Bitmap CutPic(Bitmap srcImage,int left,int top,int width,int height){

            Bitmap d = new Bitmap(width + 1, height + 1);
            for (int i = left; i <= left + width; i++)
                for (int j = top; j <= top + height; j++)
                    d.SetPixel(i - left, j - top, Color.FromArgb(srcImage.GetPixel(i, j).R, srcImage.GetPixel(i, j).G, srcImage.GetPixel(i, j).B));

            return d;
        }


        public static Color CalcDominantColor(Bitmap srcImage)
        {
            Color dominantClr=Color.FromArgb(0, 0, 0);
            int freq = 0;
                        
            Dictionary<Color, int> dic = new Dictionary<Color, int>();        
            for (int i = 0; i < srcImage.Width; i++)
                for (int j = 0; j < srcImage.Height; j++){
                    Color clr = Color.FromArgb(srcImage.GetPixel(i, j).R, srcImage.GetPixel(i, j).G, srcImage.GetPixel(i, j).B);
                    if (dic.ContainsKey(clr))
                        dic[clr] += 1;
                    else
                        dic.Add(clr, 1);
                }

            
            foreach (Color clr in dic.Keys)
            {
                int hh = 0;
                dic.TryGetValue(clr, out hh);
                if (hh > freq)
                {
                    freq = hh;
                    dominantClr = clr;
                }
            }
            return dominantClr;
        }

        public static List<Color> GetPicColors(Bitmap srcImage)
        {
            List<Color> colorList = new List<Color>();
            for (int i = 0; i < srcImage.Width; i++)
                for (int j = 0; j < srcImage.Height; j++)
                {
                    Color clr = Color.FromArgb(srcImage.GetPixel(i, j).R, srcImage.GetPixel(i, j).G, srcImage.GetPixel(i, j).B);
                    if (!colorList.Contains(clr))
                        colorList.Add(clr);
                }

            return colorList;
        }


        public static void ConvertWhiteToTransparent(Bitmap srcImage,Bitmap trgtImage)
        {
            for (int i = 0; i < srcImage.Width; i++)
                for (int j = 0; j < srcImage.Height; j++)
                {
                    if (srcImage.GetPixel(i, j).R > 200 && srcImage.GetPixel(i, j).G > 200 && srcImage.GetPixel(i, j).B > 200)
                        trgtImage.SetPixel(i, j,Color.FromKnownColor(KnownColor.Control));
                        //trgtImage.SetPixel(i, j,Color.Transparent );
                    else
                        trgtImage.SetPixel(i, j, srcImage.GetPixel(i, j));


                    if (srcImage.GetPixel(i, j).R < 50 && srcImage.GetPixel(i, j).G < 50 && srcImage.GetPixel(i, j).B < 50)
                        trgtImage.SetPixel(i, j, Color.DarkBlue);
                    else
                        trgtImage.SetPixel(i, j, srcImage.GetPixel(i, j));
                
                
                
                }

        }



        public static Dictionary<Color, int> GetColorsHeight(Bitmap srcImage,bool isTop)
        {
            Dictionary<Color, int> colorDic = new Dictionary<Color, int>();
            for (int i = 0; i < srcImage.Width; i++)
                for (int j = 0; j < srcImage.Height; j++)
                {
                    Color clr = Color.FromArgb(srcImage.GetPixel(i, j).R, srcImage.GetPixel(i, j).G, srcImage.GetPixel(i, j).B);
                    if (!colorDic.ContainsKey(clr))
                        colorDic.Add(clr, 1);
                    else
                    {
                        int num = colorDic[clr];
                        colorDic[clr] = num + 1;
                        /*
                        if (isTop)
                        {
                            if (j < colorDic[clr])
                                colorDic[clr] = j;
                        }
                        else
                        {
                            if (j > colorDic[clr])
                                colorDic[clr] = j;
                        
                        }
                        */
                     }
                }

            return colorDic;
        
        
        }

        public static void MakeTextImg(string txtImage,string imageName){

            Image mobileImg = new Bitmap(@"f:\cuts\msgScreen.jpg"); //new Bitmap(202, 16);

            /*
            for (int i = 0; i < mobileImg.Width; i++)
                for (int j = 0; j < mobileImg.Height; j++)

                    ((Bitmap)mobileImg).SetPixel(i, j, Color.White);
            */
            

            Graphics gr = Graphics.FromImage(mobileImg);
            
            Font f = new Font("Arial", 9, FontStyle.Bold);
            Brush b = new SolidBrush(Color.White);
            gr.DrawString(txtImage, f, b, new RectangleF(80, 0, 202, 16));


            mobileImg.Save(@"f:\yossiapps\meshivon\signtemplates\" + imageName + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
        }

        public static void CropImage(string srcFolder,string srcFileName, string newFileName,string trgtFolder)
        {

            Image mobileImg = new Bitmap(Path .Combine(srcFolder,srcFileName));
            /*
            for (int j = mobileImg.Height-1; j>0  ; j--)
            {
                int j1 = 0;
                for (int i = 0; i < mobileImg.Width - 1; i++)
                {
                    Color c = ((Bitmap)mobileImg).GetPixel(i, j);
                    Color c1 = ((Bitmap)mobileImg).GetPixel(i + 1, j);


                    if ((((int)c.R + (int)c.G + (int)c.B) > 383) && (((int)c1.R + (int)c1.G + (int)c1.B) <= 383)
                                                       ||
                         (((int)c.R + (int)c.G + (int)c.B) < 383) && (((int)c1.R + (int)c1.G + (int)c1.B) >= 383)
                        )
                        j1++;
                }    //((Bitmap)mobileImg).SetPixel(i, j, Color.Black);

                if (j1>180)
                MessageBox.Show(j.ToString());
            }
            */

            for (int i = 1; i <= 2; i++)
            {
                Image trgtImg = new Bitmap(1800, 1100);
                Graphics gr = Graphics.FromImage(trgtImg);
                gr.FillRectangle(new SolidBrush(Color.White), 0, 0, trgtImg.Width, trgtImg.Height);
                
                Rectangle destRect = new Rectangle(100, 100, trgtImg.Width-150, trgtImg.Height-150);
                Rectangle srcRect = new Rectangle();
                if (i == 1)
                    srcRect = new Rectangle(650, 400, 1200, 1000);
                else
                    srcRect = new Rectangle(650, 1400, 1200, 1000);

                char[] sep ={ '.' };
                string fileNameWithoutExt = newFileName.Split(sep)[0];

                gr.DrawImage(mobileImg, destRect, srcRect, GraphicsUnit.Pixel);

                //page number
                Font f = new Font("Arial", 30, FontStyle.Regular);
                Brush b = new SolidBrush(Color.Black);
                gr.DrawString(newFileName, f, b, new RectangleF(100, 30, 200, 40));

                
                
                trgtImg.Save(Path.Combine(trgtFolder , fileNameWithoutExt + "_" + i.ToString() + ".jpg"), ImageFormat.Jpeg);
            
            } 
        }


        public static void DrawMultyImage()
        {

            for (int i = 1; i <= 14190; i++)
            {
                Image trgtImg = new Bitmap(900, 600);
                Graphics gr = Graphics.FromImage(trgtImg);
                gr.FillRectangle(new SolidBrush(Color.White), 0, 0, trgtImg.Width, trgtImg.Height);

                Image srcImg1 = new Bitmap(@"C:\UDC Snapshots\ffmpeg\TL" + i.ToString() +  ".jpg");
                Image srcImg2 = new Bitmap(@"C:\UDC Snapshots\ffmpeg\TR" + i.ToString() + ".jpg");
                
                Rectangle destRect = new Rectangle(50, 50, srcImg1.Width, srcImg1.Height);
                Rectangle srcRect = new Rectangle(0, 0, srcImg1.Width, srcImg1.Height);
                gr.DrawImage(srcImg1, destRect, srcRect, GraphicsUnit.Pixel);

                destRect = new Rectangle(50 + srcImg1.Width + 100, 50, srcImg2.Width, srcImg2.Height);
                srcRect = new Rectangle(0, 0, srcImg2.Width, srcImg2.Height);
                gr.DrawImage(srcImg2, destRect, srcRect, GraphicsUnit.Pixel);

                trgtImg.Save(@"C:\UDC Snapshots\ffmpeg\img" + i.ToString() + ".jpg", ImageFormat.Jpeg);
            }        
        }

        public static void DynSizeImg()
        {
            Image srcImg = new Bitmap(@"F:\FAMILY IMAGES\napoli\napoli 017.jpg");

            double factor = 0.25;
            for (int dir = 1; dir <= 2;dir++ )
                for (int i = 1; i <= 38; i++)
                {
                    Image trgtImg = new Bitmap(srcImg.Width, srcImg.Height);
                    Graphics gr = Graphics.FromImage(trgtImg);
                    Rectangle srcRect = new Rectangle(0, 0, srcImg.Width, srcImg.Height);

                    Rectangle rightBottomDestRect = new Rectangle((int)(0.75 * srcImg.Width), (int)(0.75 * srcImg.Height), (int)(0.25 * srcImg.Width), (int)(0.25 * srcImg.Height));
                    gr.DrawImage(srcImg, rightBottomDestRect, srcRect, GraphicsUnit.Pixel);


                    Rectangle destRect = new Rectangle(0, 0, (int)(factor * srcImg.Width), (int)(factor * srcImg.Height));



                    gr.DrawImage(srcImg, destRect, srcRect, GraphicsUnit.Pixel);
                    trgtImg.Save(@"C:\UDC Snapshots\ffmpeg\img" + i.ToString() + ".jpg", ImageFormat.Jpeg);
                    if (i < 20)
                        factor += 0.04;
                    else
                        factor -= 0.04;
                 }
                        
        }

        public static void ResizeImage(Bitmap srcImage,int newWidth,int newHeight,string targetPicName,System.Drawing.Imaging.ImageFormat imgFormat)
        {
            double f = (double)srcImage.Width / (double)srcImage.Height;
            if (newWidth == -1)
                newWidth = (int)(Convert.ToDouble(newHeight) * f);
            if (newHeight == -1)
                newHeight = (int)(Convert.ToDouble(newWidth) / f);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics grPhoto = Graphics.FromImage(newImage);
            grPhoto.DrawImage(srcImage, 0, 0, newWidth, newHeight);
            newImage.Save(targetPicName, imgFormat);
        }

        public static void DrawGradientColor(Bitmap img,int width,int height,Color a,Color b)
        {
            Graphics g = Graphics.FromImage(img);
            Brush linearGradientBrush = new LinearGradientBrush(new Rectangle(0, 0, width, height), a ,b, 45);
            g.FillRectangle(linearGradientBrush, new Rectangle(0, 0, width,height));
        }


    }
}
