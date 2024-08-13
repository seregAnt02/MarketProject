using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using zero.Models.DbContext;
using System.Windows.Forms.DataVisualization.Charting;

namespace zero
{
    delegate void PatokDown();
    delegate void PatokInsert(Quotation model);
    delegate void Lbdel(string s, string control);
    //delegate void NewStringDataHandler(string data);
    delegate void NewBoolDataHandler(bool data);
    delegate void NewStringDataTwo(string key, string vol);
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TXmlConnector txmlConn;         
        //----------------------------------------------------
        public MainWindow()
        {
            InitializeComponent();

            chart_name.ChartAreas.Add(new ChartArea("main_chart"));
            var series_0 = new Series("series_0") { IsValueShownAsLabel = true , ChartType = SeriesChartType.Spline, Color = System.Drawing.Color.Red};
            var series_1 = new Series("series_1") { ChartType = SeriesChartType.Column, Color = System.Drawing.Color.Green };
            var series_2 = new Series("series_2") { ChartType = SeriesChartType.Column, Color = System.Drawing.Color.Red };
            chart_name.Series.Add(series_0);
            chart_name.Series.Add(series_1);
            chart_name.Series.Add(series_2);
            txmlConn = new TXmlConnector();

            comboBox1.Items.Add("SBER"); comboBox1.Items.Add("GAZP"); comboBox1.Items.Add("GMKN"); comboBox1.Items.Add("RNU9");
            comboBox1.Items.Add("RNH9"); comboBox1.Items.Add("ROSN"); comboBox1.Items.Add("RIH2"); comboBox1.Text = "ROSN";
            comboBox3.Items.Add("market"); comboBox3.Items.Add("open");
            comboBox3.Items.Add("excel"); comboBox3.Items.Add("zoomX"); comboBox3.Items.Add("zoomY");
            comboBox2.Items.Add("1"); comboBox2.Items.Add("2"); comboBox2.Text = "1";
            //comboBox3.Items.Add("change"); //comboBox3.Items.Add("cash");             
            RdBaza.Mode = "тест"; table_2_0.Text = RdBaza.Mode;//"боевой\тест"
            if (RdBaza.Mode == "боевой") table_2_0.Foreground = Brushes.Blue;
            // определение разделителя в числах на компьютере (запятая или точка)
            //PointChar = ','; string str = rd.last.ToString(); if (str.IndexOf('.') > 0) PointChar = '.';
            tabControl1.Items[1] = "gep"; tabControl1.Items[2] = "deal"; tabControl1.Items[3] = "stakan";
            //Enable_Password_Controls(false);// установка состояния элементов управления для смены пароля  
            txmlConn.TransaqNew.code.AddRange(new string[] { "RNH9", "RNU9", "ROSN", "RIH2" });
            txmlConn.TransaqNew.board.AddRange(new string[] { "FUT", "FUT", "TQBR", "FUT" });
            if (txmlConn.DownloadNew.MachineName == "nameSever") txmlConn.DownloadNew.Home = "nameServer";
            if (txmlConn.DownloadNew.MachineName == "nameServer") txmlConn.DownloadNew.Home = "nameServer";            
        }                                 
        //----------------------------------------------------
        //----------------------------------------------------
    }
}
