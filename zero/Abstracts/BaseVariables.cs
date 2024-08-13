using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zero.Abstracts
{
    abstract class BaseVariables
    {
        public int Secid { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Seccode { get; set; }
        public string Board { get; set; }
        public string Buysell { get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }
        public string Brokerref { get; set; }
        public double Bid { get; set; }
        public double Offer { get; set; }
        //============================================
        public BaseVariables() { }
        public BaseVariables(int secId, DateTime date, string time, string seccode,
            string board, string price, int quantity, string brokerref, double bid,
            double offer)
        {
            Secid = secId;
            Date = date;
            Time = time;
            Seccode = seccode;
            Board = board;
            Price = price;
            Quantity = quantity;
            Brokerref = brokerref;
            Bid = bid;
            Offer = offer;
        }
    }
}
