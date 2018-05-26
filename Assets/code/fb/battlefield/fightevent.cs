using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using FB.FFSM;

namespace FB.BattleField
{
    /// <summary>
    /// 和播放特效有关系
    /// FE是FightEvent的意思
    /// </summary>
    public class FightEvent
    {
        public FightEvent(int a, int b)
        {
            AID = Math.Min(a, b);
            BID = Math.Max(a, b);
        }
        public int AID;
        public int BID;
        public Dictionary<int, FightEffectPiece> AEffect = new Dictionary<int, FightEffectPiece>();
        public Dictionary<int, FightEffectPiece> BEffect = new Dictionary<int, FightEffectPiece>();
        public bool bInited = false;//是否已初始化
        public bool bUsed = false;//是否已生效

        public bool AParry = false; //A格挡
        public bool BParry = false; //B格挡
        public int happy;
        public void Clear()
        {
            AEffect.Clear();
            BEffect.Clear();
            bInited = false;
            bUsed = false;
            happy = -1;
        }
        public void Use(IBattleField battleField)
        {
            if (bUsed || !bInited) return;
            //Debug.Log("Use FightEvent");
            bUsed = true;
            FightEffectPiece finalA = null;
            foreach (FightEffectPiece eff in AEffect.Values)
            {
                if (eff.type == PieceType.Hit)
                {
                    finalA = eff;
                }
            }
            FightEffectPiece finalB = null;
            foreach (FightEffectPiece eff in BEffect.Values)
            {
                if (eff.type == PieceType.Hit)
                {
                    finalB = eff;
                }
            }
            happy = -1;
            if (finalA != null && finalB != null)
            {//互杀
                var ccB = battleField.GetCharactorController(BID);
                var ccA = battleField.GetCharactorController(AID);

                if (!finalA.hurtfiend)
                    if (ccB.idSide == ccA.idSide) finalA = null;//同阵营不伤害
                if (!finalB.hurtfiend)
                    if (ccB.idSide == ccA.idSide) finalB = null;//同阵营不伤害

                if (finalA != null && finalB != null)
                {
                    //互杀
                    ccA.OnFight_Parry(AID, BID, finalB, (ccB as CharController_Direct).GetTransform().position.x);
                    ccB.OnFight_Parry(BID, AID, finalA, (ccA as CharController_Direct).GetTransform().position.x);
                    return;
                }
            }
             if (finalA != null)
            {
                happy = AID;
                //Debug.Log("A 打 B A=" + happy);
                var ccB = battleField.GetCharactorController(BID);
                var ccA = battleField.GetCharactorController(AID);
                if (!finalA.hurtfiend)
                    if (ccB.idSide == ccA.idSide) return;//同阵营不伤害
                 if(BParry)
                 {
                     ccB.OnFight_Parry(BID, AID, finalA, (ccA as CharController_Direct).GetTransform().position.x);
                 }
                 else
                 {
                     ccB.OnFight_BeHit(BID, AID, finalA, (ccA as CharController_Direct).GetTransform().position.x);
                     ccA.OnFight_Hit(AID, BID, finalA);
                 }


            }
            if (finalB != null)
            {
                happy = BID;
                //Debug.Log("B 打 A B=" + happy);
                var ccB = battleField.GetCharactorController(BID);
                var ccA = battleField.GetCharactorController(AID);

                if (!finalB.hurtfiend)
                    if (ccB.idSide == ccA.idSide) return;//同阵营不伤害

                if (AParry)
                {
                    ccA.OnFight_Parry(AID, BID, finalB, (ccB as CharController_Direct).GetTransform().position.x);
                }
                else
                {
                    ccA.OnFight_BeHit(AID, BID, finalB, (ccB as CharController_Direct).GetTransform().position.x);
                    ccB.OnFight_Hit(BID, AID, finalB);
                }
            }

        }
        /// <summary>
        /// 设置效果，未used就是叠加，return true。已used就判断是否重复冲突，重复就return false，不重复clear再加。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="effect"></param>
        /// <returns>是否生效</returns>
        bool SetEffect(int id, int effect)
        {
            return false;
        }


        public bool AddHit(int id, int hash, int hitcount, string prop, int add, BeHurtStateTable attackeffect, Vector3 firepos, bool hold,bool hurtfriend)
        {

            //int hashi = FightEffectPiece.CalcHash(PieceType.Hit, prop, add);
            var eff = (id == AID) ? AEffect : BEffect;

            if (eff.ContainsKey(hash)) //效果不能重复添加
            {
                return false;
            }
            if (bUsed)
            {
                //if (!eff.ContainsKey(hashi))//用过了，新的效果，clear重来。
                Clear();
            }

            eff[hash] = new FightEffectPiece(PieceType.Hit, hitcount, prop, add, attackeffect, firepos, hold,hurtfriend);
            //Debug.Log("AddHitEffect:" + hitcount + "," + prop + "," + add + "," + enemytostate);
            bInited = true;

            //use 才计算happy
            //happy = id;

            return false;
        }
        public bool AddParry(int id, int hash)
        {
            if(!AParry&&!BParry)
            {
                if (id == AID)
                    AParry = true;
                else
                    BParry = true;
                return true;
            }
            else
            {
                AParry = false;
                BParry = false;
            }
            return false;
        }
    }
    public enum PieceType
    {
        Hit,
        Hold,
        Defend,

    }
    public class FightEffectPiece
    {
        public FightEffectPiece(PieceType type, int hitcount, string prop, int add, BeHurtStateTable behurt, Vector3 firepos, bool hold,bool hurtfiend)
        {
            //Debug.LogError("prop = " + prop);
            this.type = type;
            this.hitcount = hitcount;
            this.prop = prop;
            this.add = add;
            this.beHurtState = behurt;
            this.firepos = firepos;
            this.hold = hold;
            this.hurtfiend = hurtfiend;
        }
        public PieceType type;
        public int hitcount;
        public string prop;
        public int add;
        public BeHurtStateTable beHurtState;
        public Vector3 firepos;//火花位置
        public bool hold;
        public bool hurtfiend;
    }
}
