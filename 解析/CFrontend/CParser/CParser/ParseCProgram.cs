using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.Preprocess;

namespace CFrontendParser.CParser
{
    public class ParseCProgram
    {
        private CFile[] cfiles;
        private string program_name;
        private string[] filenames;
        private CStandard standard;

        public ParseCProgram(string name, string[] files, CStandard s)
        {
            this.program_name = name;
            this.filenames = files;
            this.standard = s;
        }

        public CProgram Parse()
        {
            this.cfiles = new CFile[this.filenames.Length];
            int i = 0;
            foreach (string filename in this.filenames)
            {
                ParseCFile parser = new ParseCFile(filename, false, this.standard);
                cfiles[i] = parser.Parse();
                i++;
            }

            CEntityCollection<CType> types = this.GetTypes();

            CEntityCollection<CVarDefinition> vars_g = new CEntityCollection<CVarDefinition>();
            CEntityCollection<CVarDefinition> vars_e = new CEntityCollection<CVarDefinition>();
            this.GetVars(ref vars_g, ref vars_e);

            CEntityCollection<CFunction> funs = new CEntityCollection<CFunction>();
            CEntityCollection<CFuncType> funs_e = new CEntityCollection<CFuncType>();
            this.GetFuns(ref funs, ref funs_e);

            CProgram p = new CProgram(types, vars_g, vars_e, funs, funs_e);
            p.Name = this.program_name;

            // 下面的迭代，设置每个函数所属的程序
            foreach (CFunction fun in p.Functions)
            {
                fun.Program = p;
            }
            return p;
        }

        private CEntityCollection<CType> GetTypes()
        {
            CEntityCollection<CType> all_types = new CEntityCollection<CType>();
            foreach (CFile file in this.cfiles)
            {
                foreach (CType type in file.Types)
                {
                    if (all_types.CEntityExists(type.Identifier()) == false)
                    {
                        all_types.AddCEntity(type);
                    }
                }
            }
            return all_types;
        }

        private void GetVars(ref CEntityCollection<CVarDefinition> vars_g, ref CEntityCollection<CVarDefinition> vars_e)
        {
            // 首先处理全局变量，将它们都加入 vars_g
            foreach (CFile file in this.cfiles)
            {
                foreach (CVarDefinition var in file.Vars)
                {
                    if (vars_g.CEntityExists(var.Identifier()) == false)
                    {
                        vars_g.AddCEntity(var);
                    }
                }
            }

            // 然后处理外部全局变量
            foreach (CFile file in this.cfiles)
            {
                foreach (CVarDefinition var in file.VarsExtern)
                {
                    if (vars_g.CEntityExists(var.Name) == false)
                    {
                        if (var.IsExtern == false)
                        {
                            throw new Exception(var.Name + "不是外部变量!");
                        }
                        // 如果全局变量中没有定义，则认为是外部全局变量
                        vars_e.AddCEntity(var);
                    }
                }
            }
        }

        private void GetFuns(ref CEntityCollection<CFunction> funs, ref CEntityCollection<CFuncType> funs_extern)
        {
            // 首先处理函数，将它们都加入 funs
            foreach (CFile file in this.cfiles)
            {
                string cfilename = file.Name;
                foreach (CFunction fun in file.Functions)
                {
                    if (funs.CEntityExists(fun.Identifier()) == false)
                    {
                        fun.CFile = cfilename;
                        funs.AddCEntity(fun);
                    }
                }
            }

            // 然后处理类型中的函数类型
            foreach (CFile file in this.cfiles)
            {
                foreach (CType type in file.Types)
                {
                    if (type is CFuncType)
                    {
                        CFuncType funType = (CFuncType)type;
                        // 如果该函数类型的名称不是一个函数的名称
                        // 那么认定它是一个外部函数
                        if (funs.CEntityExists(funType.Name) == false)
                        {
                            funs_extern.AddCEntity(funType);
                        }
                    }
                }
            }
        }
    }
}
