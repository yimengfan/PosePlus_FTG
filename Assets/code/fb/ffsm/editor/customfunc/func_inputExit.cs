using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;

public class func_inputExit : ICutomFunc
{

    public string funcclass
    {
        get { return "inputexit"; }
    }

    static FB.FFSM.BlockExit exitfunc = null;
    public void Init()
    {
        exitfunc = null;
    }
    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        EditorGUILayout.HelpBox("这个功能是检测输入并跳出Block", MessageType.Info);
        func.haveexit = true;

        {
            var text = EditorGUILayout.TextField("输入指令", func.strParam0.ToUpper());
            if (text != func.strParam0)
            {
                func.strParam0 = text;
                EditorUtility.SetDirty(Window_StateTable.stateTable);
            }
        }
        func.vecParam0.x = EditorGUILayout.FloatField("指令时间：", func.vecParam0.x);

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
                if (exitfunc != null)
                {
                    func.strParam1 = exitfunc.statename;
                    func.intParam0 = exitfunc.blockindex;
                }

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
