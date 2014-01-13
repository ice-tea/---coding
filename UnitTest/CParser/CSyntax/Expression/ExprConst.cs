using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Type;

namespace CFrontendParser.CSyntax.Expression
{
    /* 常量表达式 */
    public class ExprConst: CExpr
    {
        private string _value;
        public string Value { get { return this._value; } }

        private CBasicType _type;
        public CBasicType Type { get { return this._type; } }

        private ExprConst()
        {
        }

        public static ExprConst GetConst(int v)
        {
            ExprConst e = new ExprConst();
            e._type = CBasicType._int;
            e._value = v.ToString();
            return e;
        }

        public static ExprConst GetConst(float v)
        {
            ExprConst e = new ExprConst();
            e._type = CBasicType._float;
            e._value = v.ToString();
            return e;
        }

        public static ExprConst GetConst(double v)
        {
            ExprConst e = new ExprConst();
            e._type = CBasicType._double;
            e._value = v.ToString();
            return e;
        }

        /// <summary>
        /// 十六进制的常量
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static ExprConst GetConst(string v)
        {
            ExprConst e = new ExprConst();
            e._type = CBasicType._hecex;
            e._value = v;
            return e;
        }

        public override string ToString()
        {
            return this._value;
        }

        public override HashSet<CExpr> GetExpressions()
        {
            return new HashSet<CExpr>();
        }

        public override CExpr Clone()
        {
            ExprConst expr_clone = new ExprConst();
            expr_clone._type = this._type;
            expr_clone._value = this._value;
            return expr_clone;
        }
    }
}
