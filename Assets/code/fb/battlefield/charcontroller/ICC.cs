using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace FB.BattleField
{
    /// <summary>
    /// 1、ScriptName: ICC
    /// 2、InterfaceName: ICharactorController
    /// 3、人物控制器接口
    /// </summary>
    public interface ICharactorController
    {
        int idCare//关心的ID，会收到这个ID相应的事件,如果id 是 0，会收到所有事件
        {
            get;
        }
        int idSide
        {
            get;
        }
        int attackDir  //自己的方向
        {
            get;
            set;
        }
        void OnInit(IBattleField battleField);//注册时被调用
        //角色事件
        void OnChar_Death(int id);

        void OnFight_Hit(int id, int to,FightEffectPiece piece);//冲突事件击打
        void OnFight_BeHit(int id, int from,FightEffectPiece piece,float attackdir);//冲突事件被击打

        void OnUpdate(float delta);
        void OnFight_Parry(int id, int from, FightEffectPiece piece, float attackdir);
        bool Death
        {
            get;
            set;
        }

    }
}
