using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// For Parse C files to CProgram
using CFrontendParser.CParser;
using CFrontendParser.CSyntax;
using CFrontendParser.Preprocess;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CSyntax.CFG;
using CFrontendParser.CSyntax.Expression;

//For Unit datatype demention
using UnitConfigure;

//For caculate
using UnitData;
using Solver.Util;

namespace UnitTest.UnitCalculate
{
    //表达式计算部分 加上符号 正或负
    public enum Reverse { Negative = -1, Positive = 1 };
      
    //表达式接口
    public interface ExprCalculate
    {
        List<WTuple<string, double>> Calculate(CExpr exp, ref UnitDataStruct uData, Reverse r = Reverse.Positive);
    }
}
