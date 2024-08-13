using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace zero.Data
{
    class Positions
    {
        public double Free { get; set; }
        public string Sec_position { get; set; }
        //----------------------------------------------------
        //----------------------------------------------------

        private event NewStringDataTwo onPositionsEvent;
        //private TXmlConnector txmlConn;
        private GroupBox groupBox1;
        private ComboBox comboBox1;
        private Panel panel1;
        private ListView listView;
        private MainWindow main_window;
        TXmlConnector txlmConn;
        public Positions(TXmlConnector txmlConn)
        {
            this.txlmConn = txmlConn;
            onPositionsEvent += Add_Positions;            
        }
        //----------------------------------------------------
        void Collection_control() {
            foreach (Window item in Application.Current.Windows) {
                if (item is MainWindow) {
                    main_window = item as MainWindow;
                    Grid grid = (Grid)item.Content;
                    foreach (var item_control in grid.Children) {
                        WrapPanel wrap_panel = (WrapPanel)item_control;
                        GroupBox group_box = (GroupBox)item_control;
                        ListView list_view = (ListView)item_control;
                        ComboBox combo_box = (ComboBox)item_control;
                        if (wrap_panel.Name == "panel1") panel1 = wrap_panel;
                        //синхронизировать с DataGridView dataGridView2;
                        if (list_view.Name == "listView") listView = list_view;
                        if (group_box.Name == "groupBox1") groupBox1 = group_box;
                        if (combo_box.Name == "comboBox1") comboBox1 = combo_box;                        
                    }
                }
            }
        }
        public void OnNewPositionsEvent(string key, string vol)
        {
            onPositionsEvent(key, vol);
        }
        private void Add_Positions(string key, string vol)
        {
            string[] ky = key.Split(';'); string[] vl = vol.Split(';');
            string element = ""; string register = "";
            for (int x = 0; x < ky.Length; x++)
            {
                if (ky[x] == "sec_position") element = ky[x];
                if (ky[x] == "money_position") element = ky[x];
                if (ky[x] == "forts_position") element = ky[x];
                if (ky[x] == "forts_money") element = ky[x];
                if (ky[x] == "forts_collaterals") element = ky[x];
                if (ky[x] == "spot_limit") element = ky[x];
                if (element == "money_position")
                {
                    if (ky[x] == "register") register = vl[x];
                    if (register == "T0")
                    {
                        if (ky[x] == "saldo") main_window.table_0_2.Text = vl[x];
                    }
                    if (register == "Y1")
                    {
                        if (ky[x] == "saldo") main_window.table_1_2.Text = vl[x];
                    }
                    if (register == "Y2")
                    {
                        if (ky[x] == "saldoin") main_window.table_0_5.Text = vl[x];//Входящий остато
                        if (ky[x] == "saldo") main_window.table_1_5.Text = vl[x]; //Текущее сальдо
                    }
                    //if (ky[x] == "client") dataGridView2.Rows[3].Cells[0].Value = vl[x];
                    if (ky[x] == "bought") main_window.table_2_5.Text = vl[x];//Куплено

                    if (ky[x] == "sold") main_window.table_3_5.Text = vl[x]; //Продано
                }
                if (element == "sec_position")
                {
                    // if (ky[x] == "client") dataGridView2.Rows[3].Cells[0].Value = vl[x];
                    if (ky[x] == "bought") main_window.table_0_6.Text = vl[x];//Куплено
                    if (ky[x] == "sold") main_window.table_1_6.Text = vl[x];//Продано
                    if (ky[x] == "saldoin") main_window.table_2_6.Text = vl[x];//Входящий остато
                    if (ky[x] == "saldo") main_window.table_3_6.Text = vl[x]; //Текущее сальдо
                }
                if (element == "forts_position")
                {
                    if (ky[x] == "totalnet") main_window.table_0_3.Text = vl[x];//Текущая позиция по инструменту
                    if (ky[x] == "startnet") main_window.table_1_3.Text = vl[x];//Входящая позиция по инструмент 
                    //if (ky[x] == "client") dataGridView2.Rows[3].Cells[0].Value = vl[x];
                    if (ky[x] == "openbuys") main_window.table_2_3.Text = vl[x]; //В заявках на покупку
                    if (ky[x] == "opensells") main_window.table_3_3.Text = vl[x]; //В заявках на продажу   
                }
                if (element == "forts_money")
                {
                    if (ky[x] == "current") main_window.table_0_4.Text = vl[x]; //Текущие
                    if (ky[x] == "blocked") main_window.table_1_4.Text = vl[x]; //Заблокировано
                    if (ky[x] == "free")
                    {
                        if (vl[x].IndexOf(".") > 0) txlmConn.PointChar = '.';
                        if (txlmConn.PointChar == '.') Free = double.Parse(vl[x].Replace(".", ","));
                        else Free = double.Parse(vl[x]);
                        main_window.table_2_4.Text = vl[x];
                    } //Свободные
                    if (ky[x] == "varmargin") main_window.table_3_4.Text = vl[x]; //Опер. маржа
                    //if (ky[x] == "client") dataGridView2.Rows[3].Cells[0].Value = vl[x];
                }
            }
        }
    }
}
