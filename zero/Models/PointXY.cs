using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace zero.Models
{
    class PointXY
    {
        public PointXY() { }
        public PointXY(Point pointnew, Point pointold, string brokerref)
        {
            this.PointNew = pointnew;
            this.PointOld = pointold;
            this.Brokerref = brokerref;
        }
        public Point PointNew { get; set; }
        public Point PointOld { get; set; }
        public string Brokerref { get; set; }        
    }
}
