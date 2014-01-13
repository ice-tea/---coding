using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Expression;
using CFrontendParser.CSyntax.util;

using UnitData;

namespace CFrontendParser.CSyntax.CFG
{
    /*
     * switch-case 语句
     * */
    public class SwitchStmt: CStmt
    {
        /* switch 表达式 */
        private CExpr _expression;
        public CExpr Expression
        { get { return this._expression; } }

        /* 各个 case 分支 */
        private List<CPair<HashSet<CExpr>, List<CStmt>>> _cases;
        public CPair<HashSet<CExpr>, List<CStmt>> this[int i]
        {
            get { return this._cases[i]; }
        }

        /* default 语句 */
        private List<CStmt> _defaultStmt;
        public List<CStmt> DefaultStmt
        {
            get { return this._defaultStmt; }
        }

        /* 构造函数 */
        public SwitchStmt(CExpr e, List<CPair<HashSet<CExpr>, List<CStmt>>> c, List<CStmt> d)
        {
            this._expression = e;
            this._cases = c;
            this._defaultStmt = d;
        }

        public override string ToString()
        {
            string str = "";
            str += this.GetTab(this.Depth) + "switch" + "(" + this._expression.ToString() + ")" + "\n";
            str += this.GetTab(this.Depth) + "{" + "\n";
            // 各个case
            foreach (CPair<HashSet<CExpr>, List<CStmt>> branch in this._cases)
            {
                string caseStr = "";
                foreach (CExpr caseExpr in branch.First)
                {
                    caseStr += this.GetTab(this.Depth) + "case " + caseExpr.ToString() + ":" + "\n";
                }
                string stmtStr = "";
                foreach (CStmt stmt in branch.Second)
                {
                    stmt.Depth = this.Depth + 1;
                    stmtStr += stmt.ToString() + "\n";
                }
                str += caseStr + stmtStr;
            }
            // default
            str += this.GetTab(this.Depth) + "default: " + "\n";
            foreach (CStmt stmt in this._defaultStmt)
            {
                stmt.Depth = this.Depth + 1;
                str += stmt.ToString() + "\n";
            }
            str += "\n";
            str += this.GetTab(this.Depth) + "}";
            return str;
        }

        public override void Travel(Process process,ref UnitDataStruct parameter, object result)
        {
            process(this._expression,ref parameter, result);
            foreach(CPair<HashSet<CExpr>, List<CStmt>> caseItem in _cases)
            {
                foreach (CExpr expr in caseItem.First)
                {
                    process(expr,ref parameter, result);
                }
                foreach (CStmt stmt in caseItem.Second)
                {
                    process(stmt,ref parameter, result);
                }
            }
            foreach (CStmt stmt in this._defaultStmt)
            {
                process(stmt,ref parameter, result);
            }
        }
    }
}
