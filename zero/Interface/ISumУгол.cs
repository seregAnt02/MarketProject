using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zero.Interface
{
    interface ISumУгол
    {
        void Sum(List<Triangle> угол, int x, string status);
        bool Gool(double vol, double first, double last, string ss);
        bool Gool(double vol, float first, float last, string ss);
    }
}
