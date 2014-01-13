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
    public class MulCalculator : ExprCalculate
    {
        //exp is 乘法表达式， 直接返回左右表达式的乘积
        public List<WTuple<string, double>> Calculate(CExpr exp,  ref UnitDataStruct uData, Reverse r = Reverse.Positive)
        {
            List<WTuple<string, double>> list = new List<WTuple<string, double>>();
            PolyExpr realR = (PolyExpr)exp;

            CExpr c1 = realR[0];
            CExpr c2 = realR[1];
            ExprCalculate cal1 = exprCalculateFactory.calculator(c1);
            ExprCalculate cal2 = exprCalculateFactory.calculator(c2);
            List<WTuple<string, double>> cList1 = cal1.Calculate(c1, ref uData, r);
            List<WTuple<string, double>> cList2 = cal2.Calculate(c2, ref uData, r);
            //不需要增加量纲约束            
            cList1.AddRange(cList2);
            //表达式返回量纲
            list = cList1;

            return list;
        }
    }
}
