using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;

namespace zero
{
    class PointCut
    {
        //============================================================
        public PointCut(List<System.Drawing.Point> pointA, List<System.Drawing.Point> pointB,
            int count, double distance)             
        {
            this.pointA = pointA;
            this.pointB = pointB;
            this.count = count;
            this.distance = distance;            
        }
        private List<System.Drawing.Point> pointA,pointB;
        //============================================================
        public List<System.Drawing.Point> PointA
        {
            get { return pointA; }
            set { pointA = value; }
        }
        //============================================================
        public List<System.Drawing.Point> PointB
        {
            get { return pointB;}
            set { pointB = value; }
        }
        //============================================================
        private int count;
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
        //============================================================
        private double distance;
        public double Distance
        {
            get { return distance; }
            set { distance = value; }
        }
    }
}
