using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;

public class func_parry : ICutomFunc
{
    public void Init()
    {
    }
    public string funcclass
    {
        get { return "parry"; }
    }
    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        EditorGUILayout.HelpBox("这个功能是添加parry", MessageType.Info);
    }

}
