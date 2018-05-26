using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    class BlockParser_SelfEffect : IBlockParser//自身效果
    {
        public enum SelfEffect
        {
            Flash,// 闪烁
        }
        public bool Update(IFightFSM fight, BlockFunc func, int frameindex)
        {

            fight.FlashBegin(new Color(func.vecParam0.x, func.vecParam0.y, func.vecParam0.z, func.vecParam0.z), func.intParam0, func.intParam1);
            ////FixInt("攻击可命中多少敌人", ref func.intParam0);
            ////FixInt("命中HitCount", ref func.intParam1);
            ////GUILayout.Space(10);
            ////FixString("攻击影响参数", ref func.strParam0);
            ////FixInt("攻击影响值", ref func.intParam2);
            ////GUILayout.Space(10);
            ////FixString("攻击导致对方状态", ref func.strParam1);

            ////if (func.activeFrameBegin < 0 || frameindex < func.activeFrameBegin) return false;
            ////if (func.activeFrameEnd >= 0 && frameindex > func.activeFrameEnd) return false;
            //fight.FE_AddHit(func.intParam0,func.intParam1,func.strParam0, func.intParam2, func.strParam1);
            return false;
        }
    }
}
