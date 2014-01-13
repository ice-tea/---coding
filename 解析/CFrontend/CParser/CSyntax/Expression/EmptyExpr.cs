using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Expression
{
    public class EmptyExpr: CExpr
    {
        public override string ToString()
        {
            return " ";
        }

        public override CExpr Clone()
        {
            return new EmptyExpr();
        }

        public override HashSet<CExpr> GetExpressions()
        {
            return new HashSet<CExpr>();
        }
    }
}
