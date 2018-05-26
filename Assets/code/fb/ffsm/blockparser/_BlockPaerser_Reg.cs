using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    public partial class FightFSM : IFightFSM
    {
        /// <summary>
        /// 1、人物移动
        /// 2、人物攻击
        /// 3、人物按键变招
        /// 4、任务条件变招
        /// 5、特效控制
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, IBlockParser> InitBlockParserMap()
        {
            if (mapBlockParser == null)
            {
                //全小写
                mapBlockParser = new Dictionary<string, IBlockParser>();
                //运动系
                mapBlockParser["turn"] = new BlockParser_Turn();//角色转身功能
                mapBlockParser["move"] = new BlockParser_Move();//角色位移
                mapBlockParser["force"] = new BlockParser_Force();//加力，跳跃用
                //攻击系
                mapBlockParser["breaklevel"] = new BlockParser_BreakLevel();//取消等级
                mapBlockParser["attack"] = new BlockParser_Attack();//攻击判定
                mapBlockParser["letgo"] = new BlockParser_LetGo();//攻击判定
                mapBlockParser["push"] = new BlockParser_Push();//攻击判定
                //防御系
                mapBlockParser["parry"] = new BlockParser_Parry();//格挡
                //跳转系
                mapBlockParser["inputexit"] = new BlockParser_InputExit();//按键变招
                mapBlockParser["repeatexit"] = new BlockParser_RepeatExit();//特殊，再次发动同一招数时变招
                mapBlockParser["propexit"] = new BlockParser_PropExit();//取消等级
                //条件系
                mapBlockParser["whenexit"] = new BlockParser_WhenExit();//条件变招
                mapBlockParser["whenpropexit"] = new BlockParser_WhenPropExit();//条件变招
                mapBlockParser["whenforceclear"] = new BlockParser_WhenForceClear();//取消力
                mapBlockParser["whensound"] = new BlockParser_WhenSound();//播放声音
                //效果系
                mapBlockParser["worldeff"] = new BlockParser_WorldEffect();
                mapBlockParser["selfeff"] = new BlockParser_SelfEffect();
                //召唤系
                mapBlockParser["flyitem"] = new BlockParser_FlyItem();
            }
            return mapBlockParser;
        }
    }
}