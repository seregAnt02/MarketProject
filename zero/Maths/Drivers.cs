using System.Collections.Generic;
using zero.Interface;
using System.Drawing;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using zero.Structure;

namespace zero {
    class Drivers {
        private TXmlConnector txmlConn;
        Transactions transactions;
        private GroupBox groupBox1;
        private Panel panel1;        
        public Drivers(TXmlConnector txmlConn) {
            this.txmlConn = txmlConn;
            transactions = new Transactions(txmlConn);
            Collection_control();
        }        
        private MainWindow mainWindow;
        void Collection_control() {
            foreach (Window item in Application.Current.Windows) {
                if (item is MainWindow) {
                    mainWindow = item as MainWindow;
                    Grid grid = (Grid)item.Content;
                    foreach (var item_control in grid.Children) {
                        WrapPanel wrap_panel = (WrapPanel)item_control;
                        GroupBox group_box = (GroupBox)item_control;
                        
                        if (group_box.Name == "groupBox1") groupBox1 = group_box;
                        if (wrap_panel.Name == "panel1") panel1 = wrap_panel;                        
                    }
                }
            }
        }
        
    }
}
