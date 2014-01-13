using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.util;

namespace CFrontendParser.CSyntax.Type
{
    public class CFuncType: CType
    {
        /* 
         * 形参列表
         * 第一元：名称, 允许是空字符串
         * 第二元：类型
         */
        private List<CPair<string, CType>> parasType;

        public CFuncType()
        {
            this.parasType = new List<CPair<string, CType>>();
        }

        /* 返回值的类型 */
        public CType ReturnType { get; set; }

        public int Count { get { return this.parasType.Count; } }
        public CPair<string, CType> this[int i]
        {
            get { return this.parasType[i]; }

            // 如果不足，则先用int型补齐
            set 
            {
                while (this.parasType.Count <= i)
                {
                    CPrimitiveType type = new CPrimitiveType("int", CBasicType._int);
                    this.parasType.Add(new CPair<string, CType>("", type));
                }
                this.parasType[i] = value;
            }
        }

        public override uint Size()
        {
            return 4;
        }

        public override string ToString()
        {
            string result = "";
            foreach (CPair<string, CType> pair in this.parasType)
            {
                result += pair.Second.ToString() + ", ";
            }
            if (result.Length > 2)
                result = result.Substring(0, result.Length - 2);
            result = this.ReturnType.ToString() + " " + this.Name + "(" + result + ")";
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(CFuncType))
                return false;
            CFuncType funcType = (CFuncType)obj;

            // 返回值类型一致
            if (funcType.ReturnType != this.ReturnType)
                return false;

            // 参数数目一致
            if (this.Count != funcType.Count)
                return false;

            // 参数类型一致
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Second != funcType[i].Second)
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            string result = "";
            foreach (CPair<string, CType> pair in this.parasType)
            {
                result += pair.Second.ToString() + ", ";
            }
            if (result.Length > 2)
                result = result.Substring(0, result.Length - 2);
            // 注意, 不需要考虑函数名，仅考虑参数类型即可
            result = this.ReturnType.ToString() + " " + "(" + result + ")";
            return result.GetHashCode();
        }
    }
}
