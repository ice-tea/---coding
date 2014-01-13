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
    class IfStmtParser: CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            CExpr condition = null;
            CStmt trueBranch = null;
            CStmt falseBranch = null;
            CEntity entity = null;

            // 首先，处理条件表达式
            XmlNode conditionNode = node.ChildNodes[0];
            CExpressionParser eParser = new CExpressionParser();
            entity = eParser.Parse(conditionNode, cvc, ctc);
            if (entity is CExpr)
            {
                condition = (CExpr)entity;
            }
            else
            {
                throw new Exception("invalid if statement condition");
            }

            // 然后，处理真分支语句
            XmlNode trueNode = node.ChildNodes[1];
            CEntityParser tParser = CEntityParser.GetParser(trueNode);
            entity = tParser.Parse(trueNode, cvc, ctc);
            if (entity is CStmt)
            {
                trueBranch = (CStmt)entity;
            }
            else
            {
                throw new Exception("invalid if statement condition");
            }

            // 如果有假分支，则继续处理假分支语句
            if (node.ChildNodes.Count > 2)
            {
                XmlNode falseNode = node.ChildNodes[1];
                CEntityParser fParser = CEntityParser.GetParser(falseNode);
                entity = fParser.Parse(falseNode, cvc, ctc);
                if (entity is CStmt)
                {
                    falseBranch = (CStmt)entity;
                }
                else
                {
                    throw new Exception("invalid if statement condition");
                }
            }

            IfStmt stmt = new IfStmt(condition, trueBranch, falseBranch);
            return stmt;
        }
        
        public static bool Match(XmlNode node)
        {
            if (node.Name == "if_statement")
            {
                return true;
            }
            else return false;
        }
    }
}
