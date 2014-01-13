using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace C2CFG.GraphElement
{
    public class Graph
    {
        //////////////////////////////////////////////////////
        // 成员变量
        /* 入口节点，唯一，指定 */
        private Vertex start;
        public Vertex Start
        { get { return this.start; } }

        public HashSet<Vertex> Ends 
        {
            get 
            {
                HashSet<Vertex> result = new HashSet<Vertex>();
                foreach (KeyValuePair<int,Vertex> v in this.vertices)
                {
                    if (v.Value.NextEdges.Count == 0)
                    {
                        result.Add(v.Value);
                    }
                }
                if (result.Count == 0)
                {
                    result.Add(start);
                }
                return result;
            } 
        }

        /* 节点集合，使用Dictionary，可以通过节点ID快速找到对应节点 */
        private Dictionary<int, Vertex> vertices;
        //
        //////////////////////////////////////////////////////

        //////////////////////////////////////////////////////
        // 成员函数

        /* 构造函数
         * 图的入口节点是额外加的，需向实际起点与此节点相连
         * 其他节点ID从1开始
         */
        public Graph()
        {
            this.vertices = new Dictionary<int, Vertex>();
        }

        public Graph(int sid)
        {
            this.start = new Vertex(sid);
            this.vertices = new Dictionary<int, Vertex>();
            this.vertices.Add(sid, this.start);
        }

        /// <summary>
        /// 从两个已有的图，构造新图
        /// 要求两个图中的点的ID均不重复
        /// </summary>
        /// <param name="g1">图1</param>
        /// <param name="g2">图2</param>
        /// <param name="isSeq">是否顺序连接</param>
        public Graph(Graph g1, Graph g2, bool isSeq)
        {
            this.vertices = new Dictionary<int, Vertex>();
            // 图1
            // 复制各个点
            foreach (KeyValuePair<int, Vertex> pair in g1.vertices)
            {
                Vertex v = new Vertex(pair.Value.ID, pair.Value.Text);
                this.vertices.Add(v.ID, v);
            }
            // 复制各个边
            foreach (KeyValuePair<int, Vertex> pair in g1.vertices)
            {
                foreach (Tuple<string, Vertex> tuple in pair.Value.NextEdges)
                {
                    this.AddEdge(this.vertices[pair.Value.ID], this.vertices[tuple.Item2.ID], tuple.Item1);
                }
            }
            // 复制开始节点
            this.start = this.vertices[g1.start.ID];

            if (isSeq)
            {
                // 图2
                // 复制各个点
                foreach (KeyValuePair<int, Vertex> pair in g2.vertices)
                {
                    if (pair.Value.ID != g2.start.ID)
                    {
                        Vertex v = new Vertex(pair.Value.ID, pair.Value.Text);
                        this.vertices.Add(v.ID, v);
                    }
                }
                // 复制各个边
                HashSet<Vertex> g1ends = g1.Ends;
                foreach (KeyValuePair<int, Vertex> pair in g2.vertices)
                {
                    if (pair.Value.ID == g2.start.ID)
                    {
                        foreach (Vertex sVertex in g1ends)
                        {
                            foreach (Tuple<string, Vertex> tuple in pair.Value.NextEdges)
                            {
                                this.AddEdge(this.vertices[sVertex.ID], this.vertices[tuple.Item2.ID], tuple.Item1);
                            }
                        }
                    }
                    else
                    {
                        foreach (Tuple<string, Vertex> tuple in pair.Value.NextEdges)
                        {
                            this.AddEdge(this.vertices[pair.Value.ID], this.vertices[tuple.Item2.ID], tuple.Item1);
                        }
                    }
                }
            }
            else
            {
                // 图2
                // 复制各个点
                foreach (KeyValuePair<int, Vertex> pair in g2.vertices)
                {
                    if (pair.Value.ID != g2.start.ID)
                    {
                        Vertex v = new Vertex(pair.Value.ID, pair.Value.Text);
                        this.vertices.Add(v.ID, v);
                    }
                }
                // 复制各个边
                foreach (KeyValuePair<int, Vertex> pair in g2.vertices)
                {
                    if (pair.Value.ID == g2.start.ID)
                    {
                        foreach (Tuple<string, Vertex> tuple in pair.Value.NextEdges)
                        {
                            this.AddEdge(this.start, this.vertices[tuple.Item2.ID], tuple.Item1);
                        }
                    }
                    else
                    {
                        foreach (Tuple<string, Vertex> tuple in pair.Value.NextEdges)
                        {
                            this.AddEdge(this.vertices[pair.Value.ID], this.vertices[tuple.Item2.ID], tuple.Item1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 构造函数，用于处理多个分支的情况，即switch语句
        /// 要求各个图中的点的ID均不重复
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="v0"></param>
        /// <param name="graphs"></param>
        public Graph(ref int sid, Vertex v0, List<Graph> graphs)
        {
            this.start = new Vertex(sid);
            sid++;
            this.vertices = new Dictionary<int, Vertex>();
            this.vertices.Add(v0.ID, v0);
            this.AddEdge(this.start, v0, "");

            foreach (Graph graph in graphs)
            {
                // 复制各个点
                foreach (KeyValuePair<int, Vertex> pair in graph.vertices)
                {
                    if (pair.Value.ID != graph.start.ID)
                    {
                        Vertex v = new Vertex(pair.Value.ID, pair.Value.Text);
                        this.vertices.Add(v.ID, v);
                    }
                }
                // 复制各个边
                foreach (KeyValuePair<int, Vertex> pair in graph.vertices)
                {
                    if (pair.Value.ID == graph.start.ID)
                    {
                        foreach (Tuple<string, Vertex> tuple in pair.Value.NextEdges)
                        {
                            this.AddEdge(v0, this.vertices[tuple.Item2.ID], tuple.Item1);
                        }
                    }
                    else
                    {
                        foreach (Tuple<string, Vertex> tuple in pair.Value.NextEdges)
                        {
                            this.AddEdge(this.vertices[pair.Value.ID], this.vertices[tuple.Item2.ID], tuple.Item1);
                        }
                    }
                }
            }
        }

        /* 获取编号为id的节点
         */
        public Vertex GetVertex(int id)
        {

            return vertices.ContainsKey(id) ? vertices[id] : null;
        }

        /* 添加一个节点
         */
        public void AddVertex(Vertex v)
        {
            if (!this.vertices.ContainsKey(v.ID))
                this.vertices.Add(v.ID, v);
        }

        /* 添加一条边，v指向u
         */
        public void AddEdge(Vertex v, Vertex u, string txt = "")
        {
            if (v != null && u != null)
            {
                v.AddVertex(u, txt);
                this.AddVertex(v);
                this.AddVertex(u);
            }
        }

        /* 建图
         * 这里从文件读入一个图信息，实际情况下，修改此函数
         */
        public void Build(string fileName)
        {
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);

                string line;

                // 点
                while ((line = sr.ReadLine()).Length == 0) ;
                int n = int.Parse(line);
                for (int i = 1; i <= n; i++)
                {   //从1开始编号
                    while ((line = sr.ReadLine()).Length == 0) ;
                    this.AddVertex(new Vertex(i, line));
                }
                // 边
                while ((line = sr.ReadLine()).Length == 0) ;
                int m = int.Parse(line);
                for (int i = 1; i <= m; i++)
                {
                    while ((line = sr.ReadLine()).Length == 0) ;
                    string[] str = line.Split(' ');
                    int v = int.Parse(str[0]);
                    int u = int.Parse(str[1]);
                    this.AddEdge(this.GetVertex(v), this.GetVertex(u), str[2]);
                }

                //连起点
                this.AddEdge(this.GetVertex(0), this.GetVertex(1));

                sr.Close();
                fs.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace.ToString());
            }
        }

        //
        //////////////////////////////////////////////////////
    }
}
