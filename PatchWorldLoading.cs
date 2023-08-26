using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace BookmarksModNS
{
    [HarmonyPatch(typeof(WorldManager),"GoToBoard")]
    public class PatchGoToBoard
    {
        public static void Postfix(WorldManager __instance, GameBoard newBoard, Action onComplete, string transitionId)
        {
            BookmarksMod.instance.Log($"GoToBoard({newBoard.Id}) calling SetBoard");
            BookmarksMod.instance.SetBoard(newBoard.Id);
        }
    }

    [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.StartNewRound))]
    internal class PatchStartNewRound
    {
        public static void Postfix(WorldManager __instance)
        {
            BookmarksMod.instance.boards = new();
            BookmarksMod.instance.SetBoard(global::Board.Mainland);
        }
    }
}
