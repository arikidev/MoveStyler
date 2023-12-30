using System;
using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using UnityEngine;

using MoveStyler.Data;
using MoveStylerMono;

namespace MoveStyler.Patches
{

    // Patching the SetMoveStyleVisualProps to show custom Movestyles

    
    [HarmonyPatch(typeof(Reptile.CharacterVisual), nameof(CharacterVisual.SetMoveStyleVisualProps))]
    public class CharacterSetMoveStyleVisualPropsPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Char Visual");

        public static void Postfix( ref CharacterVisual __instance ,Player player, MoveStyle setMoveStyle, bool forceOff = false)
        {
            DebugLog.LogMessage(String.Format("Patch Set Prop: force? {0} ", forceOff ));

            if (player != null)
            {
                //Get #CustomMoveStyles
                MoveStyle equippedStyle = (MoveStyle)player.GetField("moveStyleEquipped").GetValue(player);

                // Process Custom MoveStyles
                DebugLog.LogMessage("Trying to set custom movement prop Mode");
                moveStyleDatabase.SetCustomMoveStylePropMode(player, setMoveStyle, forceOff);
            }
        }
    }
    

    [HarmonyPatch(typeof(Reptile.CharacterVisual), nameof(CharacterVisual.SetMoveStyleVisualAnim))]
    public class CharacterVisualSetMoveStyleVisualAnimPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Char Visual");

        public static void Postfix( ref CharacterVisual __instance, Player player, MoveStyle setMoveStyle, GameObject specialSkateboard = null)
        {

            if (setMoveStyle > MoveStyle.MAX)
            {
                Guid GUID; moveStyleDatabase.GetFirstOrConfigMoveStyleId(setMoveStyle, out GUID);
                CustomMoveStyle styleObj; moveStyleDatabase.GetCharacter(GUID, out styleObj);

                if (styleObj != null)
                {
                    //Set to use custom anim controller using
                    __instance.anim.runtimeAnimatorController = styleObj.AnimController;
                }

                DebugLog.LogMessage("MoveStyle Custom overriding");
                //return false;
            }

            //DebugLog.LogMessage("MoveStyle Default");
            //return true;
        }
    }
}
