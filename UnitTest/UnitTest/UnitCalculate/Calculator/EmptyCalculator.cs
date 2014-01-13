using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Expression;

//For caculate
using UnitData;
using Solver.Util;

namespace UnitTest.UnitCalculate.Calculator
{
    public class EmptyCalculator : ExprCalculate
    {
        public List<WTuple<string, double>> Calculate(CExpr exp, ref UnitDataStruct uData, Reverse r = Reverse.Positive)
        {
            List<WTuple<string, double>> list = new List<WTuple<string, double>>();

            return list;
        }
    }
}
