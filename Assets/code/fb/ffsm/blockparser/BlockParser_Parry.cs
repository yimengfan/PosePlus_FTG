using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    class BlockParser_Parry:IBlockParser//受力，瞬发
    {
        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            fight.FE_AddParry();
            return false;
        }
    }
}
