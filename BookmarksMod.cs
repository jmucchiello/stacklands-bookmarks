using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BookmarksModNS
{
    public class BookmarksMod : Mod
    {
        public static BookmarksMod instance;

        public Board CurrentBoard { get; private set; }

        public Dictionary<string, Board> boards = new();

        public ConfigEntry<bool> useControl;

        private void SetupConfiguration()
        {
            RuntimePlatform platform = Application.platform;
            string nameTerm = platform switch
            {
                RuntimePlatform.WindowsPlayer => "bookmarksmod_config_cntrl_pc",
                RuntimePlatform.OSXPlayer => "bookmarksmod_config_cntrl_mac",
                _ => "bookmarksmod_config_cntrl_pc",
            };

            useControl = Config.GetEntry<bool>("bookmarksmod_config_cntrl", false, new ConfigUI() { NameTerm = nameTerm });
            useControl.OnChanged = (bool value) => {
                ShiftKeys.defaultMode = (value) ? ShiftKeys.Mode.Ctrl : ShiftKeys.defaultMode = ShiftKeys.Mode.Shift;
                ShiftKeys.Reset();
                BookmarksMod.instance.Log($"Set mark using {ShiftKeys.defaultMode}");
            };
        }

        private void Awake()
        {
            instance = this;
            SetupConfiguration();
            Harmony.PatchAll();
        }
        public override void Ready()
        {
            instance = this;
            Logger.Log($"Ready!");
            ShiftKeys.defaultMode = useControl.Value ? ShiftKeys.Mode.Ctrl : ShiftKeys.defaultMode = ShiftKeys.Mode.Shift;
            BookmarksMod.instance.Log($"Set mark using {ShiftKeys.defaultMode}");
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

    [JsonObject(MemberSerialization.OptIn)]
    public class SaveMod
    {
        [JsonProperty]
#pragma warning disable 414
        private int version = 1;
#pragma warning restore 414
        [JsonProperty]
        public List<SaveBoard> boards = new();

        public SaveMod(Dictionary<string, Board> realBoards)
        {
            if (realBoards == null) return;
            foreach (string key in realBoards.Keys)
            {
                boards.Add(new SaveBoard(realBoards[key]));
            }
        }

        public Dictionary<string, Board> ToMod()
        {
            Dictionary<string, Board> realBoards = new();
            foreach (SaveBoard n in boards)
            {
                realBoards[n.id] = n.ToBoard();
            }
            return realBoards;
        }
    }
}
