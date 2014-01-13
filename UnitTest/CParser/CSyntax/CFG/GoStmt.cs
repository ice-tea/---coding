using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnitData;
namespace CFrontendParser.CSyntax.CFG
{
    public class GoStmt: CStmt
    {
        private string goTo;
        private CStmt labelStmt;

        public GoStmt(string g)
        {
            this.goTo = g;
            this.labelStmt = null;
        }

        public string GoTo
        {
            get { return this.goTo; }
        }

        public CStmt TargetStmt 
        {
            get
            {
                if (this.labelStmt == null)
                    this.labelStmt = this.GetLabelStmt(this.Parent, this.goTo);
                return this.labelStmt;
            }
        }

        private CStmt GetLabelStmt(CEntity entity, string label)
        {
            if (entity is CompoundStmt)
            {
                CompoundStmt cStmt = (CompoundStmt)entity;
                for (int i = 0; i < cStmt.StmtCount; i++)
                {
                    CStmt stmt = cStmt[i];
                    if (stmt.Labels.Contains(label))
                        return stmt;
                }
                return this.GetLabelStmt(cStmt.Parent, label);
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return this.GetTab(this.Depth) + "goto " + this.goTo;
        }

        public override void Travel(Process process,ref UnitDataStruct parameter, object result)
        {
            process(this, ref parameter, result);
        }
    }
}
