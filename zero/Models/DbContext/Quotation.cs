using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zero.Models.DbContext
{
    class Quotation
    {
        public int Id { get; set; }  
        public DateTime Date { get; set; }
        public string Seccode { get; set; }
        public string Buysell { get; set; }
        public string Board { get; set; }
        public string Decimals { get; set; }
        public string Minstep { get; set; }
        public string Lotsize { get; set; }
        public string Price { get; set; }
        public string Source { get; set; }
        public string Yield { get; set; }
        public string Buy { get; set; }
        public string Sell { get; set; }
        public string Quantity { get; set; }
        public string Last { get; set; }
        public string Openpositions { get; set; }
        public string Bid { get; set; }
        public string Offer { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Closeprice { get; set; }
        public string Waprice { get; set; }
        public string Change { get; set; }
        public string Status { get; set; }
    }
}
