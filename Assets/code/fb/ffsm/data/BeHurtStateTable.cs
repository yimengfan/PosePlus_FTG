using System;
using System.Collections.Generic;
using UnityEngine;
namespace FB.FFSM
{

    [Serializable]
    public class BeHurtStateTable
    {
        [SerializeField]
        public string name = "";
        [SerializeField]
        public List<ConditionEffect> effects = new List<ConditionEffect>();
    }
    [Serializable]
    public  class ConditionEffect
    {
        [SerializeField]
        public string onstate = "any"; //默认等于any  特殊处理
        [SerializeField]
        public bool facetome =true;
        [SerializeField]
        public string enemytostate = "";
        [SerializeField]
        public string hitfire = "";
        [SerializeField]
        public string hitsound = "";
        [SerializeField]
        public string parryfire = "";
        [SerializeField]
        public string parrysound = "";
        [SerializeField]
        public int mypauseframe;
        [SerializeField]
        public int enemypauseframe;
    }
}
