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
    public class AssignCalculator : ExprCalculate
    {
        //assign 赋值表达式，赋值左右的量纲相等
        //如果左值是可变量纲则需要对其进行更名操作
        public List<WTuple<string, double>> Calculate(CExpr exp, ref UnitDataStruct uData, Reverse r = Reverse.Positive)
        {
            List<WTuple<string, double>> list = new List<WTuple<string, double>>();
            PolyExpr realR = (PolyExpr)exp;

            string info = "";
            CExpr c1 = realR[0];
            CExpr c2 = realR[1];
            ExprCalculate cal2 = exprCalculateFactory.calculator(c2);
            List<WTuple<string, double>> cList2 = cal2.Calculate(c2, ref uData, CalculculateEntry.changeSide(r));
            
            //c1 作为左值，需要特殊处理
            ExprCalculate cal1 = new ExprIDLeftCalculator();
            List<WTuple<string, double>> cList1 = cal1.Calculate(c1, ref uData, r);
            //增加量纲约束
            //c1 + c2     unit(c1) = unit(c2)
            info = realR.ToString();
            cList1.AddRange(cList2);
            uData.AddConstraint(cList1, new DNode(UnitDataStruct.unitZero()), info);
            //打印结果
            Console.WriteLine(uData.ToString());
            Console.WriteLine(Message.ErrorMsg);
            Console.WriteLine(Message.WarningMsg);
            Console.WriteLine(Message.Msg);
            //表达式返回量纲
            list = cList2;

            return list;
        }
    }
}
