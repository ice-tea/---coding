using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
//debug
using System.Diagnostics;

namespace UnitData.Util
{
    public class CodeScope
    {
        public CodeScope()
        {
            allScopeNum = 0;
            currentScopeID = 0;
            Scopes = new List<int>();
            Scopes.Add(currentScopeID);
            vars = new Dictionary<int, SortedSet<string>>();
            vars.Add(currentScopeID, new SortedSet<string>());
            cVars = new ChangeVar();
            ComVars = new Dictionary<string, string>();
        }
        public CodeScope(CodeScope cs)
        {
            this.vars = new Dictionary<int, SortedSet<string>>(cs.vars);
            this.cVars = new ChangeVar(cs.cVars);
            this.ComVars = new Dictionary<string, string>(cs.ComVars);
        }
        //作用域信息是全局共享的
        static private List<int> Scopes;
        static private int currentScopeID;
        static private int allScopeNum;

        private Dictionary<int, SortedSet<string>> vars;
        private ChangeVar cVars;
        //结构体量纲
        private Dictionary<string, string> ComVars;

        public int CurrentScope
        {
            get { return currentScopeID; }
        }

        public void AddCom(string varName, string typeName)
        {
            //为当前作用域初始化空间
            if (!vars.ContainsKey(currentScopeID))
            {
                vars.Add(currentScopeID, new SortedSet<string>());
            }
            vars[currentScopeID].Add(varName);
            string scopeName = currentScopeID + varName;
            Debug.Assert(!ComVars.ContainsKey(scopeName));
            ComVars[scopeName] = typeName;
        }
        public string AddVar(string varName, bool isChangable = false)
        {
            //为当前作用域初始化空间
            if (!vars.ContainsKey(currentScopeID))
            {
                vars.Add(currentScopeID, new SortedSet<string>());
            }

            vars[currentScopeID].Add(varName);
            string scopeName = currentScopeID + varName;
            if (!isChangable)
                return scopeName;
            cVars.addChange(scopeName);
            return cVars.getChange(scopeName);

        }
        //通过复合变量 的Name  得到其复合类型名
        public string GetCom(string varName)
        {
            string scopeName = GetVar(varName);
            Debug.Assert(ComVars.ContainsKey(scopeName));
            return ComVars[scopeName];
        }
        //返回某个变量 带着scope的唯一标识
        public string GetVar(string var, bool isLeft = false)
        {
            int max = 0;
            foreach (int i in Scopes)
            {
                if(vars.ContainsKey(i))
                {
                    if (vars[i].Contains(var))
                        max = i;
                }
            }
            string scopeName = max + var;
            if (cVars.isChange(scopeName))
            {
                if (isLeft)
                {
                    cVars.touchChange(scopeName);
                }
                return cVars.getChange(scopeName);
            }
            return scopeName;
        }
        //增加
        public void AddScope()
        {
            allScopeNum++;
            Scopes.Add(allScopeNum);
            currentScopeID = allScopeNum;
        }
        public void MinusScope()
        {
            Debug.Assert(Scopes.Last() == currentScopeID);
            Scopes.Remove(currentScopeID);
            currentScopeID = Scopes.Last();
        }
    }
    //可变量纲
    public class ChangeNode
    {
        public ChangeNode(string name, int num)
        {
            varID = name;
            changeNum = num;
        }
        public string varID { set; get; }
        public int changeNum { set; get; }
    }
    public class ChangeVar
    {
        private List<ChangeNode> Changes = new List<ChangeNode>();
        private SortedSet<string> names = new SortedSet<string>();
        public ChangeVar() { }

        public ChangeVar( ChangeVar cv)
        {
            //List的深度拷贝
            foreach (ChangeNode c in cv.Changes)
            {
                ChangeNode temp = new ChangeNode(c.varID, c.changeNum);
                this.Changes.Add(temp);
            }
            
            this.names = new SortedSet<string>(cv.names);
        }
        public void addChange(string name)
        {
            ChangeNode node = new ChangeNode(name, 0);
            Changes.Add(node);
            names.Add(name);
        }
        public bool isChange(string name)
        {
            return names.Contains(name);
        }
        public string getChange(string name)
        {
            Debug.Assert(names.Contains(name));
            foreach (ChangeNode n in Changes)
            {
                if (n.varID.Equals(name))
                    return name + n.changeNum;
            }
            return null;
        }
        public void touchChange(string name)
        {
            Debug.Assert(names.Contains(name));
            foreach (ChangeNode n in Changes)
            {
                if (n.varID.Equals(name))
                    n.changeNum = n.changeNum + 1;
            }
        }
    }
}
