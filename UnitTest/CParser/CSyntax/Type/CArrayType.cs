using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Type
{
    public class CArrayType: CType
    {
        private Dimension _dimension;
        private CType _type;

        public int Dim
        {
            get { return _dimension.Dim; }
        }

        public CType EleType
        {
            get { return this._type; }
        }

        public uint GetLenAt(int i)
        {
            return _dimension.Len(i);
        }

        public CArrayType(CType eleType, List<uint> dims)
        {
            this._type = eleType;
            this._dimension = new Dimension();
            foreach (uint i in dims)
            {
                _dimension.AddDimLast(i);
            }
        }

        public CArrayType(CType eleType, Dimension d)
        {
            this._type = eleType;
            this._dimension = d;
            this.SetName();
        }

        private void SetName()
        {
            this.Name = this._type.ToString() + this._dimension.ToString();
        }

        public override uint Size()
        {
            uint eleSize = this._type.Size();
            return eleSize * this._dimension.Count;
        }

        public override string ToString()
        {
            return this._type.ToString() + this._dimension.ToString();
        }
    }

    /*
     * 用于表示变量的维度
     * 例如，a[3][4][5]是个三维数组，那么它的维度信息是 <3, 4, 5>
     * 即对应的List dims = <3, 4, 5>
     * */
    public class Dimension
    {
        private List<uint> dims = new List<uint>();

        /* 维度 */
        public int Dim
        { get { return dims.Count; } }

        /* 元素总数 */
        public uint Count
        {
            get
            {
                uint r = 1;
                foreach (uint d in dims)
                {
                    r = r * d;
                }
                return r;
            }
        }

        /* 新增一个维度，作为第一维 */
        public void AddDimFirst(uint d)
        {
            this.dims.Insert(0, d);
        }

        /* 新增一个维度，作为最后一维 */
        public void AddDimLast(uint d)
        {
            this.dims.Insert(dims.Count, d);
        }

        public uint Len(int dim)
        {
            return this.dims.ElementAt(dim);
        }

        public override string ToString()
        {
            string res = "";
            for (int i = 0; i < this.dims.Count; i++)
                res += "[" + this.dims.ElementAt(i) + "]";
            return res;
        }
    }
}
