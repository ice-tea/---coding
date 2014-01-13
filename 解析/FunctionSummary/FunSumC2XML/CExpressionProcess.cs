using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

namespace FunSumC2XML
{
    public partial class Summarization
    {
        private string ProcessCExpression(XmlNode node)
        {
            string nodeName = node.Name;
            switch (nodeName)
            {
                case "expression":
                    {
                        XmlNode child = node.ChildNodes[0];
                        return this.ProcessCExpression(child);
                    }
                case "expression_id":
                case "expression_cte":
                    return node.Attributes["token"].Value;
                case "expression_array":
                    {
                        XmlNode c1 = node.ChildNodes[0];
                        XmlNode c2 = node.ChildNodes[1];
                        string s1 = this.ProcessCExpression(c1);
                        string s2 = this.ProcessCExpression(c2);
                        return s1 + "[" + s2 + "]";
                    }
                case "expression_function":
                    {
                        int n = node.ChildNodes.Count;
                        XmlNode funNode = node.ChildNodes[0];
                        string result = this.ProcessCExpression(funNode);
                        string paras = "";
                        XmlNode paraNode = null;
                        for (int i = 1; i < n - 1; i++ )
                        {
                            paraNode = node.ChildNodes[i];
                            string para = this.ProcessCExpression(paraNode);
                            paras += para + ", ";
                        }
                        if (n > 1)
                        {
                            paraNode = node.ChildNodes[n - 1];
                            string para = this.ProcessCExpression(paraNode);
                            paras += para;
                        }
                        return result + "(" + paras + ")";
                    }
                case "expression_brackets":
                    {
                        XmlNode child = node.ChildNodes[0];
                        string s = this.ProcessCExpression(child);
                        return "(" + s + ")";
                    }
                case "expression_cast":
                    return this.ProcessExpressionCast(node);
                case "expression_dereference":
                case "expression_reference":
                case "expression_plus":
                case "expression_minus":
                case "expression_bitwise_complement":
                case "expression_logical_neg":
                    {
                        XmlNode child = node.ChildNodes[0];
                        string s = this.ProcessCExpression(child);
                        string op = this.GetOperator(nodeName);
                        return op + s;
                    }
                case "expression_member":
                    {
                        return node.Attributes["token"].Value;
                    }
                case "expression_post_inc":
                case "expression_post_dec":
                    {
                        XmlNode c1 = node.ChildNodes[0];
                        string s1 = this.ProcessCExpression(c1);
                        string op = this.GetOperator(nodeName);
                        return s1 + op;
                    }
                case "expression_pre_inc":
                case "expression_pre_dec":
                    {
                        XmlNode c1 = node.ChildNodes[0];
                        string s1 = this.ProcessCExpression(c1);
                        string op = this.GetOperator(nodeName);
                        return op + s1;
                    }
                case "expression_direct_member_selector":
                case "expression_indirect_member_selector":
                case "expression_bitwise_and":
                case "expression_bitwise_or":
                case "expression_bitwise_xor":
                case "expression_mul":
                case "expression_div":
                case "expression_mod":
                case "expression_add":
                case "expression_sub":
                case "expression_shift_left":
                case "expression_shift_right":
                case "expression_lesser":
                case "expression_greater":
                case "expression_lesser_equal":
                case "expression_greater_equal":
                case "expression_equal":
                case "expression_not_equal":
                case "expression_logical_and":
                case "expression_logical_or":
                case "expression_assignment":
                case "expression_mul_assignment":
                case "expression_div_assignment":
                case "expression_mod_assignment":
                case "expression_add_assignment":
                case "expression_sub_assignment":
                case "expression_shl_assignment":
                case "expression_shr_assignment":
                case "expression_bitwise_and_assignment":
                case "expression_bitwise_xor_assignment":
                case "expression_bitwise_or_assignment":
                    {
                        XmlNode c1 = node.ChildNodes[0];
                        XmlNode c2 = node.ChildNodes[1];
                        string s1 = this.ProcessCExpression(c1);
                        string s2 = this.ProcessCExpression(c2);
                        string op = this.GetOperator(nodeName);
                        //return s1 + " " + op + " " + s2;
                        return s1 + op + s2;
                    }
                case "expression_conditional":
                    return this.ProcessExpressionConditional(node);
                case "expression_sizeof":
                    return this.ProcessExpressionSizeof(node);
                case "type_name":
                    return this.ProcessTypeName(node);
                default:
                    throw new Exception("unknown expression: " + nodeName);
            }
        }

        private string GetOperator(string nodeName)
        {
            switch (nodeName)
            {
                case "expression_bitwise_or":
                    return "|";
                case "expression_bitwise_xor":
                    return "^";
                case "expression_bitwise_and":
                    return "&";
                case "expression_reference":
                    return "&";
                case "expression_dereference":
                    return "*";
                case "expression_direct_member_selector":
                    return ".";
                case "expression_indirect_member_selector":
                    return "->";
                case "expression_pre_inc":
                case "expression_post_inc":
                    return "++";
                case "expression_pre_dec":
                case "expression_post_dec":
                    return "--";
                case "expression_plus":
                case "expression_add":
                    return "+";
                case "expression_minus":
                case "expression_sub":
                    return "-";
                case "expression_bitwise_complement":
                    return "~";
                case "expression_logical_neg":
                    return "!";
                case "expression_mul":
                    return "*";
                case "expression_div":
                    return "/";
                case "expression_mod":
                    return "%";
                case "expression_shift_left":
                    return "<<";
                case "expression_shift_right":
                    return ">>";
                case "expression_lesser":
                    return "<";
                case "expression_greater":
                    return ">";
                case "expression_lesser_equal":
                    return "<=";
                case "expression_greater_equal":
                    return ">=";
                case "expression_equal":
                    return "==";
                case "expression_not_equal":
                    return "!=";
                case "expression_logical_and":
                    return "&&";
                case "expression_logical_or":
                    return "||";
                case "expression_conditional":
                    return "? :";
                case "expression_assignment":
                    return "=";
                case "expression_mul_assignment":
                    return "*=";
                case "expression_div_assignment":
                    return "/=";
                case "expression_mod_assignment":
                    return "%=";
                case "expression_add_assignment":
                    return "+=";
                case "expression_sub_assignment":
                    return "-=";
                case "expression_shl_assignment":
                    return "<<=";
                case "expression_shr_assignment":
                    return ">>=";
                case "expression_bitwise_and_assignment":
                    return "&=";
                case "expression_bitwise_xor_assignment":
                    return "^=";
                case "expression_bitwise_or_assignment":
                    return "|=";
                default:
                    throw new Exception("unknown expression: " + nodeName);
            }
        }

        /*
         * 处理 类型强制表达式
         * */
        private string ProcessExpressionCast(XmlNode node)
        {
            if (node.Name != "expression_cast")
                return "";
            // 首先处理强制类型
            XmlNode c1 = node.ChildNodes[0];
            string s1 = this.ProcessCExpression(c1);
            // 然后处理被强制转换的表达式
            XmlNode c2 = node.ChildNodes[1];
            string s2 = this.ProcessCExpression(c2);
            return "(" + s1 + ")" + s2;
        }

        /*
         * 处理 e1 ? e2 : e3
         */
        private string ProcessExpressionConditional(XmlNode node)
        {
            if (node.Name != "expression_conditional" && node.ChildNodes.Count != 3)
                return "";
            string[] strs = new string[3];
            int i = 0;
            foreach (XmlNode child in node.ChildNodes)
            {
                strs[i] = this.ProcessCExpression(child);
                i++;
            }
            return strs[0] + " ? " + strs[1] + " : " + strs[2];
        }

        /*
         * 处理 sizeof
         * */
        private string ProcessExpressionSizeof(XmlNode node)
        {
            if (node.Name != "expression_sizeof")
                return "";
            XmlNode c = node.ChildNodes[0];
            string typeName = this.ProcessCExpression(c);
            return "sizeof" + "(" + typeName + ")";
        }

        private string ProcessTypeName(XmlNode node)
        {
            if (node.Name != "type_name")
                return "";
            string result = "";
            foreach (XmlNode child in node.ChildNodes)
            {
                XmlAttribute attr = child.Attributes["token"];
                if (attr != null)
                    result += attr.Value;
            }
            // 处理可能的指针
            XmlNode ptrNode = node.SelectSingleNode("declarator/pointer");
            if (ptrNode != null)
            {
                uint level = this.ComputePtrLevel(ptrNode);
                while (level > 0)
                {
                    result += "*";
                    level--;
                }
            }
            return result;
        }
    }
}
