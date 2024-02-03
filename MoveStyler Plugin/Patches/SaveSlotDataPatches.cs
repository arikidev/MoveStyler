using HarmonyLib;
using Reptile;
using System;
using System.IO;

namespace MoveStyler.Patches
{
    [HarmonyPatch(typeof(Reptile.CharacterProgress), nameof(Reptile.CharacterProgress.Write))]
    public class CharacterProgressPatch
    {
        public static bool Prefix( ref CharacterProgress __instance)
        {
            MoveStyle movestyle = (MoveStyle)__instance.GetField("moveStyle").GetValue(__instance);

            if (movestyle > MoveStyle.MAX)
            {
                __instance.GetField("moveStyle").SetValue(__instance, MoveStyle.BMX);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterProgress), nameof(Reptile.CharacterProgress.Read))]
    public class CharacterProgressReadPatch
    {
        public static void Postfix( ref CharacterProgress __instance)
        {
            MoveStyle movestyle = (MoveStyle)__instance.GetField("moveStyle").GetValue(__instance);

            if (movestyle > MoveStyle.MAX)
            {
                __instance.GetField("moveStyle").SetValue(__instance, MoveStyle.BMX);
            }
        }
    }

    /* 
    [HarmonyPatch(typeof(Reptile.SaveSlotData), nameof(Reptile.SaveSlotData.GetCharacterProgress))]
    public class SaveSlotGetProgressPatch
    {
        public static bool Prefix(Characters character, ref CharacterProgress __result)
        {
            


            if (character != null)
            {
                if (CharacterSaveSlots.GetCharacterData(character, out CharacterProgress data))
                    {
                        __result = data;
                    }
                
                
                if (moveStyleDatabase.GetFirstOrConfigMoveStyleId(character, out Guid guid))
                {
                    
                }
                return false;
            }
            return true;
        }
    }
   */

}
