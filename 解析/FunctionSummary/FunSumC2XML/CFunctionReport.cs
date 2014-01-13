using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunSumC2XML
{
    public class CFunctionReport
    {
        /* 函数名称 */
        public string Name { set; get; }

        /* 函数所在的C文件名 */
        public string CFileName { set; get; }

        ///* 函数所在的文件的名称 */
        //public string File { set; get; }

        /* 函数返回值类型 */
        public string ReturnType { set; get; }

        /* 函数返回值表达式 */
        public HashSet<string> ReturnExpression { set; get; }

        /* 函数参数类型 */
        public List<string> ParametersType { get; set; }

        /* 函数参数名称 */
        public List<string> ParametersName { get; set; }

        /* 输出参数 */
        public HashSet<string> ModifiedParameters { get; set; }
        
        /* 函数的局部变量定义 */
        public HashSet<string> LocalVars { get; set; }

        /* 输入输出变量集合 */
        public HashSet<string> Outputs { get; set; }
        public HashSet<string> Inputs { get; set; }

        /* 被调用的函数名称 */
        public HashSet<string> Calleds { get; set; }

        /* 主调函数名称，即调用了这个函数的那些函数的名称*/
        public HashSet<string> Callees { get; set; }

        /* 函数的调用层次
         * 顶层函数是0
         */
        public int Level { get; set; }

        public CFunctionReport()
        {
            this.ReturnExpression = new HashSet<string>();
            this.ParametersName = new List<string>();
            this.ParametersType = new List<string>();
            this.ModifiedParameters = new HashSet<string>();
            this.LocalVars = new HashSet<string>();
            this.Outputs = new HashSet<string>();
            this.Inputs = new HashSet<string>();
            this.Calleds = new HashSet<string>();
            this.Callees = new HashSet<string>();
        }

        public string Prototype
        {
            get
            {
                string prototype = this.ReturnType + " " + this.Name;
                string paras = "";
                if (this.ParametersName.Count > 0)
                {
                    paras = this.ParametersType[0] + " " + this.ParametersName[0];
                    for (int i = 1; i < this.ParametersName.Count; i++)
                    {
                        paras = paras + ", " + this.ParametersType[i] + " " + this.ParametersName[i];
                    }
                }
                prototype = prototype + "(" + paras + ")";
                return prototype;
            }
        }
    }
}
