using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;

public class func_push : ICutomFunc
{
    public void Init()
    {
    }

    public string funcclass
    {
        get { return "push"; }
    }
    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        EditorGUILayout.HelpBox("这个功能是攻击判定", MessageType.Info);
    }
}
