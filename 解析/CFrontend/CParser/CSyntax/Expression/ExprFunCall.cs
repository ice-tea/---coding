using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Expression
{
    public class ExprFunCall : CExpr
    {
        /* 实际参数列表 */
        private List<CExpr> _arguments;
        /* 实际参数的数目 */
        public int Count
        {
            get { return this._arguments.Count; }
        }
        public CExpr this[int i]
        {
            get { return this._arguments.ElementAt(i); }
        }

        public ExprFunCall(string funcName, List<CExpr> args)
        {
            this.Name = funcName;
            this._arguments = args;
        }

        // 被调函数
        public CFunction Function { set; get; }

        public override string ToString()
        {
            string args = "";
            foreach (CExpr arg in this._arguments)
            {
                args += arg.ToString() + ", ";
            }
            if (args.Length > 2)
            {
                args = args.Substring(0, args.Length - 2);
            }
            return this.Name + "(" + args + ")";
        }

        public override HashSet<CExpr> GetExpressions()
        {
            HashSet<CExpr> result = new HashSet<CExpr>();
            foreach (CExpr paras in this._arguments)
            {
                HashSet<CExpr> set = paras.GetExpressions();
                result.UnionWith(set);
            }
            return result;
        }

        public override CExpr Clone()
        {
            List<CExpr> args_clone = new List<CExpr>();
            foreach (CExpr arg in this._arguments)
            {
                args_clone.Add(arg.Clone());
            }
            return new ExprFunCall(this.Name, args_clone);
        }
    }
}
