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
                for (int i = 0; i < BookmarksMod.instance.CurrentBoard.marks.Length; ++i) 
                {
                    PanAndZoom pz = BookmarksMod.instance.CurrentBoard.marks[i];
                    if (__instance.GetKeyDown(pz.key))
                    {
                        ShiftKeys.Result result = ShiftKeys.TestShiftStatus(__instance);
                        try
                        {
                            switch (result)
                            {
                                case ShiftKeys.Result.None: pz.Jump(); break;
                                case ShiftKeys.Result.Meta: pz.Set(); break;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }
        }
    }
}
