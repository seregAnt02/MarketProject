using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace zero.Control_collection {
    class Main_window_base {
        public Main_window_base() {
            Collection_control();
        }
        public MainWindow Main_window { get; set; }
        void Collection_control()
        {
            if (Application.Current != null)
                foreach (Window item in Application.Current.Windows)
                {
                    if (item is MainWindow)
                    {
                        Main_window = item as MainWindow;
                    }
                }
        }
    }
}
