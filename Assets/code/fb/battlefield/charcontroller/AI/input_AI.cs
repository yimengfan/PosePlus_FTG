using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace FB.BattleField
{

    public class Input_AI : IJoyInput
    {

        public void Init(IBattleField battlefield, CharController_Direct _self ,Input_AI_Expand.AILevel ailevel)
        {
            // curState = ;
            curState = new Command(Vector2.zero, PadButton.None);
            ai_expand = new Input_AI_Expand(this, battlefield, _self,_self.fightFSM.aiStateTable, ailevel);


        }
        Command _curstate = new Command(Vector2.zero, PadButton.None);
        public Command curState
        {
            get
            {
                return _curstate;
            }
            set
            {
                _curstate = value;
                if (cmdlist.Count == 0)
                {
                    cmdlist.Add(_curstate);
                }
                else
                {
                    if (cmdlist[cmdlist.Count - 1].Equals(_curstate) == false)
                    {
                        cmdlist.Add(_curstate);
                    }
                }
                if (cmdlist.Count > 25)
                {
                    cmdlist.RemoveAt(0);
                }
            }
        }
        public List<Command> cmdlist = new List<Command>();


        public Input_AI_Expand ai_expand = null;
        public IList<Command> GetCommandList()
        {
            return cmdlist;
        }

        public void SetCharDir(int dir)
        {

        }

        public void SetTouchMode(JoyMode leftmode, JoyMode rightmode)
        {

        }


        public bool isMenuBtnDown
        {
            get { return false; }
        }


        public JoyMode touchModeLeft
        {
            get { throw new NotImplementedException(); }
        }

        public JoyMode touchModeRight
        {
            get { throw new NotImplementedException(); }
        }

        public void Update()
        {

            if (ai_expand != null)
                ai_expand.Update();
        }
    }

    
}
