using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;

namespace FunSumC2XML
{
    class Export2Office
    {
        /// <summary>   
        /// 直接导出Word
        /// </summary>   
        /// <param name="ds">数据源DataSet</param>
        /// <param name="fileName">保存文件名(例如：a.xls)</param>   
        /// <returns></returns>   
        public void Export2Word(System.Data.DataTable dataTable, string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            if (file.Exists)
                file.Delete();
            if (dataTable == null || fileName == string.Empty)
            {
                return;
            }
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Document wordDoc = wordApp.Documents.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            string context = "";
            string catalog = "";

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (col.ColumnName == "分类号")
                    {
                        string cataValue = row[col.ColumnName].ToString();
                        if (cataValue != catalog)
                        {
                            catalog = cataValue;
                            context += "模块 " + cataValue + "\r\r";
                        }
                        //wordDoc.Paragraphs.Last.Range.Font.Name = "黑体";
                        //wordDoc.Paragraphs.Last.Range.Font.Bold = 2;
                        //wordDoc.Paragraphs.Last.Range.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorBlue;
                    }
                    else
                    {
                        context += col.ColumnName + ": ";
                        context += row[col.ColumnName].ToString() + "\r";
                        context += "\r";
                        //if (col.ColumnName == "模块名称")
                        //{
                        //    wordDoc.Paragraphs.Last.Range.Font.Bold = 2;
                        //}
                        //else
                        //{
                        //    wordDoc.Paragraphs.Last.Range.Font.Bold = 1;
                        //}
                        //wordDoc.Paragraphs.Last.Range.Font.Name = "宋体";
                        //wordDoc.Paragraphs.Last.Range.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorBlack;
                    }
                }
                context += "\r";
            }
            wordDoc.Content.Text = context;
            object wordName = file.FullName;
            wordDoc.SaveAs(ref wordName,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing);
            wordDoc.Close();
            wordApp.Quit();
            wordDoc = null;
            wordApp = null;
            GC.Collect();
            return;
        }

        /// <summary>   
        /// 直接导出Excel   
        /// </summary>   
        /// <param name="ds">数据源DataSet</param>
        /// <param name="fileName">保存文件名(例如：a.xls)</param>   
        /// <returns></returns>   
        public void Export2Excel(System.Data.DataTable dataTable, string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            if (dataTable == null || fileName == string.Empty)
            {
                return;
            }
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            int rowindex = 1;
            int colindex = 0;
            Workbook work = excel.Workbooks.Add(true);

            foreach (DataColumn col in dataTable.Columns)
            {
                colindex++;
                excel.Cells[1, colindex] = col.ColumnName;
            }
            rowindex = 1;
            foreach (DataRow row in dataTable.Rows)
            {
                rowindex++;
                colindex = 0;
                foreach (DataColumn col in dataTable.Columns)
                {
                    colindex++;
                    excel.Cells[rowindex, colindex] = row[col.ColumnName].ToString();
                }
            }
            excel.Visible = false;
            excel.ActiveWorkbook.SaveAs(file.FullName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            excel.Quit();
            excel = null;
            GC.Collect();
            return;
        }
    }
}
