using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.Type;
using CFrontendParser.CSyntax.util;
using CFrontendParser.Preprocess;
using CFrontendParser.CParser.Def;

namespace CFrontendParser.CParser
{
    public class ParseCFile
    {
        private string fileName;

        // C2XML生成的XML文件
        private XmlDocument xmlDoc;

        private CEntityCollection<CType> ctc = new CEntityCollection<CType>();
        private CEntityCollection<CVarDefinition> cvc = new CEntityCollection<CVarDefinition>();
        private CEntityCollection<CFunction> cfc = new CEntityCollection<CFunction>();

        public CEntityCollection<CType> CTypes
        { get { return this.ctc; } }

        public CEntityCollection<CVarDefinition> CVars
        { get { return this.cvc; } }

        public CEntityCollection<CFunction> CFunctions
        { get { return this.cfc; } }

        /// <summary>
        /// 构造函数，指定待分析的XML文件的名称
        /// </summary>
        /// <param name="path">XML文件的名称</param>
        public ParseCFile(string path)
        {
            //初始化一个xml实例
            this.xmlDoc = new XmlDocument();

            //导入指定xml文件
            this.xmlDoc.Load(path);
        }

        
        /// <summary>
        /// 构造函数，指定待分析的C源代码文件名称, 该文件已经做过预处理
        /// </summary>
        /// <param name="f">C源代码文件名</param>
        /// <param name="p">是否经过预处理</param>
        /// <param name="s">C语言标准</param>
        public ParseCFile(string f, bool p, CStandard s)
        {
            this.fileName = f;
            C2XML convert = new C2XML();
            this.xmlDoc = convert.C2XmlDocument(f, p, s);
        }

        /* 解析XML中定义的C程序类型和变量 */
        public CFile Parse()
        {
            // 初始化基本类型
            this.Initialize();

            // 开始遍历、解析
            XmlNode translation_unit = this.xmlDoc.SelectSingleNode("/translation_unit");
            XmlNodeList children = translation_unit.ChildNodes;
            for (int i = 0; i < children.Count; i++)
            {
                XmlNode node = children.Item(i);
                Parse(node);
            }

            CEntityCollection<CVarDefinition> gv = new CEntityCollection<CVarDefinition>();
            CEntityCollection<CVarDefinition> ex = new CEntityCollection<CVarDefinition>();
            foreach (CVarDefinition var in this.cvc.CEntityList)
            {
                if (var.IsExtern)
                    ex.AddCEntity(var);
                else
                    gv.AddCEntity(var);
            }

            return new CFile(this.fileName, this.ctc, gv, ex, this.cfc);
        }

        private void Initialize()
        {
            // 基本类型初始化
            // void
            this.ctc.AddCEntity(new CPrimitiveType("void", CBasicType._void));

            // char, unsigned char
            this.ctc.AddCEntity(new CPrimitiveType("char", CBasicType._char));
            this.ctc.AddCEntity(new CPrimitiveType("unsigned char", CBasicType._uchar));

            // short, unsigned short
            this.ctc.AddCEntity(new CPrimitiveType("short", CBasicType._short));
            this.ctc.AddCEntity(new CPrimitiveType("unsigned short", CBasicType._ushort));

            // int, unsighed, unsigned int
            // 将 long 视为 int
            this.ctc.AddCEntity(new CPrimitiveType("int", CBasicType._int));
            this.ctc.AddCEntity(new CPrimitiveType("long", CBasicType._int));
            this.ctc.AddCEntity(new CPrimitiveType("unsigned", CBasicType._uint));
            this.ctc.AddCEntity(new CPrimitiveType("unsigned int", CBasicType._uint));
            this.ctc.AddCEntity(new CPrimitiveType("unsigned long", CBasicType._uint));

            // float, double
            this.ctc.AddCEntity(new CPrimitiveType("float", CBasicType._float));
            this.ctc.AddCEntity(new CPrimitiveType("double", CBasicType._double));
        }

        private void Parse(XmlNode node)
        {
            CEntityParser parser = CEntityParser.GetParser(node);
            if (parser != null)
            {
                CEntity entity = parser.Parse(node, this.cvc, this.ctc);
                if (entity is CFunction)
                    this.cfc.AddCEntity((CFunction)entity);
            }
        }

        public static void TestParserMatch(string path)
        {
            //初始化一个xml实例
            XmlDocument xml = new XmlDocument();

            //导入指定xml文件
            xml.Load(path);
            XmlNode translation_unit = xml.SelectSingleNode("/translation_unit");

            foreach(XmlNode node in translation_unit.ChildNodes)
            {
                if (DeclareStructUnionParser.Match(node))
                {
                    Console.WriteLine("DeclareStructUnionParser");
                }
                else if (DeclareVarParser.Match(node))
                {
                    Console.WriteLine("DeclareVarParser");
                }
                else if (DeclareTypeDefParser.Match(node))
                {
                    Console.WriteLine("DeclareTypeDefParser");
                }
                else if (DeclareTypeDefParser2.Match(node))
                {
                    Console.WriteLine("DeclareTypeDefParser2");
                }
            }
        }

        public static void TestGetTranslationUnit(string path)
        {
            //初始化一个xml实例
            XmlDocument xml = new XmlDocument();

            //导入指定xml文件
            xml.Load(path);
            XmlNode translation_unit = xml.SelectSingleNode("/translation_unit");

            // 遍历子节点
            travel(translation_unit, 1);
        }

        public static void TestXMLInput(string path)
        {
            //初始化一个xml实例
            XmlDocument xml = new XmlDocument();

            //导入指定xml文件
            xml.Load(path);
            Console.WriteLine("Document: " + xml.Name);

            // 遍历子节点
            XmlNodeList children = xml.ChildNodes;
            for (int i = 0; i < children.Count; i++)
            {
                XmlNode node = children.Item(i);
                travel(node, 1);
            }
        }

        private static void travel(XmlNode element, int level)
        {
            string tab = GetTab(level);
            if (element == null)
                return;

            Console.WriteLine(tab + "name: " + element.Name);

            // 遍历属性
            XmlAttributeCollection attrs = element.Attributes;
            if (attrs != null)
            {
                for (int i = 0; i < attrs.Count; i++)
                {
                    XmlNode attr = attrs.Item(i);
                    Console.WriteLine(tab + "\t" + "attr: " + attr.Name);
                }
            }

            // 遍历子节点
            XmlNodeList children = element.ChildNodes;
            for (int i = 0; i < children.Count; i++)
            {
                XmlNode node = children.Item(i);
                travel(node, 1);
            }
        }

        private static string GetTab(int n)
        {
            string str = "";
            while (n > 0)
            {
                str += "\t";
                n--;
            }
            return str;
        }
    }
}
