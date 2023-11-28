using System;
using System.Collections.Generic;
using CommonModNS;
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
            if (I.WM.CanInteract && BookmarksMod.instance.CurrentBoard != null)
            {
                foreach (PanAndZoom pz in BookmarksMod.instance.CurrentBoard.marks)
                {
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
