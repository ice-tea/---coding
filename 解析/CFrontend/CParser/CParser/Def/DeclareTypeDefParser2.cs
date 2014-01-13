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
     * 用于解析复杂的typedef
     * 例如： 
     * typedef struct TAG_TIME
        {
            float64 mSysTime;              
            float64 mAttTime;              
            float64 mLastSysTime;          
            float64 mSatTime;              
            float64 timeGap;               
            float64 timeIncrement;         
            float64 timeError;             
            float32 timelimit;             
            boolean bTimeChoose;           
            boolean bEnableEvenTime;       
            boolean bTimeCheck;            
        }TIME_DATA;
     * */
    class DeclareTypeDefParser2 : CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            // 分析类型定义的子节点
            XmlNode typeSpecifierNode = node.SelectSingleNode("type_specifier_struct_union");
            StructUnionParser suParser = new StructUnionParser();
            CEntity entity = suParser.Parse(typeSpecifierNode, cvc, ctc);
            string typeDefName = entity.Name;

            /*string typeDefName = "";
            XmlNode suTypeNameNode = node.SelectSingleNode("type_specifier_struct_union/struct_union_id");
            if (suTypeNameNode != null)
            {
                typeDefName = suTypeNameNode.Attributes["token"].Value;
            }
            else
            {
                typeDefName = StructUnionParser.GetNameID();
            }*/

            // 获取类型名称
            string typeName = this.GetAttribute(node, "init_declarator/declarator/direct_declarator_id", "token");

            CTypeDef typeDef = new CTypeDef();
            typeDef.Name = typeName;
            typeDef.type = ctc.GetCEntity(typeDefName);
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
                    XmlNode n2 = node.SelectSingleNode("type_specifier_struct_union");
                    // 最后检查是否存在子节点 type_specifier_struct_union
                    if (n2 != null)
                        return true;
                }
            }

            return false;
        }
    }

}
