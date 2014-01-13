using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitConfigure
{
    public class dimensionOperator
    {
        public dimensionNode add(dimensionNode a,dimensionNode b)//量纲节点相加
        {
            if (dimensionEqual(a, b)) 
            {
                dimensionNode result = new dimensionNode();
                result.dimension = a.dimension;
                result.coefficient = a.coefficient + b.coefficient;
                //result.offset = a.offset + b.offset;
                return result;
            }
            return null;
        }
        public dimensionNode sub(dimensionNode a, dimensionNode b)//量纲节点相减
        {
            if (dimensionEqual(a, b))
            {
                dimensionNode result = new dimensionNode();
                result.dimension = a.dimension;
                result.coefficient = a.coefficient - b.coefficient;
                //result.offset = a.offset + b.offset;
                return result;
            }
            return null;
        }
        public dimensionNode mul(dimensionNode a, dimensionNode b)//量纲节点相乘
        {
            if (a == null || b == null) return null;
            dimensionNode result = new dimensionNode();
            for (int i = 0; i <a.dimension.Count; i++)
            {
                result.addDimension((float)a.dimension[i]+(float)b.dimension[i]);
            }
            result.coefficient = a.coefficient * b.coefficient;
            return result;
        }
        public dimensionNode mul(dimensionNode a, double b)//量纲节点乘常量
        {
            if (a == null) return null;
            a.coefficient = a.coefficient * (float)b;
            return a;
        }
        public dimensionNode div(dimensionNode a, dimensionNode b)//量纲节点相除
        {
            if (a == null || b == null) return null;
            dimensionNode result = new dimensionNode();
            for (int i = 0; i < a.dimension.Count; i++)
            {
                result.addDimension((float)a.dimension[i] - (float)b.dimension[i]);
            }
            result.coefficient = a.coefficient / b.coefficient;
            return result;
        }
        public dimensionNode div(dimensionNode a, double b)//量纲节点除常量
        {
            if (a == null) return null;
            a.coefficient = a.coefficient / (float)b;
            return a;
        }
        public dimensionNode pow(dimensionNode a, double pow)//量纲节点乘方
        {
            if (a == null) return null;
            dimensionNode result = new dimensionNode();
            for (int i = 0; i < a.dimension.Count; i++)
            {
                result.addDimension((float)a.dimension[i]* (float)pow);
            }
            result.coefficient =(float)Math.Pow(a.coefficient,pow);
            return result;
        }

        public bool dimensionEqual(dimensionNode a, dimensionNode b)//量纲节点维度是否相等
        {
            for (int i = 0; i < a.dimension.Count; i++)
            {
                if ((float)a.dimension[i] != (float)b.dimension[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool equal(dimensionNode a, dimensionNode b)//量纲节点是否完全相等
        {
            if ((a.offset != b.offset) || (a.coefficient != b.coefficient)) 
            {
                return false;
            }
            return dimensionEqual(a, b);
        }
        //ADD LIBO增加是否是常量判断
        public bool isConstant(dimensionNode a)
        {
            //系数==1  偏移==0
            if ( !Utility.IsZero(a.offset) || !Utility.IsZero(a.coefficient - 1))
            {
                return false;
            }
            for (int i = 0; i < a.dimension.Count; i++)
            {
                if ( !Utility.IsZero( (float) (a.dimension[i]) ) )
                {
                    return false;
                }
            }
            return true;
        }
        //字符串显示
        public string DimensionStr(dimensionNode tmp)
        {
            string s = "";
            if (tmp != null)
            {
                s += "d:";
                foreach (var t in tmp.dimension)
                {
                    s += t.ToString() + ",";
                }
                s += "c:" + tmp.coefficient;
                s += "o:" + tmp.offset;
            }
            else
            {
                s = ("量纲为空！");
            }
            return s;
        }
    }
    //判断是否为0
    public static class Utility
    {
        const double EPS = 1e-7;
        public static bool IsZero(double d)
        {
            return Math.Abs(d) < EPS;
        }
    }
}
