using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using System.Collections.Generic;
using System.Text;

namespace FB.FFSM
{
    public enum WhenExit
    {
        Direct,//直接跳出
        Jump_Fall,//跳跃之跌落，角色到达最高点后开始向下掉落时触发
        Jump_Floor,//跳跃之地板，角色碰到地面以后触发
        Attack,
        NotAttack,
    }
    //条件退出
    class BlockParser_WhenSound:IBlockParser
    {
        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            WhenExit exit = (WhenExit)func.intParam0;
            bool active = false;
            if (exit == WhenExit.Direct)
            {
                active = true;
            }
            else if (exit == WhenExit.Jump_Fall && fight.Jump_IsFall())
            {
                active = true;
            }
            else if (exit == WhenExit.Jump_Floor && fight.Jump_IsFloor())
            {
                active = true;
            }
            else if (exit == WhenExit.Attack && fight.IsHit())
            {
                active = true;
            }
            else if (exit == WhenExit.NotAttack && !fight.IsHit())
            {
                active = true;
            }
            if (active)
            {
                fight.PlaySound(func.strParam0);
                return true;
            }
            return false;
        }
    }
}
