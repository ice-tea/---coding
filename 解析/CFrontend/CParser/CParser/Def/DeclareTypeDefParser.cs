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
     * 用于解析简单的typedef
     * 例如： typedef unsigned int    unint32;
     * */
    class DeclareTypeDefParser : CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            XmlNodeList typeSpecfiers = node.SelectNodes("type_specifier_atomic");
            string typeStr = "int";
            if (typeSpecfiers.Count == 1)
            {
                // 获取类型标识符
                XmlNode tNode = typeSpecfiers.Item(0);
                typeStr = tNode.Attributes["token"].Value;
            }
            else if (typeSpecfiers.Count == 2)
            {
                // 处理符号标识符
                XmlNode sNode = typeSpecfiers.Item(0);
                string signStr = sNode.Attributes["token"].Value;
                if (signStr != "unsigned")
                {
                    // 两个类型标识符，第一个必须是unsigned
                    // 不考虑 long long, long double之类的情况
                }

                // 处理类型标识符
                XmlNode tNode = typeSpecfiers.Item(1);
                typeStr = "unsigned " + tNode.Attributes["token"].Value;
            }
            else
            {
                // 类型错误！
            }

            // 获取typedef的类型名称
            XmlNode typeNameNode = node.SelectSingleNode("init_declarator/declarator/direct_declarator_id");
            string typeNameStr = typeNameNode.Attributes["token"].Value;
            CTypeDef typeDef = new CTypeDef();
            typeDef.Name = typeNameStr;

            // 按照名字获取对应的基本类型
            typeDef.type = ctc.GetCEntity(typeStr);
            ctc.AddCEntity(typeDef);
            return typeDef;
        }

        public static bool Match(XmlNode node)
        {
            // 首先，检查是否存在 storage_class_specifier 子节点
            XmlNode n1 = node.SelectSingleNode("storage_class_specifier");
            if (n1 != null && n1.Attributes != null)
            {
                // 该子节点的属性 token 是否等于 "typedef"
                XmlAttribute attr = n1.Attributes["token"];
                if (attr != null && attr.Value == "typedef")
                {
                    XmlNode n2 = node.SelectSingleNode("type_specifier_atomic");
                    // 最后检查是否存在子节点 type_specifier_atomic
                    if (n2 != null)
                        return true;
                }
            }

            return false;
        }
    }

}
