using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Expression;

namespace CFrontendParser.CSyntax.CFG
{
    public enum WhileMark
    {
        _while,
        _do_while,
    }
    /*
     * while, do-while 循环语句
     * */
    public class WhileStmt: CStmt
    {
        private WhileMark _mark;
        private CExpr _condition;
        private CStmt _body;

        public WhileStmt(WhileMark m, CExpr e, CStmt b)
        {
            this._mark = m;
            this._condition = e;
            this._body = b;

            this._condition.Parent = this;
            this._body.Parent = this;
        }

        public WhileMark Mark
        { get { return this._mark; } }

        public CExpr Condition
        { get { return this._condition; } }

        public CStmt Body
        { get { return this._body; } }

        public override string ToString()
        {
            this._body.Depth = this.Depth;
            string bodyStr = this._body.ToString();
            string condStr = this._condition.ToString();
            if (this._mark == WhileMark._do_while)
            {
                return this.GetTab(this.Depth) + "do" + "\n" + bodyStr + "\n" + this.GetTab(this.Depth) + "while" + "(" + condStr + ")" + ";";
            }
            else
            {
                return this.GetTab(this.Depth) +"while" + "(" + condStr + ")" + "\n" + bodyStr + "\n" + this.GetTab(this.Depth);
            }
        }

        public override void Travel(Process process, object parameter, object result)
        {
            process(this.Condition, parameter, result);
            this.Body.Travel(process, parameter, result);
        }
    }
}
