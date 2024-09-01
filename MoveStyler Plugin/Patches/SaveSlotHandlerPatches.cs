using System;
using System.Collections.Generic;
using System.IO;
using Reptile;
using HarmonyLib;
using BepInEx.Logging;

namespace MoveStyler.Patches
{

    [HarmonyPatch(typeof(Reptile.SaveSlotHandler), nameof(Reptile.SaveSlotHandler.SetCurrentSaveSlotDataBySlotId))]
    public class SaveSlotHandlerLoadPatch
    {
        [HarmonyPriority(100)]
        public static void Postfix(int saveSlotId)
        {
            CharacterSaveSlots.LoadSlot(saveSlotId);
        }
    }

    /*
    [HarmonyPatch(typeof(Reptile.SaveSlotHandler), "SaveSaveSlot")]
    public class SaveSlotHandlerSaveSaveSlotPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} StyleDatabase");
        public static void Prefix(int saveSlotId, ref SaveSlotData ___currentSaveSlot)
        {
            CharacterProgress prog =___currentSaveSlot.GetCharacterProgress(___currentSaveSlot.currentCharacter);

            DebugLog.LogMessage($"SaveSaveSlotPatch");
            DebugLog.LogMessage($"Current CharacterProgress Movestyle: {prog.moveStyle}");

        }
    }
    */

    /**
    [HarmonyPatch(typeof(Reptile.SaveManager), nameof(Reptile.SaveManager.SaveCurrentSaveSlot))]
    public class SaveManagerPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} StyleDatabase");

        [HarmonyPriority(900)]
        public static bool Prefix(SaveManager __instance, out MoveStyle __state)
        {
            __state = MoveStyle.SKATEBOARD;
            
            //if ((bool)__instance.GetField("isAI").GetValue(__instance)) { return true; }

            DebugLog.LogMessage("SaveManagerPatch");
            
            Characters character = __instance.CurrentSaveSlot.currentCharacter;
            CharacterProgress data = __instance.CurrentSaveSlot.GetCharacterProgress(character);

            DebugLog.LogMessage($"Prefix data: {data.moveStyle}");

            if (data != null) 
            { 
                __state = data.moveStyle; 
            }
            
            if (CharUtil.GetGuidForCharacters(character, out Guid guid))
            {
                CharacterSaveSlots.CurrentSaveSlot.LastPlayedCharacter = guid;
            }

            CharacterSaveSlots.SaveSlot();

            //Override Equiptmovestyle 
            if ((int)data.moveStyle > 5)
            {
                //Set to default movestyle for character

                SaveSlotData slot = __instance.CurrentSaveSlot;
                for (int i = 0; i < slot.totalCharacterProgress.Length; i++)
                {
                    if (slot.totalCharacterProgress[i].character == data.character)
                    {
                        slot.totalCharacterProgress[i].moveStyle = new CharacterProgress(data.character).moveStyle;
                    }
                }
            }

            return true;
        }


        //Add postfix to re-set the movestyle
        [HarmonyPriority(399)]
        public static void Postfix(SaveManager __instance, MoveStyle __state)
        {
            DebugLog.LogMessage("SaveManagerPatch Postfix");
            //DebugLog.LogMessage($"base ms - {__instance.moveStyleEquipped}");

            //bool isAI = (bool)__instance.GetField("isAI").GetValue(__instance);
            //if (isAI) { return; }

            Characters character = __instance.CurrentSaveSlot.currentCharacter;
            CharacterProgress data = __instance.CurrentSaveSlot.GetCharacterProgress(character);

            DebugLog.LogMessage($"Postfix data: {data.moveStyle}");
            //data.moveStyle = __state;

            SaveSlotData slot = __instance.CurrentSaveSlot;
            for (int i = 0; i < slot.totalCharacterProgress.Length; i++)
            {
                if (slot.totalCharacterProgress[i].character == data.character)
                {
                    slot.totalCharacterProgress[i].moveStyle = new CharacterProgress(data.character).moveStyle;
                }
            }

        }

    }
    **/
}
