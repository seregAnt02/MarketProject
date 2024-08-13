using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zero.Models;

namespace zero.Models
{
    class DealVolume
    {
        public string Time { get; set; }
        public string Buysell { get; set; }
        public double Price { get; set; }        
        public int Sell { get; set; }
        public int Buy { get; set; }
    }
}
