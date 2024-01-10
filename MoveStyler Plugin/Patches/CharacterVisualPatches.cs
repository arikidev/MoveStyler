using System;
using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using UnityEngine;

using MoveStyler.Data;
using MoveStylerMono;

namespace MoveStyler.Patches
{

    // Patching the SetMoveStyleVisualProps to show custom Movestyles

    [HarmonyPatch(typeof(Reptile.CharacterVisual), nameof(CharacterVisual.InitMoveStyleProps))]
    public class CharacterVisualInitMoveStylePropsPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Char Visual");

        public static void Postfix(ref CharacterVisual __instance)
        {
            DebugLog.LogMessage("***************  Patch Init MovestyleProps  ****************");

            //Create a base object to reference all movestyles
            CustomMoveStyleVisualParent parent = __instance.gameObject.AddComponent<CustomMoveStyleVisualParent>();
            if (parent == null) { DebugLog.LogMessage("CustomMoveSyleVisuals is null "); return; }

            for (int index = 1; index <= moveStyleDatabase.NewCharacterCount; index++)
            {
                DebugLog.LogMessage($"movestyleLoop: {index} ");

                CustomMoveStyle TempCustomStyle;

                MoveStyle style = (MoveStyle)(index + MoveStyle.MAX);
                moveStyleDatabase.GetCharacter(style, out TempCustomStyle);
                GameObject obj = TempCustomStyle.Visual;

                // For Each custom Movestyle create a customMoveStyleVisual
                CustomMoveStyleVisual moveStyleVisual = new CustomMoveStyleVisual();
                if (moveStyleVisual == null) { DebugLog.LogMessage("moveStyleVisual == null"); }

                foreach (KeyValuePair<MeshRenderer, string> prop in TempCustomStyle.Props)
                {
                    GameObject newObj = UnityEngine.Object.Instantiate(prop.Key.gameObject);

                    if (newObj == null) { DebugLog.LogMessage("newObj == null"); }

                    moveStyleVisual.AddPropObject(newObj, prop.Value);
                }

                //Store Custom Movestyle in CustomMoveStyleVisualParent
                parent.CustomMoveStylesList.Add(new KeyValuePair<MoveStyle, CustomMoveStyleVisual>(style, moveStyleVisual));

                DebugLog.LogMessage("Created new movestylePropObject ");
            }

            parent.SetupPropVisuals(__instance);
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterVisual), nameof(CharacterVisual.SetMoveStyleVisualProps))]
    public class CharacterSetMoveStyleVisualPropsPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Char Visual");

        public static void Postfix(ref CharacterVisual __instance, Player player, MoveStyle setMoveStyle, bool forceOff = false)
        {

            if (player != null)
            {
                //Get #CustomMoveStyles
                MoveStyle equippedStyle = (MoveStyle)player.GetField("moveStyleEquipped").GetValue(player);

                CustomMoveStyleVisualParent MovestyleVisualParent = __instance.GetComponent<CustomMoveStyleVisualParent>();
                if (MovestyleVisualParent == null) { DebugLog.LogMessage("PropList is Null"); return; };
                MovestyleVisualParent.SetCustomMoveStyleVisualsPropMode(player, setMoveStyle, forceOff);

            }
        }
    }


    [HarmonyPatch(typeof(Reptile.CharacterVisual), nameof(CharacterVisual.SetMoveStyleVisualAnim))]
    public class CharacterVisualSetMoveStyleVisualAnimPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Char Visual");

        public static void Postfix(ref CharacterVisual __instance, Player player, MoveStyle setMoveStyle, GameObject specialSkateboard = null)
        {

            if (setMoveStyle > MoveStyle.MAX)
            {
                Guid GUID; moveStyleDatabase.GetFirstOrConfigMoveStyleId(setMoveStyle, out GUID);
                CustomMoveStyle styleObj; moveStyleDatabase.GetCharacter(GUID, out styleObj);

                if (styleObj != null)
                {
                    //Set to use custom anim controller using
                    __instance.anim.runtimeAnimatorController = styleObj.AnimController;

                    //Set use handIK
                    __instance.GetField("handIKActiveR").SetValue(__instance, styleObj.Definition.UseHandIK);
                    __instance.GetField("handIKActiveL").SetValue(__instance, styleObj.Definition.UseHandIK);

                    DebugLog.LogMessage($"Set hand IK : {styleObj.Definition.UseHandIK}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterVisual), nameof(CharacterVisual.SetSpraycan))]
    public class CharacterVisualSetSpraycanPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Set Spraycan");

        public static void Postfix(ref CharacterVisual __instance, bool set, Characters c = Characters.NONE)
        {
            
            MoveStyle moveStyle = (MoveStyle)__instance.GetField("moveStyle").GetValue(__instance);

            if (moveStyle > MoveStyle.MAX)
            {
                Guid GUID; moveStyleDatabase.GetFirstOrConfigMoveStyleId(moveStyle, out GUID);
                CustomMoveStyle styleObj; moveStyleDatabase.GetCharacter(GUID, out styleObj);

                if (styleObj != null)
                {

                    bool setIK = !set && styleObj.Definition.UseHandIK;

                    //Set use handIK
                    __instance.GetField("handIKActiveR").SetValue(__instance, styleObj.Definition.UseHandIK);

                    DebugLog.LogMessage($"Set hand IK : {styleObj.Definition.UseHandIK}");
                }
            } 
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterVisual), nameof(CharacterVisual.SetPhone))]
    public class CharacterVisualSetPhonePatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Set Phone");

        public static void Postfix(ref CharacterVisual __instance, bool set)
        {

            MoveStyle moveStyle = (MoveStyle)__instance.GetField("moveStyle").GetValue(__instance);

            if (moveStyle > MoveStyle.MAX)
            {
                Guid GUID; moveStyleDatabase.GetFirstOrConfigMoveStyleId(moveStyle, out GUID);
                CustomMoveStyle styleObj; moveStyleDatabase.GetCharacter(GUID, out styleObj);

                if (styleObj != null)
                {
                    bool setIK = !set && styleObj.Definition.UseHandIK;

                    //Set use handIK
                    __instance.GetField("handIKActiveL").SetValue(__instance, styleObj.Definition.UseHandIK);

                    DebugLog.LogMessage($"Set hand IK : {styleObj.Definition.UseHandIK}");
                }
            }
        }
    }
}
