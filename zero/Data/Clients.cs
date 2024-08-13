using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace zero.Data
{
    class Clients
    {
        public string client { get; set; }        
        private TXmlConnector txmlConn;
        private WrapPanel panel1;
        //private DataGridView dataGridView2;
        private ListView listView;
        private event NewStringDataTwo onClientsEvent;
        private MainWindow main_window;
        //----------------------------------------------------
        //----------------------------------------------------
        public Clients(TXmlConnector txmlConn)
        {
            Collection_control();
            onClientsEvent += Add_Clients;            
            this.txmlConn = txmlConn;
        }
        //----------------------------------------------------
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
                        if (wrap_panel != null && wrap_panel.Name == "panel1") panel1 = wrap_panel;
                        if (list_view != null && list_view.Name == "listView") listView = list_view;                                                
                    }
                }
            }
        }
        //----------------------------------------------------
        public void OnNewClientsEvent(string key, string vol)
        {
            onClientsEvent(key, vol);
        }
        //----------------------------------------------------
        private void Add_Clients(string key, string vol)
        {
            string[] ky = key.Split(';'); string[] vl = vol.Split(';');
            for (int x = 0; x < ky.Length; x++)
            {
                if (ky[x] == "client" && vl[2] == "4")
                {
                    main_window.table_3_0.Dispatcher.Invoke(() => { txmlConn.TransaqNew.client = vl[x]; main_window.table_3_0.Text = txmlConn.TransaqNew.client; });                    
                }
                // !!! дороботать подключено напрямую площадка ММВБ = 1 ,FORTS = 4
            }
        }
        //----------------------------------------------------
        //----------------------------------------------------
    }
}