using FB.PosePlus;
using System;
using System.Collections.Generic;

using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    public class StateTable : ScriptableObject //StateList
    {
        [SerializeField]
        public List<StateItem> allStates = new List<StateItem>();
        [SerializeField]
        public List<BeHurtStateTable> allBehurtStates = new List<BeHurtStateTable>();
    }
}
