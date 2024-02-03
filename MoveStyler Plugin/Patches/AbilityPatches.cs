using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;
using BepInEx.Logging;
using MoveStyler;

namespace MoveStyler.Patches
{

    [HarmonyPatch(typeof(Reptile.SlideAbility))]
    class SlideAbilityPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} AnimationEventRelayPatch");

        [HarmonyPrefix]
        [HarmonyPatch("IsPerformingManual", MethodType.Getter)]
        static void IsPerformingManualGetPatch(SlideAbility __instance, ref bool __result)
        {
            MoveStyle moveStyle = (MoveStyle)__instance.p.GetField("moveStyle").GetValue(__instance.p);
            //ADD ability to toggle this off per movestyle

            __result = __result || moveStyle > MoveStyle.MAX;
        }

        /*
        [HarmonyPrefix]
        [HarmonyPatch("SetSlideState")]
        static void SetSlideStatePatch(SlideAbility.SlideState setState)
        {
            //DebugLog.LogMessage($"{setState}");


        
        
        }
        */

    }
}
