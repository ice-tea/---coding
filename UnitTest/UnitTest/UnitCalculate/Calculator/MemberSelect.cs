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
    public class MemberSelect : ExprCalculate
    {
        public List<WTuple<string, double>> Calculate(CExpr exp, ref UnitDataStruct uData, Reverse r = Reverse.Positive)
        {
            List<WTuple<string, double>> list = new List<WTuple<string, double>>();

            //返回
            PolyExpr realR = (PolyExpr)exp;
            string info = "";
            CExpr c1 = realR[0];
            CExpr c2 = realR[1];
            //支持一层访问
            //TODO：更多层级测试
            string fullName = uData.getCom(c1.Name, c2.Name);
            list.Add(new WTuple<string, double>(fullName, (int)r));
            return list;
        }
    }
}
