using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CSyntax.CFG;
using CFrontendParser.CSyntax.Expression;
using CFrontendParser.CParser.Expression;

namespace CFrontendParser.CParser.CFG
{
    class ReturnStmtParser: CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            CExpr returnValue = null;
            if (node.ChildNodes.Count > 0)
            {
                CExpressionParser parser = new CExpressionParser();
                CEntity entity = parser.Parse(node.ChildNodes[0], cvc, ctc);
                if (entity is CExpr)
                {
                    returnValue = (CExpr)entity;
                }
            }
            return new ReturnStmt(returnValue);
        }

        public static bool Match(XmlNode node)
        {
            if (node.Name == "return_statement")
            {
                return true;
            }
            else return false;
        }
    }
}
