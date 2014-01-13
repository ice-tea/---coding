using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Expression;
using UnitTest.UnitCalculate.Calculator;

namespace UnitTest.UnitCalculate
{
    //工厂类角色
    class exprCalculateFactory
    {
        public static ExprCalculate calculator(CExpr r)
        {
            if (r is EmptyExpr){
                return new EmptyCalculator();
            }
            else if (r is ExprID){
                return new ExprIDCalculator();
            }
            else if (r is ExprFunCall)
            {
                return new FunctionCall();
            }
            else if (r is PolyExpr)
            {
                PolyExpr realR = (PolyExpr)r;
                switch (realR.Operator)
                {
                    //一元算术操作符
                    case OperatorType.Plus:
                    case OperatorType.Minus:
                    case OperatorType.Dec_pre:
                    case OperatorType.Inc_pre:
                    case OperatorType.Dec_post:
                    case OperatorType.Inc_post:
                        {
                            //表达式 量纲 等于子表达式返回量纲
                            CExpr child = realR[0];
                            return calculator(child);
                        }
                    //二元算术操作符                    
                    case OperatorType.Add:
                    case OperatorType.Sub:
                    case OperatorType.LT:
                    case OperatorType.GT:
                    case OperatorType.LE:
                    case OperatorType.GE:
                    case OperatorType.EQ:
                    case OperatorType.NE:
                    case OperatorType.Assign_add:
                    case OperatorType.Assign_sub:
                        {
                            return new EqualTwoCalculator();
                        }
                    case OperatorType.Mul:
                        {
                            return new MulCalculator();
                        }
                    case OperatorType.Div:
                        {
                            return new DivCalculator();
                        }
                    case OperatorType.Assign:
                        {
                            return new AssignCalculator();
                        }
                    case OperatorType.Mod:
                    case OperatorType.Assign_mul:
                    case OperatorType.Assign_div:
                    case OperatorType.Assign_mod:
                        {
                            //TODO
                            return new EmptyCalculator();
                        }
                    case OperatorType.Member_select_direct:
                        {
                            return new MemberSelect();
                        }
                    case OperatorType.Member_select_indirect:
                    case OperatorType.bit_and:
                    case OperatorType.bit_or:
                    case OperatorType.bit_xor:
                    case OperatorType.Shift_left:
                    case OperatorType.Shift_right:
                    case OperatorType.logical_and:
                    case OperatorType.logical_or:
                    case OperatorType.Assign_shl:
                    case OperatorType.Assign_shr:
                    case OperatorType.Assign_bit_xor:
                    case OperatorType.Assign_bit_and:
                    case OperatorType.Assign_bit_or:
                        {
                            //TODO
                            return new EmptyCalculator();
                        }
                    case OperatorType.Conditional:
                        {
                            //TODO
                            return new EmptyCalculator();
                        }
                    //暂未处理
                    case OperatorType.Deref:
                    case OperatorType.Ref:
                    case OperatorType.Bit_com:
                    case OperatorType.Logical_neg:
                        {
                            //TODO
                            return new EmptyCalculator();
                        }
                    default:
                        {
                            //TODO
                            return new EmptyCalculator();
                            throw new Exception("unknown expression: " + realR.Operator);

                        }
                }
            }
            else
            {
                //TODO
                return new EmptyCalculator();
                throw new Exception("unknown expression: " + r);
            }
        }
    }
}
