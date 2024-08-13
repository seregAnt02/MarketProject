using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace zero.Data
{
    delegate void NewBoolDataOkno(bool connected);
    class Status
    {        
        private event NewBoolDataOkno onStatusEvent; //NewBoolDataHandler, NewStringDataHandler, NewStringDataTwo
        private GroupBox groupBox1;
        private Button button0, button3_txt_Connect;
        private ComboBox comboBox1;
        private CheckBox checkBox2;
        public Transaq Transaq { get; }
        public bool BConnecting { get; set; }
        //--------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------
        public Status(Transaq transaq)
        {
            onStatusEvent += Add_Status;
            Transaq = transaq;
            Collection_control();
        }
        //--------------------------------------------------------------------------------
        MainWindow main_window;
        void Collection_control() {
            foreach (Window item in Application.Current.Windows) {
                if (item is MainWindow) {
                    main_window = item as MainWindow;
                    Grid grid = (Grid)item.Content;
                    foreach (var item_control in grid.Children) {                        
                        GroupBox group_box = item_control is GroupBox ? item_control as GroupBox : null;                        
                        ComboBox combo_box = item_control is ComboBox ? item_control as ComboBox : null;
                        Button button = item_control is Button ? item_control as Button : null;                        
                        CheckBox check_box = item_control is CheckBox ? item_control as CheckBox : null;                        

                        if (group_box != null && group_box.Name == "groupBox1") groupBox1 = group_box;                        
                        if (button != null) {
                            if (button.Name == "button0") button0 = button;                            
                            if (button.Name == "button3") button3_txt_Connect = button;
                        }                        
                        if (check_box != null && check_box.Name == "checkBox2") checkBox2 = check_box;
                        if (combo_box != null) {
                            if (combo_box.Name == "comboBox1") comboBox1 = combo_box;                            
                        }                        
                    }
                }
            }
        }
        //--------------------------------------------------------------------------------
        public void OnNewStatusEvent(bool connected)
        {
            onStatusEvent.BeginInvoke(connected, null, null);
        }
        //--------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------
        private void Add_Status(bool connected) {
            // отображение состояния подключения на форме                                                      
            button3_txt_Connect.Dispatcher.Invoke(() => {
                if (connected) {
                    button3_txt_Connect.Content = "online";
                    button3_txt_Connect.Visibility = Visibility.Visible;
                    button3_txt_Connect.Background = System.Windows.Media.Brushes.Green;
                    Transaq.ShowStatus("Подключение установлено");
                    //Enable_Password_Controls(true);
                    Transaq.Get_Subscribe();
                    button0.Visibility = Visibility.Visible;
                    comboBox1.Visibility = Visibility.Visible;
                    checkBox2.Visibility = Visibility.Visible;
                }
                else {
                    button3_txt_Connect.Content = "offline";
                    button3_txt_Connect.Visibility = Visibility.Visible;
                    button3_txt_Connect.Background = System.Windows.Media.Brushes.Red;
                    //Transaq.Transaq_Connect(); //повторное подключение                                
                }
            });            
        }
    }
}
