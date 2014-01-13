using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Expression;

namespace CFrontendParser.CSyntax.CFG
{
    /* if 条件语句 */
    class IfStmt : CStmt
    {
        private CExpr _condition;
        private CStmt _trueBranch;
        private CStmt _falseBranch;

        public IfStmt(CExpr c, CStmt t, CStmt f)
        {
            this._condition = c;
            this._trueBranch = t;
            this._falseBranch = f;
        }

        /* 分支条件 */
        public CExpr Condition { get { return this._condition; } }

        /* 真分支 */
        public CStmt TrueBranch { get { return this._trueBranch; } }

        /* 假分支 */
        public CStmt FalseBranch { get { return this._falseBranch; } }

        public override string ToString()
        {
            string str = "";
            str += this.GetTab(this.Depth) + "if";
            str += "(" + this._condition.ToString() + ")" + "\n";
            this._trueBranch.Depth = this.Depth;
            str += this._trueBranch.ToString() + "\n";
            if (this._falseBranch != null)
            {
                str += this.GetTab(this.Depth) + "else" + "\n";
                this._falseBranch.Depth = this.Depth;
                str += this._falseBranch.ToString() + "\n";
            }
            return str;
        }

        public override void Travel(Process process, object parameter, object result)
        {
            process(this.Condition, parameter, result);
            this.FalseBranch.Travel(process, parameter, result);
            this.TrueBranch.Travel(process, parameter, result);
        }
    }
}
