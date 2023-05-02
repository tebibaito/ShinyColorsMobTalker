using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

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


        private CommonModel()
        {

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
                capturedImage.Save("C:\\Users\\hayat\\Desktop\\test.bmp", ImageFormat.Bmp);
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

    }
}
