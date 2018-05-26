using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    class BlockParser_Attack : IBlockParser//攻击
    {

        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            //增加抓握，和跟踪点
            fight.FE_AddHit(func.intParam0, func.intParam1, func.strParam0, func.intParam2, func.strParam1, func.intParam3 > 0,func.intParam4>0);
            return false;
        }
    }
}
