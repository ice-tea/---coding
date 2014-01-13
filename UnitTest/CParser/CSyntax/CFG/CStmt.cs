using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnitData;
namespace CFrontendParser.CSyntax.CFG
{
    public delegate void Process(CEntity entity, ref UnitDataStruct parameter, object result);

    abstract public class CStmt: CEntity
    {
        abstract public void Travel(Process process, ref UnitDataStruct parameter, object result);
        /* 
         * 语句所在的作用域
         * 即距离它最近的上层复合语句
         */
        public CStmt Scope
        {
            get { return this.GetScope(); }
        }

        private CStmt GetScope()
        {
            CEntity parent = this.Parent;
            if (parent is CStmt)
            {
                CStmt parentStmt = (CStmt)parent;
                if (parentStmt is CompoundStmt)
                    return parentStmt;
                else return parentStmt.Scope;
            }
            else
            {
                return this;
            }
        }

        /* 标记 */
        public List<string> Labels { set; get; }

        /* 深度, ToString时用于控制缩进 */
        public int Depth { get; set; }

        protected string GetTab(int n)
        {
            string str = "";
            while (n > 0)
            {
                str += "\t";
                n--;
            }
            return str;
        }

        public override string Identifier()
        {
            return this.Name;
        }
    }
}
