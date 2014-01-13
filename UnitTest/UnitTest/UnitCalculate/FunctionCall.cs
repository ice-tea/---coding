using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Expression;

//For caculate
using UnitData;
using Solver.Util;
using UnitTest;

namespace UnitTest.UnitCalculate.Calculator
{
    public class FunctionCall : ExprCalculate
    {
        public List<WTuple<string, double>> Calculate(CExpr exp, ref UnitDataStruct uData, Reverse r = Reverse.Positive)
        {
            List<WTuple<string, double>> list = new List<WTuple<string, double>>();
            ExprFunCall realR = (ExprFunCall) exp;
            string left;
            string right;
            
            for (int i = 0; i < realR.Count; i++ )
            {
                right = uData.get(realR[i].Name);
                //函数
                left = uData.addVar(realR.Function.GetParameter(i));
                SimpleAssign(ref uData, left, right);
            }
            realR.Function.Body.Travel(UnitStaticTest.ProcessStmt, ref uData, "");
            //TODO: return 语句的处理
            return list;
        }
        void SimpleAssign(ref UnitDataStruct uData, string left, string right)
        {

        }
    }
}
