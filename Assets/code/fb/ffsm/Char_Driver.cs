using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.FFSM
{
    public class World
    {
        public Vector3 zero;
        public Bounds bounds;
        public LayerMask layerForCharArea;
        public LayerMask layerForBeHurt;
        public FB.BattleField.IResourceMgr extplayer;

        public void ClearEff()
        {
            effect_dark = false;
            effect_id_pauseexcept = -1;
            effect_id_camerafocus = -1;
            effect_shock = false;
        }
        public bool effect_dark;
        public int effect_id_pauseexcept;//暂停例外
        public int effect_id_camerafocus;//摄像机焦点
        public bool effect_shock;
    }


    /// <summary>
    /// 1、碰撞检测
    /// 2、力，移动
    /// 3、播放动画
    /// 4、字符驱动，大都是靠id
    /// </summary>
    public class GraphChar_Driver : IGraphChar
    {

        bool pushBox;//增加这个变量区分推人与不推人的
        public GraphChar_Driver(int id, Transform trans, World world, bool inbattle)
        {
            this.id = id;
            this.transform = trans;
            this.aniplayer = trans.GetComponent<FB.PosePlus.AniPlayer>();
            this.aniplayer.SetExtPlayer(world.extplayer);
            if (inbattle)
            {
                this.aniplayer.SkipAutoUpdate();
            }
            aniplayer.tagid = id;
            //aniplayer.SetMoveProxy(this);
            this.world = world;
            this.aniplayer.wantpos = trans.position;
            fps = aniplayer.clips[0].fps;
        }
        public FFSM.World world
        {
            get;
            private set;
        }
        public float fps
        {
            get;
            private set;
        }
        public void Destory()
        {
            GameObject.Destroy(transform.gameObject);

        }
        public int id;
        public FB.PosePlus.AniPlayer aniplayer = null;

        public Transform transform
        {
            get;
            private set;
        }
        //Vector3 wantpos;
        //Vector3 lastpos;
        //bool lastHitWall = false;
        public void Move(Vector3 add)
        {
            this.aniplayer.wantpos += add;
            //this.transform.position = this.aniplayer.wantpos;
            speed = add.magnitude * fps;
            //Debug.Log("speed="+speed);
        }
        Vector3 moveDir = Vector3.zero;
        float moveSpeed = 0;
        public void SetMoveDir(Vector3 dir, float speed)
        {
            moveDir = dir;
            moveSpeed = speed;
        }

        public void SetPos(Vector3 pos)
        {
            transform.position = this.aniplayer.wantpos = pos;
        }
        public void SetDir(int dir)
        {
            if (aniplayer == null) return;
            aniplayer.SetDir(dir);
        }

        float speed = 10;
        bool bfloor = false;
        public void Push()
        {
            pushBox = true;
        }
        public void NotPush()
        {
            pushBox = false;
        }
        public void Update(float delta)
        {
            if (aniplayer == null) return;
            aniplayer._OnUpdate(delta);

            if (aniplayer.isPause) return;
            var frame = aniplayer.frameNow;
            //transform.position = Vector3.Lerp(lastpos, wantpos, 3f * delta);
            if (frame == null) return;

            var lastpos = transform.position;
            if (moveSpeed > 0 && followpos == null)
            {//指定方向运动
                transform.position = lastpos = this.aniplayer.wantpos += moveDir * moveSpeed * delta;
            }
            //碰墙与碰人
            attackmap.Clear();
            if (frame.boxesinfo != null)
            {
                foreach (var box in frame.boxesinfo)
                {

                    if (pushBox && box.mBoxType == "box_area")
                    {
                        TestWall(delta, box);
                    }
                    if (box.mBoxType == "box_attack")
                    {
                        TestAttackList(box);
                    }
                }
            }
            //if (attackList.Count > 0)
            //{
            //    Debug.Log("attackList.count=" + attackList.Count + ",[0]=" + attackList[0]);
            //}
            //Vector3 finalPos = lastpos;
            {//插值与移动
                Vector3 dir = (this.aniplayer.wantpos - lastpos);
                var distlen = dir.magnitude;
                float dist = speed * delta;
                //var pos = wantpos;
                if (distlen > dist)
                {
                    lastpos = lastpos + dir.normalized * dist;
                }
                else
                {
                    lastpos = this.aniplayer.wantpos;
                }
                //finalPos = lastpos;
            }

            {//力的合成
                if (speedForce.x > 0)
                {
                    speedForce.x -= delta * ForceA;
                    if (speedForce.x < 0)
                        speedForce.x = 0;
                }
                else if (speedForce.x < 0)
                {
                    speedForce.x += delta * ForceA;
                    if (speedForce.x > 0)
                        speedForce.x = 0;
                }
                if (speedForce.z > 0)
                {
                    speedForce.z -= delta * ForceA;
                    if (speedForce.z < 0)
                        speedForce.z = 0;
                }
                else if (speedForce.z < 0)
                {
                    speedForce.z += delta * ForceA;
                    if (speedForce.z > 0)
                        speedForce.z = 0;
                }
                //speedForce.y = 0;
                if (!bfloor)
                {
                    speedForce.y -= delta * ForceG * ForceA;
                    if (speedForce.y < -50) speedForce.y = -50;
                }
                else
                {

                    if (speedForce.y < 0)
                        speedForce.y = 0;
                }
                //Debug.Log("speedForce:" + speedForce + ",bfloor=" + bfloor);

                this.aniplayer.wantpos += speedForce * delta;
                lastpos += speedForce * delta;

            }



            if (followpos != null)
            {
                lastpos = followpos.Value;
            }
            {
                lastpos = world.bounds.ClosestPoint(lastpos);
                this.aniplayer.wantpos = world.bounds.ClosestPoint(this.aniplayer.wantpos);
                //超出边界检查

                //Debug.Log(this.aniplayer.wantpos);
                if (this.aniplayer.wantpos.y <= world.bounds.min.y)
                {
                    bfloor = true;
                }
                else
                {
                    bfloor = false;
                }

            }

            //    transform.position = followpos.Value;
            //}
            //else
            {
                transform.position = lastpos;
            }

            //Update Hold
            if (hold != null)
            {
                var holdp = frame.dotesinfo.Find((s) => s.name == "hold");
                if (holdp != null)
                {
                    holdAdd = holdp.position;
                }
                Vector3 _holdpos = transform.TransformPoint(holdAdd);

                hold.SetFollowPos(_holdpos);
            }
            //每帧结束 设回不能推人
            if(pushBox)
              NotPush();
        }

        public Dictionary<int, Vector3> attackmap = new Dictionary<int, Vector3>();
        private void TestAttackList(PosePlus.AniBoxCollider box)
        {
            //Debug.LogWarning("TestAttack");
            Vector3 boxpos = aniplayer.transform.TransformPoint(box.mPosition);
            Vector3 boxsize = aniplayer.transform.TransformVector(box.mSize);
            boxsize.x = Mathf.Abs(boxsize.x);
            boxsize.y = Mathf.Abs(boxsize.y);
            boxsize.z = Mathf.Abs(boxsize.z);
            Bounds aabb = new Bounds(boxpos, boxsize);
            var cc = Physics.OverlapSphere(boxpos, boxsize.magnitude / 2.0f, world.layerForBeHurt.value);
            foreach (var c in cc)
            {
                if (c.transform.parent.parent == aniplayer.transform) continue;
                if (aabb.Intersects(c.bounds))
                {
                    var pos = (aabb.center + c.bounds.center) / 2;


                    var ap = c.transform.parent.parent.GetComponent<FB.PosePlus.AniPlayer>();
                    if (attackmap.ContainsKey(ap.tagid) == false)
                    {
                        attackmap.Add(ap.tagid, pos);
                    }
                }
            }
        }
        private void TestWall(float delta, PosePlus.AniBoxCollider box)
        {

            Vector3 boxpos = transform.TransformPoint(box.mPosition);
            Vector3 boxsize = transform.TransformVector(box.mSize);
            boxsize.x = Mathf.Abs(boxsize.x);
            boxsize.y = Mathf.Abs(boxsize.y);
            boxsize.z = Mathf.Abs(boxsize.z);
            Vector3 pos = box.mPosition;
            {
                boxpos += (this.aniplayer.wantpos - transform.position);
                Bounds aabb = new Bounds(boxpos, boxsize);

                var cc = Physics.OverlapSphere(boxpos, boxsize.magnitude / 2.0f, world.layerForCharArea.value);
                bool bHit = false;
                float xadd = 0;
                foreach (var c in cc)
                {
                    if (c.transform.parent.parent == transform) continue;

                    if (aabb.Intersects(c.bounds))
                    {
                        float fdir = Mathf.Sign(boxpos.x - c.bounds.center.x);
                        var cani = c.transform.parent.parent.GetComponent<PosePlus.AniPlayer>();
                        if (hold != null && cani == hold.aniplayer) continue;
                        float fdist = (Mathf.Abs(boxpos.x - c.bounds.center.x) - boxsize.x / 2.0f - c.bounds.extents.x);

                        var cpos = c.transform.parent.parent.position;
                        cpos.x += fdir * fdist * 0.5f;
                        c.transform.parent.parent.position = cpos;

                        //if (cani.dir * fdir < 0)
                        //{
                        cani.wantpos = cpos;
                        //}

                        var lpos = transform.position;
                        lpos.x += -fdir * fdist * 0.5f;
                        transform.position = lpos;
                        //Vector3 dist = boxpos.x - c.bounds.center.x;
                        //Vector3 dist = boxpos - c.bounds.center;
                        //if (dist.sqrMagnitude < 0.01f)
                        //{
                        //    dist = new Vector3((UnityEngine.Random.value - 0.5f) * 0.4f, 0, (UnityEngine.Random.value - 0.5f) * 0.4f);
                        //}
                        //dir.x += dist.x;
                        //dir.y = 0;
                        //if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z)*0.5f)
                        //    dir.z = 0;
                        bHit = true;
                        //break;
                    }
                }
                if (bHit)
                {
                    //wantpos += dir.normalized * 6 * delta;
                }
            }


        }
        //static GameObject obj = null;
        //bool TestAABBWithWall(Vector3 center, Vector3 size)
        //{
        //    //if (obj == null)
        //    //{
        //    //    obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    //}
        //    //obj.transform.position = center;
        //    //obj.transform.localScale = size;
        //    Bounds aabb = new Bounds(center, size);
        //    var cc = Physics.OverlapSphere(center, size.magnitude / 2.0f, battleField.layerForFloor.value);
        //    foreach (var c in cc)
        //    {
        //        if (c.transform.parent.parent == transform) continue;
        //        if (aabb.Intersects(c.bounds)) return true;
        //    }
        //    return false;
        //}
        string curstate;
        public void PlayAni(string name, string subclip, float cross)
        {
            aniplayer.Play(name, subclip, cross);
            curstate = name;
        }

        Vector3 speedForce = Vector3.zero;
        public void AddForceSpeed(Vector3 speed)
        {
            speedForce += speed;
        }
        public void SetForceSpeedZero()
        {
            speedForce = Vector3.zero;
        }

        public int chardir
        {
            get { return aniplayer.chardir; }
        }

        public int frameid
        {
            get { return aniplayer.frameid; }
        }


        float forceG = 9.8f;
        float forceA = 3f;

        float ForceG
        {
            get
            {
                if (forceG < 9.8f)
                    forceG = 9.8f;
                return forceG;
            }
            set { forceG = value; }
        }
        float ForceA
        {
            get
            {
                return forceA;
            }
            set { forceA = value; }
        }
        public void SetForce(float g, float a)
        {
            ForceA = a;
            ForceG = g;

        }

        public void UseForce(bool b)
        {
            //throw new NotImplementedException();
        }


        public bool isFloor
        {
            get
            {
                return bfloor;
            }
        }

        public Vector3 forceSpeed
        {
            get
            {
                return speedForce;
            }
        }




        GraphChar_Driver hold = null;
        Vector3 holdAdd = Vector3.zero;
        public void SetHold(GraphChar_Driver hold)
        {
            if (this.hold != null) return;
            this.hold = hold;
            holdAdd = Vector3.zero;
        }

        public void CancelHold()
        {
            if (this.hold != null)
            {
                this.hold.EndFollow();
            }
            this.hold = null;
        }

        Vector3? followpos;

        void SetFollowPos(Vector3 pos)
        {
            if (transform == null) return;
            Vector3 add = Vector3.zero;
            var frame = aniplayer.frameNow;
            if (frame != null)
            {
                var holdp = frame.dotesinfo.Find((s) => s.name == "behold");
                if (holdp != null)
                {
                    add = transform.TransformVector(holdp.position);
                }
            }
            followpos = pos - add;
        }
        void EndFollow()
        {
            followpos = null;
        }

        public int dir
        {
            get
            {
                return aniplayer.dir;
            }
        }


        public void FlashBegin(Color color, int time, int speed)
        {
            aniplayer.SetFlash(color, time, speed);
        }

        public void FlashEnd()
        {
            aniplayer.SetFlash(Color.white, 0, 0);
        }
    }
    //角色动画播放器，接管了默认的动画播放器，对位移做了插值处理

}
