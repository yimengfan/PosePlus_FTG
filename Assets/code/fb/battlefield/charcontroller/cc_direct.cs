using FB.PosePlus;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace FB.BattleField
{
    /// <summary>
    /// 1、ScriptName:cc_direct
    /// 2、ClassName:CharController_Direct
    /// 3、人物控制器——导航
    /// 4、打，被打，切换人物状态、播放特效
    /// </summary>
    public class CharController_Direct : ICharactorController//, FB.FFSM.ILogicCharDriver
    {

        GraphChar_Sender graphchar;
        LogicChar_Driver logicchar;
        List<int> childList = new List<int>();
        public CharController_Direct(int id, int side, IJoyInput input)
        {
            this.idCare = id;
            idSide = side;
            bCareCharEvent = true;
            bCareFightEvent = true;
            this.joy = input;
        }

        public IJoyInput joy
        {
            get;
            private set;
        }
        public int idCare
        {
            get;
            private set;
        }

        public int idSide
        {
            get;
            private set;
        }
        public string type
        {
            get;
            set;
        }
        public bool bCareCharEvent
        {
            get;
            private set;

        }

        public bool bCareFightEvent
        {
            get;
            private set;

        }
        public int attackDir
        {
            get;
            set;
        }
        public bool Death
        {
            get;
            set;
        }
        BattleField battleField;
        public FFSM.FightFSM fightFSM;
        FB.PosePlus.AniPlayer aniplayer = null;
        public Input_AI_Expand.AILevel AiLevel { get; set; }
        public void OnInit(IBattleField battleField)
        {
            this.battleField = battleField as BattleField;
            FB.FFSM.GraphChar_Driver ap = (battleField as BattleField).GetRealChar(idCare);
            var cc = ap.transform.GetComponent<FB.FFSM.com_FightFSM>();
            logicchar = new LogicChar_Driver(battleField, idCare);
            foreach (var p in cc.defaultParam)
            {
                //Debug.LogError("p.name= " + p.name);
                //Debug.LogError("p.value= " + p.value);
                logicchar.SetParam(p.name, p.value);
            }
            graphchar = new GraphChar_Sender(battleField, idCare);
            //Charactor 脱离低级趣味
            aniplayer = ap.transform.GetComponent<FB.PosePlus.AniPlayer>();

            //fps = aniplayer.clips[0].fps;
            fightFSM = new FFSM.FightFSM(this.graphchar, this.logicchar, cc.stateTable, cc.aiStateTable, cc.stateTree, joy);

            if (joy is Input_AI)
            {
                (joy as Input_AI).Init(battleField, this, AiLevel);
            }

        }

        public int Dir
        {

            get { return aniplayer.dir; }
            set { aniplayer.dir = value; }
        }
        //public float fps
        //{
        //    get;
        //    private set;
        //}
        public void OnChar_Death(int id)
        {
        }

        int attackid = -1;
        public void OnFight_Hit(int id, int to, FightEffectPiece piece)
        {
          /*  Debug.Log(id + "打了" + to);*/

            attackid = to;
            if (piece.beHurtState != null)
                fightFSM.hitcount++;

            hitcount++;

            if (piece.hold)//抓技能
            {
                if (logicchar.idHold < 0)
                {
                    logicchar.Hold(to);
                }
            }
        }
        public int hitcount = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">被击者</param>
        /// <param name="from">攻击者</param>
        /// <param name="piece">攻击效果</param>
        /// <param name="_attackdir">攻击者方向</param>
        public void OnFight_BeHit(int id, int from, FightEffectPiece piece, float posx)
        {
            
            
            //UGUIFlyHp.Instance().init(UGUIFlyHp.FlyEffect.DecHp, "- 15", GetTransform().position, id);
           /* Debug.Log(id + "被" + from + "打了");*/
            Debug.Log("idhold:" + fightFSM.charLogic.idHold);
            //判断是否处于抓技
            if(fightFSM.charLogic.idHold != -1)
            {
                var tc=                battleField.GetCharactorController(fightFSM.charLogic.idHold) as CharController_Direct;
                if (tc != null)
                {
                    tc.fightFSM.ChangeBlock("stand", 0);

                }
                //中断抓技
                Debug.Log("中断抓技");
                fightFSM.FE_LetGo();

  
            }
            //攻击功能
         /*   Debug.Log("prop=" + piece.prop.ToLower() + "=" + piece.add);*/
            int v = logicchar.GetParam(piece.prop.ToLower());
            //BattleMainLogic test = new BattleMainLogic();
            ////这个id是英雄
            //if(test.GetCurHeroIndex==id
            v += piece.add;
            logicchar.SetParam(piece.prop.ToLower(), v);
            //攻击效果
            if (piece.beHurtState == null) return;


            bool left = (posx < GetTransform().position.x); //打我的人在我左边还是右边

            int firedir = left ? 1 : -1;
            int mydir = this.aniplayer.dir; 
            //面对  ，(mydir =-1 && leftorright==1)  ||  (mydir=1 &&  leftorright==-1);
            //or 背对
            bool bFaceToMe = (left && mydir == -1) || (!left && mydir == 1);
            foreach (var e in piece.beHurtState.effects)
            {
                if (e.onstate == "any" || e.onstate == fightFSM.GetCurStateName)//当前状态符合
                {

                    if (e.facetome==bFaceToMe)
                    {
                        //播放暂停
                        aniplayer.PlayPause(e.enemypauseframe);
                        //攻击者暂停
                        (battleField as BattleField).GetRealChar(from).transform.GetComponent<AniPlayer>().PlayPause(e.mypauseframe);
                        //改变状态
                        fightFSM.ChangeBlockDelay(e.enemytostate, 0);
                        string fire =  e.hitfire;
                        string sound =  e.hitsound;
                        //播放特效
                        if (!string.IsNullOrEmpty(fire) && fire != "")
                        {
                            if (battleField.world.extplayer != null)
                                battleField.world.extplayer.PlayEffect(fire, piece.firepos, firedir);
                        }
                        //播放音效
                        if (!string.IsNullOrEmpty(sound) && sound != "")
                        {
                            if (battleField.world.extplayer != null)
                                battleField.world.extplayer.PlaySoundOnce(sound);
                        }
                        break;
                     }
                } 
            }


        }


        public void OnFight_Parry(int id, int from, FightEffectPiece piece, float posx)
        {

            Debug.Log(id + "被" + from + "打了,但是触发了格挡");
            //攻击功能
            //攻击效果
            bool left = (posx < GetTransform().position.x); //打我的人在我左边还是右边
            int firedir = left ? -1 : 1;
            int mydir = this.aniplayer.dir;
            bool bFaceToMe = (left && mydir == -1) || (!left && mydir == 1);

            if (piece.beHurtState == null) return;
            foreach (var e in piece.beHurtState.effects)
            {
               
                string fire = e.parryfire;
                string sound = e.parrysound;

                if (e.facetome == bFaceToMe)
                {
                    //播放特效
                    if (!string.IsNullOrEmpty(fire) && fire != "")
                    {
                        if (battleField.world.extplayer != null)
                            battleField.world.extplayer.PlayEffect(fire, piece.firepos, firedir);
                    }
                    //播放音效
                    if (!string.IsNullOrEmpty(sound) && sound != "")
                    {
                        if (battleField.world.extplayer != null)
                            battleField.world.extplayer.PlaySoundOnce(sound);
                    }
                    break;
                }

            }

        }
        float timer = 0;
        int lastcount = 0;
        float intervaltime = 3;
        float deathTimer = 0;
        /// <summary>
        /// 1、人物控制器更新
        /// 2、里面控制这人物状态机的更新
        /// 3、摇杆更新
        /// </summary>
        /// <param name="delta"></param>
        public void OnUpdate(float delta)
        {

            if (fightFSM != null)
            {
                fightFSM.Update();
                if (Death)
                {
                    //怪物死亡之后，三秒钟销毁怪物
                    deathTimer += delta;
                    if (deathTimer >= 15f)
                    {
                        (battleField as BattleField).Cmd_Char_Death(idCare);
                        fightFSM = null;
                        aniplayer = null;
                        graphchar = null;
                        logicchar = null;
                    }
                    return;
                }
            }
            else
            {
                return;  //死亡了直接return;
            }

            if (iLife > -1)
            {
                iLife--;
                if (iLife == -1)
                {
                    Death = true;
                }
            }
            timer += Time.deltaTime;
            if (hitcount > lastcount)
            {
                lastcount = hitcount;
                timer = 0;
            }
            if (timer > intervaltime)
                hitcount = lastcount = 0;
            var ap = (battleField as BattleField).GetRealChar(idCare);
            if (ap == null) return;
            var cc = ap.transform.GetComponent<FB.FFSM.com_FightFSM>();
            cc.Hp = this.logicchar.GetParam("hp");
            //更新方向
            if (attackDir != aniplayer.dir) attackDir = aniplayer.dir;
            if (cc.Hp <= 0 )
            {
                if (fightFSM.isCanDeath)
                    Death = true;
                else
                    Debug.Log("该状态下不能死亡！");

            }
            if (joy != null)
            {
                joy.SetCharDir(aniplayer.chardir);
                joy.Update();
            }

        }

        #region 對外開放接口
        public int GetParam(string str)
        {
            if (logicchar != null)
                return logicchar.GetParam(str);
            return 1;


        }
        public void SetParam(string name, int value)
        {
            if (logicchar == null)
                return;
            //Debug.LogError("test is in here");
            logicchar.SetParam(name, value);
        }

        int iLife = -1;
        public void SetCCLife(int life)
        {
            iLife = life;
        }
        public void SetForce(float g, float a)
        {
            graphchar.SetForce(g, a);
        }
        #endregion
        public void AddChildList(int id)
        {
            if (childList.Contains(id) == false)
                childList.Add(id); ;
        }
        public int GetChildHitCount()
        {
            int count = 0;

            foreach (var i in childList)
            {
                var cc = (battleField.GetCharactorController(i) as CharController_Direct);
                if (cc.Death == false)
                    count += cc.hitcount;
            }
            count += hitcount;
            return count;
        }

        public Transform GetTransform()
        {
            if (aniplayer != null && aniplayer.transform!=null)
                return aniplayer.transform;
            else
                return null;
        }
        public int GetCurAttackID()
        {
            return attackid;
        }
    }


}
