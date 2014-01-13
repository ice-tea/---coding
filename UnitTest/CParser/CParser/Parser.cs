using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CParser.CFG;
using CFrontendParser.CParser.Def;
using CFrontendParser.CParser.Expression;

namespace CFrontendParser.CParser
{
    abstract class CEntityParser
    {
        /* 返回值是解析结果的名字 */
        public abstract CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc);
        
        public static CEntityParser GetParser(XmlNode node)
        {
            if (DeclareVarParser.Match(node))
            {
                // 变量声明
                return new DeclareVarParser();
            }
            else if (DeclareFunctionParser.Match(node))
            {
                // 函数声明
                return new DeclareFunctionParser();
            }
            else if (FunctionParser.Match(node))
            {
                // 函数定义
                return new FunctionParser();
            }
            else if (CExpressionParser.Match(node))
            {
                // 表达式
                return new CExpressionParser();
            }
            else if (DeclareTypeDefParser.Match(node))
            {
                // 类型声明
                return new DeclareTypeDefParser();
            }
            else if (DeclareTypeDefParser2.Match(node))
            {
                // 类型声明
                return new DeclareTypeDefParser2();
            }
            else if (DeclareStructUnionParser.Match(node))
            {
                // 类型声明
                return new DeclareStructUnionParser();
            }
            else if (StructUnionParser.Match(node))
            {
                // 类型声明
                return new StructUnionParser();
            }
            else if (CompoundStmtParser.Match(node))
            {
                // 复合语句
                return new CompoundStmtParser();
            }
            else if (IfStmtParser.Match(node))
            {
                // if分支语句
                return new IfStmtParser();
            }
            else if (SwitchStmtParser.Match(node))
            {
                // switch分支语句
                return new SwitchStmtParser();
            }
            else if (ExprStmtParser.Match(node))
            {
                // 表达式语句，例如，赋值语句
                return new ExprStmtParser();
            }
            else if (BreakStmtParser.Match(node))
            {
                // break语句
                return new BreakStmtParser();
            }
            else if (ContinueStmtParser.Match(node))
            {
                // continue语句
                return new ContinueStmtParser();
            }
            else if (ReturnStmtParser.Match(node))
            {
                // 返回语句
                return new ReturnStmtParser();
            }
            else if (ForStmtParser.Match(node))
            {
                // for 语句
                return new ForStmtParser();
            }
            else if (WhileStmtParser.Match(node))
            {
                // for 语句
                return new WhileStmtParser();
            }
            else return null;
        }

        /*
         * 处理修饰符
         * */
        protected void SetModifier(CEntity entity, XmlNode node)
        {
            if (node.Name != "declaration")
                return;

            HashSet<string> modifiers = this.GetAttributes(node, ".//storage_class_specifier", "token");
            HashSet<string> s1 = this.GetAttributes(node, ".//type_qualifier", "token");
            modifiers.UnionWith(s1);
            if (modifiers.Contains("extern"))
                entity.IsExtern = true;
            if (modifiers.Contains("static"))
                entity.IsStatic = true;
            if (modifiers.Contains("const"))
                entity.IsConst = true;
            if (modifiers.Contains("volatile"))
                entity.IsConst = true;
        }

        protected HashSet<string> GetAttributes(XmlNode node, string xpath, string attrName)
        {
            HashSet<string> attrs = new HashSet<string>();
            XmlNodeList modifierList = node.SelectNodes(xpath);
            foreach (XmlNode modifier in modifierList)
            {
                XmlAttribute attr = modifier.Attributes[attrName];
                if(attr != null)
                {
                    attrs.Add(attr.Value);
                }
            }
            return attrs;
        }

        protected string GetAttribute(XmlNode node, string xpath, string attrName)
        {
            XmlNode child = node.SelectSingleNode(xpath);
            if (child == null)
                return null;
            XmlAttribute attr = child.Attributes[attrName];
            if (attr == null)
                return null;
            return attr.Value;
        }

        protected uint ComputePtrLevel(XmlNode ptrNode)
        {
            if (ptrNode.Name != "pointer")
                return 0;

            XmlNode nextNode = ptrNode.SelectSingleNode("pointer");
            if (nextNode == null)
                return 1;
            else return 1 + ComputePtrLevel(nextNode);
        }
    }

}
