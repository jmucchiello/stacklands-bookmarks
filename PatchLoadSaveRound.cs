using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Newtonsoft.Json;
using CommonModNS;

namespace BookmarksModNS
{
    public partial class BookmarksMod : Mod
    {
        private void WM_OnLoad(WorldManager wm, SaveRound saveRound)
        {
            try
            {
                SerializedKeyValuePair pair = SerializedKeyValuePairHelper.GetWithKey(saveRound.ExtraKeyValues, "blueprintmod_runtime");
                if (pair != null)
                {
                    Log($"LoadSaveRound Save Data:\r\n{pair.Value}");
                    boards = JsonConvert.DeserializeObject<SaveMod>(pair.Value)!.ToMod();
                    Log($"LoadSaveRound Save Data Loaded");
                }
            }
            catch (Exception e)
            {
                Log($"SaveModData encountered an exception");
                Log(e.StackTrace);
            }
            Log("LoadSaveRound calling SetBoard");
            SetBoard(wm.CurrentBoard.Id);
        }

        private void WM_OnSave(WorldManager wm, SaveRound saveRound)
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Include;
                using (TextWriter tw = new StringWriter())
                {
                    using (JsonWriter writer = new JsonTextWriter(tw))
                    {
                        serializer.Serialize(writer, new SaveMod(boards));
                    }
                    Log($"GetSaveRound Save Data:\r\n{tw}");
                    SerializedKeyValuePairHelper.SetOrAdd(saveRound.ExtraKeyValues, "blueprintmod_runtime", tw.ToString());
                }
            }
            catch (Exception e)
            {
                Log($"LoadModData encountered an exception\r\n{e.StackTrace}");
            }
        }

        private void WM_OnNewGame(WorldManager wm)
        {
            boards = new();
            SetBoard(global::Board.Mainland);
        }
    }
}