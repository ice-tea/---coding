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
    /*
     * 用于解析带标号的语句
     */
    class LabelStmtParser: CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            string label = this.GetAttribute(node, "label_id", "token");
            XmlNode stmtNode = node.ChildNodes[1];
            CEntityParser parser = CEntityParser.GetParser(stmtNode);
            CEntity entity = parser.Parse(stmtNode, cvc, ctc);
            if (entity is CStmt)
            {
                CStmt stmt = (CStmt)entity;
                stmt.Labels.Insert(0, label);
                return stmt;
            }
            else
            {
                throw new Exception("invalid label statement");
            }
        }

        public static bool Match(XmlNode node)
        {
            if (node.Name == "label_statement")
            {
                return true;
            }
            else return false;
        }
    }
}
