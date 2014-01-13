using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using CFrontendParser.CParser;
using CFrontendParser.CParser.Expression;
using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CSyntax.CFG;
using CFrontendParser.CSyntax.Expression;

namespace CFrontendParser.CParser.CFG
{
    class SwitchStmtParser: CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            CExpr expression = null;
            List<CPair<HashSet<CExpr>, List<CStmt>>> cases = new List<CPair<HashSet<CExpr>, List<CStmt>>>();
            List<CStmt> defaultStmts = new List<CStmt>();
            CEntity entity = null;

            // 首先，处理switch表达式
            XmlNode exprNode = node.ChildNodes[0];
            CExpressionParser exprParser = new CExpressionParser();
            entity = exprParser.Parse(exprNode, cvc, ctc);
            if (entity is CExpr)
            {
                expression = (CExpr)entity;
            }
            else
            {
                throw new Exception("invalid switch expression: " + entity.ToString());
            }

            // 然后，查找各个case语句和default语句的位置
            XmlNode stmtsNode = node.ChildNodes[1];
            List<int> casesPos = new List<int>();
            int defaultPos = -1;
            int i = 0;
            foreach (XmlNode child in stmtsNode)
            {
                if (child.Name == "case_statement")
                {
                    casesPos.Add(i);
                }
                else if (child.Name == "default_statement")
                {
                    defaultPos = i;
                }
                i++;
            }

            // 接下来，先处理各个case语句
            for (i = 0; i < casesPos.Count;i++ )
            {
                int pos = casesPos[i];
                int end = (i == casesPos.Count - 1) ? defaultPos : casesPos[i + 1];
                XmlNode caseNode = stmtsNode.ChildNodes[pos];

                // 1. 处理case表达式
                HashSet<CExpr> caseExpr = new HashSet<CExpr>();
                XmlNode stmtNode = this.ProcessCaseExprs(caseNode, cvc, ctc, ref caseExpr);

                // 2. 处理case分支的语句
                List<CStmt> stmts = new List<CStmt>();
                // 2.1 处理紧跟着case表达式的语句
                CEntityParser stmtParser = CEntityParser.GetParser(stmtNode);
                entity = stmtParser.Parse(stmtNode, cvc, ctc);
                if (entity is CStmt)
                {
                    stmts.Add((CStmt)entity);
                }
                else
                {
                    throw new Exception("invalid switch case statement: " + entity.ToString());
                } 
                // 2.2 处理其后的语句
                for (int j = pos + 1; j < end; j++)
                {
                    XmlNode restNode = stmtsNode.ChildNodes[j];
                    CEntityParser restParser = CEntityParser.GetParser(restNode);
                    entity = restParser.Parse(restNode, cvc, ctc);
                    if (entity is CStmt)
                    {
                        stmts.Add((CStmt)entity);
                    }
                    else
                    {
                        throw new Exception("invalid switch case statement: " + entity.ToString());
                    } 
                }

                // 3. 构造case分支
                CPair<HashSet<CExpr>, List<CStmt>> caseItem = new CPair<HashSet<CExpr>, List<CStmt>>(caseExpr, stmts);
                cases.Add(caseItem);
            }

            // 最后处理default语句
            // 1. 处理紧跟着default的语句
            XmlNode defaultNode = stmtsNode.ChildNodes[defaultPos];
            CEntityParser defaultParser = CEntityParser.GetParser(defaultNode.ChildNodes[0]);
            entity = defaultParser.Parse(defaultNode.ChildNodes[0], cvc, ctc);
            if (entity is CStmt)
            {
                defaultStmts.Add((CStmt)entity);
            }
            else
            {
                throw new Exception("invalid switch case statement: " + entity.ToString());
            } 
            // 2. 处理其后的语句
            for (int j = defaultPos + 1; j < stmtsNode.ChildNodes.Count; j++)
            {
                XmlNode restNode = stmtsNode.ChildNodes[j];
                CEntityParser restParser = CEntityParser.GetParser(restNode);
                entity = restParser.Parse(restNode, cvc, ctc);
                if (entity is CStmt)
                {
                    defaultStmts.Add((CStmt)entity);
                }
                else
                {
                    throw new Exception("invalid switch default statement: " + entity.ToString());
                } 
            }

            // 构造switch-case 语句， 作为返回值
            SwitchStmt switchStmt = new SwitchStmt(expression, cases, defaultStmts);
            return switchStmt;
        }

        private XmlNode ProcessCaseExprs(XmlNode caseNode, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc, ref HashSet<CExpr> exprs)
        {
            if (caseNode.Name != "case_statement")
                return caseNode;
            XmlNode caseExprNode = caseNode.ChildNodes[0];
            CExpressionParser caseParser = new CExpressionParser();
            CEntity entity = caseParser.Parse(caseExprNode, cvc, ctc);
            if (entity is CExpr)
            {
                exprs.Add((CExpr)entity);
                XmlNode stmtNode = caseNode.ChildNodes[1];
                return this.ProcessCaseExprs(stmtNode, cvc, ctc, ref exprs);
            }
            else
            {
                throw new Exception("invalid switch expression: " + entity.ToString());
            } 
        }

        public static bool Match(XmlNode node)
        {
            /* 这里强制要求 switch语句 的语句体必须包裹在花括号之间 */
            if (node.Name == "switch_statement" && 
                node.ChildNodes.Count == 2 &&
                node.ChildNodes[1].Name == "compound_statement")
            {
                return true;
            }
            else return false;
        }
    }
}
