using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEditor;
public class func_turn : ICutomFunc
{

    public string funcclass
    {
        get { return "turn"; }
    }
    public void Init()
    {

    }
    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        EditorGUILayout.HelpBox("这个功能是执行并转身，这个功能最好用1帧的", MessageType.Info);
        //if(func.activeFrameBegin)
        //{
        //    func.activeFrameBegin = -1;
        //    EditorUtility.SetDirty(Window_StateTable.stateTable);
        //}
    }
}
