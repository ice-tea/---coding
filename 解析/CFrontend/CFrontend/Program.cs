using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Diagnostics;
using System.Data;

using Microsoft.Office.Interop.Excel;
using CFrontendParser.CParser;
using CFrontendParser.CSyntax;
using CFrontendParser.Preprocess;

namespace CFrontend
{
    class Program
    {
        static void Main(string[] args)
        {
            TestCProgram();
            //TestExport();
            //Test1();
            //Test2();
        }

        static void TestCProgram()
        {
            string[] filenames = { "foo.c"
                                 };
            ParseCProgram parser = new ParseCProgram("BD2", filenames, CStandard.ANSI_C);
            CProgram p = parser.Parse();
        }

        static void Test1()
        {
            ParseCFile parseCode = new ParseCFile("foo.xml"); 
            CFile cfile = parseCode.Parse();
            Console.WriteLine(cfile.ToString());
        }

        static void Test2()
        {
            //ParseCFile parseCode = new ParseCFile("d3b_varb.xml");
            //ParseCFile parseCode = new ParseCFile("d3b_1553b.xml");
            //ParseCFile parseCode = new ParseCFile("d3b_aocs.xml");
            //ParseCFile parseCode = new ParseCFile("d3b_batt.xml");
            //ParseCFile parseCode = new ParseCFile("d3b_ecu.xml");
            //ParseCFile parseCode = new ParseCFile("d3b_heat.xml");
            //ParseCFile parseCode = new ParseCFile("d3b_intr.xml");
            //ParseCFile parseCode = new ParseCFile("d3b_tcmd.xml");
            //ParseCFile parseCode = new ParseCFile("d3b_tmry.xml");
            ParseCFile parseCode = new ParseCFile("d3b_util.xml");
            parseCode.Parse();
            Console.WriteLine(parseCode.ToString());
        }

        static void Test3()
        {
            string[] filenames = { "d3b_varb.xml" };
            foreach (string name in filenames)
            {
                ParseCFile parseCode = new ParseCFile(name);
                parseCode.Parse();
                Console.WriteLine(parseCode.ToString());
            }
        }

        public static void TestExport()
        {
            DataSet ds = new DataSet("dataSet");
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Age");
            dt.Columns.Add("Note");

            DataRow r1 = dt.NewRow();
            string[] array1 = { "Jim", "23", "NA" };
            r1.ItemArray = array1;
            dt.Rows.Add(r1);

            DataRow r2 = dt.NewRow();
            string[] array2 = { "Tom", "25", "NA" };
            r2.ItemArray = array2;
            dt.Rows.Add(r2);

            ds.Tables.Add(dt);

            DoExport(ds, null, @"test1.xlsx");

            string[] colName = { "Name", "Note"};
            DoExport(ds, new List<string>(colName), "test2.xlsx");
        }

        /// <summary>   
        /// 直接导出Excel   
        /// </summary>   
        /// <param name="ds">数据源DataSet</param>   
        /// <param name="columns">列名数组,允许为空(columns=null),为空则表使用默认数据库列名 </param>   
        /// <param name="fileName">保存文件名(例如：a.xls)</param>   
        /// <returns></returns>   
        public static bool DoExport(DataSet ds, List<string> columns, string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            if (ds.Tables.Count == 0 || fileName == string.Empty)
            {
                return false;
            }
            Application excel = new Application();
            int rowindex = 1;
            int colindex = 0;
            Workbook work = excel.Workbooks.Add(true);
            //Worksheet sheet1 = (Worksheet)work.Worksheets[0];   
            System.Data.DataTable table = ds.Tables[0];
            if (columns != null)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    colindex++;
                    if (columns[i] != null && columns[i] != "")
                    {
                        excel.Cells[1, colindex] = columns[i];
                    }
                    else
                    {
                        excel.Cells[1, colindex] = table.Columns[i].ColumnName;
                    }
                }
                rowindex = 1;
                foreach (DataRow row in table.Rows)
                {
                    rowindex++;
                    colindex = 0;
                    foreach (DataColumn col in table.Columns)
                    {
                        if (columns.IndexOf(col.ColumnName) >= 0)
                        {
                            colindex++;
                            excel.Cells[rowindex, colindex] = row[col.ColumnName].ToString();
                        }
                    }
                }
            }
            else
            {
                foreach (DataColumn col in table.Columns)
                {
                    colindex++;
                    excel.Cells[1, colindex] = col.ColumnName;
                }
                rowindex = 1;
                foreach (DataRow row in table.Rows)
                {
                    rowindex++;
                    colindex = 0;
                    foreach (DataColumn col in table.Columns)
                    {
                        colindex++;
                        excel.Cells[rowindex, colindex] = row[col.ColumnName].ToString();
                    }
                }
            }
            excel.Visible = false;
            excel.ActiveWorkbook.SaveAs(file.FullName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing); 
            excel.Quit();
            excel = null;
            GC.Collect();
            return true;
        }
    }
}
