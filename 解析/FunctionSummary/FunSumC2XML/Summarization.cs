using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;

namespace FunSumC2XML
{
    public partial class Summarization
    {
        private XmlDocument[] xmlDocs;
        private string[] CFileNames;
        private bool isXML;
        private Dictionary<string, CFunctionReport> reports;
        private HashSet<string> globalVars;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileNames"> 若干文件名 </param>
        /// <param name="isXml"> 
        /// 说明文件的类型是XML文件，还是C文件.
        /// 如果是XML文件，那么应当以.xml结束.
        /// 如果是C文件，那么应当以.c结束.
        /// </param>
        public Summarization(string[] fileNames, bool isXml)
        {
            this.reports = new Dictionary<string, CFunctionReport>();
            this.globalVars = new HashSet<string>();
            this.xmlDocs = new XmlDocument[fileNames.Length];
            this.CFileNames = new string[fileNames.Length];
            this.isXML = isXml;

            int i = 0;
            if (isXml)
            {           
                foreach (string fileName in fileNames)
                {
                    this.xmlDocs[i] = new XmlDocument();
                    this.xmlDocs[i].Load(fileName);
                    this.CFileNames[i] = fileName.Replace(".xml", ".c");
                    i++;
                }
            }
            else
            {
                C2XML convert = new C2XML();
                foreach (string fileName in fileNames)
                {
                    this.xmlDocs[i] = convert.C2XmlDocument(fileName, CStandard.ANSI_C);
                    this.CFileNames[i] = fileName;
                    i++;
                }
            }
        }

        public Summarization(string[] fileNames, CStandard standard)
        {
            this.reports = new Dictionary<string, CFunctionReport>();
            this.globalVars = new HashSet<string>();
            this.xmlDocs = new XmlDocument[fileNames.Length];
            this.CFileNames = new string[fileNames.Length];
            this.isXML = false;

            int i = 0;
            C2XML convert = new C2XML();
            foreach (string fileName in fileNames)
            {
                this.xmlDocs[i] = convert.C2XmlDocument(fileName, standard);
                this.CFileNames[i] = fileName;
                i++;
            }
        }

        /*
         * 删除生成的临时文件
         * 这些临时文件包括两类
         * 一是预处理之后的C程序文件
         * 二是C2XML生成的XML文件
         */
        public void PostProcess()
        {
            if (this.isXML)
                return;
            foreach (string str in this.CFileNames)
            {
                string fileName = this.GetFileName(str);
                FileInfo cFile = new FileInfo(fileName.Replace(".c", ".i.c"));
                if (cFile.Exists)
                    cFile.Delete();
                FileInfo xmlFile = new FileInfo(fileName.Replace(".c", ".xml"));
                if (xmlFile.Exists)
                    xmlFile.Delete();
            }
        }

        private string GetFileName(string str)
        {
            if (str.Contains("\\"))
            {
                char[] delimiterChars = { '\\' };
                string[] tmp = str.Split(delimiterChars);
                return tmp[tmp.Length - 1];
            }
            return str;
        }

        private bool IsGlobalVar(XmlNode node)
        {
            if (node.Name != "declaration")
                return false;
            XmlNode n1 = node.SelectSingleNode("init_declarator/declarator//direct_declarator_id");
            XmlNode nFun = node.SelectSingleNode("init_declarator/declarator//direct_declarator_function");
            if (n1 != null && nFun == null)
            {
                // 如果有 storage_class_specifier 
                // 则 token 取值是 extern
                XmlNode sNode = node.SelectSingleNode("storage_class_specifier");
                if (sNode != null)
                {
                    if (sNode.Attributes["token"].Value != "extern")
                        return false;
                    else
                    {
                        XmlNode n3 = node.ChildNodes[2];
                        if (n3.Name == "init_declarator")
                            return true;
                        else return false;
                    }
                }
                else
                {
                    XmlNode n2 = node.ChildNodes[1];
                    if (n2.Name == "init_declarator")
                        return true;
                    else return false;
                }
            }
            else return false;
        }

        /*
         * 函数定义
         * 既有函数签名，也有函数体
         * */
        private bool IsFunctionDefinition(XmlNode node)
        {
            if (node.Name != "function_definition")
                return false;
            XmlNode n1 = node.SelectSingleNode("declarator/direct_declarator_function");
            XmlNode n2 = node.SelectSingleNode("compound_statement");
            if (n1 != null && n2 != null)
                return true;
            else return false;
        }

        /*
         * 函数声明
         * 有函数签名，无函数体
         * */
        private bool IsFunctionDeclaration(XmlNode node)
        {
            if (node.Name != "declaration")
                return false;
            XmlNode n1 = node.SelectSingleNode("init_declarator/declarator/direct_declarator_function");
            XmlNode n2 = node.SelectSingleNode("compound_statement");
            if (n1 != null && n2 == null)
                return true;
            else return false;
        }

        public void Summary()
        {
            int i = 0;
            foreach (XmlDocument xmlDoc in this.xmlDocs)
            {
                XmlNode translate_unit = xmlDoc.SelectSingleNode("/translation_unit");
                foreach (XmlNode node in translate_unit)
                {
                    if (this.IsFunctionDefinition(node))
                    {
                        CFunctionReport report = this.GetCFunctionReport(node);
                        report.CFileName = this.CFileNames[i];
                        this.reports.Add(report.Name, report);
                    }
                    else if (this.IsGlobalVar(node))
                    {
                        string varName = this.GetGlobalVarName(node);
                        this.globalVars.Add(varName);
                    }
                }
                i++;
            }

            // 构建函数调用关系
            CallGraph graph = new CallGraph();
            foreach (CFunctionReport report in this.reports.Values)
            {
                foreach (string called in report.Calleds)
                {
                    graph.AddRelation(report.Name, called);
                }
            }
            // 计算函数调用层次
            graph.ComputeFunctionLevel();
            foreach (CFunctionReport report in this.reports.Values)
            {
                report.Callees = graph.GetCallees(report.Name);
                report.Level = graph.GetFunctionLevel(report.Name);
            }
        }

        /*
         * 获取全局变量的名称
         * */
        private string GetGlobalVarName(XmlNode node)
        {
            return this.GetNodeAttributeValue(node, "init_declarator/declarator//direct_declarator_id", "token");
        }

        /*
         * 获取 XML 节点的属性值
         */
        private string GetNodeAttributeValue(XmlNode node, string xmlPath, string attrName)
        {
            XmlNode n1 = node.SelectSingleNode(xmlPath);
            if (n1 != null)
            {
                XmlAttribute attr = n1.Attributes[attrName];
                if (attr != null)
                {
                    return attr.Value;
                }
            }
            return "";
        }
        
        //public override string ToString()
        //{
        //    string res = "";
        //    res += "| 函数名称| 返回值类型 | 返回值表达式 | 输入参数名称 | 输出参数名称 | 层次 | 上层函数 | 下层函数 | \n";
        //    foreach (CFunctionReport report in this.reports.Values)
        //    {
        //        res += "| " + report.Name + " | ";
        //        res += report.ReturnType + " | ";

        //        string retExpr = "";
        //        foreach (string e in report.ReturnExpression)
        //        {
        //            retExpr = retExpr + e + " ";
        //        }
        //        res += retExpr + " | ";

        //        string inputParas = "";
        //        string outputParas = "";
        //        for (int idx = 0; idx < report.ParametersName.Count; idx++)
        //        {
        //            if (report.ParametersType.ElementAt(idx).EndsWith("*"))
        //                outputParas = outputParas + report.ParametersName.ElementAt(idx) + " ";
        //            else inputParas = inputParas + report.ParametersName.ElementAt(idx) + " ";
        //        }
        //        res += inputParas + " | ";
        //        res += outputParas + " |";

        //        res += "\n";
        //    }
        //    return res;
        //}
        
        /*
         * 生成一个Word文档
         * */
        public void ToWordFile(string fileName)
        {
            Export2Office export = new Export2Office();
            System.Data.DataTable table = this.ExportDataTable();
            export.Export2Word(table, fileName);
        }



        /*
         * 生成一个Excel文档
         * */
        public void ToExcelFile(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            if (file.Exists)
                file.Delete();
            object wordName = file.FullName;
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook work = excelApp.Workbooks.Add(true);

            // 创建表格
            excelApp.Cells[1, 1] = "C文件名";
            excelApp.Cells[1, 2] = "函数名称";
            excelApp.Cells[1, 3] = "返回值类型";
            excelApp.Cells[1, 4] = "返回值表达式";
            excelApp.Cells[1, 5] = "输入参数";
            excelApp.Cells[1, 6] = "输出参数";
            excelApp.Cells[1, 7] = "输入全局变量";
            excelApp.Cells[1, 8] = "输出全局变量";
            excelApp.Cells[1, 9] = "上层函数";
            excelApp.Cells[1, 10] = "下层函数";
            excelApp.Cells[1, 11] = "层次";
            int tableIndex = 2;
            try
            {
                foreach (CFunctionReport report in this.reports.Values)
                {
                    // C文件名
                    excelApp.Cells[tableIndex, 1] = this.GetFileName(report.CFileName);

                    // 函数名
                    excelApp.Cells[tableIndex, 2] = report.Name;

                    // 返回值类型
                    excelApp.Cells[tableIndex, 3] = report.ReturnType;

                    // 返回值表达式
                    string retExpr = this.GetString(report.ReturnExpression);
                    excelApp.Cells[tableIndex, 4] = retExpr;

                    // 输入、输出参数
                    string inputParas = "";
                    string outputParas = "";
                    for (int idx = 0; idx < report.ParametersName.Count; idx++)
                    {
                        if (report.ParametersType.ElementAt(idx).EndsWith("*"))
                            outputParas = outputParas + report.ParametersName.ElementAt(idx) + ",";
                        else inputParas = inputParas + report.ParametersName.ElementAt(idx) + ",";
                    }
                    if (inputParas.EndsWith(","))
                        inputParas = inputParas.Substring(0, inputParas.Length - 1);
                    excelApp.Cells[tableIndex, 5] = inputParas;
                    if (outputParas.EndsWith(","))
                        outputParas = outputParas.Substring(0, outputParas.Length - 1);
                    excelApp.Cells[tableIndex, 6] = outputParas;

                    // 输入全局变量
                    string inputVars = this.GetString(report.Inputs);
                    excelApp.Cells[tableIndex, 7] = inputVars;

                    // 输出全局变量
                    string outputVars = this.GetString(report.Outputs);
                    excelApp.Cells[tableIndex, 8] = outputVars;

                    // 上层函数
                    string callees = this.GetString(report.Callees);
                    excelApp.Cells[tableIndex, 9] = callees;

                    // 下层函数
                    string calleds = this.GetString(report.Calleds);
                    excelApp.Cells[tableIndex, 10] = calleds;

                    // 层次
                    excelApp.Cells[tableIndex, 11] = report.Level.ToString();

                    tableIndex++;
                }
                excelApp.Visible = false;
                excelApp.ActiveWorkbook.SaveAs(file.FullName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);                
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            finally
            {
                excelApp.Quit();
                excelApp = null;
                GC.Collect();
            }
        }

        public System.Data.DataTable ExportDataTable()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("分类号");
            dt.Columns.Add("模块名称");
            dt.Columns.Add("函数原型");
            dt.Columns.Add("功能描述");
            dt.Columns.Add("输入项");
            dt.Columns.Add("输出项");
            dt.Columns.Add("接口");

            foreach (CFunctionReport report in this.reports.Values)
            {
                // 输入、输出参数
                string inputParas = "";
                string outputParas = "";
                for (int idx = 0; idx < report.ParametersName.Count; idx++)
                {
                    // 类型是指针型, 且被修改的变量是输出变量
                    // 这个规则不够严谨, 将来需要补充
                    if (report.ParametersType[idx].EndsWith("*") && report.ModifiedParameters.Contains(report.ParametersName[idx]) == true)
                    {
                        string paraName = report.ParametersName[idx];
                        string paraType = report.ParametersType[idx];
                        string paraStr = paraType + " " + paraName;
                        if (paraType.Contains("["))
                        {
                            int pos = paraType.IndexOf('[');
                            paraStr = paraType.Substring(0, pos) + " " + paraName + paraType.Substring(pos);
                        }
                        outputParas = outputParas + paraStr + ",";
                    }
                    else
                    {
                        string paraName = report.ParametersName[idx];
                        string paraType = report.ParametersType[idx];
                        string paraStr = paraType + " " + paraName;
                        if (paraType.Contains("["))
                        {
                            int pos = paraType.IndexOf('[');
                            paraStr = paraType.Substring(0, pos) + " " + paraName + paraType.Substring(pos);
                        }
                        inputParas = inputParas + report.ParametersName[idx] + ",";
                    }
                }
                //删掉多余的逗号
                if (inputParas.EndsWith(","))
                    inputParas = inputParas.Substring(0, inputParas.Length - 1);
                if (outputParas.EndsWith(","))
                    outputParas = outputParas.Substring(0, outputParas.Length - 1);

                // 输入全局变量
                string inputVars = this.GetString(report.Inputs);

                // 输出全局变量
                string outputVars = this.GetString(report.Outputs);

                // 返回值
                string returnStr = this.GetString(report.ReturnExpression);

                // 上层函数
                string callees = this.GetString(report.Callees);

                // 下层函数
                string calleds = this.GetString(report.Calleds);

                System.Data.DataRow r1 = dt.NewRow();
                string[] arrayStr = new string[7];
                arrayStr[0] = report.CFileName;
                arrayStr[1] = report.Name;
                arrayStr[2] = report.Prototype;
                arrayStr[3] = "...";
                arrayStr[4] = "\r" + "输入虚参: " + inputParas + "\r" + "输入全局变量: " + inputVars;
                arrayStr[5] = "\r" + "输出虚参: " + outputParas + "\r" + "输出全局变量: " + outputVars + "\r" + "返回值: " + returnStr;
                arrayStr[6] = "\r" + "上层函数: " + callees + "\r" + "下层函数: " + calleds;
                r1.ItemArray = arrayStr;
                dt.Rows.Add(r1);
            }

            return dt;
        }

        private string GetString(IEnumerable<string> list)
        {
            string result = "";
            foreach (string str in list)
            {
                result += str + ",";
            }
            if (result.EndsWith(","))
                result = result.Substring(0, result.Length - 1);
            return result;
        }
    }
}
