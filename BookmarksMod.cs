using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BookmarksModNS
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SaveMod
    {
        [JsonProperty]
        public Dictionary<string, SaveBoard> boards = new();

        public SaveMod(Dictionary<string, Board> realBoards)
        {
            if (realBoards == null) return;
            foreach (string key in realBoards.Keys)
            {
                boards[key] = new SaveBoard(realBoards[key]);
            }
        }

        public Dictionary<string, Board> ToMod()
        {
            Dictionary<string, Board> realBoards = new();
            foreach (string key in boards.Keys)
            {
                realBoards[key] = boards[key].ToBoard();
            }
            return realBoards;
        }
    }

    public class BookmarksMod : Mod
    {
        public static BookmarksMod instance;

        public Board CurrentBoard { get; private set; }

        public Dictionary<string, Board> boards = new();

        public List<Key> shiftMaskKeys = new();

        private void Awake()
        {
            instance = this;
            Harmony.PatchAll();
        }
        public override void Ready()
        {
            instance = this;
            Logger.Log($"Ready!");
        }

        public void SetBoard(string id)
        {
            Log($"SetBoard({id})");
            if (boards.TryGetValue(id, out Board board))
            {
                CurrentBoard = board;
            }
            else
            {
                boards[id] = CurrentBoard = new Board() { Id = id };
            }
        }

        public void Log(string msg)
        {
            Logger.Log(msg);
        }
    }
}