using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Expression;

namespace CFrontendParser.CSyntax.CFG
{
    public class ReturnStmt: CStmt
    {
        private CExpr _returnExpr;

        public ReturnStmt(CExpr r)
        {
            this._returnExpr = r;
        }

        public CExpr ReturnValue
        {
            get { return this._returnExpr; }
        }

        public override string ToString()
        {
            if (this._returnExpr!=null)
            return this.GetTab(this.Depth) + "return " + this._returnExpr.ToString() + ";";
            else return this.GetTab(this.Depth) + "return;";
        }

        public override void Travel(Process process, object parameter, object result)
        {
            process(this, parameter, result);
        }
    }
}
