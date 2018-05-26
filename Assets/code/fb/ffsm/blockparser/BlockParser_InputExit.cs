using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace FB.FFSM
{

    //获取到输入则退出
    class BlockParser_InputExit : IBlockParser
    {
        //public bool Init(IFightFSM fight, BlockFunc func)
        //{
        //    //fight.Turn();
        //    return false;
        //}
        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            //限制功能激活时间
            //if(func.activeFrameBegin<0||frameindex<func.activeFrameBegin)return false;
            //if(func.activeFrameEnd>=0&&frameindex>func.activeFrameEnd)return false;
            if (func.intParam1 == 1 && !fight.IsHit())              //是否攻击中
            {
//                Debug.Log("false");
                return false;
            }
            if (fight.TestCmd(func.strParam0, func.vecParam0.x))
            {
                fight.ChangeBlock(func.strParam1, func.intParam0);
              /*  Debug.Log("WantChangeBlock:" + func.strParam1 + "(" + func.intParam0 + ")");*/
                return true;

            }
            return false;
        }
    }
}
