using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunSumC2XML
{
    class CallGraph
    {
        // 所有参与了函数调用关系的函数
        private HashSet<string> functions;

        // 表示函数调用关系
        private Dictionary<string, HashSet<string>> relations;

        // 表示函数被调关系
        private Dictionary<string, HashSet<string>> relations2;

        private Dictionary<string, int> levels;

        /* 顶层函数，即不被任何函数所调用的函数 */
        public HashSet<string> TopFunctions
        {
            get
            {
                HashSet<string> calledFunctions = new HashSet<string>(this.relations2.Keys);
                HashSet<string> topFunctions = new HashSet<string>();
                foreach (string fun in this.functions)
                {
                    if (calledFunctions.Contains(fun) == false)
                        topFunctions.Add(fun);
                }
                return topFunctions;
            }
        }

        public CallGraph()
        {
            this.functions = new HashSet<string>();
            this.relations = new Dictionary<string, HashSet<string>>();
            this.relations2 = new Dictionary<string, HashSet<string>>();
            this.levels = new Dictionary<string, int>();
        }

        /*
         * 获取被 funName 所调用的函数名称集合
         * */
        public HashSet<string> GetCalleds(string funName)
        {
            HashSet<string> s1 = null;
            if (this.relations.TryGetValue(funName, out s1))
                return s1;
            return new HashSet<string>();
        }

        /*
         * 获取调用 funName 的函数名称集合
         * */
        public HashSet<string> GetCallees(string funName)
        {
            HashSet<string> s1 = null;
            if (this.relations2.TryGetValue(funName, out s1))
                return s1;
            return new HashSet<string>();
        }

        /* 新增函数调用关系 */
        public void AddRelation(string callee, string called)
        {
            this.functions.Add(callee);
            this.functions.Add(called);

            // 新增调用关系
            HashSet<string> s1 = null;
            if (this.relations.TryGetValue(callee, out s1) == false)
            {
                s1 = new HashSet<string>();
                this.relations.Add(callee, s1);
            }
            s1.Add(called);

            // 新增被调用关系
            HashSet<string> s2 = null;
            if (this.relations2.TryGetValue(called, out s2) == false)
            {
                s2 = new HashSet<string>();
                this.relations2.Add(called, s2);
            }
            s2.Add(callee);
        }

        /* 计算函数层次 */
        public void ComputeFunctionLevel()
        {
            HashSet<string> topFuns = this.TopFunctions;
            foreach (string funName in topFuns)
            {
                this.ComputeFunctionLevel(funName, 0);
            }
        }

        private void ComputeFunctionLevel(string funName, int level)
        {
            if (this.levels.ContainsKey(funName))
                return;
            this.levels.Add(funName, level);
            HashSet<string> calledFuns = null;
            if (this.relations.TryGetValue(funName, out calledFuns))
            {
                foreach (string called in calledFuns)
                {
                    this.ComputeFunctionLevel(called, level + 1);
                }
            }
        }

        /* 
         * 查询函数调用层次
         * 必须先调用 ComputeFunctionLevel
         */
        public int GetFunctionLevel(string funName)
        {
            int level;
            if (this.levels.TryGetValue(funName, out level))
                return level;
            else return -1;
        }

    }
}
