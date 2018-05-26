using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FB.PosePlus;
using FB.FFSM;

namespace FB.FFSM
{
    //仅作为一个编辑用来存变量和测试的组件，正式的BattleField使用此组件时DebugMode=false
    public class com_FightFSM : MonoBehaviour
    {
        public Transform hpBar;
        public StateTable stateTable;
        public TreeStateConfig stateTree;
        public AI_StateTable aiStateTable;
        public bool debugMode;
        public bool pushBox = false;//是否推动别的角色
        
        
        FightFSM cc = null;
        GraphChar_Driver cd = null;

        [Serializable]
        public class Param
        {
            [SerializeField]
            public string name;
            [SerializeField]
            public int value;
        }
        public List<Param> defaultParam = new List<Param>();
        int dir = 0;
        void Awake()
        {
            transform.GetComponent<AniPlayer>().IsShowBoxLine = false;
        }
        void Update()
        {
            if (debugMode || debugmodel2)
            {
                if (cc == null)
                {
                    World world = new World();
                    world.zero = Vector3.zero;
                    world.bounds = new Bounds(new Vector3(0, 2, 0), new Vector3(100, 4, 100));
                    cd = new GraphChar_Driver(0, transform, world,false);
                    //测试模式时直接调用AniPlayer
                    if (debugMode)
                    {
                        cc = new FightFSM(cd, null, stateTable,aiStateTable, stateTree, FBJoy2.g_joy);
                        cd.Update(Time.deltaTime);
                    }

                    else if (debugmodel2)
                    {
                        cc = new FightFSM(cd, null, stateTable,aiStateTable,stateTree, null);
                        cc.ChangeBlock("select", 0);

                    }

                }
                cc.Update();

                return;
            }

            if (Hp != lasthp)
                SetHPBar(Hp);
            timer += Time.deltaTime;
            if (timer >= 3 && hpBar != null)
            {
                //把alpha 改成0;隐藏           
                var color = hpBar.GetComponent<MeshRenderer>().material.color;
                if (color.a <= 0f) return;
                color.a -= Time.deltaTime;
                hpBar.GetComponent<MeshRenderer>().material.color = color;
            }
            if (dir != transform.GetComponent<AniPlayer>().dir)
            {
                dir = transform.GetComponent<AniPlayer>().dir;
                if (hpBar != null)
                {
                    if (dir == 1)
                        hpBar.rotation = Quaternion.Euler(-90, 180, 0);
                    else
                        hpBar.rotation = Quaternion.Euler(-90, -180, 0);
                }
            }

        }
        bool debugmodel2 = false;
        public void BeginDebugModel2()
        {
            debugmodel2 = true;
            if (cc != null && debugmodel2) cc.ChangeBlock("select", 0);
        }

        float lasthp;
        float timer = 0;

        float value;
        public float Hp
        {
            get{return value;}
            set
            {   
                this.value = value;
                if (this.value > HpMax)
                {
                    HpMax = this.value;
                }
            }
        }
        public float HpMax
        {
            set;
            get;
        }
        public void SetHPBar(float v)
        {
            if (hpBar == null)
                return;
            v = Hp / HpMax;
            timer = 0;
            //把alpha 改成1;显示
            var color = hpBar.GetComponent<MeshRenderer>().material.color;
            color.a = 1;
            hpBar.GetComponent<MeshRenderer>().material.color = color;

            if (v < 0.05) v = 0.05f;
            Color hp = Color.red;
            if (v > 0.5)
            {
                hp = Color.Lerp(Color.green, Color.yellow, (1.0f - v ) * 2);
            }
            else
            {
                hp = Color.Lerp(Color.yellow, Color.red, (0.5f - v ) / 0.45f);
            }
            if (Math.Abs(Hp - HpMax) <= 0.00001f)
                hpBar.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0f, 0);
            else
                hpBar.GetComponent<MeshRenderer>().material.mainTextureOffset = Vector2.Lerp(hpBar.GetComponent<MeshRenderer>().material.mainTextureOffset, new Vector2(1.0f - v , 0), 0.2f);
            var x = hpBar.GetComponent<MeshRenderer>().material.mainTextureOffset.x;
            if (Math.Abs(x - (1.0f - v )) <= 0.0001f)
            {
                lasthp = Hp;
            }
            float flash = Mathf.Abs(Time.timeSinceLevelLoad % 1.0f - 0.5f) * 2.0f;
            if (v < 0.15)
            {
                hp *= flash;
            }
            hpBar.GetComponent<MeshRenderer>().material.color = hp;



        }
    }
}