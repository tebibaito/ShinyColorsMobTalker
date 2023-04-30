using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace ShinyColorsMobTalker.Models
{
    internal class CommonModel
    {
        private static CommonModel instance;

        // 録画領域矩形情報
        public double leftTopX { get; private set; }
        public double leftTopY { get; private set; }
        public double width { get; private set; }
        public double height { get; private set; }

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

        public void SetLeftTopX(double leftTopX)
        {
            this.leftTopX = leftTopX;
        }

        public void SetLeftTopY(double leftTopY)
        {
            this.leftTopY = leftTopY;
        }

        public void SetWidth(double width)
        {
            this.width = width;
        }

        public void SetHeight(double height)
        {
            this.height = height;
        }

    }
}
