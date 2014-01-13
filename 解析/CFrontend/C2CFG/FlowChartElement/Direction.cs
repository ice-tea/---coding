/***********************************************************************
 * 类描述   ：方向枚举类
 *
 * 作者     : snowingsea
 * 完成时间 ： 2012年9月16日
 ***********************************************************************/

using System;

namespace C2CFG.FlowChartElement
{
    /* 方向 */
    public enum Direction
    {
        Left = 0,
        Up,
        Right,
        Down
    };

    /* 分支连线方向 */
    public enum DecisionDirection
    {
        Left = 0,
        Right,
        Up,
        Down
    }

    /* 过程连线方向 */
    public enum ProcessDirection
    {
        Left = 0,
        Right,
        Down,
        Up
    }

    /* 起止点连线方向 */
    public enum StartEndDirection
    {
        Down = 0,
        Up,
        Left,
        Right
    };
}
