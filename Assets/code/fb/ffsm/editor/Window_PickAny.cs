using System;
using System.Collections.Generic;

using System.Text;
using UnityEditor;
using UnityEngine;

class Window_PickAny : EditorWindow
{
    public static void ShowAny(IEnumerable<string> pick, Action<string> callback, IEnumerable<string> _showtext=null,IEnumerable<Color> _color =null)
    {
        var window = EditorWindow.GetWindow<Window_PickAny>(true, "Window_PickAny", true);
        picklist = new List<string>(pick);
        _callback = callback;
        if(_showtext!=null)
        showtext = new List<string>(_showtext);
        if(_color!=null)
        color = new List<Color>(_color);
    }


    static List<string> picklist;
    static Action<string> _callback;
    static Action<FB.FFSM.BlockExit> _funccb;
    static List<string> showtext;
    static List<Color> color;

    Vector2 pos;
    void OnGUI()
    {        
        OnGUI_PickAny();
    }

    void OnGUI_PickAny()
    {
        if (UnityEngine.GUILayout.Button("不选择"))
        {
            this.Close();
            _callback("");
        }
        pos = UnityEngine.GUILayout.BeginScrollView(pos);
        if (picklist != null)
            for (int i = 0; i < picklist.Count; i++)
            {
                if (showtext != null)
                {
                    if (color != null)
                        GUI.backgroundColor = color[i];
                    if (UnityEngine.GUILayout.Button(showtext[i]))
                    {
                        this.Close();
                        _callback(picklist[i]);
                    }
                }
                else
                {
                    if (color != null)
                        GUI.backgroundColor = color[i];
                    if (UnityEngine.GUILayout.Button(picklist[i]))
                    {
                        this.Close();
                        _callback(picklist[i]);
                    }
                }
            }

        if (UnityEngine.GUILayout.Button("==Close=="))
        {
            this.Close();
            _callback(null);
        }
        GUILayout.EndScrollView();
    }


   
}
