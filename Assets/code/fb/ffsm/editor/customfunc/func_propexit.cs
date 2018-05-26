using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;

public class func_propExit : ICutomFunc
{

    public string funcclass
    {
        get { return "propexit"; }
    }
    public void Init()
    {
        exitfunc = null;
    }

    static FB.FFSM.BlockExit exitfunc = null;
    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        EditorGUILayout.HelpBox("这个功能是攻击判定", MessageType.Info);
        func.haveexit = string.IsNullOrEmpty(func.strParam1);
        GUILayout.BeginVertical();
        bool ishit = (func.intParam3 == 1) ? true : false;
        ishit = EditorGUILayout.Toggle("是否击中", ishit);
        func.intParam3 = (ishit) ? 1 : 0;

        func.strParam0 = EditorGUILayout.TextField("属性：", func.strParam0);

        FB.FFSM.BlockParser_PropExit.PropOp exit = (FB.FFSM.BlockParser_PropExit.PropOp)func.intParam2;
        exit = (FB.FFSM.BlockParser_PropExit.PropOp)EditorGUILayout.EnumPopup(exit);
        if ((int)exit != func.intParam2)
            func.intParam2 = (int)exit;
        func.intParam1 = EditorGUILayout.IntField("数值：", func.intParam1);



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
                Window_PickExit.ShowExitFunc((_exit) =>
                {
                    exitfunc = _exit;
                });
            }

            if (exitfunc != null)
            {

                func.strParam1 = exitfunc.statename;
                func.intParam0 = exitfunc.blockindex;


            }
        }

        //int id = EditorGUILayout.IntField(_selectBlock.exits[ie].blockindex, GUILayout.Width(120));
        GUILayout.EndHorizontal();
        GUILayout.Label("跳出到状态:   " + func.strParam1);
        GUILayout.Label("blockIndex:   " + func.intParam0.ToString());
        GUILayout.EndVertical();

    }

}
