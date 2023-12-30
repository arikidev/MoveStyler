using HarmonyLib;
using Reptile;
using System;

namespace MoveStyler.Patches
{
    /*
    [HarmonyPatch(typeof(Reptile.StyleSwitchMenu), nameof(Reptile.StyleSwitchMenu.SkinButtonClicked))]
    public class StyleSwitchMenuPatch
    {
        public static void Postfix()
        {
            Player currentPlayer = WorldHandler.instance.GetCurrentPlayer();
            Characters character = (Characters)currentPlayer.GetField("character").GetValue(currentPlayer);
            if (character > Characters.MAX)
            {
                if (CharacterDatabase.GetFirstOrConfigCharacterId(character, out Guid guid))
                {
                    CharacterSaveSlots.SaveCharacterData(guid);
                }
            }
        }
    }
    */
}
