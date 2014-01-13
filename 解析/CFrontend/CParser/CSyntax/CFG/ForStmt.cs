using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Expression;

namespace CFrontendParser.CSyntax.CFG
{
    /* 
     * for 语句
     * 包括 初始化表达式、循环条件、后置表达式、循环体
     * */
    public class ForStmt: CStmt
    {
        public ExprStmt _initStmt;
        public CExpr _condition;
        public CExpr _postExpr;
        public CStmt _body;

        public ForStmt(ExprStmt i, CExpr c, CExpr p, CStmt b)
        {
            this._initStmt = i;
            this._condition = c;
            this._postExpr = p;
            this._body = b;

            this._initStmt.Parent = this;
            this._condition.Parent = this;
            this._postExpr.Parent = this;
            this._body.Parent = this;
        }

        public ExprStmt InitStmt
        { get { return this._initStmt; } }

        public CExpr Condition
        { get { return this._condition; } }

        public CExpr PostStmt
        { get { return this._postExpr; } }

        public CStmt Body
        { get { return this._body; } }

        public override string ToString()
        {
            string str = this.GetTab(this.Depth) + "for";
            str += "(";
            if (this._initStmt != null && this._initStmt.Count > 0)
                str += this._initStmt.ToString();
            else str += "; ";
            if (this._condition != null)
                str += this._condition.ToString();
            str += "; ";
            if (this._postExpr != null)
                str += this._postExpr.ToString();
            str += ")";
            str += "\n";
            this._body.Depth = this.Depth;
            str += this._body.ToString();
            return str;
        }

        public override void Travel(Process process, object parameter, object result)
        {
            process(this.InitStmt, parameter, result);
            //LIBO 对For 循环处理
            //this.InitStmt.Travel(process, parameter, result);
            process(this.Condition, parameter, result);
            process(this.PostStmt, parameter, result);
            this.Body.Travel(process, parameter, result);
        }
    }
}
