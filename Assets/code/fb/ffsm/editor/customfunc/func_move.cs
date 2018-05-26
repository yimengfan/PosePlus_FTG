using System;
using System.Collections.Generic;

using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;

public class func_move : ICutomFunc
{

    public string funcclass
    {
        get { return "move"; }
    }
    public void Init()
    {

    }
    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        EditorGUILayout.HelpBox("这个功能是移动，有两种模式，分别有不同的参数", MessageType.Info);

        int mode = 0;
        if (func.strParam0 == "byjoy")
        {
            mode = 0;
        }
        else if (func.strParam0 == "byvec")
        {
            mode = 1;
        }
        else
        {
            func.strParam0 = "byjoy";
            EditorUtility.SetDirty(Window_StateTable.stateTable);
            mode = 0;
        }
        int mode2 = GUILayout.Toolbar(mode, new string[] { "跟随摇杆", "固定向量" });
        if (mode2 != mode)
        {
            if(mode2==0)
            {
                func.strParam0 = "byjoy";
            }
            else if(mode2==1)
            {
                func.strParam0 = "byvec";
            }
            EditorUtility.SetDirty(Window_StateTable.stateTable);
        }

        if (mode2 == 0)
        {
            float speed = EditorGUILayout.FloatField("摇杆运动速度，参考值为1（米每秒）", func.vecParam0.x);
            if (speed != func.vecParam0.x)
            {
                func.vecParam0.x = speed;
                EditorUtility.SetDirty(Window_StateTable.stateTable);
            }
        }
        else if (mode2 == 1)
        {
            Vector3 move = EditorGUILayout.Vector3Field("移动增量（每帧增量）", func.vecParam0);
            Vector3 srcmove = func.vecParam0;
            if (move != srcmove)
            {
                func.vecParam0 = move;
                EditorUtility.SetDirty(Window_StateTable.stateTable);
            }

        }

    }
}
