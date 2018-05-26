using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    class BlockParser_BreakLevel : IBlockParser//攻击
    {

        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            fight.SetBreakLevel(func.intParam0);
            return false;
        }
    }
}
