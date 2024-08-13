using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;

namespace zero
{
    class Triangle
    {
        public Triangle(string time, double priceA, double priceB,
             double priceC, System.Drawing.Point точкаА, System.Drawing.Point точкаВ, System.Drawing.Point точкаС,
            int index, float dy, double last)
        {
            this.Time = time;
            this.priceA = priceA;
            this.priceB = priceB;
            this.priceC = priceC;
            this.точкаА = точкаА;
            this.точкаВ = точкаВ;
            this.точкаС = точкаС;                        
            this.Index = index;
            this.oldDy = dy;
            this.Last = last;
        }
        //============================================================================
        private double priceA, priceB, priceC, priceLow, priceHingh;
        //============================================================================
        public string Time
        {
            get; set;
        }
        //============================================================================
        public double PriceLow
        {
            get { return priceLow; }
            set { priceLow = value; }
        }
        //============================================================================
        public double PriceHi
        {
            get { return priceHingh; }
            set { priceHingh = value; }
        }
        //============================================================================
        public double PriceA
        {
            get { return priceA; }
            set { priceA = value; }
        }
        //============================================================================
        public double PriceB
        {
            get { return priceB; }
            set { priceB = value; }
        }
        //============================================================================
        public double PriceC
        {
            get { return priceC; }
            set { priceC = value; }
        }
        //============================================================================
        private System.Drawing.Point точкаА, точкаВ,точкаС, lowЛучь, highЛучь;
        //============================================================================
        public System.Drawing.Point ТочкаA
        {
            get { return точкаА; }
            set { точкаА = value; }
        }
        //============================================================================
        public System.Drawing.Point ТочкаB
        {
            get { return точкаВ; }
            set { точкаВ = value; }
        }
        //============================================================================
        public System.Drawing.Point ТочкаC
        {
            get { return точкаС; }
            set { точкаС = value; }
        }
        //============================================================================
        public System.Drawing.Point LowЛучь
        {
            get { return lowЛучь; }
            set { lowЛучь = value; }
        }
        //============================================================================
        public System.Drawing.Point HighЛучь
        {
            get { return highЛучь; }
            set { highЛучь = value; }
        }
        //============================================================================
        double площадь;
        public double Площадь
        {
            get { return площадь; }
            set { площадь = value; }
        }                
        //============================================================================        
        public int Index
        {
            get; set;            
        }
        //============================================================================
        private string aPointTime;
        public string APointTime
        {
            get { return aPointTime; }
            set { aPointTime = value; }
        }
        //============================================================================
        private string bPointTime;
        public string BPointTime
        {
            get { return bPointTime; }
            set { bPointTime = value; }
        }
        //============================================================================
        private string cPointTime;
        public string CPointTime
        {
            get { return cPointTime; }
            set { cPointTime = value; }
        }
        //============================================================================
        private string лучьLowTime;
        public string ЛучьLowTime
        {
            get { return лучьLowTime; }
            set { лучьLowTime = value; }
        }
        //============================================================================
        private string лучьHiTime;
        public string ЛучьHiTime
        {
            get { return лучьHiTime; }
            set { лучьHiTime = value; }
        }
        //============================================================================
        private int aPointCount;
        public int APointCount
        {
            get { return aPointCount; }
            set { aPointCount = value; }
        }
        //============================================================================
        private int bPointCount;
        public int BPointCount
        {
            get { return bPointCount; }
            set { bPointCount = value; }
        }
        //============================================================================
        private int cPointCount;
        public int CPointCount
        {
            get { return cPointCount; }
            set { cPointCount = value; }
        }
        //============================================================================        
        private int лучьLowCount;
        public int ЛучьLowCount
        {
            get { return лучьLowCount; }
            set { лучьLowCount = value; }
        }
        //============================================================================
        private int лучьHiCount;
        public int ЛучьHiCount
        {
            get { return лучьHiCount; }
            set { лучьHiCount = value; }
        }
        //============================================================================
        private float oldDy;
        public float Dy
        {
            get { return oldDy; }
            set { oldDy = value; }
        }
        //============================================================================
        int intersectionA;
        public int IntersectionA
        {
            get { return intersectionA; }
            set { intersectionA = value; }
        }
        //============================================================================
        int intersectionB;
        public int IntersectionB
        {
            get { return intersectionB; }
            set { intersectionB = value; }
        }
        //============================================================================
        int intersectionC;
        public int IntersectionC
        {
            get { return intersectionC; }
            set { intersectionC = value; }
        }
        //============================================================================
        public string TargetA { get; set; }
        //============================================================================
        public string TargetB { get; set; }
        //============================================================================
        public string TargetC { get; set; }
        //============================================================================
        public string TargetLo { get; set; }
        //============================================================================
        public string TargetHi { get; set; }
        //============================================================================
        public double OperationalSpred { get; set; }
        //============================================================================
        public int LowSum { get; set; }
        //============================================================================
        public int HiSum { get; set; }
        //============================================================================
        public double Last { get; set; }
        //============================================================================
    }
}
