using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zero.Models
{
    class SpredProfStop
    {
        private int id;//, countprof, countstop;
        private string buysell, status;
        private double enter, output;
        public SpredProfStop(int id, string buysell, double enter,
            double output, string status)
        {
            this.id = id;
            this.buysell = buysell;
            this.enter = enter;
            this.output = output;
            this.status = status;
        }
        public SpredProfStop() { }
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        public string Buysell
        {
            get { return buysell; }
            set { buysell = value; }
        }
        public double Enter
        {
            get { return enter; }
            set { enter = value; }
        }
        public double Output
        {
            get { return output; }
            set { output = value; }
        }
        public int CountProf
        {
            get; set;
        }
        public int CountStop
        {
            get; set;
        }
        public int CountEnterProf
        {
            get; set;
        }
        public int CountOutputProf
        {
            get; set;
        }
        public int CountEnterStop
        {
            get; set;
        }
        public int CountOutputStop
        {
            get; set;
        }
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
