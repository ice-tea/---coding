using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using CFrontendParser.CParser;
using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CSyntax.CFG;
using CFrontendParser.CSyntax.Expression;
using CFrontendParser.CParser.Expression;

namespace CFrontendParser.CParser.CFG
{
    class WhileStmtParser: CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            CExpr condition = null;
            CStmt body = null;
            CEntity entity = null;
            int cPos = 0;
            int bPos = 1;
            WhileMark mark = WhileMark._while;

            if (node.Name == "do_statement")
            {
                cPos = 1;
                bPos = 0;
                mark = WhileMark._do_while;
            }

            // 1. 处理循环条件
            CExpressionParser parser1 = new CExpressionParser();
            entity = parser1.Parse(node.ChildNodes[cPos], cvc, ctc);
            if (entity is CExpr)
            {
                condition = (CExpr)entity;
            }
            else
            {
                throw new Exception("invalid while statement");
            }

            // 2. 处理循环体
            XmlNode bodyNode = node.ChildNodes[bPos];
            CEntityParser parser2 = CEntityParser.GetParser(bodyNode);
            entity = parser2.Parse(bodyNode, cvc, ctc);
            if (entity is CStmt)
            {
                body = (CStmt)entity;
            }
            else
            {
                throw new Exception("invalid while statement");
            }

            // 3. 构造while语句
            WhileStmt stmt = new WhileStmt(mark, condition, body);
            return stmt;
        }

        public static bool Match(XmlNode node)
        {
            if (node.Name == "while_statement" || node.Name == "do_statement")
            {
                return true;
            }
            else return false;
        }
    }
}
