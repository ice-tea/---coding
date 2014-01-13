using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.CFG;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.util;
using CFrontendParser.CParser;

namespace CFrontendParser.CParser.Def
{
    class FunctionParser: CEntityParser
    {
        public override CEntity Parse(XmlNode node, CEntityCollection<CVarDefinition> cvc, CEntityCollection<CType> ctc)
        {
            DeclareFunctionParser funcParser = new DeclareFunctionParser();
            CEntity typeEntity = funcParser.Parse(node, cvc, ctc);
            XmlNode bodyNode = node.SelectSingleNode("compound_statement");
            CEntityParser bodyParser = CEntityParser.GetParser(bodyNode);
            CEntity bodyEntity = null;
            if (bodyParser != null)
            {
                bodyEntity = bodyParser.Parse(bodyNode, cvc, ctc);
            }
            List<string> paraNames = this.ParseParametersName(node);
            if (typeEntity is CFuncType)
            {
                CFuncType funcType = (CFuncType)typeEntity;                
                ctc.AddCEntity(funcType);

                if (bodyEntity != null)
                {
                    if (bodyEntity is CStmt)
                    {
                        CFunction function = new CFunction(funcType, paraNames, (CStmt)bodyEntity);
                        return function;
                    }
                    else
                    {
                        throw new Exception("fail to parse function " + typeEntity.Name);
                    }
                }
                else
                {
                    return new CFunction((CFuncType)typeEntity, paraNames, null);
                }
            }
            else
            {
                throw new Exception("fail to parse function " + typeEntity.Name);
            }
        }

        private List<string> ParseParametersName(XmlNode node)
        {
            List<string> paraNames = new List<string>();
            XmlNodeList paraNodes = node.SelectNodes("declarator/direct_declarator_function/parameter_list/parameter_declaration");
            foreach (XmlNode paraNode in paraNodes)
            {
                string paraName = this.GetAttribute(paraNode, "declarator//direct_declarator_id", "token");
                paraNames.Add(paraName);
            }
            return paraNames;
        }

        public static bool Match(XmlNode node)
        {
            if (node.Name == "function_definition")
                return true;
            else return false;
        }
    }
}
