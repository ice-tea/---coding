using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace UnitConfigure
{
    class Program
    {
        static void Main(string[] args)
        {
            dimension mytest = new dimension("dimension.xml");
            dimensionNode diNode;
            string[] t = {"m","km","cm","mm","kg","g","s","ms","A","mA","K","c","F","cd","N","dsp"};
            //打印量纲信息
            for (int i = 0; i < t.Length; i++)
            {
                Console.WriteLine(t[i]);
                diNode = mytest.getDimension(t[i]);

                if (diNode != null)
                {
                    printDimension(diNode);
                }
                Console.WriteLine();
            }

            //量纲运算
            Console.WriteLine("量纲四则运算");
            dimensionOperator Operator = new dimensionOperator();
            dimensionNode a=mytest.getDimension("m");
            dimensionNode b=mytest.getDimension("km");
            dimensionNode c = mytest.getDimension("kg");
            dimensionNode d=mytest.getDimension("g");
            Console.WriteLine("**************************************************");
            Console.WriteLine("量纲相加：m+km;m+kg");
            printDimension(Operator.add(a,b));
            printDimension(Operator.add(a, c));//不同维度量纲无法相加
            Console.WriteLine("**************************************************");
            Console.WriteLine("量纲相减：m-km;m-kg");
            printDimension(Operator.sub(a, b));
            printDimension(Operator.sub(a, c));//不同维度量纲无法相减
            Console.WriteLine("**************************************************");
            Console.WriteLine("量纲相乘：m*km;m*kg");
            printDimension(Operator.mul(a, b));
            printDimension(Operator.mul(a, c));
            Console.WriteLine("**************************************************");
            Console.WriteLine("量纲相除：m/km;m/kg");
            printDimension(Operator.div(a, b));
            printDimension(Operator.div(a, c));
            Console.WriteLine("**************************************************");
            Console.WriteLine("量纲乘方：km^3;kg^3");
            printDimension(Operator.pow(b, 3));
            printDimension(Operator.pow(c,3));
            Console.WriteLine("**************************************************");
            Console.WriteLine("量纲相等判断：m*kg=km*g");
            dimensionNode tmp = Operator.mul(a, c);//m*kg
            dimensionNode tmp2 = Operator.mul(b,d);//km*g
            printDimension(tmp);
            printDimension(tmp2);
            Console.WriteLine(Operator.equal(tmp,tmp2));//m*kg=km*g

            Console.WriteLine("**************************************************");
            printDimension(mytest.getDimension("N"));
            printDimension(mytest.getDimension("J"));
            printDimension(mytest.getDimension("G"));


            Console.ReadLine();
        }

        public static void printDimension(dimensionNode tmp)
        {
            if (tmp != null)
            {
                Console.Write("d:");
                foreach (var t in tmp.dimension)
                {
                    Console.Write(t.ToString() + ',');
                }
                Console.WriteLine();
                Console.WriteLine("c:" + tmp.coefficient);
                Console.WriteLine("o:" + tmp.offset);
            }
            else
            {
                Console.WriteLine("量纲为空！");
            }
            Console.WriteLine();
        }
    }
}
