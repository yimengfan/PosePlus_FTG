using FB.PosePlus;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using FB.BattleField;
namespace FB.FFSM
{
    /// <summary>
    /// 1、遍历招式
    /// 2、改变状态
    /// 3、输入检测
    /// </summary>
    public partial class FightFSM : IFightFSM
    {
        public FightFSM(IGraphChar player, ILogicCharDriver charLogic, StateTable table, AI_StateTable aiStateTable, TreeStateConfig stateTree, IJoyInput joy)
        {


            this.aniPlayer = player;
            this.charLogic = charLogic;
            this.stateTable = table;
            this.aiStateTable = aiStateTable;

            InitBlockParserMap();
            //StateItem 存储指令
            mapState = new Dictionary<string, StateItem>();
            foreach (var s in stateTable.allStates)
            {
                mapState[s.name] = s;
            }

            this.joy = joy;
            this.fps = aniPlayer.fps;
            ChangeBlock("stand", 0);
            mapBackState = new Dictionary<string, string>();
            if (stateTree == null) return;
            foreach (var state in stateTree.states)
            {
                if (mapState.ContainsKey(state.name))
                {
                    mapBackState[state.name] = state.name;//自己就有
                    continue;
                }
                else
                {
                    var node = stateTree.GetNode(state.name);
                    while (node != null)
                    {
                        if (mapState.ContainsKey(node.name))
                            break;
                        node = node.parent;
                    }
                    if (node == null)
                    {
                        throw new Exception("无效状态:" + state.name);
                    }
                    mapBackState[state.name] = node.name;//找到上层
                }
            }
            //player.set
        }
        Dictionary<string, string> mapBackState;
        float fps = 24.0f;
        public StateTable stateTable;
        public AI_StateTable aiStateTable;
        private IJoyInput joy;
        private IGraphChar aniPlayer;
        public ILogicCharDriver charLogic;

        //保存curState
        Dictionary<string, StateItem> mapState;
        private string curStateName;//当前状态
        private string lastStateName;//上一次的状态
        private int curBlockIndex;
        private int blockFrameBegin = -1;
        private int blockFramePass = -1;
        private StateActionBlock _curblock;
        private int curFuncID;
        private int[] listFuncCounter = new int[20];
        static Dictionary<string, IBlockParser> mapBlockParser = null;
        public string GetCurStateName
        {
            get { return curStateName; }
        }
        public int GetCurStateIndex
        {
            get { return curBlockIndex; }
        }
        public int GetCurFrame
        {
            get { return blockFramePass; }
        }
        string delayStateName = null;
        int delayBlockIndex;
        public void ChangeBlockDelay(string statename, int blockindex)
        {
            delayStateName = statename;
            delayBlockIndex = blockindex;
        }
        int curStateIndex = -1;
        public void ChangeBlock(string statename, int blockindex)
        {
            if(statename.Equals("down_prone") ||statename.Equals("down_lie")||statename.Equals("down")  )
            {
                if (mapState[curStateName].bCanDeath == false)
                {
                    Debug.LogError("该状态不能死亡！");
                    return;
                }
            }
            if(!string.IsNullOrEmpty(curStateName))
            if (curStateName.Equals("down_prone") || curStateName.Equals("down_lie") || curStateName.Equals("down"))
            {
                Debug.LogError("死亡状态不能切换！");
                return;
            }

            hitcount = 0;
            if (this.mapState.ContainsKey(statename) == false)
            {
                statename = mapBackState[statename];
                //                 Debug.Log("BackState to:" + statename );
            }
            if (this.mapState.ContainsKey(statename) == false)
            {
                throw new Exception("statename:" + statename + " block " + blockindex + "not found.");
            }
            if (this.mapState[statename].blocks == null || this.mapState[statename].blocks.Count <= blockindex)
            {
                throw new Exception("statename:" + statename + " block " + blockindex + "not found.");
            }
            curStateName = statename;
            curBlockIndex = blockindex;
            blockFrameBegin = aniPlayer.frameid;

            _curblock = this.mapState[statename].blocks[blockindex];

            //IGraphChar  播放动画
            aniPlayer.PlayAni(_curblock.playani, _curblock.playsubani, _curblock.playcross);

            //foreach (var func in _curblock.funcs)
            //{
            //    if (mapBlockParser.ContainsKey(func.classname.ToLower()) == false)
            //    {
            //        throw new Exception("do not have blockparser:" + statename + "," + blockindex + "," + func.classname);
            //    }
            //    if (mapBlockParser[func.classname.ToLower()].Init(this, func))
            //        break;
            //}
            for (int i = 0; i < listFuncCounter.Length; i++)
            {
                listFuncCounter[i] = -1;
            }
            if (charLogic != null)
                charLogic.FE_Clear();
        }



        // Update is called once per frame
        int lastframeid = -1;

        /// <summary>
        /// 1、ScriptName: FightFSM
        /// 2、ClassName: FightFSM
        /// 3、状态机，人物动画状态改变
        /// </summary>

        public void Update()
        {
            if (aniPlayer.frameid == lastframeid) return;
            else lastframeid = aniPlayer.frameid;

            if (aniPlayer == null) return;

            if (delayStateName != null)
            {
                ChangeBlock(delayStateName, delayBlockIndex);
                delayStateName = null;
            }
            else
            {

                // 

                if (joy != null && joy.GetCommandList().Count > 0)
                {
                    if (lasttime != joy.GetCommandList()[joy.GetCommandList().Count - 1].time
                        || curStateName == "stand"
                        /*|| !(curStateName.Equals(lastStateName))*/) //当输入列表变了.或者状态改变
                    {
                        CheckInput(lasttime == joy.GetCommandList()[joy.GetCommandList().Count - 1].time);
                        lasttime = joy.GetCommandList()[joy.GetCommandList().Count - 1].time;  //记录下最近遍历时间
                        lastStateName = curStateName;

                    }
                }
                //if (curStateName.Contains("fly"))
                //{
                //    Debug.Log("turn");
                //}
                //奇怪的逻辑，block是状态机，总在某个block中
                blockFramePass = aniPlayer.frameid - blockFrameBegin;
                //Debug.Log("update framepass:" + aniPlayer.frameid + "," + framepass);
                if (_curblock.blocktime >= 0 && _curblock.blocktime <= blockFramePass)
                {//block 时间到
                    if (_curblock.exits == null || _curblock.exits.Count == 0)
                    {//没配置出口,接着往后走
                        ChangeBlock(curStateName, curBlockIndex + 1);
                    }
                    else
                    {//配置了出口走零号
                        ChangeBlock(_curblock.exits[0].statename, _curblock.exits[0].blockindex);
                    }
                }
            }
            {//parse BP
                string name = curStateName;
                //else//原来的else 可能漏掉第零帧
                blockFramePass = aniPlayer.frameid - blockFrameBegin;
                if (_curblock.blocktime < 0 || _curblock.blocktime > blockFramePass)
                {
                    this.byjoyspeed = 0;
                    this.breaklevel = 0;
                    //blow code may change breaklevel
                    //so checkinput 检测breaklevel即可

                    for (curFuncID = 0; curFuncID < _curblock.funcs.Count; curFuncID++)
                    {
                        var func = _curblock.funcs[curFuncID];

                        if (blockFramePass < func.activeFrameBegin) continue;
                        if (func.activeFrameEnd >= 0 && blockFramePass > func.activeFrameEnd) continue;
                        try
                        {
                            if (mapBlockParser[func.classname].Update(this, func, blockFramePass))
                                break;
                        }
                        catch (Exception err)
                        {
                            Debug.LogError("block:" + curStateName + "(" + curBlockIndex + ")func:" + func.classname + " error");
                        }
                    }

                    usebyjoyspeed();

                    //如过bp切换了block，那么下一帧才是开头
                    if (curStateName != name)
                    {
                        blockFrameBegin++;
                    }
                }

            }


        }

        #region  输入检测

        float lasttime = 0;
        float lasttimechange = 0;
        //检索技能
        void CheckInput(bool bRepeat)
        {
            foreach (var p in stateTable.allStates)     //遍历组合键
            {
                foreach (var c in p.conditions) //遍历一个招式 所有可能的指令
                {
                    if (c.cmdActive.Length > 0 && !char.IsNumber(c.cmdActive[c.cmdActive.Length - 1]) && bRepeat)
                    {//非方向结尾的指令不能在repeat时释放出
                        continue;
                    }
                    if (string.IsNullOrEmpty(c.stateBefore) || c.stateBefore == curStateName || c.stateBefore == "any")
                    {
                        if (breaklevel == -1) continue;
                        if (c.breaklevel >= breaklevel)//技能的中断等级
                            if ((c.isair && !Jump_IsFloor()) || (!c.isair && Jump_IsFloor())) //满足空中 地上条件
                            {
                                int dir = aniPlayer.chardir;
                                if (CmdMatch.CmdListMatch(c.cmdActive.ToUpper(), joy.GetCommandList(), dir, c.cmdTime))
                                {

                                    lastStateName = p.name;
                                    //breaklevel = c.breaklevel;//成功释放，改变当前中断等级
                                    if (!string.IsNullOrEmpty(c.useattribute))
                                    {
                                        var i = charLogic.GetParam(c.useattribute);
                                        if (i - c.usage_amount >= 0)
                                            charLogic.SetParam(c.useattribute, i - c.usage_amount);
                                        else
                                            return;
                                    }
                                    if (p.name == repeatInState && aniPlayer.frameid < repeatEndFrame)
                                    {
                                        if (repeatToState == null)
                                        {
                                            Debug.LogError("error in repeatInState:" + repeatInState + ":" + curBlockIndex);
                                        }
                                        ChangeBlock(repeatToState, repeatToIndex);
                                        repeatToState = null;
                                        repeatInState = null;
                                    }
                                    else
                                    {
                                        if (p.name == null)
                                        {
                                            Debug.LogError("error in change:" + c.cmdActive);
                                        }
                                        ChangeBlock(p.name, 0);
                                    }
                                    return;
                                }
                            }

                    }
                }
            }

        }


        #endregion

        #region FuncForBlock
        public void Turn()
        {
            aniPlayer.SetDir(aniPlayer.chardir * -1);
            this.lasttime = 0;//clear 操作
        }
        float byjoyspeed = 0;
        bool lastzero = true;
        Vector3 movedir;
        void usebyjoyspeed()
        {
            if (byjoyspeed > 0)
            {

                aniPlayer.SetMoveDir(movedir, byjoyspeed);
                lastzero = false;
            }
            else if (!lastzero)
            {
                aniPlayer.SetMoveDir(movedir, 0);
                lastzero = true;
            }

        }
        public void MoveByJoy(float speed)
        {
            byjoyspeed = speed;
            var dir = joy.curState.dir.normalized;
            movedir = new Vector3(dir.x, 0, dir.y);
            //var dir = joy.curState.dir.normalized;
            //dir *= speed / fps;
            //aniPlayer.Move(new Vector3(dir.x, 0, dir.y));
        }

        public void Move(float forwardAdd, float yAdd, float zAdd)
        {
            var dir = new Vector3(forwardAdd * aniPlayer.chardir, yAdd, zAdd);
            float speed = dir.magnitude;// *fps;//从帧速改成秒速
            movedir = dir.normalized;
            byjoyspeed = speed;
            //aniPlayer.Move(new Vector3(forwardAdd * aniPlayer.chardir, yAdd, zAdd));
        }
        public bool TestCmd(string cmd, float cmdTime)
        {

            if (lasttimechange == joy.GetCommandList()[joy.GetCommandList().Count - 1].time)


                return false;
            bool b = CmdMatch.CmdListMatch(cmd.ToUpper(), joy.GetCommandList(), aniPlayer.chardir, cmdTime);

            if (b)
                lasttimechange = joy.GetCommandList()[joy.GetCommandList().Count - 1].time;
            return b;
        }


        public void AddForceSpeed(Vector3 forceSpeed)
        {
            aniPlayer.AddForceSpeed(new Vector3(forceSpeed.x * aniPlayer.chardir, forceSpeed.y, forceSpeed.z));
        }
        public void AddForceSpeedWithoutCharDir(Vector3 forceSpeed)
        {
            aniPlayer.AddForceSpeed(new Vector3(forceSpeed.x, forceSpeed.y, forceSpeed.z));
        }
        public void SetForceClear()
        {
            aniPlayer.SetForceSpeedZero();
        }

        public bool Jump_IsFall()
        {
            if (this.aniPlayer.forceSpeed.y < 0 && !this.aniPlayer.isFloor)
                return true;
            return false;
        }

        public bool Jump_IsFloor()
        {
            return this.aniPlayer.isFloor;
        }

        public void FE_AddHit(int hitenemycount, int hitcount, string prop, int add, string attackeffect, bool hold ,bool hurtfriend)
        {
            if (charLogic == null) return;
            // Debug.Log("FE_AddHit:" + listFuncCounter[curFuncID]);

            string hithash = this.curStateName + "_" + this.curBlockIndex + "_" + this.curFuncID;
            int hash = hithash.GetHashCode();
            //aniPlayer.FE_AddHit

            //传入statetable
            BeHurtStateTable _attackeffect = stateTable.allBehurtStates.Find(s => s.name == attackeffect);
            bool b = charLogic.FE_AddHit(hitenemycount,hash, hitcount, prop, add, _attackeffect, hold,hurtfriend);
               
        }
        public void FE_AddParry()
        {
            if (charLogic == null) return;
            // Debug.Log("FE_AddHit:" + listFuncCounter[curFuncID]);
            string hithash = this.curStateName + "_" + this.curBlockIndex + "_" + this.curFuncID;
            int hash = hithash.GetHashCode();

            charLogic.FE_AddParry(hash);

        }
        public void FE_LetGo()
        {
            this.charLogic.LetGo();
        }
        public int GetProp(string name)
        {
            if (charLogic == null) return 0;
            return charLogic.GetParam(name);
        }

        public void SetProp(string name, int value)
        {
            if (charLogic == null) return;

            charLogic.SetParam(name, value);
        }


        public int hitcount = 0;
        public bool IsHit()
        {
            return hitcount > 0;
        }

        int breaklevel = 0;
        //bool bIsCanBeBreak = true;
        //float breaktimer = 0;
        public void SetBreakLevel(int level)
        {
            breaklevel = level;
        }

        string repeatInState;
        string repeatToState;
        int repeatToIndex;
        int repeatEndFrame;
        public void SetRepeatExit(string state, int blockindex, int delayframe)
        {
            repeatInState = this.curStateName;
            repeatToState = state;
            repeatToIndex = blockindex;
            repeatEndFrame = this.aniPlayer.frameid + delayframe;

        }
        public void WorldEffect(bool eff0, bool eff1, bool eff2, bool eff3)
        {
            //int0 dark screen
            //int1 焦点（镜头放大跟踪）
            //int2 暂停世界（除了我）
            //int3 摇晃摄像机
            this.aniPlayer.world.effect_dark = eff0;
            this.aniPlayer.world.effect_id_camerafocus = eff1 ? this.charLogic.id : -1;
            this.aniPlayer.world.effect_id_pauseexcept = eff2 ? this.charLogic.id : -1;
            this.aniPlayer.world.effect_shock = eff3;
        }

        public void PlaySound(string name)
        {
            AudioPlayer.Instance().PlaySoundOnce(name);
        }

        public void CreateFlyItem(int hp, int life, string str, Vector3 vec)
        {
            charLogic.CreateFlyItem(hp, life, str, vec, aniPlayer.dir);
        }

        public void Push()
        {
            (aniPlayer as GraphChar_Sender).SetApPush();
        }
        #endregion



        public void FlashBegin(Color color, int time, int speed)
        {
            aniPlayer.FlashBegin(color, time, speed);
        }

        public void FlashEnd()
        {
            aniPlayer.FlashEnd();
        }

        public bool isCanDeath
        {
            get { return mapState[curStateName].bCanDeath; }
        }
    }

}
