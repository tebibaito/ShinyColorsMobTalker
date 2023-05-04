using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using Windows.Storage.Streams;
using System.IO;
using OpenCvSharp.Extensions;
using OpenCvSharp;
using Windows.Media.Ocr;
using System.Runtime.InteropServices;

namespace ShinyColorsMobTalker.Models
{
    internal class CommonModel
    {
        private static CommonModel instance;

        // 録画領域矩形情報
        public int x { get; private set; }
        public int y { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }

        public Bitmap capturedImage { get; private set; }


        private TextData currentTextData;

        private OcrEngine ocrEngine;

        private bool isStarted;

        private Mat arrowTemplate;        



        private CommonModel()
        {
            ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
            isStarted = false;

            arrowTemplate = BitmapConverter.ToMat(new Bitmap("arrow.png"));
        }
        

        public static CommonModel GetInstance()
        {
            if(instance == null)
            {
                instance = new CommonModel();
            }
            return instance;
        }

        public void InitScreenShot()
        {
            capturedImage = new Bitmap(width, height);
        }

        public void ScreenShot()
        {
            using(Graphics graphics = Graphics.FromImage(capturedImage))
            {
                graphics.CopyFromScreen(x, y, 0, 0, capturedImage.Size);                
            }
        }


        public void SetLeftTopX(double leftTopX)
        {
            this.x = (int)leftTopX;
        }

        public void SetLeftTopY(double leftTopY)
        {
            this.y = (int)leftTopY;
        }

        public void SetWidth(double width)
        {
            this.width = (int)width;
        }

        public void SetHeight(double height)
        {
            this.height = (int)height;
        }


        private async void MainProc()
        {
            while(true)
            {                
                if(isStarted && capturedImage != null)
                {
                    ScreenShot();
                    Bitmap bottomHalf = capturedImage.Clone(
                        new Rectangle(0, capturedImage.Height / 3 * 2, capturedImage.Width, capturedImage.Height / 3), 
                        capturedImage.PixelFormat);
                    Mat screenMat = BitmapConverter.ToMat(bottomHalf);
                    Mat[] planes;
                    Cv2.Split(screenMat, out planes);
                    double[] rgbMeanArray = { Cv2.Mean(planes[2])[0], Cv2.Mean(planes[1])[0], Cv2.Mean(planes[0])[0] };
                    int maxColorIndex = 0;
                    double maxVal = 0;
                    for(int i = 0; i < rgbMeanArray.Length ; i++)
                    {
                        if(maxVal < rgbMeanArray[i])
                        {
                            maxVal = rgbMeanArray[i];
                            maxColorIndex = i;
                        }
                    }
                    Bitmap binaryImage = ConvertBinayImage(capturedImage);
                    SoftwareBitmap softwareBmp = await GetSoftWareBmp(binaryImage);
                    currentTextData = await OCR(softwareBmp);                    
                    if (MatchArrow(screenMat, arrowTemplate, 0.9))
                    {
                        if((maxColorIndex == 1 || maxColorIndex == 2) && currentTextData.text.Length > 0)
                        {
                            Debug.Print($"speaker:{currentTextData.speaker}, {currentTextData.text}");
                            string query = await VoiceVoxClient.GetQuery(currentTextData.text);
                            VoiceVoxClient.Speek(query);
                        }
                        mouseClick(500);
                    }
                    else
                    {
                        if ((maxColorIndex == 1 || maxColorIndex == 2) && currentTextData.text.Length > 0)
                        {
                            mouseClick(0);
                        }
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        public void StartMainProc()
        {
            Task task = new Task(MainProc);
            task.Start();
        }

        private async Task<SoftwareBitmap> GetSoftWareBmp(Bitmap bmp)
        {
            using(var stream = new InMemoryRandomAccessStream())
            {
                bmp.Save(stream.AsStream(), ImageFormat.Bmp);
                Windows.Graphics.Imaging.BitmapDecoder decorder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
                SoftwareBitmap softwareBmp = await decorder.GetSoftwareBitmapAsync();
                return softwareBmp;
            }
        }


        private Bitmap ConvertBinayImage(Bitmap bmp)
        {
            using(Mat mat = BitmapConverter.ToMat(bmp))
            {
                using(Mat grayMat = mat.CvtColor(ColorConversionCodes.BGR2GRAY))
                {
                    Mat binaryMat = grayMat.Threshold(155.0, 255, ThresholdTypes.Binary);
                    return BitmapConverter.ToBitmap(binaryMat);
                }
            }
        }


        private async Task<TextData> OCR(SoftwareBitmap softwareBmp)
        {
            var result = await ocrEngine.RecognizeAsync(softwareBmp);
            String tmpSpeaker = "";
            String tmpSpeakerText = "";
            foreach (var line in result.Lines.Select((value, index) => new {value, index}))
            {
                if(line.index == 0)
                {
                    tmpSpeaker = line.value.Text;
                }
                else
                {                    
                    tmpSpeakerText += line.value.Text + "。";
                }
            }
            tmpSpeakerText = tmpSpeakerText.Replace(" ", "");
            return new TextData(tmpSpeaker, tmpSpeakerText);
        }

        private bool MatchArrow(Mat target, Mat template, double threshold)
        {
            using (Mat result = new Mat())
            {
                Cv2.MatchTemplate(target, template, result, TemplateMatchModes.CCoeffNormed);

                OpenCvSharp.Point minloc, maxloc;
                double minval, maxval;
                Cv2.MinMaxLoc(result, out minval, out maxval, out minloc, out maxloc);
                if(maxval >= threshold)
                {
                    return true;
                }
            }
            return false;
        }

        public void ToggleProcess()
        {
            isStarted = !isStarted;
        }


        private void mouseClick(int sleepTime)
        {
            Win32Point mousePosition = new Win32Point {
                X = 0,
                Y = 0
                };

            NativeMethods.GetCursorPos (ref mousePosition);
            INPUT[] mouseInputs = new INPUT[]
            {
                new INPUT {
                    type = NativeMethods.INPUT_MOUSE,
                    ui = new INPUT_UNION {
                        mouse = new MOUSEINPUT {
                            dwFlags = NativeMethods.MOUSEEVENTF_LEFTDOWN,
                            dx = mousePosition.X,
                            dy = mousePosition.Y,
                            mouseData = 0,
                            dwExtraInfo = IntPtr.Zero,
                            time = 0
                        }   
                    }
                },
                new INPUT {
                    type = NativeMethods.INPUT_MOUSE,
                    ui = new INPUT_UNION {
                        mouse = new MOUSEINPUT {
                            dwFlags = NativeMethods.MOUSEEVENTF_LEFTUP,
                            dx = mousePosition.X,
                            dy = mousePosition.Y,
                            mouseData = 0,
                            dwExtraInfo = IntPtr.Zero,
                            time = 0
                        }
                    }
                }
            };
            System.Threading.Thread.Sleep(sleepTime);
            NativeMethods.SendInput(2, ref mouseInputs[0], Marshal.SizeOf(mouseInputs[0]));
        }
    }
}
