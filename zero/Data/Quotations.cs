using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using zero.Abstracts;

namespace zero.Data
{
    class Quotations : BaseVariables//Котировки по инструменту(ам)
    {
        //----------------------------------------------------
        //----------------------------------------------------
        public double Last { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Waprice { get; set; }
        public double Closeprice { get; set; }
        //----------------------------------------------------
        private event NewStringDataTwo onQuotationsEvent;        
        private GroupBox groupBox1;
        private ComboBox comboBox1;
        private Panel panel1;
        private ListView listView;
        private TXmlConnector txlmConn;
        //----------------------------------------------------
        public Quotations() { }
        public Quotations(TXmlConnector txmlConn)
        {
            onQuotationsEvent += Add_Quotations;                        
            txlmConn = txmlConn;
            Collection_control();
        }
        //----------------------------------------------------
        public Quotations(int secId, DateTime date, string time, string seccode,
            string board, string price, int quantity, string brokerref,
            double bid, double offer)
            : base(secId, date, time, seccode, board, price, quantity, brokerref,
                 bid, offer)
        {

        }
        //----------------------------------------------------
        //----------------------------------------------------
        private MainWindow main_window;
        void Collection_control() {
            if (Application.Current != null)
                foreach (Window item in Application.Current.Windows) {
                if (item is MainWindow) {
                    main_window = item as MainWindow;
                    Grid grid = (Grid)item.Content;
                    foreach (var item_control in grid.Children) {                        
                        WrapPanel wrap_panel = item_control is WrapPanel ? item_control as WrapPanel : null;
                        GroupBox group_box = item_control is GroupBox ? item_control as GroupBox : null;
                        ListView list_view = item_control is ListView ? item_control as ListView : null;
                        ComboBox combo_box = item_control is ComboBox ? item_control as ComboBox : null;                        

                        if (group_box != null && group_box.Name == "groupBox1") groupBox1 = group_box;
                        if (wrap_panel != null && wrap_panel.Name == "panel1") panel1 = wrap_panel;
                        if (list_view != null && list_view.Name == "listView") listView = list_view;                                                                        
                        if (combo_box != null) {
                            if (combo_box.Name == "comboBox1") comboBox1 = combo_box;                            
                        }                        
                    }
                }
            }
        }
        public void OnNewQuotationsEvent(string key, string vol)
        {
            onQuotationsEvent(key, vol);
        }
        private void Add_Quotations(string key, string vol) {
            // добавление записи об инструменте            
            string[] ky = key.Split(';'); string[] vl = vol.Split(';');
            txlmConn.InsertBdNew.Add_quotations(ky, vl);
            string quotationV = null, dataV = null, dataK = null, seccodeK = null, quotationK = null;
            for (int x = 0; x < ky.Length; x++) {
                //if (txt_Connect.Text == "online") date = DateTime.Now;
                if (ky[x] == "time") Date = DateTime.Parse(vl[x]);
                if (ky[x] == "time") {
                    Time = DateTime.Parse(vl[x]).ToString("HH:mm:ss");
                    dataK = dataK + ky[x] + ";"; dataV = dataV + vl[x] + ";";
                }
                if (ky[x] == "quotation") { quotationK = ky[x]; quotationV = vl[x]; }
                if (ky[x] == "board") Board = vl[x];
                if (ky[x] == "seccode") Seccode = vl[x];
                comboBox1.Dispatcher.Invoke(() => {
                    if (Seccode == comboBox1.Text) {
                        if (x == 3) { dataK = quotationK + ";" + seccodeK + ";"; dataV = quotationV + ";" + Board + ";" + Seccode + ";"; }
                        if (quotationV != null) { Secid = int.Parse(quotationV); }
                        if (ky[x] == "last") {
                            dataK = dataK + ky[x] + ";"; dataV = dataV + vl[x] + ";";
                            if (vl[x].IndexOf(".") > 0) txlmConn.PointChar = '.';
                            if (txlmConn.PointChar == '.') {
                                Last = double.Parse(vl[x].Replace(".", ","));
                            }
                            else {
                                Last = double.Parse(vl[x]);
                            }
                        }
                        if (ky[x] == "quantity") { dataK = dataK + ky[x] + ";"; dataV = dataV + vl[x] + ";"; }//Quantity = int.Parse(vl[x]);
                        if (ky[x] == "bid") {
                            dataK = dataK + ky[x] + ";"; dataV = dataV + vl[x] + ";";
                            if (vl[x].IndexOf('.') > 0) {
                                Bid = double.Parse(vl[x].Replace(".", ","));
                            }
                            else {
                                Bid = double.Parse(vl[x]);
                            }
                        }
                        if (ky[x] == "offer") {
                            dataK = dataK + ky[x] + ";"; dataV = dataV + vl[x] + ";";
                            if (vl[x].IndexOf('.') > 0) Offer = double.Parse(vl[x].Replace(".", ","));
                            else Offer = double.Parse(vl[x]);
                        }
                        if (ky[x] == "open") {
                            dataK = dataK + ky[x] + ";"; dataV = dataV + vl[x] + ";";
                            if (txlmConn.PointChar == '.') Open = double.Parse(vl[x].Replace(".", ",")); else Open = double.Parse(vl[x]);
                        }
                        if (ky[x] == "high") {
                            dataK = dataK + ky[x] + ";"; dataV = dataV + vl[x] + ";";
                            if (txlmConn.PointChar == '.') High = double.Parse(vl[x].Replace(".", ",")); else High = double.Parse(vl[x]);
                        }
                        if (ky[x] == "low") {
                            dataK = dataK + ky[x] + ";"; dataV = dataV + vl[x] + ";";
                            if (vl[x].IndexOf(".") > 0) txlmConn.PointChar = '.';
                            if (txlmConn.PointChar == '.') Low = double.Parse(vl[x].Replace(".", ",")); else Low = double.Parse(vl[x]);
                        }
                        if (ky[x] == "waprice") {
                            dataK = dataK + ky[x] + ";"; dataV = dataV + vl[x] + ";";
                            if (txlmConn.PointChar == '.') Waprice = double.Parse(vl[x].Replace(".", ",")); else Waprice = double.Parse(vl[x]);
                        }
                        if (ky[x] == "closeprice") {
                            dataK = dataK + ky[x] + ";"; dataV = dataV + vl[x] + ";";
                            if (vl[x].IndexOf('.') > 0) Closeprice = double.Parse(vl[x].Replace(".", ",")); else Closeprice = double.Parse(vl[x]);
                        }
                        // if (ky[x] == "openpositions") { openpositions = vl[x]; rs.openposittions = int.Parse(vl[x]); }        
                    }

                    if (Last != 0) txlmConn.RdBazaNew.Sistema();
                    if (Last != 0 || Open != 0 || Closeprice != 0) txlmConn.RdBazaNew.Orders(Time);

                    if (Last != 0) main_window.table_0_1.Text = Last.ToString();
                    if (Time != null && Time != "") main_window.table_1_0.Text = Time;
                    main_window.table_0_0.Text = Date.ToString();
                    if (Offer != 0 || Bid != 0) {
                        main_window.table_3_9.Text = Bid + "/" + Offer;
                        main_window.table_3_8.Text = Math.Round(txlmConn.RdBazaNew.Spred(Bid, Offer), 2).ToString();
                    }
                });
                
            }                        
        }
    }
}
