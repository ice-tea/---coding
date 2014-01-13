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
    partial class CExpressionParser: CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            string nodeName = node.Name;
            switch (nodeName)
            {
                case "expression":
                    {
                        XmlNode child = node.ChildNodes[0];
                        return this.Parse(child, cvc, ctc);
                    }
                case "expression_id":
                    {
                        string idStr = node.Attributes["token"].Value;
                        return new ExprID(idStr);
                    }
                case "expression_cte":
                    {
                        string valueStr = node.Attributes["token"].Value;
                        return this.ParseConstant(valueStr);
                    }
                case "expression_array":
                    {
                        XmlNode c1 = node.ChildNodes[0];
                        XmlNode c2 = node.ChildNodes[1];
                        CExpr e1 = (CExpr)this.Parse(c1, cvc, ctc);
                        CExpr e2 = (CExpr)this.Parse(c2, cvc, ctc);
                        if (e1 is ExprArray)
                        {
                            ExprArray array = (ExprArray)e1;
                            array[array.Count] = e2;
                            return array;
                        }
                        else
                        {
                            return new ExprArray(e1, e2);
                        }
                    }
                case "expression_function":
                    {
                        int n = node.ChildNodes.Count;
                        XmlNode funNode = node.ChildNodes[0];
                        string funcName = this.GetAttribute(node, "expression_id", "token");
                        List<CExpr> args = new List<CExpr>();
                        
                        for (int i = 1; i < node.ChildNodes.Count; i++)
                        {
                            XmlNode argumentNode = node.ChildNodes[i];
                            CExpr arg = (CExpr)this.Parse(argumentNode, cvc, ctc);
                            args.Add(arg);
                        }
                        //Libo： CfuntionType 写入
                        return new ExprFunCall(funcName, args);
                    }
                case "expression_brackets":
                    {
                        XmlNode child = node.ChildNodes[0];
                        CExpr e = (CExpr)this.Parse(child, cvc, ctc);
                        return new ExprBracket(e);
                    }
                case "expression_member":
                    {
                        string token = node.Attributes["token"].Value;
                        return new ExprMem(token);
                    }
                case "expression_dereference":
                case "expression_reference":
                case "expression_plus":
                case "expression_minus":
                case "expression_bitwise_complement":
                case "expression_logical_neg":
                case "expression_post_inc":
                case "expression_post_dec":
                case "expression_pre_inc":
                case "expression_pre_dec":
                    {
                        XmlNode child = node.ChildNodes[0];
                        CExpr e1 = (CExpr)this.Parse(child, cvc, ctc);
                        OperatorType op = this.GetOperator(nodeName);
                        return new PolyExpr(op, e1);
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
                        CExpr e1 = (CExpr)this.Parse(c1, cvc, ctc);
                        CExpr e2 = (CExpr)this.Parse(c2, cvc, ctc);
                        OperatorType op = this.GetOperator(nodeName);
                        return new PolyExpr(op, e1, e2);
                    }
                case "expression_conditional":
                    {
                        XmlNode c1 = node.ChildNodes[0];
                        XmlNode c2 = node.ChildNodes[1];
                        XmlNode c3 = node.ChildNodes[2];
                        CExpr e1 = (CExpr)this.Parse(c1, cvc, ctc);
                        CExpr e2 = (CExpr)this.Parse(c2, cvc, ctc);
                        CExpr e3 = (CExpr)this.Parse(c2, cvc, ctc);
                        return new PolyExpr(OperatorType.Conditional, e1, e2, e3);
                    }
                case "expression_cast":
                    return this.ParseExpressionCast(node, cvc, ctc);
                //ADD LIBO
                case "expression_str":
                    return this.ParseExpressionStr(node, cvc, ctc);
                case "expression_sizeof":
                    return this.ParseExpressionSizeof(node, cvc, ctc);
                case "initializer":
                    return this.ParseIntializer(node, ctc);
                case "type_name":
                default:
                    throw new Exception("unknown expression: " + nodeName);
            }
        }

        public static bool Match(XmlNode node)
        {
            if (node.Name == "expression")
            {
                return true;
            }
            else return false;
        }

        private OperatorType GetOperator(string nodeName)
        {
            switch (nodeName)
            {
                case "expression_bitwise_or":
                    return OperatorType.bit_or;
                case "expression_bitwise_xor":
                    return OperatorType.bit_xor;
                case "expression_bitwise_and":
                    return OperatorType.bit_and;
                case "expression_reference":
                    return OperatorType.Ref;
                case "expression_dereference":
                    return OperatorType.Deref;
                case "expression_direct_member_selector":
                    return OperatorType.Member_select_direct;
                case "expression_indirect_member_selector":
                    return OperatorType.Member_select_indirect;
                case "expression_pre_inc":
                    return OperatorType.Inc_pre;
                case "expression_post_inc":
                    return OperatorType.Inc_post;
                case "expression_pre_dec":
                    return OperatorType.Dec_pre;
                case "expression_post_dec":
                    return OperatorType.Dec_post;
                case "expression_plus":
                    return OperatorType.Plus;
                case "expression_add":
                    return OperatorType.Add;
                case "expression_minus":
                    return OperatorType.Minus;
                case "expression_sub":
                    return OperatorType.Sub;
                case "expression_bitwise_complement":
                    return OperatorType.Bit_com;
                case "expression_logical_neg":
                    return OperatorType.Logical_neg;
                case "expression_mul":
                    return OperatorType.Mul;
                case "expression_div":
                    return OperatorType.Div;
                case "expression_mod":
                    return OperatorType.Mod;
                case "expression_shift_left":
                    return OperatorType.Shift_left;
                case "expression_shift_right":
                    return OperatorType.Shift_right;
                case "expression_lesser":
                    return OperatorType.LT;
                case "expression_greater":
                    return OperatorType.GT;
                case "expression_lesser_equal":
                    return OperatorType.LE;
                case "expression_greater_equal":
                    return OperatorType.GE;
                case "expression_equal":
                    return OperatorType.EQ;
                case "expression_not_equal":
                    return OperatorType.NE;
                case "expression_logical_and":
                    return OperatorType.logical_and;
                case "expression_logical_or":
                    return OperatorType.logical_or;
                case "expression_assignment":
                    return OperatorType.Assign;
                case "expression_mul_assignment":
                    return OperatorType.Assign_mul;
                case "expression_div_assignment":
                    return OperatorType.Assign_div;
                case "expression_mod_assignment":
                    return OperatorType.Assign_mod;
                case "expression_add_assignment":
                    return OperatorType.Assign_add;
                case "expression_sub_assignment":
                    return OperatorType.Assign_sub;
                case "expression_shl_assignment":
                    return OperatorType.Assign_shl;
                case "expression_shr_assignment":
                    return OperatorType.Assign_shr;
                case "expression_bitwise_and_assignment":
                    return OperatorType.Assign_bit_and;
                case "expression_bitwise_xor_assignment":
                    return OperatorType.Assign_bit_xor;
                case "expression_bitwise_or_assignment":
                    return OperatorType.Assign_bit_or;
                case "expression_conditional":
                default:
                    throw new Exception("unknown expression: " + nodeName);
            }
        }

        private string GetOperatorStr(string nodeName)
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
                case "expression_conditional":
                default:
                    throw new Exception("unknown expression: " + nodeName);
            }
        }
    }
}
