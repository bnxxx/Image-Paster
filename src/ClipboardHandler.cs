using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClipboardPaster
{
    public static class ClipboardHandler
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const int VK_SHIFT = 0x10;

        /// <summary>
        /// Checks if the SHIFT key is currently held down.
        /// </summary>
        private static bool IsShiftKeyDown()
        {
            return (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0 || 
                   (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
        }

        /// <summary>
        /// Attempts to retrieve an image from the Windows Clipboard.
        /// Handles both direct image objects (screenshots, web copy) and copied image files.
        /// </summary>
        private static Image GetClipboardImage()
        {
            try
            {
                // First, check if the clipboard directly contains an image (e.g., Snipping Tool, PrintScreen, web copy)
                if (Clipboard.ContainsImage())
                {
                    return Clipboard.GetImage();
                }

                // Second, check if the clipboard contains a file drop list (e.g., copied file from Explorer)
                if (Clipboard.ContainsFileDropList())
                {
                    var fileList = Clipboard.GetFileDropList();
                    if (fileList != null && fileList.Count > 0)
                    {
                        string firstFile = fileList[0];
                        if (File.Exists(firstFile))
                        {
                            string ext = Path.GetExtension(firstFile).ToLowerInvariant();
                            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".gif" || ext == ".tiff")
                            {
                                // Load image from file (using a copy to avoid locking the original file)
                                using (var bmpTemp = new Bitmap(firstFile))
                                {
                                    return new Bitmap(bmpTemp);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Silent catch as agreed in specification
            }

            return null;
        }

        /// <summary>
        /// Generates a unique default filename like Image_YYYY-MM-DD_HHMMSS.png in the target directory.
        /// </summary>
        private static string GenerateUniqueSavePath(string targetDir, out string defaultFileName)
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
            defaultFileName = string.Format("Image_{0}.png", timeStamp);
            string fullPath = Path.Combine(targetDir, defaultFileName);

            int counter = 1;
            while (File.Exists(fullPath))
            {
                defaultFileName = string.Format("Image_{0}_{1}.png", timeStamp, counter);
                fullPath = Path.Combine(targetDir, defaultFileName);
                counter++;
            }

            return fullPath;
        }

        /// <summary>
        /// Generates a unique default filename like Text_YYYY-MM-DD_HHMMSS.txt in the target directory for OCR output.
        /// </summary>
        private static string GenerateUniqueTextPath(string targetDir, out string defaultFileName)
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
            defaultFileName = string.Format("Text_{0}.txt", timeStamp);
            string fullPath = Path.Combine(targetDir, defaultFileName);

            int counter = 1;
            while (File.Exists(fullPath))
            {
                defaultFileName = string.Format("Text_{0}_{1}.txt", timeStamp, counter);
                fullPath = Path.Combine(targetDir, defaultFileName);
                counter++;
            }

            return fullPath;
        }

        /// <summary>
        /// Main entry logic when invoked with a target directory path.
        /// Operates silently in the background unless SHIFT is held down for custom save dialog.
        /// If runOcr is true, runs native Windows OCR on the clipboard image and outputs ONLY the transcribed .txt file.
        /// </summary>
        public static void ProcessPasteRequest(string targetDir, bool runOcr = false)
        {
            try
            {
                // Validate directory path
                if (string.IsNullOrWhiteSpace(targetDir) || !Directory.Exists(targetDir))
                {
                    return;
                }

                bool shiftPressed = IsShiftKeyDown();

                Image clipImage = GetClipboardImage();
                if (clipImage == null)
                {
                    // No valid image on clipboard; exit silently as per specifications
                    return;
                }

                using (clipImage)
                {
                    if (runOcr)
                    {
                        using (Bitmap bmp = clipImage as Bitmap ?? new Bitmap(clipImage))
                        {
                            if (shiftPressed)
                            {
                                using (SaveFileDialog dialog = new SaveFileDialog())
                                {
                                    string defaultName;
                                    GenerateUniqueTextPath(targetDir, out defaultName);

                                    dialog.InitialDirectory = targetDir;
                                    dialog.FileName = defaultName;
                                    dialog.Title = "Save Transcribed Text (OCR)";
                                    dialog.Filter = "Text File (*.txt)|*.txt";
                                    dialog.FilterIndex = 1;
                                    dialog.DefaultExt = "txt";
                                    dialog.RestoreDirectory = true;

                                    if (dialog.ShowDialog() == DialogResult.OK)
                                    {
                                        string ocrError;
                                        OcrTranscriber.TranscribeBitmapAndSave(bmp, dialog.FileName, out ocrError);
                                    }
                                }
                            }
                            else
                            {
                                string defaultName;
                                string textPath = GenerateUniqueTextPath(targetDir, out defaultName);
                                string ocrError;
                                OcrTranscriber.TranscribeBitmapAndSave(bmp, textPath, out ocrError);
                            }
                        }
                    }
                    else
                    {
                        if (shiftPressed)
                        {
                            // Open a clean SaveFileDialog if user held SHIFT while clicking context menu
                            using (SaveFileDialog dialog = new SaveFileDialog())
                            {
                                string defaultName;
                                GenerateUniqueSavePath(targetDir, out defaultName);

                                dialog.InitialDirectory = targetDir;
                                dialog.FileName = defaultName;
                                dialog.Title = "Save Clipboard Image";
                                dialog.Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg;*.jpeg)|*.jpg;*.jpeg|Bitmap Image (*.bmp)|*.bmp";
                                dialog.FilterIndex = 1;
                                dialog.RestoreDirectory = true;

                                if (dialog.ShowDialog() == DialogResult.OK)
                                {
                                    string savePath = dialog.FileName;
                                    string ext = Path.GetExtension(savePath).ToLowerInvariant();

                                    ImageFormat format = ImageFormat.Png;
                                    if (ext == ".jpg" || ext == ".jpeg")
                                    {
                                        format = ImageFormat.Jpeg;
                                    }
                                    else if (ext == ".bmp")
                                    {
                                        format = ImageFormat.Bmp;
                                    }

                                    clipImage.Save(savePath, format);
                                }
                            }
                        }
                        else
                        {
                            // Automatic, silent instant save
                            string defaultName;
                            string savePath = GenerateUniqueSavePath(targetDir, out defaultName);
                            clipImage.Save(savePath, ImageFormat.Png);
                        }
                    }
                }
            }
            catch
            {
                // Completely silent operation; any I/O or GDI+ errors are ignored silently without blocking the user
            }
        }
    }
}
