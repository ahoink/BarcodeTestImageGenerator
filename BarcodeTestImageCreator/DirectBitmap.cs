using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarcodeTestImageCreator
{
    /// <summary>
    /// Allows direct access to a Bitmap's pixels as bytes without the need for unsafe code or constantly (un)locking bits.
    /// Includes various image processing functions.
    /// </summary>
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public byte[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int Stride { get; private set; }
        public int Bpp { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        /// <summary>
        /// Initializes a new instance of the DirectBitmap class with a specified size and pixel format
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="bpp">Bits Per Pixel of the new DirectBitmap instance. Defaults to 32-bit</param>
        public DirectBitmap(int width, int height, int bpp = 32)
        {
            int bytesPerPx = bpp / 8;
            int stride = 4 * ((width * bytesPerPx + 3) / 4);

            PixelFormat pxFormat = PixelFormat.Undefined;
            if (bpp == 8) pxFormat = PixelFormat.Format8bppIndexed;
            else if (bpp == 32) pxFormat = PixelFormat.Format32bppArgb;

            Bpp = bpp;
            Stride = stride;
            Width = width;
            Height = height;
            Bits = new byte[stride * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, stride, pxFormat, BitsHandle.AddrOfPinnedObject());

            if (bpp == 8)
            {
                ColorPalette greyscale = Bitmap.Palette;
                for (int i = 0; i < 256; i++) greyscale.Entries[i] = Color.FromArgb(i, i, i);
                Bitmap.Palette = greyscale;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DirectBitmap class for the specified Bitmap
        /// </summary>
        /// <param name="image"></param>
        public DirectBitmap(Bitmap image)
        {
            Bpp = Image.GetPixelFormatSize(image.PixelFormat);
            Width = image.Width;
            Height = image.Height;
            Stride = 4 * ((Width * (Bpp / 8) + 3) / 4);
            Bits = new byte[Stride * Height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Stride, image.PixelFormat, BitsHandle.AddrOfPinnedObject());

            Bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            if (Bpp == 8)
            {
                ColorPalette greyscale = Bitmap.Palette;
                for (int i = 0; i < 256; i++) greyscale.Entries[i] = Color.FromArgb(i, i, i);
                Bitmap.Palette = greyscale;
            }

            Rectangle rect = new Rectangle(0, 0, Width, Height);
            BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            Marshal.Copy(ptr, Bits, 0, Bits.Length);
            image.UnlockBits(bmpData);
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
            
            Bits = null;
        }

        public byte GetPixel(int x, int y)
        {
            int loc = Stride * y + x;
            return Bits[loc];
        }

        public void SetPixel(int x, int y, byte value)
        {
            int loc = Stride * y + x;
            Bits[loc] = value;
        }

        /// <summary>
        /// Generates random noise with a Gaussian Distribution centered on any given pixel and a specified standard deviation.
        /// </summary>
        /// <param name="stdDev">Standard deviation to use for the distribution</param>
        public void Noise(int stdDev)
        {
            int seed = Environment.TickCount;
            ThreadLocal<Random> rand = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

            int lim = Height * Width;

            Parallel.For(0, lim / 2, i =>
            {
                int ind = i + i;
                int val = Bits[ind];
                int val2 = Bits[ind + 1];

                double x1, x2, w;
                do
                {
                    x1 = 2.0 * rand.Value.NextDouble() - 1.0;
                    x2 = 2.0 * rand.Value.NextDouble() - 1.0;
                    w = x1 * x1 + x2 * x2;
                } while (w >= 1.0);

                w = Math.Sqrt((-2.0 * Math.Log(w)) / w);

                int newVal = (int)(val + stdDev * x1 * w);
                newVal = Math.Max(Math.Min(newVal, 255), 0);

                int newVal2 = (int)(val2 + stdDev * x2 * w);
                newVal2 = Math.Max(Math.Min(newVal2, 255), 0);

                Bits[ind] = (byte)newVal;
                Bits[ind + 1] = (byte)newVal2;
            });
        }

        /// <summary>
        /// Blurs the image using a sliding box blur algorithm. Only works on 8-bit greyscale images
        /// </summary>
        /// <param name="level">Controls the size of the window. Higher level results in more blurring.</param>
        public void Blur(int level)
        {
            int dim = (2 * level) + 1;
            int mid = dim / 2;
            int sqr = dim * dim;
            int ylim = Height - dim + 1;
            int xlim = Width - dim + 1;

            byte[] newPx = new byte[Bits.Length];
            Buffer.BlockCopy(Bits, 0, newPx, 0, Bits.Length);

            byte[] avgs = new byte[sqr * 256];
            for (int i = 0; i < avgs.Length; i++)
            {
                avgs[i] = (byte)(i / sqr);
            }

            Parallel.For(0, ylim, y =>
            {
                int pxTot = 0;
                int row = y * Stride;
                int[] cols = new int[dim];
                for (int j = 0; j < dim; j++)
                {
                    for (int k = 0; k < dim; k++)
                    {
                        int val = Bits[row + Stride * j + k];
                        cols[k] += val;
                        pxTot += val;
                    }
                }

                byte newVal = avgs[pxTot];
                int center = row + Stride * mid + mid;
                newPx[center] = newVal;

                for (int x = 1; x < xlim; x++)
                {
                    int topLeft = row + x;

                    pxTot -= cols[0];
                    for (int i = 0; i < dim - 1; i++)
                    {
                        cols[i] = cols[i + 1];
                    }

                    cols[dim - 1] = 0;
                    for (int i = 0; i < dim; i++)
                    {
                        int val = Bits[topLeft + Stride * i + (dim - 1)];
                        cols[dim - 1] += val;
                        pxTot += val;
                    }

                    newVal = avgs[pxTot];
                    center++;

                    newPx[center] = newVal;
                }

            });

            Buffer.BlockCopy(newPx, 0, Bits, 0, newPx.Length);
        }

        /// <summary>
        /// Returns a new Bitmap rotated CCW by the specifed angle in degrees
        /// </summary>
        /// <param name="angle">Angle in Degrees</param>
        public Bitmap rotate(float angle)
        {
            int width = Width;
            int height = Height;

            // Find dimensions of new rotated image to prevent clipping
            double rads = angle * Math.PI / 180d;
            double cos = Math.Abs(Math.Cos(rads));
            double sin = Math.Abs(Math.Sin(rads));
            int newWidth = (int)Math.Round(width * cos + height * sin);
            int newHeight = (int)Math.Round(width * sin + height * cos);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            newImage.SetResolution(Bitmap.HorizontalResolution, Bitmap.VerticalResolution);

            using (Graphics g = Graphics.FromImage(newImage))
            {
                // Use high-quality settings to reduce information loss
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;

                // Set up the built-in transformation matrix to do the rotation
                g.TranslateTransform(newWidth / 2f, newHeight / 2f);

                // Rotate at the negative angle so it's rotated counter-clockwise
                g.RotateTransform(-angle);
                g.TranslateTransform(-width / 2f, -height / 2f);

                g.DrawImage(Bitmap, 0, 0);
            }

            return newImage;
        }

        /// <summary>
        /// Returns a new DirectBitmap object skewed (shear) in the x-direction
        /// </summary>
        /// <param name="angle">Angle in degrees to skew from the vertical axis</param>
        public DirectBitmap skew(int angle)
        {
            int width = Width;
            int height = Height;
            int stride = Stride;

            // check for negative angle (skew left)
            bool negative = false;
            if (angle < 0)
            {
                angle *= -1;
                negative = true;
            }

            double rads = angle * Math.PI / 180d;           // angle in radians
            double offset = Math.Tan(rads) * height;        // maximum row offset for the image

            DirectBitmap newImage = new DirectBitmap((int)(width + offset), height, Bpp);
            newImage.Bitmap.SetResolution(Bitmap.HorizontalResolution, Bitmap.VerticalResolution);

            int newStride = newImage.Stride;

            for (int i = 0; i < height; i++)
            {
                int rowOffset = 0;
                int rowStart = newStride * i;

                if (negative)                               // skew left (negative angle) starts with the minimum offset (0)
                {                                           // and slowly increase
                    rowOffset = (int)(offset * (i / (float)height));
                }
                else                                        // skew right (positive angle) starts with the maximum offset
                {                                           // and slowly decreases
                    rowOffset = (int)(offset * ((height - i) / (float)height));
                }

                for (int j = 0; j < width; j++)
                {
                    int val = Bits[j + stride * i];
                    int ind = rowStart + rowOffset + j;
                    newImage.Bits[ind] = (byte)val;
                }
            }

            return newImage;
        }

        /// <summary>
        /// Adjusts the contrast of the image by a specified amount
        /// </summary>
        /// <param name="amt">-255 to 0 decreases the contrast. 1 to 256 increases the contrast.</param>
        public void contrast(double amt)
        {
            int lim = Width * Height;
            for (int i = 0; i < lim; i++)
            {
                int val = Bits[i];
                double factor = (259 * (amt + 255)) / (255 * (259 - amt));
                int newVal = (int)(factor * (val - 128) + 128);
                newVal = Math.Max(Math.Min(newVal, 255), 0);

                Bits[i] = (byte)newVal;
            }
        }

        /// <summary>
        /// Replaces pixels in the image with those of a specified image
        /// </summary>
        /// <param name="snippet">image to be stitched (must be smaller than the destination image)</param>
        /// <param name="x">Horizontal position of the starting point (top-left corner)</param>
        /// <param name="y">Vertical position of the starting point (top-left corner)</param>
        public void stitch(DirectBitmap snippet, int x, int y)
        {
            int stride = Stride;
            int snipWidth = snippet.Width;
            int snipHeight = snippet.Height;
            int snipStride = snippet.Stride;

            for (int i = 0; i < snipHeight; i++)
            {
                for (int j = 0; j < snipWidth; j++)
                {
                    byte val = snippet.Bits[i * snipStride + j ];
                    if (val != 0)   // Do not replace pixels with a non-transparent background (ex: rotated images)
                        Bits[(i + y) * stride + (j + x)] = val;
                }
            }
        }


    }
}
