using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using FB.FFSM;


namespace FB.BattleField
{


    public class LogicChar_Driver : FB.FFSM.ILogicCharDriver
    {
        BattleField battleField;
        public int id
        {
            get;
            private set;
        }
        public int idHold
        {
            get;
            private set;
        }
        public void Hold(int id)
        {
            this.idHold = id;
            battleField.Cmd_Char_Hold(this.id, this.idHold);

        }
        public void LetGo()
        {
            this.idHold = -1;
            battleField.Cmd_Char_Hold(this.id, this.idHold);
        }
        FB.FFSM.GraphChar_Driver ap = null;
        public LogicChar_Driver(IBattleField battleField, int id)
        {
            this.battleField = battleField as BattleField;
            this.id = id;
            this.idHold = -1;
            ap = (battleField as BattleField).GetRealChar(id);

            mapParam["hp"] = 100;
            mapParam["hpmax"] = 500;
            mapParam["mp"] = 5;
            mapParam["mpmax"] = 500;

        }

        public bool FE_AddHit(int hitenemycount,int hash, int hitcount, string prop, int add, BeHurtStateTable attackeffect, bool hold ,bool hurtfriend)
        {
            if (ap.attackmap.Count == 0)
                return false;

            //Debug.Log("FE_AddHit:" + hitcount + "," + prop + "," + add + "," + enemytostate);
            //得到冲突
            foreach (var dest in ap.attackmap)
            {
                if (hitenemycount == 0) break;
                hitenemycount--;
                if (battleField.GetCharactorController(dest.Key).Death) continue;
                var fe = battleField.GetFightEvent(this.id, dest.Key);
                if (fe.AddHit(this.id, hash, hitcount, prop, add, attackeffect, dest.Value, hold,hurtfriend))
                    return true;
            }
            return false;
            //throw new NotImplementedException();
        }
        //添加格挡
        public bool FE_AddParry(int hash)
        {
            foreach (var dest in ap.attackmap)
            {
                var fe = battleField.GetFightEvent(this.id, dest.Key);
                if (fe.AddParry(this.id, hash));
                return true;
            }
          return false;
        }
        public void FE_Clear()
        {
            var es = battleField.GetFightEventsAbout(this.id);
            foreach (var fe in es)
            {
                if (fe.Value.bUsed && (fe.Value.happy == this.id || fe.Value.happy < 0))
                    //if 需要判定我是受益方，才clear
                    //or both happy <0
                    //&& bUsed
                    fe.Value.Clear();
            }
            //throw new NotImplementedException();
        }

        Dictionary<string, int> mapParam = new Dictionary<string, int>();
        public void SetParam(string name, int value)
        {
            //Debug.LogError("name =" + name);
            //Debug.LogError("value =" + value);
            if (mapParam.ContainsKey(name + "max"))
            {
                int max = mapParam[name + "max"];
                if (value > max) value = max;
            }
            if (mapParam.ContainsKey(name + "min"))
            {
                int min = mapParam[name + "min"];
                if (value < min) value = min;
            }
            mapParam[name] = value;
        }

        public int GetParam(string name)
        {
            int v = 0;
            if (mapParam.TryGetValue(name, out v))
            {
                return v;
            }
            return 0;

        }

        public void CreateFlyItem(int hp, int life, string str,Vector3 vec,int dir)
        {

            Action ctask = () =>
            {
                var p = new MyJson.JsonNode_Object();
                p["pushbox"] = new MyJson.JsonNode_ValueNumber(true);
                int cc = battleField.Cmd_CreateChar(str, 1, p);
                var pos = ap.transform.position;
                battleField.Cmd_Char_Pos(cc,new Vector3(pos.x,pos.y+1,pos.z) );
                //注册控制器
                battleField.RegCharactorController(new CharController_Direct(cc, 1, null));

                var _cc = (battleField as BattleField).GetCharactorController(cc) as CharController_Direct;
                _cc.SetParam("hp", hp);
                _cc.SetCCLife(life);
                _cc.type = "flyitem";
/*                Debug.Log("给对方加力：");*/
                vec.x *= dir;
                _cc.fightFSM.AddForceSpeed(vec);
                _cc.SetForce(9.8f, 0f);              
                //其他
                battleField.GetRealChar(cc).transform.GetComponent<FB.FFSM.com_FightFSM>().debugMode = false;

                AddChild(cc);
            };
            
            battleField.queueTask(ctask);
        }

        public void AddChild(int index)
        {
            (battleField.GetCharactorController(id) as CharController_Direct).AddChildList(index);
        }
    }

}