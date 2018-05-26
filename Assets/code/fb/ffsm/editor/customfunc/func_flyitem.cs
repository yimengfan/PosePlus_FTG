using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;


public class func_flyitem : ICutomFunc
{

    public string funcclass
    {
        get { return "flyitem"; }
    }
    public void Init()
    {

    }
    public void OnGUI(FB.FFSM.BlockFunc func)
    {

        EditorGUILayout.HelpBox("这个功能是配置func_flyitem", MessageType.Info);

        func.intParam0 = EditorGUILayout.IntField("HP", func.intParam0);
        func.intParam1 = EditorGUILayout.IntField("Life", func.intParam1);
        func.strParam0 = EditorGUILayout.TextField("飞行道具", func.strParam0);
        func.vecParam0 = EditorGUILayout.Vector3Field("加力", func.vecParam0);
    }
}
