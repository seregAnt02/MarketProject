using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using zero.Models;
using zero.Control_collection;

namespace zero {
    class TotalDeal: Main_window_base {
        private TXmlConnector txmlConn;
        private Series series;
        public TotalDeal(TXmlConnector txmlConn) {
            this.txmlConn = txmlConn;            

            if(Main_window != null) Main_window.tabControl1.SelectionChanged += TabControl1_SelectionChanged;

            /*series = Main_window.chart_name.Series["series_0"];                        
            series.Points.AddXY(0, -50);
            series.Points.AddXY(20, 40);
            series.Points.AddXY(40, 60);
            series.Points.AddXY(60, -90);
            series.Points.AddXY(80, 100);*/
        }
        //----------------------------------------------------------
        //----------------------------------------------------------        
        private void TabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e) {            
            // сделки в графике
            if (Main_window.tabControl1.SelectedIndex == 2) {
                Main_window.chart_name.Series["series_0"].Enabled = false;
                Main_window.chart_name.Series["series_1"].Enabled = true;
                Main_window.chart_name.Series["series_2"].Enabled = true;
                Main_window.chart_name.Series["series_1"].Points.Clear();
                Main_window.chart_name.Series["series_2"].Points.Clear();
                Main_window.canvas_graf.Visibility = Visibility.Hidden;
                foreach (var item in dealVol) {
                    if (item.Sell > 0) {
                        series = Main_window.chart_name.Series["series_2"];
                        series.Points.AddXY(item.Price, -item.Sell);
                    }
                    if (item.Buy > 0) {
                        series = Main_window.chart_name.Series["series_1"];
                        series.Points.AddXY(item.Price, item.Buy);
                    }
                }
            }
            // стакан в графике
            if (Main_window.tabControl1.SelectedIndex == 3) {
                Main_window.chart_name.Series["series_0"].Enabled = false;
                Main_window.chart_name.Series["series_1"].Enabled = true;
                Main_window.chart_name.Series["series_2"].Enabled = true;
                Main_window.canvas_graf.Visibility = Visibility.Hidden;
                Main_window.chart_name.Series["series_1"].Points.Clear();
                Main_window.chart_name.Series["series_2"].Points.Clear();
                foreach (var item in stakanVolume) {
                    if(item.Sell > 0) {
                        series = Main_window.chart_name.Series["series_2"];
                        series.Points.AddXY(item.Price, -item.Sell);
                    }
                    if (item.Buy > 0) {
                        series = Main_window.chart_name.Series["series_1"];
                        series.Points.AddXY(item.Price, item.Buy);
                    }
                }
            }
        }        
        //----------------------------------------------------------
        private List<DealVolume> dealVol = new List<DealVolume> { new DealVolume() };
        public void TotVolDeal(string[] ky, string[] vl) {
            double price = 0; int quantity = 0; string buysell = ""; string time = "";
            for (int x = 0; x < ky.Length; x++) {
                if (ky[x] == "time") time = vl[x];
                if (ky[x] == "price") {
                    if (txmlConn.SecurityNew.Decimals == 0) price = double.Parse(txmlConn.RdBazaNew.RenamePrice(vl[x]));
                    if (txmlConn.SecurityNew.Decimals == 2) price = double.Parse(txmlConn.RdBazaNew.RenamePrice(vl[x].Replace('.', ',')));
                }
                if (ky[x] == "quantity") quantity = int.Parse(vl[x]);
                if (ky[x] == "buysell") buysell = vl[x];
            }
            if (dealVol[0].Price == 0) dealVol[0].Price = price;
            for (int x = 0; x < dealVol.Count; x++) {
                if (dealVol[x].Price == price) {
                    dealVol[x].Time = time;
                    dealVol[x].Buysell = buysell;
                    if (buysell == "S") { dealVol[x].Sell += quantity; txmlConn.RdBazaNew.DeltaDeal -= quantity; }
                    if (buysell == "B") { dealVol[x].Buy += quantity; txmlConn.RdBazaNew.DeltaDeal += quantity; }
                    return;
                }
            }
            if (buysell == "S") {
                txmlConn.RdBazaNew.DeltaDeal -= quantity; dealVol.Add(new DealVolume { Time = time, Buy = 0, Buysell = buysell, Price = price, Sell = quantity });
            }
            if (buysell == "B") {
                txmlConn.RdBazaNew.DeltaDeal += quantity; dealVol.Add(new DealVolume { Time = time, Buy = quantity, Buysell = buysell, Price = price, Sell = 0 });
            }
        }
        //----------------------------------------------------------
        private List<StakanVolume> stakanVolume = new List<StakanVolume> { new StakanVolume() };
        public void TotVolStakan(string[] ky, string[] vl) {
            double price = 0; string source = null;
            int buy = 0, sell = 0;
            for (int x = 0; x < ky.Length; x++) {
                if (ky[x] == "price") {
                    if (txmlConn.SecurityNew.Decimals == 0) price = double.Parse(txmlConn.RdBazaNew.RenamePrice(vl[x]));
                    if (txmlConn.SecurityNew.Decimals == 2) price = double.Parse(txmlConn.RdBazaNew.RenamePrice(vl[x].Replace('.', ',')));
                }
                if (ky[x] == "buy") buy = int.Parse(vl[x]);
                if (ky[x] == "sell") sell = int.Parse(vl[x]);
                if (ky[x] == "source") source = vl[x];
            }
            if (stakanVolume[0].Price == 0) stakanVolume[0].Price = price;
            for (int x = 0; x < stakanVolume.Count; x++) {
                if (stakanVolume[x].Price == price) {
                    stakanVolume[x].Source = source;
                    if (sell > 0) { stakanVolume[x].Sell = sell; txmlConn.RdBazaNew.DeltaSatkan -= sell; }
                    if (sell < 0 && x > 0) { txmlConn.RdBazaNew.DeltaSubtract(stakanVolume[x].Sell); stakanVolume.RemoveAt(x); }
                    if (buy > 0) { stakanVolume[x].Buy = buy; txmlConn.RdBazaNew.DeltaSatkan += buy; }
                    if (buy < 0 && x > 0) { txmlConn.RdBazaNew.DeltaSubtract(stakanVolume[x].Buy); stakanVolume.RemoveAt(x); }
                    return;
                }
            }
            if (sell > 0) {
                txmlConn.RdBazaNew.DeltaSatkan -= sell; stakanVolume.Add(new StakanVolume { Price = price, Buy = 0, Sell = sell, Source = source });
            }
            if (buy > 0) {
                txmlConn.RdBazaNew.DeltaSatkan += buy; stakanVolume.Add(new StakanVolume { Price = price, Buy = buy, Sell = 0, Source = source });
            }
        }
        //----------------------------------------------------------
        //----------------------------------------------------------
    }
}
