using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace zero
{
    class Cash
    {
        public Cash() { }
        public Cash(string buysell, double вход, Point enterPointXY, string status,
            int market, long orderno, long orders, int count, string brokerref)
        {
            this.buysell = buysell;
            this.вход = вход;
            this.EnterPointXY = enterPointXY;
            this.status = status;
            this.market = market;
            this.orderno = orderno;
            this.order = orders;
            this.count = count;
            this.brokerref = brokerref;
        }
        //=====================================================
        private List<Cash> cash = new List<Cash>();

        public List<Cash> Cashs
        {
            get { return cash; }
            set { cash = value; }
        }
        //=====================================================
        private string buysell;
        public string Buysell
        {
            get { return buysell; }
            set { buysell = value; }
        }
        //=====================================================
        private double вход;
        public double Вход
        {
            get { return вход; }
            set { вход = value; }
        }
        //=====================================================
        public Point EnterPointXY { get; set; }
        //=====================================================
        private string status;
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        //=====================================================
        private int market;
        public int Market
        {
            get { return market; }
            set { market = value; }
        }
        //=====================================================
        private long orderno;
        public long Orderno
        {
            get { return orderno; }
            set { orderno = value; }
        }
        //=====================================================
        private long order;
        public long Order
        {
            get { return order; }
            set { order = value; }
        }
        //=====================================================
        private int count;
        public int Quantity
        {
            get { return count; }
            set { count = value; }
        }
        //=====================================================
        private string brokerref;
        public string Brokerref
        {
            get { return brokerref; }
            set { brokerref = value; }
        }
        //=====================================================                
    }
}
