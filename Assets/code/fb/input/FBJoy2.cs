using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 摇杆输入，
/// </summary>
public class FBJoy2 : MonoBehaviour
{
    public class JoyInput : IJoyInput
    {
        Command _curstate = new Command(Vector2.zero, PadButton.None);
        public Command curState
        {
            get
            {
                return _curstate;
            }
            set
            {
                _curstate = value;
                if (cmdlist.Count == 0)
                {
                    cmdlist.Add(_curstate);
                }
                else
                {
                    if (cmdlist[cmdlist.Count - 1].Equals(_curstate) == false)
                    {
                        cmdlist.Add(_curstate);
                    }
                }
                if (cmdlist.Count > 25)
                {
                    cmdlist.RemoveAt(0);
                }
            }
        }
        public List<Command> cmdlist = new List<Command>();
        public IList<Command> GetCommandList()
        {
            return cmdlist;
        }
        public int charDir = 1;
        public void SetCharDir(int dir)
        {
            charDir = dir;
        }
        public JoyMode left = JoyMode.Touch_Fix;
        public JoyMode right = JoyMode.Touch_Fix;
        public void SetTouchMode(JoyMode leftmode, JoyMode rightmode)
        {
            this.left = leftmode;
            this.right = rightmode;
        }


        public bool isMenuBtnDown
        {
            get;
            set;
        }


        public JoyMode touchModeLeft
        {
            get { return left; }
        }

        public JoyMode touchModeRight
        {
            get { return right; }
        }
        public void Update()
        {

        }
    }
    static IJoyInput __joy = null;
    public static IJoyInput g_joy
    {
        get
        {
            if (__joy == null)
                __joy = new JoyInput();

            return __joy;

        }
    }

    public class TouchInfo
    {
        public TouchInfo(float pad_inch, float btn_inch)
        {
            this._size_pad_inch = pad_inch;
            this._size_btn_inch = btn_inch;
        }
        public float _size_pad_inch
        {
            get;
            private set;
        }
        public float _size_btn_inch
        {
            get;
            private set;
        }


        Vector2? now;
        List<Vector2> touches = new List<Vector2>();
        public void TouchBeginTest()
        {
            now = null;
            touches.Clear();
        }
        public void TouchAdd(Vector2 pos)
        {
            touches.Add(pos);
        }
        float lastPresstime;
        Vector2 lastPressPos;
        public float timeDeltaLen = 0.33f;
        public void TouchEndTest(float dpi)
        {
            float timedelta = Time.realtimeSinceStartup - lastPresstime;
            if (touches.Count > 0)
            {
                lastPresstime = Time.realtimeSinceStartup;
                Vector2 _pos = Vector2.zero;
                foreach (var t in touches)
                {
                    _pos += t;
                }
                now = _pos / touches.Count;
                if (pressPos == null)
                {
                    //pressPos = now.Value;
                    //加入一个快操作补偿


                    if (timeDeltaLen > 0 && timedelta < timeDeltaLen)
                    {
                        pressPos = lastPressPos;// Vector2.Lerp(lastPressPos, now.Value, timedelta / timeDeltaLen);
                    }
                    else
                    {
                        pressPos = now.Value;
                    }
                }
                Vector2 Dir = now.Value - pressPos.Value;

                if (Dir.magnitude > dpi * _size_pad_inch * 0.5f)
                {
                    pressPos = now.Value - Dir.normalized * dpi * _size_pad_inch * 0.5f;

                }
            }
            else
            {
                now = null;
                if (pressPos != null)
                {

                    lastPressPos = pressPos.Value;
                    if (timedelta > timeDeltaLen)
                    {
                        pressPos = null;

                    }
                }
            }
        }

        Vector2? pressPos = null;
        public Vector2? GetPressPos()
        {
            return pressPos;
        }
        public Vector2? GetNowPos()
        {
            return now;
        }
        public Vector2? GetDir()
        {
            if (now == null || pressPos == null)
            {
                return null;
            }
            else
            {
                float dpi = Screen.dpi;
                //Debug.Log("dpi=" + dpi);
                if (dpi == 0) dpi = 96;
                Vector2 dir = now.Value - pressPos.Value;
                if (dir.magnitude < 0.125f * _size_pad_inch * dpi)
                {
                    return Vector2.zero;
                }
                else
                {
                    if (Mathf.Abs(dir.x) < 0.125f * _size_pad_inch * dpi)
                    {
                        dir.x = 0;
                    }
                    else
                    {
                        dir.x = Mathf.Sign(dir.x);
                    }
                    if (Mathf.Abs(dir.y) < 0.125f * _size_pad_inch * dpi)
                    {
                        dir.y = 0;
                    }
                    else
                    {
                        dir.y = Mathf.Sign(dir.y);
                    }
                    return dir.normalized;
                }
            }
        }
    }
    private TouchInfo LeftTouch;
    private TouchInfo RightTouch;
    // Use this for initialization
    void Start()
    {
        Start_TouchFix();
        LeftTouch = new TouchInfo(defSIZE, defSIZE * 0.25f);
        LeftTouch.timeDeltaLen = 5.0f;
        RightTouch = new TouchInfo(defSIZE, defSIZE * 0.25f);
        RightTouch.timeDeltaLen = 0;
    }


    Vector2 _dirState = Vector2.zero;
    void Update()
    {
        _dirState = Vector2.zero;
        PadButton _btnState = PadButton.None;
        JoyInput joy = g_joy as JoyInput;

        bool menu = false;

        if (!clientmain.canInput)
        {
            return;
        }
        //在这里键盘输入
        Update_Keyboard(ref _dirState, ref _btnState, ref menu);
        //在这里摇杆输入
        Update_Joy(ref _dirState, ref _btnState, ref menu);
        if (joy.left == JoyMode.Touch_Fix)
        {
            Update_TouchPadFix(ref _dirState, ref menu);
        }
        else if (joy.left == JoyMode.Touch_Dynamic)
        {
            Update_TouchPadDyn(ref _dirState, ref menu);
        }
        if (joy.right == JoyMode.Touch_Fix)
        {
            Update_TouchBtnFix(ref _btnState);
        }
        else if (joy.right == JoyMode.Touch_Dynamic)
        {
            Update_TouchBtnDyn(ref _btnState);
        }
        joy.isMenuBtnDown = menu;
        Vector2 _dir = _dirState;
        if (_dir.x < -0.5f) _dir.x = -1;
        else if (_dir.x > 0.5f) _dir.x = 1;
        else _dir.x = 0;

        if (_dir.y < -0.5f) _dir.y = -1;
        else if (_dir.y > 0.5f) _dir.y = 1;
        else _dir.y = 0;
        Command cmd = new Command(_dir, _btnState);
        joy.curState = cmd;
        //Debug.Log("cmd:" + _dirState + " _btn" + _btnState);

    }
    void Update_TouchPadDyn(ref Vector2 _dirState, ref bool menu)
    {
        float dpi = Screen.dpi;
        if (dpi == 0) dpi = this.defDPI;

        LeftTouch.TouchBeginTest();
        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if ((Screen.height - touch.position.y) <= dpi * 0.5f)
                {
                    //menu = true;
                }
                else if (touch.position.x < Screen.width / 2)
                {
                    LeftTouch.TouchAdd(touch.position);
                }
            }
        }
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if ((Screen.height - Input.mousePosition.y) < dpi * 0.5f)
            {
                //menu = true;
            }
            if (Input.mousePosition.x < Screen.width / 2)
            {
                LeftTouch.TouchAdd(Input.mousePosition);
            }
        }
#endif
        LeftTouch.TouchEndTest(dpi);
        Vector2? dir = LeftTouch.GetDir();
        if (dir != null)
        {
            _dirState += dir.Value;
        }
    }
    void Update_TouchBtnDyn(ref PadButton _btnState)
    {
        if (_btnState != PadButton.None && _btnState != PadButton.Func)
            return;

        float dpi = Screen.dpi;
        if (dpi == 0) dpi = this.defDPI;

        RightTouch.TouchBeginTest();
        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {

                if ((Screen.height - touch.position.y) <= dpi * 0.5f)
                {
                    //menu = true;
                }
                else if (touch.position.x >= Screen.width / 2)
                {
                    RightTouch.TouchAdd(touch.position);
                }
            }
        }
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {

            if ((Screen.height - Input.mousePosition.y) < dpi * 0.5f)
            {
                //menu = true;
            }
            else if (Input.mousePosition.x >= Screen.width / 2)
            {
                RightTouch.TouchAdd(Input.mousePosition);
            }

        }
#endif


        RightTouch.TouchEndTest(dpi);
        var dir = RightTouch.GetDir();
        if (dir != null)
        {
            _btnState = PadButton.Func;
            if (dir.Value.y < -0.5f)
                _btnState = PadButton.Func_Down;
            else if (dir.Value.y > 0.5f)
                _btnState = PadButton.Func_Up;
            int chardir = (g_joy as JoyInput).charDir;
            if ((chardir == -1 && dir.Value.x < -0.5f) || (chardir == 1 && dir.Value.x > 0.5f))
            {
                _btnState = PadButton.Func_Forward;
            }
        }

    }
    void Update_Keyboard(ref Vector2 _dirState, ref PadButton _btnState, ref bool isMenuDown)
    {
        if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Return))
        {
            isMenuDown = true;
        }
        //left
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) //left
        {
            _dirState.x -= 1;
            //btns[KeyCode.A].IsDown = true;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) //right
        {
            _dirState.x += 1;
            //btns[KeyCode.D].IsDown = true;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) //up
        {
            _dirState.y += 1;
            //btns[KeyCode.W].IsDown = true;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) //down
        {
            _dirState.y -= 1;
            //btns[KeyCode.S].IsDown = true;
        }

        if (_btnState != PadButton.None && _btnState != PadButton.Func)
            return;

        if (Input.GetKey(KeyCode.J)) // left
        {
            _btnState = PadButton.Func;
            //btns[KeyCode.J].IsDown = true;
        }
        if (Input.GetKey(KeyCode.K)) // down
        {
            _btnState = PadButton.Func_Down;
            //btns[KeyCode.K].IsDown = true;
        }
        if (Input.GetKey(KeyCode.I)) // up
        {
            _btnState = PadButton.Func_Up;
            //btns[KeyCode.I].IsDown = true;
        }
        if (Input.GetKey(KeyCode.L)) // right
        {
            _btnState = PadButton.Func_Forward;
            //btns[KeyCode.L].IsDown = true;
        }
    }
    void Update_Joy(ref Vector2 _dirState, ref PadButton _btnState, ref bool isMenuDown)
    {
        float x = Input.GetAxisRaw("X axis");
        float y = Input.GetAxisRaw("Y axis");// mark as Invert

        _dirState.x += x;
        _dirState.y += y;
        //if (x < -0.5f)
        //{
        //    _dirState.x -= 1;
        //}
        //else if (x > 0.5f)
        //{
        //    _dirState.x += 1;
        //}

        //if (y < -0.5f)
        //{
        //    _dirState.y -= 1;
        //}
        //else if (y > 0.5f)
        //{
        //    _dirState.y += 1;
        //}

        if (_btnState != PadButton.None && _btnState != PadButton.Func)
            return;

        //right
        if (Input.GetButton("joystick button 2")) // left
        {
            _btnState = PadButton.Func;
        }
        if (Input.GetButton("joystick button 0")) // down
        {
            _btnState = PadButton.Func_Down;
        }
        if (Input.GetButton("joystick button 3")) //up 
        {
            _btnState = PadButton.Func_Up;
        }
        if (Input.GetButton("joystick button 1")) // right
        {
            _btnState = PadButton.Func_Forward;
            Debug.Log("joystick button 1");
        }
        if (Input.GetButton("joystick button 6")) //up 
        {
            //_btnState = PadButton.Func_Up;
            isMenuDown = true;
            Debug.Log("joystick button 6");
        }
        if (Input.GetButton("joystick button 7")) // right
        {
            isMenuDown = true;
            //_btnState = PadButton.Func_Forward;
            Debug.Log("joystick button 7");
        }

    }

    // Use this for initialization
    Vector2 centerpos = new Vector2(0, 0);
    float joysize = 0;
    void Update_TouchPadFix(ref Vector2 _dirState, ref bool menu)
    {
        float dpi = Screen.dpi;
        if (dpi == 0) dpi = this.defDPI;

        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].phase == TouchPhase.Began || Input.touches[i].phase == TouchPhase.Moved
                || Input.touches[i].phase == TouchPhase.Stationary)
            {
                var pos = Input.touches[i].position;

                pos.y = Screen.height - pos.y;
                //if (pos.y < dpi * 0.5f)
                //{
                //    menu = true;
                //}
                if (Vector2.Distance(pos, centerpos) < joysize)
                {
                    Vector2 dir = (pos - centerpos);
                    dir.y *= -1;
                    float len = dir.magnitude / joysize * 4;
                    dir.Normalize();
                    dir *= len;

                    _dirState += dir;
                }
            }
        }

#if UNITY_EDITOR
        //需要的时候得模拟一下
        //if (Input.multiTouchEnabled == false)
        {
            Vector2 pos = Input.mousePosition;
            pos.y = Screen.height - pos.y;

            if (Input.GetMouseButton(0))
            {
                if (pos.y < dpi * 0.5f)
                {
                    //menu = true;
                }
                if (Vector2.Distance(pos, centerpos) < joysize)
                {
                    Vector2 dir = (pos - centerpos);
                    dir.y *= -1;
                    float len = dir.magnitude / joysize * 4;
                    dir.Normalize();
                    dir *= len;

                    _dirState += dir;
                }

            }
        }

#endif
    }

    void Start_TouchFix()
    {
        float dpi = Screen.dpi;
        if (dpi == 0) dpi = defDPI;

        float size = dpi * defSIZE;
        joysize = size;

        {
            BtnInfo i = new BtnInfo();
            i.dest = new Rect(-size * 4.5f / 3 + size / 3, -size / 3 * 2.5f, size / 3, size / 3);
            i.id = KeyCode.J;
            i.uv = CalcUI(joyback, 64 * 4, 0, 64, 64);
            i.uvdown = CalcUI(joyback, 64 * 4 + 64, 0, 64, 64);
            i.padstate = PadButton.Func;
            btnInfo.Add(i);
        }
        {
            BtnInfo i = new BtnInfo();
            i.dest = new Rect(-size * 4.5f / 3 + size / 3 * 2, -size / 3 * 2.5f + size / 3, size / 3, size / 3);
            i.id = KeyCode.K;
            i.uv = CalcUI(joyback, 64 * 4, 64, 64, 64);
            i.uvdown = CalcUI(joyback, 64 * 4 + 64, 64, 64, 64);
            i.padstate = PadButton.Func_Down;
            btnInfo.Add(i);
        }
        {
            BtnInfo i = new BtnInfo();
            i.dest = new Rect(-size * 4.5f / 3 + size / 3 * 2, -size / 3 * 2.5f - size / 3, size / 3, size / 3);
            i.id = KeyCode.I;
            i.uv = CalcUI(joyback, 64 * 6, 128, 64, 64);
            i.uvdown = CalcUI(joyback, 64 * 6 + 64, 128, 64, 64);
            i.padstate = PadButton.Func_Up;
            btnInfo.Add(i);
        }
        {
            BtnInfo i = new BtnInfo();
            i.dest = new Rect(-size * 4.5f / 3 + size / 3 * 3, -size / 3 * 2.5f, size / 3, size / 3);
            i.id = KeyCode.L;
            i.uv = CalcUI(joyback, 64 * 4, 128, 64, 64);
            i.uvdown = CalcUI(joyback, 64 * 4 + 64, 128, 64, 64);
            i.padstate = PadButton.Func_Forward;
            btnInfo.Add(i);
        }

    }

    // Update is called once per frame
    void Update_TouchBtnFix(ref PadButton _btnState)
    {

        if (_btnState != PadButton.None && _btnState != PadButton.Func)
            return;
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].phase == TouchPhase.Began || Input.touches[i].phase == TouchPhase.Moved
                || Input.touches[i].phase == TouchPhase.Stationary)
            {
                var pos = Input.touches[i].position;

                pos.y = Screen.height - pos.y;

                foreach (var bi in btnInfo)
                {
                    Rect r = bi.dest;
                    r.x += Screen.width;
                    r.y += Screen.height;
                    if (r.Contains(pos))
                        _btnState = bi.padstate;
                }
            }
        }
#if UNITY_EDITOR
        //需要的时候得模拟一下
        //if (Input.multiTouchEnabled == false)
        {
            Vector2 pos = Input.mousePosition;
            pos.y = Screen.height - pos.y;
            if (Input.GetMouseButton(0))
            {

                foreach (var bi in btnInfo)
                {
                    if (Input.GetMouseButton(0))
                    {
                        Rect r = bi.dest;
                        r.x += Screen.width;
                        r.y += Screen.height;
                        if (r.Contains(pos))
                            _btnState = bi.padstate;
                        //bi.bdown = bi.dest.Contains(pos);
                    }

                }
            }
        }
#endif
    }
    //NewBehaviourScript b;
    public Texture2D joyback;
    public int defDPI = 120;            //当检测不到屏幕dpi时使用的DPI
    public float defSIZE = 0.8f;        //摇杆大小、英寸
    public bool Pos2Key = true;         //是否将位置映射回按键
    Rect CalcUI(Texture2D tex, int x, int y, int width, int height)
    {
        Rect rect = new Rect();
        rect.x = (float)(x + 0.5f) / (float)tex.width;
        rect.y = 1.0f - (float)(y + height - 0.5f) / (float)tex.height;
        rect.width = (float)(width - 0.5f) / (float)tex.width;
        rect.height = (float)(height - 1.0f) / (float)tex.height;
        return rect;


    }
    void DrawRect(Vector2 pos, float size, int x, int y, int width, int height)
    {
        Rect src = CalcUI(joyback, x, y, width, height);
        GUI.DrawTextureWithTexCoords(new Rect(pos.x - size / 2, pos.y - size / 2, size, size), joyback, src);
    }

    public class BtnInfo
    {
        public KeyCode id;
        public Rect dest;
        public Rect uv;
        public Rect uvdown;
        public PadButton padstate;
    }
    public static List<BtnInfo> btnInfo = new List<BtnInfo>();
    void OnGUI()
    {



        JoyInput joy = g_joy as JoyInput;
        if (joy.left == JoyMode.Touch_Fix)
        {
            float dpi = Screen.dpi;
            if (dpi == 0) dpi = defDPI;

            float centrex = 0;
            float centrey = 0;
            float size = dpi * defSIZE;
            //            Debug.Log("size=" + dpi + "," + defSIZE);
            {
                float left = size / 3.0f / 2;
                float top = Screen.height - size / 3.0f / 2 - size;
                centrex = left + size * 0.5f;
                centrey = top + size * 0.5f;
                centerpos.x = centrex;
                centerpos.y = centrey;
            }
            bool wdown = false;
            bool sdown = false;
            bool adown = false;
            bool ddown = false;
            if (-_dirState.y < -0.5f) wdown = true;
            if (-_dirState.y > 0.5f) sdown = true;
            if (_dirState.x < -0.5f) adown = true;
            if (_dirState.x > 0.5f) ddown = true;
            //w
            DrawRect(new Vector2(centrex, centrey - size / 3), size / 3, wdown ? 64 : 0, 64 * 3, 64, 64);

            //s
            DrawRect(new Vector2(centrex, centrey + size / 3), size / 3, sdown ? 64 * 2 + 64 : 64 * 2, 64 * 3, 64, 64);

            //a
            DrawRect(new Vector2(centrex - size / 3, centrey), size / 3, adown ? 128 * 2 + 64 : 128 * 2, 64 * 3, 64, 64);

            //d
            DrawRect(new Vector2(centrex + size / 3, centrey), size / 3, ddown ? 192 * 2 + 64 : 192 * 2, 64 * 3, 64, 64);


            //point
            DrawRect(new Vector2(centrex + _dirState.x * size / 4, centrey - _dirState.y * size / 4), size / 3, 64 * 3, 64, 64, 64);

        }
        else if (joy.left == JoyMode.Touch_Dynamic)
        {
            var padpos = LeftTouch.GetPressPos();
            if (padpos != null)
            {

                float dpi = Screen.dpi;
                if (dpi == 0) dpi = defDPI;

                Vector2 center = padpos.Value;
                center.y = Screen.height - center.y;

                float size = LeftTouch._size_pad_inch * dpi;
                DrawRect(new Vector2(center.x, center.y), size, 0, 0, 64 * 3, 64 * 3);

                size = LeftTouch._size_btn_inch * dpi;
                var now = LeftTouch.GetNowPos();
                if (now != null)
                {
                    Vector2 posnow = now.Value;
                    posnow.y = Screen.height - posnow.y;
                    DrawRect(new Vector2(posnow.x, posnow.y), size, 64 * 3, 64, 64, 64);
                }

            }
        }
        if (joy.right == JoyMode.Touch_Fix)
        {
            foreach (var bi in btnInfo)
            {
                Rect r = bi.dest;
                r.x = Screen.width + r.x;
                r.y = Screen.height + r.y;
                if (joy.curState.state == bi.padstate)
                {
                    GUI.DrawTextureWithTexCoords(r, joyback, bi.uvdown);
                }
                else
                {
                    GUI.DrawTextureWithTexCoords(r, joyback, bi.uv);
                }
            }
        }
        else if (joy.right == JoyMode.Touch_Dynamic)
        {
            float dpi = Screen.dpi;
            if (dpi == 0) dpi = defDPI;
            float size = RightTouch._size_btn_inch * dpi;
            Vector2? pos = RightTouch.GetNowPos();
            if (pos != null)
            {
                var posnow = pos.Value;
                posnow.y = Screen.height - posnow.y;
                DrawRect(new Vector2(posnow.x, posnow.y), size, 64 * 3, 64, 64, 64);
            }
        }
    }

}