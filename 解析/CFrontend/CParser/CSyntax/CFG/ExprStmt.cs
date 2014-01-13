using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Expression;

namespace CFrontendParser.CSyntax.CFG
{
    /* 表达式语句 */
    public class ExprStmt: CStmt
    {
        /* 语句中的各个表达式 */
        private List<CExpr> _expressions;

        public ExprStmt(List<CExpr> lst)
        {
            if (lst != null)
            {
                this._expressions = lst;
            }
            else
            {
                this._expressions = new List<CExpr>();
            }
            foreach (CExpr expr in this._expressions)
            {
                expr.Parent = this;
            }
        }

        public int Count
        {
            get { return this._expressions.Count; }
        }

        public CExpr this[int i]
        {
            get
            {
                if (i >= 0 && i < this._expressions.Count)
                    return this._expressions[i];
                else return null;
            }
        }

        public override string ToString()
        {
            string str = this.GetTab(this.Depth);
            foreach (CExpr expr in this._expressions)
            {
                str += expr.ToString() + ", ";
            }
            if (str.Length > 2)
                str = str.Substring(0, str.Length - 2);
            return str + ";";
        }

        public override void Travel(Process process, object parameter, object result)
        {
            foreach (CExpr expr in this._expressions)
            {
                process(expr, parameter, result);
            }
        }
    }
}
