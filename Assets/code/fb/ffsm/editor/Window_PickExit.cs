using System;
using System.Collections.Generic;

using System.Text;
using UnityEditor;
using UnityEngine;

class Window_PickExit : EditorWindow
{


    static List<string> picklist;
    static Action<FB.FFSM.BlockExit> _funccb;
    static List<string> showtext;
    static List<Color> color;


    public static void ShowExitFunc(Action<FB.FFSM.BlockExit> callback)
    {
        var window = EditorWindow.GetWindow<Window_PickExit>(true, "Window_PickExitFunc", true);
        _funccb = callback;
    }

    void OnGUI()
    {
        OnGUI_PickExitFunc();
    }
    Vector2 pos;
    void OnGUI_PickExitFunc()
    {
        FB.FFSM.BlockExit exit = new FB.FFSM.BlockExit();
        //计算list
        int exitcount = Window_StateTable.selectBlock.exits.Count;
        if (Window_StateTable.selectBlock.exits.Count == 0)
        {
            for (int i = 0; i < Window_StateTable.selectItem.blocks.Count; i++)
            {
                if (Window_StateTable.selectBlock == Window_StateTable.selectItem.blocks[i])
                {
                    if (i + 1 <= Window_StateTable.selectItem.blocks.Count)
                    {
                        picklist = new List<string>() { "next" };
                        
                        exit.statename = Window_StateTable.selectItem.name;
                        exit.blockindex = i + 1;
                    }
                }
                else if (Window_StateTable.selectBlock == Window_StateTable.selectItem.blocks[Window_StateTable.selectItem.blocks.Count-1])
                    if (EditorUtility.DisplayDialog("错误", "我是最后一个block，请为我配一个exit", "是的"))
                    {
                        return;
                    }

            }

        }
        else
        {
            picklist = new List<string>();
            for (int i = 0; i < exitcount; i++)
            {
                if (Window_StateTable.selectBlock.exits[i].statename != null && Window_StateTable.selectBlock.exits[i].blockindex != null)
                {
                    picklist.Add(((Window_StateTable.selectBlock.exits[i].statename=="")?"请先选择exit的状态名":Window_StateTable.selectBlock.exits[i].statename) + " | " + Window_StateTable.selectBlock.exits[i].blockindex.ToString());

                }
            }
        }

        pos = UnityEngine.GUILayout.BeginScrollView(pos);


        if (picklist != null)
            if (picklist[0] == "next")
            {
                if (GUILayout.Button("Next"))
                {
                    this.Close();
                    _funccb(exit);
                }
            }
            else
            {
                for (int i = 0; i < picklist.Count; i++)
                {
                    if (showtext != null)
                    {
                        if (color != null)
                            GUI.backgroundColor = color[i];
                        if (UnityEngine.GUILayout.Button(showtext[i]))
                        {
                            this.Close();
                            _funccb(Window_StateTable.selectBlock.exits[i]);
                        }
                    }
                    else
                    {
                        if (color != null)
                            GUI.backgroundColor = color[i];
                        if (UnityEngine.GUILayout.Button(picklist[i]))
                        {
                            this.Close();
                            _funccb(Window_StateTable.selectBlock.exits[i]);
                        }
                    }
                }
            }


        if (UnityEngine.GUILayout.Button("==Close=="))
        {
            this.Close();
            _funccb(null);
        }
        GUILayout.EndScrollView();
    }
}

