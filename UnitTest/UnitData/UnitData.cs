using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitData.Util;
//For caculate
using Solver;
using Solver.Util;
using UnitConfigure;

namespace UnitData
{
    public class UnitDataStruct
    {
        private CodeScope _scope;      //作用域
        private GauseSolver _solver;   //求解器
        private ConstNum _constNums;   //常量

        private static dimension unitInfo = new dimension("dimension.xml");

        public UnitDataStruct()
        {
            _scope = new CodeScope();
            _solver = new GauseSolver();
            _constNums = new ConstNum();
        }
        //拷贝构造
        public UnitDataStruct(UnitDataStruct ud)
        {
            this._solver = new GauseSolver(ud._solver);
            this._scope = new CodeScope(ud._scope);
            this._constNums = new ConstNum(ud._constNums);
        }
        //静态函数，返回无量纲类型
        public static dimensionNode unitZero()
        {
            return unitInfo.getDimension(0);
        }
        public void AddConstraint(ICollection<WTuple<string, double>> constraint, DNode constant, string info = "")
        {
            _solver.AddConstraint(constraint, constant, info);
        }
        public void Union(UnitDataStruct another)
        {
            this._solver.Union(another._solver);
        }
        //作用域更改
        //增加
        public void AddScope()
        {
            _scope.AddScope();
        }
        public void MinusScope()
        {
            _scope.MinusScope();
        }
        //量纲变量注册
        public string register(string annotation)
        {
            annotation = annotation.Replace(" ", "");//去除字符串空格
            Regex pre = new Regex("^{\"#(.*)\",\"(.*)\"}");//匹配整体{type,type}结构
            Regex digit_type = new Regex("^-?\\d+[\\.]?\\d*$");//匹配数字
            Regex variable_type = new Regex("^[a-zA-Z]+\\d*$");//匹配变量
            Regex struct_type = new Regex("^{([a-zA-Z]+\\d*)}([a-zA-Z]+\\d*)");//匹配结构体


            Match pre_match = pre.Match(annotation);
            //判断是否符合字符串类型
            if (pre_match.Success)
            {
                string left = pre_match.Groups[1].ToString();
                string right = pre_match.Groups[2].ToString();
                Match digit_match = digit_type.Match(left);

                //判断是否为数字
                if (digit_match.Success)
                {
                    registerNum(left, right);
                }

                //判断是否为变量
                Match variable_match = variable_type.Match(left);
                if (variable_match.Success)
                {
                    registerVar(left, right);
                }

                //判断是否为结构体
                Match struct_match = struct_type.Match(left);
                if (struct_match.Success)
                {
                    string fName = struct_match.Groups[2].ToString();
                    string sName = struct_match.Groups[1].ToString();
                    registerCom(sName, fName, right);
                }

            }
            return null;
        }
        public string get(string name, bool isLeft = false)
        {
            string UnitSystemName;

            Regex digit_type = new Regex("^-?\\d+[\\.]?\\d*$");//匹配数字
            Regex variable_type = new Regex("^[a-zA-Z]+\\d*$");//匹配变量


            Match digit_match = digit_type.Match(name);
            //判断是否为数字
            if (digit_match.Success)
            {
                if (name.Contains('.'))
                {
                    UnitSystemName = _constNums.getConstFloat(float.Parse(name));
                    return UnitSystemName;
                }
                else
                {
                    UnitSystemName = _constNums.getConstInt(int.Parse(name));
                    return UnitSystemName;
                }
            }

            //判断是否为变量
            Match variable_match = variable_type.Match(name);
            if (variable_match.Success)
            {
                UnitSystemName = _scope.GetVar(name, isLeft);
                return UnitSystemName;
            }

            return null;
        }
        //数字量纲注册
        public void registerNum(string left, string right)
        {
            string name;
            if (left.Contains('.'))
            {
                name = _constNums.registerFloat(left);
            }
            else
            {
                name = _constNums.registerInt(left);
            }
            //在量纲类型系统中注册
            dimensionNode unitAnnotate = unitInfo.getDimension(right);

            List<WTuple<string, double>> list = new List<WTuple<string, double>>();
            list.Clear();
            list.Add(new WTuple<string, double>(name, 1));
            _solver.AddConstraint(list, new DNode(unitAnnotate), left + " = " + right);

        }
        //一般量纲注册
        public void registerVar(string left, string right)
        {
            string name;
            if (right == "?")
            {
                _scope.AddVar(left, true);
            }
            else
            {
                name = _scope.AddVar(left);
                //在量纲类型系统中注册
                dimensionNode unitAnnotate = unitInfo.getDimension(right);

                List<WTuple<string, double>> list = new List<WTuple<string, double>>();
                list.Clear();
                list.Add(new WTuple<string, double>(name, 1));
                _solver.AddConstraint(list, new DNode(unitAnnotate), name + " = " + right);
            }

        }
        //复合结构注册
        public void registerCom(string sName, string fName, string right)
        {
            string name = sName + "." + fName;
            //在量纲类型系统中注册
            dimensionNode unitAnnotate = unitInfo.getDimension(right);

            List<WTuple<string, double>> list = new List<WTuple<string, double>>();
            list.Clear();
            list.Add(new WTuple<string, double>(name, 1));
            _solver.AddConstraint(list, new DNode(unitAnnotate), name + " = " + right);

        }
        public void addCom(string varName, string typeName)
        {
            _scope.AddCom(varName, typeName);
        }
        //变量名
        public string getCom(string varName, string fName)
        {
            return _scope.GetCom(varName) + "." + fName;
        }
        public string addVar(string varName)
        {
            return _scope.AddVar(varName);
        }
        public override string ToString()
        {
            return _solver.ToString();
        }

    }
}
