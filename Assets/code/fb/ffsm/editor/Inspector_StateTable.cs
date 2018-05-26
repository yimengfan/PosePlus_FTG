using System;
using System.Collections.Generic;

using System.Text;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(FB.FFSM.StateTable))]
public class Inspector_StateTable : Editor
{

    public override void OnInspectorGUI()
    {

        var stateTable = target as FB.FFSM.StateTable;

        if (stateTable != null)
        {
            EditorGUILayout.HelpBox("从这里打开招式编辑器将不能获取动画信息", MessageType.Warning);
            if (GUILayout.Button("打开招式编辑器"))
            {
                Window_StateTable.Show(stateTable, null, null);
            }
        }
        if (this.target == null) return;
        base.OnInspectorGUI();

    }



}

