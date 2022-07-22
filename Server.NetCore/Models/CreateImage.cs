﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JointWatermark
{
    internal class CreateImage
    {
        public static Task<string> CreatePic(int width, int height, string binpath)
        {
            return  Task.Run(() =>
            {
                var hei = height * 0.13;
                Bitmap bmp = new Bitmap(width, (int)hei);                      //改图只显示最近输入的700个点的数据曲线。
                                                                               //  Graphics graphics = Graphics.FromImage(bmp);
                                                                               //   SolidBrush brush1 = new SolidBrush(Color.FromArgb(255, 0, 0));
                                                                               //   graphics.FillRectangle(brush1, 0, 0, 700, 550);//Brushes.Sienna
                for (int i = 0; i < bmp.Width; i++)
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color c = Color.FromArgb(255, 255, 255);
                        bmp.SetPixel(i, j, c);
                    }

                var sPath = binpath + "temp";
                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }

                var rtlPath = sPath + $"{Path.DirectorySeparatorChar}{Guid.NewGuid().ToString("N")}.jpg";

                bmp.Save(rtlPath, System.Drawing.Imaging.ImageFormat.Jpeg);//指定图片格式   
                bmp.Dispose();
                return rtlPath;
            });
            
        }


        public static Task AddWaterMarkImg(string sFile, string dFile, string waterFile, string datetime, string deviceName, Bitmap map1, Tuple<int, int> tuple, bool preview, string mount, string xy, string binpath, double scale = 1, double opacity = 0.8, int locationX = -1, int locationY = -1)
        {

            return CreateWatermark(sFile, dFile, waterFile, datetime, deviceName, map1, tuple, preview, mount, xy, binpath, 1, 0.8, -1, -1).ContinueWith(c =>
            {
                Bitmap sourceImage = new Bitmap(sFile);
                Bitmap waterImage = new Bitmap(c.Result);

                double xs = (double)(sourceImage.Height / 2) / waterImage.Height;
                float waterWidth = (float)(waterImage.Width * xs);


               //拼接图片
                var w = tuple.Item1;
                var h = tuple.Item2 + sourceImage.Height;
                RectangleF area = new RectangleF(0, 0, waterWidth, h);
                var _bitmap = new Bitmap(w, h, PixelFormat.Format24bppRgb);
                _bitmap.SetResolution(96.0F, 96.0F); // 重点
                                                     //_bitmap.SetResolution(300, 300);
                Graphics _g = Graphics.FromImage(_bitmap);
                _g.FillRectangle(Brushes.White, new Rectangle(0, 0, w, h));
                _g.DrawImage(map1, 0, 0, w, map1.Height);
                map1.SetResolution(72, 72);
                _g.DrawImage(waterImage, 0, map1.Height, w, waterImage.Height);

                var sPath = binpath + "output";
                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }

                dFile = $@"{sPath}{Path.DirectorySeparatorChar}{dFile}";
               //保存图片
                _bitmap.Save(dFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            });

        }


        public static Task<string> CreateWatermark(string sFile, string dFile, string waterFile, string datetime, string deviceName, Bitmap map1, Tuple<int, int> tuple, bool preview, string mount, string xy, string binpath, double scale = 1, double opacity = 0.8, int locationX = -1, int locationY = -1)
        {
            return Task.Run(() =>
            {
                Bitmap sourceImage = new Bitmap(sFile);
                Bitmap waterImage = new Bitmap(waterFile);

                Bitmap bitmap = new Bitmap(sourceImage, sourceImage.Width, sourceImage.Height);
                Graphics g = Graphics.FromImage(bitmap);
                double xs = (double)(sourceImage.Height / 2) / waterImage.Height;
                float waterWidth = (float)(waterImage.Width * xs);
                float waterHeight = (float)(waterImage.Height * xs);

                //下面定义一个矩形区域      
                float rectWidth = waterWidth;
                float rectHeight = waterHeight;

                if (locationX == -1) locationX = (int)(((double)2 / 3) * sourceImage.Width - rectWidth) + 20;
                if (locationY == -1) locationY = (int)(0.5 * sourceImage.Height - 0.5 * rectHeight);
                //声明矩形域 画logo
                RectangleF textArea = new RectangleF(locationX - 30, locationY + 5, rectWidth, rectHeight);
                Bitmap w_bitmap = ChangeOpacity(waterImage, scale, opacity);
                g.DrawImage(w_bitmap, textArea);

                //字体比例
                double fontxs = ((double)sourceImage.Height / 156);
                if (fontxs < 1) fontxs = 1;

                //划线
                g.DrawLine(new Pen(Color.LightGray, (int)(2 * fontxs)), new Point(locationX + (int)rectWidth + 10, (int)(0.8 * sourceImage.Height)), new Point(locationX + (int)rectWidth + 10, (int)(0.3 * sourceImage.Height)));

                //写字
                var font = new Font("微软雅黑", (int)(25 * fontxs), FontStyle.Bold);
                var brush = new SolidBrush(Color.Black);
                var point = new Point(locationX + (int)rectWidth + 50, (int)(0.3 * sourceImage.Height));
                g.DrawString(mount, font, brush, point);


                font = new Font("微软等线Light", (int)(20 * fontxs), FontStyle.Regular);
                var c = ColorTranslator.FromHtml("#919191");
                brush = new SolidBrush(c);
                point = new Point(locationX + (int)rectWidth + 50, (int)(0.6 * sourceImage.Height));
                g.DrawString(xy, font, brush, point);

                //画时间
                font = new Font("微软等线Light", (int)(20 * fontxs), FontStyle.Regular);
                c = ColorTranslator.FromHtml("#919191");
                brush = new SolidBrush(c);
                point = new Point(100, (int)(0.6 * sourceImage.Height));
                g.DrawString(datetime, font, brush, point);

                //画设备
                font = new Font("微软雅黑", (int)(28 * fontxs), FontStyle.Bold);
                brush = new SolidBrush(Color.Black);
                point = new Point(100, (int)(0.25 * sourceImage.Height));
                g.DrawString(deviceName, font, brush, point);

                var sPath = binpath + "temp";
                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }
                var resultPath = sPath + Path.DirectorySeparatorChar + dFile;
                bitmap.Save(resultPath, ImageFormat.Jpeg);

                waterImage.Dispose();
                w_bitmap.Dispose();
                bitmap.Dispose();
                g.Dispose();
                sourceImage.Dispose();
                return resultPath;
            });
        }

        /// <summary>
        /// 改变图片的透明度
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="opacityvalue">透明度</param>
        /// <returns></returns>
        private static Bitmap ChangeOpacity(Image waterImg, double scale, double opacityvalue)
        {

            float[][] nArray ={ new float[] {1, 0, 0, 0, 0},

                                new float[] {0, 1, 0, 0, 0},

                                new float[] {0, 0, 1, 0, 0},

                                new float[] {0, 0, 0, (float)opacityvalue, 0},

                                new float[] {0, 0, 0, 0, 1}};

            ColorMatrix matrix = new ColorMatrix(nArray);

            ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            var waterWidth = (int)(waterImg.Width * scale);
            var waterHeight = (int)(waterImg.Height * scale);

            //缩放水印图片
            Bitmap waterBitMap = new Bitmap(waterWidth, waterHeight);
            Graphics waterG = Graphics.FromImage(waterBitMap);
            waterG.DrawImage(waterImg, 0, 0, waterWidth, waterHeight);


            Bitmap resultImage = new Bitmap(waterWidth, waterHeight);
            Graphics g = Graphics.FromImage(resultImage);
            g.DrawImage(waterBitMap, new Rectangle(0, 0, waterWidth, waterHeight), 0, 0, waterWidth, waterHeight, GraphicsUnit.Pixel, attributes);

            return resultImage;
        }
    }
}
