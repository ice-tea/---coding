using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace UParser
{
    class Decoder
    {
        private int line =-1;//行号
        private List<Tuple<string,string,int>> listS = new List<Tuple<string, string, int>>();//等号左边，等号右边，行号

        //private List<Tuple<string, string,int, int>> listMacro = new List<Tuple<string,string,int, int>>();//常量宏,以及其他宏，第三元为标志位，0标示常量宏
        private Tuple<string, string, int, int> macroDef=null;

        private string str;
        private int index = 0;//指向未处理的第一个字符

        private int codeEnd1 = -1;//指向#define前的最后一个字符
        private int codeEnd2 = -1;//指向//@前的最后一个字符

        private int lastState = 0;//0表示行首，1表示上一行有块注释没结束

        public Decoder()
        {
            str = "";
        }
       /* public Decoder(string s,int li)
        {
            str = s;
            line = li;
        }  */     

        private List<Tuple<string, string, int>> getListS()
        {
            return listS;
        }
        //private List<Tuple<string, string,int,int>> getListMacro()
        //{
        //    return listMacro;
        //}
        private Tuple<string, string, int, int> getMacroDef()
        {
            return macroDef;
        }
        private int getCodeE2()
        {
            return codeEnd2;
        }
        public void Decode(StreamReader sr, StreamWriter sw)
        {     
            str = "";
           
            while ((str = sr.ReadLine()) != null)
            {
                this.DecodeAss();
          
                //List<Tuple<string, string, int>> listS = this.getListS();
                //List<Tuple<string, string, int, int>> listMacro = this.getListMacro();

                //printAss(listS, listMacro,sw);
                printAss(sr,sw);
            };
        }
        //private void printAss(List<Tuple<string, string, int>> listS, List<Tuple<string, string, int, int>> listMacro,StreamWriter sw)
        private void printAss(StreamReader sr, StreamWriter sw)
        {
            //if (listMacro.Count() == 0 && listS.Count() == 0)//没有宏，没有注释，不用处理
            if (macroDef== null && listS.Count() == 0)//没有宏，没有注释，不用处理
            {
                sw.WriteLine(str);//+ " line" + line);
                return;
            }
           
            if (codeEnd1 != -1)//本行有宏存在
            {
                string code = str.Substring(0,codeEnd1+ 1);
                sw.WriteLine(code);
            }

            /*IEnumerator im = listMacro.GetEnumerator();
            while (im.MoveNext())
            {
                Tuple<string, string, int, int> t1 = (Tuple<string, string, int, int>)im.Current;
                if (t1.Item3 == 0)//常量宏const double  
                {
                    sw.WriteLine("const double " + t1.Item1 + "=" + t1.Item2 + ";");//+" line" + line);
                }
                else//t1.Item3==1
                {
                    sw.WriteLine("#define " + t1.Item1 + " " + t1.Item2);
                }
            }*/

            if (macroDef != null)
            {
                if (macroDef.Item3 == 0)//常量宏const double  
                {
                    sw.WriteLine("const double " + macroDef.Item1 + "=" + macroDef.Item2 + ";");//+" line" + line);
                }
                else//macroDef.Item3==1
                {
                    sw.WriteLine("#define " + macroDef.Item1 + " " + macroDef.Item2);
                }
                /*处理多行#define*/
                string s=macroDef.Item2.TrimEnd();
                char ch=s[s.Length-1];
                while(ch=='\\')
                {
                    s = sr.ReadLine();
                    sw.WriteLine(s);
                    line++;

                    s = s.TrimEnd();
                    ch = s[s.Length - 1];
                }
            }

            IEnumerator ie = listS.GetEnumerator();
            while (ie.MoveNext())
            {
                Tuple<string, string, int> t = (Tuple<string, string, int>)ie.Current;

                if (t.Item2 == "")
                {
                    sw.WriteLine("//" + t.Item1);//+ " line" + line);
                }
                    //生成量纲声明函数，实参是 string字符串
                    //ADD LIBO 使用字符串声明形式 为了唯一性 加入行号
                else
                    sw.WriteLine("char* MYUNIT" + line +"[] = { \"" + t.Item1 + "\" , \"" + t.Item2 + "\" };");//+ " line" + line);
            }

            if (this.getCodeE2() != -1)////@前是否需要输出
            {
                string code = str.Substring(0, this.getCodeE2() + 1);
                sw.WriteLine(code);
            }                    
        }
        private void DecodeAss()
        {
            codeEnd1 = -1;
            codeEnd2 = -1;
            index = 0;
            line++;
            listS.Clear();
          // listMacro.Clear();
            macroDef = null;
           // if (str.Length-index <= 3)
           //     return;
            if(lastState==1)
            {
                Star1();
            }
            else
                Consume();
        }
        

        private void Pound()//#号处理
        {
            int tid = index;
            //index指向#后的第一个字符，可能是空格
            while (str[index] == ' ')
            {
                index++;
            }
            if (str[index] != 'd')//一定不是define定义
            {
                index = tid;
                Consume();
                return;
            }
            string s = str.Substring(index, 6);
            if (s != "define")
            {
                index = tid;
                Consume();
            }
            else
            {
                index = index + 6;
                codeEnd1 = tid - 2;//指向#号前最后一个字符的
                MacroDeal();
            }
        }
        private void MacroDeal()
        {
            //index指向define后的第一个字符
           // codeEnd1 = index - 8;//指向#号前最后一个字符的

            while(str[index]==' ')//在前导空格存在的情况下更新index值
            {
                index++;
            }
            
            string a;
            string b;
            Tuple<string, string,int, int> t;
            string s = str.Substring(index);
            s = s.TrimEnd();

            int tid = index;
            while(str[tid]!=' ')
            {
                tid++;
            }
            a = str.Substring(index,tid-index);
            b = str.Substring(tid).Trim();


            if (this.IsNum(b))
            {
                t = new Tuple<string, string, int, int>(a, b, 0, line);
            }
            else
            {
                t = new Tuple<string, string, int, int>(a, b, 1, line);
            }
           // listMacro.Add(t);
            macroDef = t;
        }
        
        //处理//@前字符
        private void Consume()
        {
            if (str.Length - index <= 3)
                return;

            while (index < str.Length - 1 && str[index] != '/' && str[index] != '\'' && str[index] != '\"' && str[index] != '#')
            {
                index++;
            }
            if (index == str.Length - 1)//行末
                return;

            if (str[index] == '/')
            {
                index++;
                Slash1();
            }
            else if (str[index] == '\"' || str[index] == '\'')
            {
                char ch = str[index];
                index++;
                QuoteDeal(ch);
            }
            else if (str[index] == '#')
            {
                index++;
                Pound();
            }
        }
        //遇到第一个/
        private void Slash1()
        {
            if (str[index] == '/')
            {
                index++;
                Slash2();
            }
            else if (str[index] == '*')
            {
                index++;
                Star1();
            }
            else if (str[index] == '\"' || str[index] == '\'')
            {
                char ch = str[index];
                index++;
                QuoteDeal(ch);
            }
            else 
            {
                index++;//index++;
                Consume();
            }
        }
        private void Slash2()
        {
            if (str[index] == '@')
            {
                index++;
                Resolve();
            }
            else if (str[index] == '\"' || str[index] == '\'')
            {
                char ch = str[index];
                index++;
                QuoteDeal(ch);
            }
            else
            {
                index++;//index++;
                Consume();
            }
        }

        private void Resolve()
        {
            if (index-4>=0)
            {
                codeEnd2 = index - 4;
            }
            string dim = str.Substring(index);

            dim = dim.Replace(" ", "");//去除多余空格
            string[] ele = dim.Split(';');
            string s = "";
            Tuple<string, string, int> t;
            for (int i = 0; i < ele.Length; i++)
            {
                s = ele[i];
                if (s != "")//处理行末；
                {
                    string[] ops = s.Split('=');
                    if (ops.Length == 2)
                    {
                         t = new Tuple<string, string,int>(ops[0], ops[1],line);
                    }
                    else
                    {
                        //处理分行间非a=b的情况，按照注释输出
                        t = new Tuple<string, string, int>(ops[0], "", line);
                       // listS.Add(ops[0]);
                       // listS.Add("");
                    }
                    listS.Add(t);
                }
            }
            return;
        }
        private void Star1()
        {
            if (str.Length == 0 && lastState == 1)
            {
               // lastState = 1;
                return;
            }

            while (index<str.Length-1&&str[index] != '*')
            {
                index++;
            }
            if (index == str.Length - 1)
            {
                lastState = 1;
                return;
            }
            index++;
            Star2();
        }

        private void Star2()
        {
            if (str[index] == '/')
            {
                lastState = 0;
                index++;
                Consume();
            }
            else if (str[index] == '*' && str[index+1] == '/')
            {//处理块注释结束时**/的情况
                lastState = 0;
                index = index + 2 ;
                Consume();
            }
            else
            {
                index++;//index++;
                Star1();
            }
        }
        //处理单引号和双引号
        private void QuoteDeal(char ch)
        {
            //引号后面第一个位置
            while (str[index] != ch)
            {
                if(str[index]=='\\')//处理引号中出现的转义字符
                    index++;
                index++;
            }
            //此时index指向第二个引号

            index++;//引号后面第一个位置
            Consume();
        }
        
        //一个可以匹配整数、浮点数的正则表达式
        private bool IsNum(string s)
        {
            return Regex.IsMatch(s, @"^[+-]?([0-9]*\.?[0-9]+|[0-9]+\.?[0-9]*)([eE][+-]?[0-9]+)?$");
        }
    }
}
