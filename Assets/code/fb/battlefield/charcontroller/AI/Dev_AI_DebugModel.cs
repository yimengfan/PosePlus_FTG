using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using FB.FFSM;
using FB.PosePlus;


namespace FB.BattleField
{

  public class Dev_AI_DebugModel : MonoBehaviour
    {

      AI_StateTable ai_table;
      com_FightFSM fightfsm;
      AniPlayer ap;
      StateTable table;
      void Awake()
      {
          
          fightfsm = transform.GetComponent<com_FightFSM>();
          ap = transform.GetComponent<AniPlayer>();
          ai_table = fightfsm.aiStateTable;
          table = fightfsm.stateTable;
      }

      public void DebugModelStart()
      {
          Awake();
          ai_table.allstates.Clear();
          foreach(var s in table.allStates)
          {
              AI_StateItem item = new AI_StateItem();
           
              //技能分析
              AI_StateAttribute attribute = new AI_StateAttribute();
              if (s.conditions.Count == 0) continue;
              attribute.cmdStr = s.conditions[0].cmdActive;
              attribute.name = s.name;
              ParseBlock(s.blocks[0],item, attribute); //遍历此状态可能的变招
              foreach (var attr in item.cmdAttribute)
              {
                  if (attr.attackpos == 0)
                  {
                       item.statetype = 1;//移动               
                  }
                  else
                  {
                      item.statetype = 2;//攻击
                      if (attr.attackpos + attr.movenum.x <= 2)
                          item.attacktype = 1;//近距离
                      else if (attr.attackpos + attr.movenum.x <= 4)
                          item.attacktype = 2;//中距离
                      else 
                          item.attacktype = 3;//远距离
                      break;
                  }
              }
              ai_table.allstates.Add(item);//加入指令表
          }
        
       }

      void ParseBlock(StateActionBlock block,AI_StateItem ai_item, AI_StateAttribute ai_attribute)
      {
          foreach (var f in block.funcs)
          {
              if (f.classname.Equals("attack"))
              {
                  AniClip ac = ap.GetClip(block.playani);
                  SubClip subAc = null;
                  if (block.playsubani != "" && block.playsubani != null)
                      subAc = ac.GetSubClip(block.playsubani);
                  int begin = 0;
                  int end = 0;
                  if (subAc != null)
                  {
                      begin = (int)subAc.startframe;
                      end = (int)subAc.endframe;
                  }
                  else
                  {
                      begin = 0;
                      end = ac.frames.Count - 1;
                  }
                  for (int i = begin; i <= end; i++) //第一层，遍历frame
                  {
                      foreach (var box in ac.frames[i].boxesinfo) //第二层遍历box
                      {
                          if (box.mBoxType.Equals("box_attack"))
                          {
                              float pos = (box.mPosition.z + box.mSize.z / 2);
                              if (ai_attribute.attackpos < pos)
                              {
                                  ai_attribute.attackpos = pos;
                              }

                          }
                      }
                   }
              }
              else if (f.classname.Equals("move") || f.classname.Equals("force"))
              {
                  ai_attribute.movenum = (ai_attribute.movenum + f.vecParam0);    //记下位移 作为判断条件
              }
          }
          foreach(var s in table.allStates)
          {
              if (s.conditions.Count == 0) continue;
              foreach(var c in s.conditions)
              {
                  if(!c.stateBefore.Equals("stand")&&!c.stateBefore.Equals("walk"))
                  {
                      if(c.stateBefore.Equals(ai_attribute.name))
                      {
                          AI_CanChangeState _s = new AI_CanChangeState();
                          _s.state = s.name;
                          if (c.cmdActive != "5")
                          {
                              _s.cmdstr = c.cmdActive;
                              ai_attribute.canChangeState.Add(_s);
                          }
                      }
                  }
              }
          }
          ai_item.cmdAttribute.Insert(0, ai_attribute);
      }


      /// <summary>
      /// 解析block Func
      /// </summary>
      /// <param name="block"></param>
      /// <param name="ai_item"></param>
      /// <param name="ai_attribute"></param>
//       void ParseBlock(StateActionBlock block, AI_StateItem ai_item,AI_StateAttribute ai_attribute)
//       {
//           foreach (var f in block.funcs)
//           {
//               if (f.classname.Equals("inputexit") || f.classname.Equals("repeatexit"))//输入指令跳转，记录下指令
//               {
//                   AI_StateAttribute ai_attr = new AI_StateAttribute();
//                   if (f.classname.Equals("repeatexit")) //repeatexit 要相同指令
//                       ai_attr.cmdStr = ai_attribute.cmdStr; 
//                   else
//                   ai_attr.cmdStr = f.strParam0;
//                   if(!string.IsNullOrEmpty(f.strParam0)&& !f.strParam0.Equals("") );
//                   {
//                      var _state  = table.allStates.Find(a=>a.name ==f.strParam1);
//                      StateActionBlock _block = _state.blocks[f.intParam0];
//                      if (_block != null)
//                      {
//                          ParseBlock(_block, ai_item, ai_attr);
// 
//                      }
//                      else
//                          Debug.Log("找不到block：" + f.strParam1 + ",index:" + f.intParam0);
//                       
//                   }                 
//               }
// 
//               else if (f.classname.Equals("attack"))
//               {
//                   AniClip ac = ap.GetClip(block.playani);
//                   SubClip subAc = null;
//                   if (block.playsubani != "" && block.playsubani != null)
//                       subAc = ac.GetSubClip(block.playsubani);
//                   int begin = 0;
//                   int end = 0;
//                   if (subAc != null)
//                   {
//                       begin = (int)subAc.startframe;
//                       end = (int)subAc.endframe;
//                   }
//                   else
//                   {
//                       begin = 0;
//                       end = ac.frames.Count - 1;
//                   }
//                   for (int i = begin; i <= end; i++) //第一层，遍历frame
//                   {
//                       foreach (var box in ac.frames[i].boxesinfo) //第二层遍历box
//                       {
//                           if (box.mBoxType.Equals("box_attack"))
//                           {
//                               float pos = (box.mPosition.z + box.mSize.z / 2);
//                               if (ai_attribute.attackpos < pos)
//                               {
//                                   ai_attribute.attackpos = pos;
//                               }
// 
//                           }
//                       }
// 
//                   }
//               }
//               else if (f.classname.Equals("move") || f.classname.Equals("force"))
//               {
//                   ai_attribute.movenum = (ai_attribute.movenum + f.vecParam0);    //记下位移 作为判断条件
//               }
//           }
//           
//           ai_item.cmdAttribute.Insert(0,ai_attribute);
//       }
    }
}
