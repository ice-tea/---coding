using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FunSumC2XML;

namespace FunctionSummary
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
        }

        static void Test1()
        {
            string[] fileNames = { "d3b_varb.xml", "d3b_util.xml", "d3b_tmry.xml", "d3b_tcmd.xml", "d3b_intr.xml", "d3b_heat.xml", "d3b_ecu.xml", "d3b_batt.xml", "d3b_aocs.xml", "d3b_1553b.xml" };
            //string[] fileNames = { "foo1.xml" };
            Summarization s = new Summarization(fileNames, true);
            s.Summary();
            s.ToExcelFile("result.xlsx");
            s.ToWordFile("result.docx");
        }

        static void Test2()
        {
            string[] fileNames = { "dfh3b_1553.c", 
                                   "dfh3b_aocs.c",
                                   "dfh3b_batt.c",
                                   "dfh3b_dbug.c",
                                   "dfh3b_dicu.c",
                                   "dfh3b_fdir.c",
                                   "dfh3b_heat.c",
                                   "dfh3b_iems.c",
                                   "dfh3b_iepc.c",
                                   "dfh3b_intr.c",
                                   "dfh3b_mdct.c",
                                   "dfh3b_orbt.c",
                                   "dfh3b_para.c",
                                   "dfh3b_pgmc.c",
                                   "dfh3b_tcmd.c",
                                   "dfh3b_tmry.c",
                                   "dfh3b_util.c",
                                   "dfh3b_varb.c"
                                 };
            //string[] fileNames = { "foo.c" };
            Summarization s = new Summarization(fileNames, false);
            s.Summary();
            s.ToExcelFile("result.xlsx");
            //s.ToExcelFile("result.xlsx");
            //Console.WriteLine(s.ToString());
        }

        /*
         * 测试 Keil 51 C
         * */
        static void Test3()
        {
            string[] fileNames = { "d3b_1553b.c", 
                                   "d3b_aocs.c",
                                   "d3b_batt.c",
                                   "d3b_ecu.c",
                                   "d3b_heat.c",
                                   "d3b_intr.c",
                                   "d3b_tcmd.c",
                                   "d3b_tmry.c",
                                   "d3b_util.c",
                                   "d3b_varb.c"
                                 };
            //string[] fileNames = { "d3b_1553b.c" };
            Summarization s = new Summarization(fileNames, CStandard.Keil_C_51);
            s.Summary();
            s.ToWordFile("result.docx");
            //Console.WriteLine(s.ToString());
        }
    }
}
