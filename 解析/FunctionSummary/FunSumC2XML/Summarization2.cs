using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

namespace FunSumC2XML
{
    public partial class Summarization
    {
        private CFunctionReport GetCFunctionReport(XmlNode node)
        {
            CFunctionReport report = new CFunctionReport();

            // 函数名
            report.Name = this.GetFunctionName(node);
            //if (report.Name == "C_BusOutTm")
            //{
            //    report.Name = "C_BusOutTm";
            //}

            // 函数返回值类型
            report.ReturnType = this.GetFunctionReturnType(node);

            // 函数参数名、参数类型
            List<string> paraNames = new List<string>();
            List<string> paraTypes = new List<string>();
            this.GetFunctionParameters(node, out paraNames, out paraTypes);
            report.ParametersName = paraNames;
            report.ParametersType = paraTypes;

            // 局部变量
            report.LocalVars = this.GetLocalVars(node);

            // 返回值表达式
            report.ReturnExpression = this.GetFunctionReturns(node, report);

            // 输入、输出
            this.SummaryInOut(node, ref report);

            // 最后，检查函数调用关系
            report.Calleds = this.GetCalledFunctions(node);

            return report;
        }

        /*
         * 获取函数体中的局部变量的名称
         * 这里假设不同作用域中不会出现重名的局部变量
         * */
        private HashSet<string> GetLocalVars(XmlNode node)
        {
            HashSet<string> vars = new HashSet<string>();
            XmlNodeList declarations = node.SelectNodes(".//declaration");
            foreach (XmlNode var in declarations)
            {
                string varName = this.GetNodeAttributeValue(var, "init_declarator/declarator//direct_declarator_id", "token");
                vars.Add(varName);
            }
            return vars;
        }

        private HashSet<string> GetFunctionReturns(XmlNode node, CFunctionReport report)
        {
            HashSet<string> returns = new HashSet<string>();
            XmlNodeList returnStmts = node.SelectNodes(".//return_statement");
            foreach (XmlNode returnStmt in returnStmts)
            {
                //HashSet<string> s1 = this.GetExpressionIDs(returnStmt, this.globalVars, new HashSet<string>(report.ParametersName), report.LocalVars);
                //HashSet<string> s1 = this.GetExpressionIDs(returnStmt, null, null, null, true);
                //returns.UnionWith(s1);
                if (returnStmt.ChildNodes.Count != 0)
                {
                    XmlNode expressionNode = returnStmt.ChildNodes[0];
                    string exprStr = this.ProcessCExpression(expressionNode);
                    returns.Add(exprStr);
                }
            }
            return returns;
        }

        private void GetFunctionParameters(XmlNode node, out List<string> paraNames, out List<string> paraTypes)
        {
            paraNames = new List<string>();
            paraTypes = new List<string>();
            if (node.Name != "function_definition")
                return;

            XmlNodeList paras = node.SelectNodes("declarator/direct_declarator_function/parameter_list/parameter_declaration");
            foreach (XmlNode para in paras)
            {
                string typeName = this.GetNodeAttributeValue(para, "type_specifier_atomic", "token");
                string paraName = this.GetNodeAttributeValue(para, "declarator/direct_declarator_id", "token");
                XmlNode ptrNode = para.SelectSingleNode("declarator/pointer");
                // 如果该参数是指针型
                if (ptrNode != null)
                {
                    uint level = ComputePtrLevel(ptrNode);
                    while (level > 0)
                    {
                        typeName += "*";
                        level--;
                    }
                }
                paraNames.Add(paraName);
                paraTypes.Add(typeName);
            }
        }

        /*
         * 获取函数的名称
         * */
        private string GetFunctionName(XmlNode node)
        {
            return this.GetNodeAttributeValue(node, "declarator/direct_declarator_function/direct_declarator_id", "token");
        }

        /*
         * 获取函数的返回值类型
         * */
        private string GetFunctionReturnType(XmlNode node)
        {
            string typeName = this.GetNodeAttributeValue(node, "type_specifier_atomic", "token");
            XmlNode ptrNode = node.SelectSingleNode("declarator/pointer");
            if (ptrNode != null)
            {
                uint level = ComputePtrLevel(ptrNode);
                while (level > 0)
                {
                    typeName += "*";
                    level--;
                }
            }
            return typeName;
        }

        /*
         * 计算指针的级数
         */
        private uint ComputePtrLevel(XmlNode ptrNode)
        {
            if (ptrNode == null || ptrNode.Name == "pointer")
            {
                XmlNode nextNode = ptrNode.SelectSingleNode("pointer");
                if (nextNode == null)
                    return 1;
                else return 1 + ComputePtrLevel(nextNode);
            }
            else
            {
                return 0;
            }
        }

        /*
         * 查询函数的输入和输出
         * 输入的node是函数定义节点
         * */
        private void SummaryInOut(XmlNode node, ref CFunctionReport report)
        {
            XmlNode body = node.SelectSingleNode("compound_statement");

            // 首先，处理初始化模块
            XmlNodeList initializerNodes = body.SelectNodes(".//initializer");
            foreach (XmlNode initializer in initializerNodes)
            {
                XmlNode expression = initializer.ChildNodes[0];
                HashSet<string> inputs = this.GetExpressionIDs(expression, this.globalVars, new HashSet<string>(report.ParametersName), report.LocalVars, true);
                report.Inputs.UnionWith(inputs);
            }

            // 然后，获取所有的赋值语句
            XmlNodeList assignmentNodes = body.SelectNodes(".//expression_assignment");
            foreach (XmlNode assignment in assignmentNodes)
            {
                XmlNode leftNode = assignment.ChildNodes[0];
                HashSet<string> outputs = this.GetExpressionIDs(leftNode, this.globalVars, new HashSet<string>(report.ParametersName), report.LocalVars, true);
                report.Outputs.UnionWith(outputs);
                HashSet<string> outParas = this.GetExpressionIDs(leftNode, new HashSet<string>(report.ParametersName));
                report.ModifiedParameters.UnionWith(outParas);
                
                XmlNode rightNode = assignment.ChildNodes[1];
                HashSet<string> inputs = this.GetExpressionIDs(rightNode, this.globalVars, new HashSet<string>(report.ParametersName), report.LocalVars, true);
                report.Inputs.UnionWith(inputs);
            }

            // 然后，检查if, switch, while语句的条件
            this.SummaryInOutBranch(node, ".//if_statement", 0, ref report);
            this.SummaryInOutBranch(node, ".//switch_statement", 0, ref report);
            this.SummaryInOutBranch(node, ".//while_statement", 0, ref report);

            // 检查 do-while, for 语句的条件
            this.SummaryInOutBranch(node, ".//do_statement", 1, ref report);
            this.SummaryInOutBranch(node, ".//for_statement", 1, ref report);
        }

        private void SummaryInOutBranch(XmlNode node, string xpath, int pos, ref CFunctionReport report)
        {
            XmlNodeList stmts = node.SelectNodes(xpath);
            foreach (XmlNode stmt in stmts)
            {
                if (pos < stmt.ChildNodes.Count)
                {
                    XmlNode exprNode = stmt.ChildNodes[pos];
                    HashSet<string> condVars = this.GetExpressionIDs(exprNode, this.globalVars, new HashSet<string>(report.ParametersName), report.LocalVars, true);
                    report.Inputs.UnionWith(condVars);
                }
            }
        }

        private HashSet<string> GetCalledFunctions(XmlNode node)
        {
            HashSet<string> functions = new HashSet<string>();
            XmlNodeList functionNodes = node.SelectNodes(".//expression_function");
            foreach (XmlNode fNode in functionNodes)
            {
                XmlNode idNode = fNode.SelectSingleNode("expression_id");
                if (idNode == null)
                    continue;
                XmlAttribute attr = idNode.Attributes["token"];
                if (attr != null)
                {
                    string calledFunction = attr.Value;
                    functions.Add(calledFunction);
                }
            }
            return functions;
        }

        /*
         * 最后一个参数 noFunction 取值为 true 时，将剔除 函数名称
         * */
        private HashSet<string> GetExpressionIDs(XmlNode node, HashSet<string> globals, HashSet<string> paras, HashSet<string> locals, bool noFunction)
        {
            HashSet<string> IDs = new HashSet<string>();
            HashSet<string> excepts = new HashSet<string>();
            if (paras != null)
                excepts.UnionWith(paras);
            if (locals != null)
                excepts.UnionWith(locals);
            XmlNodeList idNodes = null;
            if (node.Name == "expression_id")
            {
                string idStr = this.GetExpressionID(node, globals, excepts, noFunction);
                if (idStr != null)
                {
                    IDs.Add(idStr);
                }
            }
            else
            {
                idNodes = node.SelectNodes(".//expression_id");
                foreach (XmlNode id in idNodes)
                {
                    string idStr = this.GetExpressionID(id, globals, excepts, noFunction);
                    if (idStr != null)
                    {
                        IDs.Add(idStr);
                    }
                }
            }
            return IDs;
        }

        /*
         * 如果返回值为null, 表示没有获得id
         * */
        private string GetExpressionID(XmlNode idNode, HashSet<string> globals, HashSet<string> excepts, bool noFunction)
        {
            if (noFunction)
            {
                if (idNode.ParentNode.Name == "expression_function")
                    return null;
            }
            XmlAttribute attr = idNode.Attributes["token"];
            if (attr != null)
            {
                string idStr = attr.Value;
                if (globals != null)
                {
                    if (globals.Contains(idStr) == true && excepts.Contains(idStr) == false)
                        return idStr;
                }
                else
                {
                    if (excepts.Contains(idStr) == false)
                        return idStr;
                }
            }
            return null;
        }

        private HashSet<string> GetExpressionIDs(XmlNode node, HashSet<string> scope)
        {
            HashSet<string> IDs = new HashSet<string>();
            XmlNodeList idNodes = null;
            if (node.Name == "expression_id")
            {
                string idStr = this.GetExpressionID(node, scope);
                if (idStr != null)
                {
                    IDs.Add(idStr);
                }
            }
            else
            {
                idNodes = node.SelectNodes(".//expression_id");
                foreach (XmlNode id in idNodes)
                {
                    string idStr = this.GetExpressionID(id, scope);
                    if (idStr != null)
                    {
                        IDs.Add(idStr);
                    }
                }
            }
            return IDs;
        }

        /*
         * 如果返回值为null, 表示没有获得id
         * */
        private string GetExpressionID(XmlNode idNode, HashSet<string> scope)
        {
            if (idNode == null || scope == null)
                return null;
            if (idNode.ParentNode.Name == "expression_function")
                return null;
            XmlAttribute attr = idNode.Attributes["token"];
            if (attr != null)
            {
                string idStr = attr.Value;
                if (scope.Contains(idStr) == true) 
                    return idStr;
            }
            return null;
        }
    }
}
