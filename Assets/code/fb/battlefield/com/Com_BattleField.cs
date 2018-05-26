using UnityEngine;
using System.Collections;
using FB.BattleField;
using System.Collections.Generic;

public class Com_BattleField : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        battleField = new BattleField();
        Dictionary<string, string> usedchars = LoadChars();
        battleField.InitBattleField(this.transform, floor.bounds, layerForCharArea, layerForBeHurt, usedchars);

    }
    public camfollow cameraFollow;
    private static Dictionary<string, string> LoadChars()
    {
        string txt = Resources.Load<TextAsset>("char/chars").text;
        if (txt[0] > 256) txt = txt.Substring(1);
        var chars = txt.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, string> usedchars = new Dictionary<string, string>();
        foreach (var c in chars)
        {
            usedchars[c] = "test";
        }
        return usedchars;
    }
    BattleField battleField;
    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        if (delta > 0.1f) delta = 0.1f;
        battleField.Update(delta);
    }

    string activePortal;
    bool bShowBox = false;
    void OnGUI()
    {
        bool bbox = GUI.Toggle(new Rect(-100 + Screen.width / 2, 0, 200, 30), bShowBox, "显示碰撞盒子");
        if (bbox != bShowBox)
        {
            bShowBox = bbox;
            foreach (var c in battleField.createChars)
            {
                c.Value.aniplayer.IsShowBoxLine = bbox;
            }
        }
        var oc = GUI.color;
        int y = 0;
        foreach (var p in battleField.portals)
        {
            if (p.Key == activePortal)
            {
                GUI.color = new Color(0.6f, 0.6f, 1.0f);
            }
            else
            {
                GUI.color = Color.white;
            }
            if (GUI.Button(new Rect(0, y * 30, 100, 30), "入口:" + p.Key))
            {
                activePortal = p.Key;
            }
            y++;
        }
        GUI.color = oc;

        if (string.IsNullOrEmpty(activePortal) == false)
        {
            y = 0;
            foreach (var c in battleField.canUsedCharactors)
            {

                if (GUI.Button(new Rect(Screen.width - 200, y * 30, 200, 30), "创建角色:" + c))
                {

                    if (activePortal == "playerin")
                    {//替换玩家控制角色
                        //杀死之前的
                        var list = battleField.GetCCNotDeathBySide(1);
                        if (list.Count > 0)
                        {
                            battleField.Cmd_Char_Death(list[0].idCare);
                        }
                        //创建与定位
                        var p = new MyJson.JsonNode_Object();
                        p["pushbox"] = new MyJson.JsonNode_ValueNumber(true);
                        int cc = battleField.Cmd_CreateChar("char/" + c.Key, 1, p);
                        battleField.Cmd_Char_Pos(cc, battleField.portals[activePortal]);
                        //注册控制器
                        battleField.RegCharactorController(new CharController_Direct(cc, 1, FBJoy2.g_joy));
                        //摄像机跟随
                        cameraFollow.FollowObj = battleField.GetRealChar(cc).transform;
                        //其他
                        battleField.GetRealChar(cc).transform.GetComponent<FB.FFSM.com_FightFSM>().debugMode = false;
                        battleField.GetRealChar(cc).aniplayer.IsShowBoxLine = bShowBox;
                    }
                    else
                    {//创建一个渣
                        //创建与定位
                        var p = new MyJson.JsonNode_Object();
                        p["pushbox"] = new MyJson.JsonNode_ValueNumber(false);
                        int cc = battleField.Cmd_CreateChar("char/"+c.Key, 2, p);
                        battleField.Cmd_Char_Pos(cc, battleField.portals[activePortal]);
                        //注册控制器
                        var ai = new Input_AI();
                        battleField.RegCharactorController(new CharController_Direct(cc, 2, null));
                        //其他
                        battleField.GetRealChar(cc).transform.GetComponent<FB.FFSM.com_FightFSM>().debugMode = false;
                        battleField.GetRealChar(cc).aniplayer.IsShowBoxLine = bShowBox;
                    }
                }
                y++;
            }
        }
    }
    public Collider floor;
    public LayerMask layerForCharArea;
    public LayerMask layerForBeHurt;
}

