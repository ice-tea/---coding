using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnitData;
namespace CFrontendParser.CSyntax.CFG
{
    public enum StmtType
    {
        _break,
        _continue,
        //_empty,
    }

    public class SimpleStmt: CStmt
    {
        private StmtType _sType;

        public SimpleStmt(StmtType t)
        {
            this._sType = t;
        }

        public StmtType SType
        {
            get { return _sType; }
        }

        public override string ToString()
        {
            string str = this.GetTab(this.Depth);
            switch (this._sType)
            {
                case StmtType._break:
                    return str + "break;";
                case StmtType._continue:
                    return str + "continue;";
                default:
                    return str +";";
            }
        }

        public override void Travel(Process process, ref UnitDataStruct parameter, object result)
        {
            process(this,ref parameter, result);
        }
    }
}
