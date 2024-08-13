using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;

namespace zero.Models
{
    class AvgPointXYTaboo
    {        
        public AvgPointXYTaboo(System.Drawing.Point pointXY)
        {
            this.PointXY = pointXY;
        }
        public AvgPointXYTaboo(System.Drawing.Point pointXY, string taboo)
        {
            this.PointXY = pointXY;
            this.Taboo = taboo;
        }
        public System.Drawing.Point PointXY { get; set; }
        public string Taboo { get; set; }
    }
}
