using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace zero.Models
{
    class IndexABC
    {
        public IndexABC()
        { }
        public IndexABC(int индексА, int индексБ, int индексС)
        {
            this.ИндексА = индексА;
            this.ИндексБ = индексБ;
            this.ИндексС = индексС;
        }
        public IndexABC(int индексА, int индексБ, int индексС,
            int intersectionA, int intersectionB, int intersectionC)
        {
            this.ИндексА = индексА;
            this.ИндексБ = индексБ;
            this.ИндексС = индексС;
            this.intersectionA = intersectionA;
            this.intersectionB = intersectionB;
            this.intersectionC = intersectionC;
        }
        public int ИндексА { get; set; }
        public int ИндексБ { get; set; }
        public int ИндексС { get; set; }
        public int intersectionA { get; set; }
        public int intersectionB { get; set; }
        public int intersectionC { get; set; }
    }
}
