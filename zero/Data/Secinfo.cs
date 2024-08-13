using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace zero.Data
{
    class Secinfo
    {
        //public int Decimals { get; set; }
        public string Seccode { get; set; }
        public double Maxprice { get; set; }
        public double Minprice { get; set; }
        //----------------------------------------------------
        //----------------------------------------------------

        private event NewStringDataTwo onSecinfoEvent;
        //private TXmlConnector txmlConn;
        private GroupBox groupBox1;
        private ComboBox comboBox1;
        public Secinfo(TXmlConnector txmlConn)
        {
            onSecinfoEvent += Add_Secinfo;
            Collection_control();
        }
        //----------------------------------------------------
        MainWindow main_window;
        void Collection_control() {
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
        public void OnNewSecinfoEvent(string key, string vol)
        {
            onSecinfoEvent(key, vol);
        }
        //----------------------------------------------------
        private void Add_Secinfo(string key, string vol)
        {
            // обнавление иформаций по инструменту            
                string seccode = null;
                string[] ky = key.Split(';'); string[] vl = vol.Split(';');
                for (int x = 0; x < ky.Length; x++)
                {
                    if (ky[x] == "seccode")
                        seccode = vl[x];
                comboBox1.Dispatcher.Invoke(() => {
                    if (seccode == comboBox1.Text) {
                        //if (ky[x] == "decimals")
                        //Decimals = int.Parse(vl[x]);
                        if (ky[x] == "maxprice")
                            Maxprice = double.Parse(vl[x]);
                        if (ky[x] == "minprice")
                            Minprice = double.Parse(vl[x]);
                    }
                });                    
                }            
        }
        //----------------------------------------------------
        //----------------------------------------------------
    }
}
