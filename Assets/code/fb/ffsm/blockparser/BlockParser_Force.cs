using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    class BlockParser_Force:IBlockParser//受力，瞬发
    {
        //public bool Init(IFightFSM fight, BlockFunc func)
        //{
        //    return false;
        //}
        public enum Conditon
        {
            Direct = 0,//直接跳出
            Jump_Fall,//跳跃之跌落，角色到达最高点后开始向下掉落时触发
            Jump_Floor,//跳跃之地板，角色碰到地面以后触发
        }
        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            //if (func.activeFrameBegin < 0 || frameindex < func.activeFrameBegin) return false;
            //if (func.activeFrameEnd >= 0 && frameindex > func.activeFrameEnd) return false;

            if (func.intParam0 == 1)
            {
                fight.SetForceClear();
            }

            if (func.intParam1 == 1)
            {
                var dir = FBJoy2.g_joy.curState.dir;
                fight.AddForceSpeedWithoutCharDir(new Vector3(func.vecParam0.x * dir.x, 0, func.vecParam0.z * dir.y));
            }
            else
            {
                fight.AddForceSpeed(func.vecParam0);
             /*   Debug.Log("Force:" + func.vecParam0);*/
            }
            return false;
        }
    }
}
