using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Diagnostics;
//using System.Windows;

using Solver.Util;
using UnitConfigure;

namespace Solver
{
    class Program
    {
        static void Main(string[] args)
        {
            //test_gause_union();
            //return;

            test_gause();
            return;

        }

        static void test_gause()
        {
            //量纲 y 时间单位 s        c 面积单位 m*m
            //     x 速度单位 m/s
            //     z 速度单位 km/s
            //运算（y*x)*(y*z) 与 c 不同
            dimension mytest = new dimension("dimension.xml");
            dimensionOperator Operator = new dimensionOperator();
            dimensionNode a = mytest.getDimension("m");
            dimensionNode b = mytest.getDimension("km");
            dimensionNode c = Operator.mul(a, a);

            GauseSolver solver = new GauseSolver();
            List<WTuple<string, double>> list = new List<WTuple<string, double>>();

            string info = "";

            list.Clear();
            // #z * #y  = km
            info = "y+z=km";
            list.Add(new WTuple<string, double>("y", 1));
            list.Add(new WTuple<string, double>("z", 1));
            solver.AddConstraint(list, new DNode(b), info);

            list.Clear();
            // #x * #y  = m
            info = "x+y=m";
            list.Add(new WTuple<string, double>("x", 1));
            list.Add(new WTuple<string, double>("y", 1));
            solver.AddConstraint(list, new DNode(a), info);

            list.Clear();
            // #z * #y *#x * #y  = m*m
            // 运算错误
            info = "x+z+2y=m*m";
            list.Add(new WTuple<string, double>("x", 1));
            list.Add(new WTuple<string, double>("y", 2));
            list.Add(new WTuple<string, double>("z", 1));
            solver.AddConstraint(list, new DNode(c), info);

            Console.WriteLine(solver.ToString());
            Console.WriteLine(Message.ErrorMsg);
            Console.WriteLine(Message.WarningMsg);
            Console.WriteLine(Message.Msg);
        }


        static void test_tuple()
        {
            int n = 5;
            WTuple<int, string>[] arr = new WTuple<int, string>[n];
            arr[0] = new WTuple<int, string>(2, "2");
            arr[1] = new WTuple<int, string>(3, "3");
            arr[2] = new WTuple<int, string>(1, "1");
            arr[3] = new WTuple<int, string>(1, "0");
            arr[4] = new WTuple<int, string>(1, "2");
            Array.Sort<WTuple<int, string>>(arr);
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine(arr[i]);
            }
        }
    }
}
