using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace ClipboardPaster
{
    public static class OcrTranscriber
    {
        /// <summary>
        /// Transcribes text from the image at imagePath using native Windows 10/11 OCR,
        /// and saves the resulting text into a text file at textPath.
        /// </summary>
        public static bool TranscribeAndSave(string imagePath, string textPath, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                if (!File.Exists(imagePath))
                {
                    errorMessage = string.Format("Image file not found: {0}", imagePath);
                    return false;
                }

                byte[] pngBytes;
                using (Bitmap bmp = new Bitmap(imagePath))
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    pngBytes = ms.ToArray();
                }

                // Create WinRT stream from image bytes
                InMemoryRandomAccessStream winrtStream = new InMemoryRandomAccessStream();
                using (DataWriter writer = new DataWriter(winrtStream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(pngBytes);
                    WaitForResult(writer.StoreAsync());
                    WaitForResult(writer.FlushAsync());
                }

                BitmapDecoder decoder = WaitForResult(BitmapDecoder.CreateAsync(winrtStream));
                SoftwareBitmap softwareBitmap = WaitForResult(decoder.GetSoftwareBitmapAsync());

                var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                if (ocrEngine == null)
                {
                    errorMessage = "Failed to initialize Windows OCR Engine from user profile languages. OCR may not be supported or language pack is missing.";
                    return false;
                }

                OcrResult result = WaitForResult(ocrEngine.RecognizeAsync(softwareBitmap));
                string transcribedText = result != null ? result.Text : string.Empty;

                if (string.IsNullOrWhiteSpace(transcribedText))
                {
                    transcribedText = "[No readable text detected in image by Windows OCR Engine]";
                }

                File.WriteAllText(textPath, transcribedText);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Transcribes text from a System.Drawing.Bitmap directly and writes to textPath.
        /// </summary>
        public static bool TranscribeBitmapAndSave(Bitmap bmp, string textPath, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                byte[] pngBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    pngBytes = ms.ToArray();
                }

                InMemoryRandomAccessStream winrtStream = new InMemoryRandomAccessStream();
                using (DataWriter writer = new DataWriter(winrtStream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(pngBytes);
                    WaitForResult(writer.StoreAsync());
                    WaitForResult(writer.FlushAsync());
                }

                BitmapDecoder decoder = WaitForResult(BitmapDecoder.CreateAsync(winrtStream));
                SoftwareBitmap softwareBitmap = WaitForResult(decoder.GetSoftwareBitmapAsync());

                var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                if (ocrEngine == null)
                {
                    errorMessage = "Failed to initialize Windows OCR Engine.";
                    return false;
                }

                OcrResult result = WaitForResult(ocrEngine.RecognizeAsync(softwareBitmap));
                string transcribedText = result != null ? result.Text : string.Empty;

                if (string.IsNullOrWhiteSpace(transcribedText))
                {
                    transcribedText = "[No readable text detected in image by Windows OCR Engine]";
                }

                File.WriteAllText(textPath, transcribedText);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private static T WaitForResult<T>(IAsyncOperation<T> op)
        {
            while (op.Status == AsyncStatus.Started)
            {
                Thread.Sleep(10);
            }
            if (op.Status == AsyncStatus.Error)
            {
                throw new Exception("WinRT AsyncOperation failed during OCR processing.");
            }
            return op.GetResults();
        }

        private static uint WaitForResult(IAsyncOperation<uint> op)
        {
            while (op.Status == AsyncStatus.Started)
            {
                Thread.Sleep(10);
            }
            if (op.Status == AsyncStatus.Error)
            {
                throw new Exception("WinRT AsyncOperation failed during stream write.");
            }
            return op.GetResults();
        }

        private static bool WaitForResult(IAsyncOperation<bool> op)
        {
            while (op.Status == AsyncStatus.Started)
            {
                Thread.Sleep(10);
            }
            if (op.Status == AsyncStatus.Error)
            {
                throw new Exception("WinRT AsyncOperation failed during stream flush.");
            }
            return op.GetResults();
        }
    }
}
