using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//For Parse C files' unit annotation
using UParser;

// For Parse C files to CProgram
using CFrontendParser.CParser;
using CFrontendParser.CSyntax;
using CFrontendParser.Preprocess;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CSyntax.CFG;
using CFrontendParser.CSyntax.Expression;

//For Unit datatype demention
using UnitConfigure;

//For caculate
using UnitData;
using Solver.Util;

using UnitTest.UnitCalculate;

namespace UnitTest
{
    class UnitStaticTest
    {
        //For Test
        private CProgram _program;
        private string[] sourcefilenames;
        private UnitDataStruct unitData;

        public UnitStaticTest(string[] filenames)
        {
            Intilazition();
            ProduceProgram(filenames);
        }
        private void Intilazition()
        {
            //Unit Configure File
            unitData = new UnitDataStruct();
        }
        private void ProduceProgram(string[] filenames)
        {
            sourcefilenames = filenames;
            //得到注释中的量纲信息
            UnitParser Uparser = new UnitParser(filenames);
            Uparser.Parse();

            //更改目标文件
            for (int i = 0; i < sourcefilenames.Length; i++)
            {
                sourcefilenames[i] = sourcefilenames[i].Replace(".c", "Unit.c");
            }
            ParseCProgram parser = new ParseCProgram("BD2", sourcefilenames, CStandard.ANSI_C);
            _program = parser.Parse();

        }
        
        //-------------------------------------------------------------------------
        //测试入口
        public string Test()
        {
            string msg = "";
            msg = TestGlobalVar();
            TestFuntions(); //以函数为单位测试

            Console.WriteLine("测试完成");
            Console.WriteLine(unitData.ToString());
            Console.WriteLine(Message.ErrorMsg);
            Console.WriteLine(Message.WarningMsg);
            Console.WriteLine(Message.Msg);
            return msg;
        }
        //全局变量测试入口---测试量纲注释所变成的全局量声明
        //TODO:一行多个声明？ 复杂结构体注释处理
        private string TestGlobalVar()
        {
            string msg = "";
            CEntityCollection<CVarDefinition> global = _program.GlobalVars;
            foreach (CVarDefinition var in global.CEntityList)
            {
                //更新全局求解器
                if (IsUnitAnnotate(var))
                    msg = AddUnitConstraint(var, ref unitData);
                else
                {
                    if (var.Type.IsPrimitive)
                        unitData.addVar(var.Name);
                    else
                        unitData.addCom(var.Name, var.Type.Name);
                }
            }
            Console.WriteLine("全局变量测试完成");
            Console.WriteLine(unitData.ToString());
            Console.WriteLine(Message.ErrorMsg);
            Console.WriteLine(Message.WarningMsg);
            Console.WriteLine(Message.Msg);
            return msg;
        }
        //检测是否是量纲注释
        private bool IsUnitAnnotate(CVarDefinition var)
        {
            //char * 空格敏感
            if (var.Type.Name == "char *" && var.Name.IndexOf("MYUNIT") == 0)
                return true;
            else
                return false;
        }
        //量纲注释中  抽取的约束 加入Solver
        private string AddUnitConstraint(CVarDefinition var, ref UnitDataStruct uData)
        {
            string inital = var.InitialValue.ToString();
            unitData.register(inital);
            return "";
        }
        //-----------------------------------------------------------------------------------
        //函数量纲测试入口
        //各自享有共同的当前全局量纲信息
        private void TestFuntions()
        {
            UnitDataStruct globalUnitData = this.unitData;
            foreach (CFunction fun in this._program.Functions)
            {
                TestEachFuction(globalUnitData, fun);
            }
        }
        private void TestEachFuction(UnitDataStruct globalUnitData, CFunction fun)
        {
            //先加入local 变量声明中的量纲信息
            UnitDataStruct funUnitData = globalUnitData;
            //进入函数之前 增加scope量
            unitData.AddScope();

            //TestFuntionLocalVar(ref funUnitData, fun);
            //TODO: 全局复杂数据类型中隐含？
            string msg = "";
            //TestFuntionLocalVar();
            fun.Body.Travel(ProcessStmt,ref funUnitData, msg);
        }
        //全局变量测试入口---测试量纲注释所变成的全局量声明
        //TODO:一行多个声明？ 复杂结构体注释处理
        private string TestFuntionLocalVar(ref UnitDataStruct uData, CFunction fun)
        {
            string msg = "";
            List<CVarDefinition> vars = fun.LocalVars;
            foreach (CVarDefinition var in vars)
            {
                //更新求解器
                if (IsUnitAnnotate(var))
                    msg = AddUnitConstraint(var, ref uData);
                else
                {
                    if (var.Type.IsPrimitive)
                        unitData.addVar(var.Name);
                    else
                        unitData.addCom(var.Name, var.Type.Name);
                }
            }
            Console.Write(fun.Name);
            Console.WriteLine("函数局部变量注释完成");
            Console.WriteLine(uData.ToString());
            Console.WriteLine(Message.ErrorMsg);
            Console.WriteLine(Message.WarningMsg);
            Console.WriteLine(Message.Msg);
            return msg;
        }
        //------------------------------------------------------------------------------------
        /// <summary>
        /// 代理：处理语句
        /// </summary>
        public static Process ProcessStmt = delegate(CEntity entity, ref UnitDataStruct parameter, object result)
        {
            //entity may be 
            //1.ReturnStmt  2.GoStmt  3.SimpleStmt(break;continue;)
            //Add 4.CExpr
            if (entity is CStmt && parameter is UnitDataStruct)
            {
                //只有ReturnStmt 需要处理
                if (entity is ReturnStmt)
                {
                    //处理表达式
                    CExpr expr = ((ReturnStmt)entity).ReturnValue;
                    UnitDataStruct data = (UnitDataStruct)parameter;
                    CalculculateEntry.Calculate(expr, ref data);
                }
            }
            else if (entity is CEntity && parameter is UnitDataStruct)
            {
                //处理表达式
                CExpr expr = (CExpr)entity;
                UnitDataStruct data = (UnitDataStruct)parameter;
                CalculculateEntry.Calculate(expr, ref data);
            }
        };
    }
}
