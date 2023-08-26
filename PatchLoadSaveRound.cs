using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Newtonsoft.Json;

namespace BookmarksModNS
{
    [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.LoadSaveRound))]
    internal class PatchLoadSaveRound
    {
        public static void Prefix(WorldManager __instance, SaveRound saveRound)
        {
            try
            {
                SerializedKeyValuePair pair = SerializedKeyValuePairHelper.GetWithKey(saveRound.ExtraKeyValues, "blueprintmod_runtime");
                if (pair != null)
                {
                    BookmarksMod.instance.Log($"LoadSaveRound Save Data:\r\n{pair.Value}");
                    BookmarksMod.instance.boards = JsonConvert.DeserializeObject<SaveMod>(pair.Value)!.ToMod();
                    BookmarksMod.instance.Log($"LoadSaveRound Save Data Loaded");
                }
            }
            catch (Exception e)
            {
                BookmarksMod.instance.Log($"SaveModData encountered an exception");
                BookmarksMod.instance.Log(e.StackTrace);
            }
        }

        public static void Postfix(WorldManager __instance, SaveRound saveRound)
        {
            BookmarksMod.instance.Log("LoadSaveRound calling SetBoard");
            BookmarksMod.instance.SetBoard(__instance.CurrentBoard.Id);
        }
    }

    [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.GetSaveRound))]
    internal class PatchGetSaveRound
    {
        public static void Postfix(WorldManager __instance, SaveRound __result)
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Include;
                using (TextWriter tw = new StringWriter())
                {
                    using (JsonWriter writer = new JsonTextWriter(tw))
                    {
                        serializer.Serialize(writer, new SaveMod(BookmarksMod.instance.boards));
                    }
                    BookmarksMod.instance.Log($"GetSaveRound Save Data:\r\n{tw}");
                    SerializedKeyValuePairHelper.SetOrAdd(__result.ExtraKeyValues, "blueprintmod_runtime", tw.ToString());
                }
            }
            catch (Exception e)
            {
                BookmarksMod.instance.Log($"LoadModData encountered an exception\r\n{e.StackTrace}");
            }
        }
    }
}