using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;

namespace UnitConfigure
{
    public class dimension
    {
        private string xmlPath;
        //private HashSet<dimensionNode> resultSet=new HashSet<dimensionNode>();
        private Hashtable resultTable = new Hashtable();

        public dimension(string path)
        {
            xmlPath = path;
            getBaseDimension();
            getExtDimension();
        }
        public dimensionNode getDimension(int i)
        {
            //Console.WriteLine("该记录从hash表中取得某量纲"); 确保m 在其中
            dimensionNode a = (dimensionNode)this.resultTable["m"];
            dimensionOperator Operator = new dimensionOperator();
            return Operator.pow(a, 0);
        }
        public dimensionNode getDimension(string dimName)//使用该方法获取量纲节点
        {
            if (this.resultTable.ContainsKey(dimName))
            {
                //Console.WriteLine("该记录从hash表中取得");
                return (dimensionNode)this.resultTable[dimName];
            }
            else
            {
                Console.WriteLine("量纲未查找到！");
                return null;
            }
        }
        
        private int getBaseDimension()//读取xml基础量纲
        {
            XmlDocument dimensionXMl = new XmlDocument();
            dimensionXMl.Load(xmlPath);

            XmlNodeList baseNodeList = dimensionXMl.SelectNodes("/dimension/base/node");
            if (baseNodeList != null)
            {
                for (int i = 0; i < baseNodeList.Count; i++)//遍历基础量纲节点
                {
                    string name = baseNodeList[i].Attributes["name"].Value;
                    //所查找内容为基础量纲

                    dimensionNode dimNode = new dimensionNode();
                    dimNode.setDimensionName(name);

                    Console.WriteLine("初始化基础量纲:" + name);

                    for (int j = 0; j < baseNodeList.Count; j++)//生成量纲节点
                    {
                        if (j == i)
                        {
                            dimNode.addDimension(1);
                        }
                        else
                        {
                            dimNode.addDimension(0);
                        }
                        dimNode.addCoefficientAndOffset(1, 0);
                    }
                    this.resultTable.Add(name, dimNode);
                    // return dimNode;

                    //所查找内容可能为基础子量纲

                    XmlNodeList baseChildList = baseNodeList[i].ChildNodes;
                    if (baseChildList != null)
                    {
                        for (int k = 0; k < baseChildList.Count; k++)
                        {
                            string nameChild = baseChildList[k].Attributes["name"].Value;
                            //所查找内容为基础子量纲

                            dimensionNode dimNode2 = new dimensionNode();
                            dimNode2.setDimensionName(nameChild);

                            Console.WriteLine("初始化基础子量纲:" + nameChild);

                            for (int l = 0; l < baseNodeList.Count; l++)
                            {
                                if (l == i)
                                {
                                    float coefficient = float.Parse(baseChildList[k].Attributes["coefficient"].Value);
                                    float offset = float.Parse(baseChildList[k].Attributes["offset"].Value);
                                    dimNode2.addCoefficientAndOffset(coefficient, offset);
                                    dimNode2.addDimension(1);
                                }
                                else
                                {
                                    dimNode2.addDimension(0);
                                }
                            }
                            this.resultTable.Add(nameChild, dimNode2);
                            //return dimNode;

                        }
                    }
                }
            }
            return 0;
        }

        private int getExtDimension()//读取xml扩展量纲，有待完善
        {
            XmlDocument dimensionXMl = new XmlDocument();
            dimensionXMl.Load(xmlPath);
            XmlNodeList extNodeList = dimensionXMl.SelectNodes("/dimension/extends/node");
            if(extNodeList!=null)
            {
                for (int i = 0; i < extNodeList.Count; i++)
                {
                    string name = extNodeList[i].Attributes["name"].Value;
                    string descript = extNodeList[i].Attributes["descript"].Value;
                    Console.WriteLine("初始化扩展量纲:" + name + "具体信息为:" + descript);
                    this.resultTable.Add(name, parseExtDimension(descript));

                }
            }
            return 0;
        }

        private dimensionNode parseExtDimension(String define)
        {
            dimensionOperator myOperator=new dimensionOperator();
            dimensionNode tmpNode=null;
            String left = "";
            String right = "";
            if (define.Length >= 1)
            {
                ArrayList result= parseSplit(define);
                left=(String)result[1];
                right = (String)result[2];
                tmpNode=getDimension(right);

                if(tmpNode==null)
                {
                    return null;
                }
                switch((String)result[0])
                {
                    case "":
                        return tmpNode;
                    case "/":
                        return myOperator.div(parseExtDimension(left),tmpNode);
                    case "*":
                        return myOperator.mul( parseExtDimension(left),tmpNode);
                    default: return null; 
                }
            }
            return null;
        }
/*        private ArrayList parseSplit(String parseString)
        {
            ArrayList result = new ArrayList();
            String res = "";
            String tmp = parseString.Substring(0, 1);
            while (tmp != "/" && tmp != "*" && parseString.Length >= 1)
            {
                res = res + tmp;
                parseString = parseString.Substring(1, parseString.Length - 1);
                if (parseString.Length != 0)
                {
                    tmp = parseString.Substring(0, 1);
                }
                else
                {
                    tmp = "";
                }
            }
            result.Add(tmp);
            result.Add(res);
            if (parseString.Length <1)
            {
               result.Add("");
            }
            else 
            {
               result.Add(parseString.Substring(1,parseString.Length-1));
            }
            return result;
         }
*/
        public ArrayList parseSplit(String parseString)
        {
//            Console.WriteLine(parseString);
            ArrayList result = new ArrayList();
            String res = "";
            String tmp = parseString.Substring(parseString.Length-1, 1);
            while (tmp != "/" && tmp != "*" && parseString.Length >= 1)
            {
                res = tmp+res;
                parseString = parseString.Substring(0, parseString.Length - 1);
                if (parseString.Length != 0)
                {
                    tmp = parseString.Substring(parseString.Length-1, 1);
                }
                else
                {
                    tmp = "";
                }
            }
            result.Add(tmp);
            if (parseString.Length < 1)
            {
                result.Add("");
            }
            else
            {
                result.Add(parseString.Substring(0, parseString.Length - 1));
            }
            result.Add(res);


//            Console.WriteLine("operator:"+result[0]+"left:"+result[1]+" right:"+result[2]);
//            Console.WriteLine();

            return result;
        }
    }
}
