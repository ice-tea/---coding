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
    /*
     * 表达式语句
     * <!ELEMENT expression_statement (expression*)>
     * */
    class ExprStmtParser: CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            List<CExpr> expressions = new List<CExpr>();
            foreach (XmlNode child in node.ChildNodes)
            {
                CExpressionParser parser = new CExpressionParser();
                CEntity entity = parser.Parse(child, cvc, ctc);
                if (entity is CExpr)
                {
                    expressions.Add((CExpr)entity);
                }
                else
                {
                    throw new Exception("invalid expression in expression statement");
                }
            }
            ExprStmt stmt = new ExprStmt(expressions);
            return stmt;
        }


        public static bool Match(XmlNode node)
        {
            if (node.Name == "expression_statement")
            {
                return true;
            }
            else return false;
        }
    }
}
