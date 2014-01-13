using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace UParser
{
    public class UnitParser
    {
        private string[] filenames;

        public UnitParser(string[] files)
        {
            this.filenames = files;
        }

        //对源程序 量纲注释信息做变形
        public void Parse()
        {
            Decoder de = new Decoder();
            FileStream inFileStream;
            FileStream outFileStream;
            foreach (string filename in this.filenames)
            {
                inFileStream = File.Open(filename, FileMode.Open, FileAccess.Read);

                //创建目标文件
                string unitFilename = filename.Replace(".c" , "Unit.c");
                outFileStream = File.Open(unitFilename, FileMode.Create, FileAccess.Write);

                StreamReader sr = new StreamReader(inFileStream);
                StreamWriter sw = new StreamWriter(outFileStream);

                de.Decode(sr, sw);

                sr.Close();
                sw.Close();
                inFileStream.Close();
                outFileStream.Close();
            }
        }
        static void Main(string[] args)
        {
            Decoder de = new Decoder();
            FileStream inFileStream = File.Open("in.c", FileMode.Open, FileAccess.Read);

            FileStream outFileStream = File.Open("out.c", FileMode.Open, FileAccess.Write);

            StreamReader sr = new StreamReader(inFileStream);
            StreamWriter sw = new StreamWriter(outFileStream);

            de.Decode(sr,sw);

            sr.Close();
            sw.Close();
            inFileStream.Close();
            outFileStream.Close();
        }
    }
}

