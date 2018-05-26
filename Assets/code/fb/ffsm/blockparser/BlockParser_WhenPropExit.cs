using System;
using System.Collections.Generic;
using System.Text;

namespace FB.FFSM
{

    //条件退出
    public class BlockParser_WhenPropExit:IBlockParser
    {
        public enum WhenExit
        {
            NotEqual,//!=
            Equal,//=
            GreatEqual,//>=
            LessEqual,//<=
            Great,//>
            Less,//<
        }

        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            bool active = false;
            int v = fight.GetProp(func.strParam0);
            int tv = func.intParam1;
            WhenExit when = (WhenExit)func.intParam2;

            if (when == WhenExit.Equal && v == tv)
                active = true;
            else if (when == WhenExit.Great && v > tv)
                active = true;
            else if (when == WhenExit.GreatEqual && v >= tv)
                active = true;
            else if (when == WhenExit.Less && v < tv)
                active = true;
            else if (when == WhenExit.LessEqual && v <= tv)
                active = true;
            else if (when == WhenExit.NotEqual && v != tv)
                active = true; 
            if (active)
            {
                fight.ChangeBlock(func.strParam1, func.intParam0);
                return true;
            }
            return false;
        }
    }
}
