using System;
using System.Threading;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using zero.Data;
using System.Windows.Controls;
using System.Windows;
using System.IO;
using System.Collections;


namespace zero
{
	class MarshalUTF8
	{
		static private  UTF8Encoding _utf8;

		//--------------------------------------------------------------------------------
		public MarshalUTF8()
		{
			_utf8 = new UTF8Encoding();
		}

		//--------------------------------------------------------------------------------
		public IntPtr StringToHGlobalUTF8(String data)
		{
            Byte[] dataEncoded = _utf8.GetBytes(data + "\0");

			int size = Marshal.SizeOf(dataEncoded[0]) * dataEncoded.Length;

			IntPtr pData = Marshal.AllocHGlobal(size);

			Marshal.Copy(dataEncoded, 0, pData, dataEncoded.Length);

			return pData;
		}

		//--------------------------------------------------------------------------------
		static public   String PtrToStringUTF8(IntPtr pData)
		{
			// this is just to get buffer length in bytes
			String errStr = Marshal.PtrToStringAnsi(pData);
			int length = errStr.Length;

			Byte[] data = new byte[length];
			Marshal.Copy(pData, data, 0, length);

			return _utf8.GetString(data);
		}
		//--------------------------------------------------------------------------------
	}
	class TXmlConnector
	{       
        public Status StatusNew { get; set; }
        public Alltrades AlltradesNew { get; set; }                 
        public Clients ClientsNew { get; set; }
        public Orders OrdersNew { get; set; }
        public Positions PositionsNew { get; set; }
        public Quotations QuotationsNew { get; set; }
        public Quotes QuotesNew { get; set; }
        public Secinfo SecinfoNew { get; set; }
        public Security SecurityNew { get; set; }
        public Trades TradesNew { get; set; }
        public Transaq TransaqNew { get; }
        public Download DownloadNew { get; }
        public RdBaza RdBazaNew { get; }    
        public InsertBd InsertBdNew { get; }
        public Lbdel Lb { get; }        
        public char PointChar { get { return '.'; } set { } }
        private ListBox listBox1 { get; }
        public TotalDeal Total_deal { get; }
        //==========================================================================
        CallBackDelegate myCallbackDelegate;
        AutoResetEvent statusDisconnected = new AutoResetEvent(true);
        const String EX_SETTING_CALLBACK = "Не смог установить функцию обратного вызова";
        delegate bool CallBackDelegate(IntPtr pData);
        //private static bool bConnected; // флаг наличия подключения к серверу        
        public static int statusTimeout;
        //public static bool FormReady; 
        //--------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------
        public TXmlConnector()
        {
            Lb = new Lbdel(List_box);            
            TransaqNew = new Transaq(this);
            RdBazaNew = new RdBaza(this);
            Total_deal = new TotalDeal(this);
            InsertBdNew = new InsertBd(this);
            DownloadNew = new Download(this);
            StatusNew = new Status(TransaqNew);

            if (this.RdBazaNew.AutoTestCheck)
            {
                myCallbackDelegate = new CallBackDelegate(myCallBack);
                ConnectorSetCallback();
            }            

            AlltradesNew = new Alltrades(this);
            ClientsNew = new Clients(this);
            OrdersNew = new Orders(this);
            PositionsNew = new Positions(this);
            QuotationsNew = new Quotations(this);
            QuotesNew = new Quotes(this);
            SecinfoNew = new Secinfo(this);
            SecurityNew = new Security(this);
            TradesNew = new Trades(this);

            string path = Directory.GetCurrentDirectory();//Application.ExecutablePath;
            TransaqNew.AppDir = path.Substring(0, path.LastIndexOf('\\') + 1);

            if (this.RdBazaNew.AutoTestCheck)
            {
                string logPath = "D:\\Logs\\\0"; // !!! для сервера деревня "D" диск ... доделать
                if (ConnectorInitialize(logPath, 3)) statusDisconnected.Set();
            }            

            WindowCollection windowCollection = Application.Current != null ? Application.Current.Windows : null;

            if(Application.Current != null)
            foreach( var item in ((Grid)Application.Current.MainWindow.Content).Children) {
                if(item is ListBox) listBox1 = (ListBox)item;

            }
        }		       
        //--------------------------------------------------------------------------------

        private MarshalUTF8 utf8;
        public MarshalUTF8 Utf8
        {
            get { return utf8 = new MarshalUTF8(); }
        }        
        //--------------------------------------------------------------------------------
        public void ConnectorSetCallback()                                                    
        {
            if (!SetCallback(myCallbackDelegate))
            {
                throw (new Exception(EX_SETTING_CALLBACK));
            }            
        }
		//--------------------------------------------------------------------------------
        bool myCallBack(IntPtr pData)
		{
            string res;
			String data = MarshalUTF8.PtrToStringUTF8(pData);
			FreeMemory(pData);
            res = Transaq_HandleData(data);
            if (res == "server_status") New_Status();
            return true;
		}
        //--------------------------------------------------------------------------------        
        public String ConnectorSendCommand(String command)
		{
			IntPtr pData = Utf8.StringToHGlobalUTF8(command);//кодирует строку в байт
			IntPtr pResult = SendCommand(pData);
			String result = MarshalUTF8.PtrToStringUTF8(pResult);
			Marshal.FreeHGlobal(pData);
			FreeMemory(pResult);
			return result;
		}
        //--------------------------------------------------------------------------------
        public bool ConnectorInitialize(String Path, Int16 LogLevel)
        {
            IntPtr pPath = Utf8.StringToHGlobalUTF8(Path);
            IntPtr pResult = Initialize(pPath, LogLevel);
            if (!pResult.Equals(IntPtr.Zero))
            {
                String result = MarshalUTF8.PtrToStringUTF8(pResult);
                Marshal.FreeHGlobal(pPath);
                FreeMemory(pResult);
                log.WriteLog(result);
                return false;
            }
            else
            {
                Marshal.FreeHGlobal(pPath);
                log.WriteLog("Initialize() OK");
                return true;
            }

        }
        //--------------------------------------------------------------------------------
        public void ConnectorUnInitialize()
        {

            if (statusDisconnected.WaitOne(statusTimeout))
            {
                IntPtr pResult = UnInitialize();

                if (!pResult.Equals(IntPtr.Zero))
                {
                    String result = MarshalUTF8.PtrToStringUTF8(pResult);
                    FreeMemory(pResult);
                    log.WriteLog(result);
                }
                else
                {
                    log.WriteLog("UnInitialize() OK");
                }
            }
            else
            {
                log.WriteLog("WARNING! Не дождались статуса disconnected. UnInitialize() не выполнено.");
            }

        }
        //--------------------------------------------------------------------------------         
        void New_Status()
        {
            //if (FormReady)
            ////onNewStatusEvent.BeginInvoke(bConnected, null, null);
            StatusNew.OnNewStatusEvent(StatusNew.BConnecting);
            if (StatusNew.BConnecting)
            {
                statusDisconnected.Reset();
            }
            else
            {
                statusDisconnected.Set();
            }
        }
        //--------------------------------------------------------------------------------        
        string Transaq_HandleData(string data)
        {
            // обработка данных, полученных коннектором от сервера Транзак
            string sTime = DateTime.Now.ToString("HH:mm:ss.fff");
            string info = "";
            // включить полученные данные в строку вывода в лог-файл
            string textForWindow = data;
            //log.WriteLog("ServerData: " + data);
            XmlReaderSettings xs = new XmlReaderSettings();
            xs.IgnoreWhitespace = true;
            xs.ConformanceLevel = ConformanceLevel.Fragment;
            xs.ProhibitDtd = false;
            XmlReader xr = XmlReader.Create(new System.IO.StringReader(data), xs);
            string section = "";
            string line = "";
            string str = "";
            string ename = "";
            string evalue = "";
            string attr = "";
            string elementEnd = "";
            //string values = "";
            // обработка "узлов" 
            //................................................................................
            //................................................................................
            while (xr.Read())
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                    case XmlNodeType.EndElement:
                        ename = xr.Name; break;
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Comment:
                    case XmlNodeType.XmlDeclaration:
                        evalue = xr.Value; break;
                    case XmlNodeType.DocumentType:
                        ename = xr.Name; evalue = xr.Value; break;
                    default: break;
                }
                //................................................................................
                //................................................................................
                // определяем узел верхнего уровня - "секцию"
                if (xr.Depth == 0)
                {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        section = ename;
                        if ((section != "boards") && (section != "securities") && (section != "pits") && (section != "sec_info_upd") && (textForWindow.Length > 0))
                        {                         
                            textForWindow = "";
                        }
                        line = "";
                        str = "";
                        for (int i = 0; i < xr.AttributeCount; i++)
                        {
                            str = str + xr.GetAttribute(i) + ";";
                        }
                    }
                    if (xr.NodeType == XmlNodeType.EndElement)
                    {
                        //line = "";
                        //section = "";
                    }
                    if (xr.NodeType == XmlNodeType.Text)
                    {
                        str = str + evalue + ";";
                    }
                }
                //................................................................................
                // данные для рынков
                if (section == "markets")
                {
                    //xe = (XElement)XNode.ReadFrom(xr);

                    if (ename == "market")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            line = "";
                            str = "";
                            for (int i = 0; i < xr.AttributeCount; i++)
                            {
                                str = str + xr.GetAttribute(i) + ";";
                            }
                        }
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            line = "add market: " + str;
                            str = "";
                            //!!! дороботать подключено напрямую площадка ММВБ
                        }
                        if (xr.NodeType == XmlNodeType.Text)
                        {
                            str = str + evalue + ";";
                        }
                    }
                }
                //................................................................................
                if (section == "quotations")//катировки
                {
                    if (ename == "quotation")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            line = "";
                            str = "";
                            for (int i = 0; i < xr.AttributeCount; i++)
                            {
                                str = str + xr.GetAttribute(i) + ";";
                                elementEnd = xr.Name;
                            }
                        }
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            line = "add quotation: " + str;                            
                            QuotationsNew.OnNewQuotationsEvent(elementEnd, str);
                            str = ""; elementEnd = "";
                        }
                    }
                    else
                    {
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            elementEnd = elementEnd + ";" + xr.Name;
                            str = str + evalue + ";";
                        }
                    }
                }
                //................................................................................
                if (section == "sec_info_upd")// информация по инструменту
                {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        for (int i = 0; i < xr.AttributeCount; i++)
                        {
                            str = str + xr.GetAttribute(i) + ";";
                            elementEnd = xr.Name;
                        }
                    }
                    if (xr.NodeType == XmlNodeType.EndElement)
                    {
                        if (ename != "sec_info_upd")
                        {
                            elementEnd = elementEnd + xr.Name + ";";
                            str = str + evalue + ";";
                        }
                        if (ename == "sec_info_upd")
                        {
                            line = "add sec_info: " + str;
                            SecinfoNew.OnNewSecinfoEvent(elementEnd, str);
                            str = ""; elementEnd = "";
                        }
                    }
                }
                //................................................................................
                if (section == "quotes")//стакан
                {
                    if (ename == "quote")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            line = "";
                            str = "";
                            for (int i = 0; i < xr.AttributeCount; i++)
                            {
                                str = str + xr.GetAttribute(i) + ";";
                                elementEnd = xr.Name + ";";
                            }
                        }
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            line = "add quote: " + str;                           
                            QuotesNew.OnNewQuotesEvent(elementEnd, str);
                            str = ""; elementEnd = "";
                        }
                    }
                    if (xr.NodeType == XmlNodeType.EndElement)
                    {
                        elementEnd = elementEnd + xr.Name + ";";
                        str = str + evalue + ";";
                    }
                }
                //................................................................................                
                if (section == "candlekinds")// данные для таймфреймов
                {
                    if (ename == "kind")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            line = "";
                            str = "";
                        }
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            line = "add kind: " + str;                            
                            str = "";
                        }
                    }
                    else
                    {
                        if (xr.NodeType == XmlNodeType.Text)
                        {
                            str = str + evalue + ";";
                        }
                    }
                }
                //................................................................................                
                if (section == "securities")// данные для инструментов
                {
                    if (ename == "security")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            line = "";
                            str = "";
                            for (int i = 0; i < xr.AttributeCount; i++)
                            {
                                str = str + xr.GetAttribute(i) + ";";
                                elementEnd = xr.Name + ";active;";
                            }
                        }
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            line = "add security: " + str;                            
                            SecurityNew.OnNewSecurityEvent(elementEnd, str);
                            str = ""; elementEnd = "";
                        }
                    }
                    if (xr.NodeType == XmlNodeType.EndElement)
                    {
                        elementEnd = elementEnd + xr.Name + ";";
                        str = str + evalue + ";";
                    }
                }
                //................................................................................                
                if (section == "candles")// данные по свечам
                {
                    if (ename == "candles")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            line = "";
                            str = "";
                            for (int i = 0; i < xr.AttributeCount; i++)
                            {
                                str = str + xr.GetAttribute(i) + ";";
                            }
                        }
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            line = "add security: " + str;                           
                            str = "";
                        }
                    }
                    if (ename == "candle")
                    {

                    }
                }
                //................................................................................
                // данные по клиенту
                if (section == "client")
                {
                    if (ename == "client")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            line = "";
                            str = "";
                            for (int i = 0; i < xr.AttributeCount; i++)
                            {
                                str = str + xr.GetAttribute(i) + ";";
                                elementEnd = xr.Name + ";remove;";
                            }
                        }
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            line = "add client: " + str;
                            ClientsNew.OnNewClientsEvent(elementEnd, str);
                            str = ""; elementEnd = "";
                        }
                    }
                    if (xr.NodeType == XmlNodeType.EndElement)
                    {
                        elementEnd = elementEnd + xr.Name + ";";
                        str = str + evalue + ";";
                    }
                }
                //................................................................................
                if (section == "portfolio_tplus")
                {
 
                }
                //................................................................................
                // данные для позиций
                if (section == "positions")
                {
                        if (ename == "positions")
                        {
                            if (xr.NodeType == XmlNodeType.Element)
                            {
                                line = ""; str = ";";
                            }
                            if (xr.NodeType == XmlNodeType.EndElement)
                            {
                                line = "add positions: " + str;                            
                            PositionsNew.OnNewPositionsEvent(elementEnd, str);
                                str = ""; elementEnd = "";
                            }
                        }
                        if (ename != "positions")
                        {
                            if (xr.NodeType == XmlNodeType.Element)
                            {
                                elementEnd = elementEnd + xr.Name + ";";
                            }
                            if (xr.NodeType == XmlNodeType.EndElement)
                            {

                                str = str + evalue + ";";
                            }
                        }                  
                }
                //................................................................................
                if (section == "overnight")
                {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        line = "";
                        str = "";
                        for (int i = 0; i < xr.AttributeCount; i++)
                        {
                            str = str + "<" + xr.GetAttribute(i) + ">;";
                        }
                        line = "set overnight status: " + str;
                    }
                }
                //................................................................................
                // данные о статусе соединения с сервером
                if (section == "server_status")
                {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        line = "";
                        str = "";
                        string attr_connected = xr.GetAttribute("connected");//true,false,error ??? после выключения интернета
                        if (attr_connected == "true") StatusNew.BConnecting = true;
                        if (attr_connected == "false") StatusNew.BConnecting = false;

                        for (int i = 0; i < xr.AttributeCount; i++)
                        {
                            attr = xr.GetAttribute(i);
                            str = str + i.ToString() + ":<" + attr + ">;";
                        }
                        line = "set server_status: " + str;
                    }

                }
                //................................................................................
                if (section == "orders") //обрабатываем заявки
                {
                    if (ename == "order")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            line = ""; str = ";";
                            for (int i = 0; i < xr.AttributeCount; i++)
                            {
                                str = xr.GetAttribute(i) + ";";
                            }
                        }
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            line = "add orders: " + str;                            
                            OrdersNew.OnNewOrdersEvent(elementEnd, str);
                            str = ""; elementEnd = "";
                        }
                    }
                    if (ename == "stoporder")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            line = ""; str = ";";
                            for (int i = 0; i < xr.AttributeCount; i++)
                            {
                                str = xr.GetAttribute(i) + ";";
                            }
                        }
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            line = "add orders: " + str;                            
                            str = ""; elementEnd = "";
                        }
                    }
                    if (ename != "orders")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            if (ename == "stoploss")
                                for (int i = 0; i < xr.AttributeCount; i++)
                                {
                                    str = str + xr.GetAttribute(i) + ";";
                                }
                            elementEnd = elementEnd + xr.Name + ";";
                        }
                        if (xr.NodeType == XmlNodeType.EndElement)
                            str = str + evalue + ";";
                    }
                }
                //................................................................................
                if (section == "trades")//Сделка(и) клиента
                {
                    if (ename == "trade")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            line = ""; str = "";
                        }
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            line = "add orders: " + str;                                  
                            TradesNew.OnNewTradesEvent(elementEnd, str);                  
                            str = ""; elementEnd = "";
                        }
                    }
                    if (ename != "trades" && ename != "trade")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                            elementEnd = elementEnd + xr.Name + ";";
                        if (xr.NodeType == XmlNodeType.EndElement)
                            str = str + evalue + ";";
                    }
                }
                //................................................................................
                if (section == "alltrades")//сделки по инструментам
                {
                    if (ename == "trade")
                    {
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            line = "";
                            str = "";
                            for (int i = 0; i < xr.AttributeCount; i++)
                            {
                                str = str + xr.GetAttribute(i) + ";";
                                elementEnd = xr.Name;
                            }
                        }
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            line = "add trade: " + str;                            
                            AlltradesNew.OnNewAlltradesEvent(elementEnd, str);
                            str = ""; elementEnd = "";
                        }
                    }
                    else
                    {
                        if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            elementEnd = elementEnd + ";" + xr.Name;
                            str = str + evalue + ";";
                        }
                    }
                }
                //................................................................................
                if (section == "ticks")
                {

                }

                //................................................................................
                if (line.Length > 0)
                {
                    //line = new string(' ',xr.Depth*2) + line;
                    if (info.Length > 0) info = info + (char)13 + (char)10;
                    info = info + line;
                }
            }
            //if (info.Length > 0) log.WriteLog(info);

            return section;
            // вывод дополнительной информации для удобства отладки
        }        
        //--------------------------------------------------------------------------------
        private void List_box(string s, string control)
        {

            if (listBox1 != null && control == "listBox1")
            {
                listBox1.Dispatcher.Invoke(() => { listBox1.Items.Insert(0, s); });                
            }
        }
        //--------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------
        // файл библиотеки TXmlConnector.dll должен находиться в одной папке с программой

        [DllImport("txmlconnector.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern bool SetCallback(CallBackDelegate pCallback);

        //[DllImport("txmlconnector.dll", CallingConvention = CallingConvention.StdCall)]
        //private static extern bool SetCallbackEx(CallBackExDelegate pCallbackEx, IntPtr userData);

        [DllImport("txmlconnector.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern IntPtr SendCommand(IntPtr pData);

        [DllImport("txmlconnector.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern bool FreeMemory(IntPtr pData);

        [DllImport("TXmlConnector.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr Initialize(IntPtr pPath, Int32 logLevel);

        [DllImport("TXmlConnector.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr UnInitialize();

        [DllImport("TXmlConnector.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr SetLogLevel(Int32 logLevel);
        //--------------------------------------------------------------------------------
	}	
}
