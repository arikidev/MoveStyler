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

        [HarmonyPriority(399)] //AfterCrewBoom
        public static void Postfix(Characters character, ref CharacterProgress __result)
        {
            Guid characterGuid;

            //DebugLog.LogMessage($"get CharacterProgress {character} ");

            if (CharUtil.GetGuidForCharacters(character, out characterGuid))
            {
                if (CharacterSaveSlots.GetCharacterData(characterGuid, out CharacterProgress data, character))
                {
                    if (data == null) { DebugLog.LogMessage("data was null"); return; }

                    if (moveStyleDatabase.HasMovestyle(data.moveStyle) || data.moveStyle < MoveStyle.MAX)
                    {
                        //Override Savedata with movestyle and movestyleskin
                        if (__result != null)
                        {
                            __result.moveStyle = data.moveStyle;
                            __result.moveStyleSkin = data.moveStyleSkin;
                            data.unlocked = __result.unlocked;
                            data.version = __result.version;
                        }
                    }
                    //__result = data;
                    //DebugLog.LogMessage($"get charData {data.character} | movestyle {data.moveStyle}");
                    
                    return;
                } 
            }
            return;
        }
    }

    
    [HarmonyPatch(typeof(Reptile.CharacterProgress), nameof(Reptile.CharacterProgress.Write))]
    public class CharacterProgressPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} SaveSlotWritePatch");

        [HarmonyPriority(399)]
        public static void Prefix(ref CharacterProgress __instance, BinaryWriter writer, out MoveStyle __state)
        {
            __state = __instance.moveStyle;

            if (__instance is CustomCharacterProgress) { return; }

            if ((int)__instance.moveStyle > 5) 
            {
                //Set to default movestyle for character
                __instance.moveStyle = new CharacterProgress(__instance.character).moveStyle;
            }
        }

        [HarmonyPriority(399)] 
        public static void Postfix(ref CharacterProgress __instance, BinaryWriter writer, MoveStyle __state)
        {
            __instance.moveStyle = __state;
            
        }


    }
    
}
