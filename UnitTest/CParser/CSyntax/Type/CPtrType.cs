using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Type
{

    /*
     * 用于表示指针类型
     * 暂时不支持函数指针
     * */
    public class CPtrType : CType
    {
        private CType pto;
        private uint level;

        /*
         * 指针指向的类型
         * */
        public CType Point2Type
        {
            get { return this.pto; }
        }

        /*
         * 指针的层级
         * 例如， int *** , 指向的类型是 int, 层级是 3
         * */
        public uint Level { get { return this.level; } }

        public CPtrType(CType t, uint l)
        {
            this.pto = t;
            this.level = l;

            // 设置指针类型的名称

            string name = t.Name;
            for (int i = 0; i < this.level; i++)
                name += "*";
            this.Name = name;
        }

        /* 按照规定，指针占4个字节
         * */
        public override uint Size()
        {
            return 4;
        }

        public override string ToString()
        {
            string res = pto.Name + " ";
            if (pto.IsDerived)
            {
                CDerivedType dType = (CDerivedType)this.pto;
                if (dType.IsUnion)
                    res = "union " + res;
                else if (dType.IsStruct)
                    res = "struct " + res;
            }
            for (int i = 0; i < this.level; i++)
                res += "*";
            return res;
        }
    }
}
