using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.util;

namespace Summarization
{
    public class CFunctionReport
    {
        /// <summary>
        /// 函数名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 函数所在的C文件名
        /// </summary>
        public string CFileName { set; get; }

        /// <summary>
        /// 函数返回值类型
        /// </summary>
        public string ReturnType { set; get; }

        /// <summary>
        /// 函数返回值表达式
        /// </summary>
        public HashSet<string> ReturnExpression { set; get; }

        /// <summary>
        /// 函数输入参数, 第一元：类型, 第二元：名称
        /// </summary>
        public List<CPair<string, string>> InputParameters { get; set; }

        /// <summary>
        /// 函数输出参数, 第一元：类型, 第二元：名称
        /// </summary>
        public List<CPair<string, string>> OutputParameters { get; set; }

        /// <summary>
        /// 函数的局部变量定义, 第一元：类型, 第二元：名称
        /// </summary>
        public HashSet<CPair<string, string>> LocalVars { get; set; }

        /// <summary>
        /// 输入输出变量集合, 第一元：类型, 第二元：名称
        /// </summary>
        public HashSet<CPair<string, string>> Inputs { get; set; }

        /// <summary>
        /// 输入输出变量集合, 第一元：类型, 第二元：名称
        /// </summary>
        public HashSet<CPair<string, string>> Outputs { get; set; }

        /// <summary>
        /// 被调用的函数名称
        /// </summary>
        public HashSet<string> Calleds { get; set; }

        /// <summary>
        /// 主调函数名称，即调用了这个函数的那些函数的名称
        /// </summary>
        public HashSet<string> Callees { get; set; }

        /// <summary>
        /// 函数的调用层次, 顶层函数是0
        /// </summary>
        public int Level { get; set; }

        public CFunctionReport()
        {
            this.ReturnExpression = new HashSet<string>();
            this.InputParameters = new List<CPair<string, string>>();
            this.OutputParameters = new List<CPair<string, string>>();
            this.LocalVars = new HashSet<CPair<string, string>>();
            this.Outputs = new HashSet<CPair<string, string>>();
            this.Inputs = new HashSet<CPair<string, string>>();
            this.Calleds = new HashSet<string>();
            this.Callees = new HashSet<string>();
        }
    }
}
