using System;
using System.Collections.Generic;

using System.Text;
using UnityEditor;
using UnityEngine;
using FB.PosePlus;



class Window_StateBlockFuncs : EditorWindow
{
    static int selectFrame = -1;

    static FB.FFSM.BlockFunc selectFunc = null;
    static void SelectSelectFunc(FB.FFSM.BlockFunc func)
    {
        selectFunc = func;
        if (regfuncs == null || selectFunc == null) return;
        string classname = selectFunc.classname;
        if (!string.IsNullOrEmpty(classname))
            if (regfuncs.ContainsKey(classname))
            {
                var func2 = regfuncs[classname];
                if (func2 != null)
                    func2.Init();
            }
    }
    static SubClip _subclip = null;
    static AniClip _clip = null;
    //public static void Show(int curfunc = -1, int curframe = -1)
    //{
    //var window = EditorWindow.GetWindow<Window_StateBlockFuncs>(false, "Block详细编辑");

    //window.Init();

    //if (curframe >= 0)
    //selectFrame = curframe;
    //if(curfunc >=0)
    //selectFunc  = Window_StateTable.selectBlock.funcs[curfunc];

    //}
    static FB.PosePlus.AniPlayer _aniPlayer;
    public static void Show(FB.PosePlus.AniPlayer aniPlayer = null, AniClip clip = null, int curfunc = -1, int curframe = -1)
    {
        var window = EditorWindow.GetWindow<Window_StateBlockFuncs>(false, "Block详细编辑");

        window.Init();
        if (clip != null)
        {
            _clip = Window_StateTable.aniPlayer.GetClip(Window_StateTable.selectBlock.playani);
            _subclip = clip.GetSubClip(Window_StateTable.selectBlock.playsubani);
            Debug.Log(Window_StateTable.selectBlock.playani);
        }
        else
        {
            _clip = clip;
            _subclip = clip.GetSubClip(Window_StateTable.selectBlock.playsubani);
        }
        if (curframe >= 0)
            selectFrame = curframe;
        if (curfunc >= 0)
        {
            SelectSelectFunc(Window_StateTable.selectBlock.funcs[curfunc]);
        }
        else
        {
            SelectSelectFunc(null);
        }
        clip = Window_StateTable.aniPlayer.GetClip(Window_StateTable.selectBlock.playani);
        if (clip != null)
            _subclip = clip.GetSubClip(Window_StateTable.selectBlock.playsubani);
        if (curfunc >= 0 && curfunc <= Window_StateTable.selectBlock.funcs.Count - 1)
        {
            SelectSelectFunc(Window_StateTable.selectBlock.funcs[curfunc]);
        }
        if (aniPlayer != null)
        {
            _aniPlayer = aniPlayer;
        }


        pickfunc = null;
    }
    void Init()
    {
        FB.FFSM.StateActionBlock block = Window_StateTable.selectBlock;

        if (block.blocktime == 0)
        {
            //将blocktime设置为动画长度
            useClipCount(block, false);
        }
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        OnGUIHead();
        if (Window_StateTable.selectItem == null || Window_StateTable.selectBlock == null)
        {
            return;
        }
        OnGUITimeLine();
        Layout_DrawSeparator(Color.white);
        OnGUIFunction();
        GUILayout.EndVertical();
        //点击设置位置

        if (_subclip != null)
        {
            if (_subclip.loop || selectFrame < box.Count - 1)
                if (selectFrame > 0 && box.Count > 0)
                    Window_StateTable.aniPlayer.SetPose(Window_StateTable.aniPlayer.GetClip(Window_StateTable.selectBlock.playani), selectFrame % box.Count + (int)_subclip.startframe, true);
        }
        else
        {
            if (_clip.aniLoop || selectFrame < box.Count - 1)
                if (selectFrame > 0 && box.Count > 0)
                    Window_StateTable.aniPlayer.SetPose(Window_StateTable.aniPlayer.GetClip(Window_StateTable.selectBlock.playani), selectFrame % box.Count, true);

        }
        this.Repaint();

    }
    void OnGUIHead()
    {
        if (Window_StateTable.selectItem == null || Window_StateTable.selectBlock == null)
        {
            EditorGUILayout.HelpBox("必须先选中一个Block", MessageType.Warning);
            return;
        }
        GUILayout.Label("select Item:" + Window_StateTable.selectItem.name + "\\" + Window_StateTable.selectBlock.playani);
    }

    Vector2 postimeline;
    Vector2 postimeline2;
    int targetblocktime = 0;
    void SetAssetDirty()
    {
        EditorUtility.SetDirty(Window_StateTable.stateTable);
        SelectSelectFunc(null);
        selectFrame = -1;
    }
    void SetAssetDirtyFunc()
    {
        EditorUtility.SetDirty(Window_StateTable.stateTable);
    }

    //设置blocktime为动画长度
    void useClipCount(FB.FFSM.StateActionBlock block, bool isConFirm)
    {
        if (Window_StateTable.aniPlayer == null)
        {
            EditorUtility.DisplayDialog("warning", "打开姿势不对，这个姿势无法取得aniPlayer", "滚开");
            return;
        }
        var clip = Window_StateTable.aniPlayer.GetClip(block.playani);
        if (clip == null)
        {

            EditorUtility.DisplayDialog("warning", "hehe，动画就找不到,玩个蛋", "羞愧的滚开");
            return;
        }
        else
        {
            int length = clip.frames.Count;
            if (string.IsNullOrEmpty(block.playsubani) == false)
            {
                var subclip = clip.GetSubClip(block.playsubani);
                if (subclip == null)
                {
                    EditorUtility.DisplayDialog("warning", "hehe，子动画又找不到,玩个蛋", "羞愧的滚开X2");
                    return;
                }
                else
                {
                    length = (int)subclip.endframe - (int)subclip.startframe + 1;
                }
            }
            if (isConFirm)
            {
                if (EditorUtility.DisplayDialog("warning", "确认修改", "然", "再想想"))
                {
                    block.blocktime = length;
                    SetAssetDirty();
                    return;
                }
            }
            else
            {

                block.blocktime = length;
                SetAssetDirty();
                return;
            }
        }
    }

    void OnGUITimeLine()
    {
        var block = Window_StateTable.selectBlock;
        GUILayout.BeginHorizontal();
        GUILayout.Label("TimeLine");
        if (GUILayout.Button("Save"))
        {
            //如果没有设置动画 就会提示。如果设置的动画但是blocktime=0就会强制把blocktime+1
            if (block.playani == "")
            {
                if (EditorUtility.DisplayDialog("提示", "如果没有动画,则长度等于0", "容朕再改改", "知道了"))
                {
                    return;
                }
            }
            else if (block.blocktime == 0)
            {
                block.blocktime = 1;
                //AssetDatabase.SaveAssets();
            }
            AssetDatabase.SaveAssets();

        }
        GUILayout.EndHorizontal();
        #region 长度编辑区
        {//长度编辑区
            GUILayout.BeginHorizontal();
            GUILayout.Label("Block 长度修改", GUILayout.Width(100));
            if (GUILayout.Button("长度归零", GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("warning", "确认修改", "然", "再想想"))
                {
                    block.blocktime = 1;
                    SetAssetDirty();
                    return;
                }
            }
            if (GUILayout.Button("无限长度", GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("warning", "确认修改", "然", "再想想"))
                {
                    block.blocktime = -1;
                    SetAssetDirty();
                    return;
                }
            }
            if (GUILayout.Button("使用动画长度", GUILayout.Width(150)))
            {

                useClipCount(block, true);

            }
            GUILayout.Space(100);
            targetblocktime = EditorGUILayout.IntField(targetblocktime, GUILayout.Width(50));
            if (GUILayout.Button("指定长度", GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("warning", "确认修改", "然", "再想想"))
                {
                    block.blocktime = targetblocktime;
                    SetAssetDirty();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

        }
        #endregion
        Layout_DrawSeparator(Color.white);
        #region timeline
        postimeline = GUILayout.BeginScrollView(postimeline, false, true, GUILayout.MinHeight(250));
        GUILayout.BeginHorizontal();
        {
            int framecount = block.blocktime;

            GUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            GUILayout.Label("TimeLine 长度:" + (framecount < 0 ? "无限" : framecount.ToString()), GUILayout.Height(18));
            if (GUILayout.Button("Add Function", GUILayout.Width(86)))
            {
                block.funcs.Add(new FB.FFSM.BlockFunc());
                SetAssetDirty();
                return;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("帧数:");
            GUILayout.Space(2);
            GUILayout.Label("box_attack:");
            GUILayout.Space(2);
            GUILayout.Label("box_area:");
            GUILayout.Space(2);
            GUILayout.Label("box_behurt:");
            GUILayout.Space(8);
            for (int i = 0; i < block.funcs.Count; i++)
            {
                var func = block.funcs[i];
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(func.classname + "(" + func.activeFrameBegin + "-" + func.activeFrameEnd + ")"))
                {
                    SelectSelectFunc(func);
                    bEditDetail = false;
                }
                if (GUILayout.Button("上", GUILayout.Width(26)))
                {
                    if (i > 0)
                    {
                        var funccc = block.funcs[i - 1];
                        block.funcs[i - 1] = func;
                        block.funcs[i] = funccc;
                        SetAssetDirty();
                        return;
                    }
                }
                if (GUILayout.Button("下", GUILayout.Width(26)))
                {
                    if (i < block.funcs.Count - 1)
                    {
                        var funccc = block.funcs[i + 1];
                        block.funcs[i + 1] = func;
                        block.funcs[i] = funccc;
                        SetAssetDirty();
                        return;
                    }
                }
                if (GUILayout.Button("删", GUILayout.Width(26)))
                {
                    if (EditorUtility.DisplayDialog("warning", "确认删除", "删", "再想想"))
                    {
                        block.funcs.RemoveAt(i);
                        SetAssetDirty();
                        return;
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(45);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            postimeline2 = GUILayout.BeginScrollView(postimeline2);



            GUILayout.BeginHorizontal();
            //-1表示无限帧，无限帧显示150个可以了
            if (framecount < 0) framecount = 150;
            for (int j = 0; j < (framecount); j++)
            {
                GUILayout.Label(" " + j.ToString(), GUILayout.Width(25), GUILayout.Height(19));
            }


            GUILayout.EndHorizontal();
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            for (int j = 0; j < (framecount); j++)
            {
                int _framecount = 0;
                if (box.Count > 0)
                {
                    if (_subclip != null) _framecount = j % box.Count + (int)_subclip.startframe;
                    else _framecount = j % box.Count;
                }

                GUILayout.Label(" " + _framecount.ToString(), GUILayout.Width(25));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            #region 动画信息
            SelectAniDetail(framecount, "box_attack");
            SelectAniDetail(framecount, "box_area");
            SelectAniDetail(framecount, "box_behurt");
            #endregion
            GUILayout.Space(3);
            for (int i = 0; i < block.funcs.Count; i++)
            {
                var func = Window_StateTable.selectBlock.funcs[i];
                int endframe = func.activeFrameEnd;
                if (endframe < 0) endframe = 150;   //-1表示无限帧，无限帧显示150个可以了
                GUILayout.BeginHorizontal();
                var oc = GUI.backgroundColor;
                for (int j = 0; j < (framecount); j++)
                {

                    if (func.activeFrameBegin < 0)//开始帧小于零说明这个func没有生存周期
                    {
                        GUI.backgroundColor = (j == selectFrame || func == selectFunc) ? Color.Lerp(Color.yellow, Color.black, 0.5f) : Color.black;
                        if (j == selectFrame) GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("-", GUILayout.Width(25)))
                        {
                            SelectSelectFunc(func);
                            bEditDetail = false;
                            selectFrame = j;
                        }
                    }
                    else if (func.activeFrameBegin <= j && j <= endframe)
                    {
                        GUI.backgroundColor = (j == selectFrame || func == selectFunc) ? Color.Lerp(Color.yellow, Color.green, 0.5f) : Color.green;
                        string t = "";
                        if (func.activeFrameBegin == j)
                            t += "B";
                        if (j == endframe)
                            t += "E";
                        if (j == selectFrame) GUI.backgroundColor = Color.red;
                        if (GUILayout.Button(t, GUILayout.Width(25)))
                        {
                            SelectSelectFunc(func);
                            bEditDetail = false;
                            selectFrame = j;
                            _aniPlayer.SetPose(_clip, j);
                        }
                    }
                    else
                    {
                        GUI.backgroundColor = (j == selectFrame || func == selectFunc) ? Color.yellow : oc;
                        if (j == selectFrame) GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("", GUILayout.Width(25)))
                        {
                            SelectSelectFunc(func);
                            bEditDetail = false;
                            selectFrame = j;
                        }
                    }
                }
                GUI.backgroundColor = oc;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();


        GUILayout.EndScrollView();
        #endregion
    }


    bool bEditDetail = false;
    void OnGUIFunction()
    {
        if (selectFunc == null)
        {
            GUILayout.Label("select Func Null:");
            return;
        }
        GUILayout.Label("select Func:" + selectFunc.classname);

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(250));
        //选择功能
        OnGUI_ListCustomFunc();

        GUILayout.Space(5);
        //timeedit
        string labellife = "功能生效时间(帧)";
        if (selectFunc.activeFrameBegin >= 0)
        {
            labellife += "(" + selectFunc.activeFrameBegin + "-";
            if (selectFunc.activeFrameEnd < 0)
            {
                labellife += "无限)";
            }
            else
            {
                labellife += selectFunc.activeFrameEnd + ")";
            }
        }
        else
        {
            labellife = "<<时间无效>>";
        }
        GUILayout.Label(labellife);
        GUILayout.BeginHorizontal();
        //         if (GUILayout.Button("开始时间:无效"))
        //         {
        //             selectFunc.activeFrameBegin = -1;
        //             SetAssetDirtyFunc();
        //         }
        if (GUILayout.Button("开始时间:选中帧"))
        {
            if (selectFrame > selectFunc.activeFrameEnd)
            {
                selectFunc.activeFrameEnd = selectFrame;
            }
            selectFunc.activeFrameBegin = selectFrame;
            SetAssetDirtyFunc();
        }

        if (GUILayout.Button("结束时间:选中帧"))
        {
            if (selectFrame < selectFunc.activeFrameBegin)
            {
                selectFunc.activeFrameBegin = selectFrame;
            }
            selectFunc.activeFrameEnd = selectFrame;
            SetAssetDirtyFunc();
        }
        if (GUILayout.Button("结束时间:无限"))
        {
            selectFunc.activeFrameEnd = -1;
            SetAssetDirtyFunc();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        //
        //nameedit
        bEditDetail = GUILayout.Toggle(bEditDetail, "详细编辑（如果你要的功能已经又右侧的自定义界面，别用这个）");
        if (bEditDetail)
        {

            {
                string name = EditorGUILayout.TextField("classname:", selectFunc.classname);
                if (name != selectFunc.classname)
                {
                    selectFunc.classname = name;
                    SetAssetDirtyFunc();
                }
            }
            {
                EditorGUILayout.Toggle("HaveExit:", selectFunc.haveexit);
            }
            {
                string strParam0 = EditorGUILayout.TextField("strParam0:", selectFunc.strParam0);
                if (strParam0 != selectFunc.strParam0)
                {
                    selectFunc.strParam0 = strParam0;
                    SetAssetDirtyFunc();
                }
            }
            {
                string strParam1 = EditorGUILayout.TextField("strParam1:", selectFunc.strParam1);
                if (strParam1 != selectFunc.strParam1)
                {
                    selectFunc.strParam1 = strParam1;
                    SetAssetDirtyFunc();
                }
            }
            {
                int intParam0 = EditorGUILayout.IntField("intParam0:", selectFunc.intParam0);
                if (intParam0 != selectFunc.intParam0)
                {
                    selectFunc.intParam0 = intParam0;
                    SetAssetDirtyFunc();
                }
            }
            {
                int intParam1 = EditorGUILayout.IntField("intParam1:", selectFunc.intParam1);
                if (intParam1 != selectFunc.intParam1)
                {
                    selectFunc.intParam1 = intParam1;
                    SetAssetDirtyFunc();
                }
            }
            {
                Vector4 vecParam0 = EditorGUILayout.Vector4Field("vecParam0:", selectFunc.vecParam0);
                if (vecParam0 != selectFunc.vecParam0)
                {
                    selectFunc.vecParam0 = vecParam0;
                    SetAssetDirtyFunc();
                }
            }
        }
        if (selectFunc == null)
        {
            GUILayout.Label("select Func Null:");
            return;
        }

        GUILayout.EndVertical();
        Layout_DrawSeparatorV(Color.white);

        GUILayout.BeginVertical();
        GUILayout.Label("Custom Func Helper.");
        OnGUI_CustomFunc();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();



        GUILayout.EndHorizontal();
        Layout_DrawSeparator(Color.white);
        this.Repaint();
    }


    static Dictionary<string, ICutomFunc> regfuncs = null;

    static string pickfunc = null;
    bool isselectchange = false;
    void OnGUI_ListCustomFunc()
    {
        if (GUILayout.Button("选择功能"))
        {
            //这里要扩展
            var pmap = FB.FFSM.FightFSM.InitBlockParserMap();
            //根据pmap.keys 显示
            //如过pmapkey有，但是regfuncs没有的，就要手动编辑
            Window_PickAny.ShowAny(pmap.Keys, (str) =>
            {
                pickfunc = str;
                this.Focus();
            });

            isselectchange = true;
        }
        if (pickfunc != null && pickfunc != selectFunc.classname)
        {
            selectFunc.classname = pickfunc;
            SetAssetDirtyFunc();
            pickfunc = null;
        }
    }

    void RegFunc(ICutomFunc func)
    {
        regfuncs[func.funcclass] = func;
    }
    void OnGUI_CustomFunc()
    {
        if (regfuncs == null)
        {
            regfuncs = new Dictionary<string, ICutomFunc>();
            RegFunc(new func_test());
            RegFunc(new func_turn());
            RegFunc(new func_move());
            RegFunc(new func_attack());
            RegFunc(new func_letgo());
            RegFunc(new func_force());
            RegFunc(new func_breaklevel());
            RegFunc(new func_inputExit());
            RegFunc(new func_repeatExit());
            RegFunc(new func_propExit());
            RegFunc(new func_whenExit());
            RegFunc(new func_whenpropexit());
            RegFunc(new func_whenforceclear());
            RegFunc(new func_worldEffect());
            RegFunc(new func_whensound());
            RegFunc(new func_flyitem());
            RegFunc(new func_parry());
            RegFunc(new func_push());
        }
        string classname = selectFunc.classname;
        if (!string.IsNullOrEmpty(classname))
            if (regfuncs.ContainsKey(classname))
            {
                var func = regfuncs[classname];
                if (func != null)
                {
                    regfuncs[classname].OnGUI(selectFunc);
                    if (isselectchange)
                    {
                        bEditDetail = false;
                        isselectchange = false;
                    }

                    return;
                }


                if (isselectchange)
                {

                    isselectchange = false;

                }
            }
            else
            {
                EditorGUILayout.HelpBox("这个功能没有自定义界面，开始手动编辑吧", MessageType.Info);
                bEditDetail = true;
            }

    }


    List<List<FB.PosePlus.AniBoxCollider>> box = new List<List<AniBoxCollider>>();
    bool isshow = false;
    void SelectAniDetail(int framecount, string select)
    {
        box.Clear();
        var _clip = Window_StateTable.aniPlayer.GetClip(Window_StateTable.selectBlock.playani);
        var _subclip = _clip.GetSubClip(Window_StateTable.selectBlock.playsubani);
        int start = -1;
        int end = -1;
        if (_subclip == null)
        {
            start = 0;
            end = _clip.frames.Count - 1;
        }
        else
        {
            start = (int)_subclip.startframe;
            end = (int)_subclip.endframe;
        }
        //检索box的数据

        for (int i = start; i <= end; i++)
        {
            box.Add(_clip.frames[i].boxesinfo.FindAll(b => b.mBoxType == select));
        }
        GUILayout.BeginHorizontal();
        for (int j = 0; j < (framecount); j++)
        {
            int _framecount = j % box.Count;
            Color bc = GUI.backgroundColor;
            string buttstr = "-";
            if (box[_framecount] != null && box[_framecount].Count > 0)
            {

                if ((j <= _framecount)
                   || (j > _framecount && _subclip == null && _clip != null && _clip.aniLoop)
                   || (j > _framecount && _subclip != null && _subclip.loop))
                {
                    if (Window_StateTable.aniPlayer.boxcolor.ContainsKey(select))
                    {
                        GUI.backgroundColor = Window_StateTable.aniPlayer.boxcolor[select].boxcolor;
                    }
                    buttstr = box[_framecount].Count.ToString();
                }


            }
            if (j == selectFrame) GUI.backgroundColor = Color.red;
            if (GUILayout.Button(buttstr, GUILayout.Width(25)))
            {
                selectFrame = j;
            }

            GUI.backgroundColor = bc;
        }
        GUILayout.EndHorizontal();
    }
    void playAni(int frame)
    {
        int theFrame = 0;

        if (_subclip != null)
        {
            if (_subclip.name != "")
            {
                if (_subclip.loop)
                {
                    theFrame = (int)((selectFrame % (_subclip.endframe + 1 - _subclip.startframe)) + _subclip.startframe);
                }
                else if (selectFrame > _subclip.endframe - _subclip.startframe)
                {
                    theFrame = (int)((_subclip.endframe - _subclip.startframe) + _subclip.startframe);
                }
                else
                {
                    theFrame = selectFrame;
                }
            }
        }
        else
        {
            if (_clip != null)
            {
                if (_clip.loop)
                {
                    theFrame = selectFrame % _clip.frames.Count;
                }
                else if (selectFrame >= _clip.frames.Count)
                {
                    selectFrame = _clip.frames.Count - 1;
                }
                else
                {
                    theFrame = selectFrame;
                }
            }
        }
        _aniPlayer.SetPose(_clip, theFrame);
    }


    #region 辅助画线函数
    public static void Layout_DrawSeparator(Color color, float height = 4f)
    {

        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, Screen.width, height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(height);
    }
    public static void Layout_DrawSeparatorV(Color color, float width = 4f)
    {

        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, width, rect.height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(width);
    }
    #endregion

}
interface ICutomFunc
{
    string funcclass
    {
        get;
    }
    void Init();
    void OnGUI(FB.FFSM.BlockFunc func);
    //需要啥找他们要
    //Window_StateTable.selectBlock;
    //Window_StateTable.selectItem;
    //Window_StateTable.charController;
}