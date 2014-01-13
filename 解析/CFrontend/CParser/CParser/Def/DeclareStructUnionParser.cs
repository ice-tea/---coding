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
     * 用于解析结构体、联合体类型的定义
     * 例如： 
     * struct _exception {
        int type;       
        char *name;     
        double arg1;    
        double arg2;    
        double retval;  
        } ;
     * */
    class DeclareStructUnionParser : CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            StructUnionParser suParser = new StructUnionParser();
            XmlNode suNode = node.SelectSingleNode("type_specifier_struct_union");
            return suParser.Parse(suNode, cvc, ctc);
        }

        public static bool Match(XmlNode node)
        {
            // 首先，检查是否存在 storage_class_specifier 子节点
            //XmlNode n1 = node.SelectSingleNode("/declaration/storage_class_specifier");
            XmlNode n1 = node.SelectSingleNode("storage_class_specifier");
            if (n1 == null)
            {
                //XmlNode n2 = node.SelectSingleNode("/declaration/type_specifier_struct_union");
                XmlNode n2 = node.SelectSingleNode("type_specifier_struct_union");
                if (n2 != null)
                    return true;
            }

            return false;
        }
    }

    /* 
     * 用于解析结构体、联合体类型的定义
     * 是 DeclareStructUnionParser 和 DeclareTypeDefParser2 的基础
     * */
    class StructUnionParser : CEntityParser
    {
        private static uint nameID = 0;

        public static string GetNameID()
        {
            nameID ++;
            return "__unknown_struct_union_" + nameID;
        }

        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            CDerivedType cdType = new CDerivedType();

            // 检查是union 还是 struct
            string metaTypeStr = this.GetAttribute(node, "struct_union", "token");
            if (metaTypeStr != null && metaTypeStr == "union")
                cdType.IsUnion = true;
            else cdType.IsUnion = false;

            // 获取类型的名称
            cdType.Name = this.GetAttribute(node, "struct_union_id", "token");
            if (cdType.Name == null) 
            {
                // 匿名的结构体/联合体
                cdType.Name = GetNameID();
            }

            // 处理结构体/联合体的各个域成员
            XmlNodeList fieldNodeList = node.SelectNodes("struct_declaration");
            foreach (XmlNode fieldNode in fieldNodeList)
            {
                CField field = this.HandleField(fieldNode, cvc, ctc);
                cdType.AddField(field);
            }

            ctc.AddCEntity(cdType);
            return cdType;
        }

        /*
         * 处理结构体d
         * */
        private CField HandleField(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            CType ctype = null;
            string typeName = "int";
            string fieldName = "";
            Dimension dim = new Dimension();
            uint bit = 0;

            // 首先处理类型标识符
            XmlNode tsaNode = node.SelectSingleNode("type_specifier_atomic");
            if (tsaNode != null)
            {
                // 直接使用了之前定义过的类型
                typeName = tsaNode.Attributes["token"].Value;
            }
            else
            {
                // 说明类型标识符的位置是一个类型定义
                // type_specifier_struct_union
                XmlNode suNode = node.SelectSingleNode("type_specifier_struct_union");
                StructUnionParser suParser = new StructUnionParser();
                CEntity entity = suParser.Parse(suNode, cvc, ctc);
                typeName = entity.Name;
            }

            // 然后处理field的名称
            /*
             *  <struct_declarator>
                    <declarator>
                        <direct_declarator_id token="arg1"/>
                    </declarator>
                </struct_declarator>
             * */
            XmlNode nameNode = node.SelectSingleNode("struct_declarator/declarator/direct_declarator_id");

            if (nameNode != null)
            {
                fieldName = nameNode.Attributes["token"].Value;
                // 接下来处理位域信息
                // 例如：
                /* unsigned EPA : 1;
                 * 
                 * <struct_declaration>
                        <type_specifier_atomic token="unsigned"/>
                        <struct_declarator>
                            <declarator>
                                <direct_declarator_id token="EPA"/>
                            </declarator>
                            <expression_cte token="1"/>
                        </struct_declarator>
                    </struct_declaration>
                */
                XmlNode bitNode = node.SelectSingleNode("struct_declarator/expression_cte");
                if (bitNode != null)
                {
                    bit = UInt32.Parse(bitNode.Attributes["token"].Value);
                }
            }
            else
            {
                //该成员是数组
                XmlNode arrayNode = node.SelectSingleNode("struct_declarator/declarator/direct_declarator_array");
                if (arrayNode != null)
                {
                    // 实例化 DeclaratorArrayParser 处理数组类型的成员
                    DeclaratorArrayParser arrayParser = new DeclaratorArrayParser();
                    arrayParser.Parse(arrayNode);
                    dim = arrayParser.Dim;
                    fieldName = arrayParser.VarName;
                }
                else
                {
                    // 出错！
                    fieldName = "UknownField";
                }
            }
            
            // 最后检查是不是指针类型
            XmlNode ptrNode = node.SelectSingleNode("struct_declarator/declarator/pointer");
            if (ptrNode != null)
            {
                // 如果是指针类型
                uint level = ComputePtrLevel(ptrNode);
                CType ptoType = ctc.GetCEntity(typeName);
                ctype = new CPtrType(ptoType, level);
            }
            else
            {
                // 不是指针类型
                ctype = ctc.GetCEntity(typeName);
            }
            if (ctype != null)
                ctc.AddCEntity(ctype);

            CField field = new CField(fieldName, ctype, dim, bit);

            return field;
        }

        public static bool Match(XmlNode node)
        {
            if (node.Name == "type_specifier_struct_union")
                return true;
            else return false;
        }
    }

}
