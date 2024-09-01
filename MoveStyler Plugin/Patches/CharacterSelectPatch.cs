
using HarmonyLib;
using Reptile;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Playables;
using UnityEngine;
using MoveStyler;

namespace MoveStyler.Patches
{
    // Dont really need this should remove


    [HarmonyPatch(typeof(Reptile.CharacterSelect))]
    public class CharacterSelectSetPlayerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetPlayerToCharacter")]
        public static void SetPlayerToCharacterPostfix(Player ___player, Characters __state)
        {

            if (!CharUtil.GetGuidForCharacters(__state, out Guid guid)) { return; }

            if (CharacterSaveSlots.GetCharacterData(guid, out CharacterProgress characterProgress, __state))
            {
                if (moveStyleDatabase.HasMovestyle(characterProgress.moveStyle))
                {
                    ___player.SetCurrentMoveStyleEquipped(characterProgress.moveStyle);
                }
            }
        }

    }

}
