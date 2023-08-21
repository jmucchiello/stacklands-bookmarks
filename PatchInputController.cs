using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace BookmarksModNS
{
    [HarmonyPatch(typeof(InputController),"Update")]
    internal class PatchInputController 
    {
        public static void Postfix(InputController __instance)
        {
            if (WorldManager.instance.CanInteract && BookmarksMod.instance.CurrentBoard is not null)
            {
//                BookmarksMod.instance.SetBoard(WorldManager.instance.GetCurrentBoardSafe().Id);
                for (int i = 0; i < BookmarksMod.instance.CurrentBoard.marks.Length; ++i) 
                {
                    PanAndZoom pz = BookmarksMod.instance.CurrentBoard.marks[i];
                    if (!__instance.GetKey(Key.LeftAlt) && !__instance.GetKey(Key.RightAlt) &&
                        !__instance.GetKey(Key.LeftApple) && !__instance.GetKey(Key.RightApple) &&
                        !__instance.GetKey(Key.LeftCommand) && !__instance.GetKey(Key.RightCommand) &&
                        !__instance.GetKey(Key.LeftCtrl) && !__instance.GetKey(Key.RightCtrl)
                       )
                    {
                        if (__instance.GetKeyDown(pz.key) && (__instance.GetKey(Key.LeftShift) || __instance.GetKey(Key.RightShift)))
                        {
                            pz.Set();
                        }
                        else if (__instance.GetKeyDown(pz.key))
                        {
                            pz.Jump();
                        }
                    }
                }
            }
        }
    }
}
