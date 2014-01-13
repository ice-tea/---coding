using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver.Util
{
    public static class Utility
    {
        const double EPS = 1e-7;
        public static bool IsZero(double d)
        {
            return Math.Abs(d) < EPS;
        }
    }
}
