using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.IO;
using System.Diagnostics;

namespace CFrontendParser.Preprocess
{
    public enum CStandard
    {
        ANSI_C,
        Keil_C_51
    }

    class C2XML
    {
        private string cFileName;
        private string cInFileName;
        private string xmlFileName;

        public XmlDocument C2XmlDocument(string fileName, bool preprocessed, CStandard standard)
        {
            this.cFileName = fileName;
            if (fileName.Contains("\\"))
            {
                char[] delimiterChars = { '\\' };
                string[] tmp = fileName.Split(delimiterChars);
                fileName = tmp[tmp.Length - 1];
            }
            this.cInFileName = fileName.Replace(".c", ".i.c");
            this.xmlFileName = fileName.Replace(".c", ".xml");

            this.RunXML(preprocessed, standard);

            XmlDocument doc = new XmlDocument();
            doc.Load(this.xmlFileName);
            return doc;
        }

        /*
         * 首先，利用VC的编译器做预处理
         * 然后，调用C2XML生成XML文件
         * */
        private void RunXML(bool preprocessed, CStandard standard)
        {
            if (preprocessed)
            {
                // 必要的处理
                this.CFileProcess(this.cFileName, standard);

                string cmd = @"C2XML.exe -n " + cInFileName + " -o " + xmlFileName;
                RunCmd(cmd);
            }
            else
            {
                string fileName = "tmp.bat";
                FileInfo file = new FileInfo("setting.bat");
                FileInfo file2 = new FileInfo(fileName);
                if (file2.Exists)
                    file2.Delete();
                File.Copy(file.Name, file2.Name);
                FileStream fs = new FileStream(file2.Name, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                string cmd = "cl.exe /EP " + cFileName + " > " + cInFileName;
                sw.WriteLine(cmd);
                sw.Close();
                RunBat(fileName);

                // 必要的处理
                this.CFileProcess(this.cInFileName, standard);

                cmd = @"C2XML.exe -n " + cInFileName + " -o " + xmlFileName;
                RunCmd(cmd);
            }
        }

        /*
         * 移除预处理之后，一些不被C2XML识别的符号
         * */
        private void CFileProcess(string fileName, CStandard standard)
        {
            FileStream inStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inStream);
            string line;
            StringBuilder result = new StringBuilder();

            while ((line = reader.ReadLine()) != null)
            {
                string tmp = line.Trim();
                // 删除空行
                if (tmp.Length == 0)
                    continue;
                // 删除一些编译器相关的约定
                if (tmp.StartsWith("#pragma"))
                    continue;
                line = line.Replace("__cdecl", "");
                int pos = line.IndexOf("__attribute__");
                if (pos > 0)
                {
                    line = line.Remove(pos - 1) + ";";
                }
                if (standard == CStandard.Keil_C_51)
                {
                    // 移除Keil C 51的特征
                    // 首先，移除 存储类型修饰符
                    string[] specialModifiers = { "data", "bdata", "idata", "xdata", "pdata", "code" };
                    foreach (string modifer in specialModifiers)
                    {
                        line = line.Replace(" " + modifer + " ", " ");
                    }
                    // 然后，处理sfr, sbit
                    if (line.StartsWith("sfr "))
                    {
                        line = line.Replace("sfr ", "char * ");
                    }
                    else if (line.StartsWith("sbit "))
                    {
                        line = line.Replace("sbit ", "char * ");
                        line = line.Replace("^", "+");
                    }
                }
                result.AppendLine(line);
            }
            reader.Close();
            inStream.Close();

            StreamWriter fileWrite = new StreamWriter(this.cInFileName, false);
            fileWrite.WriteLine(result);
            fileWrite.Close();
        }

        private void RunCmd(string cmd)
        {
            string batFileName = "tmp.bat";
            FileInfo file = new FileInfo(batFileName);
            if (file.Exists)
                file.Delete();
            StreamWriter batWrite = new StreamWriter(file.FullName, false, Encoding.Default);
            batWrite.WriteLine(cmd);
            batWrite.Close();

            this.RunBat(batFileName);
        }

        private void RunBat(string batFileName)
        {
            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(batFileName);

            /* 
             * 若要使用 StandardOutput，
             * 您必须将 ProcessStartInfo.UseShellExecute 设置为 false，
             * 并且将 ProcessStartInfo ..::.RedirectStandardOutput 设置为 true。 
             * 否则，读取 StandardOutput 流时将引发异常。*/
            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;

            Process process = new Process();
            process.StartInfo = myProcessStartInfo;
            process.Start();
            while (!process.HasExited)
            {
                process.WaitForExit();
            }
        }
    }
}
