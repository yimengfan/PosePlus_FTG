using System;
using System.Collections.Generic;

using System.Text;

namespace FB.FFSM
{

    //条件退出
    public class BlockParser_PropExit : IBlockParser
    {
        public enum PropOp
        {
            Set,//=
            Add,//+
            Eat,//if(>n)-=n;
        }

        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {
            if (func.intParam3 == 1 && !fight.IsHit())              //是否攻击中
            {
                //                Debug.Log("false");
                return false;
            }
            bool active = false;
            int v = fight.GetProp(func.strParam0);
            int tv = func.intParam1;
            PropOp op = (PropOp)func.intParam2;
            if (op == PropOp.Add)
            {
                v += tv;
                fight.SetProp(func.strParam0, v);
                active = true;
            }
            else if (op == PropOp.Set)
            {
                v = tv;
                fight.SetProp(func.strParam0, v);
                active = true;
            }
            else if (op == PropOp.Eat && v >= tv)
            {
                v -= tv;
                fight.SetProp(func.strParam0, v);
                active = true;
            }

            if (active && !string.IsNullOrEmpty(func.strParam1))
            {
                fight.ChangeBlock(func.strParam1, func.intParam0);
                return true;
            }
            return false;
        }
    }
}
