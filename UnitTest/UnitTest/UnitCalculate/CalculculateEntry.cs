using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Expression;

//For caculate
using UnitData;
using Solver.Util;
namespace UnitTest.UnitCalculate
{
    class CalculculateEntry
    {
        public static Reverse changeSide(Reverse r)
        {
            if (r == Reverse.Positive)
                return Reverse.Negative;
            else
                return Reverse.Positive;
        }
        //表达式检测函数
        static public List<WTuple<string, double>> Calculate(CExpr exp,  ref UnitDataStruct uData, Reverse r = Reverse.Positive)
        {
            ExprCalculate cal = exprCalculateFactory.calculator(exp);
            List<WTuple<string, double>> list = cal.Calculate(exp, ref uData, r);

            return list;
        }
    }
}
