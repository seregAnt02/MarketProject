using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using zero.Models;

namespace zero
{
    class AvgPointXY
    {
        //============================================================
        private List<AvgPointXYTaboo> p1 = new List<AvgPointXYTaboo>();
        public List<AvgPointXYTaboo> P1
        {
            get { return p1; }
            set { p1 = value; }
        }
        //============================================================
        private List<AvgPointXYTaboo> p2 = new List<AvgPointXYTaboo>();
        public List<AvgPointXYTaboo> P2
        {
            get { return p2; }
            set { p2 = value; }
        }                        
        //============================================================
        private List<double> timeX = new List<double>();
        public List<double> TimeX
        {
            get { return timeX; }
            set { timeX = value; }
        }        
        //============================================================
        private List<float> oldDy = new List<float>();
        public List<float> OldDy
        {
            get { return oldDy; }
            set { oldDy = value; }
        }
        //============================================================
        private List<float> newDy = new List<float>();
        public List<float> NewdDy
        {
            get { return newDy; }
            set { newDy = value; }
        }
        //============================================================
        private List<double> spredAvgXTime = new List<double>();
        public List<double> SpredAvgXTime
        {
            get { return spredAvgXTime; }
            set { spredAvgXTime = value; }
        }
    }
}
