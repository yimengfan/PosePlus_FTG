using System;
using System.Collections.Generic;

using System.Text;

namespace FB.FFSM
{
    class BlockParser_Turn:IBlockParser
    {
        //public bool Init(IFightFSM fight, BlockFunc func)
        //{
        //    fight.Turn();
        //    return false;
        //}

        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            fight.Turn();
            return false;
        }
    }
}
