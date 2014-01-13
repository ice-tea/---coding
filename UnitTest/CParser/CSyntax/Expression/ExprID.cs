using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Expression
{
    public class ExprID: CExpr
    {
        public ExprID(string str)
        {
            this.Name = str;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override HashSet<CExpr> GetExpressions()
        {
            HashSet<CExpr> result = new HashSet<CExpr>();
            result.Add(this);
            return result;
        }

        public override CExpr Clone()
        {
            return new ExprID(this.Name);
        }
    }
}
