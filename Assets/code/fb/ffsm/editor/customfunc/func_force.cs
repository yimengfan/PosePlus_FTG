using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;


public class func_force : ICutomFunc
{

    public string funcclass
    {
        get { return "force"; }
    }
    public void Init()
    {

    }
    public void OnGUI(FB.FFSM.BlockFunc func)
    {

        EditorGUILayout.HelpBox("这个功能是配置func_force", MessageType.Info);
    
        bool bisclearforce = (func.intParam0 == 1) ? true : false;
        bisclearforce = EditorGUILayout.Toggle("是否清除力", bisclearforce);
        func.intParam0 = (bisclearforce) ? 1 : 0;

        bool bisjumpwithfix = (func.intParam1 == 1) ? true : false;
        bisjumpwithfix = EditorGUILayout.Toggle("摇杆移动", bisjumpwithfix);
        func.intParam1 = (bisjumpwithfix) ? 1 : 0;

        func.vecParam0 = EditorGUILayout.Vector3Field("速度", func.vecParam0);
        
    }
}
