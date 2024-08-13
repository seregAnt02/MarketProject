using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using zero.Models;

namespace zero
{
    class PointMas
    {                                                      
        //===================================
        public PointMas(){}        
        //===================================
        private List<double> priceGep = new List<double> { 0 };
        public List<double> PriceGep
        {
            get { return priceGep; }
            set { priceGep = value; }
        }
        private List<double> spredGep = new List<double> { 0 };
        //===================================
        public List<double> SpredGep
        {
            get { return spredGep; }
            set { spredGep = value; }
        }
        //===================================
        private List<int> countPrice = new List<int> { 0 };
        public List<int> CountPrice
        {
            get { return countPrice; }
            set { countPrice = value; }
        }
        //===================================
        private List<double> bidOfferSum = new List<double> { 0 };
        public List<double> BidOfferSum
        {
            get { return bidOfferSum; }
            set { bidOfferSum = value; }
        }
        //===================================
        private List<double> timePrice = new List<double> { 0 };
        public List<double> TimePrice
        {
            get { return timePrice; }
            set { timePrice = value; }
        }
        //===================================
        private List<List<PointXY>> p1 = new List<List<PointXY>> { new List<PointXY>() };
        public List<List<PointXY>> P1
        {
            get { return p1; }
            set { p1 = value; }
        }        
        //===================================
        private List<int> count = new List<int> { 0 };
        public List<int> Count
        {
            get { return count; }
            set { count = value; }
        }       
        //===================================
        private TimeSpan timeNew; 
        public TimeSpan TimeNew
        {
            get { return timeNew; }
            set { timeNew = value; }
        }
        //===================================
        private TimeSpan timeOld;
        public TimeSpan TimeOld
        {
            get { return timeOld; }
            set { timeOld = value; }
        }
        //===================================
        private List<double> priceAvg = new List<double> { 0 };
        public List<double> PriceAvg
        {
            get { return priceAvg; }
            set { priceAvg = value; }
        }      
    }
}
