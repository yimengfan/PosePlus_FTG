using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;



class func_whensound:ICutomFunc
{
    public string funcclass
    {
        get { return "whensound"; }
    }

    public void Init()
    {
        throw new NotImplementedException();
    }

    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        EditorGUILayout.HelpBox("这个功能是配置func_whensound", MessageType.Info);
        var exit = (FB.FFSM.BlockParser_WhenExit.WhenExit)func.intParam0;
        exit = (FB.FFSM.BlockParser_WhenExit.WhenExit)EditorGUILayout.EnumPopup("条件", exit);
        func.strParam0 = EditorGUILayout.TextField("声源", func.strParam0);
        if ((int)exit != func.intParam0)
        {
            func.intParam0 = (int)exit;
            EditorUtility.SetDirty(Window_StateTable.stateTable);
        }
        
    }
}
