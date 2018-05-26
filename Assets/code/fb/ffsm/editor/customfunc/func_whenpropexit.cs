using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;

public class func_whenpropexit : ICutomFunc
{

    public string funcclass
    {
        get { return "whenpropexit"; }
    }
    public void Init()
    {
        pickstr_exitstate = null;
        pickstr_exitint = null;
    }
    static string pickstr_exitstate = null;
    static string pickstr_exitint = null;
    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        EditorGUILayout.HelpBox("这个功能是配置PropExit", MessageType.Info);
        FB.FFSM.BlockParser_WhenPropExit.WhenExit opera = (FB.FFSM.BlockParser_WhenPropExit.WhenExit)func.intParam2;

        EditorGUILayout.BeginHorizontal();
/*        GUILayout.Label("类型：", GUILayout.Width(30));*/
        //str0
        func.strParam0 = EditorGUILayout.TextField(func.strParam0, GUILayout.Width(50));
        var exit2 = (FB.FFSM.BlockParser_WhenPropExit.WhenExit)EditorGUILayout.EnumPopup( opera,GUILayout.Width(200));
        //int2
        func.intParam2 = (int)exit2;
/*        GUILayout.Label("数值：", GUILayout.Width(30));*/
        func.intParam1 = EditorGUILayout.IntField(func.intParam1, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        //stand
        GUILayout.BeginHorizontal();
        GUILayout.Label("状态:", GUILayout.Width(50));
        string butt_str = "选择";
        if(!string.IsNullOrEmpty(func.strParam1)&&func.strParam1!= "")
        {
            butt_str = func.strParam1;
        }
        List<string> allstatename = new List<string>();
        foreach (var state in Window_StateTable.stateTable.allStates)
        {
            allstatename.Add(state.name);
        }

        if (GUILayout.Button(butt_str, GUILayout.Width(butt_str.Length*7+15)))
        {
            Window_PickState.Show(Window_StateTable.StateTree.states, (str) =>
            {
                pickstr_exitstate = str;
            },true);
        }

        if (!string.IsNullOrEmpty(pickstr_exitstate) && pickstr_exitstate!= func.strParam1)
        {
            //str1
            func.strParam1 = pickstr_exitstate;
            pickstr_exitstate =null;
            func.intParam0 = -1;
        }

      
        GUILayout.EndHorizontal();
       //index
        GUILayout.BeginHorizontal();
        GUILayout.Label("index:", GUILayout.Width(50));
        string idex_str = "选择";
        if (!string.IsNullOrEmpty(func.strParam1) && func.strParam1 != "" && func.intParam0>=0)
        {
            idex_str = func.intParam0.ToString();
        }
        var select_state = Window_StateTable.stateTable.allStates.Find(s => s.name == func.strParam1);
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
            Window_PickAny.ShowAny(allblockname, (str) =>
            {
                pickstr_exitint = str;
                //this.Focus();
                //Repaint();
            });

        }

        if (!string.IsNullOrEmpty(pickstr_exitint))
        {
            //int0
            int.TryParse(pickstr_exitint, out func.intParam0);
            pickstr_exitint = null;
        }
        //int id = EditorGUILayout.IntField(_selectBlock.exits[ie].blockindex, GUILayout.Width(120));
        GUILayout.EndHorizontal();
    }
}
