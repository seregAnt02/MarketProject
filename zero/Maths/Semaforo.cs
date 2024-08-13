using zero.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using zero.Brain;
using zero.Models;
//using zero.Tests;
using System.Windows;

namespace zero
{
    class Semaforo : Figure, IPercentSum
    {
        private TXmlConnector txmlConn;
        ArtificialIntelligence brain;
        Analysis analysis;
        public Semaforo(TXmlConnector txmlConn)
        {
            this.txmlConn = txmlConn;
            triangleSum = new TriangleSum(txmlConn);
            brain = new ArtificialIntelligence(txmlConn);
            analysis = new Analysis(txmlConn);
        }                
        //=================================================================================
        List<PointCut> vector = new List<PointCut>();
        AvgPointXYTaboo pointA = new AvgPointXYTaboo(new Point());
        AvgPointXYTaboo pointB = new AvgPointXYTaboo(new Point());
        AvgPointXYTaboo pointC = new AvgPointXYTaboo(new Point());
        public void Driver()
        {
            int index = Triangle.Count == 0 ? 0 : Triangle[Triangle.Count - 1].Index;
            for (int x = index; x < AvgPointXY.P1.Count; x++)
            {                
                pointA.PointXY = AvgPointXY.P1[x].PointXY; // point A             
                //priceA.PriceXY = PointPrice(RdBaza.LowPrice, 0, AvgPointXY.P1[x].PointXY.Y); // price A
                for (int y = index; y < AvgPointXY.P1.Count; y++)
                {
                    pointB.PointXY = AvgPointXY.P2[y].PointXY;// point B
                    //priceB.PriceXY = PointPrice(RdBaza.LowPrice, 0, AvgPointXY.P2[y].PointXY.Y);// price B
                    for (int z = index; z < AvgPointXY.P1.Count; z++)
                    {
                        //p3 = AvgPointXY.P1[z].PointXY;// point в резерве
                        pointC.PointXY = AvgPointXY.P2[z].PointXY;// point C                  
                        //priceC.PriceXY = PointPrice(RdBaza.LowPrice, 0, AvgPointXY.P2[z].PointXY.Y);// price C                              
                        //получаем входные данные                        
                        if (TriangleForm())
                        {
                            ReversBC(y, z);
                            Треугольник(x, y, z);
                        }
                    }
                }
            }
        }
        //============================================================================       
        double reversYB;
        void ReversBC(int y, int z)
        {            
            if (pointB.PointXY.Y > pointC.PointXY.Y)
            {
                reversYB = pointB.PointXY.Y;
                pointB = new AvgPointXYTaboo(new Point(pointB.PointXY.X, pointC.PointXY.Y), null);
                pointC = new AvgPointXYTaboo(new Point(pointC.PointXY.X, reversYB));                   
            }            
        }
        //============================================================================        
        bool TriangleForm()
        {
            bool fl = false;
            if (pointA.PointXY.X < pointB.PointXY.X && pointA.PointXY.X < pointC.PointXY.X)//точка А
            {
                if (pointB.PointXY.Y < pointC.PointXY.Y && pointA.PointXY.Y >
                    pointB.PointXY.Y && pointA.PointXY.Y < pointC.PointXY.Y) fl = true;//флет                    
                if (pointB.PointXY.X < pointC.PointXY.X && pointA.PointXY.Y > 
                    pointB.PointXY.Y && pointA.PointXY.Y > pointC.PointXY.Y) fl = true;//шорт                    
                if (pointB.PointXY.X > pointC.PointXY.X && pointA.PointXY.Y < 
                    pointB.PointXY.Y && pointA.PointXY.Y < pointC.PointXY.Y) fl = true;//лонг                    
            }
            return fl;
        }                     
        //============================================================================                        
        List<Triangle> triangle = new List<Triangle>(); string status;
        void Треугольник(int x, int y, int z)
        {

            status = null; bool fl = false;
            for (int f = 0; f < Triangle.Count; f++)
            {
                if (pointA.PointXY == Triangle[f].ТочкаA)
                {
                    if (pointB.PointXY.Y < Triangle[f].ТочкаB.Y)
                    {
                        Triangle[f].ТочкаB = pointB.PointXY;
                        Triangle[f].PriceB = PointPrice(RdBaza.LowPrice, Triangle[f].ТочкаB.Y);
                        Triangle[f].Time = txmlConn.QuotationsNew.Time; Triangle[f].Last = txmlConn.QuotationsNew.Last; status = "loLo";
                    }
                    if (pointC.PointXY.Y > Triangle[f].ТочкаC.Y)
                    {
                        Triangle[f].ТочкаC = pointC.PointXY;
                        Triangle[f].PriceC = PointPrice(RdBaza.LowPrice, Triangle[f].ТочкаC.Y);
                        Triangle[f].Time = txmlConn.QuotationsNew.Time; Triangle[f].Last = txmlConn.QuotationsNew.Last; status = "hiHi";
                    }
                    if (status != null) Matfak(Triangle[f].ТочкаA, Triangle[f].ТочкаB, Triangle[f].ТочкаC, f);
                    fl = true; txmlConn.RdBazaNew.Semaforos.TriangleSums.LowHiSum(Triangle, f, status);
                }
                else if (txmlConn.Grafs.Barrel.DyLow != Triangle[f].Dy) { Change(f); status = "change"; }
                if (x == Triangle[f].Index) fl = true;
                if (status != null) PointLoHiЛучь(f);
                SumPrice(f); //Brain.TrianlgleMas(f);
                TriangleSums.Sum(Triangle, f, status);
                txmlConn.RdBazaNew.Semaforos.analysis.TargetABC(Triangle, f, status);// технологическое
                txmlConn.RdBazaNew.Semaforos.TriangleSums.TargetSpred(Triangle, f, status);
            }
            if (!fl && x != -1) AddTriangle(x);            
        }
        //============================================================================
        void AddTriangle(int x)
        {
            double priceA = PointPrice(RdBaza.LowPrice, pointA.PointXY.Y);
            double priceB = PointPrice(RdBaza.LowPrice, pointB.PointXY.Y);
            double priceC = PointPrice(RdBaza.LowPrice, pointC.PointXY.Y);
            Triangle.Add(new Triangle(txmlConn.QuotationsNew.Time, priceA, priceB, priceC, pointA.PointXY,
                pointB.PointXY, pointC.PointXY, x, txmlConn.Grafs.Barrel.DyLow, txmlConn.QuotationsNew.Last));
            PointLoHiЛучь(Triangle.Count - 1); Matfak(pointA.PointXY, pointB.PointXY, pointC.PointXY, -2);
            SumPrice(Triangle.Count - 1);
            txmlConn.RdBazaNew.Semaforos.analysis.TargetABC(Triangle, Triangle.Count - 1, status);// технологическое
            //serv.down.Pause();// технологическая пауза !!!
        }
        //============================================================================        
        IndexABC indexABC;
        void SumPrice(int f)
        {
            if (f == 0) indexABC = new IndexABC();
            if (pointA.PointXY.Y == Triangle[f].ТочкаA.Y) // point A 
            {
                Triangle[indexABC.ИндексА].IntersectionA = 0;
                indexABC.intersectionA++; indexABC.ИндексА = f;
                if (indexABC.intersectionA > 1)
                    Triangle[f].IntersectionA = indexABC.intersectionA;                
            }
            if (pointB.PointXY.Y == Triangle[f].ТочкаB.Y) // point B
            {
                Triangle[indexABC.ИндексБ].IntersectionB = 0;
                indexABC.intersectionB++; indexABC.ИндексБ = f;
                if (indexABC.intersectionB > 1)
                    Triangle[f].IntersectionB = indexABC.intersectionB;                
            }
            if (pointC.PointXY.Y == Triangle[f].ТочкаC.Y) // point C
            {
                Triangle[indexABC.ИндексС].IntersectionC = 0;
                indexABC.intersectionC++; indexABC.ИндексС = f;
                if (indexABC.intersectionC > 1)
                    Triangle[f].IntersectionC = indexABC.intersectionC;                
            }
        }
        //============================================================================
        void Change(int f)
        {
            float sum = txmlConn.RdBazaNew.Sum(Triangle[f].Dy, txmlConn.Grafs.Barrel.DyLow);
            Triangle[f].ТочкаA = new Point(Triangle[f].ТочкаA.X, Triangle[f].ТочкаA.Y + sum);
            Triangle[f].ТочкаB = new Point(Triangle[f].ТочкаB.X, Triangle[f].ТочкаB.Y + sum);
            Triangle[f].ТочкаC = new Point(Triangle[f].ТочкаC.X, Triangle[f].ТочкаC.Y + sum);
            Triangle[f].LowЛучь = new Point(Triangle[f].LowЛучь.X, Triangle[f].LowЛучь.Y + sum);
            Triangle[f].HighЛучь = new Point(Triangle[f].HighЛучь.X, Triangle[f].HighЛучь.Y + sum);
            Triangle[f].PriceA = PointPrice(RdBaza.LowPrice, Triangle[f].ТочкаA.Y);
            Triangle[f].PriceB = PointPrice(RdBaza.LowPrice, Triangle[f].ТочкаB.Y);
            Triangle[f].PriceC = PointPrice(RdBaza.LowPrice, Triangle[f].ТочкаC.Y);
            Triangle[f].Dy = txmlConn.Grafs.Barrel.DyLow;
        }        
        //============================================================================                 
        void PointLoHiЛучь(int x)
        {
            double loX = Сумма(Triangle[x].ТочкаA.X, Triangle[x].ТочкаB.X);
            double hiX = Сумма(Triangle[x].ТочкаA.X, Triangle[x].ТочкаC.X);
            double loY = Сумма(Triangle[x].ТочкаA.Y, Triangle[x].ТочкаB.Y);
            double hiY = Сумма(Triangle[x].ТочкаA.Y, Triangle[x].ТочкаC.Y);
            if (Triangle[x].ТочкаA.Y > Triangle[x].ТочкаB.Y)//шорт
            {
                triangle[x].LowЛучь = new Point(Triangle[x].ТочкаB.X + loX, Triangle[x].ТочкаB.Y - loY);
                triangle[x].HighЛучь = new Point(Triangle[x].ТочкаC.X + hiX, (Triangle[x].ТочкаC.Y - hiY));
            }
            if (Triangle[x].ТочкаA.Y > Triangle[x].ТочкаB.Y && Triangle[x].ТочкаA.Y < Triangle[x].ТочкаC.Y)//флет
            {
                triangle[x].LowЛучь = new Point(Triangle[x].ТочкаB.X + loX, Triangle[x].ТочкаB.Y - loY);
                triangle[x].HighЛучь = new Point(Triangle[x].ТочкаC.X + hiX, (Triangle[x].ТочкаC.Y + hiY));
            }
            if (Triangle[x].ТочкаA.Y < Triangle[x].ТочкаB.Y)//лонг
            {
                triangle[x].LowЛучь = new Point(Triangle[x].ТочкаB.X + loX, Triangle[x].ТочкаB.Y + loY);
                triangle[x].HighЛучь  = new Point(Triangle[x].ТочкаC.X + hiX, (Triangle[x].ТочкаC.Y + hiY));
            }
            triangle[x].PriceLow = PointPrice(RdBaza.LowPrice, triangle[x].LowЛучь.Y);
            triangle[x].PriceHi = PointPrice(RdBaza.LowPrice, triangle[x].HighЛучь.Y);            
        }
        //=================================================================================
        public double PointPrice(double startPrice, double endPoint)
        {
            double vol = 0; stepSpred = StepSpred(RdBaza.LowPrice);
            double spred = endPoint * stepSpred;
            vol = startPrice + (spred * startPrice / 100); //vol = startPrice + (spred * serv.RdBaza.Scale);                       
            return Math.Round(vol, txmlConn.SecurityNew.Decimals);
        }
        //=================================================================================
        double stepSpred;
        public double StepSpred(double startPrice)
        {
            stepSpred = txmlConn.RdBazaNew.Prozent(startPrice, startPrice + txmlConn.SecurityNew.Minstep);
            if (stepSpred < 0) stepSpred = -stepSpred;
            return stepSpred;
        }
        //=================================================================================
        float SuM(float vol1, float vol2)
        {
            float vol = 0;
            if (vol1 > vol2) vol = vol1 - vol2;
            if (vol1 < vol2) vol = vol2 - vol1;
            if (vol < 0) vol = -vol;
            return vol;
        }
        //=================================================================================
        void Matfak(Point точкаА, Point точкаВ, Point точкаС, int x)
        {
            double A, B, C, angleA, angleB, angleC;
            double x1 = точкаА.X; double x2 = точкаВ.X; double x3 = точкаС.X;
            double y1 = точкаА.Y; double y2 = точкаВ.Y; double y3 = точкаС.Y;
            A = Math.Round(Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2)), 2);
            B = Math.Round(Math.Sqrt(Math.Pow((x3 - x2), 2) + Math.Pow((y3 - y2), 2)), 2);
            C = Math.Round(Math.Sqrt(Math.Pow((x1 - x3), 2) + Math.Pow((y1 - y3), 2)), 2);
            double p = (A + B + C) / 2;
            double S = Math.Round(Math.Sqrt(p * (p - A) * (p - B) * (p - C)), 2);
            angleA = Math.Round(Math.Asin((2 * S) / (A * B)) * (180 / Math.PI), 2);
            angleB = Math.Round(Math.Asin((2 * S) / (B * C)) * (180 / Math.PI), 2);
            angleC = 180 - angleA - angleB;
            //h_c=(2√(p(p-a)(p-b)(p-c) ))/c
            double h_c = (2 * Math.Sqrt(p * (p - A) * (p - B) * (p - C))) / C;
            if (x == -2) triangle[triangle.Count - 1].Площадь = S; else triangle[x].Площадь = S;
        }        
        //=================================================================================
        void Nook(Point pointA, Point pointB, Point pointC)
        {
            //float a1, b1, c1, a2, b2, c2;
            //LineEquation(pointA, pointB);
            //a1 = A; b1 = B; c1 = C;
            //LineEquation(pointA, pointC);
            //a2 = A; b2 = B; c2 = C;
            // PointF point_a = CrossingPoint(a1, b1, c1, a2, b2, c2);                
            //Треугольник(pointA, pointB, pointC);
        }
        //=================================================================================
        double DistanceBetweenTwoPoints(Point point1, Point point2)
        {
            double dx = point1.X - point2.X;
            double dy = point1.Y - point2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        //=================================================================================    
        //проверка пересечения
        public bool areCrossing(Point p1, Point p2, Point p3, Point p4)
        {
            double v1 = vector_mult(p4.X - p3.X, p4.Y - p3.Y, p1.X - p3.X, p1.Y - p3.Y);
            double v2 = vector_mult(p4.X - p3.X, p4.Y - p3.Y, p2.X - p3.X, p2.Y - p3.Y);
            double v3 = vector_mult(p2.X - p1.X, p2.Y - p1.Y, p3.X - p1.X, p3.Y - p1.Y);
            double v4 = vector_mult(p2.X - p1.X, p2.Y - p1.Y, p4.X - p1.X, p4.Y - p1.Y);
            if ((v1 * v2) < 0 && (v3 * v4) < 0)
                return true;
            return false;
        }
        //=================================================================================
        //векторное произведение
        private double vector_mult(double ax, double ay, double bx, double by)
        {
            return ax * by - bx * ay;
        }
        //=================================================================================
        //поиск точки пересечения
        Point CrossingPoint(float a1, float b1, float c1, float a2, float b2, float c2)
        {

            Point pt = new Point();
            double d = (double)(a1 * b2 - b1 * a2);
            double dx = (double)(-c1 * b2 + b1 * c2);
            double dy = (double)(-a1 * c2 + c1 * a2);
            pt.X = (int)(dx / d);
            pt.Y = (int)(dy / d);
            return pt;
        }
        //=================================================================================
        //построение уравнения прямой
        double A, B, C;//коэффициенты уравнения прямой вида: Ax+By+C=0
        public void LineEquation(Point p1, Point p2)
        {
            A = p2.Y - p1.Y;
            B = p1.X - p2.X;
            C = -p1.X * (p2.Y - p1.Y) + p1.Y * (p2.X - p1.X);
        }
        //=================================================================================
        public override double Сумма(double vol1, double vol2)
        {
            double vol = 0;
            if (vol1 * vol2 != 0)
            {
                if (vol1 > vol2) vol = vol1 - vol2; else vol = vol2 - vol1;
            }
            if (vol1 == 0) vol = vol2;
            return vol;
        }
        //================================================================================  
        TriangleSum triangleSum;
        public TriangleSum TriangleSums
        {
            get { return triangleSum; }
            //set { triangleSum = value; }
        }  
        //================================================================================  
        public List<PointCut> Vector
        {
            get { return vector; }
            set { vector = value; }
        }
        //================================================================================
        public List<Triangle> Triangle
        {
            get { return triangle; }
            set { triangle = value; }
        }
        //=================================================================================
        AvgPointXY avgPointXY = new AvgPointXY();
        public AvgPointXY AvgPointXY
        {
            get { return avgPointXY; }
            set { avgPointXY = value; }
        }
        //=================================================================================
        int interSection;
        public int InterSection
        {
            get { return interSection; }
            set { interSection = value; }
        }
        //=================================================================================
        int trendSum;
        public int TrendSum
        {
            get { return trendSum; }
            set { trendSum = value; }
        }
        //================================================================================
        public double Step
        {
            get { return stepSpred; }
        }
        //================================================================================
        public ArtificialIntelligence Brain
        {
            get { return brain; }
        }
        //================================================================================
        public string StatusLoHi
        {
            get { return status; }
            set { status = value; }
        }
        //================================================================================
    }
}
