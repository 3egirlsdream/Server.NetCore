﻿using Server.NetCore.Models;
using System;
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
        public static Task<string> CreatePic(int width, int height)
        {
            return  Task.Run(() =>
            {
                var hei = height * 0.13;
                if (width < height)
                {
                    hei = height * 0.13 * 0.5;
                }
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

                if (!Directory.Exists(Global.Path_temp))
                {
                    Directory.CreateDirectory(Global.Path_temp);
                }

                var rtlPath = Global.Path_temp + $"{Global.SeparatorChar}{Guid.NewGuid().ToString("N")}.jpg";

                bmp.Save(rtlPath, System.Drawing.Imaging.ImageFormat.Jpeg);//指定图片格式   
                bmp.Dispose();
                return rtlPath;
            });
            
        }


        public static Task AddWaterMarkImg(string sFile, string dFile, string waterFile, string datetime, string deviceName, Bitmap map1, Tuple<int, int> tuple, string mount, string xy)
        {

            return CreateWatermark(sFile, dFile, waterFile, datetime, deviceName, tuple, mount, xy, 1, 0.8, -1, -1).ContinueWith(c =>
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

                if (!Directory.Exists(Global.Path_output))
                {
                    Directory.CreateDirectory(Global.Path_output);
                }

                dFile = $@"{Global.Path_output}{Global.SeparatorChar}{dFile}";
                //保存图片
                _bitmap.Save(dFile, System.Drawing.Imaging.ImageFormat.Jpeg);

                _g.Dispose();
                _bitmap.Dispose();
                sourceImage.Dispose();
                waterImage.Dispose();
                map1.Dispose();
            });

        }


        public static Task<string> CreateWatermark(string emptyWatermark, string dFile, string logoFile, string datetime, string deviceName, Tuple<int, int> tuple, string mount, string xy, double scale = 1, double opacity = 0.8, int locationX = -1, int locationY = -1)
        {
            return Task.Run(() =>
            {
                Node Producer, Date, Params, XY;

                Bitmap emptyWmMap = new Bitmap(emptyWatermark);
                Bitmap logoMap = new Bitmap(logoFile);

                Bitmap bitmap = new Bitmap(emptyWmMap, emptyWmMap.Width, emptyWmMap.Height);
                Graphics g = Graphics.FromImage(bitmap);
                //logo比例系数
                double xs = (double)(emptyWmMap.Height / 2) / logoMap.Height;
                //字体比例系数
                float fontxs = ((float)emptyWmMap.Height / 156);
                if (fontxs < 1) fontxs = 1;
                if (tuple.Item1 < tuple.Item2)
                {
                    fontxs *= 0.8f;
                    xs *= 0.8;
                }

                //下面定义一个矩形区域      
                float waterWidth = (float)(logoMap.Width * xs);
                float waterHeight = (float)(logoMap.Height * xs);

                //绘制右侧镜头参数
                float fontSize = 25 * fontxs;
                var font = new Font(Global.FontFamily, (int)fontSize, FontStyle.Bold);
                var size = GetFontSize(g, mount + '\0', fontSize);
                var oneSize = GetFontSize(g, "F", fontSize);
                var padding_right = GetFontSize(g, "23mm", fontSize);
                var brush = new SolidBrush(Color.Black);
                Params = new Node(emptyWmMap.Width - (int)size.Width - (int)padding_right.Width, (int)(0.3 * emptyWmMap.Height));
                var point = new Point(Params.X, Params.Y);
                g.DrawString(mount, font, brush, point);

                //绘制右侧坐标（镜头型号）
                var font20 = (20 * fontxs);
                XY = new Node(Params.X, (int)(1.04 * size.Height +  Params.Y));
                var font20Size = GetFontSize(g, "F", font20);
                font = new Font(Global.FontFamilyLight, (int)font20, FontStyle.Regular);
                var c = ColorTranslator.FromHtml("#919191");
                brush = new SolidBrush(c);
                point = new Point(XY.X, XY.Y);
                g.DrawString(xy, font, brush, point);

                //绘制分界划线
                var lStart = new Point(Params.X - (int)(oneSize.Width * 0.6), Params.Y);
                var lEnd = new Point(lStart.X, XY.Y + (int)font20Size.Height);
                g.DrawLine(new Pen(Color.LightGray, (int)(2 * fontxs)), lEnd, lStart);


                if (locationX == -1) locationX = (int)(((double)2 / 3) * emptyWmMap.Width - waterWidth) + 20;
                if (locationY == -1) locationY = (int)(0.5 * emptyWmMap.Height - 0.5 * waterHeight);
                //声明矩形域 画logo
                RectangleF textArea = new RectangleF(lStart.X - (int)(oneSize.Width * 0.6) - waterWidth, lStart.Y, waterWidth, waterHeight);
                Bitmap w_bitmap = ChangeOpacity(logoMap, scale, opacity);
                g.DrawImage(w_bitmap, textArea);


                //绘制设备信息
                var font28 = (28 * fontxs);
                font = new Font(Global.FontFamily, (int)font28, FontStyle.Bold);
                brush = new SolidBrush(Color.Black);
                Producer = new Node((int)(100 * xs), Params.Y);
                point = new Point(Producer.X, Producer.Y);
                g.DrawString(deviceName, font, brush, point);

                //画时间
                font = new Font(Global.FontFamilyLight, (int)font20, FontStyle.Regular);
                c = ColorTranslator.FromHtml("#919191");
                brush = new SolidBrush(c);
                Date = new Node((int)(100 * xs), XY.Y);
                point = new Point(Date.X, Date.Y);
                g.DrawString(datetime, font, brush, point);


                if (!Directory.Exists(Global.Path_temp))
                {
                    Directory.CreateDirectory(Global.Path_temp);
                }
                var resultPath = Global.Path_temp + $"{Global.SeparatorChar}temp_" + dFile;
                bitmap.Save(resultPath, ImageFormat.Jpeg);

                logoMap.Dispose();
                w_bitmap.Dispose();
                bitmap.Dispose();
                g.Dispose();
                emptyWmMap.Dispose();
                return resultPath;
            });
        }

        private static SizeF GetFontSize(System.Drawing.Graphics g, string str, float _size)
        {
            System.Drawing.Font font = new System.Drawing.Font(new System.Drawing.FontFamily(Global.FontFamily), _size);
            System.Drawing.SizeF size = g.MeasureString(str, font);
            return size;
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
            g.Dispose();
            waterBitMap.Dispose();
            return resultImage;
        }
    }

    public struct Node
    {
        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
