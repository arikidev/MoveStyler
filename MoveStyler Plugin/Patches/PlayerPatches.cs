﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Reptile;
using UnityEngine;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using MoveStyler;

using MoveStyler.Data;

namespace MoveStyler.Patches
{


    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.SetCurrentMoveStyleEquipped))]
    [HarmonyBefore("sgiygas.crewBoom")]
    public class PlayerSetCurrentMoveStyleEquippedPatch
    {

        public static void Postfix(ref MoveStyle setMoveStyleEquipped)
        {
            //Clean output to prevent CrewBoom from saving a customMovestyles
            if (setMoveStyleEquipped > MoveStyle.MAX)
            {
                setMoveStyleEquipped = MoveStyle.ON_FOOT;
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.Player), "SetMoveStyle")]
    public class PlayerSetMoveStylePatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Player Patches");

        public static bool Prefix(ref Player __instance, MoveStyle setMoveStyle, bool changeProp, bool changeAnim, GameObject specialSkateboard = null)
        {

            if (setMoveStyle > MoveStyle.MAX)
            {

                CharacterVisual characterVisual = (CharacterVisual)__instance.GetField("characterVisual").GetValue(__instance);

                if (changeAnim)
                {
                    int curAnim = (int)__instance.GetField("curAnim").GetValue(__instance);

                    characterVisual.SetMoveStyleVisualAnim(__instance, setMoveStyle, specialSkateboard);
                    if (curAnim != 0)
                    {
                        int newAnim = curAnim;
                        __instance.GetField("curAnim").SetValue(__instance, 0);
                        __instance.PlayAnim(newAnim, false, false, -1f);
                    }
                }

                //Set MoveStyle enum on Character
                __instance.GetField("moveStyle").SetValue(__instance, setMoveStyle);

                //Apply Visual Change to CustomMoveStyle
                if (changeProp)
                {
                    characterVisual.SetMoveStyleVisualProps(__instance, setMoveStyle, false);
                }

                //Set MovementStats
                CustomMoveStyle customMoveStyle;
                moveStyleDatabase.GetCharacter(setMoveStyle, out customMoveStyle);
                //ToDo Set this up correctly
                customMoveStyle.setCustomMovementStats(__instance);

                return false; // Skip is Enum is Custom
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.InitAnimation))]
    public class PlayerInitAnimationPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Player Patches");

        public static void Postfix(ref Player __instance)
        {

            MoveStyle originalStyle = (MoveStyle)__instance.GetField("moveStyle").GetValue(__instance);

            //Get Animinfo set
            Dictionary<int, Player.AnimInfo>[] animInfoSets = (Dictionary<int, Player.AnimInfo>[])__instance.GetField("animInfosSets").GetValue(__instance);

            //Resize Array to match new custom styles
            Array.Resize(ref animInfoSets, animInfoSets.Length + moveStyleDatabase.NewMovestyleCount + 1);
            __instance.GetField("animInfosSets").SetValue(__instance, animInfoSets);

            for (int id = 1; id <= moveStyleDatabase.NewMovestyleCount; id++)
            {
                
                MoveStyle moveStyle = id + MoveStyle.MAX;
                
                //Add a new Dictionary to the animInfo Array
                animInfoSets[(int)moveStyle] =  new Dictionary<int, Player.AnimInfo>();

                if (animInfoSets.Length < (int)moveStyle)
                {
                    DebugLog.LogWarning("Anim Info2 Array Length is not long enough");
                    return;
                }

                //Get CustomMovestyleObj from MoveStyle Int
                CustomMoveStyle styleObj; moveStyleDatabase.GetCharacter(moveStyle, out styleObj);

                //Set anim info for the current custom movestyle
                styleObj.InitCustomAnimInfo(__instance, moveStyle);
            }

            //Return to default movestyle
            __instance.GetField("moveStyle").SetValue(__instance, originalStyle);

        }
    }

    // Update Animation Patch to send through Player Speed to the Animator
    [HarmonyPatch(typeof(Reptile.Player), "UpdateAnim")]
    public class PlayerUpdateAnimPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Player Patches");

        public static void Postfix(ref Player __instance)
        {
            float speed = __instance.GetForwardSpeed();
            CharacterVisual charVis = (CharacterVisual)__instance.GetField("characterVisual").GetValue(__instance);
            Animator anim = charVis.anim;

            charVis.anim.SetFloat("forwardSpeed", speed);
        }

    }

    // Allow induvidual animations override the HandIk settings.
    [HarmonyPatch(typeof(Reptile.Player), "PlayAnim")]
    public class PlayerPlayAnimPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Player Patches");

        public static void Postfix(ref Player __instance, int newAnim, Dictionary<int, Player.AnimInfo>[]  ___animInfosSets, ref CharacterVisual ___characterVisual, bool forceOverwrite = false, bool instant = false, float atTime = -1f)
        {
            if (__instance == null) { DebugLog.LogMessage($"player instance is null"); return; }

            MoveStyle style = (MoveStyle)__instance.GetField("moveStyle").GetValue(__instance);

            if  (style > MoveStyle.MAX)
            {
                CustomMoveStyle customMovestyle; moveStyleDatabase.GetCharacter(style, out customMovestyle);
                CharacterVisual charVis = (CharacterVisual)__instance.GetField("characterVisual").GetValue(__instance);

                if (charVis == null || customMovestyle == null) { DebugLog.LogMessage($"character visual or customMovestyle is null"); return; }

                bool lHandIK = customMovestyle.Definition.UseHandLIK;
                bool rHandIK = customMovestyle.Definition.UseHandRIK;

                // If Custom animInfo is preset check IK tags
                if (customMovestyle.customAnimInfoDict.ContainsKey(newAnim))
                {
                    lHandIK = customMovestyle.customAnimInfoDict[newAnim].lHandIKOverride ^ customMovestyle.Definition.UseHandLIK;
                    rHandIK = customMovestyle.customAnimInfoDict[newAnim].rHandIKOverride ^ customMovestyle.Definition.UseHandRIK;
                }

                CustomMoveStyleVisualParent parent = CustomMoveStyleVisualParent.GetCustomMoveStyleVisualParent(charVis);
                if (parent == null) { DebugLog.LogMessage($"CustomMoveStyleVisualParent is null"); return; }

                parent.LHandIKCurrent = lHandIK;
                parent.RHandIKCurrent = rHandIK;

                //Dictionary<int,Player.AnimInfo> animInfos = ___animInfosSets[(int)style];
                //DebugLog.LogMessage($"FeetIK = {___characterVisual.feetIK}");
                //___characterVisual.feetIK = animInfos[newAnim].feetIK;
            }
        }
    }

    //A really dumb fix for other mod crashes
    [HarmonyPatch(typeof(Reptile.Player), "DoTrick")]
    public class PlayerDoTrick
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Player Patches");

        [HarmonyPriority(701)] //Manually Setting a Higher Priority to fix issues with null inputs
        public static void Prefix(Player __instance, ref string trickName, int trickNum = 0, Player.TrickType type = Player.TrickType.NONE)
        {
            //DebugLog.LogMessage("****************Patching Do Trick*****************");
            if (trickName == null)
            {
                trickName = "";
            }
        }
    }

    /*
    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.SetCharacter))]
    public class PlayerInitOverridePatch
    {
        public static void Prefix(ref Characters setChar)
        {
            if (CharacterDatabase.HasCharacterOverride)
            {
                if (CharacterDatabase.GetCharacterValueFromGuid(CharacterDatabase.CharacterOverride, out Characters character))
                {
                    if (character > Characters.MAX)
                    {
                        setChar = character;
                    }
                }
            }
        }

        public static void Postfix(Player __instance, Characters setChar)
        {
            if (CharacterDatabase.HasCharacterOverride)
            {
                CharacterDatabase.SetCharacterOverrideDone();
            }

            if (__instance == WorldHandler.instance.GetCurrentPlayer())
            {
                if (CharacterDatabase.GetCharacter(setChar, out CustomCharacter character))
                {
                    var info = new CrewBoomAPI.CharacterInfo(character.Definition.CharacterName, character.Definition.GraffitiName);
                    CrewBoomAPIDatabase.UpdatePlayerCharacter(info);
                }
                else
                {
                    CrewBoomAPIDatabase.UpdatePlayerCharacter(null);
                }
            }
        }
    }
    */

    //To Do switch to a system that request movestyle switching by name
    /**
    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.SetCurrentMoveStyleEquipped))]
    public class PlayerSetMovestyleEquipped
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Player Patches");

        
        /**
        public static void Postfix(Player __instance, MoveStyle setMoveStyleEquipped)
        {
            bool isAi = (bool) __instance.GetField("isAI").GetValue(__instance);
            if (!isAi)
            {
                Characters character = (Characters) __instance.GetField("character").GetValue(__instance);
                if (CharUtil.GetGuidForCharacters(character, out Guid guid))
                {
                    if (CharacterSaveSlots.GetCharacterData(guid, out CharacterProgress progress, character))
                    {
                        progress.moveStyle = setMoveStyleEquipped;
                        CharacterSaveSlots.SaveCharacterData(guid);
                        DebugLog.LogMessage("SetCurrentMoveStyleEquippedPatch saving char");
                    }   
                }
            }
        }
    }**/

    /** //Patch for saving
    [HarmonyPatch(typeof(Reptile.Player), "SaveSelectedCharacter")]
    public class PlayerSaveCharacterPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} PlayerSaveCharacterPatch");

        //Prefix save current movestyle custom
        //Override movestyleEquipped for original and crewboom
        [HarmonyPriority(900)]
        public static bool Prefix(Player __instance, Characters selectedCharacter, ref MoveStyle ___moveStyleEquipped, out MoveStyle __state )
        {
            DebugLog.LogMessage("PlayerSaveSelectedCharacterPatch");

            __state = ___moveStyleEquipped;

            if ((bool)__instance.GetField("isAI").GetValue(__instance)) { return true;}

            if (CharUtil.GetGuidForCharacters(selectedCharacter, out Guid guid))
            {
                CharacterSaveSlots.CurrentSaveSlot.LastPlayedCharacter = guid;
            }

            CharacterSaveSlots.SaveSlot();

            //Override Equiptmovestyle 

            ___moveStyleEquipped = MoveStyle.SKATEBOARD;

            return true;
        }


        //Add postfix to re-set the movestyle
        [HarmonyPriority(399)]
        public static void Postfix(Player __instance, Characters selectedCharacter, ref MoveStyle ___moveStyleEquipped, MoveStyle __state)
        {
            DebugLog.LogMessage("PlayerSaveSelectedCharacterPatch Postfix");
            DebugLog.LogMessage($"base ms - {__instance.moveStyleEquipped}");

            bool isAI = (bool)__instance.GetField("isAI").GetValue(__instance);
            if (isAI) { return; }

            ___moveStyleEquipped = __state;

        }
    }
    **/

    [HarmonyPatch(typeof(Reptile.Player), "FixedUpdateAbilities")]
    public class PlayerFixedUpdateAbilitiesPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} PlayerSaveCharacterPatch");

        //Change in place ___inwalkzone to stop movestyles from switching to on foot unless triggered by button.
        public static void Prefix(Player __instance, ref bool ___inWalkZone, ref MoveStyle ___moveStyleEquipped, out bool __state)
        {
            __state = ___inWalkZone;

            if (___moveStyleEquipped > MoveStyle.MAX)
            {
                if (moveStyleDatabase.GetCharacter(___moveStyleEquipped, out CustomMoveStyle custom))
                {
                    if (custom.Definition.StopSwitchToOnFoot)
                    {
                        ___inWalkZone = false;
                    }
                }
            }

        }

        public static void Postfix(Player __instance, ref bool ___inWalkZone, ref MoveStyle ___moveStyleEquipped, bool __state)
        {
            ___inWalkZone = __state;
        }
    }

}
