using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CSyntax.CFG;
using CFrontendParser.CParser.Def;

namespace CFrontendParser.CParser.CFG
{
    class CompoundStmtParser: CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            // 首先，处理局部变量声明
            CEntityCollection<CVarDefinition> localVars = new CEntityCollection<CVarDefinition>();
            DeclareVarParser parser1 = new DeclareVarParser();
            int idx;
            for (idx = 0; idx < node.ChildNodes.Count; idx++)
            {
                XmlNode child = node.ChildNodes[idx];
                if (child.Name == "declaration")
                {
                    CEntity entity = parser1.Parse(child, localVars, ctc);
                    if (!(entity is CVarDefinition))
                    {
                        throw new Exception("invalid local variable declaration: " + entity.Name);
                    }
                }
                else break;
            }

            // 然后，处理其他语句
            List<CStmt> stmts = new List<CStmt>();
            while (idx < node.ChildNodes.Count)
            {
                XmlNode child = node.ChildNodes[idx];
                CEntityParser parser2 = CEntityParser.GetParser(child);
                CEntityCollection<CVarDefinition> localVars2 = new CEntityCollection<CVarDefinition>();
                CEntity entity = parser2.Parse(child, localVars2, ctc);
                if ( entity is CStmt)
                {
                    stmts.Add((CStmt)entity);
                }
                else
                {
                    throw new Exception("invalid statement");
                }
                idx++;
            }

            CompoundStmt cStmt = new CompoundStmt(localVars, stmts);
            return cStmt;
        }

        public static bool Match(XmlNode node)
        {
            if (node.Name == "compound_statement")
            {
                return true;
            }
            else return false;
        }
    }
}
