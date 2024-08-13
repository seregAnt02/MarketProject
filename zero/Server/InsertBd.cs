using System;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using zero.Models.DbContext;

namespace zero
{
    class InsertBd
    {
        private GroupBox groupBox1;
        Quotation quotations;
        private CheckBox checkBox2;
        private Button button3_txt_Connect;
        private Panel panel1;
        private TXmlConnector txmlConn;
        private ListView listView;
        //----------------------------------------------------
        public InsertBd(TXmlConnector txmlConn)
        {            
            patokInsert = new PatokInsert(Connecting);            
            this.txmlConn = txmlConn;
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
                        ProgressBar progress_bar = item_control is ProgressBar ? item_control as ProgressBar : null;
                        Label label = item_control is Label ? item_control as Label : null;
                        ListBox list_box = item_control is ListBox ? item_control as ListBox : null;
                        TabControl tab_control = item_control is TabControl ? item_control as TabControl : null;
                        Image image = item_control is Image ? item_control as Image : null;
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
                            if (button.Name == "button3") button3_txt_Connect = button;
                        }                        
                        if (check_box != null && check_box.Name == "checkBox2") checkBox2 = check_box;                                                
                    }
                }
            }
        }
        //======================================================================
        public void Add_quotations(string[] ky, string[] vl)//катировки
        {
            // добавление записи об инструменте                        
            checkBox2.Dispatcher.Invoke(() => { if (checkBox2.IsChecked.Value) Quotations(ky, vl); });
        }
        //======================================================================
        public void Add_alltrades(string[] ky, string[] vl) //сделки
        {
            checkBox2.Dispatcher.Invoke(() => { if (checkBox2.IsChecked.Value) Alltrades(ky, vl); });            
        }
        //======================================================================
        public void Add_quotes(string[] ky, string[] vl)//стакан
        {
            checkBox2.Dispatcher.Invoke(() => { if (checkBox2.IsChecked.Value) Quotes(ky, vl); });            
        }
        //======================================================================
        public void Add_securities(string[] ky, string[] vl)
        {
            checkBox2.Dispatcher.Invoke(() => { if (checkBox2.IsChecked.Value) Securities(ky, vl); });            
        }
        //======================================================================
        PatokInsert patokInsert;
        void Quotations(string[] key, string[] vol)
        {
            quotations = new Quotation();
            for (int x = 0; x < key.Length; x++)
            {
                if (vol[x] != "")
                {
                    if (key[x] == "seccode") quotations.Seccode = vol[x];
                    if (key[x] == "board") quotations.Board = vol[x];
                    if (key[x] == "quantity") quotations.Quantity = vol[x];
                    if (key[x] == "last") quotations.Last = vol[x];
                    if (key[x] == "openpositions") quotations.Openpositions = vol[x];
                    if (key[x] == "bid") quotations.Bid = vol[x];
                    if (key[x] == "offer") quotations.Offer = vol[x];
                    if (key[x] == "high") quotations.High = vol[x];
                    if (key[x] == "low") quotations.Low = vol[x];
                    if (key[x] == "closeprice") quotations.Closeprice = vol[x];
                    if (key[x] == "waprice") quotations.Waprice = vol[x];
                    if (key[x] == "change") quotations.Change = vol[x];
                }
            }
            if (button3_txt_Connect.Content.ToString() == "online") patokInsert.BeginInvoke(quotations, null, null);
        }
        //======================================================================
        void Securities(string[] key, string[] vol)
        {
            quotations = new Quotation();
            string ky = "", vl = "", seccode = ""; for (int x = 0; x < key.Length; x++)
            {
                if (vol[x] != "")
                {
                    if (key[x] == "active") { ky = key[x]; vl = "'" + vol[x] + "'"; }
                    if (key[x] == "seccode") seccode = vol[x];
                    if (seccode == txmlConn.OrdersNew.Seccode)
                    {
                        if (key[x] == "seccode") { ky = ky + "," + key[x]; vl = vl + ",'" + vol[x] + "'"; }
                        if (key[x] == "board") { ky = ky + "," + key[x]; vl = vl + ",'" + vol[x] + "'"; }
                        if (key[x] == "decimals") { ky = ky + "," + key[x]; vl = vl + "," + vol[x]; }
                        if (key[x] == "minstep") { ky = ky + "," + key[x]; vl = vl + "," + vol[x]; }
                        if (key[x] == "lotsize") { ky = ky + "," + key[x]; vl = vl + "," + vol[x]; }
                    }
                }
            }
            if (seccode == txmlConn.OrdersNew.Seccode && button3_txt_Connect.Content.ToString() == "online")
                patokInsert.BeginInvoke(quotations, null, null);
        }
        //======================================================================
        void Quotes(string[] key, string[] vol)
        {
            quotations = new Quotation();
            for (int x = 0; x < key.Length; x++)
            {
                if (vol[x] != "")
                {
                    if (key[x] == "seccode") quotations.Seccode = vol[x];
                    if (key[x] == "board") quotations.Board = vol[x];
                    if (key[x] == "price") quotations.Price = vol[x];
                    if (key[x] == "source") quotations.Source = vol[x];
                    if (key[x] == "yield") quotations.Yield = vol[x];
                    if (key[x] == "buy") quotations.Buy = vol[x];
                    if (key[x] == "sell") quotations.Sell = vol[x];
                }
            }
            if (button3_txt_Connect.Content.ToString() == "online")
                patokInsert.BeginInvoke(quotations, null, null);
        }
        //======================================================================
        void Alltrades(string[] key, string[] vol)
        {
            quotations = new Quotation();
            for (int x = 0; x < key.Length; x++)
            {
                if (vol[x] != "")
                {
                    if (key[x] == "seccode") quotations.Seccode = vol[x];
                    if (key[x] == "board") quotations.Board = vol[x];
                    if (key[x] == "quantity") quotations.Quantity = vol[x];
                    if (key[x] == "price") quotations.Price = vol[x];
                    if (key[x] == "buysell") quotations.Buysell = vol[x];
                }
            }
            if (button3_txt_Connect.Content.ToString() == "online")
                patokInsert.BeginInvoke(quotations, null, null);
        }
        //======================================================================        
        void Connecting(Quotation model)
        {
            using (EFDbContext dbContext = new EFDbContext())
            {                
                        model.Date = txmlConn.QuotationsNew.Date;
                        dbContext.Quotations.Add(model);
                        //dbContext.Database.Connection.ConnectionString = Soures;
                        //dbContext.Database.Connection.Open();
                        dbContext.SaveChanges();
                //dbContext.Database.Connection.Close();
                main_window.table_0_9.Text = txmlConn.QuotationsNew.Date.TimeOfDay.ToString();                    
                //Если контекст создается в коде приложения, не забывайте удалять контекст, когда он больше не требуется.                
                //При работе с длительно существующем контексте учитывайте следующее:
                //Несколько объектов и их ссылок при загрузке в память, может быстро увеличить потребление памяти контекста. Это может вызвать снижение производительности.
                //Контекст не является потокобезопасным, поэтому его не следует использовать совместно в нескольких потоках одновременно выполняют его работу.
                //Если исключение вызывает контекст быть в состоянии неустранимых, всего приложения может завершиться.
                //Вероятность возникновения проблем с параллелизмом возрастает по мере увеличения разрыва между временем запроса и временем обновления данных.
                dbContext.Dispose();  //не пробовал!!
            }
        }
        //======================================================================
        private string soures;
        public string Soures
        {
            get
            {
                return soures = @"Data Source=" + txmlConn.DownloadNew.Home +
                    ";Initial Catalog=baza;User ID=bazaName;Password=password;Connection Timeout=150";
            }
            set { value = soures; }
        }
    }
}
