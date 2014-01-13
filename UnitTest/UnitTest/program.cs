using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestGlobalVar();
            //TestAdd();
            //TestMutiple();
            //TestChange();
            //TestScope();
            //TestBranch();
            //TestConstNum();
            //TestStruct();
            TestFunction();
        }

        //测试全局变量声明
        static void TestGlobalVar()
        {
            string[] filenames = { "test1.c"
                                 };
            UnitStaticTest mytest = new UnitStaticTest(filenames);

            mytest.Test();
        }
        //测试函数计算
        static void TestAdd()
        {
            string[] filenames = { "test2.c"
                                 };
            UnitStaticTest mytest = new UnitStaticTest(filenames);

            mytest.Test();
        }
        //测试乘法
        static void TestMutiple()
        {
            string[] filenames = { "test3.c"
                                 };
            UnitStaticTest mytest = new UnitStaticTest(filenames);

            mytest.Test();
        }
        //测试可变
        static void TestChange()
        {
            string[] filenames = { "testChange.c"
                                 };
            UnitStaticTest mytest = new UnitStaticTest(filenames);

            mytest.Test();
        }
        //测试可变
        static void TestScope()
        {
            string[] filenames = { "testScope.c"
                                 };
            UnitStaticTest mytest = new UnitStaticTest(filenames);

            mytest.Test();
        }
        //测试分支
        static void TestBranch()
        {
            string[] filenames = { "testBranch.c"
                                 };
            UnitStaticTest mytest = new UnitStaticTest(filenames);

            mytest.Test();
        }
        //测试常量
        static void TestConstNum()
        {
            string[] filenames = { "testConstNum.c"
                                 };
            UnitStaticTest mytest = new UnitStaticTest(filenames);

            mytest.Test();
        }
        //测试结构体
        static void TestStruct()
        {
            string[] filenames = { "testStruct.c"
                                 };
            UnitStaticTest mytest = new UnitStaticTest(filenames);

            mytest.Test();
        }
        //测试函数调用
        static void TestFunction()
        {
            string[] filenames = { "testFunction.c"
                                 };
            UnitStaticTest mytest = new UnitStaticTest(filenames);

            mytest.Test();
        }
    }  
}
