using System.Collections.Generic;
using zero.Interface;
using zero.Structure;
using zero.Models;


namespace zero
{
    class TriangleSum : ITarget
    {
        private TXmlConnector txmlConn;        
        public TriangleSum(TXmlConnector txmlConn)
        {
            this.txmlConn = txmlConn;           
        }
        //======================================================================
        public bool Gool(double vol, double first, double last, string ss)
        {
            bool fl = false; if (vol * first * last > 0)
            {
                if (first < last) if (first < vol && vol < last) fl = true;
                if (first > last) if (first > vol && vol > last) fl = true;
                if (ss == "=") if (first == vol || last == vol) fl = true;
            }
            return fl;
        }
        public bool Gool(double vol, float first, float last, string ss)
        {
            bool fl = false; if (vol * first * last > 0)
            {
                if (first < last) if (first < vol && vol < last) fl = true;
                if (first > last) if (first > vol && vol > last) fl = true;
                if (ss == "=") if (first == vol || last == vol) fl = true;
            }
            return fl;
        }        
        //======================================================================
        public LoHiSpred LoHiSpred { get; set; }
        public void TargetSpred(List<Triangle> angle, int f, string status)
        {
            if (f == 0) LoHiSpred = new LoHiSpred();
            if (angle[f].TargetLo == "x") LoHiSp(angle, f, angle[f].PriceLow);
            if (angle[f].TargetB == "x") LoHiSp(angle, f, angle[f].PriceB);
            if (angle[f].TargetA == "x") LoHiSp(angle, f, angle[f].PriceA);
            if (angle[f].TargetC == "x") LoHiSp(angle, f, angle[f].PriceC);
            if (angle[f].TargetHi == "x") LoHiSp(angle, f, angle[f].PriceHi);
            if (LoHiSpred != null && f == angle.Count - 1)
                LoHiSpred.Spred = txmlConn.RdBazaNew.Spred(LoHiSpred.LoPrice, LoHiSpred.HiPrice);
        }
        //======================================================================
        int lowSum, hiSum; public int sumLowHi;
        int SumLowHi(List<Triangle> angle, int f)
        {
            int count = 0;
            if (f == 0) { lowSum = 0; hiSum = 0; }
            lowSum += angle[f].LowSum;
            hiSum += angle[f].HiSum;
            if (f == angle.Count - 1 && lowSum + hiSum > 0)
                count = hiSum - lowSum;
            return count;            
        }
        //======================================================================
        void LoHiSp(List<Triangle> angle, int f, double price)
        {            
            if (LoHiSpred.LoPrice * LoHiSpred.HiPrice == 0)
            {
                LoHiSpred.LoPrice = price;
                LoHiSpred.HiPrice = price;
            }
            else
            {
                if (LoHiSpred.LoPrice > price) LoHiSpred.LoPrice = price;
                if (LoHiSpred.HiPrice < price) LoHiSpred.HiPrice = price;
            }           
        }
        //======================================================================
        public void LowHiSum(List<Triangle> angle, int f, string status)
        {
            if (status != null)
            {
                if (status == "loLo") angle[f].LowSum++;
                if (status == "hiHi") angle[f].HiSum++;
            }
        }
        //======================================================================
        public string TargetABC(List<Triangle> угол, int f, string status)//if (угол[x].IntersectionB > 0)
        {
            string support = null;                                                 
            if (Gool(угол[f].PriceLow, priceY1, oldPrice, "="))
            {
                угол[f].ЛучьLowTime = txmlConn.QuotationsNew.Time; угол[f].ЛучьLowCount++;
                support = "PriceLow";
            }
            if (Gool(угол[f].PriceB, priceY1, oldPrice, "="))
            {
                угол[f].BPointTime = txmlConn.QuotationsNew.Time; угол[f].BPointCount++;
                support = "PriceB";
            }
            if (Gool(угол[f].PriceA, priceY1, oldPrice, "="))
            {
                угол[f].APointTime = txmlConn.QuotationsNew.Time; угол[f].APointCount++;
                support = "PriceA";
            }            
            if (Gool(угол[f].PriceC, priceY1, oldPrice, "="))
            {
                угол[f].CPointTime = txmlConn.QuotationsNew.Time; угол[f].CPointCount++;
                support = "PriceC";
            }
            if (Gool(угол[f].PriceHi, priceY1, oldPrice, "="))
            {
                угол[f].ЛучьHiTime = txmlConn.QuotationsNew.Time; угол[f].ЛучьHiCount++;
                support = "PriceHingh";
            }            
            return support;
        }
        //======================================================================
        public string LastLowHi(double support)
        {
            string vol = null; if (support > 0)
            {
                if (priceY1 < support) vol = "S";
                if (priceY1 > support) vol = "B";
            }
            return vol;
        }        
        //======================================================================               
    }
}
