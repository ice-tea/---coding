using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnitConfigure;

namespace Solver.Util
{
    public class DNode
    {
        public dimensionNode node { set; get; }

        public DNode(dimensionNode n)
        {
            node = n;
        }
        public DNode(DNode n)
        {
            node = n.node;
        }
        //四则运算
        public static DNode operator +(DNode x, DNode y)
        {
            //高斯消元中的加法  对应量纲节点相乘
            dimensionOperator Operator = new dimensionOperator();
            return new DNode( Operator.mul(x.node, y.node) );
        }

        public static DNode operator -(DNode x, DNode y)
        {
            //高斯消元中的减法  对应量纲节点相除
            dimensionOperator Operator = new dimensionOperator();
            return new DNode(Operator.div(x.node, y.node));
        }

        //高斯消元中 项的倍数
        public static DNode operator *(DNode n, double d)
        {
            //高斯消元中的倍乘  对应量纲节点指数运算
            dimensionOperator Operator = new dimensionOperator();
            return new DNode(Operator.pow(n.node, d));
        }

        public static DNode operator *(double d, DNode n)
        {
            //高斯消元中的倍乘  对应量纲节点指数运算
            dimensionOperator Operator = new dimensionOperator();
            return new DNode(Operator.pow(n.node, d));
        }

        public static DNode operator /(DNode n, double d)
        {
            //高斯消元中的除法  对应量纲节点指数运算
            dimensionOperator Operator = new dimensionOperator();
            return new DNode(Operator.pow(n.node, 1/d));
        }

        public static DNode operator /(double d, DNode n)
        {
            //高斯消元中的除法  对应量纲节点指数运算
            dimensionOperator Operator = new dimensionOperator();
            return new DNode(Operator.pow(n.node, 1 / d));
        }
        public bool IsZero() 
        {
            //调用量纲节点的 常量纲判定函数
            dimensionOperator Operator = new dimensionOperator();
            return Operator.isConstant(node);
        }


        public override string ToString()
        {
            //调用量纲节点 的Str转换
            dimensionOperator Operator = new dimensionOperator();
            return "<" + Operator.DimensionStr(node) + ">";
        }
    }
}
