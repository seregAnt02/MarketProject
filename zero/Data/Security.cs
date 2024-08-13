using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using zero.Abstracts;

namespace zero.Data
{
    class Security : BaseVariables//// добавление записи об инструменте 
    {
        public int Id { get; set; }        
        public int Decimals { get; set; }
        public double Minstep { get; set; }
        //----------------------------------------------------
        //----------------------------------------------------

        private event NewStringDataTwo onSecurityEvent;        
        private TXmlConnector txmlConn;
        private GroupBox groupBox1;
        private ComboBox comboBox1;
        private Button button3_txt_Connect;
        private WrapPanel panel1;
        private ListView listView;
        //----------------------------------------------------
        public Security(TXmlConnector txmlConn)
        {
            onSecurityEvent += Add_Security;            
            this.txmlConn = txmlConn;            
            Collection_control();
        }
        //----------------------------------------------------
        public Security(int secId, DateTime date, string time, string seccode,
            string board, string price, int quantity, string brokerref,
            double bid, double offer)
            : base(secId, date, time, seccode, board, price, quantity, brokerref,
                 bid, offer)
        {

        }
        //----------------------------------------------------
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
                        ComboBox combo_box = item_control is ComboBox ? item_control as ComboBox : null;
                        Button button = item_control is Button ? item_control as Button : null;                        
                        if (group_box != null && group_box.Name == "groupBox1") groupBox1 = group_box;
                        if (wrap_panel != null && wrap_panel.Name == "panel1") panel1 = wrap_panel;
                        if (list_view != null && list_view.Name == "listView") listView = list_view;
                        if (button != null) {                        
                            if (button.Name == "button3") button3_txt_Connect = button;
                        }                                                
                        if (combo_box != null) {
                            if (combo_box.Name == "comboBox1") comboBox1 = combo_box;                            
                        }                        
                    }
                }
            }
        }
        //----------------------------------------------------
        public void OnNewSecurityEvent(string key, string vol)
        {
            onSecurityEvent(key, vol);
        }
        //----------------------------------------------------
        private void Add_Security(string key, string vol)
        {            
                    // добавление записи об инструменте   
                    string[] ky = key.Split(';'); string[] vl = vol.Split(';');
                    txmlConn.InsertBdNew.Add_securities(ky, vl);
                    string seccode = null;
                    for (int x = 0; x < ky.Length; x++)
                    {
                button3_txt_Connect.Dispatcher.Invoke(() => { if (button3_txt_Connect.Content.ToString() == "online") Date = DateTime.Now; });                        
                        if (ky[x] == "time") Time = DateTime.Parse(vl[x]).ToString("HH:mm:ss");
                        //if (ky[x] == "security") { }
                        //if (ky[x] == "active") { }
                        //if (ky[x] == "board") { }
                        if (ky[x] == "seccode") seccode = vl[x];
                comboBox1.Dispatcher.Invoke(() => {
                    if (comboBox1.Text == seccode) {
                        Seccode = vl[0];
                        if (ky[x] == "decimals") Decimals = int.Parse(vl[x]);
                        if (ky[x] == "minstep") {
                            if (vl[x].IndexOf(".") > 0) txmlConn.PointChar = '.';
                            if (txmlConn.PointChar == '.') Minstep = double.Parse(vl[x].Replace(".", ","));
                            else Minstep = double.Parse(vl[x]);
                            main_window.table_1_9.Text = Minstep.ToString();
                        }
                        //if (ky[x] == "lotsize") lotsize = int.Parse(vl[x]);
                    }
                });                        
                    }                
        }
        //----------------------------------------------------
        //----------------------------------------------------
    }
}
