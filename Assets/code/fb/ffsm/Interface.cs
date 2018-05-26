using FB.PosePlus;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{

    /// <summary>
    /// 1、状态机的接口
    /// 2、人物移动控制
    /// 3、人物状态改变
    /// 4、人物跳
    /// 5、增加攻击-FightEvent
    /// </summary>
    public interface IFightFSM
    {
        void ChangeBlock(string statename, int blockindex);

        void Turn();
        void MoveByJoy(float speed);
        void Move(float forwardAdd, float yAdd, float zAdd);
        bool TestCmd(string cmd, float cmdTime);

        void AddForceSpeed(Vector3 forceSpeed);
        void AddForceSpeedWithoutCharDir(Vector3 forceSpeed);
        void SetForceClear();

        bool Jump_IsFall();
        bool Jump_IsFloor();
        /// <summary>
        /// FrameEvent 增加伤害，如果这帧能有效return true;
        /// </summary>
        /// <param name="name">伤害名字</param>
        /// <param name="count">触发次数</param>
        /// <param name="hitcount">击打次数</param>
        /// <param name="prop">伤害影响的属性</param>
        /// <param name="add">属性增量</param>
        void FE_AddHit(int hitenemycount, int hitcount, string prop, int add, string attackeffect, bool hold,bool hurtfriend);
        void FE_LetGo();
        void SetBreakLevel(int level);

        void SetRepeatExit(string state, int blockindex, int delayframe);
        int GetProp(string name);
        void SetProp(string name, int value);
        bool IsHit();

        //int0 dark screen
        //int1 焦点（镜头放大跟踪）
        //int2 暂停世界（除了我）
        //int3 摇晃摄像机
        void WorldEffect(bool eff0, bool eff1, bool eff2, bool eff3);

        void PlaySound(string name);

        void CreateFlyItem(int hp,int life,string str,Vector3 vec);
        void FE_AddParry();
        void FlashBegin(Color color, int time,int speed);
        void FlashEnd();
        void Push();
    }
     
    /// <summary>
    /// 1、状态解析
    /// 2、此接口被很多类继承，各个类的方法不同，多态与继承
    /// </summary>
    public interface IBlockParser
    {
        /// <summary>
        /// 初始化block
        /// </summary>
        /// <param name="fight"></param>
        /// <param name="func"></param>
        /// <returns>如果返回true，则立即中断后续parser</returns>
        //bool Init(IFightFSM fight, BlockFunc func);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fight"></param>
        /// <param name="func"></param>
        /// <returns>如果返回true，则立即中断后续parser</returns>
        bool Update(IFightFSM fight, BlockFunc func, int frameindex);
    }

    //图形用的角色
    /// <summary>
    /// 1、移动
    /// 2、播放动画
    /// 3、设置力
    /// </summary>
    public interface IGraphChar
    {
        World world
        {
            get;
        }
        int chardir
        {
            get;
        }
        int frameid
        {
            get;
        }
        void SetDir(int dir);
        void Move(Vector3 add);

        //给一个速度和方向运动
        void SetMoveDir(Vector3 dir, float speed);
        void PlayAni(string clip, string subclip, float cross);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g">重力加速度，默认取9.8</param>
        /// <param name="a">空气阻力</param>
        void SetForce(float g, float a);//设置力
        void AddForceSpeed(Vector3 speed);//添加一个运动
        void SetForceSpeedZero();
        void UseForce(bool b);//是否使用力的开关，如果关掉，就不动了。

        bool isFloor
        {
            get;
        }
        Vector3 forceSpeed
        {
            get;
        }
        float fps
        {
            get;
        }

        int dir
        {
            get;
        }

        void FlashBegin(Color color, int time, int speed);
        void FlashEnd();
    }

    /// <summary>
    /// 1、设置参数
    /// 2、得到参数
    /// 3、增加攻击
    /// </summary>
    public interface ILogicCharDriver
    {
        int id
        {
            get;
        }

        int idHold //当前抓着的敌人
        {
            get;
            
        }
        void Hold(int id);
        void LetGo();
        void SetParam(string name, int value);
        int GetParam(string name);

        //CD内部维护一个命中列表

        //第三方维护一个冲突列表（list of FightEventFrame）
        //冲突列表为命中列表的双方
        //FightEventFrame;//战斗事件帧
        bool FE_AddHit(int hitenemycount,int hash, int hitcount, string prop, int add, BeHurtStateTable attackeffect, bool hold,bool hurtfriend);
        bool FE_AddParry(int hash);
        void FE_Clear();

        void CreateFlyItem(int hp, int life, string str,Vector3 vec,int dir);
        //bool AddToEventFrame(xxxx)
        //FFSM向CharDriver尝试添加事件，添加成功减一，添多少由FFSM自己决定。
    }
}
