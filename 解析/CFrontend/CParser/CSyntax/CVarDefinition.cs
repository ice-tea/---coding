using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.Expression;

namespace CFrontendParser.CSyntax
{
    /* 变量定义 */
    public class CVarDefinition : CEntity
    {
        /* 类型 */
        private CType _type;
        /* 初始值 */
        private CExpr _initialValue;

        public CType Type { get { return this._type; } }
        public CExpr InitialValue { get { return this._initialValue; } }

        public CVarDefinition(string name, CType t, CExpr initial)
        {
            this.Name = name;
            this._type = t;
            this._initialValue = initial;
        }

        public override string ToString()
        {
            string str = "";
            CType type = this.Type;
            if (type is CPtrType)
            {
                CPtrType type1 = (CPtrType)type;
                CType type2 = type1.Point2Type;
                if (type2 is CArrayType)
                {
                    CArrayType type3 = (CArrayType)type2;
                    uint ptrLevel = type1.Level;
                    string stars = "";
                    while (ptrLevel > 0)
                    {
                        stars += "*";
                        ptrLevel--;
                    }
                    str = type3.EleType.ToString() + " " + "(" + stars + this.Name + ")";
                    string dimStr = type3.ToString();
                    int pos = dimStr.IndexOf('[');
                    str += dimStr.Substring(pos);

                }
                else
                {
                    str = type1.ToString() + " " + this.Name;
                }
            }
            else if (type is CArrayType)
            {
                CArrayType type1 = (CArrayType)type;
                str = type1.EleType.ToString() + " " + this.Name;
                string dimStr = type1.ToString();
                int pos = dimStr.IndexOf('[');
                str += dimStr.Substring(pos);
            }
            else
            {
                str = type.ToString() + " " + this.Name;
            }

            // 处理修饰符
            if (this.IsConst)
                str = "const " + str;
            if (this.IsExtern)
                str = "extern " + str;
            if (this.IsStatic)
                str = "static " + str;
            if (this.InitialValue != null)
                str = str + " = " + this.InitialValue.ToString();
            return str;
        }

        public override int GetHashCode()
        {
            return this.Identifier().GetHashCode();
        }

        public override string Identifier()
        {
            string str = this.Name;
            if (this.IsExtern)
                str = "extern " + str;
            //if (this.IsStatic)
            //    str = "static " + str;
            //if (this.IsConst)
            //    str = "const " + str;
            return str;
        }
    }
}
