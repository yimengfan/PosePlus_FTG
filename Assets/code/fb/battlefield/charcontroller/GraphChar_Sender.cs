using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace FB.BattleField
{
    /// <summary>
    ///1、 GraphCharSender,是CC使用的向实际角色发消息控制的中转模块
    ///2、CC代表的是人物控制器
    /// </summary>
    public class GraphChar_Sender : FB.FFSM.IGraphChar
    {
        IBattleField battleField;
        int id;
        FB.FFSM.GraphChar_Driver ap = null;
        public GraphChar_Sender(IBattleField battleField, int id)
        {
            this.battleField = battleField;
            this.id = id;
            ap = (battleField as BattleField).GetRealChar(id);
        }
        public FFSM.World world
        {
            get { return (battleField as BattleField).world; }
        }
        public int chardir
        {
            get
            {
                return ap.chardir;
            }
        }

        public int frameid
        {
            get { return ap.frameid; }
        }

        public void SetDir(int dir)
        {
            battleField.Cmd_Char_SetDir(this.id, dir);
        }

        public void Move(Vector3 add)
        {

            battleField.Cmd_Char_Move(this.id, add);
        }
        public void SetMoveDir(Vector3 dir, float speed)
        {
            battleField.Cmd_Char_SetMoveDir(this.id, dir, speed);
        }
        public void PlayAni(string clip, string subclip, float cross)
        {
            battleField.Cmd_Char_PlayAni(this.id, clip, subclip, cross);
        }


        public void SetForce(float g, float a)
        {
            if (ap != null)
                ap.SetForce(g, a);
        }

        public void AddForceSpeed(Vector3 speed)
        {

            battleField.Cmd_Char_AddForceSpeed(id, speed);

        }
        public void SetForceSpeedZero()
        {

            battleField.Cmd_Char_SetForceSpeedZero(id);

        }

        public void UseForce(bool b)
        {
        }
        public Vector3 forceSpeed
        {
            get
            {
                var ap = (battleField as BattleField).GetRealChar(id);

                return ap.forceSpeed;
            }
        }
        public bool isFloor
        {
            get
            {
                var ap = (battleField as BattleField).GetRealChar(id);

                return ap.isFloor;
            }
        }


        public float fps
        {
            get { return ap.fps; }
        }

        public int dir
        {
            get { return ap.dir; }
        }
        
        public void FlashBegin(Color color, int time, int speed)
        {
            battleField.Cmd_Char_Flash(id, color, time, speed);
        }

        public void FlashEnd()
        {
            battleField.Cmd_Char_Flash(id,Color.white,0,0);
        }

        public void SetApPush()
        {
            if(ap!=null)
             ap.Push();
        }
    }

}
