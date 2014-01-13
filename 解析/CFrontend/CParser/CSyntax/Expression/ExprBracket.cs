using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Expression
{
    /// <summary>
    /// 括号表达式
    /// </summary>
    public class ExprBracket: CExpr
    {
        private CExpr _expr;
        public CExpr Expr
        { get { return this._expr; } }

        public ExprBracket(CExpr e)
        {
            this._expr = e;
        }

        public override string ToString()
        {
            return "(" + this._expr.ToString() + ")";
        }

        public override HashSet<CExpr> GetExpressions()
        {
            return this._expr.GetExpressions();
        }

        public override CExpr Clone()
        {
            CExpr expr_clone = this._expr.Clone();
            return new ExprBracket(expr_clone);
        }
    }
}
