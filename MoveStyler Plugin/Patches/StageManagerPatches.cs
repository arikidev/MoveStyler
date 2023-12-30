using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using System;

namespace MoveStyler.Patches
{
    /*
    [HarmonyPatch(typeof(Reptile.StageManager), "GetPlayerCharacter")]
    public class SetupPatch
    {
        public static bool Prefix(ref Characters __result)
        {
            Guid lastPlayedCharacter = CharacterSaveSlots.CurrentSaveSlot.LastPlayedCharacter;
            if (lastPlayedCharacter != Guid.Empty)
            {
                if (CharacterDatabase.GetCharacterValueFromGuid(lastPlayedCharacter, out Characters character))
                {
                    __result = character;
                    return false;
                }
            }

            return true;
        }
    }
    */
}
