using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using zero.Abstracts;
using System.Windows.Controls;
using System.Windows;

namespace zero.Data
{
    //delegate void NewStringData();
    class Trades : BaseVariables   //Сделка(и) клиента
    {                
        public string Currentpos { get; set; }
        public long Orderno { get; set; }
        public string Tradeno { get; set; }
        public string Items { get; set; }
        //----------------------------------------------------                
        private GroupBox groupBox1;
        private event NewStringDataTwo onTradesEvent;
        private Button button3_txt_Connect;
        private Panel panel1;
        private ListView listView;
        private TXmlConnector txmlConn;
        //----------------------------------------------------
        public Trades(TXmlConnector txmlConn)
        {
            onTradesEvent += Add_trades;                        
            this.txmlConn = txmlConn;
            Collection_control();
        }
        //----------------------------------------------------
        public Trades(int secId, DateTime date, string time, string seccode, 
            string board, string price, int quantity, string brokerref, 
            double bid, double offer) :
            base(secId, date, time, seccode, board, price, quantity, brokerref,
                bid, offer)
        {

        }
        //----------------------------------------------------
        MainWindow main_window;
        void Collection_control() {
            foreach (Window item in Application.Current.Windows) {
                if (item is MainWindow) {
                    main_window = item as MainWindow;
                    Grid grid = (Grid)item.Content;
                    foreach (var item_control in grid.Children) {                        
                        WrapPanel wrap_panel = item_control is WrapPanel ? item_control as WrapPanel : null;
                        GroupBox group_box = item_control is GroupBox ? item_control as GroupBox : null;
                        ListView list_view = item_control is ListView ? item_control as ListView : null;
                        Button button = item_control is Button ? item_control as Button : null;                                                
                       
                        if (group_box != null && group_box.Name == "groupBox1") groupBox1 = group_box;
                        if (wrap_panel != null && wrap_panel.Name == "panel1") panel1 = wrap_panel;
                        if (list_view != null && list_view.Name == "listView") listView = list_view;
                        if (button != null) {                            
                            if (button.Name == "button3") button3_txt_Connect = button;
                        }                                                
                    }
                }
            }
        }
        //----------------------------------------------------
        public void OnNewTradesEvent(string key, string vol)
        {
            onTradesEvent(key, vol);
        }        
        private void Add_trades(string key, string vol)
        {

            string[] ky = key.Split(';'); string[] vl = vol.Split(';');                        
            for (int x = 0; x < ky.Length; x++)
            {
                if (button3_txt_Connect.Content.ToString() == "online")
                    Date = DateTime.Now; if (ky[x] == "time" && vl[x] != "") Date = DateTime.Parse(vl[x]);
                if (ky[x] == "time")
                {
                    Time = DateTime.Parse(vl[x]).ToString("HH:mm:ss");
                    main_window.table_3_3.Text = vl[x];
                }
                if (ky[x] == "seccode")
                { Seccode = vl[x]; main_window.table_0_3.Text = vl[x]; }
                if (ky[x] == "buysell")
                { Buysell = vl[x]; main_window.table_1_3.Text = vl[x]; }
                if (ky[x] == "price")
                {
                    if (txmlConn.PointChar == '.') Price = vl[x].Replace(".", ","); else Price = vl[x];
                    main_window.table_2_3.Text = vl[x];
                }
                if (ky[x] == "quantity") { Quantity = int.Parse(vl[x]); main_window.table_3_3.Text = vl[x]; }
                if (ky[x] == "currentpos") Currentpos = vl[x];
                if (ky[x] == "orderno") Orderno = long.Parse(vl[x]);
                //if (ky[x] == "tradeno") tradeno = vl[x];
                if (ky[x] == "items") Items = vl[x];
                if (ky[x] == "brokerref") Brokerref = vl[x];
                if (ky.Length - 1 == x)
                    txmlConn.RdBazaNew.Calculate(Time, Seccode, Buysell, double.Parse(Price), Orderno, Quantity, Brokerref); // сделки системы                
            }
            txmlConn.RdBazaNew.OrdernoQuantity(Orderno, Quantity);
            string transact = txmlConn.RdBazaNew.TransactionShow(Time);// технологическое
            txmlConn.Lb.BeginInvoke(transact + "{ " + txmlConn.QuotationsNew.Last + "/" + txmlConn.QuotationsNew.Bid +
                "/" + txmlConn.QuotationsNew.Offer + " }" + "[ trades ]", "listBox1", null, null);
            string str = "{" + Time + "} " + Seccode + " / " + Buysell + " / " + Price + " / " + Quantity + " / " + "( " + Orderno + " )";
            txmlConn.Lb.BeginInvoke(str + "{ " + txmlConn.QuotationsNew.Last + "/" + txmlConn.QuotationsNew.Bid + 
                "/" + txmlConn.QuotationsNew.Offer + " }", "listBox1", null, null);
        }
        //----------------------------------------------------
        //----------------------------------------------------
    }
}
