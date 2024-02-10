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
    
}
