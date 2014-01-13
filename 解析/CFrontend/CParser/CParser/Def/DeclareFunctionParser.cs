using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.util;

namespace CFrontendParser.CParser.Def
{
    /* 
     * 用于处理函数声明
     * 结果是CFuncType
     * */
    class DeclareFunctionParser: CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            CFuncType funcType = new CFuncType();
            // modifier
            this.SetModifier(funcType, node);

            // Signature
            SignatureParser sigParser = new SignatureParser();
            sigParser.Parse(node, cvc, ctc);
            funcType.Name = sigParser.FunctionName;
            funcType.ReturnType = sigParser.ReturnType;
            int i;
            for (i = 0; i < sigParser.Count; i++)
            {
                funcType[i] = sigParser[i];
            }

            ctc.AddCEntity(funcType);
            return funcType;
        }

        public static bool Match(XmlNode node)
        {
            if (node.Name == "declaration")
            {
                XmlNode c = node.SelectSingleNode("init_declarator/declarator/direct_declarator_function");
                if (c != null)
                    return true;
                else return false;
            }
            else return false;
        }
    }

    /* 
     * 查找函数的返回值类型
     * 以及函数的参数列表
     * */
    class SignatureParser : CEntityParser
    {
        private string functionName;
        private CType returnType;
        private List<string> parametersName;
        private List<CType> parametersType;

        public string FunctionName
        {
            get { return this.functionName; }
        }

        public CType ReturnType
        {
            get { return this.returnType; }
        }

        /* 参数的数目 */
        public int Count
        {
            get { return this.parametersType.Count; }
        }

        public CPair<string, CType> this[int idx]
        {
            get
            {
                return new CPair<string, CType>(this.parametersName[idx], this.parametersType[idx]);
            }
        }

        public SignatureParser()
        {
            this.parametersName = new List<string>();
            this.parametersType = new List<CType>();
        }

        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            // 首先，获取函数名称
            if (node.Name == "declaration")
                this.functionName = this.GetAttribute(node, "init_declarator/declarator/direct_declarator_function/direct_declarator_id", "token");
            else
                this.functionName = this.GetAttribute(node, "declarator/direct_declarator_function/direct_declarator_id", "token");

            // 然后，获取返回值类型
            this.returnType = this.ParseType(node, ctc);

            // 接下来处理参数列表
            XmlNodeList parameterNodes = node.SelectNodes(".//parameter_declaration");
            foreach (XmlNode paraNode in parameterNodes)
            {
                this.ParseParameter(paraNode, ctc);
            }

            return this.returnType;
        }
        
        private CType ParseType(XmlNode node, CEntityCollection<CType> ctc)
        {
            // 按照名字获取返回值类型
            string typeName = this.GetAttribute(node, "type_specifier_struct_union/struct_union_id", "token");
            if (typeName == null || typeName.Length == 0)
            {
                typeName = this.GetAttribute(node, "type_specifier_atomic", "token");
            }
            CType type = ctc.GetCEntity(typeName);

            // 接下来，检查是否是指针型返回值
            XmlNode ptrNode;
            // 函数声明
            if (node.Name == "declaration")
            {
                ptrNode = node.SelectSingleNode("init_declarator/declarator/pointer");
            }
            else
            {
                // 其他情况
                ptrNode = node.SelectSingleNode("declarator/pointer");
            }
            if (ptrNode != null)
            {
                uint ptrLevel = this.ComputePtrLevel(ptrNode);
                type = new CPtrType(type, ptrLevel);
            }
            ctc.AddCEntity(type);
            return type;
        }

        private void ParseParameter(XmlNode node, CEntityCollection<CType> ctc)
        {
            if (node.Name != "parameter_declaration")
                return;
            string paraName = this.GetAttribute(node, "declarator/direct_declarator_id", "token");
            CType type = this.ParseType(node, ctc);
            this.parametersName.Add(paraName);
            this.parametersType.Add(type);
        }
    }
}
