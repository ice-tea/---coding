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

namespace CFrontendParser.CParser.Expression
{
    partial class CExpressionParser : CEntityParser
    {
        private CExpr ParseExpressionSizeof(XmlNode node, CEntityCollection<CVarDefinition> vars, CEntityCollection<CType> types)
        {
            if (node.Name != "expression_sizeof")
                return null;
            XmlNode child = node.ChildNodes[0];
            if (child.Name == "type_name")
            {
                CType type = this.ParseTypeName(child, types);
                return new ExprSizeof(type);
            }
            else if (child.Name == "expression_brackets")
            {
                CExpressionParser parser = new CExpressionParser();
                CExpr expr = (CExpr)parser.Parse(child.ChildNodes[0], vars, types);
                return new ExprSizeof(expr);
            }
            else
            {
                throw new Exception("invalid sizeof: " + child.Name);
            }
        }

        private ExprConst ParseConstant(string value)
        {
            if (value.StartsWith("0x") || value.StartsWith("0X"))
            {
                return ExprConst.GetConst(value);
            }

            try
            {
                return ExprConst.GetConst(System.Convert.ToInt32(value));
            }
            catch
            {
                try
                {
                    return ExprConst.GetConst(float.Parse(value));
                }
                catch
                {
                    try
                    {
                        return ExprConst.GetConst(double.Parse(value));
                    }
                    catch
                    {
                        return ExprConst.GetConst(0);
                    }
                }
            }
        }

        private CExpr ParseIntializer(XmlNode node, CEntityCollection<CType> types)
        {
            if (node.Name != "initializer")
                throw new Exception("invalid initializer: " + node.Name);
            if (node.ChildNodes.Count == 1)
            {
                CExpressionParser parser = new CExpressionParser();
                CExpr initializer = (CExpr)parser.Parse(node.FirstChild, null, types);
                return initializer;
            }
            else
            {
                List<CExpr> initializers = new List<CExpr>();
                foreach (XmlNode initialNode in node.ChildNodes)
                {
                    CExpressionParser parser = new CExpressionParser();
                    CExpr initializer = (CExpr)parser.Parse(initialNode, null, types);
                    initializers.Add(initializer);
                }
                ExprInitializer expr = new ExprInitializer(initializers);
                return expr;
            }
        }

        private CExpr ParseExpressionCast(XmlNode node, CEntityCollection<CVarDefinition> vars, CEntityCollection<CType> types)
        {
            if (node.Name != "expression_cast")
                return null;
            // 首先处理强制类型
            XmlNode c1 = node.ChildNodes[0];
            CType type = this.ParseTypeName(c1, types);
            // 然后处理被强制转换的表达式
            XmlNode c2 = node.ChildNodes[1];
            CExpr e = (CExpr)this.Parse(c2, vars, types);
            return new ExprCast(type, e);
        }

        //ADD LIBO
        private CExpr ParseExpressionStr(XmlNode node, CEntityCollection<CVarDefinition> vars, CEntityCollection<CType> types)
        {
            if (node.Name != "expression_str")
                return null;
            string s = "";
            XmlAttribute attr =  node.Attributes["token"];
            s = attr.Value;
            //处理多余的 双引号
            return new ExprStr(s);
        }

        private CType ParseTypeName(XmlNode node, CEntityCollection<CType> types)
        {
            if (node.Name != "type_name")
                throw new Exception("invalid type cast XML node: " + node.Name);

            string typeName = "";
            foreach (XmlNode child in node.ChildNodes)
            {
                XmlAttribute attr = child.Attributes["token"];
                if (attr != null)
                    typeName += attr.Value + " ";
            }
            if (typeName.Contains("volatile"))
            {
                typeName = typeName.Replace("volatile", "");
            }
            typeName = typeName.Trim();
            CType type0 = types.GetCEntity(typeName);
            // 处理可能的指针
            XmlNode ptrNode = node.SelectSingleNode("declarator/pointer");
            if (ptrNode != null)
            {
                uint level = this.ComputePtrLevel(ptrNode);
                CPtrType type1 = new CPtrType(type0, level);
                types.AddCEntity(type1);
                return type1;
            }
            else
            {
                return type0;
            }
        }
    }
}
