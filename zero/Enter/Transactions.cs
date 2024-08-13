using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace zero.Structure
{
    class Transactions
    {
        TXmlConnector txmlConn;
        private CheckBox checkBox1;
        private GroupBox groupBox1;
        private MainWindow main_window;
        public Transactions(TXmlConnector txmlConn)
        {
            this.txmlConn = txmlConn;            
            Collection_control();
        }
        //================================================================================   
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
                        Button button = (Button)item_control;
                        ProgressBar progress_bar = (ProgressBar)item_control;
                        CheckBox check_box = (CheckBox)item_control;
                        Label label = (Label)item_control;
                        ListBox list_box = (ListBox)item_control;
                        TabControl tab_control = (TabControl)item_control;
                        System.Drawing.Image image = (System.Drawing.Image)item_control;
                        TextBox text_box = (TextBox)item_control;
                        RadioButton radio_button = (RadioButton)item_control;

                        if (group_box.Name == "groupBox1") groupBox1 = group_box;                        
                        if (check_box.Name == "checkBox1") checkBox1 = check_box;                                           }
                }
            }
        }
        public void Enter(double vol, int f, System.Drawing.Point enterPointXY)
        {
            //if (serv.RdBaza.rezult == null || serv.RdBaza.rezult.IsCompleted)
            vol = double.Parse(txmlConn.RdBazaNew.RenamePrice(vol.ToString()));//!!!            
            string buysell = null;//serv.RdBaza.Semaforos.TriangleSums.LastLowHi(vol);
            if (Revers(vol, buysell))
            {
                if (txmlConn.RdBazaNew.TransaqString.Order != 0)
                    txmlConn.RdBazaNew.Cancel(txmlConn.QuotationsNew.Time,
                        txmlConn.RdBazaNew.TransaqString.Вход, txmlConn.RdBazaNew.TransaqString.Quantity);
                if (txmlConn.RdBazaNew.TransaqString.Order == 0)
                {
                    //serv.RdBaza.TransaqString.Вход = vol;
                    txmlConn.RdBazaNew.TransaqString.Buysell = buysell;
                    txmlConn.RdBazaNew.TransaqString.Brokerref = Brokerref(); txmlConn.RdBazaNew.TransaqString.Status = "вход";
                    txmlConn.RdBazaNew.TransaqString.Quantity = Lot(); txmlConn.RdBazaNew.TransaqString.Market = -1;
                    txmlConn.RdBazaNew.TransaqString.EnterPointXY = enterPointXY;
                    //if (serv.RdBaza.TransaqString.Status != "analysis")
                      //  serv.lb.BeginInvoke("ордер => " + serv.RdBaza.TransactionShow(Server.time) +
                        //    "{ " + Server.last + "/" + Server.bid + "/" + Server.offer + " }", "listBox1", Color.Blue, null, null);
                }
            }
        }
        //================================================================================
        public string PriceMarket(string price, string buysell)
        {
            if (txmlConn.RdBazaNew.TransaqString.Market == -1)
            {
                if (buysell == "S") price = txmlConn.QuotationsNew.Low.ToString();
                if (buysell == "B") price = txmlConn.QuotationsNew.High.ToString();
            }
            return price;
        }
        //================================================================================
        int Lot()
        {
            //if (RdBaza.Mode == "тест") serv.Free = 13000;
            int lot = 1;//(int)((serv.Free / ((Server.last) / 10)) / 3);
            //if (lot == 0) lot = 1;
            if (txmlConn.RdBazaNew.TransaqString.Cashs.Count > 0)
            {
                int quianty = txmlConn.RdBazaNew.TransaqString.Cashs[0].Quantity;
                if (quianty == lot) lot = lot + lot;
                if (checkBox1.IsChecked.Value || txmlConn.RdBazaNew.ProfStop - txmlConn.RdBazaNew.StopDeal < StopDeal)
                    lot = quianty;
            }
            return lot;
        }
        //================================================================================
        bool Revers(double vol, string buysell)
        {
            bool fl = false;
            if (OpenClose(buysell))
            {
                if (txmlConn.RdBazaNew.TransaqString.Cashs.Count == 0) fl = true;
                for (int x = 0; x < txmlConn.RdBazaNew.TransaqString.Cashs.Count; x++)
                {
                    if (buysell != txmlConn.RdBazaNew.TransaqString.Cashs[x].Buysell) fl = true;
                    if (fl)
                    {
                        if (buysell == txmlConn.RdBazaNew.TransaqString.Cashs[x].Buysell &&
                            vol == txmlConn.RdBazaNew.TransaqString.Вход) fl = false;
                    }
                }
                if (buysell == txmlConn.RdBazaNew.TransaqString.Buysell &&
                            vol == txmlConn.RdBazaNew.TransaqString.Вход) fl = false;
            }            
            return fl;
        }
        //================================================================================
        bool OpenClose(string buysell)
        {
            bool fl = false;
            if (buysell != null && buysell != "")
            {
                if (!checkBox1.IsChecked.Value && txmlConn.RdBazaNew.ProfStop - txmlConn.RdBazaNew.StopDeal >= StopDeal) fl = true;
                if (txmlConn.RdBazaNew.TransaqString.Cashs.Count > 0)
                {
                    //if (serv.checkBox1.Checked || serv.RdBaza.ProfStop - serv.RdBaza.StopDeal < StopDeal) fl = true;       !! раскомментировать             
                }
            }
            return fl;
        }
        //================================================================================            
        int index;
        public string Brokerref()
        {
            string id = null; if (index == 33) index = 0;
            if (txmlConn.RdBazaNew.TransaqString.Status == "вход" && index > 0) id = Abc[index - 1];
            else id = Abc[index++];
            return id;
        }
        //================================================================================                 
        public List<string> Abc
        {
            get
            {
                return new List<string>() { "а","б","в","г","д","е","ж","з","и","к","л","м","н","о","п","р","с",
            "т","у","ф","х","ш","щ","ч","ь","ъ","ц","э","й","ы","ю","ё","я" };
            }
        }
        //================================================================================        
        int stop;
        public int StopDeal   // !! заменить на 1%
        {
            get
            {
                return stop = -1;
            }
            set
            {
                stop = value;
            }
        }   
    }
}
