using System;
using System.Collections.Generic;

using System.Text;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(FB.BattleField.Dev_AI_DebugModel))]
public class Inspector_AI_DebugModel : Editor
{

    public override void OnInspectorGUI()
    {

        var ai_table = target as FB.BattleField.Dev_AI_DebugModel;    
        base.OnInspectorGUI();
          
        if (GUILayout.Button("计算AI出招表"))
        {
            ai_table.DebugModelStart();
        }
        
    }



}

