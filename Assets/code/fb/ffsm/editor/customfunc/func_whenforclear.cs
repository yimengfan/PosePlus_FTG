using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;


public class func_whenforceclear : ICutomFunc
{

    public string funcclass
    {
        get { return "whenforceclear"; }
    }
    public void Init()
    {

    }
    public void OnGUI(FB.FFSM.BlockFunc func)
    {

        EditorGUILayout.HelpBox("这个功能是配置func_forceclear", MessageType.Info);
        var exit = (FB.FFSM.BlockParser_WhenExit.WhenExit)func.intParam0;
        exit = (FB.FFSM.BlockParser_WhenExit.WhenExit)EditorGUILayout.EnumPopup("条件", exit);
        if ((int)exit != func.intParam0)
        {
            func.intParam0 = (int)exit;
            EditorUtility.SetDirty(Window_StateTable.stateTable);
        }
        
    }
}
