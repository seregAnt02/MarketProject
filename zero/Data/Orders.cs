using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zero.Abstracts;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace zero.Data
{
    class Orders : BaseVariables //Заявка(и) клиента
    {
        //----------------------------------------------------
        //----------------------------------------------------
        public string Stoporder { get; set; }                
        public int Balance { get; set; }
        public long Order { get; set; }
        public string Status { get; set; }
        public long Orderno { get; set; }
        public string Withdrawtime { get; set; }
        public string Comission { get; set; }
        //----------------------------------------------------
        private GroupBox groupBox1;
        private Button button0, button2, buttonConnect;
        private event NewStringDataTwo onOrdersEvent;        
        private Panel panel1;
        private ListView listView;
        private TXmlConnector txmlConn;
        //----------------------------------------------------
        public Orders(TXmlConnector txmlConn)
        {
            Collection_control();
            onOrdersEvent += Add_Orders;                        
            this.txmlConn = txmlConn;
        }
        //----------------------------------------------------        
        //----------------------------------------------------
        MainWindow main_window;
        void Collection_control() {
            if(Application.Current != null)
            foreach (Window item in Application.Current.Windows) {
                if (item is MainWindow) {
                    main_window = item as MainWindow;
                    Grid grid = (Grid)item.Content;
                    foreach (var item_control in grid.Children) {
                        WrapPanel wrap_panel = item_control is WrapPanel ? item_control as WrapPanel : null;
                        GroupBox group_box = item_control is GroupBox ? item_control as GroupBox : null;
                        ListView list_view = item_control is ListView ? item_control as ListView : null;
                        ComboBox combo_box = item_control is ComboBox ? item_control as ComboBox : null;
                        Button button = item_control is Button ? item_control as Button : null;
                        RadioButton radio_button = item_control is RadioButton ? item_control as RadioButton : null;
                        CheckBox check_box = item_control is CheckBox ? item_control as CheckBox : null;
                        TextBox text_box = item_control is TextBox ? item_control as TextBox : null;

                        if (group_box != null && group_box.Name == "groupBox1") groupBox1 = group_box;
                        if (wrap_panel != null && wrap_panel.Name == "panel1") panel1 = wrap_panel;
                        if (list_view != null && list_view.Name == "listView") listView = list_view;
                        if (button != null) {
                            if (button.Name == "button0") button0 = button;                            
                            if (button.Name == "button2") button2 = button;
                            if (button.Name == "button3") buttonConnect = button;
                        }                                                                                                
                    }
                }
            }
        }
        public void OnNewOrdersEvent(string key, string vol)
        {
            onOrdersEvent(key, vol);
        }
        private void Add_Orders(string key, string vol)
        {
            string[] ky = key.Split(';'); string[] vl = vol.Split(';');
            string element = null; for (int x = 0; x < ky.Length; x++)
            {
                if (buttonConnect != null && buttonConnect.Content.ToString() == "online") Date = DateTime.Now; if (ky[x] == "time" && vl[x] != "")
                    Date = DateTime.Parse(vl[x]);
                if (ky[x] == "time") Time = DateTime.Parse(vl[x]).ToString("HH:mm:ss");
                if (ky[x] == "seccode") Seccode = vl[x];
                if (ky[x] == "order") element = ky[x];
                if (ky[x] == "stoporder") element = ky[x];
                if (element == "order")
                {
                    if (ky[x] == "order") Order = long.Parse(vl[x]);
                    if (ky[x] == "orderno") Orderno = long.Parse(vl[x]);
                    if (ky[x] == "price")
                    {
                        if (vl[x].IndexOf(".") > 0) txmlConn.PointChar = '.';
                        if (txmlConn.PointChar == '.') Price = vl[x].Replace(".", ","); else Price = vl[x];
                    }
                    if (ky[x] == "buysell") Buysell = vl[x];
                    if (ky[x] == "quantity") Quantity = int.Parse(vl[x]);
                    if (ky[x] == "brokerref") Brokerref = vl[x];
                    if (ky[x] == "balance") Balance = int.Parse(vl[x]);
                    if (ky[x] == "status")
                    {
                        Status = vl[x];
                        //if (vl[x] == "active") if (vl[x] == "disabled") if (vl[x] == "cancelled") if (vl[x] == "matched")
                    }
                    if (ky[x] == "withdrawtime") Withdrawtime = vl[x];
                    if (ky[x] == "comission") { main_window.table_2_4.Text = vl[x]; }

                }
                //if (element == "stoporder")                
                if (ky.Length - 1 == x)
                {
                    string transaq = null; if (Status != "")
                    {
                        if (Order == txmlConn.RdBazaNew.TransaqString.Order)
                        {
                            txmlConn.RdBazaNew.TransaqString.Вход = double.Parse(Price);
                            txmlConn.RdBazaNew.TransaqString.Status = Status;
                            txmlConn.RdBazaNew.TransaqString.Orderno = Orderno;
                        }
                        if (txmlConn.RdBazaNew.TransaqString.Order == 0 && Status == "active")
                        {
                            txmlConn.RdBazaNew.TransaqString.Order = Order;
                            txmlConn.RdBazaNew.TransaqString.Status = Status;
                        }
                        if (Status == "cancelled" || Status == "removed" ||
                        Status == "denied" || Status == "rejected" || Status == "disabled")
                        {
                            txmlConn.RdBazaNew.TransaqString.Order = 0; txmlConn.RdBazaNew.TransaqString.Market = 0;
                            txmlConn.RdBazaNew.TransaqString.Orderno = 0;
                            if (txmlConn.RdBazaNew.TransaqString.Status != "вход") txmlConn.RdBazaNew.TransaqString.Status = null;
                        }
                    }
                    transaq = txmlConn.RdBazaNew.TransactionShow(Time);// технологическое
                    GridActiv(Buysell, double.Parse(Price), Status);
                    txmlConn.Lb.BeginInvoke(transaq + "{ " + Price + "/" + Bid + "/" + Offer + " }" + "[ orders ]", "listBox1",  null, null);
                }
            }
        }
        void GridActiv(string buysell, double price, string status)
        {
            if (status == "active" && txmlConn.RdBazaNew.Balance != 0)
            {
                main_window.table_1_7.Text = buysell + " => " + price;
                main_window.table_1_7.Foreground = Brushes.Red;
            }
            if (status == "matched" || status == "cancelled" || status == "disabled")
            { main_window.table_1_7.Text = txmlConn.RdBazaNew.Balance.ToString(); main_window.table_1_7.Foreground = Brushes.Black; }
            if (status == "active" && txmlConn.RdBazaNew.TransaqString.Order != 0 && button2 != null)
            { button2.Background = Brushes.Green; }
            else
            {

                if(button2 != null) button2.Background = Brushes.Yellow;
            }
        }
    }
}
