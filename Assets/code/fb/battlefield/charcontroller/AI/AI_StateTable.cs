using System;
using System.Collections.Generic;
using UnityEngine;
namespace FB.FFSM
{
    [Serializable]
    public class AI_StateItem
    {
        [SerializeField]
        public int statetype; // 1位移,2 攻击
        [SerializeField]
        public int attacktype; // 1 近, 2 中 ,3 远
        [SerializeField]
        public List<AI_StateAttribute> cmdAttribute = new List<AI_StateAttribute>();//技能指令分段，如果有多段 需要放完之后 再次释放第二段
    }
    [Serializable]
    public class AI_StateAttribute
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public string cmdStr;//指令
        [SerializeField]
        public Vector4 movenum;  //移动距离
        [SerializeField]
        public float attackpos = 0;//攻击距离
        [SerializeField]
        public List<AI_CanChangeState> canChangeState = new List<AI_CanChangeState>();
    }
    [Serializable]
    public class AI_CanChangeState
    {
        [SerializeField]
        public string state;
        [SerializeField]
        public string cmdstr;
    }
    public class AI_StateTable : ScriptableObject
    {
        [SerializeField]
        public string name;//该角色名
        [SerializeField]
        public List<AI_StateItem> allstates = new List<AI_StateItem>();  //出招分析表
    }
}