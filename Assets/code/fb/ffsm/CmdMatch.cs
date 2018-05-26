using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 命令匹配
/// </summary>
class CmdMatch
{
    static int CalcDir(Vector2 v, int dir)
    {
        //谁TM说的左右不分
        int x = 0;
        if (v.x < -0.5) x = -1;
        else if (v.x > 0.5) x = 1;

        int y = 0;
        if (v.y < -0.5) y = -1;
        else if (v.y > 0.5) y = 1;

        return (x * dir + 1) + (y + 1) * 3 + 1;
    }
    static bool MatchDir(char dir, int _dir)
    {
        switch (dir)
        {
            case '2':
                return (_dir == 1 || _dir == 2 || _dir == 3);
            case '4':
                return (_dir == 1 || _dir == 4 || _dir == 7);
            case '6':
                return (_dir == 3 || _dir == 6 || _dir == 9);
            case '8':
                return (_dir == 7 || _dir == 8 || _dir == 9);
            default:
                return false;
        }

    }
    static bool MatchKey(char key, PadButton state)
    {
        switch (key)
        {
            case 'L':
                return (state == PadButton.Func_Forward);
            case 'I':
                return (state == PadButton.Func_Up);
            case 'K':
                return (state == PadButton.Func_Down);
            case 'J':
                return (state == PadButton.Func);
            default:
                return false;
        }
    }
    public static bool CmdListMatch(string cmdstr, IList<Command> cmdlist, int dir, float lifetime = 0.5f)
    {
        int iseed = cmdlist.Count - 1;
        if (iseed < 0) return false;
        float timeend = Time.realtimeSinceStartup;
        for (int i = cmdstr.Length - 1; i >= 0; i--)
        {
            if (iseed < 0) return false;

            if (timeend - cmdlist[iseed].time > lifetime && lifetime > 0)
                return false;
            //if (iseed + 1 < cmdlist.Count && cmdlist.Count > 1)
            //    if (cmdlist[iseed + 1].time - cmdlist[iseed].time > lifetime) return false;
            char c = cmdstr[i];
            switch (c)
            {
                case 'R':
                    if (cmdlist[iseed].state != PadButton.None)
                        return false;
                    iseed--;
                    break;
                case 'L':
                case 'I':
                case 'K':
                case 'J':
                    if (!MatchKey(c, cmdlist[iseed].state))
                        return false;
                    if (i > 0 && char.IsNumber(cmdstr[i - 1]))
                    {
                        //上一位是数字，继续比较，不减一
                    }
                    else
                    {
                        iseed--;
                    }
                    break;
                case '2':
                case '4':
                case '6':
                case '8':
                    {
                        bool bMatchNum = false;
                        while (iseed >= 0)
                        {
                            if (iseed + 1 < cmdlist.Count)
                                if (cmdlist[iseed + 1].time - cmdlist[iseed].time > lifetime && lifetime > 0) return false; //再次循环的时候也要做时间判断
                            int _dir = CalcDir(cmdlist[iseed].dir, dir);
                            if (_dir == 5)
                            {
                                iseed--;
                                if (i == cmdstr.Length - 1) break;
                                else continue;
                            }
                            else
                            {
                                if (MatchDir(c, _dir))
                                {
                                    bMatchNum = true;
                                    iseed--;
                                    while (iseed > 0 && MatchDir(c, CalcDir(cmdlist[iseed].dir, dir)))
                                    {
                                        iseed--;
                                    }
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (!bMatchNum)
                            return false;
                    }
                    break;
                case '5':
                    {
                        int _dir = CalcDir(cmdlist[iseed].dir, dir);
                        if (_dir != 5)
                            return false;
                        iseed--;
                    }
                    break;
                default:
                    string str = "";
                    if (c.ToString() == "")
                    {
                        str = "空指令";
                    }
                    else
                    {
                        str = c.ToString();
                    }
                    throw new Exception("not support code." + str);
            }
        }
        //Debug.Log("match got:" + cmdstr);
        return true;
    }
}
