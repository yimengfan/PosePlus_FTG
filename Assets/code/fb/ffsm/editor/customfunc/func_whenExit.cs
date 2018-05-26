using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;

public class func_whenExit : ICutomFunc
{

    public string funcclass
    {
        get { return "whenexit"; }
    }

    static FB.FFSM.BlockExit exitfunc;
    public void Init()
    {
        exitfunc = null;
    }
    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        EditorGUILayout.HelpBox("这个功能是检测输入并跳出Block", MessageType.Info);
        func.haveexit = true;

        func.haveexit = true;
        {
            FB.FFSM.BlockParser_WhenExit.WhenExit exit = (FB.FFSM.BlockParser_WhenExit.WhenExit)func.intParam1;
            var exit2 = (FB.FFSM.BlockParser_WhenExit.WhenExit)EditorGUILayout.EnumPopup("条件", exit);

            if (exit2 != exit)
            {
                func.intParam1 = (int)exit2;
                EditorUtility.SetDirty(Window_StateTable.stateTable);
            }

        }
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
