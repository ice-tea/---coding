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
    class ForStmtParser: CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            ExprStmt initStmt = null;
            CExpr condition = null;
            CExpr postExpr = null;
            CStmt body = null;
            CEntity entity = null;

            // 1. 处理初始语句
            ExprStmtParser parser1 = new ExprStmtParser();
            entity = parser1.Parse(node.ChildNodes[0], cvc, ctc);
            if (entity is ExprStmt)
            {
                initStmt = (ExprStmt)entity;
            }
            else
            {
                throw new Exception("invalid for statement");
            }

            // 2. 处理循环条件
            CExpressionParser parser2 = new CExpressionParser();
            entity = parser2.Parse(node.ChildNodes[1].ChildNodes[0], cvc, ctc);
            if (entity is CExpr)
            {
                condition = (CExpr)entity;
            }
            else
            {
                throw new Exception("invalid for statement");
            }

            // 3. 处理 post 表达式
            if (node.ChildNodes.Count == 4)
            {
                CExpressionParser parser3 = new CExpressionParser();
                entity = parser3.Parse(node.ChildNodes[2], cvc, ctc);
                if (entity is CExpr)
                {
                    postExpr = (CExpr)entity;
                }
                else
                {
                    throw new Exception("invalid for statement");
                }
            }
            else
            {
                postExpr = new EmptyExpr();
            }

            // 4. 处理循环体
            XmlNode bodyNode = node.ChildNodes[node.ChildNodes.Count - 1];
            CEntityParser parser4 = CEntityParser.GetParser(bodyNode);
            entity = parser4.Parse(bodyNode, cvc, ctc);
            if (entity is CStmt)
            {
                body = (CStmt)entity;
            }
            else
            {
                throw new Exception("invalid for statement");
            }

            // 5. 构造 for 语句
            ForStmt stmt = new ForStmt(initStmt, condition, postExpr, body);
            return stmt;

        }

        public static bool Match(XmlNode node)
        {
            if (node.Name == "for_statement")
            {
                return true;
            }
            else return false;
        }
    }
}
