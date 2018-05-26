using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    class BlockParser_WorldEffect : IBlockParser//攻击
    {

        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            //四个效果
            //int0 dark screen
            //int1 焦点（镜头放大跟踪）
            //int2 暂停世界（除了我）
            //int3 摇晃摄像机
            fight.WorldEffect(func.intParam0 > 0, func.intParam1 > 0, func.intParam2 > 0, func.intParam3 > 0);
            return false;
        }
    }
}
