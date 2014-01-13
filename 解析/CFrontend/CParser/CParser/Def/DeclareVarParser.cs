using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.Expression;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CParser.Expression;

namespace CFrontendParser.CParser.Def
{

    /* 
     * 用于解析变量定义
     * 例如： 
     * unint32 B1553ImportData[128]; 
     * */
    class DeclareVarParser : CEntityParser
    {
        /*
         *     
         * <declaration>
               <type_specifier_atomic token="boolean"/>
               <init_declarator>
                   <declarator>
                       <direct_declarator_id token="bflgOrbitCalEnd"/>
                   </declarator>
               </init_declarator>
           </declaration>
        */

        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            // 获取类型名称
            XmlNode typeNode = node.SelectSingleNode("type_specifier_atomic");
            string typeName = "";
            if (typeNode != null)
            {
                typeName = typeNode.Attributes["token"].Value;
            }
            else
            {
                typeNode = node.SelectSingleNode("type_specifier_struct_union");
                XmlNode idNode = typeNode.SelectSingleNode("struct_union_id");
                if (idNode == null)
                {
                    StructUnionParser suParser = new StructUnionParser();
                    CEntity entity = suParser.Parse(typeNode, cvc, ctc);
                    typeName = entity.Name;
                }
                else
                {
                    typeName = idNode.Attributes["token"].Value;
                    if (ctc.CEntityExists(typeName) == false)
                    {
                        StructUnionParser suParser = new StructUnionParser();
                        suParser.Parse(typeNode, cvc, ctc);
                    }
                }
            }

            XmlNodeList initDeclaratorNodes = node.SelectNodes("init_declarator");
            CType type = ctc.GetCEntity(typeName);
            foreach (XmlNode initNode in initDeclaratorNodes)
            {
                this.ProcessInitDeclarator(type, initNode, cvc, ctc);
            }
            return cvc.CEntities.ElementAt(cvc.Count - 1);
        }

        private void ProcessInitDeclarator(CType typeBasic, XmlNode initNode, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            string varName = null;
            CType type = null;

            // 检查是不是指针变量
            // 形如 int *p[10]
            XmlNode ptrNode = initNode.SelectSingleNode("declarator/pointer");
            if (ptrNode != null)
            {
                uint ptrLevel = this.ComputePtrLevel(ptrNode);
                type = new CPtrType(typeBasic, ptrLevel);
                ctc.AddCEntity(type);
            }
            else
            {
                type = typeBasic;
            }

            // 检查是不是数组变量
            XmlNode arrayNode = initNode.SelectSingleNode("declarator/direct_declarator_array");
            if (arrayNode != null)
            {
                // 如果是数组变量
                DeclaratorArrayParser arrayParser = new DeclaratorArrayParser();
                arrayParser.Parse(arrayNode);

                varName = arrayParser.VarName;

                type = new CArrayType(type, arrayParser.Dim);
                ctc.AddCEntity(type);
            }
            else
            {
                // 获取变量名称
                XmlNode varNameNode = initNode.SelectSingleNode("declarator//direct_declarator_id");
                varName = varNameNode.Attributes["token"].Value;
            }

            // 检查是不是指针变量
            // 形如 int (*p)[10];
            XmlNode ptrBraketsNode = initNode.SelectSingleNode("declarator//direct_declarator_brakets//pointer");
            if (ptrBraketsNode != null)
            {
                uint ptrLevel = this.ComputePtrLevel(ptrBraketsNode);
                type = new CPtrType(type, ptrLevel);
                ctc.AddCEntity(type);
            }

            // 处理初始值
            CExpr initialValue = null;
            XmlNode initializerNode = initNode.SelectSingleNode("initializer");
            if (initializerNode != null)
            {
                CExpressionParser parser = new CExpressionParser();
                initialValue = (CExpr)parser.Parse(initializerNode, cvc, ctc);
            }

            // 构造变量定义
            CVarDefinition var = new CVarDefinition(varName, type, initialValue);
            // 处理修饰符
            this.SetModifier(var, initNode.ParentNode);
            cvc.AddCEntity(var);
        }

        //public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        //{
        //    // 获取类型名称
        //    XmlNode typeNode = node.SelectSingleNode("type_specifier_atomic");
        //    string typeName = "";
        //    if (typeNode != null)
        //    {
        //        typeName = typeNode.Attributes["token"].Value;
        //    }
        //    else
        //    {
        //        typeNode = node.SelectSingleNode("type_specifier_struct_union");
        //        XmlNode idNode = typeNode.SelectSingleNode("struct_union_id");
        //        if (idNode == null)
        //        {
        //            StructUnionParser suParser = new StructUnionParser();
        //            CEntity entity = suParser.Parse(typeNode, cvc, ctc);
        //            typeName = entity.Name;
        //        }
        //        else
        //        {
        //            typeName = idNode.Attributes["token"].Value;
        //            if (ctc.CEntityExists(typeName) == false)
        //            {
        //                StructUnionParser suParser = new StructUnionParser();
        //                suParser.Parse(typeNode, cvc, ctc);
        //            }
        //        }
        //    }


        //    // 检查是不是数组变量
        //    XmlNode arrayNode = node.SelectSingleNode("init_declarator/declarator/direct_declarator_array");
        //    string varName = null;
        //    CType type = null;
        //    if (arrayNode != null)
        //    {
        //        // 如果是数组变量
        //        if (typeName == "B_SHeatParamDft")
        //        {
        //            typeName = "B_SHeatParamDft";
        //        }
        //        DeclaratorArrayParser arrayParser = new DeclaratorArrayParser();
        //        arrayParser.Parse(arrayNode);

        //        varName = arrayParser.VarName;
                
        //        type = ctc.GetCEntity(typeName);
        //        type = new CArrayType(type, arrayParser.Dim);
        //        ctc.AddCEntity(type);
        //    }
        //    else
        //    {
        //        // 获取变量名称
        //        XmlNode varNameNode = node.SelectSingleNode("init_declarator/declarator/direct_declarator_id");
        //        varName = varNameNode.Attributes["token"].Value;
        //        type = ctc.GetCEntity(typeName);
        //    }
        //    // 检查是不是指针变量
        //    XmlNode ptrNode = node.SelectSingleNode("init_declarator/declarator/pointer");
        //    if (ptrNode != null)
        //    {
        //        uint ptrLevel = this.ComputePtrLevel(ptrNode);
        //        type = new CPtrType(type, ptrLevel);
        //        ctc.AddCEntity(type);
        //    }

        //    // 处理初始值
        //    CExpr initialValue = null;
        //    XmlNode initializerNode = node.SelectSingleNode("init_declarator/initializer");
        //    if (initializerNode != null)
        //    {
        //        CExpressionParser parser = new CExpressionParser();
        //        initialValue = (CExpr)parser.Parse(initializerNode, cvc, ctc);
        //    }

        //    // 构造变量定义
        //    CVarDefinition var = new CVarDefinition(varName, type, initialValue);
        //    // 处理修饰符
        //    this.SetModifier(var, node);
        //    cvc.AddCEntity(var);
        //    return var;
        //}

        public static bool Match(XmlNode node)
        {
            XmlNode n1 = node.SelectSingleNode("init_declarator/declarator/direct_declarator_id");
            XmlNode na = node.SelectSingleNode("init_declarator/declarator/direct_declarator_array");
            XmlNode n2 = node.SelectSingleNode("storage_class_specifier");
            if (node.Name == "declaration" && (n1 != null || na != null))
            {
                if (n2 == null)
                {
                    return true;
                }
                else
                {
                    XmlAttribute attr = n2.Attributes["token"];
                    if (attr != null && attr.Value == "typedef")
                        return false;
                    else return true;
                }
            }
            else
            {
                return false;
            }
        }
    }

    /*
     * declarator_array
     * 用于解析数组变量
     * 解析的结果放在属性 Dim 和 VarName 中
     * 分别表示该变量的 维度 和 名称
     * 
     * */
    class DeclaratorArrayParser
    {
        private Dimension dim;
        private string varName;

        public Dimension Dim { get { return dim; } }
        public string VarName { get { return varName; } }

        public DeclaratorArrayParser()
        {
            dim = new Dimension();
        }

        /*
         * <direct_declarator_array>
               <direct_declarator_array>
                   <direct_declarator_array>
                       <direct_declarator_id token="B1553ImportData"/>
                       <expression_cte token="32"/>
                   </direct_declarator_array>
                   <expression_cte token="64"/>
               </direct_declarator_array>
               <expression_cte token="128"/>
           </direct_declarator_array>
         * */
        public void Parse(XmlNode node)
        {
            XmlNode varNameNode = node.SelectSingleNode("direct_declarator_id");
            if (varNameNode != null)
            {
                varName = varNameNode.Attributes["token"].Value;
                XmlNode dimNode = node.ChildNodes[1];
                if (dimNode != null)
                {
                    uint len = this.ArrayLengthEvalue(dimNode);
                    dim.AddDimLast(len);
                }
            }
            else
            {
                XmlNode braketNode = node.SelectSingleNode("direct_declarator_brakets//direct_declarator_id");
                if (braketNode != null)
                {
                    varName = braketNode.Attributes["token"].Value;
                    XmlNode dimNode = node.ChildNodes[1];
                    if (dimNode != null)
                    {
                        uint len = this.ArrayLengthEvalue(dimNode);
                        dim.AddDimLast(len);
                    }
                }
                else
                {
                    XmlNode arrayNode = node.SelectSingleNode("direct_declarator_array");
                    if (arrayNode != null)
                    {
                        Parse(arrayNode);
                        XmlNode dimNode = node.ChildNodes[1];
                        if (dimNode != null)
                        {
                            uint len = this.ArrayLengthEvalue(dimNode);
                            dim.AddDimLast(len);
                        }
                    }
                }
            }
        }

        private uint ArrayLengthEvalue(XmlNode node)
        {
            switch (node.Name)
            {
                case "expression_cte":
                    {
                        string value = node.Attributes["token"].Value;
                        return System.Convert.ToUInt32(value);
                    }
                case "expression_add":
                    {
                        XmlNode n1 = node.ChildNodes[0];
                        XmlNode n2 = node.ChildNodes[1];
                        uint v1 = this.ArrayLengthEvalue(n1);
                        uint v2 = this.ArrayLengthEvalue(n2);
                        return v1 + v2;
                    }
                case "expression_sub":
                    {
                        XmlNode n1 = node.ChildNodes[0];
                        XmlNode n2 = node.ChildNodes[1];
                        uint v1 = this.ArrayLengthEvalue(n1);
                        uint v2 = this.ArrayLengthEvalue(n2);
                        return v1 - v2;
                    }
                case "expression_mul":
                    {
                        XmlNode n1 = node.ChildNodes[0];
                        XmlNode n2 = node.ChildNodes[1];
                        uint v1 = this.ArrayLengthEvalue(n1);
                        uint v2 = this.ArrayLengthEvalue(n2);
                        return v1 * v2;
                    }
            }
            return 1;
        }
    }
}
