using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.CFG;

namespace CFrontendParser.CSyntax
{
    /* 函数定义 */
    public class CFunction: CEntity
    {
        public string CFile { get; set; }
        public CProgram Program { get; set; }

        private List<string> _parameters;
        private CFuncType _signature;
        private CStmt _body;

        public CFuncType Signature { get { return this._signature; } }
        public CStmt Body { get { return this._body; } }
        public int ParameterCount { get { return this._parameters.Count; } }

        public List<CVarDefinition> LocalVars
        {
            get
            {
                List<CVarDefinition> vars = new List<CVarDefinition>();
                if (this._body is CompoundStmt)
                {
                    CompoundStmt cstmt = (CompoundStmt)this._body;
                    for(int i = 0; i < cstmt.LocalVarCount;i++)
                    {
                        vars.Add(cstmt.GetLocalVar(i));
                    }
                }
                return vars;
            }
        }

        public string GetParameter(int i)
        {
            return this._parameters[i];
        }

        public CFunction(CFuncType type, List<string> paras, CStmt stmt)
        {
            this.Name = type.Name;
            this._parameters = paras;
            this._signature = type;
            this._body = stmt;
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < this._signature.Count; i++)
            {
                result += this._signature[i].Second.ToString() + " " + this._parameters[i] + ", ";
            }
            if (result.Length > 2)
                result = result.Substring(0, result.Length - 2);
            result = this._signature.ReturnType.ToString() + " " + this.Name + "(" + result + ")";
            // 函数体
            if (this._body == null)
            {
                result += "\n{\n}\n";
            }
            else
            {
                result += "\n" + this._body.ToString();
            }
            return result;
        }

        /// <summary>
        /// 函数的标识符
        /// </summary>
        /// <returns>即 函数名称</returns>
        public override string Identifier()
        {
            return this.Name;
        }
    }
}
