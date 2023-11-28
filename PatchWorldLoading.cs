using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine.InputSystem;
using CommonModNS;

namespace BookmarksModNS
{
    [HarmonyPatch(typeof(WorldManager),"GoToBoard")]
    public class PatchGoToBoard
    {
        public static void Prefix(WorldManager __instance, GameBoard newBoard, Action onComplete, string transitionId)
        {
            BookmarksMod.instance.GoToBoard = newBoard.Id;
        }
    }
}
