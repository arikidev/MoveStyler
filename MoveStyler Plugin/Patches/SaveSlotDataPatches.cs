using HarmonyLib;
using Reptile;
using System;
using System.IO;
using BepInEx.Logging;

namespace MoveStyler.Patches
{
    [HarmonyPatch(typeof(Reptile.SaveSlotData), nameof(Reptile.SaveSlotData.GetCharacterProgress))]
    public class SaveSlotGetProgressPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} SaveSlotGetProgressPatch");

        [HarmonyPriority(900)]
        public static void Postfix(Characters character, ref CharacterProgress __result)
        {
            
            CharacterProgress baseProgress = __result;

            Guid characterGuid;

            if (CharUtil.GetGuidForCharacters(character, out characterGuid))
            {
                if (CharacterSaveSlots.GetCharacterData(characterGuid, out CharacterProgress data))
                {
                    if (data == null) { DebugLog.LogMessage("data was null"); return; }

                    if (moveStyleDatabase.HasMovestyle(data.moveStyle) || data.moveStyle < MoveStyle.MAX)
                    {

                        if (baseProgress != null)
                        {
                            data.unlocked = baseProgress.unlocked;
                            data.version = baseProgress.version;
                        }
                        __result = data;
                        return;
                    }  
                } 
            }
            return;
        }
    }
}
