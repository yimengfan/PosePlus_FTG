using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;

public class func_attack : ICutomFunc
{
    static string BeHurtState = null;
    public void Init()
    {
        BeHurtState = null;
    }

    public string funcclass
    {
        get { return "attack"; }
    }

    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        EditorGUILayout.HelpBox("这个功能是攻击判定", MessageType.Info);
        func.haveexit = false;


        FixInt("攻击可命中多少敌人", ref func.intParam0);
        FixInt("命中HitCount", ref func.intParam1);
        GUILayout.Space(10);
        FixString("攻击影响参数", ref func.strParam0);
        FixInt("攻击影响值", ref func.intParam2);
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("攻击导致对方状态", GUILayout.Width(145));
        if (GUILayout.Button(func.strParam1))
        {
            //Window_StateLeadState.Show(Window_StateTable.StateTree.states, (str) => {
            //    func.strParam1 = str;
            //    EditorUtility.SetDirty(Window_StateTable.stateTable);
            //},false);
            List<string> allBehurtStates = new List<string>();
            foreach (var item in Window_StateTable.stateTable.allBehurtStates)
            {
                allBehurtStates.Add(item.name);
            }
            Window_PickAny.ShowAny(allBehurtStates, (str) =>
            {
                BeHurtState = str;
            });
        }
        if (!string.IsNullOrEmpty(BeHurtState))
        {
            func.strParam1 = BeHurtState;
            BeHurtState = null;
        }
        GUILayout.EndHorizontal();
        int nhold = GUILayout.Toggle(func.intParam3 > 0, "是否具有抓持效果") ? 1 : 0;
        if (nhold != func.intParam3)
        {
            func.intParam3 = nhold;
            EditorUtility.SetDirty(Window_StateTable.stateTable);
        }

        int nhurt = GUILayout.Toggle(func.intParam4 > 0, "是否伤害队友") ? 1 : 0;
        if (nhurt != func.intParam4)
        {
            func.intParam4 = nhurt;
            EditorUtility.SetDirty(Window_StateTable.stateTable);
        }
    }
    void FixInt(string desc, ref int value)
    {
        int v = EditorGUILayout.IntField(desc, value);
        if (v != value)
        {
            value = v;
            EditorUtility.SetDirty(Window_StateTable.stateTable);
        }
    }
    void FixString(string desc, ref string value)
    {
        string v = EditorGUILayout.TextField(desc, value);
        if (v != value)
        {
            value = v;
            EditorUtility.SetDirty(Window_StateTable.stateTable);
        }
    }
}
public class func_letgo : ICutomFunc
{

    public string funcclass
    {
        get { return "letgo"; }
    }

    public void Init()
    {

    }

    public void OnGUI(FB.FFSM.BlockFunc func)
    {
        EditorGUILayout.HelpBox("这个功能是松开被抓持的目标", MessageType.Info);
    }
}