using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Expression
{
    public enum OperatorType
    {
        Member_select_direct,
        Member_select_indirect,
        Inc_post,
        Inc_pre,
        Dec_post,
        Dec_pre,
        Plus,
        Minus,
        Ref,
        Deref,
        Bit_com, // bitwise complement
        Logical_neg,
        Sizeof,
        Cast,
        Mul,
        Div,
        Mod,
        Add,
        Sub,

        // Shift Expressions 
        Shift_left,
        Shift_right,

        // Relational Expressions
        LT,
        LE,
        EQ,
        NE,
        GE,
        GT,

        bit_and,
        bit_xor,
        bit_or,

        logical_and,
        logical_or,

        // 条件表达式, ? :
        Conditional,

        // Assignement And Expressions 
        Assign,
        Assign_mul,
        Assign_div,
        Assign_mod,
        Assign_add,
        Assign_sub,
        Assign_shl,
        Assign_shr,
        Assign_bit_and,
        Assign_bit_or,
        Assign_bit_xor
    }

    abstract public class CExpr: CEntity
    {
        /// <summary>
        /// 克隆，获得一个副本
        /// </summary>
        /// <returns></returns>
        abstract public CExpr Clone();

        /// <summary>
        /// 获取子表达式
        /// </summary>
        /// <returns> </returns>
        abstract public HashSet<CExpr> GetExpressions();


        /// <summary>
        /// 是不是赋值表达式
        /// 包括各种 =, +=, ..
        /// 以及 ++, --
        /// </summary>
        public bool IsAssign
        {
            get
            {
                if (this is PolyExpr)
                {
                    PolyExpr expr = (PolyExpr)this;

                    if (expr.Count == 1 &&
                        (expr.Operator == OperatorType.Inc_post ||
                         expr.Operator == OperatorType.Inc_pre ||
                         expr.Operator == OperatorType.Dec_post ||
                         expr.Operator == OperatorType.Dec_pre))
                    {
                        return true;
                    }

                    if (expr.Count == 2 &&
                        (expr.Operator == OperatorType.Assign ||
                         expr.Operator == OperatorType.Assign_add ||
                         expr.Operator == OperatorType.Assign_bit_and ||
                         expr.Operator == OperatorType.Assign_bit_or ||
                         expr.Operator == OperatorType.Assign_bit_xor ||
                         expr.Operator == OperatorType.Assign_div ||
                         expr.Operator == OperatorType.Assign_mod ||
                         expr.Operator == OperatorType.Assign_mul ||
                         expr.Operator == OperatorType.Assign_shl ||
                         expr.Operator == OperatorType.Assign_shr ||
                         expr.Operator == OperatorType.Assign_sub)
                        )
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        public override string Identifier()
        {
            return this.Name;
            //throw new NotImplementedException();
        }
        protected string GetOperator(OperatorType ot)
        {
            switch (ot)
            {
                case OperatorType.bit_or:
                    return "|";
                case OperatorType.bit_xor:
                    return "^";
                case OperatorType.bit_and:
                    return "&";
                case OperatorType.Bit_com:
                    return "~";
                case OperatorType.Ref:
                    return "&";
                case OperatorType.Deref:
                    return "*";
                case OperatorType.Member_select_direct:
                    return ".";
                case OperatorType.Member_select_indirect:
                    return "->";
                case OperatorType.Inc_post:
                case OperatorType.Inc_pre:
                    return "++";
                case OperatorType.Dec_post:
                case OperatorType.Dec_pre:
                    return "--";
                case OperatorType.Plus:
                case OperatorType.Add:
                    return "+";
                case OperatorType.Minus:
                case OperatorType.Sub:
                    return "-";
                case OperatorType.Logical_neg:
                    return "!";
                case OperatorType.Mul:
                    return "*";
                case OperatorType.Div:
                    return "/";
                case OperatorType.Mod:
                    return "%";
                case OperatorType.Shift_left:
                    return "<<";
                case OperatorType.Shift_right:
                    return ">>";
                case OperatorType.LT:
                    return "<";
                case OperatorType.GT:
                    return ">";
                case OperatorType.LE:
                    return "<=";
                case OperatorType.GE:
                    return ">=";
                case OperatorType.EQ:
                    return "==";
                case OperatorType.NE:
                    return "!=";
                case OperatorType.logical_and:
                    return "&&";
                case OperatorType.logical_or:
                    return "||";
                case OperatorType.Assign:
                    return "=";
                case OperatorType.Assign_mul:
                    return "*=";
                case OperatorType.Assign_div:
                    return "/=";
                case OperatorType.Assign_mod:
                    return "%=";
                case OperatorType.Assign_add:
                    return "+=";
                case OperatorType.Assign_sub:
                    return "-=";
                case OperatorType.Assign_shl:
                    return "<<=";
                case OperatorType.Assign_shr:
                    return ">>=";
                case OperatorType.Assign_bit_and:
                    return "&=";
                case OperatorType.Assign_bit_xor:
                    return "^=";
                case OperatorType.Assign_bit_or:
                    return "|=";
                case OperatorType.Conditional:
                default:
                    throw new Exception("unknown expression: " + ot);
            }
        }
    }
}
