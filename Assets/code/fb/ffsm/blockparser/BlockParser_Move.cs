using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    class BlockParser_Move:IBlockParser
    {
        //public bool Init(IFightFSM fight, BlockFunc func)
        //{
        //    //fight.Turn();
        //    return false;
        //}

        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            ////限制功能激活时间,外部限制去
            //if(func.activeFrameBegin<0||frameindex<func.activeFrameBegin)return false;
            //if(func.activeFrameEnd>=0&&frameindex>func.activeFrameEnd)return false;
            if(func.strParam0=="byjoy")
            {
                fight.MoveByJoy(func.vecParam0.x);
            }
            else if(func.strParam0=="byvec")
            {

                fight.Move(func.vecParam0.x, func.vecParam0.y, func.vecParam0.z);
             /*   Debug.Log("move:" + func.vecParam0);*/
            }
            return false;
        }
    }
}
