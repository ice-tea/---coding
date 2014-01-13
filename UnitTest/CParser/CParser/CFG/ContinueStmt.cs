using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CSyntax.CFG;

namespace CFrontendParser.CParser.CFG
{
    class ContinueStmtParser: CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            return new SimpleStmt(StmtType._break);
        }

        public static bool Match(XmlNode node)
        {
            if (node.Name == "continue_statement")
            {
                return true;
            }
            else return false;
        }
    }
}
