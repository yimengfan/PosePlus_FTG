using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    class BlockParser_LetGo : IBlockParser//松开被抓住的敌人
    {

        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            fight.FE_LetGo();
            return false;
        }
    }
}
