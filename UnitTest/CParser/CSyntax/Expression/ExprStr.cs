using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//ADD LIBO
//add CExpr Type: ExprStr

namespace CFrontendParser.CSyntax.Expression
{
    public class ExprStr : CExpr
    {
        private string _value;
        public string Value { get { return this._value; } }

        private ExprStr()
        {
        }
        public ExprStr(string s)
        {
            _value = s;
        }

        public override string ToString()
        {
            return this._value;
        }

        public override CExpr Clone()
        {
            ExprStr expr_clone = new ExprStr();
            expr_clone._value = this._value;
            return expr_clone;
        }

        public override HashSet<CExpr> GetExpressions()
        {
            return new HashSet<CExpr>();
        }
    }
}
