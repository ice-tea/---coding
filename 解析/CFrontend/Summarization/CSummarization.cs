using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.Preprocess;
using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.CFG;
using CFrontendParser.CSyntax.Expression;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CParser;

namespace Summarization
{
    public partial class CSummarization
    {
        private CProgram _program;

        public CSummarization(string program, string[] cfiles, CStandard s)
        {
            ParseCProgram parser = new ParseCProgram(program, cfiles, s);
            this._program = parser.Parse();
        }

        /// <summary>
        /// 获取函数报告
        /// 这些报告按照函数所属的C文件名分类
        /// </summary>
        /// <returns>
        /// key: C文件名
        /// value: 该文件中的函数的报告
        /// </returns>
        public Dictionary<string, List<CFunctionReport>> GenerateReports()
        {
            Dictionary<string, List<CFunctionReport>> lst = new Dictionary<string, List<CFunctionReport>>();
            foreach (CFunction fun in this._program.Functions)
            {
                CFunctionReport report = new CFunctionReport();
                List<CFunctionReport> reports = null;
                if (lst.TryGetValue(report.CFileName, out reports) == false)
                {
                    reports = new List<CFunctionReport>();
                    lst.Add(report.CFileName, reports);
                }
                reports.Add(report);
            }
            return lst;
        }

        public CFunctionReport GenerateReport(CFunction function)
        {
            CStmt body = function.Body;

            if (body is CompoundStmt)
            {
                CompoundStmt body2 = (CompoundStmt)body;
                for (int i = 0; i < body2.StmtCount; i++)
                {
                    CStmt si = body2[i];
                    if (si is WhileStmt)
                    {
                        WhileStmt whileStmt = (WhileStmt)si;
                        
                    }
                }
            }

            CFunctionReport report = new CFunctionReport();

            /* 函数所属的文件名，函数的名称 */
            report.CFileName = function.CFile;
            report.Name = function.Name;

            /* 返回值类型 */
            report.ReturnType = function.Signature.ReturnType.ToString();

            /* 返回值列表 */
            report.ReturnExpression = this.GetReturnValues(function);

            /* 参数列表 */
            int i = 0;
            CFuncType signature = function.Signature;
            for (i = 0; i < function.ParameterCount; i++)
            {
                CType paraType = signature[i].Second;
                string paraTypeStr = paraType.ToString();
                string paraName = function.GetParameter(i);
                if (paraType is CPtrType)
                {
                    report.OutputParameters.Add(new CPair<string, string>(paraTypeStr, paraName));
                }
                else
                {
                    report.InputParameters.Add(new CPair<string, string>(paraTypeStr, paraName));
                }
            }

            /* 局部变量 */
            foreach (CVarDefinition var in function.LocalVars)
            {
                string typeName = var.Type.ToString();
                string varName = var.Name;
                report.LocalVars.Add(new CPair<string, string>(typeName, varName));
            }

            return report;
        }

        private HashSet<string> GetReturnValues(CFunction function)
        {
            HashSet<string> result = new HashSet<string>();
            function.Body.Travel(this.ProcessReturn, null, result);
            return result;
        }

        /// <summary>
        /// 代理：分析返回值表达式
        /// </summary>
        Process ProcessReturn = delegate(CEntity entity, object parameter, object result)
        {
            if (entity is ReturnStmt && result is HashSet<string>)
            {
                HashSet<string> set = (HashSet<string>)result;
                ReturnStmt stmt = (ReturnStmt)entity;
                string str = stmt.ReturnValue.ToString();
                set.Add(str);
            }
        };

        private void GetInOut(CFunction function, HashSet<string> inputs, HashSet<string> outputs)
        {
            CPair<HashSet<string>, HashSet<string>> result = new CPair<HashSet<string>, HashSet<string>>(inputs, outputs);
            function.Body.Travel(ProcessInOut, function, result);
        }

        Process ProcessInOut = delegate(CEntity entity, object parameter, object result)
        {
            if (parameter is CFunction && 
                result is CPair<HashSet<string>, HashSet<string>> && 
                entity is CExpr)
            {
                CFunction function = (CFunction)parameter;
                CPair<HashSet<string>, HashSet<string>> InOut = (CPair<HashSet<string>, HashSet<string>>)result;
                CExpr expr = (CExpr)entity;

                HashSet<CExpr> inputs = expr.GetExpressions();
                foreach (CExpr input in inputs)
                {
                    InOut.First.Add(input.ToString());
                }

                HashSet<CExpr> outputs = expr.GetExpressions();
                foreach (CExpr output in outputs)
                {
                    InOut.First.Add(output.ToString());
                }
            }
        };
    }
}
