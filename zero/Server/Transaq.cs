using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Xml.XPath;
using System.Net;
using System.Threading;
using System.Data.SqlClient;
using System.Data.Common;
using System.Reflection;
using System.Web;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Net.Sockets;
using System.Diagnostics;
using zero.exel;
//using zero.Structure;
using System.Windows.Controls;
using System.Windows;
using zero.Models;
using System.Threading.Tasks;
//using System.Windows.Media;

namespace zero
{
     class Transaq
    {
        //----------------------------------------------------
        //----------------------------------------------------
        delegate void ExcellBegin();        
        Excell excel;
        public List<string> code = new List<string>();
        public List<string> board = new List<string>();        
        private TXmlConnector txmlConn;
        private GroupBox groupBox1;
        private Button button0, button2, button3_txt_Connect, button1Down;
        private RadioButton radioButton1, radioButton2;
        private ComboBox comboBox1, comboBox2, comboBox3;
        private CheckBox checkBox2;
        private ListView listView;
        private Panel panel1;
        private TextBox textBox1;                
        //----------------------------------------------------
        //----------------------------------------------------
        public Transaq(TXmlConnector txmlConn)//, Control.ControlCollection controls
        {            
            this.txmlConn = txmlConn;
            excel = new Excell(txmlConn);
            //serv.down.ReaderPointXY();
            //serv.down.ReaderAvgPointXY();
            //excelPatok = new ExcellBegin(excel.Excel);
            Collection_control();

            if(button0 != null) button0.Click += Button0_Click;
            if(button3_txt_Connect != null) button3_txt_Connect.Click += btInterfice;
        }        
        //----------------------------------------------------
        MainWindow main_window;
        void Collection_control()
        {
            if (Application.Current != null)
                foreach (Window item in Application.Current.Windows)
                {
                    if (item is MainWindow)
                    {
                        main_window = item as MainWindow;
                        Grid grid = (Grid)item.Content;
                        foreach (var item_control in grid.Children)
                        {
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
                            if (button != null)
                            {
                                if (button.Name == "button0") button0 = button;
                                if (button.Name == "button1") button1Down = button;
                                if (button.Name == "button2") button2 = button;
                                if (button.Name == "button3") button3_txt_Connect = button;
                            }
                            if (radio_button != null)
                            {
                                if (radio_button.Name == "radioButton1") radioButton1 = radio_button;
                                if (radio_button.Name == "radioButton2") radioButton2 = radio_button;
                            }
                            if (check_box != null && check_box.Name == "checkBox2") checkBox2 = check_box;
                            if (combo_box != null)
                            {
                                if (combo_box.Name == "comboBox1") comboBox1 = combo_box;
                                if (combo_box.Name == "comboBox2") comboBox2 = combo_box;
                                if (combo_box.Name == "comboBox3") comboBox3 = combo_box;
                            }
                            if (text_box != null && text_box.Name == "textBox1") textBox1 = text_box;
                        }
                    }
                }
        }
        //----------------------------------------------------
        public int session_timeout, request_timeout;
        public string AppDir; // путь к папке приложения
        public string sLogin; // логин пользователя для сервера Transaq
        public string sPassword; // пароль пользователя для сервера Transaq
        public string ServerIP; // IP адрес сервера Transaq
        public string ServerPort; // номер порта сервера Transaq        
        public void Transaq_Connect()
        {
            // проверка наличия параметров
            session_timeout = 25; request_timeout = 10;
            TXmlConnector.statusTimeout = session_timeout * 1000;            
            ServerIP = "server";
            ServerPort = "port";
            sLogin = "login";
            sPassword = "password";
            ConnectingReflect();
            // формирование текста команды
            string cmd = "<command id=\"connect\">";
            cmd = cmd + "<login>" + sLogin + "</login>";
            cmd = cmd + "<password>" + sPassword + "</password>";
            cmd = cmd + "<host>" + ServerIP + "</host>";
            cmd = cmd + "<port>" + ServerPort + "</port>";
            cmd = cmd + "<logsdir>" + AppDir + "</logsdir>";
            cmd = cmd + "<rqdelay>100</rqdelay>";
            cmd = cmd + "<session_timeout>" + session_timeout.ToString() + "</session_timeout> ";
            cmd = cmd + "<request_timeout>" + request_timeout.ToString() + "</request_timeout>";
            cmd = cmd + "<micex_registers>true</micex_registers>";
            cmd = cmd + "</command>";
            // отправка команды
            if (RdBaza.Mode != "тест") { txmlConn.RdBazaNew.TransaqString.Cashs.Clear();
                txmlConn.RdBazaNew.ProfStop = 0; txmlConn.RdBazaNew.StopDeal = 0; txmlConn.RdBazaNew.SumDeal = 0; }
            txmlConn.DownloadNew.CashAdd();            
            //TXmlConnector.statusDisconnected.Reset(); !!! разобраться для чего внедрил разработчик, не исключенно что это я ?
            string res = txmlConn.ConnectorSendCommand(cmd);
            txmlConn.Lb.BeginInvoke(res, "listBox1", null, null);
        }
        //----------------------------------------------------
        public void ConnectingReflect()
        {
            button3_txt_Connect.Dispatcher.Invoke(() => {
                button3_txt_Connect.Content = "connecting";
                if (button3_txt_Connect.Background != System.Windows.Media.Brushes.Red) button3_txt_Connect.Background = System.Windows.Media.Brushes.Blue;
                ShowStatus("Подключение к серверу...");
            });                
        }
        //----------------------------------------------------
        private void btInterfice(object sender, EventArgs e)
        {
            if (txmlConn.StatusNew.BConnecting) {
                string res = Transaq_Disconnect();
            }
            else if (!txmlConn.StatusNew.BConnecting) {
                Task task = new Task(new Action(Transaq_Connect));
                task.Start();
            }
            button1Down.Visibility= Visibility.Hidden;
        }
        //----------------------------------------------------
        public string Transaq_Disconnect()
        {
            // отключение коннектора от сервера Транзак
            //DisconnectingReflect();            
            string cmd = "<command id=\"disconnect\"/>";
            //TXmlConnector.statusDisconnected.Reset();
            string res = txmlConn.ConnectorSendCommand(cmd);            
            ShowStatus("disconnect => " + res);
            return res;
        }
        //----------------------------------------------------
        public void ShowStatus(string status_str)
        {
            // вывод сообщения в строке статуса формы
            txmlConn.Lb.BeginInvoke(status_str, "listBox1", null, null);
            // txt_Status.Refresh();
        }
        //----------------------------------------------------
        public string Newcondorder(string time, string price, string buysell, string brokerref, int quantity)
        {
            // формирование текста команды
            string seccode = txmlConn.OrdersNew.Seccode; string condType = null;            
            if (comboBox3.Text != "" && comboBox2.Text != "") quantity = int.Parse(comboBox2.Text);            
            if (txmlConn.SecurityNew.Decimals == 2) price = price.Replace(',', '.');
            if (buysell == "S") condType = "AskOrLast"; if (buysell == "B") condType = "BidOrLast";
            string cmd = "<command id=\"newcondorder\">";
            cmd = cmd + "<secid>" + txmlConn.TradesNew.Secid + "</secid>";
            cmd = cmd + "<security><board>" + txmlConn.OrdersNew.Board + 
                "</board><seccode>" + txmlConn.OrdersNew.Seccode + "</seccode></security>";
            cmd = cmd + "<client>" + client + "</client>";
            cmd = cmd + "<price>" + price + "</price>";
            // cmd = cmd + "<hidden>скрытое количество в лотах</hidden>";
            cmd = cmd + "<quantity>" + quantity + "</quantity>";
            cmd = cmd + "<buysell>" + buysell + "</buysell>";
            if (txmlConn.OrdersNew.Board == "TQBR") cmd = cmd + "<usecredit/>";
            //cmd = cmd + "<bymarket/>";
            cmd = cmd + "<brokerref>" + brokerref + "</brokerref>";
            cmd = cmd + "<cond_type>" + condType + "</cond_type>";
            cmd = cmd + "<cond_value>" + price + "</cond_value>";
            cmd = cmd + "<validafter>0</validafter>"; //validafter можно передать значение "0", если заявка начинает действовать немедленно.            
            cmd = cmd + "<validbefore>till_canceled</validbefore>";//Для validbefore значение "0" означает, что заявка будет действительна до конца сессии.
            //validbefore может принимать текстовое значение "till_canceled", которое говорит о том, что заявка будет актуальна, пока ее не снимут.                                   
            cmd = cmd + "</command>";
            // отправка команды            
            //TXmlConnector.statusDisconnected.Reset();
            string res = null; res = txmlConn.ConnectorSendCommand(cmd);            
            txmlConn.Lb.BeginInvoke(time + " /" + res + "{ " + txmlConn.QuotationsNew.Last + 
                "/" + txmlConn.QuotationsNew.Bid + "/" + txmlConn.QuotationsNew.Offer + " }", "listBox1", null, null);
            return res;
        }
        public string client;
        //----------------------------------------------------
        public string NewOrder()
        {
            // формирование текста команды
            string seccode = txmlConn.OrdersNew.Seccode;            
                if (comboBox3.Text != "" && comboBox2.Text != "") txmlConn.OrdersNew.Price = comboBox2.Text;            
            txmlConn.OrdersNew.Price = null;// ?
            if (txmlConn.SecurityNew.Decimals == 2) txmlConn.OrdersNew.Price = txmlConn.OrdersNew.Price.Replace(',', '.');
            string cmd = "<command id=\"neworder\">";
            cmd = cmd + "<secid>" + txmlConn.OrdersNew.Secid + "</secid>";
            cmd = cmd + "<security><board>" + txmlConn.OrdersNew.Board + "</board><seccode>" + seccode + "</seccode></security>";
            cmd = cmd + "<client>" + client + "</client>";
            cmd = cmd + "<price>" + txmlConn.OrdersNew.Price + "</price>";
            // cmd = cmd + "<hidden>скрытое количество в лотах</hidden>";
            cmd = cmd + "<quantity>" + txmlConn.OrdersNew.Quantity+ "</quantity>";
            cmd = cmd + "<buysell>" + txmlConn.OrdersNew.Buysell+ "</buysell>";
            /*Примечание для FORTS: На рынке FORTS не доступны параметры usecredit и nosplit.Также для параметра unfilled не доступно значение ImmOrCancel. Для опционов также не доступны рыночные заявки.*/
            if (comboBox1.Text == "TQBR") cmd = cmd + "<usecredit/>";
            //cmd = cmd + "<bymarket/>"; //по рынку                        
            cmd = cmd + "<brokerref>" + txmlConn.OrdersNew.Brokerref+ "</brokerref>";//(будет возвращено в составе структур order и trade)
            // cmd = cmd + "<unfilled>PutInQueue</unfilled>";//(другие возможные значения: CancelBalance, ImmOrCancel)
            // cmd = cmd + "<expdate>дата экспирации (только для ФОРТС)</expdate>";//(задается в формате 23.07.2012 00:00:00 (не обязательно)
            cmd = cmd + "</command>";
            // отправка команды
            // log.WriteLog("SendCommand: <command id=\"connect\"><login>" + rs.sLogin + "</login><password>*</password><host>" + rs.ServerIP + "</host><port>" + rs.ServerPort + "</port><logsdir>" + rs.AppDir + "</logsdir><rqdelay>100</rqdelay></command>");
            //TXmlConnector.statusDisconnected.Reset();
            string res = null; res = txmlConn.ConnectorSendCommand(cmd);
            //  log.WriteLog("ServerReply: " + res);
            txmlConn.Lb.BeginInvoke(txmlConn.OrdersNew.Time + " /" + res + "{ " + txmlConn.QuotationsNew.Last + "/" +
                txmlConn.QuotationsNew.Bid + "/" + txmlConn.QuotationsNew.Offer + " }", "listBox1", null, null);            
            return res;
        }
        //---------------------------------------------------- 
        public string Cancelorder(string time, long namber)
        {
            string cmd = "";
            cmd = "<command id=\"cancelorder\">";
            cmd = cmd + "<transactionid>" + namber + "</transactionid>";//номер из структуры orders
            cmd = cmd + "</command>";
            // отправка команды
            //log.WriteLog("SendCommand: <command id=\"connect\"><login>" + rs.sLogin + "</login><password>*</password><host>" + rs.ServerIP + "</host><port>" + rs.ServerPort + "</port><logsdir>" + rs.AppDir + "</logsdir><rqdelay>100</rqdelay></command>");
            //TXmlConnector.statusDisconnected.Reset();
            string res = txmlConn.ConnectorSendCommand(cmd);            
                main_window.table_0_7.Text = txmlConn.RdBazaNew.Delta.ToString(); main_window.table_0_7.Text = txmlConn.RdBazaNew.Balance.ToString();            
            txmlConn.Lb.BeginInvoke("{" + time + "}" + res + 
                "cancelorder*" + "/" + txmlConn.OrdersNew.Seccode + "/" + "№ " + namber + "{ " + txmlConn.QuotationsNew.Last + "/" +
                txmlConn.QuotationsNew.Bid + "/" + txmlConn.QuotationsNew.Offer + " }", "listBox1", null, null);
            return res;
        }
        //----------------------------------------------------         
        public void Get_Subscribe()
        {            
                for (int x = 0; x < code.Count; x++)
                    if (code[x] == comboBox1.Text)
                    {
                        string cmd = "<command id=\"subscribe\">";
                        cmd = cmd + "<alltrades><security><board>" + board[x] + "</board><seccode>" + code[x] + "</seccode></security></alltrades>";
                        cmd = cmd + "<quotations><security><board>" + board[x] + "</board><seccode>" + code[x] + "</seccode></security></quotations>";
                        cmd = cmd + "<quotes><security><board>" + board[x] + "</board><seccode>" + code[x] + "</seccode></security></quotes>";
                        cmd = cmd + "</command>";
                        // log.WriteLog("SendCommand: " + cmd);
                        //TXmlConnector.statusDisconnected.Reset();
                        string res = txmlConn.ConnectorSendCommand(cmd);
                        //log.WriteLog("ServerReply: " + res);
                    }            
        }
        //----------------------------------------------------
        public void CancelStoporder(string namber)
        {
            string cmd = null;
            if (namber != "")
            {
                cmd = "<command id=\"cancelstoporder\">";
                cmd = cmd + "<transactionid>" + namber + "</transactionid>";//номер из структуры orders
                cmd = cmd + "</command>";
                // отправка команды
                //TXmlConnector.statusDisconnected.Reset();
                string res = txmlConn.ConnectorSendCommand(cmd);
                //gr2.Rows[0].Cells[7].Value = rd.vol57; dataGridView2.Rows[1].Cells[7].Value = rd.vol58; gr2.Rows[2].Cells[7].Value = rd.vol60;
                txmlConn.Lb.BeginInvoke(res + "cancelstoporder*" + "/" + txmlConn.OrdersNew.Seccode + "/" + "№ " + namber, "listBox1", null, null);
            }
        }
        //----------------------------------------------------
        void GetHistory()
        {
            string cmd = "<command id=\"gethistorydata\">";
            cmd = cmd + "<security><board>FUT</board><seccode>RNZ5</seccode></security><period>1</period><count>60</count><reset>false</reset></command>";
            //log.WriteLog("SendCommand: " + cmd);
            string res = txmlConn.ConnectorSendCommand(cmd);
            //log.WriteLog("ServerReply: " + res);
        }
        //----------------------------------------------------
        string Alltrades(string data)
        {
            string[] sec = data.Split(';');
            string cmd = "<alltrades><trade secid =" + "внутренний код" + ">";
            cmd = cmd + "<seccode> Код инструмента </seccode>";
            cmd = cmd + "<tradeno>биржевой номер сделки</tradeno>";
            cmd = cmd + "<time>время сделки</time>";
            cmd = cmd + "<board> наименование борда </board>";
            cmd = cmd + "<price>цена сделки</price>";
            cmd = cmd + "<quantity>объем сделки</quantity>";
            cmd = cmd + "<buysell>покупка (B) / продажа (S)</buysell>";
            cmd = cmd + "<openinterest>кол-во открытых позиций на срочном рынке</openinterest>";
            cmd = cmd + "<period>Период торгов (O - открытие, N - торги, С - закрытие)</period>";
            cmd = cmd + "</trade><trade secid =\"внутренний код\">\".... \"</trade></alltrades>";
            return cmd;
        }
        //----------------------------------------------------
        void Get_Transaq_History(string SecurityID, string TimeframeID, string HistoryLength, string ResetFlag)
        {
            // запрос исторических данных для инструмента
            string cmd = "<command id=\"gethistorydata\" ";
            cmd = cmd + "secid=\"" + SecurityID + "\" ";
            cmd = cmd + "period=\"" + TimeframeID + "\" ";
            cmd = cmd + "count=\"" + HistoryLength + "\" ";
            string s = "false";
            if (bool.Parse(ResetFlag)) s = "true";
            cmd = cmd + "reset=\"" + s + "\"/>";
            string res = txmlConn.ConnectorSendCommand(cmd);
            txmlConn.Lb.BeginInvoke(res + "History*", "listBox1", null, null);
        }
        //----------------------------------------------------
        public void PassChange()
        {
            // Проверяем пароль
            // Правило: только латинские буквы и цифры, минимум 4, максимум 19
            string passPattern = @"^[a-zA-Z0-9]{4,20}$"; string result;
            //пороли: sergy7,sergy702
            string oldPass = "sergy702"; string newPass = "sergy666"; //sLogin = "FZTC2387A"; sPassword = "xX6U7cKk";
            Match m = Regex.Match(newPass, passPattern);
            if (!txmlConn.StatusNew.BConnecting) { ShowStatus("Для смены пароля подключитесь к серверу"); }
            else if (m.Success)
            {
                string cmd = String.Format("<command id=\"change_pass\" oldpass=\"{0}\" newpass=\"{1}\" />", oldPass, newPass);
                // log.WriteLog("SendCommand: <command id=\"change_pass\" oldpass=\"*\" newpass=\"*\" />");
                result = txmlConn.ConnectorSendCommand(cmd);
                //log.WriteLog("ServerReply: " + result);
                ShowStatus(result);
                XDocument aXmlDoc = XDocument.Load(new System.IO.StringReader(result));
                if (aXmlDoc.Root.Name == "result")
                {
                    if (aXmlDoc.Root.Attribute("success").Value == "true")
                    {
                        ShowStatus("Пароль изменен");
                        //log.WriteLog("Password was changed");
                    }
                    else if (aXmlDoc.Root.Attribute("success").Value == "false")
                    {
                        ShowStatus(aXmlDoc.Root.Value);
                    }
                }
                else { ShowStatus("Произошла ошибка"); }
            }
            else { ShowStatus("Введите верный новый пароль"); }
        }
        //----------------------------------------------------
        public void NewStoporder()
        {
            string board = txmlConn.OrdersNew.Board; string seccode = txmlConn.OrdersNew.Seccode;
            string buysell = ""; string cmd = ""; string quantity = "1", action = "stop";
            char[] mas = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string[] vl = null;            
                if (radioButton1.IsChecked.Value) buysell = "B";
                if (radioButton2.IsChecked.Value) buysell = "S";
                for (int x = 0; x < mas.Length; x++)
                    if (comboBox2.Text != "" && mas[x] == comboBox2.Text.ElementAt(0)) { quantity = comboBox2.Text; }
                vl = textBox1.Text.Split('/');                 
            string activ = ""; if (vl[0] != "") activ = vl[0];
            string order = ""; if (vl.Length == 1) order = vl[0]; if (vl.Length == 2) order = vl[1];
            if (activ.IndexOf(".") > 0) activ = activ.Replace(".", ",");
            if (order.IndexOf(".") > 0) order = order.Replace(".", ",");
            //double pm = 0; if (buysell == "S") pm = 1; if (buysell == "B") pm = -1;
            if (buysell != "")
            {
                cmd = "<command id=\"newstoporder\">";
                cmd = cmd + "<security><board>" + board + "</board><seccode>" + seccode + "</seccode></security>";
                cmd = cmd + "<client>" + client + "</client>";
                cmd = cmd + "<buysell>" + buysell + "</buysell>";
                //  cmd = cmd + "<linkedorderno>номер связной заявки</linkedorderno>";//(при отсутствии тэга без привязки)
                //  cmd = cmd + "<validfor>заявка действительно до</validfor>";//(не обязательно) 
                //cmd = cmd + "<expdate>дата экспирации (только для ФОРТС)</expdate>(не обязательно)";//(не обязательно)
                if (action == "stop")
                {
                    cmd = cmd + "<stoploss><activationprice>" + activ + "</activationprice>";
                    cmd = cmd + "<orderprice>" + order + "</orderprice>";
                    //cmd = cmd + "<bymarket/> - Выставить заявку по рынку";//(в этом случае orderprice игнорируется)
                    cmd = cmd + "<quantity>" + quantity + "</quantity>";//(в этом случае orderprice игнорируется)
                    // cmd = cmd + "<usecredit/> - использование кредита";                           
                    // cmd = cmd + "<guardtime>Защитное время</guardtime>";//(не обязательно)
                    // cmd = cmd + "<brokerref>Примечание брокера</brokerref>";//(не обязательно)
                    cmd = cmd + "</stoploss></command>";
                }
                if (action == "profit")
                {
                    cmd = cmd + "<takeprofit>";
                    cmd = cmd + "<activationprice>" + vl[0] + "</activationprice>";
                    cmd = cmd + "<quantity>" + quantity + "</quantity>";
                    //cmd = cmd + "<usecredit/> - использование кредита";
                    //cmd = cmd + "<guardtime>Защитное время</guardtime>";//(не обязательно)
                    //cmd = cmd + "<brokerref>Примечание брокера</brokerref>";//(не обязательно)
                    // cmd = cmd + "<correction>Коррекция</correction>";
                    //cmd = cmd + "<spread>Защитный спрэд</spread>";
                    cmd = cmd + "<bymarket/></takeprofit></command>";
                }
                //TXmlConnector.statusDisconnected.Reset();
                string res = txmlConn.ConnectorSendCommand(cmd);
                txmlConn.Lb.BeginInvoke(res + "newstoporder*" + txmlConn.QuotationsNew.Date + "/" + seccode + "/" + activ +
                    "/" + order + " {" + buysell + "}", "listBox1", null, null);
            }
        }
        //----------------------------------------------------
        public void NewStoporder(string orderprice, string activationprice)
        {
            string board = txmlConn.OrdersNew.Board; string seccode = txmlConn.OrdersNew.Seccode;
            string buysell = ""; string cmd = ""; string action = "stop";
            if (txmlConn.RdBazaNew.Delta > 0) buysell = "S";
            if (txmlConn.RdBazaNew.Delta < 0) buysell = "B";
            // for (int x = 0; x < rs.code.Count; x++) if (rs.code[x] == cmBox1.Text) { seccode = rs.code[x]; board = rs.boaard[x]; }
            for (int x = 0; x < activationprice.Length; x++)
                if (activationprice[x] == ',') { activationprice = activationprice.Replace(",", "."); orderprice = orderprice.Replace(",", "."); break; }
            cmd = "<command id=\"newstoporder\">";
            cmd = cmd + "<security><board>" + board + "</board><seccode>" + seccode + "</seccode></security>";
            cmd = cmd + "<client>" + client + "</client>";
            cmd = cmd + "<buysell>" + buysell + "</buysell>";
            //  cmd = cmd + "<linkedorderno>номер связной заявки</linkedorderno>";//(при отсутствии тэга без привязки)
            //  cmd = cmd + "<validfor>заявка действительно до</validfor>";//(не обязательно) 
            //cmd = cmd + "<expdate>дата экспирации (только для ФОРТС)</expdate>(не обязательно)";//(не обязательно)
            if (action == "stop")
            {
                cmd = cmd + "<stoploss><activationprice>" + activationprice + "</activationprice>";
                cmd = cmd + "<orderprice>" + orderprice + "</orderprice>";
                //cmd = cmd + "<bymarket/> - Выставить заявку по рынку";//(в этом случае orderprice игнорируется)
                // if (Plus(rs.vol57) != 0) cmd = cmd + "<quantity>" + Plus(rs.vol57) + "</quantity>";
                // cmd = cmd + "<usecredit/> - использование кредита";
                // cmd = cmd + "<guardtime>Защитное время</guardtime>";//(не обязательно)
                // cmd = cmd + "<brokerref>Примечание брокера</brokerref>";//(не обязательно)
                cmd = cmd + "</stoploss></command>";
            }
            if (action == "profit")
            {
                cmd = cmd + "<takeprofit>";
                cmd = cmd + "<activationprice>" + activationprice + "</activationprice>";
                cmd = cmd + "<quantity>1</quantity>";
                //cmd = cmd + "<usecredit/> - использование кредита";
                //cmd = cmd + "<guardtime>Защитное время</guardtime>";//(не обязательно)
                //cmd = cmd + "<brokerref>Примечание брокера</brokerref>";//(не обязательно)
                // cmd = cmd + "<correction>Коррекция</correction>";
                //cmd = cmd + "<spread>Защитный спрэд</spread>";
                cmd = cmd + "<bymarket/></takeprofit></command>";
            }
            cmd = cmd + "<bymarket/></takeprofit></command>";
            //TXmlConnector.statusDisconnected.Reset();
            string res = txmlConn.ConnectorSendCommand(cmd);
            //lb.BeginInvoke(res + "newstoporder" + "/" + rs.date.TimeOfDay + rs.seccode + "/" + activationprice + "/" + orderprice + "{" + buysell + "{" + Plus(rs.vol57) + "}" + action + "}"
            //, "listBox1", Color.Empty, null, null);
        }
        //----------------------------------------------------
        void Enable_Password_Controls(bool bEnable)
        {
            // установка состояния элементов управления для смены пароля
            //  txtOldPass.Enabled = bEnable;
            // txtNewPass.Enabled = bEnable;
            // checkHide.Enabled = bEnable;
            // btnPassChange.Enabled = bEnable;
        }
        //----------------------------------------------------
        private void Button0_Click(object sender, RoutedEventArgs e) {
            if (comboBox3.Text == "market" || comboBox3.Text == "open") { LoHiNull(); Order(); }
            if (comboBox3.Text == "cash") comboBox3.Items.Remove("cash");
            if (comboBox3.Text == "change") { PassChange(); comboBox3.Items.Remove("change"); }
            //if (comboBox3.Text == "excel") excelPatok.BeginInvoke(null, null);                            
        }        
        //----------------------------------------------------
        void Order()
        {
            string transaq = null; string time = txmlConn.QuotationsNew.Time;            
            if (radioButton1.IsChecked.Value) txmlConn.QuotationsNew.Buysell = "B";
            if (radioButton2.IsChecked.Value) txmlConn.QuotationsNew.Buysell = "S";
            txmlConn.QuotationsNew.Brokerref = null;//txmlConn.RdBazaNew.Semaforos.TriangleSums.Drivers.Transact.Brokerref();
            if (comboBox2.Text != null) txmlConn.QuotationsNew.Quantity = Convert.ToInt32(comboBox2.Text);
            //else txmlConn.QuotationsNew.Quantity = 1;
            if (textBox1.Text != "" && textBox1.Text.IndexOf(';') == -1)
                txmlConn.QuotationsNew.Price = txmlConn.RdBazaNew.RenamePrice(textBox1.Text);
            string order = null; if (txmlConn.QuotationsNew.Buysell != null && comboBox3.Text == "market")
            {
                if (txmlConn.RdBazaNew.TransaqString.Order == 0)
                {
                    if (txmlConn.QuotationsNew.Buysell == "S" && txmlConn.QuotationsNew.Low != 0)
                    {
                        txmlConn.QuotationsNew.Price = txmlConn.QuotationsNew.Low.ToString(); order = " market order => ";
                    }
                    if (txmlConn.QuotationsNew.Buysell == "B" && txmlConn.QuotationsNew.High != 0)
                    {
                        txmlConn.QuotationsNew.Price = txmlConn.QuotationsNew.High.ToString(); order = " market order => ";
                    }
                    txmlConn.QuotationsNew.Price = txmlConn.RdBazaNew.RenamePrice(txmlConn.QuotationsNew.Price);
                    txmlConn.RdBazaNew.TransactStringAdd("вход", -1, 0);
                }
            }
            if (textBox1.Text != "" && txmlConn.RdBazaNew.TransaqString.Order == 0)
            {
                if (comboBox3.Text == "open" && txmlConn.QuotationsNew.Buysell != null)// && serv.RdBaza.Nolimit(time, buysell, price, "вход") != "removed"
                {
                    txmlConn.RdBazaNew.TransactStringAdd("вход", 0, 0);
                    order = " limit order => ";
                }
            }
            if (order != null)
            {
                transaq = txmlConn.QuotationsNew.Buysell + ";" + txmlConn.QuotationsNew.Price +
                    ";" + comboBox3.Text + ";" + txmlConn.QuotationsNew.Quantity;
                txmlConn.Lb.BeginInvoke(time + order + transaq, "listBox1", null, null);
                textBox1.Text = null; comboBox2.Text = null; comboBox3.Text = null;
            }
        }
        //----------------------------------------------------
        void LoHiNull()
        {
            if (txmlConn.QuotationsNew.Low == 0) txmlConn.QuotationsNew.Low = RdBaza.LowPrice;
            if (txmlConn.QuotationsNew.High == 0) txmlConn.QuotationsNew.High = RdBaza.HingPrice;
        }
        //----------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;            
            if (txmlConn.RdBazaNew.TransaqString.Order != 0)
                if (txmlConn.RdBazaNew.TransaqString.Status == "active" || txmlConn.RdBazaNew.TransaqString.Status == "watching")
                    txmlConn.RdBazaNew.Cancel(txmlConn.QuotationsNew.Time, txmlConn.RdBazaNew.TransaqString.Вход, txmlConn.RdBazaNew.TransaqString.Quantity);
            for (int x = 0; x < txmlConn.RdBazaNew.TransaqString.Cashs.Count; x++)
            {
                double en = txmlConn.RdBazaNew.TransaqString.Cashs[x].Вход;
                if (txmlConn.RdBazaNew.TransaqString.Cashs[x].Order != 0)
                    if (txmlConn.RdBazaNew.TransaqString.Status == "active" || txmlConn.RdBazaNew.TransaqString.Status == "watching")
                        txmlConn.RdBazaNew.Cancel(txmlConn.QuotationsNew.Time, en, txmlConn.RdBazaNew.TransaqString.Cashs[x].Quantity);
            }
            //if (comboBox3.Text == "zoomX" || comboBox3.Text == "zoomY") txmlConn.Grafs.DynamicsPictBox.Zoom(button.Text);
        }
        //----------------------------------------------------   
        public void Enter(string buysell, double price, string status, int market, int quantity)
        {
            txmlConn.RdBazaNew.TransaqString.Cashs.Insert(0, new Cash(buysell, price, new System.Drawing.Point(), status, market, 0, 0, quantity, null));
        }
        //----------------------------------------------------
        public Excell Excells
        {
            get { return excel; }
        }
        //----------------------------------------------------
        //----------------------------------------------------
    }
}
