using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Office.Interop.Visio;

namespace C2CFG.Visio
{
    /* 节点类型 */
    public enum MasterItem
    {
        _process = 0,    //过程
        _decision,       //分支
        _start_end       //起止点
    };

    public class VisioChart
    {
        //////////////////////////////////////////////////////
        // 成员变量
        /* Visio App类 */
        private Application m_App;

        /* 文档类 */
        private Document stencil;

        /* 定义几种图形的类型 */
        private Master Process;     //过程
        private Master Decision;    //分支
        private Master Start_End;   //起止点

        /* 图形对应方向值 */
        private readonly short[] DecisionDirection = { 0, 2, 1, 3 };
        private readonly short[] ProcessDirection = { 0, 3, 1, 2 };
        private readonly short[] StartEndDirection = { 2, 1, 3, 0 };
        //
        //////////////////////////////////////////////////////

        //////////////////////////////////////////////////////
        // 成员函数
        /* 构造函数 */
        public VisioChart()
        {
            m_App = null;
        }

        public Application GetApplicationClass()
        {
            return this.m_App;
        }

        /* 初始化app类 */
        public void InitApplicationClass()
        {
            this.m_App = new Application();
        }

        /* 打开Visio文件 */
        public bool Open(string sFile)
        {
            bool bRet = true;
            if (m_App == null) this.InitApplicationClass();

            try
            {
                m_App.Documents.Open(sFile);
                this.InitMasters();
            }
            catch (System.Exception e)
            {
                Console.Error.WriteLine("Open:" + sFile + " failed,reason:" + e.ToString());
                bRet = false;
            }

            return bRet;
        }

        /* 新建Visio文档 */
        public bool New()
        {
            bool bRet = true;
            if (m_App == null) this.InitApplicationClass();

            try
            {
                m_App.Documents.Add("");
                this.InitMasters();
            }
            catch (System.Exception e)
            {
                Console.Error.WriteLine("New failed,reason:" + e.ToString());
                bRet = false;
            }

            return bRet;
        }

        /* 初始化图形类型 */
        private void InitMasters()
        {
            if (this.m_App == null)
                return;

            // 停靠模具窗口“UML　活动”（用的是UML活动里的图形，因为画的是路线图，还要标出关键路径）
            short flags = (short)Microsoft.Office.Interop.Visio.VisOpenSaveArgs.visOpenDocked;
            stencil = this.m_App.Documents.OpenEx("BASFLO_M.VSS", flags); //是模具窗口

            this.Decision = stencil.Masters.get_ItemU(@"Decision");     //判断
            this.Process = stencil.Masters.get_ItemU(@"Process");       //过程
            this.Start_End = stencil.Masters.get_ItemU(@"Start/End");   //起止点
        }

        /* 画点 */
        public Shape DrowNode(MasterItem item, double x, double y, string txt = null)
        {
            Page page = this.m_App.ActivePage;
            Master master = null;
            switch (item)
            {
                case MasterItem._decision:
                    master = this.Decision;
                    break;
                case MasterItem._process:
                    master = this.Process;
                    break;
                case MasterItem._start_end:
                    master = this.Start_End;
                    break;
                default:
                    break;
            }
            if (master == null) return null;
            Shape shape = page.Drop(master, x, y);
            if (txt != null)
                shape.Text = txt;
            return shape;
        }

        /* 画线 */
        public void DrowEdge(Shape beginShape, Shape endShape, short beginDirection, short endDirection, string txt = "", Tuple<double, double> offset = null)
        {
            Page page = this.m_App.ActivePage;

            //画线        ->  Drop(线类型，此无关，此无关)
            Shape CnnShape = page.Drop(stencil.Application.ConnectorToolDataObject, 0, 0);
            if (txt != null)
                CnnShape.Text = txt;
            //起始节点
            Cell BeginXCell = CnnShape.get_CellsU("BeginX");
            BeginXCell.GlueTo(beginShape.get_CellsSRC(7, this.getDirection(beginShape, beginDirection), 0));
            //终止节点
            Cell EndXCell = CnnShape.get_CellsU("EndX");
            EndXCell.GlueTo(endShape.get_CellsSRC(7, this.getDirection(endShape, endDirection), 0));
            /* get_CellsSRC中的第一个参数和第三个参数不需要修改，
             * 我也暂时没搞清楚这两个参数的用处。。。
             * 第二个参数表示线具体与这个节点的那个位置相连
             *--------------------------------------------------------------*/

            CnnShape.get_Cells("EndArrow").Formula = "=5";

            if (offset != null)
            {
                CnnShape.get_CellsSRC(
                    (short)VisSectionIndices.visSectionObject,
                    (short)VisRowIndices.visRowShapeLayout,
                    (short)VisCellIndices.visSLOConFixedCode).FormulaForceU = "3";

                CnnShape.get_CellsSRC((short)VisSectionIndices.visSectionFirstComponent, 1, 0).FormulaForceU = "0";
                CnnShape.get_CellsSRC((short)VisSectionIndices.visSectionFirstComponent, 1, 1).FormulaForceU = "0";
                CnnShape.get_CellsSRC((short)VisSectionIndices.visSectionFirstComponent, 2, 0).FormulaForceU = "0";
                CnnShape.get_CellsSRC((short)VisSectionIndices.visSectionFirstComponent, 2, 1).FormulaForceU = offset.Item1 + "";
                CnnShape.get_CellsSRC((short)VisSectionIndices.visSectionFirstComponent, 3, 0).FormulaForceU = offset.Item2 + "";
                CnnShape.get_CellsSRC((short)VisSectionIndices.visSectionFirstComponent, 3, 1).FormulaForceU = offset.Item1 + "";

            }
        }

        /* 获取对应节点的方向 */
        private short getDirection(Shape shape, short d)
        {
            short direction = 0;
            string shapeName = shape.Name;
            if (shapeName.IndexOf(@"Decision") >= 0)
                direction = this.DecisionDirection[d];
            else if (shapeName.IndexOf(@"Process") >= 0)
                direction = this.ProcessDirection[d];
            else if (shapeName.IndexOf(@"Start/End") >= 0)
                direction = this.StartEndDirection[d];

            return direction;
        }

        //
        //////////////////////////////////////////////////////
    }
}
