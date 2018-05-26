using System;
using System.Collections.Generic;

using System.Text;
using UnityEditor;
using UnityEngine;


//=======
//=====================
[CustomEditor(typeof(FB.FFSM.com_FightFSM))]
public class Inspector_fightfsm : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (this.target == null) return;

        var con = target as FB.FFSM.com_FightFSM;

        if (con.stateTable == null)
        {
            EditorGUILayout.HelpBox("选择一张配置表,必须来自StateTable的表", MessageType.Info);
        }
        else
        {
            if (GUILayout.Button("打开招式编辑器"))
            {
                Window_StateTable.Show(con.stateTable, con.stateTree, con.GetComponent<FB.PosePlus.AniPlayer>());
            }
        }
    }



}

