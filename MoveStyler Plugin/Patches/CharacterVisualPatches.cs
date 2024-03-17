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
            //DebugLog.LogMessage("***************  Patch Init MovestyleProps  ****************");

            //Create a base object to reference all movestyles. Attach it to the AnimObject to allow for anim relay events
            CustomMoveStyleVisualParent parent = __instance.anim.gameObject.AddComponent<CustomMoveStyleVisualParent>();
            if (parent == null) { DebugLog.LogMessage("CustomMoveSyleVisuals is null "); return; }

            for (int index = 1; index <= moveStyleDatabase.NewMovestyleCount; index++)
            {
                //DebugLog.LogMessage($"movestyleLoop: {index} ");

                CustomMoveStyle TempCustomStyle;

                MoveStyle style = (MoveStyle)(index + MoveStyle.MAX);
                moveStyleDatabase.GetCharacter(style, out TempCustomStyle);
                GameObject obj = TempCustomStyle.Visual;

                // For Each custom Movestyle create a customMoveStyleVisual
                CustomMoveStyleVisual moveStyleVisual = new CustomMoveStyleVisual();
                if (moveStyleVisual == null) { DebugLog.LogMessage("moveStyleVisual == null"); }

                foreach (KeyValuePair<GameObject, string> prop in TempCustomStyle.Props)
                {
                    GameObject newObj = UnityEngine.Object.Instantiate(prop.Key);

                    if (newObj == null) { DebugLog.LogMessage("newObj == null"); }

                    moveStyleVisual.AddPropObject(newObj, prop.Value);
                }
                //Todo add ability to scale props

                //Store Custom Movestyle in CustomMoveStyleVisualParent
                parent.CustomMoveStylesList.Add(new KeyValuePair<MoveStyle, CustomMoveStyleVisual>(style, moveStyleVisual));

                //DebugLog.LogMessage("Created new movestylePropObject ");
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

                CustomMoveStyleVisualParent MovestyleVisualParent = CustomMoveStyleVisualParent.GetCustomMoveStyleVisualParent(__instance);
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

                CustomMoveStyle styleObj; moveStyleDatabase.GetCharacter(setMoveStyle, out styleObj);

                if (styleObj != null)
                {
                    //Set to use custom anim controller using
                    __instance.anim.runtimeAnimatorController = styleObj.AnimController;

                    //Set use handIK
                    __instance.GetField("handIKActiveR").SetValue(__instance, styleObj.Definition.UseHandRIK);
                    __instance.GetField("handIKActiveL").SetValue(__instance, styleObj.Definition.UseHandLIK);

                    CustomMoveStyleVisualParent parent = CustomMoveStyleVisualParent.GetCustomMoveStyleVisualParent(__instance);
                    parent.LHandIKCurrent = styleObj.Definition.UseHandLIK;
                    parent.RHandIKCurrent = styleObj.Definition.UseHandRIK;

                    //DebugLog.LogMessage($"Set hand IK : {styleObj.Definition.UseHandRIK} | {styleObj.Definition.UseHandLIK}");
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
                CustomMoveStyle styleObj; moveStyleDatabase.GetCharacter(moveStyle, out styleObj);

                if (styleObj != null)
                {

                    CustomMoveStyleVisualParent parent = CustomMoveStyleVisualParent.GetCustomMoveStyleVisualParent(__instance);

                    bool setIK = false;

                    if (set) { setIK = false; }
                    else { setIK = parent.RHandIKCurrent; }

                    //Set use handIK
                    __instance.GetField("handIKActiveR").SetValue(__instance, setIK);

                    //DebugLog.LogMessage($"Set hand IK : {setIK}");
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
                CustomMoveStyle styleObj; moveStyleDatabase.GetCharacter(moveStyle, out styleObj);

                if (styleObj != null)
                {
                    CustomMoveStyleVisualParent parent = CustomMoveStyleVisualParent.GetCustomMoveStyleVisualParent(__instance);

                    bool setIK = false;

                    if (set) { setIK = false; }
                    else { setIK = parent.LHandIKCurrent; }


                    //Set use handIK
                    __instance.GetField("handIKActiveL").SetValue(__instance, setIK);

                    //DebugLog.LogMessage($"Set hand IK : {setIK}");
                }
            }
        }
    }
}
