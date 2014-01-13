using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CSyntax.CFG;
using CFrontendParser.CSyntax.Expression;

namespace CFrontendParser.CSyntax.Expression
{
    /* 强制类型表达式 */
    class ExprCast: CExpr
    {
        /* 目的类型 */
        private CType _castType;
        public CType CastType
        { get { return this._castType; } }

        /* 原表达式 */
        private CExpr _expr;
        public CExpr Expr
        {
            get { return this._expr; }
        }

        public ExprCast(CType t, CExpr e)
        {
            this._castType = t;
            this._expr = e;
        }

        public override string ToString()
        {
            return "(" + this._castType.ToString() + ")" + this._expr.ToString();
        }

        public override HashSet<CExpr> GetExpressions()
        {
            return this._expr.GetExpressions();
        }

        public override CExpr Clone()
        {
            CExpr expr_clone = this._expr.Clone();
            return new ExprCast(this._castType, expr_clone);
        }
    }
}
