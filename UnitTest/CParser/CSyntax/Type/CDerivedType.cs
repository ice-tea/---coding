using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Type
{
    public class CDerivedType : CType
    {
        private bool isUnion;
        private List<CField> fields = new List<CField>();

        public bool IsUnion
        {
            get { return this.isUnion; }
            set { this.isUnion = value; }
        }

        public bool IsStruct
        {
            get { return !this.isUnion; }
            set { this.isUnion = !value; }
        }

        public uint FieldCount
        {
            get
            {
                if (this.fields.Count > 0)
                    return (uint)this.fields.Count;
                else return 0;
            }
        }

        public CField GetField(int i)
        {
            return this.fields[i];
        }

        public CField GetField(string fieldName)
        {
            foreach (CField field in this.fields)
            {
                if (fieldName == field.Name)
                    return field;
            }
            return null;
        }

        public void AddField(CField f)
        {
            f.Parent = this;
            this.fields.Add(f);
        }

        public override uint Size()
        {
            if (this.fields == null || this.fields.Count == 0)
                return 0;

            CField field0 = this.fields.ElementAt(0);
            uint size = field0.Type.Size() * field0.Dim.Count;
            if (this.isUnion)
            {
                for (int i = 1; i < this.fields.Count; i++)
                {
                    CField field_i = this.fields.ElementAt(i);
                    uint count = field_i.Dim.Count;
                    CType type_i = field_i.Type;
                    uint size_t = type_i.Size();
                    uint si = size_t * count;
                    if (si > size)
                        size = si;
                }
            }
            else
            {
                for (int i = 1; i < this.fields.Count; i++)
                {
                    CField field_i = this.fields.ElementAt(i);
                    uint count = field_i.Dim.Count;
                    CType type_i = field_i.Type;
                    uint size_t = type_i.Size();
                    uint si = size_t * count;
                    size = size + si;
                }
            }
            return size;
        }

        public override string ToString()
        {
            string res = "";
            if (this.isUnion)
                res += "union ";
            else res += "struct ";

            res += this.Name;
            res += "\n{";

            // 各个成员
            foreach (CField field in this.fields)
            {
                res += "\t" + field.ToString() + "\n";
            }

            res += "}";
            return res;
        }
    }

    public class CField: CEntity
    {
        private CType _type;
        private Dimension _dimension;
        private uint _bit;

        public CField(string n, CType t, Dimension d, uint b)
        {
            this.Name = n;
            this._type = t;
            this._dimension = d;
            this._bit = b;
        }

        public CType Type
        {
            get { return this._type; }
        }

        /* 表示维度 */
        public Dimension Dim
        {
            get { return this._dimension; }
        }

        /* 用于位域，如果是0，表示不是位域 */
        public uint Bit
        {
            get { return this._bit; }
        }

        public override string ToString()
        {
            string res = Type.Name + " " + this.Name;
            if (this.Dim != null)
                res += this.Dim.ToString();
            if (this._bit > 0)
                res += " : " + this._bit;
            return res;
        }

        public override string Identifier()
        {
            if (this.Parent != null)
                return this.Parent.Name + " "+ this.Name;
            else return this.Name;
            //throw new NotImplementedException();
        }
    }
}
