using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Expression
{
    /* 初始化表达式，例如
     * sPcCmdQue PcQueCrypt2Pub[6] =
        {
            {0,  0xC20D0007},      
            {2 , 0x420D2007},      
            {4 , 0x420D4007},      
            {6 , 0xC20D0007},      
            {8 , 0x420D2007},      
            {10, 0x420D4007},      
        };
     * 的右值，就是一个初始化表达式     * 
     * */
    class ExprInitializer: CExpr
    {
        private List<CExpr> _initializers;

        public int Count { get { return this._initializers.Count; } }
        public CExpr this[int i]
        { get { return this._initializers.ElementAt(i); } }

        public ExprInitializer(List<CExpr> initial)
        {
            this._initializers = initial;
        }

        public override string ToString()
        {
            string str = "";
            foreach (CExpr e in this._initializers)
            {
                str += e.ToString() + ", ";
            }
            if (str.Length > 2)
            {
                str = str.Substring(0, str.Length - 2);
            }
            return "{" + str + "}";
        }

        public override HashSet<CExpr> GetExpressions()
        {
            HashSet<CExpr> result = new HashSet<CExpr>();
            foreach (CExpr expr in this._initializers)
            {
                HashSet<CExpr> set = expr.GetExpressions();
                result.UnionWith(set);
            }
            return result;
        }

        public override CExpr Clone()
        {
            List<CExpr> inits_clone = new List<CExpr>();
            foreach (CExpr expr in this._initializers)
            {
                inits_clone.Add(expr.Clone());
            }
            return new ExprInitializer(inits_clone);
        }
    }
}
