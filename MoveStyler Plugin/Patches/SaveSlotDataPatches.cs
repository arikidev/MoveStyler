using HarmonyLib;
using Reptile;
using System;

namespace MoveStyler.Patches
{
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
