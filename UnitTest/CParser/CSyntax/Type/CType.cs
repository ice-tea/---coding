using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Type
{
    public enum CBasicType
    {
        _void,
        _int,
        _uint,
        _char,
        _uchar,
        _short,
        _ushort,
        _float,
        _double,
        _hecex,
        _string,
    }

    public abstract class CType: CEntity
    {
        public static CBasicType GetBasicType(string typeName, bool signed)
        {
            switch (typeName)
            {
                case "char":
                    if (signed)
                        return CBasicType._char;
                    else return CBasicType._uchar;
                case "short":
                    if (signed)
                        return CBasicType._short;
                    else return CBasicType._ushort;
                case "int":
                    if (signed)
                        return CBasicType._int;
                    else return CBasicType._uint;
                case "float":
                    return CBasicType._float;
                case "double":
                    return CBasicType._double;
                default:
                    // 无法识别的类型，视为整形
                    return CBasicType._int;
            }
        }

        /* 返回类型的长度，以字节为单位 */
        public abstract uint Size();

        public bool IsPrimitive
        {
            get
            {
                if (this.GetType() == typeof(CPrimitiveType))
                    return true;
                else return false;                    
            }
        }

        public bool IsDerived
        {
            get
            {
                if (this.GetType() == typeof(CDerivedType))
                    return true;
                else return false;
            }
        }

        public bool IsTypeDef
        {
            get
            {
                if (this.GetType() == typeof(CTypeDef))
                    return true;
                else return false;
            }
        }

        /// <summary>
        /// 类型的标识符
        /// </summary>
        /// <returns>即 类型名称</returns>
        public override string Identifier()
        {
            return this.Name;
        }
    }

    class CPrimitiveType: CType
    {
        private CBasicType btype;

        public CPrimitiveType(string name, CBasicType bt)
        {
            Name = name;
            btype = bt;
        }

        public CBasicType Type
        { get { return btype; } }

        public override uint Size()
        {
            switch (btype)
            {
                case CBasicType._char:
                case CBasicType._uchar:
                    return 1;
                case CBasicType._short:
                case CBasicType._ushort:
                    return 2;
                case CBasicType._int:
                case CBasicType._uint:
                case CBasicType._float:
                    return 4;
                case CBasicType._double:
                    return 8;
                default:
                    // 默认为 4 字节
                    return 4;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    

    class CTypeDef : CType
    {
        public CType type
        { get; set; }

        public override uint Size()
        {
            return this.type.Size();
        }

        public override string ToString()
        {
            return this.Name;
            //return "typedef " + type.Name + " " + this.Name;
        }
    }    
}
