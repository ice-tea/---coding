using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.Expression
{
    public class PolyExpr: CExpr
    {
        private OperatorType _operator;
        public OperatorType Operator { get { return this._operator; } }

        private List<CExpr> _operands;
        public int Count { get { return this._operands.Count; } }
        public CExpr this[int idx]
        {
            get { return this._operands.ElementAt(idx); }
        }

        public PolyExpr(OperatorType op, CExpr expr)
        {
            this._operator = op;
            this._operands = new List<CExpr>();
            this._operands.Add(expr);
        }

        public PolyExpr(OperatorType op, CExpr e1, CExpr e2)
        {
            this._operator = op;
            this._operands = new List<CExpr>();
            this._operands.Add(e1);
            this._operands.Add(e2);
        }

        public PolyExpr(OperatorType op, CExpr e1, CExpr e2, CExpr e3)
        {
            this._operator = op;
            this._operands = new List<CExpr>();
            this._operands.Add(e1);
            this._operands.Add(e2);
            this._operands.Add(e3);
        }

        public override string ToString()
        {
            switch (this._operator)
            {
                case OperatorType.Deref:
                case OperatorType.Ref:
                case OperatorType.Plus:
                case OperatorType.Minus:
                case OperatorType.Bit_com:
                case OperatorType.Logical_neg:
                case OperatorType.Dec_pre:
                case OperatorType.Inc_pre:
                    {
                        CExpr child = this._operands[0];
                        string s = this.GetOperator(this._operator);
                        return s + child.ToString();
                    }
                case OperatorType.Dec_post:
                case OperatorType.Inc_post:
                    {
                        CExpr child = this._operands[0];
                        string s = this.GetOperator(this._operator);
                        return child.ToString() + s;
                    }
                case OperatorType.Member_select_direct:
                case OperatorType.Member_select_indirect:
                case OperatorType.bit_and:
                case OperatorType.bit_or:
                case OperatorType.bit_xor:
                case OperatorType.Mul:
                case OperatorType.Div:
                case OperatorType.Mod:
                case OperatorType.Add:
                case OperatorType.Sub:
                case OperatorType.Shift_left:
                case OperatorType.Shift_right:
                case OperatorType.LT:
                case OperatorType.GT:
                case OperatorType.LE:
                case OperatorType.GE:
                case OperatorType.EQ:
                case OperatorType.NE:
                case OperatorType.logical_and:
                case OperatorType.logical_or:
                case OperatorType.Assign:
                case OperatorType.Assign_mul:
                case OperatorType.Assign_div:
                case OperatorType.Assign_mod:
                case OperatorType.Assign_add:
                case OperatorType.Assign_sub:
                case OperatorType.Assign_shl:
                case OperatorType.Assign_shr:
                case OperatorType.Assign_bit_xor:
                case OperatorType.Assign_bit_and:
                case OperatorType.Assign_bit_or:
                    {
                        CExpr c1 = this._operands[0];
                        CExpr c2 = this._operands[1];
                        string op = this.GetOperator(this._operator);
                        string s1 = c1.ToString();
                        string s2 = c2.ToString();
                        return s1 + op + s2;
                    }
                case OperatorType.Conditional:
                    {
                        CExpr c1 = this._operands[0];
                        CExpr c2 = this._operands[1];
                        CExpr c3 = this._operands[2];
                        return c1.ToString() + "?" + c2.ToString() + ":" + c3.ToString();
                    }
                default:
                    throw new Exception("unknown expression: " + this._operator);
            }
        }

        public override HashSet<CExpr> GetExpressions()
        {
            HashSet<CExpr> result = new HashSet<CExpr>();
            foreach (CExpr expr in this._operands)
            {
                HashSet<CExpr> set = expr.GetExpressions();
                result.UnionWith(set);
            }
            return result;
        }

        public override CExpr Clone()
        {
            if (this._operands.Count == 1)
            {
                return new PolyExpr(this._operator, this._operands[0].Clone());
            }
            else if (this._operands.Count == 2)
            {
                return new PolyExpr(this._operator, this._operands[0].Clone(), this._operands[1].Clone());
            }
            else
            {
                return new PolyExpr(this._operator, this._operands[0].Clone(), this._operands[1].Clone(), this._operands[2].Clone());
            }
        }
    }
}
