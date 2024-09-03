using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Forms;

namespace zero.Data
{
    class Quotes
    {
        public string Seccode { get; set; }
        //----------------------------------------------------
        //----------------------------------------------------

        private event NewStringDataTwo onQuotesEvent;
        private TXmlConnector txmlConn;        
        private GroupBox groupBox1;
        private ComboBox comboBox1;
        //----------------------------------------------------
        public Quotes(TXmlConnector txmlConn)
        {
            onQuotesEvent += Add_Quotes;
            this.txmlConn = txmlConn;
            Collection_control();
        }
        //----------------------------------------------------
        MainWindow main_window;
        void Collection_control() {
            if (Application.Current != null)
                foreach (Window item in Application.Current.Windows) {
                if (item is MainWindow) {
                    main_window = item as MainWindow;
                    Grid grid = (Grid)item.Content;
                    foreach (var item_control in grid.Children) {                                                
                        GroupBox group_box = item_control is GroupBox ? item_control as GroupBox : null;                        
                        ComboBox combo_box = item_control is ComboBox ? item_control as ComboBox : null;                        
                        

                        if (group_box != null && group_box.Name == "groupBox1") groupBox1 = group_box;                                                                        
                        if (combo_box != null) {
                            if (combo_box.Name == "comboBox1") comboBox1 = combo_box;                            
                        }                        
                    }
                }
            }
        }
        //----------------------------------------------------
        //----------------------------------------------------
        public void OnNewQuotesEvent(string key, string vol)
        {
            onQuotesEvent(key, vol);
        }
        private void Add_Quotes(string key, string vol) {
            string[] ky = key.Split(';'); string[] vl = vol.Split(';');
            txmlConn.InsertBdNew.Add_quotes(ky, vl);
            string seccode = null; for (int x = 0; x < ky.Length; x++) if (ky[x] == "seccode") seccode = vl[x];
            comboBox1.Dispatcher.Invoke(() => { if (comboBox1.Text == seccode) txmlConn.Total_deal.TotVolStakan(ky, vl); });            
        }
    }
}
