using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    class BlockParser_Push : IBlockParser//攻击
    {

        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            //推人
            fight.Push();
            return false;
        }
    }
}
