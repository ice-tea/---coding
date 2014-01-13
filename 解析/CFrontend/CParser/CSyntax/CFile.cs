using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.util;
using CFrontendParser.CSyntax.Type;

namespace CFrontendParser.CSyntax
{
    public class CFile: CEntity
    {
        private CEntityCollection<CType> types;
        private CEntityCollection<CVarDefinition> globals;
        private CEntityCollection<CVarDefinition> externs;
        private CEntityCollection<CFunction> functions;

        public CFile(string filename, CEntityCollection<CType> t, CEntityCollection<CVarDefinition> g, CEntityCollection<CVarDefinition> e, CEntityCollection<CFunction> f)
        {
            this.Name = filename;
            this.types = t;
            this.globals = g;
            this.externs = e;
            this.functions = f;
        }

        public override string Identifier()
        {
            return this.Name;
        }

        public override string ToString()
        {
            string res = "types: \n";
            foreach (CType type in this.types.CEntities)
            {
                res = res + type.ToString() + "\n";
            }
            res = res + "\nextern vars: \n";
            foreach (CVarDefinition var in this.externs.CEntities)
            {
                res = res + var.ToString() + "\n";
            }
            res = res + "\nglobal vars: \n";
            foreach (CVarDefinition var in this.globals.CEntities)
            {
                res = res + var.ToString() + "\n";
            }
            res = res + "\nfunctions: \n";
            foreach (CFunction fun in this.functions.CEntities)
            {
                res = res + fun.ToString() + "\n";
            }

            return res;
        }

        /* 遍历所有的类型 */
        public IEnumerable<CType> Types
        {
            get { return this.types.CEntityList; }
        }

        /* 遍历所有的全局变量 */
        public IEnumerable<CVarDefinition> Vars
        {
            get { return this.globals.CEntityList; }
        }

        /* 遍历所有的外部全局变量 */
        public IEnumerable<CVarDefinition> VarsExtern
        {
            get { return this.externs.CEntityList; }
        }

        /* 遍历所有的外部全局变量 */
        public IEnumerable<CFunction> Functions
        {
            get { return this.functions.CEntityList; }
        }
    }
}
