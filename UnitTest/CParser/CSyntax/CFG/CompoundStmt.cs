using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.util;

using UnitData;
namespace CFrontendParser.CSyntax.CFG
{
    /* 复合语句 */
    public class CompoundStmt: CStmt
    {
        /* 局部变量声明 */
        private CEntityCollection<CVarDefinition> _localVars;
        /* 语句 */
        private List<CStmt> _stmts;

        public CompoundStmt(CEntityCollection<CVarDefinition> vars, List<CStmt> s)
        {
            if (vars != null)
            {
                this._localVars = vars;
                foreach (CVarDefinition var in this._localVars.CEntityList)
                {
                    var.Parent = this;
                }
            }
            else
            {
                this._localVars = new CEntityCollection<CVarDefinition>();
            }

            if (s != null)
            {
                this._stmts = s;
                foreach (CStmt stmt in this._stmts)
                {
                    stmt.Parent = this;
                }
            }
            else
            {
                this._stmts = new List<CStmt>();
            }
        }

        public int LocalVarCount { get { return this._localVars.CEntities.Count; } }

        public CVarDefinition GetLocalVar(string name)
        {
            return this._localVars.GetCEntity(name);
        }

        public CVarDefinition GetLocalVar(int i)
        {
            return this._localVars.CEntityList[i];
        }

        //public List<CVarDefinition> GetLocalVar()
        //{
        //    return this._localVars.CEntityList;
        //}

        public int StmtCount
        { get { return this._stmts.Count; } }

        /// <summary>
        /// 访问复合语句的子语句
        /// </summary>
        /// <param name="index">子语句的标号，从0开始</param>
        /// <returns>子语句</returns>
        public CStmt this[int index]
        {
            get { return this._stmts.ElementAt(index); }
        }

        public override string ToString()
        {
            string str = "";
            str += this.GetTab(this.Depth) + "{" + "\n";
            foreach (CStmt stmt in this._stmts)
            {
                stmt.Depth = this.Depth + 1;
                str += stmt.ToString() + "\n";
            }
            str += this.GetTab(this.Depth) + "}";
            return str;
        }

        public override void Travel(Process process, ref UnitDataStruct parameter, object result)
        {
            //遍历局部变量定义
            foreach (CVarDefinition var in _localVars.CEntityList)
            {
                //更新全局求解器
                if (var.Type.Name == "char *" && var.Name.IndexOf("MYUNIT") == 0)
                {
                    string inital = var.InitialValue.ToString();
                    parameter.register(inital);
                }
                else
                {
                    if (var.Type.IsPrimitive)
                        parameter.addVar(var.Name);
                    else
                        parameter.addCom(var.Name, var.Type.Name);
                }
            }
            foreach (CStmt stmt in this._stmts)
            {
                stmt.Travel(process,ref parameter, result);
            }
        }
    }
}
