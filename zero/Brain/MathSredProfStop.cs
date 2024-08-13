using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zero.Models;

namespace zero
{
    class MathSredProfStop
    {
        List<SpredProfStop> spProfStop = new List<SpredProfStop> { new SpredProfStop() }; int id;
        //===============================================================================
        public void ProfStop(string time, string buySell, double enter, double outPut)
        {            
            string status = null; for (int x = 0; x < spProfStop.Count; x++)
            {
                if (buySell == "S" && enter > outPut || buySell == "B" && enter < outPut)// prof
                {                    
                    status = "prof"; NullValume(x, time, buySell, enter, outPut, status);
                    spProfStop[x].CountProf++; spProfStop[x].ID = ++id;
                    if (spProfStop[x].Enter == enter) spProfStop[x].CountEnterProf++;
                    if (spProfStop[x].Output == outPut) spProfStop[x].CountOutputProf++;                    
                }
                if (buySell == "S" && enter < outPut || buySell == "B" && enter > outPut)// stop
                {
                    status = "stop"; NullValume(x, time, buySell, enter, outPut, status);
                    spProfStop[x].CountStop++; spProfStop[x].ID = ++id;
                    if (spProfStop[x].Enter == enter) spProfStop[x].CountEnterStop++;
                    if (spProfStop[x].Output == outPut) spProfStop[x].CountOutputStop++;
                }
                if (x == spProfStop.Count - 1)
                    AddProfStop(time, buySell, enter, outPut, status);
            }
        }
        //===============================================================================
         void NullValume(int x, string time, string buySell,
             double enter, double outPut, string status)
        {
            if (spProfStop[x].ID == 0)
            {
                spProfStop[x].ID = ++id;
                spProfStop[x].Buysell = buySell;
                spProfStop[x].Enter = enter;
                spProfStop[x].Output = outPut;
                spProfStop[x].Status = status;
            }
        }
        //===============================================================================
        void AddProfStop(string time, string buySell,
             double enter, double outPut, string status)
        {
            if (status != null)
                spProfStop.Add(new SpredProfStop(++id, buySell, enter, outPut, status));
        }
        //===============================================================================
    }
}
