using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    class BlockParser_FlyItem:IBlockParser//受力，瞬发
    {
        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {

            fight.CreateFlyItem(func.intParam0, func.intParam1, func.strParam0,func.vecParam0);
            return false;
        }
    }
}
