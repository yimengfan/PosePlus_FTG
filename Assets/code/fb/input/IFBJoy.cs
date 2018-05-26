using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//            Y
//      X            B
//            A
public enum PadButton
{
    None,
    Func,
    Func_Down,
    Func_Forward,
    Func_Up,
}
public enum JoyMode
{
    Touch_Dynamic,//浮动触摸 浮动摇杆+手势  只有这里用CharDir
    Touch_Fix,// 固定摇杆加固定按钮
    Touch_Disable,//关闭触摸与显示
}
public struct Command
{
    public Command(Vector2 dir, PadButton state)
    {
        this.dir = dir;
        this.state = state;
        this.time = Time.realtimeSinceStartup;
    }
    public Vector2 dir ;
    public PadButton state;
    public float time;
    public override bool Equals(object obj)
    {
        if (obj is Command)
        {
            Command other = (Command)obj;
            
            if (this.state != other.state) return false;

            int x = 0;
            if (this.dir.x < -0.5f) x = -1;
            else if (this.dir.x > 0.5f) x = 1;
            int ox = 0;
            if (other.dir.x < -0.5f) ox = -1;
            else if (other.dir.x > 0.5f) ox = 1;
            if (x != ox) return false;
            
            int y = 0;
            if (this.dir.y < -0.5f) y = -1;
            else if (this.dir.y > 0.5f) y = 1;
            int oy = 0;
            if (other.dir.y < -0.5f) oy = -1;
            else if (other.dir.y > 0.5f) oy = 1;
            if (y != oy) return false;

            return true;

        }
        else
        {
            return false;
        }
    }
    public override int GetHashCode()
    {
        return dir.GetHashCode() + state.GetHashCode();
    }
}

/// <summary>
/// 摇杆输入接口
/// </summary>
public interface IJoyInput
{
    Command curState
    {
        get;
    }
    //得到的list是唯一的，无论获取多少次都是一样的。
    //但是里面的内容会变化。
    IList<Command> GetCommandList();
    //输入

    bool isMenuBtnDown
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dir">+1 朝右， -1 朝左</param>
    void SetCharDir(int dir);

    void SetTouchMode(JoyMode leftmode, JoyMode rightmode);

    JoyMode touchModeLeft
    {
        get;
    }
    JoyMode touchModeRight
    {
        get;
    }
    void Update();

}

