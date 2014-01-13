using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Expression
{
    public class ExprMem : CExpr
    {
        public ExprMem(string str)
        {
            this.Name = str;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override HashSet<CExpr> GetExpressions()
        {
            return new HashSet<CExpr>();
        }

        public override CExpr Clone()
        {
            return new ExprMem(this.Name);
        }
    }
}
