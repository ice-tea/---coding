using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.util
{
    public class CPair<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }

        public CPair(T1 t1, T2 t2)
        {
            this.First = t1;
            this.Second = t2;
        }
    }
}
