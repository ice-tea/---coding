using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Type;

namespace CFrontendParser.CSyntax.Expression
{
    /* sizeof 表达式 */
    public enum SizeofMark
    {
        _type,
        _expr,
    }

    public class ExprSizeof: CExpr
    {
        private SizeofMark _mark;
        private CExpr _expr;
        private CType _type;

        public SizeofMark Mark
        {
            get { return this._mark; }
        }

        public CExpr Expr
        {
            get { return this._expr; }
        }

        public CType Type
        {
            get { return this._type; }
        }

        public ExprSizeof(CType t)
        {
            this._mark = SizeofMark._type;
            this._type = t;
        }

        public ExprSizeof(CExpr e)
        {
            this._mark = SizeofMark._expr;
            this._expr = e;
        }

        public override string ToString()
        {
            string str = "";
            if (this._mark == SizeofMark._expr)
                str = this._expr.ToString();
            else str = this._type.ToString();
            return "sizeof" + "(" + str + ")";
        }

        public override HashSet<CExpr> GetExpressions()
        {
            if (this._mark == SizeofMark._expr)
            {
                return this._expr.GetExpressions();
            }
            else
            {
                return new HashSet<CExpr>();
            }
        }

        public override CExpr Clone()
        {
            if (this._mark == SizeofMark._expr)
            {
                return new ExprSizeof(this._expr.Clone());
            }
            else
            {
                return new ExprSizeof(this._type);
            }
        }
    }
}
