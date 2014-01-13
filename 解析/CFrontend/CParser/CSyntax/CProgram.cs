using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.util;

namespace CFrontendParser.CSyntax
{
    public class CProgram: CEntity
    {
        /* 类型 */
        private CEntityCollection<CType> _types;

        /* 全局变量 */
        private CEntityCollection<CVarDefinition> _vars_global;

        /* 外部全局变量 */
        private CEntityCollection<CVarDefinition> _vars_extern;

        /* 函数 */
        private CEntityCollection<CFunction> _functions;

        /* 外部函数 */
        private CEntityCollection<CFuncType> _functions_extern;

        public CProgram(CEntityCollection<CType> types, CEntityCollection<CVarDefinition> vars_g, CEntityCollection<CVarDefinition> vars_e, CEntityCollection<CFunction> funs, CEntityCollection<CFuncType> funs_e)
        {
            this._types = types;
            this._vars_global = vars_g;
            this._vars_extern = vars_e;
            this._functions = funs;
            this._functions_extern = funs_e;
        }

        public override string Identifier()
        {
            return this.Name;
        }

        public IEnumerable<CFunction> Functions
        { get { return this._functions.CEntityList; } }

        public CEntityCollection<CVarDefinition> GlobalVars
        { get { return this._vars_global; } }
    }
}
