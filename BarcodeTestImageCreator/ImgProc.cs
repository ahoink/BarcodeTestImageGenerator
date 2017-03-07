using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime;

namespace BarcodeTestImageCreator
{
    class ImgProc
    {
        public delegate void dlgProgress(int value);
        public event dlgProgress Progress;

        static object locker = new object();

        private int seed = Environment.TickCount;

        int sizeErrs = 0;
        int resolErrs = 0;
        int genericErrs = 0;

        int[] settings = new int[5];

        // ---------- Main Snippet Function - returns true upon successful completion ---------- //
        public bool snippet(string path, string output)
        {
            List <string> images = getImageNames(path);
            int numImgs = images.Count;
            if (numImgs == 0)
            {
                MainForm.dispError("Could not find any TIFF files in the given path:\n" + path, "File(s) not found");
                return false;
            }

            string txtFile;
            Dictionary<string, int[]> coordinates = getCoordinates(path, out txtFile);
            int numCoords = coordinates.Count;
            if (numCoords == 0)
            {
                MainForm.dispError("Could not find any coordinate (.txt) files in the given path:\n" + path, "File(s) not found");
                return false;
            }

            //if (numCoords != numImgs)
            //{
            //    string extra = "";
            //    if (numCoords > numImgs)
            //    {
            //        extra = "coordinates";
            //        coordinates.RemoveRange(numImgs, numCoords - numImgs);
            //        numCoords = coordinates.Count;
            //    }
            //    else
            //    {
            //        extra = "images";
            //        images.RemoveRange(numCoords, numImgs - numCoords);
            //        numImgs = images.Count;
            //    }
            //    MainForm.dispError("Number of coordinates does not match number of images.\nThe program will ignore the extra " + extra, "File Mismatch");
            //}

            // Create a new folder if one doesn't already exist. Otherwise ask to delete it
            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }
            else
            {
                bool overwrite = MainForm.dispDirExists(output);
                if (overwrite)
                {                   
                    Directory.Delete(output, true);
                    while (Directory.Exists(output)) Thread.Sleep(0);
                    Directory.CreateDirectory(output);

                }
                else return false;
            }

            // Delete the previous error log if it exists
            if (File.Exists("error.log")) File.Delete("error.log");

            // Create truth.tsv in the new snippets folder
            createTruth(txtFile, output);

            Progress?.Invoke(10);

            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            for (int i = 0; i < numImgs; i++)
            {
                // Don't need multithreading for cropping - it's already pretty fast (smaller images = faster save time)
                string image = images[i];
                if (coordinates.ContainsKey(image))
                {
                    int[] coords = coordinates[image];
                    cropImage(image, output, coords);
                    Progress?.Invoke((i + 1) * 100 / numImgs);        // Invoke Progress delegate to update progress bar with percentage of images processed
                }
                else
                {
                    sizeErrs++;
                    using (StreamWriter f = File.AppendText("error.log"))
                    {
                        f.WriteLine("ERROR:No coordinates found for barcode");
                        f.WriteLine("\tImage: " + path);
                    }
                }
            }

            timer.Stop();
            Console.WriteLine("Snippet Time: " + timer.ElapsedMilliseconds);

            // Display message for any errors that occurred
            if (sizeErrs > 0 || genericErrs > 0)
            {
                string msg = "";

                if (sizeErrs > 0)
                {
                    msg += sizeErrs.ToString() + " snippet(s) could not be created due to the region defined by the coordinates being too small or nonexistent.\n";
                }
                if (genericErrs > 0)
                {
                    msg += genericErrs.ToString() + " generic error(s) occured while attempting to save images. Rerun to try again.\n";
                }

                msg += "\nCheck the error log for more information";
                MainForm.dispError(msg, "Error(s) Occured");
            }

            return true;
        }



        // ---------- Main Stitch Function - returns true upon successful completion ---------- //
        public bool stitch(string snippetPath, string baseImPath, string output, int mode, bool randomize)
        {
            List<string> barcodes = getImageNames(snippetPath);
            int numBarcodes = barcodes.Count;
            if (numBarcodes == 0) return false;

            List<string> images = getImageNames(baseImPath);
            int numImgs = images.Count;
            if (numImgs == 0) return false;

            // Find largest image
            long maxIm = 0;
            foreach (var f in images)
            {
                maxIm = Math.Max(maxIm, new FileInfo(f).Length);
            }

            // Find largest barcode snippet
            long maxBc = 0;
            foreach (var f in barcodes)
            {
                maxBc = Math.Max(maxBc, new FileInfo(f).Length);
            }

            // Set the maxThreads to prevent memory overflow (32-bit max is 1.2GB, set maxThreads to limit up to 1GB)
            int maxThreads = (int)Math.Max(Environment.ProcessorCount, (1e9 / (maxIm * 3 + maxBc * 3)));
            ThreadPool.SetMaxThreads(maxThreads, 1000);

            string txtFile;
            Dictionary<string, int[]> edgeDetects = getCoordinates(baseImPath, out txtFile);

            // Create a new folder if one doesn't already exist. Otherwise ask to delete it
            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }
            else
            {
                bool overwrite = MainForm.dispDirExists(output);
                if (overwrite)
                {

                    try
                    {
                        Directory.Delete(output, true);
                        while (Directory.Exists(output)) Thread.Sleep(0);   // Hacky fix to the directory sometimes not deleting immediately
                        Directory.CreateDirectory(output);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e);
                    }
                    
                }
                else return false;
            }

            // Delete the previous error log if it exists
            if (File.Exists("error.log")) File.Delete("error.log");

            // Check settings config file
            if (File.Exists("settings.conf"))
            {
                string[] lines;
                using (StreamReader f = new StreamReader("settings.conf"))
                {
                    lines = f.ReadToEnd().Split('\n');
                }
                for (int i = 0; i < 5; i++)
                {
                    settings[i] = int.Parse(lines[i].Split('=')[1]);
                }
            }

            // Dictionary for barcode truth values
            Dictionary<string, string> barcodeTruth = getTruth(snippetPath + "truth.tsv");
            if (barcodeTruth.Count == 0)
            {
                MainForm.dispError("No truth file was found in the selected snippet folder.\ntruth.tsv will not be generated for the new images.", "Missing File");
            }

            StringBuilder truth = new StringBuilder();
            truth.AppendLine("H\tFILE\tBARCODE\tBARCODE_TYPE\tBARCODE_RESULTS");

            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            int target = 0;
            // Perform task based on which stitching mode was selected (1 = one-to-one  2 = all possible combinations  other = specific number)
            if (mode == 1)
            {
                target = numImgs;
                for (int i = 0; i < numImgs; i++)                               // Paste one barcode onto every base image, once
                {
                    string barcode = barcodes[i % numBarcodes];
                    string image = images[i];
                    int[] edge = new int[4];
                    if (edgeDetects.ContainsKey(image))
                    {
                        edge = edgeDetects[image];
                    }

                    string imName = output + image.Replace(baseImPath, "").Replace(".tif", "_stitched.tif");

                    ThreadPool.QueueUserWorkItem(state => pasteBarcode(barcode, image, imName, edge, randomize));
                    if (barcodeTruth.ContainsKey(barcode))                      // Append line to the truth file, if the truth value exists
                    {
                        truth.AppendLine("T\t" + imName + "\tBARCODE\t9\t" + barcodeTruth[barcode]);
                    }
                }
            }
            else if (mode == 2)
            {
                int indLen = numBarcodes.ToString().Length;
                target = numImgs * numBarcodes;
                
                for (int i = 0; i < numImgs; i++)                               // Paste every barcode onto a separate copy of each base image
                {
                    string image = images[i];
                    string imName = output + image.Replace(baseImPath, "").Replace(".tif", "_stitched");
                    int[] edge = new int[4];
                    if (edgeDetects.ContainsKey(image))
                    {
                        edge = edgeDetects[image];
                    }

                    for (int j = 0; j < numBarcodes; j++)
                    {
                        string ind = j.ToString().PadLeft(indLen, '0');         // Same base image is being used, so append a number with leading zeros
                        string barcode = barcodes[j];
                        
                        ThreadPool.QueueUserWorkItem(state => pasteBarcode(barcode, image, imName + ind + ".tif", edge, randomize));
                        if (barcodeTruth.ContainsKey(barcode))
                        {
                            truth.AppendLine("T\t" + imName + ind + ".tif" + "\tBARCODE\t9\t" + barcodeTruth[barcode]);
                        }
                    }
                }
            }
            else                                                                // Specific number of images
            {
                Random rand = new Random();
                target = Math.Min(mode, numImgs * numBarcodes);                 // Target is specified number, unless it's greater than all possible combinations
                double n = 1d * target / numImgs;                               // number of barcodes to use per image
                int bcLim = (int)n;                                             // number of barcodes to use per image, rounded down
                int imLim = (int)(numImgs - (n - bcLim) * numImgs);             // number of images to use the rounded down value
                int indLen = bcLim.ToString().Length;
                
                for (int i = 0; i < imLim; i++)
                {
                    string image = images[i];
                    string imName = output + image.Replace(baseImPath, "").Replace(".tif", "_stitched");
                    int[] edge = new int[4];
                    if (edgeDetects.ContainsKey(image))
                    {
                        edge = edgeDetects[image];
                    }

                    for (int j = 0; j < bcLim; j++)
                    {
                        string ind = j.ToString().PadLeft(indLen, '0');
                        string barcode = barcodes[rand.Next(numBarcodes)];

                        ThreadPool.QueueUserWorkItem(state => pasteBarcode(barcode, image, imName + ind + ".tif", edge, randomize));

                        if (barcodeTruth.ContainsKey(barcode))
                        {
                            truth.AppendLine("T\t" + imName + ind + ".tif" + "\tBARCODE\t9\t" + barcodeTruth[barcode]);
                        }
                    }
                }

                bcLim = (int)Math.Ceiling(n);                                   // number of barcodes to use per image, rounded up
                indLen = bcLim.ToString().Length;
                for (int i = imLim; i < numImgs; i++)
                {
                    string image = images[i];
                    string imName = output + image.Replace(baseImPath, "").Replace(".tif", "_stitched");
                    int[] edge = new int[4];
                    if (edgeDetects.ContainsKey(image))
                    {
                        edge = edgeDetects[image];
                    }

                    for (int j = 0; j < bcLim; j++)
                    {
                        string ind = j.ToString().PadLeft(indLen, '0');
                        string barcode = barcodes[rand.Next(numBarcodes)];

                        ThreadPool.QueueUserWorkItem(state => pasteBarcode(barcode, image, imName + ind + ".tif", edge, randomize));

                        if (barcodeTruth.ContainsKey(barcode))
                        {
                            truth.AppendLine("T\t" + imName + ind + ".tif" + "\tBARCODE\t9\t" + barcodeTruth[barcode]);
                        }
                    }
                }

            }

            // Create truth file for the new images (creates entry for each new image regardless if they were successfully created or not)           
            using (StreamWriter f = new StreamWriter(output + "truth.tsv"))
            {
                f.Write(truth);
            }
            Progress?.Invoke(1);
            barcodeTruth = null;
            truth = null;
            images = null;
            barcodes = null;
            edgeDetects = null;

            // Force garbage collection to make sure unneed lists/arrays are cleared. 
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            int a, b, c, d;

            // Watch output directory to update progress bar
            int numFiles = Directory.GetFiles(output).Length;
            while (numFiles < (target - sizeErrs - genericErrs))
            {
                numFiles = Directory.GetFiles(output).Length - 1;
                Progress?.Invoke(numFiles * 100 / target);
                ThreadPool.GetMaxThreads(out a, out b);
                ThreadPool.GetAvailableThreads(out c, out d);
                Console.WriteLine("Work Threads: " + (a - c) + "\tPort Threads: " + (b - d));   
            }
            Progress?.Invoke(100);                                      // Force 100% progress in case rounding doesn't reach 100

            timer.Stop();
            Console.WriteLine("Total stitching time: " + timer.ElapsedMilliseconds);

            // Display message for any errors found
            if (sizeErrs > 0 || resolErrs > 0 || genericErrs > 0)
            {
                string msg = "";

                if (sizeErrs > 0)
                {
                    msg += sizeErrs.ToString() + " image(s) could not be created due to size mismatch (barcode snippet larger than base image).\n";
                }
                if (resolErrs > 0)
                {
                    msg += resolErrs.ToString() + " image(s) had a resolution mismatch between the barcode snippet and base image and might not have saved properly.\n";
                }
                if (genericErrs > 0)
                {
                    msg += genericErrs.ToString() + " generic error(s) occured while attempting to save images. Rerun to try again.\n";
                }

                msg += "\nCheck the error log for more information";
                MainForm.dispError(msg, "Error(s) Occured");
            }

            return true;
        }



        // ------------------------- FUNCTIONS FOR PARSING TEXT FILES ------------------------- //

        // Return all image file names in the specified folder as a list
        private List<string> getImageNames(string path)
        {
            List<string> names = new List<string>();

            if (!Directory.Exists(path))                                // Invalid path will return an empty list
            {
                return names;
            }

            names = Directory.EnumerateFiles(path)                      // Grab all tif files at once
                .Where(f => f.Split('.')
                .Last() == "tif")
                .ToList();     



            //names = Directory.EnumerateFiles(path, "*.tif")
            //    .OrderByDescending(f => f.Length)
            //    .Select(f => f.ToString())
            //    .ToList();

            return names;
        }

        // Returns coordinates parsed from text file as a list of int arrays
        private Dictionary<string, int[]> getCoordinates(string path, out string txtFile)
        {
            Dictionary<string, int[]> coords = new Dictionary<string, int[]>();
             txtFile = null;

            if (!Directory.Exists(path))
            {           
                return coords;
            }

            string[] textFiles = Directory.EnumerateFiles(path)
                                .Where(f => f.Split('.').Last() == "txt").ToArray();    // Grab all text files at once 
            foreach (string textFile in textFiles)
            {
                string[] lines = File.ReadAllLines(textFile);
                int numLines = lines.Length;

                for (int i = 0; i < numLines; i++)
                {
                    int[] rect = new int[4];
                    string line = lines[i];
                    if (line.Length <= 1) continue;

                    string[] elements = line.Split('\t');                               // Split on tab
                    string name = elements[0];

                    try                                                                 // try-catch incase the text file is improperly formatted
                    {
                        for (int j = 1; j <= 4; j++)
                        {
                            rect[j - 1] = int.Parse(elements[j]);                       // Parse the tab delimited values as ints
                        }
                    }
                    catch (FormatException)
                    {
                        break;
                    }
                    coords.Add(name, rect);
                }

                if (coords.Count > 0)
                {
                    txtFile = textFile;
                    break;
                }
            }
          
            return coords;
        }

        // Creates truth file for barcodes in the snippet folder
        private void createTruth(string path, string newPath)
        {
            // The file is assumed to exist and be formatted properly at this point

            using (StreamWriter f = new StreamWriter(newPath + "truth.tsv"))
            {
                string[] lines = File.ReadAllLines(path);
                int numLines = lines.Length;
                
                for (int i = 0; i < numLines; i++)
                {
                    string line = lines[i];
                    if (line.Length <= 1) continue;

                    string[] elements = line.Split('\t');

                    string[] pathSplit = elements[0].Split('\\');
                    string bcName = pathSplit[pathSplit.Length - 1].Replace(".tif", "_snippet.tif");

                    f.WriteLine(newPath + bcName + "\t" + elements[5]);
                }
            }

        }

        // Returns a dictionary of the barcode values from the truth file in the snippet folder
        private Dictionary<string, string> getTruth(string path)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (!File.Exists(path))         // Make sure truth file exists
            {
                return dict;
            }

            string[] lines = File.ReadAllLines(path);
            int numLines = lines.Length;

            for (int i = 0; i < numLines; i++)
            {
                string line = lines[i];
                if (line.Length <= 1) continue;

                string[] elements = line.Split('\t');
                dict.Add(elements[0], elements[1]);
            }

            return dict;
        }



        // ------------------------- IMAGE PROCESSING FUNCTIONS ------------------------- //

        // Crop Image to specified region and save to the new folder
        private void cropImage(string path, string newPath, int[] region)
        {        
            Bitmap src = Image.FromFile(path) as Bitmap;
           
            region = edgeDetectBC(src, region);

            // Region = Top, Bottom, Left, Right
            int x = region[2];
            int y = region[0];
            int width = region[3] - region[2];
            int height = region[1] - region[0];

            // Check if the region is too small or negative
            if (width <= 90 || height <= 90)
            {
                sizeErrs++;

                using (StreamWriter f = File.AppendText("error.log"))
                {
                    f.WriteLine("ERROR: Region too small to be a valid barcode");
                    f.WriteLine("\tImage: " + path);
                    f.WriteLine("\tRegion: " + width + "x" + height + " starting at (" + x + ", " + y + ")");
                }
                src.Dispose();
                return;
            }

            Rectangle bcBounds = new Rectangle(x, y, width, height);                        // Region formed from coordinates
            Bitmap barcode = new Bitmap(bcBounds.Width, bcBounds.Height);                   // Blank canvas
            barcode.SetResolution(src.HorizontalResolution, src.VerticalResolution);        // Maintain resolution

            using (Graphics g = Graphics.FromImage(barcode))                                // Create a blank canvas
            {
                g.DrawImage(src, new Rectangle(0, 0, barcode.Width, barcode.Height),        // Draw an image from "bcBounds" sized region of src onto a "barcode" sized region
                                 bcBounds,
                                 GraphicsUnit.Pixel);              
            }

            DirectBitmap reformat = convertTo8bpp(barcode);                                 // Convert to 8-bit greyscale (new Bitmaps default to 32-bit ARGB)
            //float angle = findAngle(src, region);                                           // Find angle of barcode so it can be straightened
            //Bitmap newim = reformat.rotate(angle);                                          // Rotate to a horizontal orientation
            //newim = cropClose(newim);                                                       // Re-crop the barcode
            //reformat = convertTo8bpp(newim);                                                // convert again
            //newim.Dispose();


            string[] pathSplit = path.Split('\\');
            string imName = pathSplit[pathSplit.Length - 1].Split('.')[0] + "_snippet.tif";
            try
            {
                saveTiff(reformat.Bitmap, newPath + imName);                                // Save as <original name>_snippet.tif
            }
            catch (Exception)
            {
                genericErrs++;
            }

            reformat.Dispose();
            src.Dispose();
            barcode.Dispose();
        }

        // Paste specified barcode onto the specified image and save to the new folder
        private void pasteBarcode(string bcPath, string imPath, string newPath, int[] edge, bool randomize)
        {
            Bitmap barcode = Image.FromFile(bcPath) as Bitmap;
            Bitmap baseImg = Image.FromFile(imPath) as Bitmap;

            // Maximum 5000x5000, PetProcess doesn't like images that are bigger than this
            if (baseImg.Width > 5000 || baseImg.Height > 5000)
            {
                int wDiff = Math.Max(0, baseImg.Width - 5000);
                int hDiff = Math.Max(0, baseImg.Height - 5000);

                Rectangle crop = new Rectangle(wDiff / 2, hDiff / 2, baseImg.Width - wDiff, baseImg.Height - hDiff);
                using (Bitmap temp = new Bitmap(crop.Width, crop.Height))
                {
                    temp.SetResolution(baseImg.HorizontalResolution, baseImg.VerticalResolution);

                    using (Graphics g = Graphics.FromImage(temp))
                    {
                        g.DrawImage(baseImg, new Rectangle(0, 0, temp.Width, temp.Height),
                                         crop,
                                         GraphicsUnit.Pixel);
                    }

                    using (DirectBitmap reformat = convertTo8bpp(temp))
                    {
                        baseImg = new Bitmap(reformat.Bitmap);
                        baseImg.SetResolution(temp.HorizontalResolution, temp.VerticalResolution);                        
                    }
                }

                edge[0] = Math.Min(edge[0], 4990);
                edge[1] = Math.Max(edge[1], 10);
                edge[2] = Math.Max(edge[2], 10);
                edge[3] = Math.Min(edge[3], 4990);
            }

            // Size mismatch (barcode bigger than base image) breaks out of function
            if ((barcode.Width >= baseImg.Width) || (barcode.Height >= baseImg.Height))
            {
                sizeErrs++;
                // set tempMsg since the bitmaps are being disposed
                string tempMsg = "\tBarcode: " + bcPath + " (" + barcode.Width + "x" + barcode.Height + ")" +
                    "\n\tBase Image: " + imPath + " (" + baseImg.Width + "x" + baseImg.Height + ")";

                // Immediately dispose of bitmaps to make memory available again
                barcode.Dispose();
                baseImg.Dispose();

                lock (locker)
                {
                    // Log the error
                    using (StreamWriter f = File.AppendText("error.log"))
                    {
                        f.WriteLine("ERROR:Barcode larger than base image");
                        f.WriteLine(tempMsg);
                    }
                }
                    return;
            }
            
            // Resolution mismatch - function still continues
            if (Math.Round(barcode.HorizontalResolution) != Math.Round(baseImg.HorizontalResolution) ||
                Math.Round(barcode.VerticalResolution) != Math.Round(baseImg.VerticalResolution))
            {
                resolErrs++;

                lock (locker)
                {
                    using (StreamWriter f = File.AppendText("error.log"))
                    {
                        f.WriteLine("WARNING:Barcode and base image resolutions don't match");
                        f.WriteLine("\tBarcode: " + bcPath + " (" + barcode.HorizontalResolution + ")");
                        f.WriteLine("\tBase Image: " + imPath + " (" + baseImg.HorizontalResolution + ")");
                    }
                }
            }

            // Get edges if they weren't in the text file
            if (edge.All(x => x == 0))
            {
                edge = edgeDetect(baseImg);
            }

            // Thread-safe RNG 
            ThreadLocal<Random> rand = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

            // Adjust the barcode based on any user settings
            if (settings[0] != 0)   // Rotate
            {
                
                using (DirectBitmap temp = new DirectBitmap(barcode))
                {
                    barcode.Dispose();
                    if (settings[0] == 999) // 999 in settings means random                    
                        barcode = temp.rotate(rand.Value.Next(-359, 360));
                    else
                        barcode = temp.rotate(settings[0]);
                }
            }
            if (settings[1] != 0)   // Skew (shear)
            {
                using (DirectBitmap temp = new DirectBitmap(barcode))
                {
                    barcode.Dispose();
                    if (settings[1] == 999)
                        barcode =  new Bitmap(temp.skew(rand.Value.Next(-45, 46)).Bitmap);
                    else
                        barcode = new Bitmap(temp.skew(settings[1]).Bitmap);
                }
            }

            if (edge[3] - barcode.Width <= edge[2] || edge[0] - barcode.Height <= edge[1])
            {
                sizeErrs++;
                string tempMsg = "\tBarcode: " + bcPath + " (" + barcode.Width + "x" + barcode.Height + ")";

                barcode.Dispose();
                baseImg.Dispose();
                rand.Dispose();

                lock (locker)
                {
                    using (StreamWriter f = File.AppendText("error.log"))
                    {
                        f.WriteLine("ERROR:Barcode larger than area defined by base image edges");
                        f.WriteLine(tempMsg);
                        f.WriteLine("\tBase Image: " + imPath + " (" + (edge[3] - edge[2]) + "x" + (edge[0] - edge[1]) + ")");
                    }
                }

                return;
            }

            // edge = Bottom, Top, Left, Right           
            Point pastePt;
            
            if (randomize) // Random point inside valid area (within bounds of actual document)
            {                                            
                pastePt = new Point(rand.Value.Next(edge[2], edge[3] - barcode.Width), rand.Value.Next(edge[1], edge[0] - barcode.Height));               
            }
            else // Center
            {
                pastePt = new Point((edge[3] + edge[2]) / 2 - barcode.Width / 2, (edge[0] + edge[1]) / 2 - barcode.Height / 2);
            }

            DirectBitmap barcodeDirect = Image.GetPixelFormatSize(barcode.PixelFormat) == 32 ? 
                convertTo8bpp(barcode) : new DirectBitmap(barcode);
            barcode.Dispose();
            barcode = null;

            DirectBitmap baseImageDirect = Image.GetPixelFormatSize(baseImg.PixelFormat) == 32 ?
                convertTo8bpp(baseImg) : new DirectBitmap(baseImg);
            baseImg.Dispose();
            baseImg = null;

            // Make sure there are black edges
            int lim = Math.Max(baseImageDirect.Width, baseImageDirect.Height);
            for (int i = 0; i < lim; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (i < baseImageDirect.Width)
                    {
                        baseImageDirect.Bits[i + j * baseImageDirect.Stride] = 0;
                        baseImageDirect.Bits[i + (baseImageDirect.Height - 1 - j) * baseImageDirect.Stride] = 0;
                    }
                    if (i < baseImageDirect.Height)
                    {
                        baseImageDirect.Bits[j + i * baseImageDirect.Stride] = 0;
                        baseImageDirect.Bits[(baseImageDirect.Width - 1 - j) + i * baseImageDirect.Stride] = 0;
                    }
                }
            }

            baseImageDirect.stitch(barcodeDirect, pastePt.X, pastePt.Y);
            barcodeDirect.Dispose();
            barcodeDirect = null;

            // Adjust the output image based on any user settings (999 in settings means random)
            if (settings[2] == 999) baseImageDirect.Noise(rand.Value.Next(36));
            else if (settings[2] != 0) baseImageDirect.Noise(settings[2]);
            if (settings[3] == 999) baseImageDirect.Blur(rand.Value.Next(4));
            else if (settings[3] != 0) baseImageDirect.Blur(settings[3]);
            if (settings[4] == 999) baseImageDirect.contrast(rand.Value.Next(-255, 256));
            else if (settings[4] != 0) baseImageDirect.contrast(settings[4]);
            rand.Dispose();

            try
            {
                saveTiff(baseImageDirect.Bitmap, newPath);      
            }
            catch (Exception)
            {
                genericErrs++;
            }
            
            baseImageDirect.Dispose();
            baseImageDirect = null;
        }

        // Attempts to detect the edge of a document by starting at each side and moving in until a light pixel is reached
        // Returns int array in the form of {Bottom, Top, Left, Right}
        private int[] edgeDetect(Bitmap im)
        {
            int[] edge = new int[4];

            // Left
            int x = 0;
            int y = im.Height / 2;
            while (im.GetPixel(x, y).R < 50) x++;
            edge[2] = x;

            // Right
            x = im.Width - 1;
            while (im.GetPixel(x, y).R < 50) x--;
            edge[3] = x;

            // Top
            x = im.Width / 2;
            y = 0;
            while (im.GetPixel(x, y).R < 50) y++;
            edge[1] = y;

            // Bottom
            y = im.Height - 1;
            while (im.GetPixel(x, y).R < 50) y--;
            edge[0] = y;

            return edge;
        }

        // Finds the edges of a barcode being using the passed region which defines its estimated location
        private int[] edgeDetectBC(Bitmap im, int[] region)
        {
            //Top, Bottom, Left, Right
            // Move out 20px on all sides
            int top = region[0];
            int bottom = region[1];
            int left = region[2];
            int right = region[3];
            int panOut = 20;

            int[] edge = new int[4];

            // Left
            int temp = right + panOut;                                      // temp keeps track of the previously found edge
            int cnt = 0;
            for (int y = top; y < bottom; y += 5)                           // Start y at the top and move down 5px each iteration
            {
                int x = Math.Max(left - panOut, 0);                         // Start x at the far left, out an additional 20px
                while (im.GetPixel(x, y).R > 60 && x < temp) x++;           // move right until an edge is found (or when the previous value is reached)
                if (x == temp) cnt++;
                else cnt = 0;
                if (cnt >= 10) break;                                       // With 10 identical values found consecutively, a lesser value probably won't be found
                temp = x;
            }
            edge[2] = left > temp ? temp - 30 : left - 30;                  // Pad 30px to the found edge

            // Right
            temp = left - panOut;
            cnt = 0;
            for (int y = top; y < bottom; y += 5)
            {
                int x = Math.Min(right + panOut, im.Width);
                while (im.GetPixel(x, y).R > 60 && x > temp) x--;
                if (x == temp) cnt++;
                else cnt = 0;
                if (cnt >= 10) break;
                temp = x;
            }
            edge[3] = right < temp ? temp + 30 : right + 30;

            // Top
            temp = bottom + panOut;
            cnt = 0;
            for (int x = left; x < right; x += 5)
            {
                int y = Math.Max(top - panOut, 0);
                while (im.GetPixel(x, y).R > 60 && y < temp) y++;
                if (y == temp) cnt++;
                else cnt = 0;
                if (cnt >= 10) break;
                temp = y;
            }
            edge[0] = top > temp ? temp - 30 : top - 30;

            // Bottom
            temp = top - panOut;
            cnt = 0;
            for (int x = left; x < right; x += 5)
            {
                int y = Math.Min(bottom + panOut, im.Height);
                while (im.GetPixel(x, y).R > 60 && y > temp) y--;
                if (y == temp) cnt++;
                else cnt = 0;
                if (cnt >= 10) break;
                temp = y;
            }
            edge[1] = bottom < temp ? temp + 30 : bottom + 30;

            return edge;
        }

        // Returns the angle in degrees at which a barcode is oriented
        private float findAngle(Bitmap im, int[] region)
        {
            //region = {Top, Bottom, Left, Right}

            List<Point> pts = new List<Point>();
            int temp = region[3] - 5;
            int start = region[2];
            int lim = region[0] + 5 * 30;
            for (int y = region[0]; y < lim; y += 3)
            {
                int x = start;
                while (im.GetPixel(x, y).R > 60 && x < temp + 5) x++;
                if (x == temp)
                {
                    while (im.GetPixel(x, y).R <= 60) x--;
                    x++;
                    start = x;
                    pts.Add(new Point(y, x));
                }
                if (im.GetPixel(x, y).R <= 60) temp = x;
            }

            if (pts.Count <= 1) return 0;

            // Linear regression to find approximate slope and y-intercept
            // NOTE: x and y are swapped for this calculation so x becomes the dependent variable
            double xMean = 0;
            double yMean = 0;
            double slopeNume = 0;
            double slopeDenom = 0;

            foreach (Point pt in pts)
            {
                xMean += pt.X;
                yMean += pt.Y;
            }
            xMean /= pts.Count;
            yMean /= pts.Count;
            
            foreach (Point pt in pts)
            {
                double xMinusMean = (pt.X - xMean);
                slopeNume += xMinusMean * (pt.Y - yMean);
                slopeDenom += xMinusMean * xMinusMean;
            }

            double slope = slopeNume / slopeDenom;
            double intc = yMean - slope * xMean;

            //double angleRads = Math.Atan((pts.Last().Y - pts[0].Y) * 1d / (pts.Last().X - pts[0].X));
            double angleRads = Math.Atan(((pts.Last().X * slope + intc) - (pts[0].X * slope + intc)) / (pts.Last().X - pts[0].X));
            float angleDeg = -(float)(angleRads * 180 / Math.PI);
            if ((region[1] - region[0]) > (region[3] - region[2])) angleDeg -= 90;      // subtract 90 so all barcodes get rotated to a horizontal orientation
            return angleDeg;
        }

        // Crops a horizontally oriented barcode to a region closer to the barcode (with a 30px pad on all sides)
        private static Bitmap cropClose(Bitmap im)
        {
            Point center = new Point(im.Width / 2, im.Height / 2);
            int[] edge = new int[4];
            int whiteLim = 16;
            int threshold = 60;

            // Left
            int x = center.X;                                   // Start at the center
            int y = center.Y;
            int whiteCnt = 0;
            while (whiteCnt < whiteLim)                         // Move left until a series of light pixels is seen
            {
                if (im.GetPixel(x, y).R > 60) whiteCnt++;
                else whiteCnt = 0;
                x--;
            }

            edge[2] = x - (30 - whiteLim);                     // 30px padding to the edge of the barcode

            // Right
            x = center.X;
            y = center.Y;
            whiteCnt = 0;
            while (whiteCnt < whiteLim)
            {
                if (im.GetPixel(x, y).R > threshold) whiteCnt++;
                else whiteCnt = 0;
                x++;
            }
            edge[3] = x + (30 - whiteLim);

            // Top
            x = center.X;
            y = center.Y;
            while (im.GetPixel(x, y).R > 50) x--;               // Move left until a dark pixel is seen
            while (                                             // Move up, while checking three pixels at a time, until no dark pixels are seen
                im.GetPixel(x - 1, y).R <= threshold ||
                im.GetPixel(x, y).R <= threshold ||
                im.GetPixel(x + 1, y).R <= threshold
                ) y--;
            edge[0] = y - 30;

            // Bottom
            x = center.X;
            y = center.Y;
            while (im.GetPixel(x, y).R > 50) x--;
            while (
                im.GetPixel(x - 1, y).R <= threshold ||
                im.GetPixel(x, y).R <= threshold ||
                im.GetPixel(x + 1, y).R <= threshold
                ) y++;
            edge[1] = y + 30;


            x = edge[2];
            y = edge[0];
            int width = edge[3] - edge[2];
            int height = edge[1] - edge[0];

            Rectangle bcBounds = new Rectangle(x, y, width, height);
            Bitmap barcode = new Bitmap(bcBounds.Width, bcBounds.Height);
            barcode.SetResolution(im.HorizontalResolution, im.VerticalResolution);

            // recrop the image
            using (Graphics g = Graphics.FromImage(barcode))
            {
                g.DrawImage(im, new Rectangle(0, 0, barcode.Width, barcode.Height),
                                 bcBounds,
                                 GraphicsUnit.Pixel);
            }

            return barcode;
        }

        // Call Bitmap.Save() using Tiff encoding and no compression
        private void saveTiff(Bitmap image, string path)
        {
            ImageCodecInfo tiff = ImageCodecInfo.GetImageEncoders()[3];
            EncoderParameters encodeParams = new EncoderParameters(1);
            encodeParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression,
                (long)EncoderValue.CompressionNone);
            image.Save(path, tiff, encodeParams);
        }

        // Convert a 32-bit ARGB Bitmap to an 8-bit greyscale Bitmap (new Bitmaps default to 32-bit)
        private DirectBitmap convertTo8bpp(Bitmap image)
        {
            int h = image.Height;
            int w = image.Width;

            DirectBitmap newImage = new DirectBitmap(w, h, 8);
            newImage.Bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            Rectangle rect = new Rectangle(0, 0, w, h);
            int bytesLen = h * w * 4;
            byte[] bytes32 = new byte[bytesLen];

            BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            Marshal.Copy(ptr, bytes32, 0, bytesLen);
            image.UnlockBits(bmpData);

            int newStride = 4 * ((w + 3) / 4);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    newImage.Bits[y * newStride + x] = bytes32[(y * w + x) * 4];
                }
            }

            return newImage;
        }

    }
}
