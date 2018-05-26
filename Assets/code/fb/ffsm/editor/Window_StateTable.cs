using System;
using System.Collections.Generic;

using System.Text;
using UnityEditor;
using UnityEngine;
using FB.PosePlus;



class Window_StateTable : EditorWindow
{
    static FB.FFSM.StateTable _stateTable;
    static FB.FFSM.TreeStateConfig _stateTree;
    public static FB.FFSM.TreeStateConfig StateTree
    {
        get { return Window_StateTable._stateTree; }
        set { Window_StateTable._stateTree = value; }
    }
    public static FB.FFSM.StateTable stateTable
    {
        get
        {
            return _stateTable;
        }
        set
        {
            _stateTable = value;
            _selectItem = null;
            SelectSelectBlock(null);
        }
    }
    static FB.FFSM.StateItem _selectItem;
    static FB.FFSM.BeHurtStateTable _selectBeHurtItem;
    static FB.FFSM.StateActionBlock _selectBlock;

    public static FB.FFSM.StateItem selectItem
    {
        get
        {
            return _selectItem;
        }
        set
        {
            _selectItem = value;
            SelectSelectBlock(null);
            statebefore_pickstr = null;
        }
    }
    public static FB.FFSM.BeHurtStateTable selectBeHurtItem
    {
        get
        {
            return _selectBeHurtItem;
        }
        set
        {
            _selectBeHurtItem = value;

            hitFire = null;
            pickstr_state_EnemyToState = null;
            statebefore_pickstr = null;
        }
    }
    public static FB.PosePlus.AniPlayer aniPlayer
    {
        get;
        private set;
    }
    public static void SelectSelectBlock(FB.FFSM.StateActionBlock block)
    {
        _selectBlock = block;
        playsubani_pickstr = null;
        playani_pickstr = null;
        exitblock_pickstr = null;
    }
    public static FB.FFSM.StateActionBlock selectBlock
    {
        get
        {
            return _selectBlock;
        }

    }
    public static void Show(FB.FFSM.StateTable table, FB.FFSM.TreeStateConfig tree, FB.PosePlus.AniPlayer aniplayer)
    {
        var window = EditorWindow.GetWindow<Window_StateTable>(false, "FightFSM编辑器");
        StateTree = tree;
        window.Init(table, tree, aniplayer);
    }

    void Init(FB.FFSM.StateTable table, FB.FFSM.TreeStateConfig tree, FB.PosePlus.AniPlayer aniplayer)
    {
        _stateTable = table;
        aniPlayer = aniplayer;
        treestateconfig = tree;
    }

    string currentWindow = "";
    Vector3 tablepos;
    FB.FFSM.TreeStateConfig treestateconfig = null;
    void OnGUI()
    {
        if (stateTable == null)
        {
            EditorGUILayout.HelpBox("select a statetable first", MessageType.Warning);
            return;
        }
        GUILayout.BeginHorizontal();
        OnGUIListArea();
        Layout_DrawSeparatorV(Color.white);
        GUILayout.BeginVertical();
        if (currentWindow == "selectBeHurtItem")
        {
            OnGUIBeHurtStateItem();
        }
        else if (currentWindow == "selectItem")
        {
            OnGUIBlockCondition();
            OnGUIStateItem();
            Layout_DrawSeparator(Color.white);
            OnGUIBlockItem();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
    int iTools;
    void OnGUIListArea()
    {
        GUILayout.BeginVertical(GUILayout.Width(320));

        GUILayout.Label("StateList:" + stateTable.name);
        if (GUILayout.Button("Save"))
        {
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("Add State Item"))
        {
            if (iTools == 0)
            {
                stateTable.allStates.Add(new FB.FFSM.StateItem());
            }
            else if (iTools == 2)
            {
                stateTable.allBehurtStates.Add(new FB.FFSM.BeHurtStateTable());
            }
        }
        GUILayout.BeginHorizontal();
        string[] strtitle = { "StateTable", "StateTree", "BeHurtState" };
        iTools = GUILayout.Toolbar(iTools, strtitle);

        GUILayout.EndHorizontal();

        tablepos = GUILayout.BeginScrollView(tablepos);
        string message = "";
        if (iTools == 1)
        {
            if (treestateconfig == null)
            {
                EditorUtility.DisplayDialog("错误", "看咩看啊？没看过错误框提示啊？\n连个tree文件都选不好，还不死去aniPlayer加上....\n这么多年书都读哪里去了(～ o ～)~zZ", "好的，大爷！");
                iTools = 0;
            }
            else
            {
                OnGui_ShowTree();
            }
        }
        else if (iTools == 0)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < stateTable.allStates.Count; i++)
            {
                var s = stateTable.allStates[i];
                if (names.Contains(s.name))
                {
                    message += "重名state:" + s.name + "\n";
                }
                if (string.IsNullOrEmpty(s.name))
                {
                    message += "存在没有名字的State\n";
                }
                names.Add(s.name);
                GUILayout.BeginHorizontal();
                string text = GUILayout.TextField(s.name);
                if (text != s.name)
                {
                    s.name = text.ToLower();
                    EditorUtility.SetDirty(stateTable);
                }
                GUILayout.Label("Block(" + s.blocks.Count + ")Condition(" + s.conditions.Count + ")");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit"))
                {
                    EditorUtility.SetDirty(stateTable);
                    if (string.IsNullOrEmpty(s.name) || s.name == "")
                    {
                        if (EditorUtility.DisplayDialog("错误", "该状态名为空，请更改后编辑！", "OK"))
                        {
                            selectItem = null;
                            return;
                        }
                    }
                    else
                    {
                        var _state = stateTable.allStates.FindAll(a => a.name == s.name);
                        if (_state.Count > 1)
                        {
                            if (EditorUtility.DisplayDialog("错误", "该状态重名，请更改后编辑！", "OK"))
                            {
                                selectItem = null;
                                return;
                            }
                            return;
                        }
                    }
                    currentWindow = "selectItem";
                    selectItem = s;
                    return;
                }
                if (GUILayout.Button("Move Up"))
                {
                    if (i > 0)
                    {
                        var sup = stateTable.allStates[i - 1];
                        stateTable.allStates[i - 1] = s;
                        stateTable.allStates[i] = sup;
                        EditorUtility.SetDirty(stateTable);
                        selectItem = null;
                        return;
                    }
                }
                if (GUILayout.Button("Move Down"))
                {
                    if (i < stateTable.allStates.Count - 1)
                    {
                        var sdown = stateTable.allStates[i + 1];
                        stateTable.allStates[i + 1] = s;
                        stateTable.allStates[i] = sdown;
                        EditorUtility.SetDirty(stateTable);
                        selectItem = null;
                        return;
                    }
                }
                if (GUILayout.Button("Delete"))
                {
                    if (EditorUtility.DisplayDialog("warning", "确认删除", "OK", "Cancle"))
                    {
                        stateTable.allStates.RemoveAt(i);
                        EditorUtility.SetDirty(stateTable);
                        selectItem = null;
                        return;
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        else if (iTools == 2)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < stateTable.allBehurtStates.Count; i++)
            {
                var s = stateTable.allBehurtStates[i];
                if (names.Contains(s.name))
                {
                    message += "重名state:" + s.name + "\n";
                }
                if (string.IsNullOrEmpty(s.name))
                {
                    message += "存在没有名字的State\n";
                }
                names.Add(s.name);
                //名字那一行
                GUILayout.BeginHorizontal();
                {
                    string text = GUILayout.TextField(s.name);
                    if (text != s.name)
                    {
                        s.name = text.ToLower();
                        EditorUtility.SetDirty(stateTable);
                    }
                    GUILayout.Label("Effects：" + s.effects.Count);
                }
                GUILayout.EndHorizontal();
                //编辑、上移、下移的行
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Edit"))
                    {
                        EditorUtility.SetDirty(stateTable);
                        if (string.IsNullOrEmpty(s.name) || s.name == "")
                        {
                            if (EditorUtility.DisplayDialog("错误", "该状态名为空，请更改后编辑！", "OK"))
                            {
                                selectBeHurtItem = null;
                                return;
                            }
                        }
                        else
                        {
                            var _state = stateTable.allBehurtStates.FindAll(a => a.name == s.name);
                            if (_state.Count > 1)
                            {
                                if (EditorUtility.DisplayDialog("错误", "该状态重名，请更改后编辑！", "OK"))
                                {
                                    selectBeHurtItem = null;
                                    return;
                                }
                                return;
                            }
                        }
                        currentWindow = "selectBeHurtItem";
                        selectBeHurtItem = s;
                        return;
                    }
                    if (GUILayout.Button("Move Up"))
                    {
                        if (i > 0)
                        {
                            var sup = stateTable.allBehurtStates[i - 1];
                            stateTable.allBehurtStates[i - 1] = s;
                            stateTable.allBehurtStates[i] = sup;
                            EditorUtility.SetDirty(stateTable);
                            selectBeHurtItem = null;
                            return;
                        }
                    }
                    if (GUILayout.Button("Move Down"))
                    {
                        if (i < stateTable.allBehurtStates.Count - 1)
                        {
                            var sdown = stateTable.allBehurtStates[i + 1];
                            stateTable.allBehurtStates[i + 1] = s;
                            stateTable.allBehurtStates[i] = sdown;
                            EditorUtility.SetDirty(stateTable);
                            selectBeHurtItem = null;
                            return;
                        }
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        if (EditorUtility.DisplayDialog("warning", "确认删除", "OK", "Cancle"))
                        {
                            stateTable.allBehurtStates.RemoveAt(i);
                            EditorUtility.SetDirty(stateTable);
                            selectBeHurtItem = null;
                            return;
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
        EditorGUILayout.HelpBox(message, message == "" ? MessageType.None : MessageType.Warning);
        GUILayout.EndVertical();

    }

    Vector2 ItemWinpos = Vector2.zero;
    List<Rect> rect = new List<Rect>();
    int blockw = 0;
    int blockh = 0;
    class Line
    {
        public Line(Vector2 from, Vector2 to)
        {
            this.from = from;
            this.to = to;
        }
        public Vector2 from;
        public Vector2 to;
        public void Draw()
        {
            if (Mathf.Abs(from.x - to.x) > 38)
            {
                float x = from.x * .5f + to.x * 0.5f;
                Handles.DrawBezier(from, to, new Vector2(x, from.y), new Vector2(x, to.y), Color.yellow, null, 5.0f);
            }
            else if (Mathf.Abs(from.x - to.x) == 37)
            {
                float x = from.x * .5f + to.x * 0.5f;
                Handles.DrawBezier(from, to, new Vector2(x, from.y), new Vector2(x, to.y), Color.green, null, 5.0f);
            }
            else
            {
                float x = from.x + Math.Abs(to.y - from.y) / 10.0f + 50;
                Handles.DrawBezier(from, to, new Vector2(x, from.y), new Vector2(x, to.y), Color.red, null, 5.0f);
            }
            //Handles.DrawLine
        }

    }
    List<Line> drawlines = new List<Line>();
    List<int> winsize = new List<int>();
    #region 绘制受击的特效列表
    public Vector2 effectP;
    //FB.FFSM.ConditionEffect tempData = new FB.FFSM.ConditionEffect();
    static string pickstr_state_EnemyToState = null;
    static string hitFire = null;
    static string hitSound = null;
    static string parryFire = null;
    static string parrySound = null;
    int currentEditIndex = 0;
    /// <summary>
    /// 绘制受击的特效列表
    /// </summary>
    void OnGUIBeHurtStateItem()
    {
        GUILayout.BeginHorizontal();
        if (_selectBeHurtItem != null)
        {
            GUILayout.Label("Current Effect：" + _selectBeHurtItem.name, GUILayout.Width(200));
            if (GUILayout.Button("New", GUILayout.Width(100)))
            {
                _selectBeHurtItem.effects.Add(new FB.FFSM.ConditionEffect());
            }
        }
        GUILayout.EndHorizontal();
        //下方显示已拥有的特效状态
        Layout_DrawSeparator(Color.white);
        if (selectBeHurtItem != null)
        {
            effectP = GUILayout.BeginScrollView(effectP);
            for (int i = 0; i < _selectBeHurtItem.effects.Count; i++)
            {
                FB.FFSM.ConditionEffect tempData = new FB.FFSM.ConditionEffect();
                tempData = _selectBeHurtItem.effects[i];

                GUILayout.BeginHorizontal();
                GUILayout.Label("ID:", GUILayout.Width(50));
                GUILayout.Label(i.ToString(), GUILayout.Width(50));
                if (GUILayout.Button("Move Up", GUILayout.Width(100)))
                {
                    _selectBeHurtItem.effects.Reverse(i - 1, 2);
                }
                if (GUILayout.Button("Move Down", GUILayout.Width(100)))
                {
                    _selectBeHurtItem.effects.Reverse(i, 2);
                }
                if (GUILayout.Button("Delete", GUILayout.Width(100)))
                {
                    if (EditorUtility.DisplayDialog("FBI Warming !!!", "是否删除此项？", "OK", "Cancle"))
                    {
                        _selectBeHurtItem.effects.RemoveAt(i);
                        return;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("OnState:", GUILayout.Width(100));
                string onState = "any";
                if (_selectBeHurtItem.effects[i].onstate != "")
                {
                    onState = _selectBeHurtItem.effects[i].onstate;
                }
                if (GUILayout.Button(onState, GUILayout.Width(200)))
                {
                    currentEditIndex = i;
                    Window_PickState.Show(treestateconfig.states, (str) =>
                    {
                        statebefore_pickstr = str;
                        this.Focus();
                    }, true);
                }
                if (!string.IsNullOrEmpty(statebefore_pickstr))
                {
                    _selectBeHurtItem.effects[currentEditIndex].onstate = statebefore_pickstr;
                    statebefore_pickstr = null;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("FaceToMe:", GUILayout.Width(100));
                _selectBeHurtItem.effects[i].facetome = EditorGUILayout.Toggle(_selectBeHurtItem.effects[i].facetome, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("EnemyToState:", GUILayout.Width(100));
                string EnemyToState = "any";
                if (_selectBeHurtItem.effects[i].enemytostate != "")
                {
                    EnemyToState = _selectBeHurtItem.effects[i].enemytostate;
                }
                if (GUILayout.Button(EnemyToState, GUILayout.Width(200)))
                {
                    currentEditIndex = i;
                    Window_PickState.Show(treestateconfig.states, (str) =>
                    {
                        pickstr_state_EnemyToState = str;
                        this.Focus();
                    }, false);
                }
                if (!string.IsNullOrEmpty(pickstr_state_EnemyToState))
                {
                    _selectBeHurtItem.effects[currentEditIndex].enemytostate = pickstr_state_EnemyToState;
                    pickstr_state_EnemyToState = null;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("HitFire:", GUILayout.Width(100));
                if (GUILayout.Button(_selectBeHurtItem.effects[i].hitfire, GUILayout.Width(200)))
                {
                    currentEditIndex = i;
                    List<string> effectsList = new List<string>();
                    GameObject[] effectsGO = (GameObject[])Resources.LoadAll<GameObject>("effect/fire");
                    foreach (GameObject GO in effectsGO)
                    {
                        effectsList.Add("effect/fire/" + GO.gameObject.name);
                    }
                    Window_PickAny.ShowAny(effectsList, (str) =>
                    {
                        hitFire = str;
                        this.Focus();
                    });
                }
                if (hitFire != null)
                {
                    _selectBeHurtItem.effects[currentEditIndex].hitfire = hitFire;
                    hitFire = null;
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.Label("HitSound:", GUILayout.Width(100));
                if (GUILayout.Button(_selectBeHurtItem.effects[i].hitsound, GUILayout.Width(200)))
                {
                    currentEditIndex = i;
                    List<string> effectsList = new List<string>();
                    GameObject[] effectsGO = (GameObject[])Resources.LoadAll<GameObject>("audio");
                    foreach (GameObject GO in effectsGO)
                    {
                        effectsList.Add("audio/" + GO.gameObject.name);
                    }
                    Window_PickAny.ShowAny(effectsList, (str) =>
                    {
                        hitSound = str;
                        this.Focus();
                    });
                }
                if (hitSound != null)
                {
                    _selectBeHurtItem.effects[currentEditIndex].hitsound = hitSound;
                    hitSound = null;
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("ParryFire:", GUILayout.Width(100));
                if (GUILayout.Button(_selectBeHurtItem.effects[i].parryfire, GUILayout.Width(200)))
                {
                    currentEditIndex = i;
                    List<string> effectsList = new List<string>();
                    GameObject[] effectsGO = (GameObject[])Resources.LoadAll<GameObject>("effect/fire");
                    foreach (GameObject GO in effectsGO)
                    {
                        effectsList.Add("effect/fire/" + GO.gameObject.name);
                    }
                    Window_PickAny.ShowAny(effectsList, (str) =>
                    {
                        parryFire = str;
                        this.Focus();
                    });
                }
                if (parryFire != null)
                {
                    _selectBeHurtItem.effects[currentEditIndex].parryfire = parryFire;
                    parryFire = null;
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.Label("ParrySound:", GUILayout.Width(100));
                if (GUILayout.Button(_selectBeHurtItem.effects[i].parrysound, GUILayout.Width(200)))
                {
                    currentEditIndex = i;
                    List<string> effectsList = new List<string>();
                    GameObject[] effectsGO = (GameObject[])Resources.LoadAll<GameObject>("audio");
                    foreach (GameObject GO in effectsGO)
                    {
                        effectsList.Add("audio/" + GO.gameObject.name);
                    }
                    Window_PickAny.ShowAny(effectsList, (str) =>
                    {
                        parrySound = str;
                        this.Focus();
                    });
                }
                if (parrySound != null)
                {
                    _selectBeHurtItem.effects[currentEditIndex].parrysound = parrySound;
                    parrySound = null;
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("MyPauseFrame:", GUILayout.Width(100));
                _selectBeHurtItem.effects[i].mypauseframe = EditorGUILayout.IntField(_selectBeHurtItem.effects[i].mypauseframe, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("EnemyPauseFrame:", GUILayout.Width(100));
                _selectBeHurtItem.effects[i].enemypauseframe = EditorGUILayout.IntField(_selectBeHurtItem.effects[i].enemypauseframe, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                Layout_DrawSeparator(Color.white);
            }

            GUILayout.EndScrollView();
        }
    }
    #endregion
    void OnGUIStateItem()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Item editor:", GUILayout.Width(100));
        if (selectItem != null)
        {
            var oc = GUI.color;
            GUI.color = Color.red;
            GUILayout.Label(selectItem.name, GUILayout.Width(100));
            GUI.color = oc;

            if (GUILayout.Button("AddBlock", GUILayout.Width(100)))
            {
                selectItem.blocks.Add(new FB.FFSM.StateActionBlock());
                EditorUtility.SetDirty(stateTable);
            }

            GUILayout.Space(50);
            if (GUILayout.Button("Save", GUILayout.Width(100)))
            {
                //安全检查 如果最后一个block没有exit并且最后一个block的blocktime不是无限长就会弹出提示框
                if (selectItem.blocks[selectItem.blocks.Count - 1].exits.Count == 0 && selectItem.blocks[selectItem.blocks.Count - 1].blocktime != -1)
                {
                    EditorUtility.DisplayDialog("错误", "没有出口exit，请检查一遍", "是的", " 是的");

                }
                else
                {
                    EditorUtility.SetDirty(stateTable);
                }
            }
        }
        else
        {
            GUILayout.Label("NULL", GUILayout.Width(100));
        }
        GUILayout.Space(100);

        GUILayout.EndHorizontal();
        ItemWinpos = GUILayout.BeginScrollView(ItemWinpos);
        if (selectItem != null)
        {
            GUILayout.BeginHorizontal();


            GUILayout.Box("", GUILayout.Width(blockw), GUILayout.Height(blockh));
            blockw = 0;
            blockh = 0;

            this.BeginWindows();
            drawlines.Clear();

            for (int i = 0; i < selectItem.blocks.Count; i++)
            {
                if (i <= winsize.Count)
                {
                    winsize.Add(0);
                }
                var block = selectItem.blocks[i];
                GUILayout.Window(i, new Rect(i * 200 + 20, 20, 150, winsize[i]), (id) =>
                {
                    if (GUI.Button(new Rect(0, 0, 25, 16), "<<"))
                    {

                        if (id > 0)
                        {
                            if (EditorUtility.DisplayDialog("warning", "确认前移", "移", "再想想"))
                            {
                                var schange = selectItem.blocks[id - 1];
                                selectItem.blocks[id - 1] = block;
                                selectItem.blocks[id] = schange;
                                EditorUtility.SetDirty(stateTable);

                                SelectSelectBlock(null);
                                return;
                            }

                        }



                    }
                    if (GUI.Button(new Rect(25, 0, 25, 16), ">>"))
                    {
                        if (id < selectItem.blocks.Count - 1)
                        {
                            if (EditorUtility.DisplayDialog("warning", "确认后移", "移", "再想想"))
                            {
                                var schange = selectItem.blocks[id + 1];
                                selectItem.blocks[id + 1] = block;
                                selectItem.blocks[id] = schange;
                                EditorUtility.SetDirty(stateTable);

                                SelectSelectBlock(null);
                                return;
                            }

                        }

                    }
                    if (GUI.Button(new Rect(115, 0, 35, 16), "Edit"))
                    {
                        SelectSelectBlock(block);
                        if (block.playani == "" && block.blocktime > 0)
                        {
                            EditorUtility.DisplayDialog("错误", "该动画名为NULL，但是bolocktime大于0，请检查！", "OK");
                        }
                    }
                    int winh = 0;
                    string ani = "<<empty>>";
                    if (string.IsNullOrEmpty(block.playani) == false)
                    {
                        ani = block.playani;
                        // if (string.IsNullOrEmpty(block.playsubani) == false)
                        // {
                        ani += "\nsub:" + ((block.playsubani == "") ? "空" : block.playsubani);
                        // }
                        ani += ("\ncross:C(" + block.playcross + ")");
                    }
                    GUILayout.Label("ani:" + ani);
                    winh += 16; ;
                    GUILayout.Label("BlockLength:" + (block.blocktime < 0 ? "无限" : (block.blocktime + "帧")));
                    winh += 16;


                    if (block.exits == null || block.exits.Count == 0)
                    {
                        winh += 30;
                        if (id < selectItem.blocks.Count - 1)
                        {
                            drawlines.Add(new Line(new Vector2(id * 200 + 140 + 20, 20 + 25 + 12), new Vector2(id * 200 + 200 + 20, 20 + 8)));
                            if (GUI.Button(new Rect(125, 25, 24, 24), "自"))
                            {
                                if (block.playani == "" && block.blocktime > 0)
                                {
                                    EditorUtility.DisplayDialog("错误", "该动画名为NULL，但是bolocktime大于0，请检查！", "OK");
                                }
                                SelectSelectBlock(block);
                            }
                        }
                    }
                    else
                    {
                        for (int ie = 0; ie < block.exits.Count; ie++)
                        {
                            winh += 30;
                            Vector2 exitPoint = new Vector2(id * 200 + 142 + 25, 20 + 25 + 12 + ie * 30);
                            if (block.exits[ie].statename == selectItem.name)
                            {
                                if (block.exits[ie].blockindex != id)
                                {
                                    exitPoint = new Vector2((block.exits[ie].blockindex - 1) * 200 + 200 + 20, 20 + 8);
                                }
                                drawlines.Add(new Line(new Vector2(id * 200 + 142 + 20, 20 + 25 + 12 + ie * 30), exitPoint));
                            }
                            if (GUI.Button(new Rect(125, ie * 30 + 25, 24, 24), ie.ToString()))
                            {
                                if (block.playani == "" && block.blocktime > 0)
                                {
                                    EditorUtility.DisplayDialog("错误", "该动画名为NULL，但是bolocktime大于0，请检查！", "OK");
                                }
                                SelectSelectBlock(block);
                            }
                        }
                    }

                    if (GUILayout.Button("Function(" + block.funcs.Count + ")", GUILayout.Width(100)))
                    {
                        SelectSelectBlock(block);
                        var clip = Window_StateTable.aniPlayer.GetClip(block.playani);
                        if (clip == null)
                        {
                            EditorUtility.DisplayDialog("错误", "都没动画你编辑Function个P啊！\n 还不死去添加个动画！", "奴家接旨。");
                            return;
                        }
                        Window_StateBlockFuncs.Show(aniPlayer, clip);
                    }
                    var oc = GUI.color;
                    GUI.color = Color.green;
                    int count = 0;
                    for (int m = 0; m < block.funcs.Count; m++)
                    {
                        var func = block.funcs[m];
                        if (GUILayout.Button("Func:" + func.classname, GUILayout.Width(100)))
                        {
                            SelectSelectBlock(block);
                            var clip = Window_StateTable.aniPlayer.GetClip(_selectBlock.playani);
                            if (clip == null)
                            {
                                EditorUtility.DisplayDialog("错误", "都没动画你编辑Function个P啊！\n 还不死去添加个动画！", "奴家接旨。");
                                return;
                            }
                            Window_StateBlockFuncs.Show(aniPlayer, clip, m, 0);
                        }
                        if (func.haveexit)
                        {
                            int _count = 0;

                            if (block.exits.Count > 0)
                            {
                                foreach (var e in block.exits)
                                {
                                    if (func.strParam1 != "")
                                        if (func.strParam1 == e.statename && func.intParam0 == e.blockindex)
                                        {
                                            //第一层循环确定左边，第二层循环确定右边
                                            drawlines.Add(new Line(new Vector2(id * 200 + 142 - 19, 20 + 25 + 90 + count * 21), new Vector2(id * 200 + 140 + 20, 20 + 25 + 12 + _count * 30)));
                                        }

                                    _count++;
                                }

                            }
                            else
                            {
                                drawlines.Add(new Line(new Vector2(id * 200 + 142 - 19, 20 + 25 + 90 + count * 21), new Vector2(id * 200 + 140 + 20, 20 + 25 + 10)));

                            }

                        }
                        count++;
                    }
                    GUI.color = oc;
                    winh = Mathf.Max(winh, 16 * (3 + block.funcs.Count));
                    winsize[id] = winh;
                }, "block" + i.ToString("D02"));
                int hallwin = winsize[i];
                //显示其他出口
                for (int ie = 0; ie < block.exits.Count; ie++)
                {
                    if (block.exits[ie].statename != selectItem.name)
                    {
                        int windowi = i;
                        int windowie = ie;
                        // Debug.Log(ie);
                        int a = ie;
                        //window 的ID之前没考虑多个block的情况重复了

                        GUILayout.Window(i * 1000 + ie + 100, new Rect(i * 200 + 40, 100 + hallwin, 130, 80), (id) =>
                        {
                            int iee = windowie;
                            GUILayout.Box("state:" + block.exits[iee].statename);
                            //GUILayout.Label("state:" + block.exits[iee].statename);
                            GUILayout.Label("blockindex:" + block.exits[iee].blockindex);
                        }, "output" + ie);
                        drawlines.Add(new Line(new Vector2(i * 200 + 142 + 20, 20 + 25 + 15 + ie * 20), new Vector2(i * 200 + 150 + 20, 100 + hallwin + 8)));

                        hallwin += 80;
                    }
                }

                blockw += 200;
                blockh = Math.Max(hallwin + 150, blockh);

            }
            testExit();
            this.EndWindows();
            GUILayout.EndHorizontal();


            Handles.BeginGUI();
            foreach (var l in drawlines)
            {
                l.Draw();
            }
            Handles.EndGUI();


        }
        GUILayout.EndScrollView();
    }

    Vector2 exitpos;
    static string statebefore_pickstr = null;
    static int sb_pickint = -1;
    static string playani_pickstr = null;
    static string playsubani_pickstr = null;
    static string pickstr_exitstate = null;
    static int es_pickint = -1;
    static string exitblock_pickstr = null;
    static int eb_pickint = -1;
    void OnGUIBlockItem()
    {
        GUILayout.BeginVertical(GUILayout.Height(200));
        if (selectBlock == null)
        {
            GUILayout.Label("Select Block:Null");
        }
        else
        {
            string label = "block";
            for (int i = 0; i < selectItem.blocks.Count; i++)
            {
                if (selectItem.blocks[i] == selectBlock)
                {
                    label += i.ToString("D02");
                    break;
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Select Block:" + label + " ani:" + selectBlock.playani + "\n" + selectBlock.playsubani, GUILayout.Width(300));
            if (GUILayout.Button("Delete Block", GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("warning", "确认删除", "OK", "Cancle"))
                {
                    selectItem.blocks.Remove(selectBlock);
                    EditorUtility.SetDirty(stateTable);
                    SelectSelectBlock(null);
                    return;
                }
            }

            GUILayout.Space(50);
            if (GUILayout.Button("Save", GUILayout.Width(100)))
            {
                EditorUtility.SetDirty(stateTable);
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Edit Functions", GUILayout.Width(200)))
            {
                var clip = Window_StateTable.aniPlayer.GetClip(_selectBlock.playani);
                if (clip == null)
                {
                    EditorUtility.DisplayDialog("错误", "都没动画你编辑Function个P啊！\n 还不死去添加个动画！", "奴家接旨。");
                    return;
                }
                Window_StateBlockFuncs.Show(aniPlayer, clip);
                //编辑function
            }
            //block基本属性编辑
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("动画名：", GUILayout.Width(60));
            if (aniPlayer != null)
            {
                string butt_str = "选择";
                if (_selectBlock.playani != "")
                {
                    butt_str = _selectBlock.playani;
                }
                List<string> allaniname = new List<string>();
                foreach (var ani in aniPlayer.clips)
                {
                    if (!string.IsNullOrEmpty(ani.name))
                    {
                        allaniname.Add(ani.name);
                    }
                }
                if (GUILayout.Button(butt_str, GUILayout.Width(80)))
                {
                    Window_PickAny.ShowAny(allaniname, (str) =>
                    {

                        playani_pickstr = str;
                        this.Focus();
                        Repaint();
                    });

                }
                if (!string.IsNullOrEmpty(playani_pickstr))
                {
                    _selectBlock.playani = playani_pickstr;
                    playani_pickstr = null;


                    //查询子动画
                    var anip = aniPlayer.clips.Find(ani => ani.name == _selectBlock.playani);
                    if (anip != null)
                        foreach (var ani in anip.subclips)
                        {
                            allaniname.Add(ani.name);
                        }
                    var ishave = allaniname.Find(a => a == _selectBlock.playsubani);
                    if (string.IsNullOrEmpty(ishave))
                        _selectBlock.playsubani = "";
                }
            }
            else
            {
                _selectBlock.playani = GUILayout.TextField(_selectBlock.playani, GUILayout.Width(80));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("子动画名：", GUILayout.Width(60));
            if (aniPlayer != null)
            {
                string butt_str = "选择";
                if (_selectBlock.playani != "" || !string.IsNullOrEmpty(_selectBlock.playani))
                {
                    butt_str = _selectBlock.playsubani;
                }
                List<string> allaniname = new List<string>();
                var anip = aniPlayer.clips.Find(ani => ani.name == _selectBlock.playani);
                if (anip != null)
                    foreach (var ani in anip.subclips)
                    {
                        allaniname.Add(ani.name);
                    }
                if (GUILayout.Button(butt_str, GUILayout.Width(80)))
                {
                    Window_PickAny.ShowAny(allaniname, (str) =>
                    {

                        playsubani_pickstr = str;
                        this.Focus();
                        Repaint();
                    });
                }
                if (!string.IsNullOrEmpty(playsubani_pickstr))
                {
                    _selectBlock.playsubani = playsubani_pickstr;
                    playsubani_pickstr = null;
                }
                EditorUtility.SetDirty(stateTable);
            }
            else
            {
                _selectBlock.playsubani = GUILayout.TextField(_selectBlock.playsubani, GUILayout.Width(80));
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("PlayCorss：", GUILayout.Width(60));
            _selectBlock.playcross = EditorGUILayout.FloatField(_selectBlock.playcross, GUILayout.Width(80));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("BlockTime：", GUILayout.Width(60));
            _selectBlock.blocktime = EditorGUILayout.IntField(_selectBlock.blocktime, GUILayout.Width(80));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();

            testExit();//检测出口并且提醒；

            //Exits 编辑
            GUILayout.Label("exit count:" + _selectBlock.exits.Count, GUILayout.Width(100));
            if (GUILayout.Button("Add Exit", GUILayout.Width(100)))
            {
                _selectBlock.exits.Add(new FB.FFSM.BlockExit());
                EditorUtility.SetDirty(stateTable);
            }

            GUILayout.EndHorizontal();
            Layout_DrawSeparator(Color.white);

            exitpos = GUILayout.BeginScrollView(exitpos, GUILayout.Height(150));
            GUILayout.BeginHorizontal();

            for (int ie = 0; ie < _selectBlock.exits.Count; ie++)
            {
                GUILayout.BeginVertical();

                GUILayout.Label("exit" + ie.ToString("D02"));
                GUILayout.BeginHorizontal();
                GUILayout.Label("状态名:", GUILayout.Width(50));

                string butt_str = "选择";
                if (_selectBlock.exits[ie].statename != "" && !string.IsNullOrEmpty(_selectBlock.exits[ie].statename))
                {
                    butt_str = _selectBlock.exits[ie].statename;
                }
                List<string> allstatename = new List<string>();
                foreach (var state in stateTable.allStates)
                {
                    allstatename.Add(state.name);
                }

                if (GUILayout.Button(butt_str, GUILayout.Width(120)))
                {
                    es_pickint = ie;
                    Window_PickState.Show(treestateconfig.states, (str) =>
                    {
                        pickstr_exitstate = str;
                        this.Focus();
                        Repaint();
                    }, true
                    );

                }


                if (!string.IsNullOrEmpty(pickstr_exitstate) && es_pickint == ie)
                {
                    selectBlock.exits[ie].statename = pickstr_exitstate;
                    pickstr_exitstate = null;
                    es_pickint = -1;

                }
                // = str;
                EditorUtility.SetDirty(stateTable);


                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("index:", GUILayout.Width(50));
                string idex_str = "选择";

                idex_str = _selectBlock.exits[ie].blockindex.ToString();

                var select_state = stateTable.allStates.Find(s => s.name == selectBlock.exits[ie].statename);
                List<string> allblockname = new List<string>();
                int count = 0;
                if (select_state != null)
                    foreach (var block in select_state.blocks)
                    {

                        allblockname.Add(count.ToString());
                        count++;
                    }

                if (GUILayout.Button(idex_str, GUILayout.Width(120)))
                {
                    eb_pickint = ie;
                    Window_PickAny.ShowAny(allblockname, (str) =>
                    {
                        exitblock_pickstr = str;
                        this.Focus();
                        Repaint();
                    });

                }


                if (!string.IsNullOrEmpty(exitblock_pickstr) && ie == eb_pickint)
                {
                    int.TryParse(exitblock_pickstr, out selectBlock.exits[ie].blockindex);
                    eb_pickint = -1;
                    exitblock_pickstr = null;
                }
                //int id = EditorGUILayout.IntField(_selectBlock.exits[ie].blockindex, GUILayout.Width(120));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (GUILayout.Button("<<", GUILayout.Width(30)))
                {
                    if (ie > 0)
                    {
                        if (EditorUtility.DisplayDialog("提示", "是否要左移？", "别逼逼", "我再想想"))
                        {
                            var temp = _selectBlock.exits[ie];
                            _selectBlock.exits[ie] = _selectBlock.exits[ie - 1];
                            _selectBlock.exits[ie - 1] = temp;
                            EditorUtility.SetDirty(stateTable);
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "首帧无法左移！", "OK");

                    }

                }
                GUILayout.Space(10);
                if (GUILayout.Button("DEL", GUILayout.Width(40)))
                {
                    _selectBlock.exits.RemoveAt(ie);
                }
                GUILayout.Space(10);
                if (GUILayout.Button(">>", GUILayout.Width(30)))
                {
                    if (ie < _selectBlock.exits.Count)
                    {
                        if (EditorUtility.DisplayDialog("提示", "是否要右移？", "别逼逼", "我再想想"))
                        {
                            var temp = _selectBlock.exits[ie];
                            _selectBlock.exits[ie] = _selectBlock.exits[ie + 1];
                            _selectBlock.exits[ie + 1] = temp;
                            EditorUtility.SetDirty(stateTable);
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "首帧无法右移！", "OK");

                    }

                }

                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                Layout_DrawSeparatorV(Color.white);
            }
            GUILayout.EndHorizontal();
            if (_selectBlock.exits.Count == 0)
            {
                GUILayout.Space(150);
            }
            Layout_DrawSeparator(Color.white);
            GUILayout.EndScrollView();

        }
        GUILayout.EndVertical();
    }

    private static void testExit()
    {
        bool isexit = false;
        string massagetex = "";
        if (_selectItem.blocks != null)
        {
            foreach (var block in _selectItem.blocks)
            {
                foreach (var blockfunc in block.funcs)
                {
                    if (blockfunc.strParam1 != null)
                    {
                        isexit = true;
                    }
                }
                if (block.exits.Count > 0)
                {
                    isexit = true;
                }
            }
            if (!isexit || _selectItem.blocks[_selectItem.blocks.Count - 1].exits.Count == 0)
            {
                if (_selectItem.blocks.Count > 0)
                {
                    if (_selectItem.blocks[_selectItem.blocks.Count - 1].blocktime != -1)
                    {
                        massagetex = "检测到部分block没有出口";
                        EditorGUILayout.HelpBox(massagetex, massagetex == "" ? MessageType.None : MessageType.Warning);
                    }
                }
                else
                {
                    massagetex = "检测到部分block没有出口";
                    EditorGUILayout.HelpBox(massagetex, massagetex == "" ? MessageType.None : MessageType.Warning);
                }
            }
        }
    }

    Vector2 conditionpos;
    List<int> conwinsize = new List<int>();
    void OnGUIBlockCondition()
    {
        //return;

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("条件：", GUILayout.Width(50));
        GUILayout.Space(5);
      
            
        if (GUILayout.Button("AddCondition", GUILayout.Width(100)))
        {
            if (selectItem == null) return;
            selectItem.conditions.Add(new FB.FFSM.StateCondition());
            EditorUtility.SetDirty(stateTable);
        }

        GUILayout.Space(50);
        if (GUILayout.Button("Save", GUILayout.Width(100)))
        {
            bool bCheck = false;
            if (selectItem.conditions != null)

                foreach (var con in selectItem.conditions)
                {
                    if (con.cmdActive == "")
                    {
                        EditorUtility.DisplayDialog("错误", "保存失败，有输入指令为空，请检查！", "OK");
                        bCheck = true;
                        break;
                    }
                }
            if (!bCheck)
                EditorUtility.SetDirty(stateTable);
        }

        GUILayout.Space(50);
        selectItem.bCanDeath = GUILayout.Toggle(selectItem.bCanDeath,"是否可死亡",GUILayout.Width(100));

        GUILayout.EndHorizontal();
        Layout_DrawSeparator(Color.white);

        conditionpos = GUILayout.BeginScrollView(conditionpos, true, false, GUILayout.Height(190));
        GUILayout.BeginHorizontal();

        if (selectItem != null && selectItem.conditions.Count > 0)
        {
            for (int i = 0; i < selectItem.conditions.Count; i++)
            {

                var con = selectItem.conditions[i];

                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Space(50);
                GUILayout.Label("Condition:" + i.ToString());
                GUILayout.EndHorizontal();
                //state
                GUILayout.BeginHorizontal();

                GUILayout.Label("前置状态", GUILayout.Width(50));
                GUILayout.Space(5);
                string butt_str = "选择";
                if (con.stateBefore != "")
                {
                    butt_str = con.stateBefore;
                }
                List<string> allstatename = new List<string>();
                //选择前置状态
                if (treestateconfig != null)
                {
                    foreach (var state in treestateconfig.states)
                    {
                        allstatename.Add(state.name);
                    }
                }
                //if (statebefore_pickstr.Count <= i) statebefore_pickstr.Add("");
                if (GUILayout.Button(butt_str, GUILayout.Width(120)))
                {
                    sb_pickint = i;
                    Window_PickState.Show(treestateconfig.states, (str) =>
                    {
                        statebefore_pickstr = str; this.Focus();
                    }, true
                    );

                }

                if (!string.IsNullOrEmpty(statebefore_pickstr) && i == sb_pickint)
                {
                    con.stateBefore = statebefore_pickstr;
                    sb_pickint = -1;
                    statebefore_pickstr = null;
                }
                //sb_pickint++;
                GUILayout.EndHorizontal();
                //
                GUILayout.BeginHorizontal();
                GUILayout.Label("触发指令", GUILayout.Width(50));
                GUILayout.Space(5);
                con.cmdActive = GUILayout.TextField(con.cmdActive.ToUpper(), GUILayout.Width(120));
                GUILayout.Space(5);
                if (i != 0)
                    if (GUILayout.Button("<<", GUILayout.Width(30)))
                    {
                        if (EditorUtility.DisplayDialog("提示", "是否要左移？", "是的", "我再想想"))
                        {
                            var temp = selectItem.conditions[i];
                            selectItem.conditions[i] = selectItem.conditions[i - 1];
                            selectItem.conditions[i - 1] = temp;
                            EditorUtility.SetDirty(stateTable);
                        }

                    }
                GUILayout.EndHorizontal();
                //                
                GUILayout.BeginHorizontal();
                GUILayout.Label("指令间隔", GUILayout.Width(50));
                GUILayout.Space(5);
                con.cmdTime = EditorGUILayout.FloatField(con.cmdTime, GUILayout.Width(120));

                GUILayout.Space(5);
                if (i != selectItem.conditions.Count - 1)
                    if (GUILayout.Button(">>", GUILayout.Width(30)))
                    {
                        if (EditorUtility.DisplayDialog("提示", "是否要右移？", "是的", "别"))
                        {
                            var temp = selectItem.conditions[i];
                            selectItem.conditions[i] = selectItem.conditions[i + 1];
                            selectItem.conditions[i + 1] = temp;
                            EditorUtility.SetDirty(stateTable);
                        }

                    }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("中断等级", GUILayout.Width(50));
                GUILayout.Space(5);
                con.breaklevel = EditorGUILayout.IntField(con.breaklevel, GUILayout.Width(120));
                GUILayout.EndHorizontal();

                con.isair = EditorGUILayout.Toggle("空中释放", con.isair, GUILayout.Width(175));

                GUILayout.BeginHorizontal();
                GUILayout.Label("消耗属性", GUILayout.Width(50));
                GUILayout.Space(5);
                con.useattribute = EditorGUILayout.TextField(con.useattribute, GUILayout.Width(120));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("消耗数值", GUILayout.Width(50));
                GUILayout.Space(5);
                con.usage_amount = EditorGUILayout.IntField(con.usage_amount, GUILayout.Width(120));
                GUILayout.EndHorizontal();

                if (GUILayout.Button("DEL", GUILayout.Width(180)))
                {
                    selectItem.conditions.RemoveAt(i);
                    EditorUtility.SetDirty(stateTable);
                }
                GUILayout.Space(5);
                //Layout_DrawSeparatorV(Color.white);
                GUILayout.EndVertical();

                Layout_DrawSeparatorV(Color.white);
            }


        }



        else
        {
            GUILayout.BeginVertical();
            GUILayout.Space(115);
            GUILayout.EndVertical();
        }

        GUILayout.EndHorizontal();
        Layout_DrawSeparator(Color.white);
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    List<FB.FFSM.TreeStateItem> rootnodeList = new List<FB.FFSM.TreeStateItem>();
    Dictionary<FB.FFSM.TreeStateItem, bool> treedata = new Dictionary<FB.FFSM.TreeStateItem, bool>();
    List<FB.FFSM.TreeStateItem> tree;
    void OnGui_ShowTree()
    {

        List<string> father = new List<string>();
        tree = new List<FB.FFSM.TreeStateItem>(treestateconfig.states);
        foreach (FB.FFSM.TreeStateItem s in tree)
        {
            if (string.IsNullOrEmpty(s.parent) || s.parent == "")
            {
                var _s = rootnodeList.Find(temps => s.name == temps.name);
                father.Add(s.name);
                if (_s == null)
                {
                    rootnodeList.Add(s);

                }
            }
        }

        GUILayout.BeginVertical();
        TreeButton("", 0);
        GUILayout.Space(10);

        GUILayout.Label("This StateTable");
        foreach (var s in stateTable.allStates)
        {
            if (!isRepeat(s.name))
            {
                if (GUILayout.Button(s.name, GUILayout.Width(s.name.Length * 6 + 15)))
                {
                    selectItem = s;
                }
            }
            // GUILayout.Label(s.name);
        }

        GUILayout.EndVertical();
    }

    private bool isRepeat(string p)
    {
        bool isrepeat = false;
        foreach (var tsi in tree)
        {
            if (p == tsi.name)
            {
                isrepeat = true;
            }
        }
        return isrepeat;
    }

    public void TreeButton(string str, int space)
    {

        List<FB.FFSM.TreeStateItem> father = tree.FindAll(t => str == t.parent);

        foreach (FB.FFSM.TreeStateItem f in father)
        {
            FB.FFSM.TreeStateItem _state = tree.Find(t => t.parent == f.name);

            GUILayout.BeginHorizontal();
            GUILayout.Space(space);

            if (_state != null)
            {
                if (!treedata.ContainsKey(f))
                    treedata.Add(f, true);
                treedata[f] = GUILayout.Toggle(treedata[f], "", GUILayout.Width(10));
            }
            else
            {
                GUILayout.Space(10);
            }
            Color oc = GUI.color;
            GUI.color = Color.red;
            var state = stateTable.allStates.Find(s => s.name == f.name);
            if (state == null)
                GUILayout.Label("(miss)", GUILayout.Width(40));
            GUI.color = oc;
            //设置显示颜色           
            switch (f.color)
            {
                case FB.FFSM.stateColor.Normal:
                    break;
                case FB.FFSM.stateColor.Red:
                    GUI.color = new Color(1f, 0.5f, 0f);
                    break;
                case FB.FFSM.stateColor.Yellow:
                    GUI.color = Color.yellow;
                    break;
                case FB.FFSM.stateColor.Blue:
                    GUI.color = Color.blue;
                    break;
                default:
                    break;
            }

            if (GUILayout.Button(f.name, GUILayout.Width(f.name.Length * 6 + 15)))
            {
                if (state == null)
                {
                    FB.FFSM.StateItem newSI = new FB.FFSM.StateItem();
                    newSI.name = f.name;
                    newSI.blocks = new List<FB.FFSM.StateActionBlock>();
                    newSI.conditions = new List<FB.FFSM.StateCondition>();
                    selectItem = newSI;
                    stateTable.allStates.Add(newSI);
                }
                else
                {
                    selectItem = state;

                }
            }
            GUI.color = oc;


            GUILayout.EndHorizontal();

            if (treedata.ContainsKey(f))
                if (treedata[f])
                {
                    TreeButton(f.name, space + 15);
                }


        }
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