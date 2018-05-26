using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;


namespace FB.FFSM
{

    [Serializable]
    public class StateItem //State
    {
        [SerializeField]
        public string name = "";
        //条件和行为两个部分构成
        [SerializeField]
        public List<StateCondition> conditions = new List<StateCondition>();
        //行为
        [SerializeField]
        public List<StateActionBlock> blocks = new List<StateActionBlock>();

        [SerializeField]
        public bool bCanDeath = true;
    }
    [Serializable]
    public class StateCondition
    {
        [SerializeField]
        public string stateBefore = "";//前置状态
        [SerializeField]
        public string cmdActive = "";//激活指令
        [SerializeField]
        public float cmdTime = 0.5f;//指令有效输入时间
        [SerializeField]
        public int breaklevel = 0;//中断等级
        [SerializeField]
        public bool isair = false;//释放地点 是否为空中
        [SerializeField]
        public string useattribute; //使用属性
        [SerializeField]
        public int usage_amount = 0;//使用量

    }


    //站 play 
    //走 block*3 1 播站到走动画，2播走的动画（跳出条件是输入打断） 3.播走到站动画
    //转 block 0time

    //这一坨不按照设计施工，实在是无法忍
    [Serializable]
    public class StateActionBlock//站、走、停、转
    {
        [SerializeField]
        public string playani = "";
        [SerializeField]
        public string playsubani = "";
        [SerializeField]
        public float playcross = 0.1f;
        [SerializeField]
        public int blocktime;//-1 无限时间 //0 不占时间的block //>0 此block延续固定帧数。
        [SerializeField]
        public List<BlockExit> exits = new List<BlockExit>();//出口
        //如果是 blocktime>=0,时间后自动走出口零，指令型出口包括其他出口均由func触发。
        [SerializeField]
        public List<BlockFunc> funcs = new List<BlockFunc>();//代表block的功能
    }
    [Serializable]
    public class BlockExit//block的出口连到另一个block
    {
        [SerializeField]
        public string statename = "";
        [SerializeField]
        public int blockindex = 0;
    }
    [Serializable]
    public class BlockFunc//块功能
    {
        [SerializeField]
        public bool haveexit = false;
        //当haveexit =true 时，strParam1 和 intParam0代表跳转出口
        [SerializeField]
        public string classname;//块类型名称
        [SerializeField]//Func的活动帧，仅有这个范围内Func起作用
        public int activeFrameBegin;
        [SerializeField]
        public int activeFrameEnd;
        [SerializeField]
        public string strParam0 ="";//string参数一个
        [SerializeField]
        public string strParam1 ="";//string参数
        [SerializeField]
        public string strParam2="";//string参数
        [SerializeField]
        public int intParam0;//int参数两个
        [SerializeField]
        public int intParam1;
        [SerializeField]
        public int intParam2;
        [SerializeField]
        public int intParam3;
        [SerializeField]
        public int intParam4;
        [SerializeField]
        public Vector4 vecParam0;//向量参数一个
    }


}
