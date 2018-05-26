using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using FB.FFSM;


namespace FB.BattleField
{
   public  enum StateMachine
    {
        Base = 0x0001,
        idle = Base << 1,          //待机
        surrounding = Base << 2,     //环伺
        attack = Base << 3,         //攻击
        leave = Base << 4           //离开
    }
  
    public class Input_AI_Expand
    {
        public enum AILevel
        {
            Soldiers,
            Boss,
            NewComer
        }

        CharController_Direct selfCC;
        CharController_Direct enemyCC;
        IBattleField battlefield;
        Input_AI input;
        AILevel aiLevel = AILevel.Soldiers;
        IState curState = null;
        float aiPatroltimer = 0;
        float attackProbability = 0.5f; //攻击几率

        //控制难度的数值
        float selectSkill = 0.9f;            //选技能合适度
        float ponderFrequency = 1f;          //思考频率
        float inputWrongProbability = 0.01f; //输入错误率
        float inutTimeInterval = 0.2f;       //输入时间
        int priority = 0;                    //技能优先等级
        int attackNumMax = 2;
        //初始数值
        Vector3 vIdlePos;
        AI_StateTable ai_table;
        StateTable  stateTable;
        public Input_AI_Expand(Input_AI input, IBattleField battlefield, CharController_Direct _self, FB.FFSM.AI_StateTable table ,AILevel aiLevel)
        {
            this.selfCC = _self;
            this.battlefield = battlefield;
            this.input = input;
            this.aiLevel = aiLevel;
            this.ai_table = table;
            //ai改变属性
            AILevelChange();

            vIdlePos = (battlefield as BattleField).GetRealChar(selfCC.idCare).transform.position;
          
            enemyCC = battlefield.GetAllCCBySide(1)[0] as CharController_Direct;
            //enemyCC.SetCCLife(0);
            stateTable = (battlefield as BattleField).GetRealChar(selfCC.idCare).transform.GetComponent<FB.FFSM.com_FightFSM>().stateTable;
            Start();
            ChangeState(StateMachine.idle);
           
            ai_attackstate_map = new Dictionary<int, List<AI_StateItem>>();
            for (int i = 0; i <= 3;i++ )
            {
              ai_attackstate_map[i] = new List<AI_StateItem>();//0 移动系列
            }
            if (ai_table == null || ai_table.allstates == null)
            {
                return;
            }
            foreach(var a in ai_table.allstates)
            {

                ai_attackstate_map[a.attacktype].Add(a);
            }
        }

        void AILevelChange()
        {
            switch (aiLevel)
            {
                case AILevel.Soldiers:
                    break;
                case AILevel.Boss:
                    break;
                case AILevel.NewComer:
                    {
                        selectSkill = 0.5f;           //选技能合适度
                        ponderFrequency = 10f;        //思考频率
                        inputWrongProbability = 0.5f; //输入错误率
                        inutTimeInterval = 0.2f;      //输入时间
                        priority = 0;                    //技能优先等级
                    }
                    break;
                default:
                    break;
            }
           


        }


        Dictionary<StateMachine, IState> mFSMMap = new Dictionary<StateMachine, IState>();
        public void Start()
        {
            mFSMMap[StateMachine.idle] = new stateIdle();
            mFSMMap[StateMachine.surrounding] = new stateSurround();
            mFSMMap[StateMachine.attack] = new stateAttack();
            mFSMMap[StateMachine.leave] = new stateLeave();
            foreach (var v in mFSMMap.Values)
            {
                v.OnInit(this);
            }
        }

        float ai_intervel_time = 0;
        int lastrand;
        System.Random rand = new System.Random();

        #region AIIdle
        float standtime = 0;
        public void AIIdle()
        {
            
        }
#endregion
        #region  AISurround
        float dir = 1;
        public void AISurround()
        {
            var ra = rand.Next(100);
            if(ra >80)

            if ((Mathf.Abs(mypos.x - enemypos.x) <= 1f))
            {
                if (mypos.x > enemypos.x)
                    dir = 1;
                else
                    dir = -1;
            }
            else if ((Mathf.Abs(mypos.x - enemypos.x) >= 2.5f))
            {
                if (mypos.x > enemypos.x)
                    dir = -1;
                else
                    dir = 1;
            }
            else
                return;
            //加一个空指令,中断上个指令    
            input.curState = (new Command(new Vector2(0,-1), PadButton.None));
            Command cmd = new Command();

            cmd = new Command(new Vector2(dir, 0), PadButton.None); //前                  
            input.curState = cmd;
        }
#endregion
        #region  AI Attack
       public void AIAttack()
       {
            //思考指令
           if (!isAttacking) //不处于某个技能释放阶段
           {
               if (Time.realtimeSinceStartup - setStateTimer >= ponderFrequency) //攻击指令间隔3s
               {  
                   //调整方向
                   if (mypos.x < enemypos.x)
                       selfCC.Dir = 1;
                   else
                       selfCC.Dir = -1;
                   var _ran = rand.Next(100);
                   if(_ran<30)  //位移几率30%
                   {
                       attacktype = 0;  //位移技能
                   }
                   else
                   {
                        //筛选用攻击类型
                        if ((Mathf.Abs(mypos.x - enemypos.x) < 1.5f))
                            attacktype = 1; //近技能
                        else if ((Mathf.Abs(mypos.x - enemypos.x) < 3f))
                            attacktype = 2; //中技能

                        else if ((Mathf.Abs(mypos.x - enemypos.x) < 5f))
                            attacktype = 3;  //远技能
                   }
                   var ra = rand.Next(100); //随机因子
                   if (attacktype != 0)
                   {
                       //是否筛选合适技能
                       if (ra > selectSkill * 100)
                       {
                           attacktype = 3 - attacktype + 1;
                           Debug.Log("选择不合适技能类型！");
                       }
                   }

                   if (ai_attackstate_map[attacktype].Count > 0)
                   {
                       var ran = rand.Next(ai_attackstate_map[attacktype].Count - 1); //随机筛选技能
                       attackItem = ai_attackstate_map[attacktype][ran];
                       isAttacking = true;
                   }
               }
               else
               {
                   AISurround();
               }

            }
            SimulateAttack();
        }

        bool isAttacking = false;
        int attacktype = 0;
        float setStateTimer = 0;
        Dictionary<int, List<AI_StateItem>> ai_attackstate_map;

        AI_StateItem attackItem;
        int cur_attack_stage = 0; //当前攻击阶段
        float enterkeytimer = 0;
       /// <summary>
       /// 模拟攻击
       /// </summary>
        void SimulateAttack()
        {
            if (!isAttacking) return;
            //调整方向
            if (mypos.x < enemypos.x)
                selfCC.Dir = 1;
            else
                selfCC.Dir = -1;
            //Z轴移动到
            if ((Mathf.Abs(mypos.z - enemypos.z) > 0.2f))
            {
                Vector2 dir = (mypos.z > enemypos.z) ? new Vector2(0, -1) : new Vector2(0, 1);
                input.curState = new Command(dir, PadButton.None);
                return;

            }         
          
            string cmdstr ="";
            if(cur_attack_stage >0)
            {
                //不变招 就直接终止
                if (!isBreakChangeState && !isFuncChangeState)
                {
                    isAttacking = false;
                    cur_attack_stage = 0;
                    setStateTimer = Time.realtimeSinceStartup;
                    return;
                }     
  
                //函数类变招
                if (isFuncChangeState)
                {     
                    if (IsFuncChangeStatge())
                    {
                        cmdstr = enterKey;
                        Debug.Log("变招：" + cmdstr);
                    }
                    if (string.IsNullOrEmpty(enterKey) || enterKey.Equals("") || enterKey.Equals("stand") || enterKey.Equals("5"))
                    {
                        isFuncChangeState = false;
                        return;
                    }
                   
                }
                //中断类变招
                else if (isBreakChangeState)
                {
                    var list = attackItem.cmdAttribute[0].canChangeState;
                    //函数类变招
                    if (list.Count > 0)
                    {
                        int _rand = rand.Next(list.Count - 1);
                        cmdstr = list[_rand].cmdstr;
                        Debug.Log("中断招：" + cmdstr);
                    }
                    isBreakChangeState = false;
                }
            }
            else
            {
                cmdstr = attackItem.cmdAttribute[cur_attack_stage].cmdStr;
            }

            SimulateCmd(cmdstr);

            //是否变招逻辑
            var r = rand.Next(100);
            if (r > 10) //是否变招
            {
                if (r < 45)
                {
                    isBreakChangeState = true;
                }                  
                else
                {
                    isFuncChangeState = true;
                }

            }
            else
            {
                isBreakChangeState = false;
                isFuncChangeState = false;
            }
            cur_attack_stage++;
        }

        void SimulateCmd(string cmdstr)
        {
            var ra = rand.Next(100);
            //模拟攻击
            for (int i = 0; i <= cmdstr.Length - 1; i++)
            {
                if (ra < inputWrongProbability * 100)
                {
                    Debug.Log("输入错误概率！");
                    return;
                }
                Command cmd = new Command();
                switch (cmdstr[i])
                {
                    case '2':
                        cmd.dir = new Vector2(0, -1);
                        break;
                    case '4':
                        cmd.dir = new Vector2(-1, 0);
                        break;
                    case '6':
                        cmd.dir = new Vector2(1, 0);
                        break;
                    case '8':
                        cmd.dir = new Vector2(0, 1);
                        break;
                    case '5':
                        cmd.dir = Vector2.zero;
                        break;
                    case 'J':
                        cmd.state = PadButton.Func;
                        break;
                    case 'I':
                        cmd.state = PadButton.Func_Up;
                        break;
                    case 'L':
                        cmd.state = PadButton.Func_Forward;
                        break;
                    case 'K':
                        cmd.state = PadButton.Func_Down;
                        break;
                    case 'R':
                        cmd.state = PadButton.None;
                        break;
                }
                cmd.time = Time.realtimeSinceStartup;
                input.cmdlist.Add(cmd);
            }
        }
        string lastStateName = "";
        int lasstStateIndex = -1;
        int lastFramePass = -1;
        int activeFrameBegin = -1;
        string enterKey;
        bool isFuncChangeState = false;//函数变招
        bool isBreakChangeState = false;//中断函数变招
        FB.FFSM.StateItem stateItem;
        FB.FFSM.StateActionBlock stateBlock;
        bool IsFuncChangeStatge()
        {
            //func 变招
            if (selfCC.fightFSM.GetCurStateIndex == lasstStateIndex && selfCC.fightFSM.GetCurStateName.Equals(lastStateName) && selfCC.fightFSM.GetCurFrame == lastFramePass) return false;
            //检查block 是否改变
            if (!selfCC.fightFSM.GetCurStateName.Equals(lastStateName))
            {
                lastStateName = selfCC.fightFSM.GetCurStateName;
                foreach (var s in stateTable.allStates)
                {
                    if (s.name.Equals(lastStateName))
                    {
                        stateItem = s;
                        stateBlock = s.blocks[selfCC.fightFSM.GetCurStateIndex];
                        break;
                    }
                }
                foreach (var f in stateBlock.funcs)
                {
                    if (f.classname.Equals("inputexit"))
                    {
                        activeFrameBegin = f.activeFrameBegin;
                        enterKey = f.strParam0;
                    }
                    else if (f.classname.Equals("repeatexit"))
                    {
                        activeFrameBegin = f.activeFrameBegin;
                        enterKey = stateItem.conditions[0].cmdActive;
                    }
                }
            }
            //检查blockIndex 是否改变
            if (selfCC.fightFSM.GetCurStateIndex != lasstStateIndex)
            {
                lasstStateIndex = selfCC.fightFSM.GetCurStateIndex;
                stateBlock = stateItem.blocks[lasstStateIndex];
                foreach (var f in stateBlock.funcs)
                {
                    if (f.classname.Equals("inputexit"))
                    {
                        activeFrameBegin = f.activeFrameBegin;
                        enterKey = f.strParam1;
                    }
                    else if (f.classname.Equals("repeatexit"))
                    {
                        activeFrameBegin = f.activeFrameBegin;
                        enterKey = stateItem.conditions[0].cmdActive;
                    }
                }
            }
            //检查frame 是否改变
            if (selfCC.fightFSM.GetCurFrame != lastFramePass)
                lastFramePass = selfCC.fightFSM.GetCurFrame;

            if(string.IsNullOrEmpty(enterKey) || enterKey.Equals(""))
                return false;

            if (lastFramePass >=activeFrameBegin)
                return true;

            return false;
        }

#endregion
        #region AILeave
        public void AILeave()
        {
            if (!(Mathf.Abs(mypos.x - vIdlePos.x) < 0.2f && (Mathf.Abs(mypos.z - vIdlePos.z) < 0.2f))) //是否移动到了
            {
                //先横向走，然后纵向走
                if (Mathf.Abs(mypos.x -vIdlePos.x) > 0.2f)
                {
                    Vector2 dir = (mypos.x > vIdlePos.x) ? new Vector2(-1, 0) : new Vector2(1, 0);
                    input.curState = new Command(dir, PadButton.None);
                }
                else if (Mathf.Abs(mypos.z - vIdlePos.z) > 0.2f)
                {
                    Vector2 dir = (mypos.z > vIdlePos.z) ? new Vector2(0, -1) : new Vector2(0, 1);
                    input.curState = new Command(dir, PadButton.None);
                }
            }
            else
            {
                //加一个空指令,中断上个指令
                input.curState = (new Command(Vector2.zero, PadButton.None));
            }
        }
#endregion
       

        float standtimer = 0;
        Vector3 mypos;
        Vector3 enemypos;
        public void Update()
        {
            if (selfCC.Death)
            {
                input.curState = new Command(Vector2.zero, PadButton.None);
                return;
            }
            if (selfCC.fightFSM.GetCurStateName.Equals("stand"))
            {
                standtimer += Time.deltaTime;
                if (standtimer <= 1f)
                    return;
            }
            //状态机update
            if(curState != null)
             curState.OnUpdate(Time.deltaTime);

            //一进入就为stand状态
            mypos = (battlefield as BattleField).GetRealChar(selfCC.idCare).transform.position;

            if ((battlefield as BattleField).GetRealChar(enemyCC.idCare) == null)
            {
                return;
            }

            enemypos = (battlefield as BattleField).GetRealChar(enemyCC.idCare).transform.position;

            //模式判定
            switch (aiLevel)
            {
                case AILevel.Soldiers:
                case AILevel.Boss:
                    if (Mathf.Abs(mypos.x - enemypos.x) < 6)
                    {
                        //检测多少人在攻击
                        if (CheckAttackNum() < attackNumMax)

                            ChangeState(StateMachine.attack);
                        else
                            ChangeState(StateMachine.surrounding);

                    }
                    else if (Mathf.Abs(mypos.x - enemypos.x) > 10) //离开区域范围
                    {
                        ChangeState(StateMachine.leave);
                    }
                    break;
                case AILevel.NewComer:
                    if (Mathf.Abs(mypos.x - enemypos.x) < 6)
                    {
                        //检测多少人在攻击
                        if (CheckAttackNum() < attackNumMax)

                            ChangeState(StateMachine.attack);
                        else
                            ChangeState(StateMachine.surrounding);

                    }
                    else if (Mathf.Abs(mypos.x - enemypos.x) > 10) //离开区域范围
                    {
                        ChangeState(StateMachine.leave);
                    }
                    break;
                default:
                    break;
            }
           
        }

        public  StateMachine laststate = StateMachine.Base;
        public  void ChangeState(StateMachine state)
        {
            if (laststate == state) return;
            laststate = state;

            if (curState != null)
                curState.OnExit();

            curState = mFSMMap[state];
        }

        //检测多少队友在攻击
        int CheckAttackNum()
        {
            var cc = battlefield.GetAllCCBySide(selfCC.idSide);//得到队友
            int num = 0;
            foreach (CharController_Direct c in cc)
            {
                if (c != null && c.joy is Input_AI)
                {
                    if ((c.joy as Input_AI).ai_expand.laststate == StateMachine.attack)
                        num++;
                }
            }
            return num;
        }
    }


}