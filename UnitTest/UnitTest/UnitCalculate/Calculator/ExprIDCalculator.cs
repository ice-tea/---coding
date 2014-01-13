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
    public class ExprIDCalculator : ExprCalculate
    {
        public List<WTuple<string, double>> Calculate(CExpr exp, ref UnitDataStruct uData, Reverse r = Reverse.Positive)
        {
            List<WTuple<string, double>> list = new List<WTuple<string, double>>();

            //返回变量的id 标识  不仅仅是 realR.Name
            ExprID realR = (ExprID)exp;
            //带有scope 的唯一标识
            string scopeName = uData.get(exp.Name);
            list.Add(new WTuple<string, double>(scopeName, (int)r ));
            return list;
        }
    }
    //作为左值的 ID
    public class ExprIDLeftCalculator : ExprCalculate
    {
        public List<WTuple<string, double>> Calculate(CExpr exp, ref UnitDataStruct uData, Reverse r = Reverse.Positive)
        {
            List<WTuple<string, double>> list = new List<WTuple<string, double>>();

            //返回变量的id 标识  不仅仅是 realR.Name
            ExprID realR = (ExprID)exp;
            string scopeName = uData.get(exp.Name, true);
            list.Add(new WTuple<string, double>(scopeName, (int)r));
            return list;
        }
    }
}
