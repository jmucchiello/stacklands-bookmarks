using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using CommonModNS;

namespace BookmarksModNS
{
    public enum ModifierKey { SHIFT, CONTROL, ALT }

    public partial class BookmarksMod : Mod
    {
        public static BookmarksMod instance;

        public static ModifierKey ModifierKey
        {
            get => instance?.modifierKey.Value ?? ModifierKey.SHIFT;
            private set
            {
                if (instance?.modifierKey != null)
                    instance.modifierKey.Value = value;
            }
        }

        public static bool JumpToZero
        {
            get => instance?.jumpToZero.Value ?? false;
            private set
            {
                if (instance?.jumpToZero != null)
                    instance.jumpToZero.Value = value;
            }
        }

        public Board CurrentBoard { get; private set; }

        public Dictionary<string, Board> boards = new();

        private ConfigToggledEnum<ModifierKey> modifierKey;
        private ConfigEntryBool jumpToZero;

        private void SetupConfiguration()
        {
            RuntimePlatform platform = Application.platform;
            modifierKey = new("bookmarksmod_modifierkey", Config, ModifierKey.SHIFT, new ConfigUI()
            {
                NameTerm = "bookmarksmod_modifierkey",
                TooltipTerm = "bookmarksmod_modifierkey_tooltip",
                OnUI = delegate (ConfigEntryBase b)
                {
                    if (Config.Data.TryGetValue("bookmarksmod_config_cntrl", out JToken oldValue))
                    {
                        if (oldValue.HasValues && oldValue.Value<bool>())
                        {
                            modifierKey.Value = ModifierKey.CONTROL;
                            Config.Data.Remove("bookmarksmod_config_cntrl");
                        }
                    }
                }
            })
            {
                currentValueColor = Color.blue,
                onDisplayEnumText = delegate (ModifierKey value)
                {
                    string plat = platform == RuntimePlatform.OSXPlayer ? "mac" : "win";
                    return I.Xlat($"bookmarksmod_modifier_{value}_{plat}");
                }
            };
            jumpToZero = new("bookmarksmod_jumptozero", Config, false, new()
            {
                NameTerm = "bookmarksmod_jumptozero",
                TooltipTerm = "bookmarksmod_jumptozero_tooltip"
            })
            {
                currentValueColor = Color.blue
            };
            Config.OnSave = ApplyConfig;
        }

        private void ApplyConfig()
        {
            ShiftKeys.Reset();
            ShiftKeys.defaultMode = modifierKey.Value;
        }

        public PlayList playList = new PlayList();
        //private Task<int> audioCount;
        private void LoadAudio()
        {
            int audioCount = playList.Start(Path + "\\Sounds");
        }

        private void Awake()
        {
            Logger.Log($"Path = {Path}");
            instance = this;
            LoadAudio();
            SetupConfiguration();
            WorldManagerPatches.GetSaveRound += WM_OnSave;
            WorldManagerPatches.LoadSaveRound += WM_OnLoad;
            WorldManagerPatches.StartNewRound += WM_OnNewGame;
            WorldManagerPatches.Update += WM_Update;
            WorldManagerPatches.ApplyPatches(Harmony);
            Harmony.PatchAll();
        }
        public override void Ready()
        {
            instance = this;
            ApplyConfig();
            Log($"Set mark using {ShiftKeys.defaultMode}");
            //            audioCount.Wait();
            //            Log($"Count = {audioCount.Result}");
            Log($"Ready!");
        }

        public void SetBoard(string id)
        {
            Log($"SetBoard({id})");
            if (boards.TryGetValue(id, out Board board))
            {
                CurrentBoard = board;
                if (JumpToZero)
                {
                    CurrentBoard.UpdateKeys();
                    PanAndZoom paz = board.marks.FirstOrDefault(x => x.key == Key.Digit0);
                    Log($"SetBoard {(paz != null ? paz.zoom.ToString() : "null")}");
                    paz?.Jump();
                    I.WM.CurrentBoard.StartCoroutine(jump(paz));
                }
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

        public static IEnumerator jump(PanAndZoom paz)
        {
            yield return null;
            paz?.Jump();
        }

        private readonly static List<string> traces = new List<string>();
        private void WM_Update(WorldManager _)
        {
            foreach (string s in traces)
                Log(s);
            traces.Clear();
        }
        public void Trace(string msg)
        {
            traces.Add(msg);
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
            foreach (SaveBoard sb in boards)
            {
                realBoards[sb.id] = sb.ToBoard();
            }
            return realBoards;
        }
    }
}
