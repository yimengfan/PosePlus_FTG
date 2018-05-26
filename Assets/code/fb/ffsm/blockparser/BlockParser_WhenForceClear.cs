using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    class BlockParser_WhenForceClear:IBlockParser//受力，瞬发
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

            switch ((Conditon)func.intParam0)
            {
                case Conditon.Direct:
                    fight.SetForceClear();

                    break;
                case Conditon.Jump_Fall:
                    if (fight.Jump_IsFall())
                    {
                        fight.SetForceClear();
                    }
                    break;
                case Conditon.Jump_Floor:

                    if (fight.Jump_IsFloor())
                    {
                        fight.SetForceClear();
                       /* Debug.Log("setzero！");*/
                    }
                    break;
                default:
                    break;
            }          
            return false;
        }
    }
}
