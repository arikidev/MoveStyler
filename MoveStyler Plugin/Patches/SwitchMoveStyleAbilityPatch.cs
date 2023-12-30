using System;
using System.IO;
using Reptile;
using UnityEngine;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;

using MoveStyler.Data;

namespace MoveStyler.Patches
{

    [HarmonyPatch(typeof(Reptile.SwitchMoveStyleAbility), nameof(SwitchMoveStyleAbility.OnStartAbility))]
    public class MoveSwitchOnStartAbility
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Ability SwitchStyle");

        public static bool Prefix(ref SwitchMoveStyleAbility __instance)
        {
            
            MoveStyle style =  (MoveStyle)__instance.p.GetField("moveStyle").GetValue(__instance.p);
            if (style > MoveStyle.MAX)
            {
                DebugLog.LogMessage("AbilitySwitch is Custom");
                //return false;
            }

            DebugLog.LogMessage("AbilitySwitch is Default");
            return true;
            
        }
    }

}
