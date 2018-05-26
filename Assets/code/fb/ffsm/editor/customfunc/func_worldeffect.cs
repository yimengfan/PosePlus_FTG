using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;

public class func_worldEffect : ICutomFunc
{

    public string funcclass
    {
        get { return "worldeff"; }
    }
    public void Init()
    {
       
    }

    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        //int0 dark screen
        //int1 焦点（镜头放大跟踪）
        //int2 暂停世界（除了我）
        //int3 摇晃摄像机
        EditorGUILayout.HelpBox("这个功能是触发世界级效果", MessageType.Info);
        {
            int b = EditorGUILayout.Toggle("dark screen", func.intParam0 > 0) ? 1 : 0;
            if(b!=func.intParam0)
            {
                func.intParam0 = b;
                EditorUtility.SetDirty(Window_StateTable.stateTable);
            }
        }
        {
            int b = EditorGUILayout.Toggle("焦点（镜头放大跟踪）", func.intParam1 > 0) ? 1 : 0;
            if (b != func.intParam1)
            {
                func.intParam1 = b;
                EditorUtility.SetDirty(Window_StateTable.stateTable);
            }
        }
        {
            int b = EditorGUILayout.Toggle("暂停世界（除了我）", func.intParam2 > 0) ? 1 : 0;
            if (b != func.intParam2)
            {
                func.intParam2 = b;
                EditorUtility.SetDirty(Window_StateTable.stateTable);
            }
        }
        {
            int b = EditorGUILayout.Toggle("摇晃摄像机", func.intParam3 > 0) ? 1 : 0;
            if (b != func.intParam3)
            {
                func.intParam3 = b;
                EditorUtility.SetDirty(Window_StateTable.stateTable);
            }
        }

    }

}
