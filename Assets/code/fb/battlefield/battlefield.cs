using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FB.PosePlus;
using System;
namespace FB.BattleField
{
    public interface IBattleField
    {
        //注册与获取角色控制器
        void RegCharactorController(ICharactorController c);
        ICharactorController GetCharactorController(int id);
        IList<ICharactorController> GetAllCharactorController();
        IList<ICharactorController> GetCCNotDeathBySide(int side);
        IList<ICharactorController> GetAllCCBySide(int side);
        //角色驱动

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="type">角色类型</param>
        /// <param name="side">角色阵营</param>
        /// <returns></returns>
        int Cmd_CreateChar(string type, int side, MyJson.IJsonNode param);
        void Cmd_Char_Parent(int id, int parentID, string followpoint);
        void Cmd_Char_Pos(int id, Vector3 pos);
        void Cmd_Char_PlayAni(int id, string name, string subname, float cross);
        void Cmd_Char_Move(int id, Vector3 add);
        void Cmd_Char_SetMoveDir(int id, Vector3 dir, float speed);
        void Cmd_Char_SetDir(int id, int dir);
        void Cmd_Char_Block(int id, int frame);//定格
        void Cmd_Char_Death(int id);

        void Cmd_Char_AddForceSpeed(int id, Vector3 speed);
        void Cmd_Char_SetForceSpeedZero(int id);

        void Cmd_Char_Hold(int id, int holdid);

        void Cmd_Char_Flash(int id, Color color, int time, int speed);
    }

    //动画角色

    /// <summary>
    /// 1、命令，创建人物
    /// 2、命令，父物体
    /// 3、命令，位置点
    /// 4、命令，播放动画
    /// 5、命令，移动
    /// 6、命令，设置移动方向
    /// 7、命令，设置方向
    /// 8、命令，状态
    /// 9、命令，死亡
    /// </summary>
    public class BattleField : IBattleField
    {

        public FB.FFSM.World world = new FB.FFSM.World();
        public void InitBattleField(Transform scene, Bounds boundsFloor, LayerMask layerForCharArea, LayerMask layerForBeHurt, IDictionary<string, string> canUsedCharactors)
        {
            world.bounds = boundsFloor;
            world.layerForCharArea = layerForCharArea;
            world.layerForBeHurt = layerForBeHurt;
            foreach (Transform p in scene.Find("portal"))
            {
/*                p.gameObject.SetActive(false);*/

                if (p.name == "zero")
                {
                    world.zero = p.position;
                }
                else
                {
                    portals[p.name] = p.position;
                }
            }

            foreach (var c in canUsedCharactors)
            {
                this.canUsedCharactors[c.Key] = c.Value;
            }
        }
        //public Vector3 SceneZero;
        public Dictionary<string, string> canUsedCharactors = new Dictionary<string, string>();
        public Dictionary<string, Vector3> portals = new Dictionary<string, Vector3>();
        //public Bounds boundsFloor;
        public Dictionary<int, ICharactorController> cc = new Dictionary<int, ICharactorController>();

        public void RegCharactorController(ICharactorController c)
        {
            if (cc.ContainsKey(c.idCare) == false)
            {
                cc[c.idCare] = c;
                c.OnInit(this);
            }
            else
            {
                throw new Exception("CC 只能注册一次");
            }

        }

        public ICharactorController GetCharactorController(int id)
        {
            ICharactorController c = null;
            if (cc.TryGetValue(id, out c))
            {
                return c;
            }
            else
            {
                return null;
            }
        }

        public IList<ICharactorController> GetAllCharactorController()
        {
            List<ICharactorController> clist = new List<ICharactorController>();
            foreach (var c in cc.Values)
            {
                if (c != null)
                {
                    clist.Add(c);
                }
            }
            return clist;

        }
        public IList<ICharactorController> GetCCNotDeathBySide(int side)
        {
            List<ICharactorController> clist = new List<ICharactorController>();
            foreach (var c in cc.Values)
            {
                if (c != null && c.idSide == side&&!c.Death)
                {

                    clist.Add(c);

                }
            }
            return clist;
        }

        public IList<ICharactorController> GetAllCCBySide(int side)
        {
            List<ICharactorController> clist = new List<ICharactorController>();
            foreach (var c in cc.Values)
            {
                if (c != null && c.idSide == side)
                {

                    clist.Add(c);

                }
            }
            return clist;
        }

        public Dictionary<int, FB.FFSM.GraphChar_Driver> createChars = new Dictionary<int, FB.FFSM.GraphChar_Driver>();
        public FB.FFSM.GraphChar_Driver GetRealChar(int id)
        {
            FB.FFSM.GraphChar_Driver obj = null;
            if (createChars.TryGetValue(id, out obj))
            {
                return obj;
            }
            else
            {
                return null;
            }
        }
        int freeid = 0;
        GameObject roleroot;
        public int Cmd_CreateChar(string name, int side, MyJson.IJsonNode param)
        {
            //Debug.LogError("Cmd_CreateChar name= " + name);
            if (roleroot == null)
            {
                roleroot = GameObject.Find("role");
                if (roleroot == null)
                    roleroot = new GameObject("role");
            }

            GameObject inst = null;
            if (world.extplayer != null)
            {
                inst = (world.extplayer as IResourceMgr).CreateChar(name);
            }
            else
            {
                inst = GameObject.Instantiate(Resources.Load<GameObject>(name));
            }
            //if (reloadObjMap.ContainsKey(name))
            //{
            //    inst = GameObject.Instantiate(reloadObjMap[name]);
            //}
            inst.SetActive(true);
            inst.transform.SetParent(roleroot.transform, true);
            freeid++;
            createChars[freeid] = new FB.FFSM.GraphChar_Driver(freeid, inst.transform, world, true);
            return freeid;
        }

        public void Cmd_Char_Parent(int id, int parentID, string followpoint)
        {
            throw new System.NotImplementedException();
        }

        public void Cmd_Char_Pos(int id, Vector3 pos)
        {
            createChars[id].SetPos(pos);
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="subname"></param>
        /// <param name="cross"></param>
        public void Cmd_Char_PlayAni(int id, string name, string subname, float cross)
        {
            //Debug.LogError("Cmd_Char_PlayAni id= " + id);
            //Debug.LogError("Cmd_Char_PlayAni name= " + name);
            //Debug.LogError("Cmd_Char_PlayAni subname= " + subname);
            //Debug.LogError("Cmd_Char_PlayAni cross= " + cross);
            if (createChars.ContainsKey(id))
                createChars[id].PlayAni(name, subname, cross);
        }

        public void Cmd_Char_Move(int id, Vector3 add)
        {

            createChars[id].Move(add);


        }
        public void Cmd_Char_SetMoveDir(int id, Vector3 dir, float speed)
        {
            createChars[id].SetMoveDir(dir, speed);
        }
        public void Cmd_Char_SetDir(int id, int dir)
        {
            createChars[id].SetDir(dir);
        }

        public void Cmd_Char_Block(int id, int frame)
        {
            throw new System.NotImplementedException();

        }


        public void Cmd_Char_Death(int id)
        {
            createChars[id].Destory();
            createChars.Remove(id);
            
        }
       
        Dictionary<int, Dictionary<int, FightEvent>> mapFightEvent = new Dictionary<int, Dictionary<int, FightEvent>>();
        public FightEvent GetFightEvent(int id, int another)
        {
            if (mapFightEvent.ContainsKey(id) == false)
                mapFightEvent[id] = new Dictionary<int, FightEvent>();
            if (mapFightEvent.ContainsKey(another) == false)
                mapFightEvent[another] = new Dictionary<int, FightEvent>();

            if (mapFightEvent[id].ContainsKey(another))
            {
                return mapFightEvent[id][another];
            }
            else
            {
                FightEvent f = new FightEvent(id, another);
                mapFightEvent[id][another] = mapFightEvent[another][id] = f;
                return f;
            }

        }
        public Dictionary<int, FightEvent> GetFightEventsAbout(int id)
        {
            if (mapFightEvent.ContainsKey(id) == false)
                mapFightEvent[id] = new Dictionary<int, FightEvent>();
            return mapFightEvent[id];
        }
        /// <summary>
        /// battlefield脚本
        /// 三个遍历循环foreach
        /// 1、碰墙与碰人，就是碰撞--图形角色更新
        /// 2、修改人物动画状态--逻辑角色的更新
        /// 3、特效释放--打击事件的更新
        /// </summary>
        /// <param name="delta"></param>
        public void Update(float delta)
        {
            DoTask();
            //OnUpdate_PreLoad();


            world.ClearEff();

            //CC驱动
            foreach (var c in cc)
            {
                if (c.Value != null)
                {
                    c.Value.OnUpdate(delta);
                }
            }
            //图形驱动
            foreach (var c in createChars.Values)
            {
                c.Update(delta);
            }

            foreach (var map in mapFightEvent.Values)
            {
                foreach (var v in map.Values)
                {
                    v.Use(this);
                }
            }
        }



        public void Cmd_Char_AddForceSpeed(int id, Vector3 speed)
        {
            createChars[id].AddForceSpeed(speed);
        }
        public void Cmd_Char_SetForceSpeedZero(int id)
        {
            createChars[id].SetForceSpeedZero();
        }

        public void ClearALL()
        {
            foreach (var v in createChars.Values)
            {
                GameObject.Destroy(v.transform.gameObject);
            }
            createChars.Clear();
            cc.Clear();  //控制器清空
        }


        public void Cmd_Char_Hold(int id, int holdid)
        {
            if (holdid >= 0)
                createChars[id].SetHold(createChars[holdid]);
            else
                createChars[id].CancelHold();
        }

        Queue<Action> tasks = new Queue<Action>();
        public void queueTask(Action task)
        {
            tasks.Enqueue(task);
        }

        void DoTask()
        {
            if(tasks.Count>0)
            {
                tasks.Dequeue()();
            }
        }


        public void Cmd_Char_Flash(int id, Color color, int time, int speed)
        {
            if (time > 0 && speed > 0)
            {
                createChars[id].FlashBegin(color, time, speed);
            }
            else
            {
                createChars[id].FlashEnd();
            }
        }
    }
}