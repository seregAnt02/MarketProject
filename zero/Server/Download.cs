using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Drawing;
using System.IO;
//using zero.Structure;
using zero.Models;
using zero.Models.DbContext;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace zero
{
    class Download
    {
        //----------------------------------------------------
        //----------------------------------------------------
        string home;     
        private GroupBox groupBox1;
        private Panel panel1;
        private ListView listView;
        private Button button1Down;
        private ProgressBar progressBar1;
        private Button button3_txt_Connect;
        private CheckBox checkBox2;
        private Label label2;        
        private TXmlConnector txmlConn;
        private MainWindow main_window;
        //----------------------------------------------------   
        public Download(TXmlConnector txmlConn)
        {                           
            patokDown = new PatokDown(SqlReader);
            Collection_control();

            button1Down.Click += btТест;
            this.txmlConn = txmlConn;
        }
        //----------------------------------------------------
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
                        System.Windows.Controls.Image image = item_control is System.Windows.Controls.Image ? item_control as System.Windows.Controls.Image : null;
                        WrapPanel wrap_panel = item_control is WrapPanel ? item_control as WrapPanel : null;
                        GroupBox group_box = item_control is GroupBox ? item_control as GroupBox : null;
                        ListView list_view = item_control is ListView ? item_control as ListView : null;
                        ComboBox combo_box = item_control is ComboBox ? item_control as ComboBox : null;
                        Button button = item_control is Button ? item_control as Button : null;
                        RadioButton radio_button = item_control is RadioButton ? item_control as RadioButton : null;
                        CheckBox check_box = item_control is CheckBox ? item_control as CheckBox : null;
                        TextBox text_box = item_control is TextBox ? item_control as TextBox : null;

                        if (progress_bar != null && progress_bar.Name == "progressBar1") progressBar1 = progress_bar;                                                
                        if (label != null) {
                            if (label.Name == "label2") label2 = label;                            
                        }                        
                        if (group_box != null && group_box.Name == "groupBox1") groupBox1 = group_box;
                        if (wrap_panel != null && wrap_panel.Name == "panel1") panel1 = wrap_panel;
                        if (list_view != null && list_view.Name == "listView") listView = list_view;
                        if (button != null) {                            
                            if (button.Name == "button1") button1Down = button;                            
                            if (button.Name == "button3") button3_txt_Connect = button;
                        }                        
                        if (check_box != null && check_box.Name == "checkBox2") checkBox2 = check_box;                        
                    }
                }
            }
        }
        //----------------------------------------------------
        AutoResetEvent wh = new AutoResetEvent(false);
        PatokDown patokDown;
        //----------------------------------------------------
        List<Quotation> DataLinq()
        {
            EFDbContext dbContext = new EFDbContext();            
                //"SELECT * FROM step WHERE time BETWEEN'{0}'AND'{1}'order by id asc" "01.03.2019 10:00:00" "01.03.2019 24:00:00"    
                var dateTime1 = new DateTime(2020, 02, 01, 00, 00, 00);
                //var dateTime2 = new DateTime(2019, 03, 01, 00, 00, 00);            
                List<Quotation> vol = dbContext.Quotations.Where(s => s.Date == dateTime1).ToList();
                //Если контекст создается в коде приложения, не забывайте удалять контекст, когда он больше не требуется.                
                //При работе с длительно существующем контексте учитывайте следующее:
                //Несколько объектов и их ссылок при загрузке в память, может быстро увеличить потребление памяти контекста. Это может вызвать снижение производительности.
                //Контекст не является потокобезопасным, поэтому его не следует использовать совместно в нескольких потоках одновременно выполняют его работу.
                //Если исключение вызывает контекст быть в состоянии неустранимых, всего приложения может завершиться.
                //Вероятность возникновения проблем с параллелизмом возрастает по мере увеличения разрыва между временем запроса и временем обновления данных.
                //dbContext.Dispose();
                return vol;            
        }
        //----------------------------------------------------
        public void SqlEfReader()
        {
            string key = null, vol = null; int progress = 0;
            List<Quotation> data = DataLinq();
            button1Down.Content = "пауза";
            wh.WaitOne(); for (int x = 0; x < data.Count; x++)
            {
                if (IsAlive) wh.WaitOne();                
                if (data[x].Price != null)
                {
                    if (data[x].Buysell != null)
                    {
                        key = null; vol = null;
                        if (data[x].Date != new DateTime().Date) { key = key + "time" + ";"; vol = vol + data[x].Date + ";"; }
                        if (data[x].Seccode != null) { key = key + "seccode" + ";"; vol = vol + data[x].Seccode + ";"; }
                        if (data[x].Board != null) { key = key + "board" + ";"; vol = vol + data[x].Board + ";"; }
                        if (data[x].Price != null) { key = key + "price" + ";"; vol = vol + data[x].Price.TrimEnd() + ";"; }
                        if (data[x].Buysell != null) { key = key + "buysell" + ";"; vol = vol + data[x].Buysell + ";"; }
                        if (data[x].Quantity != null) { key = key + "quantity" + ";"; vol = vol + data[x].Quantity.TrimEnd() + ";"; }
                        txmlConn.AlltradesNew.OnNewAlltradesEvent(key, vol);//сделки
                    }
                    if (data[x].Buy != null || data[x].Sell != null)
                    {
                        key = null; vol = null;
                        if (data[x].Date != new DateTime().Date) { key = key + "time" + ";"; vol = vol + data[x].Date + ";"; }
                        if (data[x].Seccode != null) { key = key + "seccode" + ";"; vol = vol + data[x].Seccode + ";"; }
                        if (data[x].Board != null) { key = key + "board" + ";"; vol = vol + data[x].Board + ";"; }
                        if (data[x].Price != null) { key = key + "price" + ";"; vol = vol + data[x].Price.TrimEnd() + ";"; }
                        if (data[x].Buy != null) { key = key + "buy" + ";"; vol = vol + data[x].Buy.TrimEnd() + ";"; }
                        if (data[x].Sell != null) { key = key + "sell" + ";"; vol = vol + data[x].Sell.TrimEnd() + ";"; }
                        if (data[x].Source != null) { key = key + "source" + ";"; vol = vol + data[x].Source.TrimEnd() + ";"; }
                        if (data[x].Yield != null) { key = key + "yield" + ";"; vol = vol + data[x].Yield.TrimEnd() + ";"; }
                        txmlConn.QuotesNew.OnNewQuotesEvent(key, vol);//стакан
                    }
                }
                if (data[x].Last != null)
                {
                    key = null; vol = null;
                    if (data[x].Date != new DateTime().Date) { key = key + "time" + ";"; vol = vol + data[x].Date + ";"; }
                    if (data[x].Seccode != null) { key = key + "seccode" + ";"; vol = vol + data[x].Seccode + ";"; }
                    if (data[x].Board != null) { key = key + "board" + ";"; vol = vol + data[x].Board + ";"; }
                    if (data[x].Openpositions != null) { key = key + "openpositions" + ";"; vol = vol + data[x].Openpositions.TrimEnd() + ";"; }
                    if (data[x].Bid != null) { key = key + "bid" + ";"; vol = vol + data[x].Bid.TrimEnd() + ";"; }
                    if (data[x].Offer != null) { key = key + "offer" + ";"; vol = vol + data[x].Offer.TrimEnd() + ";"; }
                    if (data[x].High != null) { key = key + "high" + ";"; vol = vol + data[x].High.TrimEnd() + ";"; }
                    if (data[x].Low != null) { key = key + "low" + ";"; vol = vol + data[x].Low.TrimEnd() + ";"; }
                    if (data[x].Closeprice != null) { key = key + "closeprice" + ";"; vol = vol + data[x].Closeprice.TrimEnd() + ";"; }
                    if (data[x].Waprice != null) { key = key + "waprice" + ";"; vol = vol + data[x].Waprice.TrimEnd() + ";"; }
                    if (data[x].Change != null) { key = key + "change" + ";"; vol = vol + data[x].Change.TrimEnd() + ";"; }
                    if (data[x].Status != null) { key = key + "status" + ";"; vol = vol + data[x].Status.TrimEnd() + ";"; }
                    key = key + "last" + ";"; vol = vol + data[x].Last.TrimEnd() + ";";
                    txmlConn.QuotesNew.OnNewQuotesEvent(key, vol);//катировки
                }
                Progress_bar(x, data, progress++);
            }            
            wh.Close(); patokDown.EndInvoke(rezultDown);
        }
        //----------------------------------------------------
        void Progress_bar(int x, List<Quotation> data, int progress)
        {
            progressBar1.Value = progress * 100 / data.Count;
            if (x == data.Count - 1) progressBar1.Value = 0; button1Down.Content = "финиш";
        }
        //----------------------------------------------------
        public void SqlReader()
        {
            //Server.minstep = 0.05; Server.decimals = 2;
            txmlConn.SecurityNew.Minstep = 1; txmlConn.SecurityNew.Decimals = 0;// RNH9
            string dataStr = string.Format("SELECT * FROM step WHERE time BETWEEN'{0}'AND'{1}'order by id asc",
                         "10.01.2019 10:00:00", "10.01.2019 24:00:00");//today_time.AddDays(1) ORDER BY id ASC
            SqlConnection conn = new SqlConnection(@"Data Source=" + home + ";Initial Catalog=baza;User ID=name;Password=paswword;Connection Timeout=160"); 
            SqlDataAdapter adapter = new SqlDataAdapter(dataStr, conn);// создаем адаптер, подготавливаем команду ,подключаем соединение.
            DataSet dataset = new DataSet(); // набор данных
            adapter.SelectCommand.CommandTimeout = 300;
            int stroka = adapter.Fill(dataset);// заполняем набор данных
            conn.Close();// закрываем соединение,каторое нам больше не нужно.
            adapter.Dispose();
            //CashAdd();
            int progress = 0; string key = null, vol = null; int idx = 0; int x = 0;
            button1Down.Content = "пауза";
            wh.WaitOne(); while (stroka != 0 && true)
            {
                if (IsAlive) wh.WaitOne();
                DataRow row = dataset.Tables[0].Rows[x]; object[] column = row.ItemArray;
                if (column[1].ToString() != "") { key = key + "time" + ";"; vol = vol + column[1].ToString().Trim() + ";"; idx = 1; }//rs.date = DateTime.Parse(column[1].ToString().Trim());
                if (column[2].ToString() != "") { key = key + "seccode" + ";"; vol = vol + column[2].ToString().Trim() + ";"; idx = 1; }
                if (column[3].ToString() != "") { key = key + "buysell" + ";"; vol = vol + column[3].ToString().Trim() + ";"; idx = 2; }
                if (column[4].ToString() != "") { key = key + "board" + ";"; vol = vol + column[4].ToString().Trim() + ";"; idx = 1; }
                if (column[5].ToString() != "") { key = key + "decimals" + ";"; vol = vol + column[5].ToString().Trim() + ";"; idx = 4; }
                if (column[6].ToString() != "") { key = key + "minstep" + ";"; vol = vol + column[6].ToString().Trim() + ";"; idx = 4; }
                if (column[7].ToString() != "") { key = key + "lotsize" + ";"; vol = vol + column[7].ToString().Trim() + ";"; idx = 4; }
                if (column[8].ToString() != "") { key = key + "price" + ";"; vol = vol + column[8].ToString().Trim() + ";"; idx = 2; }
                if (column[9].ToString() != "") { key = key + "source" + ";"; vol = vol + column[9].ToString().Trim() + ";"; idx = 3; }
                if (column[10].ToString() != "") { key = key + "yield" + ";"; vol = vol + column[10].ToString().Trim() + ";"; idx = 3; }
                if (column[11].ToString() != "") { key = key + "buy" + ";"; vol = vol + column[11].ToString().Trim() + ";"; idx = 3; }
                if (column[12].ToString() != "") { key = key + "sell" + ";"; vol = vol + column[12].ToString().Trim() + ";"; idx = 3; }
                if (column[13].ToString() != "") { key = key + "quantity" + ";"; vol = vol + column[13].ToString().Trim() + ";"; idx = 2; }
                if (column[14].ToString() != "") { key = key + "last" + ";"; vol = vol + column[14].ToString().Trim() + ";"; idx = 1; }
                if (column[15].ToString() != "") { key = key + "openpositions" + ";"; vol = vol + column[15].ToString().Trim() + ";"; idx = 1; }
                if (column[16].ToString() != "") { key = key + "bid" + ";"; vol = vol + column[16].ToString().Trim() + ";"; idx = 1; }
                if (column[17].ToString() != "") { key = key + "offer" + ";"; vol = vol + column[17].ToString().Trim() + ";"; idx = 1; }
                if (column[18].ToString() != "") { key = key + "high" + ";"; vol = vol + column[18].ToString().Trim() + ";"; idx = 1; }
                if (column[19].ToString() != "") { key = key + "low" + ";"; vol = vol + column[19].ToString().Trim() + ";"; idx = 1; }
                if (column[20].ToString() != "") { key = key + "closeprice" + ";"; vol = vol + column[20].ToString().Trim() + ";"; idx = 1; }
                if (column[21].ToString() != "") { key = key + "waprice" + ";"; vol = vol + column[21].ToString().Trim() + ";"; idx = 1; }
                if (column[22].ToString() != "") { key = key + "change" + ";"; vol = vol + column[22].ToString().Trim() + ";"; idx = 1; }
                if (column[23].ToString() != "") { key = key + "status" + ";"; vol = vol + column[23].ToString().Trim() + ";"; idx = 1; }
                //if (column[24].ToString() != "") { key = key + "active" + ";"; vol = vol + column[24].ToString().Trim() + ";"; idx = 4; }
                // if (column[25].ToString() != "") { key = key + "openinterest" + ";"; vol = vol + column[18].ToString().Trim() + ";"; idx = 2; }                                                                                                         
                if (idx == 1) txmlConn.QuotationsNew.OnNewQuotationsEvent(key, vol);//катировки
                if (idx == 2) txmlConn.AlltradesNew.OnNewAlltradesEvent(key, vol);//сделки
                if (idx == 3) txmlConn.QuotesNew.OnNewQuotesEvent(key, vol);//стакан
                if (idx == 4) txmlConn.SecurityNew.OnNewSecurityEvent(key, vol);//Список инструментов
                progress++; progressBar1.Value = progress * 100 / stroka;
                key = null; vol = null; if (dataset.Tables[0].Rows.Count - 1 == x) break; x++;
            }
            progressBar1.Value = 0; button1Down.Content = "финиш";
            wh.Close(); dataset.Dispose(); patokDown.EndInvoke(rezultDown);
        }
        //----------------------------------------------------
        public void Pause()
        {
            txmlConn.DownloadNew.IsAlive = true;
            button1Down.Content = "пауза";
        }
        //----------------------------------------------------
        IAsyncResult rezultDown;
        private void btТест(object sender, EventArgs e)
        {
            button3_txt_Connect.Visibility =  Visibility.Hidden; button1Down.Background = System.Windows.Media.Brushes.Red;
            if (button1Down.Content.ToString() == "")
            {
                IsAlive = true; button1Down.Content = "загрузка";
                rezultDown = patokDown.BeginInvoke(null, null);
            }
            if (button1Down.Content.ToString() == "цикл" && !IsAlive)
            {
                button1Down.Content = "пауза";
                IsAlive = true; return;
            }
            if (button1Down.Content.ToString() == "пауза")
            {
                checkBox2.Visibility = Visibility.Hidden;
                if (!IsAlive) { IsAlive = true; button1Down.Content = "пауза"; return; }
                if (IsAlive) { button1Down.Content = "цикл"; IsAlive = false; wh.Set(); }
            }
        }
        //----------------------------------------------------   
        public void CashAdd() {
            string reader = StReader();
            if (reader != null && reader != "") {
                string[] mas = reader.Split(';');
                label2.Content = reader;
                txmlConn.RdBazaNew.TransaqString.Cashs.Add(
                new Cash(mas[0], double.Parse(mas[1]), new System.Drawing.Point(), mas[3],
                int.Parse(mas[4]), long.Parse(mas[5]), long.Parse(mas[6]), int.Parse(mas[7]), mas[8]));
                if (mas[0] == "S") main_window.table_0_7.Text = "-" + mas[7];
                else main_window.table_0_7.Text = mas[7];
                main_window.table_1_7.Text = mas[1];
            }
        }
        //----------------------------------------------------
        public void ReaderPointXY()
        {
            string myPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pointXY.tri");
            if (!Directory.Exists(myPath))
            {
                Stream fs = File.OpenRead(myPath);
                string s = null; bool fl = false; using (StreamReader read = new StreamReader(fs))
                {
                    int x = 0; while (true)
                    {
                        s = read.ReadLine();
                        if (s != null)
                        {
                            PointXYAdd(s, ref x);
                            fl = true;
                        }
                        if (s == null) break;
                    }
                    read.Close();
                }
                fs.Close();
                if (fl) txmlConn.Lb.BeginInvoke("{" + DateTime.Now.TimeOfDay + "} " + " загрузка \"pointXY\" завершена", "listBox1", null, null);
            }
        }                
        //----------------------------------------------------
        void PointXYAdd(string s, ref int x)
        {
            string[] mas = s.Split(';');
            if (txmlConn.RdBazaNew.PointXY.PriceGep[0] == 0)
            {
                txmlConn.RdBazaNew.PointXY.PriceGep[0] = double.Parse(mas[0]);
                txmlConn.RdBazaNew.PointXY.P1[0].Add(new PointXY(new System.Drawing.Point(int.Parse(mas[1]), int.Parse(mas[2])),
                    new System.Drawing.Point(int.Parse(mas[3]), int.Parse(mas[4])), null));                
                txmlConn.RdBazaNew.PointXY.Count[0] = int.Parse(mas[5]); 
                txmlConn.RdBazaNew.PointXY.SpredGep[x] = double.Parse(mas[7]); txmlConn.RdBazaNew.PointXY.CountPrice[x] = int.Parse(mas[8]);
                txmlConn.RdBazaNew.PointXY.BidOfferSum[x] = double.Parse(mas[9]); txmlConn.RdBazaNew.PointXY.TimePrice[x] = double.Parse(mas[10]);
                txmlConn.RdBazaNew.PointXY.PriceAvg[x] = double.Parse(mas[11]);
                txmlConn.RdBazaNew.X1 = int.Parse(mas[12]); txmlConn.RdBazaNew.Y1 = int.Parse(mas[13]);
            }
            else
            {
                if (s != "")
                {
                    txmlConn.RdBazaNew.PointXY.PriceGep[x] = double.Parse(mas[0]);
                    txmlConn.RdBazaNew.PointXY.P1[0].Add(new PointXY(new System.Drawing.Point(int.Parse(mas[1]), int.Parse(mas[2])),
                    new System.Drawing.Point(int.Parse(mas[3]), int.Parse(mas[4])), null));
                    txmlConn.RdBazaNew.PointXY.Count[x] = int.Parse(mas[5]); txmlConn.RdBazaNew.PointXY.SpredGep[x] = double.Parse(mas[7]);
                    txmlConn.RdBazaNew.PointXY.CountPrice[x] = int.Parse(mas[8]); txmlConn.RdBazaNew.PointXY.BidOfferSum[x] = double.Parse(mas[9]);
                    txmlConn.RdBazaNew.PointXY.TimePrice[x] = double.Parse(mas[10]); txmlConn.RdBazaNew.PointXY.PriceAvg[x] = double.Parse(mas[11]);
                }
                else
                {
                    txmlConn.RdBazaNew.PointXY.PriceGep.Add(0);
                    txmlConn.RdBazaNew.PointXY.P1[0].Add(new PointXY(new System.Drawing.Point(), new System.Drawing.Point(), null));
                    txmlConn.RdBazaNew.PointXY.Count.Add(0); txmlConn.RdBazaNew.PointXY.SpredGep.Add(0);
                    txmlConn.RdBazaNew.PointXY.CountPrice.Add(0); txmlConn.RdBazaNew.PointXY.BidOfferSum.Add(0);
                    txmlConn.RdBazaNew.PointXY.TimePrice.Add(0); txmlConn.RdBazaNew.PointXY.PriceAvg.Add(0);
                    x++;
                }
            }
        }
        //----------------------------------------------------
        string StReader()
        {
            string myPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cash.tri");
            string str = null; if (!Directory.Exists(myPath))
            {
                Stream fs = File.OpenRead(myPath);
                using (StreamReader read = new StreamReader(fs))
                {
                    while (true)
                    {
                        string s = read.ReadLine();
                        str = str + s;
                        if (s == null) break;
                    }
                    read.Close();
                }
                fs.Close();
            }
            return str;
        }
        //----------------------------------------------------
        public string MachineName
        {
            get { return Environment.MachineName; }
        }
        public string Home
        {
            get { return home; }
            set { home = value; }
        }
        public bool IsAlive { get; set; }
    }
}
