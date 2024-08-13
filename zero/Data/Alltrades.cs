using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using zero.Control_collection;

namespace zero.Data
{
    class Alltrades: Main_window_base //Сделки по инструменту(ам) alltrades
    {
        public string Seccode { get; set; }
        //----------------------------------------------------
        //----------------------------------------------------

        private event NewStringDataTwo onAlltradesEvent;
        private TXmlConnector txmlConn;       
        public Alltrades(TXmlConnector txmlConn)
        {
            onAlltradesEvent += Add_Alltrades;            
            this.txmlConn = txmlConn;            
        }
        //----------------------------------------------------        
        //----------------------------------------------------
        public void OnNewAlltradesEvent(string key, string vol)
        {
            onAlltradesEvent(key, vol);
        }
        //----------------------------------------------------
        private void Add_Alltrades(string key, string vol) {
            string[] ky = key.Split(';'); string[] vl = vol.Split(';');
            txmlConn.InsertBdNew.Add_alltrades(ky, vl);
            Main_window.comboBox1.Dispatcher.Invoke(() => {
                string seccode = null; for (int x = 0; x < ky.Length; x++) if (ky[x] == "seccode") seccode = vl[x];
                if (Main_window.comboBox1.Text == seccode) txmlConn.Total_deal.TotVolDeal(ky, vl);
            });            
        }
        //----------------------------------------------------
        //----------------------------------------------------
    }
}
