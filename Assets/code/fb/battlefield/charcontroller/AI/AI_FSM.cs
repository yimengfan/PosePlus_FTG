using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB.BattleField
{
    public interface IState
    {
        void OnInit(Input_AI_Expand mgr);
        void OnUpdate(float delta);
        void OnExit();
    }
    public class stateIdle : IState
    {

        Input_AI_Expand stateMgr;
        public void OnInit(Input_AI_Expand mgr)
        {
            stateMgr = mgr;
        }

        public void OnUpdate(float delta)
        {
            stateMgr.AIIdle();
        }

        public void OnExit()
        {

        }
    }
    public class stateSurround : IState
    {

        Input_AI_Expand stateMgr;
        public void OnInit(Input_AI_Expand mgr)
        {
            stateMgr = mgr;
        }

        public void OnUpdate(float delta)
        {
            stateMgr.AISurround();
        }

        public void OnExit()
        {

        }
    }
    public class stateAttack : IState
    {
        Input_AI_Expand stateMgr;
        public void OnInit(Input_AI_Expand mgr)
        {
            stateMgr = mgr;
        }

        public void OnUpdate(float delta)
        {
            stateMgr.AIAttack();
        }

        public void OnExit()
        {

        }
    }
    public class stateLeave : IState
    {

        Input_AI_Expand stateMgr;
        public void OnInit(Input_AI_Expand mgr)
        {
            stateMgr = mgr;
        }

        public void OnUpdate(float delta)
        {
            stateMgr.AILeave();
        }

        public void OnExit()
        {

        }
    }

}