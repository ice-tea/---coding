using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2CFG.GraphElement
{
    public class Vertex
    {
        //////////////////////////////////////////////////////
        // 成员变量
        /* 节点编号，唯一 */
        private int id;
        public int ID
        { get { return this.id; } }

        /* 节点上的文字信息 */
        public string Text
        { get; set; }

        /* 入度 */
        private int inDegree;
        public int InDegree
        { get { return this.inDegree; } }

        /* 出度 */
        public int OutDegree
        { get { return this.successors.Count; } }

        /* 后继结点列表(实际为后继边列表)
         * 每个元素都是二元组, 第一元是描述性文字, 第二元是后继结点
         * 例如, 一个分支语句, 它有两个后继结点, 指向这两个后继结点的边上的描述性文字可以分别是"真", "假"
         */
        private List<Tuple<string, Vertex>> successors;
        public List<Tuple<string, Vertex>> NextEdges
        { get { return this.successors; } }
        //
        //////////////////////////////////////////////////////

        //////////////////////////////////////////////////////
        // 成员函数

        /* 构造函数
         */
        public Vertex(int id, string txt = "")
        {
            this.id = id;
            this.Text = txt;
            this.inDegree = 0;
            this.successors = new List<Tuple<string, Vertex>>();
        }

        /* 添加后继节点：将节点加入后继节点，入度加1
         *   实质上为加边
         */
        public void AddVertex(Vertex v, string txt = "")
        {
            if (v == null) return;

            this.successors.Add(new Tuple<string, Vertex>(txt, v));
            v.inDegree++;
        }

        /*  删除边，将边从后继列表中删除，同时入度减1
         */
        public void RemoveEdge(Tuple<string, Vertex> edge)
        {
            edge.Item2.inDegree--;
            this.successors.Remove(edge);
        }

        /* 重载ToString函数
         */
        public override string ToString()
        {
            return this.ID + ". " + this.Text + "\n";
        }
        /* 重载GetHashCode函数，哈希用
         */
        public override int GetHashCode()
        {
            return this.ID; //因为ID是唯一的
        }
        //
        //////////////////////////////////////////////////////
    }
}
