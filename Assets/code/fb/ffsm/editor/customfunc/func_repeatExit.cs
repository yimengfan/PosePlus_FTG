using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;

public class func_repeatExit : ICutomFunc
{

    public string funcclass
    {
        get { return "repeatexit"; }
    }
    public void Init()
    {
        exitfunc = null;
    }
    static FB.FFSM.BlockExit exitfunc = null;
    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        EditorGUILayout.HelpBox("这个功能是重复输入则跳出Block", MessageType.Info);
        func.haveexit = true;


        int nf = EditorGUILayout.IntField("多少帧内重复有效", func.intParam2);
        if (nf != func.intParam2)
        {
            func.intParam2 = nf;
            EditorUtility.SetDirty(Window_StateTable.stateTable);
        }

        bool isHitOn = (func.intParam1 == 1) ? true : false;
        isHitOn = EditorGUILayout.Toggle("是否击中：", isHitOn);
        func.intParam1 = (isHitOn) ? 1 : 0;

        GUILayout.BeginHorizontal();
        {
            List<string> tempstr = new List<string>();
            for (int i = 0; i < Window_StateTable.selectBlock.exits.Count; i++)
            {
                tempstr.Add(i.ToString());
            }
            string butt_str = "选择";
            if (GUILayout.Button(butt_str, GUILayout.Width(80)))
            {
                //如果此block没有配置exit就会添加一个next的选项最终会指向下一个block
                Window_PickExit.ShowExitFunc((exit) =>
                {
                    exitfunc = exit;
                });
            }


            if (exitfunc != null)
            {
                func.strParam1 = exitfunc.statename;
                func.intParam0 = exitfunc.blockindex;
                exitfunc = null;
            }



            EditorUtility.SetDirty(Window_StateTable.stateTable);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("跳出到状态:   " + func.strParam1);
        GUILayout.Label("blockIndex:   " + func.intParam0.ToString());
        GUILayout.EndVertical();
    }
}
