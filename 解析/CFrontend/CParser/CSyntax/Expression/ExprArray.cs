using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Expression
{
    public class ExprArray: CExpr
    {
        /* 数组变量/表达式 */
        private CExpr _array;
        public CExpr Array { get { return this._array; } }

        /* 下标表达式 */
        private List<CExpr> _indexs;
        /* 下标的数目 */
        public int Count { get { return this._indexs.Count; } }
        /* 对下标进行操作 */
        public CExpr this[int i]
        {
            get { return this._indexs.ElementAt(i); }
            set 
            {
                if (i < 0)
                    return;
                // 如果下标的数目不足，则需要补齐
                if (i >= this._indexs.Count)
                {
                    while (this._indexs.Count <= i)
                    {
                        ExprConst zero = ExprConst.GetConst(0);
                        this._indexs.Add(zero);
                    }
                }
                this._indexs[i] = value; 
            }
        }

        public ExprArray(CExpr e1, CExpr e2)
        {
            this._array = e1;
            this._indexs = new List<CExpr>();
            this._indexs.Add(e2);
        }

        public ExprArray(CExpr e1, List<CExpr> idx)
        {
            this._array = e1;
            this._indexs = idx;
        }

        public override string ToString()
        {
            string str = this._array.ToString();
            //if (str == "pbcmd->buff")
            //{
            //    str = "pbcmd->buff";
            //}
            foreach (CExpr index in this._indexs)
            {
                str = str + "[" + index.ToString() + "]";
            }
            return str;
        }

        public override HashSet<CExpr>  GetExpressions()
        {
            HashSet<CExpr> result = new HashSet<CExpr>();
            result.Add(this);
            foreach (CExpr idx in this._indexs)
            {
                HashSet<CExpr> set = idx.GetExpressions();
                result.UnionWith(set);
            }
            return result;
        }

        public override CExpr Clone()
        {
            CExpr array_clone = this._array.Clone();
            List<CExpr> indexs_clone = new List<CExpr>();
            foreach (CExpr idx in this._indexs)
            {
                CExpr idx_clone = idx.Clone();
                indexs_clone.Add(idx_clone);
            }
            ExprArray expr_clone = new ExprArray(array_clone, indexs_clone);
            return expr_clone;
        }
    }
}
