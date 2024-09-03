using System;
using System.Collections.Generic;
using System.Threading;
using zero.Abstracts;
using zero.Models;
using System.Windows.Controls;
using System.Windows.Media;
using zero.Control_collection;
using System.Windows.Shapes;

namespace zero
{
    class RdBaza : Main_window_base
    {
        delegate void Shut(string key, string vol, string status);                
        //================================================================================                    
        //public Template template;        
        MathSredProfStop mathSredProfStop;
        
        //private ToolStrip toolStrip;
        //private ToolStripButton toolStripButtonDataGrid, toolStripbuttonLine;
        //private ToolStripComboBox toolStripComboBoxSizeMode;
        //private GroupBox groupBox1;        
        private TXmlConnector txmlConn;        
        public RdBaza(TXmlConnector txmlConn)
        {                                    
            this.txmlConn = txmlConn;     
            //template = new Template(serv);
            this.DealPointXY = new DealPointXY(txmlConn);            
            transaqString = new Cash(null, 0, new System.Drawing.Point(), null, 0, 0, 0, 0, null);                                    
            mathSredProfStop = new MathSredProfStop();                                  
            if(Main_window != null) Main_window.tabControl1.SelectionChanged += TabControl1_SelectionChanged;
        }
        //================================================================================
        //================================================================================
        private void TabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(Main_window.tabControl1.SelectedIndex == 0) {
                Line line1 = new Line();
                line1.X1 = 100;
                line1.Y1 = 30;
                line1.X2 = 200;
                line1.Y2 = 250;
                line1.Stroke = Brushes.Red;
                Line line2 = new Line();
                line2.X1 = 100;
                line2.Y1 = 150;
                line2.X2 = 200;
                line2.Y2 = 30;
                line2.Stroke = Brushes.Blue;
                Main_window.canvas_graf.Children.Add(line1);
                Main_window.canvas_graf.Children.Add(line2);
                Main_window.wrap_panel.Visibility = System.Windows.Visibility.Hidden;
                Main_window.canvas_graf.Visibility = System.Windows.Visibility.Visible;
            }
            if (Main_window.tabControl1.SelectedIndex == 1) {
                Main_window.chart_name.Series["series_1"].Points.Clear();
                Main_window.canvas_graf.Visibility = System.Windows.Visibility.Hidden;
                if (pointXY.PriceGep != null) for (int x = 0; x < pointXY.PriceGep.Count; x++) {
                        Main_window.chart_name.Series["series_0"].Enabled = false;
                        Main_window.chart_name.Series["series_2"].Enabled = false;
                        Main_window.chart_name.Series["series_1"].Enabled = true;                                                
                        /*if (tabControl1.SelectedIndex == 5 && timeavg != 0 && pointXY.TimePrice[x] > timeavg) {
                            chart1.Series["Series2"].Points[x].YValues[0] = pointXY.TimePrice[x];                            
                        }*/
                        if (Main_window.tabControl1.SelectedIndex == 1 && pointXY.SpredGep[x] != 0) {
                            Main_window.chart_name.Series["series_1"].Points.AddXY(pointXY.PriceGep[x], 0);                            
                            Main_window.chart_name.Series["series_1"].Points[x].YValues[0] = pointXY.SpredGep[x];
                            Main_window.chart_name.Series["series_1"].Points[x].Color = System.Drawing.Color.Gray;
                        }
                        /*if (tabControl1.SelectedIndex == 6 && bidofferavg != 0 && pointXY.BidOfferSum[x] > bidofferavg) {
                            chart1.Series["Series2"].Points[x].YValues[0] = pointXY.BidOfferSum[x];
                            chart1.Series["Series2"].Points[x].Color = Color.Gray;
                        }*/
                    }

            }
        }
        //================================================================================           
        //================================================================================

        int mus;
        public void Calculate(string time, string seccode, string buysell,
            double price, long orderno, int quantity, string brokerref)
        {
            string str = null;            
            if (Main_window.comboBox1.Text == seccode)
            {
                AddDeal(buysell, price, orderno, 0, brokerref);
                mus = quantity; Delta = 0;
                for (int x = 0; x < TransaqString.Cashs.Count; x++)
                {
                    InsertCount(x, time, buysell, price, orderno, quantity, brokerref);
                    if (mus != 0 && TransaqString.Cashs[x].Buysell != buysell)
                    { Delete(time, buysell, price, orderno, quantity, brokerref, ref x); sumDeal++; }                    
                    if (x != -1)
                    {
                        if (TransaqString.Cashs[x].Buysell == "B") Delta += TransaqString.Cashs[x].Quantity;
                        if (TransaqString.Cashs[x].Buysell == "S") Delta -= TransaqString.Cashs[x].Quantity;
                        Balance = TransaqString.Cashs[x].Вход;
                        str += TransactionCashShow(time, x);
                    }
                    if (x == -1) { str = null; Balance = 0; Delta = 0; }
                }
                Main_window.label2.Dispatcher.Invoke(() => {
                    Main_window.label2.Content = str;
                    Main_window.table_0_7.Text = Delta.ToString();
                    Main_window.table_1_7.Text = Balance.ToString();
                    Main_window.table_0_8.Text = sumDeal + "{" + buySellCount + "}";
                });                    
            }
        }
        //===============================================================================
        void InsertCount(int x, string time, string buysell,
            double price, long orderno, int quantity, string brokerref)
        {
            if (TransaqString.Cashs[x].Buysell == buysell &&
                price == TransaqString.Cashs[x].Вход) TransaqString.Cashs[x].Quantity += quantity;
        }
        //===============================================================================
        void AddDeal(string buysell, double price,
             long orderno, int quantity, string brokerref)
        {
            if (TransaqString.Cashs.Count == 0 || TransaqString.Cashs.Count > 0 &&
                TransaqString.Cashs[0].Buysell == buysell && TransaqString.Cashs[0].Вход != price)//!! проверяет только нулевой ...                
            {                
                TransaqString.Cashs.Add(new Cash(buysell, price, new System.Drawing.Point(), "matched",
                         0, orderno, 0, quantity, brokerref));                 
            }
        }
        //=============================================================================== 
        void Delete(string time, string buysell, double price,
             long orderno, int quantity, string brokerref, ref int x)
        {
            StopLoss(time, TransaqString.Cashs[x].Вход, TransaqString.Cashs[x].Quantity, price, x);
            int sum = TransaqString.Cashs[x].Quantity;
            if (sum - mus == 0)
            {
                mus = 0; TransaqString.Cashs.RemoveAt(x); x--;
            }
            if (mus > 0 && sum - mus > 0)
            {
                sum = sum - mus; mus = 0;
                TransaqString.Cashs[x].Quantity = sum;
            }
            if (sum - mus < 0)
            {
                mus = mus - sum; sum = 0;                
                CashInsert(time, buysell, price, orderno, mus, brokerref,ref x);                
            }
        }
        //================================================================================ 
        void CashInsert(string time, string buysell, double price,
             long orderno, int quantity, string brokerref,ref int x)
        {
            if (TransaqString.Cashs.Count == 1)
            {
                TransaqString.Cashs[x].Buysell = buysell;
                TransaqString.Cashs[x].Вход = price;
                TransaqString.Cashs[x].Orderno = orderno;
                TransaqString.Cashs[x].Quantity = quantity;
                TransaqString.Cashs[x].Brokerref = brokerref;
            }
            else { TransaqString.Cashs.RemoveAt(x); x--; }
        }
        //================================================================================ 
        public void TransactStringAdd(string status, int market, long order)
        {
            TransaqString.Buysell = txmlConn.QuotationsNew.Buysell;
            TransaqString.Вход = double.Parse(txmlConn.QuotationsNew.Price);
            TransaqString.Status = status;
            TransaqString.Market = market;
            TransaqString.Order = order;
            TransaqString.Quantity = txmlConn.QuotationsNew.Quantity;
            TransaqString.Brokerref = txmlConn.QuotationsNew.Brokerref;
        }
        //================================================================================       
        public string TransactionCashShow(string time, int x)
        {
            string str = null;
            str = "{" + time + "} " + TransaqString.Cashs[x].Buysell + ";" + TransaqString.Cashs[x].Вход + ";" +
                 null + ";" + TransaqString.Cashs[x].Status + ";" +
                 TransaqString.Cashs[x].Market + ";" + TransaqString.Cashs[x].Orderno + ";" +
                 TransaqString.Cashs[x].Order + ";" + TransaqString.Cashs[x].Quantity + ";" +
                 TransaqString.Cashs[x].Brokerref + "\r";
            return str;
        }
        public string TransactionShow(string time)
        {
            string str = null;
            str = "{" + time + "} " + TransaqString.Buysell + ";" + TransaqString.Вход + ";" +
                 null + ";" + TransaqString.Status + ";" + TransaqString.Market +
                 ";" + TransaqString.Orderno + ";" + TransaqString.Order + ";" +
                 TransaqString.Quantity + ";" + TransaqString.Brokerref + "\r";
            return str;
        }
        //================================================================================            
        public void Orders(string time)
        {
            string res = null; if (Goal(time, TransaqString.Вход))
            {
                if (TransaqString.Status == "вход" && TransaqString.Order == 0)
                {
                    if (mode == "боевой" && Main_window.button3.Content.ToString() == "online")
                    {
                        res = txmlConn.TransaqNew.NewOrder(); // порядок
                    }
                    string[] idFlag = ParsId(res); TransaqString.Order = long.Parse(idFlag[1]);
                    if (mode == "тест")
                    {
                        if (TransaqString.Market == 0)
                            ServOrder(time, TransaqString.Вход, "active", TransaqString.Quantity);
                        if (TransaqString.Market == -1)
                            ServOrder(time, TransaqString.Вход, "matched", TransaqString.Quantity);
                    }
                }
                else if (mode == "тест" && TransaqString.Status == "active")
                    ServOrder(time, TransaqString.Вход, "matched", TransaqString.Quantity);
            }
        }
        //================================================================================
        public void OrdernoQuantity(long orderno, int quantity)
        {
            if(TransaqString.Orderno == orderno)
            {
                TransaqString.Quantity -= quantity;
                if (TransaqString.Quantity == 0)
                {
                    TransaqString.Order = 0; TransaqString.Orderno = 0; 
                    TransaqString.Brokerref = null; TransaqString.Market = 0;
                    TransaqString.Status = null;
                }
            }
        }
        //================================================================================           
        bool Goal(string timeEnter, double en)
        {
            bool fl = false; if (en != 0) 
            {                
                double bo = BidOff(TransaqString.Buysell);
                if (TransaqString.Status == "вход" && Target(en, txmlConn.QuotationsNew.Last, LastOld(), "=")) fl = true;
                if (TransaqString.Buysell == "S" &&
                    TransaqString.Status == "active" && bo > en) fl = true;
                if (TransaqString.Buysell == "B" && 
                    TransaqString.Status == "active" && bo < en) fl = true;                
            }
            if (transaqString.Market == -1) fl = true;            
            return fl;
        }
        //================================================================================        
        double LastOld()
        {
            double lastActiv = 0;
            if (TransaqString.Buysell == "S") lastActiv = txmlConn.QuotationsNew.High;
            if (TransaqString.Buysell == "B") lastActiv = txmlConn.QuotationsNew.Low;
            return lastActiv;
        }
        //================================================================================    
        double BidOff(string buysell)
        {
            double vol = 0;           
            if (buysell == "B") vol = txmlConn.QuotationsNew.Bid;
            if (buysell == "S") vol = txmlConn.QuotationsNew.Offer;
            return vol;
        }
        //================================================================================          
        public void Cancel(string time, double en, int quantity)
        {
            if (Mode == "боевой" && Main_window.button3.Content.ToString() == "online")
            {
                string res = null;if (TransaqString.Status != null)
                {
                    res = txmlConn.TransaqNew.Cancelorder(time, TransaqString.Order);
                    string[] idFlag = ParsId(res);
                    if (idFlag[0] == "true")
                    {
                        TransaqString.Order = 0; TransaqString.Orderno = 0;
                    }
                }                
            }
            if (Mode == "тест") ServOrder(time, en, "cancelled", quantity);
        }
        //================================================================================            
        public string ServOrder(string time, double en, string status, int quantity) {
            Thread.Sleep(300);// !!!
            string key = null, vol = null, price = null;
            if (TransaqString.Market == -1) {
                if (TransaqString.Buysell == "S") price = txmlConn.QuotationsNew.Bid.ToString();
                if (TransaqString.Buysell == "B") price = txmlConn.QuotationsNew.Offer.ToString();
            }
            else price = RenamePrice(en.ToString());
            key = "time;seccode;order;orderno;price;buysell;quantity;brokerref;status";
            long orderno = TransaqString.Orderno; if (status == "active") {
                string[] idFlag = ParsId(null); //!!!
                orderno = long.Parse(idFlag[1]);
                //status = Nolimit(time, TransaqString.Buysell, TransaqString.Вход, status);
            }
            var comboBox1 = Main_window != null ? Main_window.comboBox1.Text : null;
            vol = time + ";" + comboBox1 + ";" + TransaqString.Order + ";"
                + orderno + ";" + price + ";" + TransaqString.Buysell + ";"
                + quantity + ";" + TransaqString.Brokerref + ";" + status;
            txmlConn.OrdersNew.OnNewOrdersEvent(key, vol);// ордер синхронный метод
            if (status == "matched") txmlConn.TradesNew.OnNewTradesEvent(key, vol);//сделка !!! синхронный метод                                

            return status;
        }
        //================================================================================
        public string Nolimit(string time, string buysell, double open, string status)
        {            
            if (buysell != null)
            {
                if (buysell == "S" && open < txmlConn.QuotationsNew.Offer || buysell == "B" && open > txmlConn.QuotationsNew.Bid) status = "removed";                
            }
            if (status != null) txmlConn.Lb.BeginInvoke(time + " => " + buysell + " " + open + "  " + status, "listBox1",  null, null);
            return status;
        }
        //================================================================================    
        string[] mas;
        public string[] ParsId(string res)
        {
            string order = "0"; int sum = 0; flag = null;
            mas = new string[] { null, "0"};
            if (res != null)
                for (int x = 0; x < res.Length; x++)
                {
                    if (x == 0) order = null;
                    if (sum == 3 && res[x] != '\"') order = order + res[x];
                    if (res[x] == '"') sum++;
                    if (mas[0] != "true" && x > 16 && x < 21) mas[0] = FlagTrue(res[x]);
                }
            if (mas[0] == "true" && order != null) mas[1] = order;
            if (mode == "тест") mas[1] = new Random().Next(1000, 10000).ToString();            
            return mas;
        }
        //================================================================================         
        string flag;
        string FlagTrue(char znak)
        {            
            if (znak == 't' || flag != null) flag += znak;
            return flag;
        }        
        //=================================================================================
        //int avgcount, countsum;
        double lastold, timesum, timeavg, bidoffer, bidofferavg, avgLast;
        public void Sistema()
        {
            double spred = 0; double gep = 0;
            if (lastold * txmlConn.QuotationsNew.Last > 0)
                gep = Spred(lastold, txmlConn.QuotationsNew.Last); if (gep > 0)
            {
                avgLast = double.Parse(RenamePrice(Math.Round((lastold + txmlConn.QuotationsNew.Last)
                    / 2, txmlConn.SecurityNew.Decimals).ToString()));
                spred = Math.Round(gep, 3); LowHi(avgLast);
            }
            pointXY.TimeNew = TimeSpan.Zero; if (txmlConn.QuotationsNew.Time != null)
                pointXY.TimeNew = TimeSpan.Parse(txmlConn.QuotationsNew.Time);
            if (avgLast != 0) for (int x = 0, z = pointXY.PriceGep.Count - 1; x < pointXY.PriceGep.Count; x++, z--)
                {
                    SortList(avgLast, x);                    
                    if (x == 0) { timesum = 0; bidoffer = 0; } //countsum = 0;                   
                    if (pointXY.PriceGep[x] == avgLast)
                    {
                        if (spred < pointXY.SpredGep[x]) pointXY.CountPrice[x] = 0;
                        pointXY.SpredGep[x] += spred;
                        pointXY.CountPrice[x] = pointXY.CountPrice[x] + 1;
                        pointXY.BidOfferSum[x] += Spred(txmlConn.QuotationsNew.Bid, txmlConn.QuotationsNew.Offer);                        
                        if (pointXY.TimeOld.TotalSeconds * pointXY.TimeNew.TotalSeconds != 0)
                        {
                            double vol = Sum(pointXY.TimeNew.TotalSeconds, pointXY.TimeOld.TotalSeconds);
                            pointXY.TimePrice[x] += vol / 100;
                        }                        
                    }
                    Graf(x); 
                    timesum += pointXY.TimePrice[x]; //countsum += pointXY.CountPrice[x];
                    bidoffer += pointXY.BidOfferSum[x];
                }            
            if (pointXY.TimePrice.Count * timesum != 0) timeavg = timesum / pointXY.TimePrice.Count;
            //if (pointXY.CountPrice.Count * countsum != 0) avgcount = countsum / pointXY.CountPrice.Count;
            if (pointXY.BidOfferSum.Count * bidoffer != 0) bidofferavg = bidoffer / pointXY.BidOfferSum.Count;
            if (avgLast > 0 && pointXY.TimeNew.TotalSeconds != 0) pointXY.TimeOld = pointXY.TimeNew;
            if (txmlConn.QuotationsNew.Last != 0) lastold = txmlConn.QuotationsNew.Last;            
                string str = LowPrice + "<=>" + hingPrice;
            Main_window.label4.Content = str + "\r\n" + DeltaSatkan + "\r\n" + DeltaDeal;            
        }
        //================================================================================        
        public string RenamePrice(string price)
        {
            string st = null;
            if (txmlConn.SecurityNew.Minstep != 1)// !!!! доделать
                for (int x = 0; x < price.Length; x++)
                {
                    if (price.Length - 1 == x)
                    {
                        double mus = double.Parse(price[x].ToString());
                        if (mus >= 1 && mus <= 2) { price = st + "0"; st = null; }
                        if (mus > 2 && mus <= 7) { price = st + "5"; st = null; }
                        if (mus > 7 && mus <= 10)
                        {
                            double add = 0;
                            if (txmlConn.SecurityNew.Minstep == 0.05) add = 0.1;
                            if (txmlConn.SecurityNew.Minstep == 1) add = 10;
                            price = (double.Parse(st + "0") + add).ToString();
                            st = null;
                        }
                    }
                    if (x < price.Length) st = st + price[x];
                }
            return price;
        }
        //================================================================================
        void LowHi(double avgLast)
        {
            if (lowPrice * hingPrice == 0) { lowPrice = avgLast; hingPrice = avgLast; }
            if (avgLast < lowPrice) lowPrice = avgLast;
            if (avgLast > hingPrice) hingPrice = avgLast;
        }
        //================================================================================
        double oldLast; int oldX, oldY;
        int y1, x1; 
        void Graf(int x)
        {
            if (pointXY.PriceGep[x] == avgLast)
            {
                X1 += (int)pointXY.TimePrice[x];
                Y1 += (int)(Scale * Prozent(oldLast, avgLast));
                System.Drawing.Point pointOld = new System.Drawing.Point(oldX, oldY), pointNew = new System.Drawing.Point(X1, Y1);
                if (Y1 != 0 && oldX * oldY != 0)
                {
                    pointXY.P1[x].Add(new PointXY(pointNew, pointOld, null));                                                            
                }
                Pointcount(Y1, x);
                DealPointXY.InsertDealPointXY(pointXY, x);                                
                oldLast = avgLast; oldX = X1; oldY = Y1;
            }            
            //txmlConn.Grafs.Barrel.Barrels(status, Y1);
            Kanal(x); KanalSort(x, pointXY.PriceGep[x]); Avgpoint(x);
        }                
        //================================================================================    
        public double Prozent(double first, double last)
        {
            double vol = 0;
            if (first * last != 0)
            {
                if (last > first && last != 0) vol = (float)(100 - (first / last * 100));
                if (last < first && last != 0) vol = -(float)(100 - (last / first * 100));
            }
            return vol;
        }
        //================================================================================    
        double EnMatched(string buysell, double en)
        {
            double vol = 0;
            if (buysell == "S") vol = txmlConn.QuotationsNew.Bid;
            if (buysell == "B") vol = txmlConn.QuotationsNew.Offer;
            return vol;
        }                
        //=================================================================================
        double MinusPlus(double vol)
        {
            if (vol < 0) vol = -vol;
            return vol;
        }
        //================================================================================    
        string ss;
        void SortList(double lastavg, int x)
        {
            if (x == 0) ss = null;
            if (x == 0 && lastavg > pointXY.PriceGep[x])
            {                
                if (pointXY.PriceGep[x] == 0) pointXY.PriceGep[x] = lastavg;
                else
                {
                    pointXY.PriceGep.Insert(x, lastavg); pointXY.SpredGep.Insert(x, 0);
                    pointXY.CountPrice.Insert(x, 0); pointXY.TimePrice.Insert(x, 0); pointXY.BidOfferSum.Insert(x, 0);
                    pointXY.P1.Insert(x, new List<PointXY>()); pointXY.Count.Insert(x, 0);                    
                    pointXY.PriceAvg.Insert(x, 0); ss = "yes";
                }
            }
            if (x < pointXY.PriceGep.Count - 1 && ss != "yes" && Target(lastavg, pointXY.PriceGep[x], pointXY.PriceGep[x + 1], null))
            {
                pointXY.PriceGep.Insert(x + 1, lastavg); pointXY.SpredGep.Insert(x + 1, 0);
                pointXY.CountPrice.Insert(x + 1, 0); pointXY.BidOfferSum.Insert(x + 1, 0); pointXY.TimePrice.Insert(x + 1, 0);
                pointXY.P1.Insert(x + 1, new List<PointXY>()); pointXY.Count.Insert(x + 1, 0);                
                pointXY.PriceAvg.Insert(x + 1, 0); ss = "yes";
            }
            if (lastavg == pointXY.PriceGep[x]) ss = "yes";
            if (ss == null && pointXY.PriceGep.Count - 1 == x)
            {                
                pointXY.PriceGep.Add(lastavg); pointXY.SpredGep.Add(0); pointXY.CountPrice.Add(0);
                pointXY.TimePrice.Add(0); pointXY.BidOfferSum.Add(0);
                pointXY.P1.Add(new List<PointXY>()); pointXY.Count.Add(0);                
                pointXY.PriceAvg.Add(0);                
            }

        }                             
        //================================================================================
        int deltaavgpoint; double copydeltaavgpoint;
        List<double> kanal;                
        void Pointcount(double y1, int x)
        {
            int target = 0; for (int i = 0; i < pointXY.P1[x].Count; i++)
            {
                if (Target(y1, pointXY.P1[x][i].PointNew.Y, pointXY.P1[x][i].PointOld.Y, null)) target++;
                if (target != 0 && i == pointXY.P1[x].Count - 1) pointXY.Count[x] = target;
            }
        }
        //================================================================================
        bool Target(double vol, double price1, double price2, string ss)
        {
            bool fl = false;
            if (price1 < price2) if (price1 < vol && vol < price2) fl = true;
            if (price1 > price2) if (price1 > vol && vol > price2) fl = true;
            if (ss == "=") if (price1 == vol || price2 == vol) fl = true;
            return fl;
        }              
        //================================================================================            
        int sumcount, count, onethird; double avgsum;
        void Avgpoint(int x)
        {
            if (x == 0) { sumcount = 0; avgsum = 0; count = 0; }
            int vol = pointXY.Count[x]; if (vol < 0) vol = -vol; if (vol != 0)
            {
                sumcount += vol; count++; avgsum += pointXY.PriceGep[x];
            }
            if (vol > onethird) onethird = vol;
            if (x == pointXY.P1.Count - 1)
            {
                if (sumcount != 0) avgpointcount = sumcount / count;
                if(avgsum != 0) avgpoint = Math.Round(avgsum / count, 
                    txmlConn.SecurityNew.Decimals);
                if (onethird != 0) onethirdpoint = (onethird / 2) + (onethird / 3);
                if (copydeltaavgpoint != 0)
                {
                    if (avgpoint < copydeltaavgpoint) { if (deltaavgpoint > 0) deltaavgpoint = 0; deltaavgpoint--; }
                    if (avgpoint > copydeltaavgpoint) { if (deltaavgpoint < 0) deltaavgpoint = 0; deltaavgpoint++; }
                }
                copydeltaavgpoint = avgpoint;
            }
        }
        //================================================================================    
        int index;
        void Kanal(int x)
        {
            if (x == 0) index = 0;
            if (x - index <= 1)
            {
                if (pointXY.Count[x] > avgpointcount)
                {
                    if (pointXY.Count[x] != 0) pointXY.Count[x] = -pointXY.Count[x];
                }
                index = x;
            }
            if (Flkanal(pointXY.Count, x)) pointXY.Count[x] = -pointXY.Count[x];
        }
        //================================================================================    
        int indexsort; double avgreadkanal;
        void KanalSort(int x, double last)
        {
            if (x == 0) { kanal = new List<double>(); indexsort = 0; }
            if (pointXY.Count[x] < 0)
            {
                if (kanal.Count > 0)
                {
                    if (last > kanal[0] || indexsort == kanal.Count - 1 && last > kanal[0]) { kanal.Insert(0, last); indexsort++; }
                    if (last < kanal[kanal.Count - 1]) { kanal.Add(last); indexsort++; }
                    if (kanal.Count > 1 && Target(last, kanal[indexsort - 1], kanal[indexsort], null)) { kanal.Insert(indexsort, last); indexsort++; }
                }
                if (kanal.Count == 0) kanal.Add(last);
            }
            avgreadkanal = KanalAvg(indexsort, x);
        }
        //================================================================================    
        bool Flkanal(List<int> pointcount, int x)
        {
            bool fl = false;
            if (pointcount[x] < 0 && x - 1 >= 0 && pointcount[x - 1] >= 0 && pointcount[x - 1] < avgpointcount &&
                  x + 1 <= pointcount.Count - 1 && pointcount[x + 1] >= 0 && pointcount[x + 1] < avgpointcount) fl = true;
            if (x - 1 < 0 && pointcount[x] < 0) fl = true;
            return fl;
        }
        //================================================================================    
        double sumreadkanal; int countreadkanal;
        double KanalAvg(int idx, int x)
        {
            if (x == 0) { sumreadkanal = 0; countreadkanal = 0; }
            double spred = 0; double vol = 0; if (idx > 0 && kanal.Count - 1 > countreadkanal)
            {
                spred = Spred(kanal[idx - 1], kanal[idx]);
                sumreadkanal += spred; countreadkanal++;
            }
            if(kanal != null && idx != 0 && x == pointXY.PriceGep.Count - 1) { vol = sumreadkanal / countreadkanal; }
            return vol;
        }
        //================================================================================                    
        public void TabControl1()
        {            
                /*if (txmlConn.SecurityNew.Decimals == 0) chart1.ChartAreas[0].CursorX.Interval = 1;
                if (txmlConn.SecurityNew.Decimals == 2) chart1.ChartAreas[0].CursorX.Interval = 0.05;
                chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
                chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
                chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                if (tabControl1.SelectedIndex == 0)
                {
                    panel1.Visible = true; pictureBox1.Visible = true;                    
                    //serv.panel1.Size = new Size(626, 376);                    
                    //serv.panel1.AutoScrollPosition = Point.Empty;
                    //serv.radioButton4.Location = new Point(451, 31);//703, 64
                }
                if (tabControl1.SelectedIndex == 1 || tabControl1.SelectedIndex == 5 || tabControl1.SelectedIndex == 6) //геп
                {
                    panel1.Visible = true; chart1.Visible = true; pictureBox1.Visible = false;                    
                    chart1.Series["Series2"].Points.Clear();
                    chart1.Series["Series3"].Points.Clear();
                    chart1.Series["Series3"].IsVisibleInLegend = false;
                    chart1.Series["Series2"].Color = Color.Gray;
                    if (pointXY.PriceGep != null) for (int x = 0; x < pointXY.PriceGep.Count; x++)
                        {                            
                            chart1.Series["Series2"].Points.AddXY(pointXY.PriceGep[x], 0);
                            if (tabControl1.SelectedIndex == 5 && timeavg != 0 && pointXY.TimePrice[x] > timeavg)
                            {
                                chart1.Series["Series2"].Points[x].YValues[0] = pointXY.TimePrice[x];
                                chart1.Series["Series2"].Points[x].Color = Color.Gray;
                            }
                            if (tabControl1.SelectedIndex == 1 && pointXY.SpredGep[x] != 0)
                            {
                                chart1.Series["Series2"].Points[x].YValues[0] = pointXY.SpredGep[x];
                                chart1.Series["Series2"].Points[x].Color = Color.Gray; }
                            if (tabControl1.SelectedIndex == 6 && bidofferavg != 0 && pointXY.BidOfferSum[x] > bidofferavg)
                            {
                                chart1.Series["Series2"].Points[x].YValues[0] = pointXY.BidOfferSum[x];
                                chart1.Series["Series2"].Points[x].Color = Color.Gray;
                            }                            
                        }
                }
                if (tabControl1.SelectedIndex == 2) //сделки
                {
                    chart1.Series["Series2"].Points.Clear();
                    chart1.Series["Series3"].Points.Clear();
                    panel1.Visible = true; chart1.Visible = true;
                    label4.Visible = true; pictureBox1.Visible = false;                    
                    chart1.Series["Series3"].IsVisibleInLegend = true;
                    chart1.Series["Series2"].Color = Color.Red;
                    for (int x = 0; x < dealVol.Count; x++)
                    {
                        chart1.Series["Series3"].Points.AddXY(dealVol[x].Price, dealVol[x].Buy);
                        chart1.Series["Series2"].Points.AddXY(dealVol[x].Price, -dealVol[x].Sell);
                    }
                }
                if (tabControl1.SelectedIndex == 3)//стакан
                {                                        
                    chart1.Series["Series2"].Points.Clear();
                    chart1.Series["Series3"].Points.Clear();
                    panel1.Visible = true; chart1.Visible = true;
                    label4.Visible = true;
                    pictureBox1.Visible = false;
                    chart1.Series["Series3"].IsVisibleInLegend = true;
                    chart1.Series["Series2"].Color = Color.Red;                    
                    if (toolStripButtonDataGrid.CheckState == CheckState.Checked) chart1.Dock = DockStyle.Fill;
                    if (toolStripButtonDataGrid.CheckState == CheckState.Unchecked) chart1.Dock = DockStyle.None;
                    for (int x = 0; x < stakanVolume.Count; x++)
                    {
                        chart1.Series["Series3"].Points.AddXY(stakanVolume[x].Price, stakanVolume[x].Buy);
                        chart1.Series["Series2"].Points.AddXY(stakanVolume[x].Price, -stakanVolume[x].Sell);
                    }
                }
                if (tabControl1.SelectedIndex == 4)
                {
                    panel1.Visible = true; pictureBox1.Visible = true;
                    chart1.Visible = false;
                    if (toolStripButtonDataGrid.CheckState == CheckState.Checked) pictureBox1.Dock = DockStyle.Fill;
                    if (toolStripButtonDataGrid.CheckState == CheckState.Unchecked)
                    {
                        pictureBox1.Dock = DockStyle.None;
                        pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                        pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
                    }
                    toolStripComboBoxSizeMode.Enabled = true;
                } // графика
                if (tabControl1.SelectedIndex == 0 || tabControl1.SelectedIndex == 4)
                    txmlConn.Grafs.PointGraphics();//serv.pictureBox1.Invalidate();      
                if(tabControl1.SelectedIndex != 4) toolStripComboBoxSizeMode.Enabled = false; */                         
        }                       
        //================================================================================                                                         
        void StopLoss(string vr, double en, int quantity, double close, int x)
        {
            string deal = null;
            if (TransaqString.Cashs[x].Buysell == "S" && en < close)
            {
                double sp = Math.Round(Spred(en, close), 3); stopDeal += sp * quantity; //if (stop == null) stop = en.ToString(); else stop = stop + ";" + en;
                string s = "{" + vr + "} " + "S" + " /  " + en + "/ " + close + "/ " + (sp * quantity) +
                    "{" + txmlConn.QuotationsNew.Bid + "/" + txmlConn.QuotationsNew.Offer + "}"; //cs.vol52 = null;
                Main_window.table_1_7.Text = lastDeal.ToString();
                deal = "stop"; txmlConn.Lb.BeginInvoke(deal + " " + s + " =>" + TransaqString.Cashs[x].Brokerref, "listBox1", null, null);
            }
            if (TransaqString.Cashs[x].Buysell == "S" && en > close)
            {
                double sp = Math.Round(Spred(en, close), 3); profStop += sp * quantity;
                string s = "{" + vr + "} " + "S" + " / " + en + "/ " + close + "/ " + (sp * quantity) + 
                    "{" + txmlConn.QuotationsNew.Bid + "/" + txmlConn.QuotationsNew.Offer + "}"; //cs.vol52 = null;
                Main_window.table_1_7.Text = lastDeal.ToString();
                deal = "prof"; txmlConn.Lb.BeginInvoke(deal + " " + s + "=>" + TransaqString.Cashs[x].Brokerref, "listBox1", null, null);
            }
            if (TransaqString.Cashs[x].Buysell == "B" && en > close)
            {
                double sp = Math.Round(Spred(en, close), 3); stopDeal += sp * quantity; //if (stop == null) stop = en.ToString(); else stop = stop + ";" + en;
                string s = "{" + vr + "} " + "B" + " /  " + en + "/ " + close + "/ " + (sp * quantity) + 
                    "{" + txmlConn.QuotationsNew.Bid + "/" + txmlConn.QuotationsNew.Offer + "}"; //cs.vol52 = null;
                Main_window.table_1_7.Text = lastDeal.ToString(); //dataGridView2.Rows[2].Cells[7].Value = vol60; 
                deal = "stop"; txmlConn.Lb.BeginInvoke(deal + " " + s + "=>" + TransaqString.Cashs[x].Brokerref, "listBox1", null, null);
            }
            if (TransaqString.Cashs[x].Buysell == "B" && en < close)
            {
                double sp = Math.Round(Spred(en, close), 3); profStop += sp * quantity;
                string s = "{" + vr + "} " + "B" + " / " + en + "/ " + close + "/ " + (sp * quantity) + "{" + txmlConn.QuotationsNew.Bid +
                    "/" + txmlConn.QuotationsNew.Offer + "}"; //cs.vol52 = null;
                Main_window.table_1_7.Text = lastDeal.ToString(); //dataGridView2.Rows[2].Cells[7].Value = vol60; 
                deal = "prof"; txmlConn.Lb.BeginInvoke(deal + " " + s + "=>" + TransaqString.Brokerref, "listBox1", null, null);
            }
            Main_window.table_3_7.Text = profStop - stopDeal + " %";
            BuySellCount(deal, x);
        }
        //================================================================================    
        int buySellCount;
        void BuySellCount(string deal, int x)
        {
            if (deal == "prof" && x < TransaqString.Cashs.Count)
            {
                if (transaqString.Cashs[x].Buysell == "S") buySellCount--;
                if (transaqString.Cashs[x].Buysell == "B") buySellCount++;
            }            
        }
        //================================================================================            
        
        //================================================================================
        public void DeltaSubtract(int vol)
        {            
            if (DeltaSatkan < 0) DeltaSatkan += vol;
            if (DeltaSatkan > 0) DeltaSatkan -= vol;
        }
        //================================================================================            
            
        //================================================================================    
        double SuM(double vol1, double vol2)
        {
            double vol = 0; if (vol1 * vol2 > 0)
            {
                if (vol1 > vol2) vol = vol1 - vol2;
                if (vol1 < vol2) vol = vol2 - vol1;
                if (vol < 0) vol = -vol;
            }
            return vol;
        }                                        
        //================================================================================    
        public double Sum(double sum, double mus)
        {
            double vol = 0;
            vol = sum - mus; if (sum < mus) vol = mus - sum;
            if (vol != 0 && vol <= 0) vol = mus - sum; //if (vol > 0) mus = 0;
            return vol;
        }
        //================================================================================    
        public float Sum(float sum, float mus)
        {
            float vol = 0;
            vol = sum - mus; if (sum < mus) vol = mus - sum;
            if (vol != 0 && vol <= 0) vol = -vol; //if (vol > 0) mus = 0;
            return vol;
        }
        //================================================================================    
        public double Spred(double last, double first)
        {
            double spred = 0;
            if (last < 0) last = -last; if (first < 0) first = -first;
            if (last != 0 && first != 0)
            {
                if (first < last)
                    spred = 100 - (first / last * 100);
                if (first > last)
                    spred = 100 - (last / first * 100);
            }
            return spred;
        }
        //================================================================================    
        public int Avgprice()
        {
            int vol = 0;
            if(txmlConn.QuotationsNew.Last * avgpoint != 0)
            {
                if(txmlConn.QuotationsNew.Last < avgpoint) vol = -1; if(txmlConn.QuotationsNew.Last > avgpoint) vol = 1;
            }
            return vol;
        }                                        
        //================================================================================  
        private PointMas pointXY = new PointMas();
        public PointMas PointXY
        {
            get { return pointXY; }
            set { pointXY = value; }
        }        
        //================================================================================  
        int avgpointcount;
        public int AvgPointXYCount
        {
            get { return avgpointcount; }
            set { avgpointcount = value; }
        }
        //================================================================================  
        double onethirdpoint;
        public double OneThirdPoint
        {
            get { return onethirdpoint; }
            set { onethirdpoint = value; }
        }
        //================================================================================  
        double avgsup;
        public double AvgSup
        {
            get { return avgsup; }
            set { avgsup = value; }
        }
        //================================================================================  
        double avgpoint;
        public double AvgPoint
        {
            get { return avgpoint; }
            set { avgpoint = value; }
        }
        //================================================================================  
        double maxpoint, minpoint;
        public double MaxPoint
        {
            get { return maxpoint; }
            set { maxpoint = value; }
        }
        //================================================================================  
        public double MinPoint
        {
            get { return minpoint; }
            set { minpoint = value; }
        }                                               
        //================================================================================  
        List<double> dealPrice = new List<double>();
        public List<double> DealPrice
        {
            get { return dealPrice; }
            set { dealPrice = value; }
        }
        //================================================================================  
        List<List<int>> dealQuantity = new List<List<int>>();
        public List<List<int>> DealQuantity
        {
            get { return dealQuantity; }
            set { dealQuantity = value; }
        }
        //================================================================================          
        private Cash transaqString;
        public Cash TransaqString
        {
            get { return transaqString; }
            set { transaqString = value; }
        }        
        //================================================================================    
        static string mode;
        public static string Mode
        {
            get { return mode; }
            set { mode = value; }
        }
        //================================================================================    
        double lastDeal;
        public double Balance
        {
            get { return lastDeal; }
            set { lastDeal = value; }
        }
        //================================================================================    
        double buySell;
        public double Delta
        {
            get { return buySell; }
            set { buySell = value; }
        }
        //================================================================================    
        int sumDeal;
        public int SumDeal
        {
            get { return sumDeal; }
            set { sumDeal = value; }
        }
        //================================================================================    
        double profStop;
        public double ProfStop
        {
            get { return profStop; }
            set { profStop = value; }
        }
        //================================================================================    
        double stopDeal;
        public double StopDeal
        {
            get { return stopDeal; }
            set { stopDeal = value; }
        }
        //================================================================================   
        static double lowPrice, hingPrice;
        public static double LowPrice
        {
            get { return lowPrice; }
            set { lowPrice = value; }
        }
        //================================================================================  
        public static double HingPrice
        {
            get { return hingPrice; }
            set { hingPrice = value; }
        }
        //================================================================================                     
        public double Scale
        {
            get { return 270; }            
        }                
        //================================================================================
        public int DeltaDeal { get; set; }
        //================================================================================
        public int DeltaSatkan { get; set; }
        //================================================================================
        double spredAvgXTime;
        public double SpredAvgXTime
        {
            get { return spredAvgXTime; }
            set { spredAvgXTime = value; }
        }
        //================================================================================
        public int Y1
        {
            get { return y1; }
            set { y1 = value; }
        }
        //================================================================================
        public int X1
        {
            get { return x1; }
            set { x1 = value; }
        }
        //================================================================================
        public double AvgLast
        {
            get { return avgLast; }
            set { avgLast = value; }
        }
        //================================================================================
        public DealPointXY DealPointXY { get; }        
        //================================================================================
        public float Y1Operational { get; set; }
        //================================================================================        
        //================================================================================        
    }
}
