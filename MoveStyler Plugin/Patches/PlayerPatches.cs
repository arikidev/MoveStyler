using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Reptile;
using UnityEngine;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;

using MoveStyler.Data;

namespace MoveStyler.Patches
{
    
    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.SetCurrentMoveStyleEquipped))]
    public class PlayerSetCurrentMoveStyleEquippedPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME}Player Patches");

        public static bool Prefix(ref MoveStyle setMoveStyleEquipped, bool changeProp, bool changeAnim)
        {
            if (setMoveStyleEquipped > MoveStyle.MAX)
            {
                //DebugLog.LogMessage("Set Movestyle Equipt is custom");
                return true;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Reptile.Player), "SetMoveStyle")]
    public class PlayerSetMoveStylePatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Player Patches");

        public static bool Prefix(ref Player __instance, MoveStyle setMoveStyle, bool changeProp, bool changeAnim, GameObject specialSkateboard = null)
        {
            //DebugLog.LogMessage("Patching SetMoveStyle:");

            if (setMoveStyle > MoveStyle.MAX)
            {
                //DebugLog.LogMessage("setMovestyle is custom");

                CharacterVisual characterVisual = (CharacterVisual)__instance.GetField("characterVisual").GetValue(__instance);

                if (changeAnim)
                {
                    int curAnim = (int)__instance.GetField("curAnim").GetValue(__instance);

                    characterVisual.SetMoveStyleVisualAnim(__instance, setMoveStyle, specialSkateboard);
                    if (curAnim != 0)
                    {
                        int newAnim = curAnim;
                        __instance.GetField("curAnim").SetValue(__instance, 0);
                        //__instance.curAnim = 0; Above line to set fields
                        __instance.PlayAnim(newAnim, false, false, -1f);
                    }
                }

                //Set MoveStyle enum on Character
                __instance.GetField("moveStyle").SetValue(__instance, setMoveStyle);

                //Apply Visual Change to CustomMoveStyle
                if (changeProp)
                {
                    //DebugLog.LogMessage("Updating Visual");
                    characterVisual.SetMoveStyleVisualProps(__instance, setMoveStyle, false);
                }

                //Set MovementStats
                CustomMoveStyle customMoveStyle;
                moveStyleDatabase.GetCharacter(setMoveStyle, out customMoveStyle);
                //ToDo Set this up correctly
                customMoveStyle.setCustomMovementStats(__instance);

                return false; // Skip is Enum is Custom
            }
            //DebugLog.LogMessage("SetMovestyle is default style");

            return true;
        }
    }

    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.InitAnimation))]
    public class PlayerInitAnimationPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Player Patches");

        public static void Postfix(ref Player __instance)
        {
            //DebugLog.LogMessage("Start Init Animation Patch");

            MoveStyle originalStyle = (MoveStyle)__instance.GetField("moveStyle").GetValue(__instance);

            Dictionary<int, Player.AnimInfo>[] animInfoSets = (Dictionary<int, Player.AnimInfo>[])__instance.GetField("animInfosSets").GetValue(__instance);

            //Resize Array to match new custom styles
            Array.Resize(ref animInfoSets, animInfoSets.Length + moveStyleDatabase.NewCharacterCount + 1);
            __instance.GetField("animInfosSets").SetValue(__instance, animInfoSets);

            for (int id = 1; id <= moveStyleDatabase.NewCharacterCount; id++)
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

            //DebugLog.LogMessage($"forwardSpeed : {speed}");
            //DebugLog.LogMessage($"forwardSpeedGet : {charVis.anim.GetFloat("forwardSpeed")}");

        }

    }

    // Allow induvidual animations override the HandIk settings.
    [HarmonyPatch(typeof(Reptile.Player), "PlayAnim")]
    public class PlayerPlayAnimPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Player Patches");

        public static void Postfix(ref Player __instance, int newAnim, bool forceOverwrite = false, bool instant = false, float atTime = -1f)
        {
            MoveStyle style = (MoveStyle)__instance.GetField("moveStyle").GetValue(__instance);
            
            if  (style > MoveStyle.MAX)
            {
                CustomMoveStyle customMovestyle; moveStyleDatabase.GetCharacter(style, out customMovestyle);
                CharacterVisual charVis = (CharacterVisual)__instance.GetField("characterVisual").GetValue(__instance);


                bool lHandIK = customMovestyle.Definition.UseHandLIK;
                bool rHandIK = customMovestyle.Definition.UseHandRIK;

                // If Custom animInfo is preset check IK tags
                if (customMovestyle.customAnimInfoDict.ContainsKey(newAnim))
                {
                    lHandIK = customMovestyle.customAnimInfoDict[newAnim].lHandIKOverride ^ customMovestyle.Definition.UseHandLIK;
                    rHandIK = customMovestyle.customAnimInfoDict[newAnim].rHandIKOverride ^ customMovestyle.Definition.UseHandRIK;
                }

                charVis.GetField("handIKActiveL").SetValue(charVis, lHandIK);
                charVis.GetField("handIKActiveR").SetValue(charVis, rHandIK);

                CustomMoveStyleVisualParent parent = CustomMoveStyleVisualParent.GetCustomMoveStyleVisualParent(charVis);
                parent.LHandIKCurrent = lHandIK;
                parent.RHandIKCurrent = rHandIK;

                //DebugLog.LogMessage($" PlayAnimtion: {newAnim} ik: {lHandIK} | {rHandIK} ");
            }
        }
    }

    //A really dump fix for other mod crashes
    [HarmonyPatch(typeof(Reptile.Player), "DoTrick")]
    public class PlayerDoTrick
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Player Patches");

        [HarmonyPriority(701)] //Manually Setting a Higher Priority to fix issues with null inputs
        public static void Prefix(Player __instance, ref string trickName, int trickNum = 0, Player.TrickType type = Player.TrickType.NONE)
        {
            DebugLog.LogMessage("****************Patching Do Trick*****************");


            if (trickName == null)
            {
                trickName = "";
            }
        }
    }

    /** Removed Because its no longer needed
    [HarmonyPatch(typeof(Reptile.Player), "PlayAnim")]
    public class PlayerPlayAnimPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Player Patches");

        public static bool Prefix(ref Player __instance, int newAnim, bool forceOverwrite = false, bool instant = false, float atTime = -1f)
        {
            return true; //Testing reEnabling normal anims
        }
    }
    */

    /*
    [HarmonyPatch(typeof(Reptile.Player), "UpdateAnim")] //Reflection? idk what this does
    public class PlayerUpdateAnimPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Player Patches");

        public static bool Prefix(ref Player __instance)
        {
            return true; //Testing reEnabling normal anims

            MoveStyle equippedStyle = (MoveStyle)__instance.GetField("moveStyleEquipped").GetValue(__instance);

            // Process Custom Movestyles
            if (equippedStyle > MoveStyle.MAX)
            {
                //DebugLog.LogMessage("Custom Update Anim");
                return false;
            }

            return true;
        }
    }
    */

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

    /*
    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.SetOutfit))]
    public class PlayerSetOutfitPatch
    {
        public static bool Prefix(int setOutfit, Player __instance, CharacterVisual ___characterVisual, Characters ___character)
        {
            if (!CharacterDatabase.HasCharacter(___character))
            {
                return true;
            }

            bool isAi = (bool) __instance.GetField("isAI").GetValue(__instance);
            if (!isAi)
            {
                Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(___character).outfit = setOutfit;

                if (___character > Characters.MAX)
                {
                    if (CharacterDatabase.GetFirstOrConfigCharacterId(___character, out Guid guid))
                    {
                        CharacterSaveSlots.SaveCharacterData(guid);
                    }
                }
            }

            if (CharUtil.TrySetCustomOutfit(___characterVisual, setOutfit, out SkinnedMeshRenderer firstActiveRenderer))
            {
                ___characterVisual.mainRenderer = firstActiveRenderer;
            }

            return false;
        }
    }
    */

    /* To Do switch to a system that request movestyle switching by name
    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.SetCurrentMoveStyleEquipped))]
    public class PlayerSetMovestyleEquipped
    {
        public static void Postfix(Player __instance, MoveStyle setMoveStyleEquipped)
        {
            bool isAi = (bool) __instance.GetField("isAI").GetValue(__instance);
            if (!isAi)
            {
                Characters character = (Characters) __instance.GetField("character").GetValue(__instance);
                if (character > Characters.MAX)
                {
                    if (CharacterDatabase.GetFirstOrConfigCharacterId(character, out Guid guid))
                    {
                        if (CharacterSaveSlots.GetCharacterData(guid, out CharacterProgress progress))
                        {
                            progress.moveStyle = setMoveStyleEquipped;
                            CharacterSaveSlots.SaveCharacterData(guid);
                        }
                    }
                }
            }
        }
    }
    */

    /*
    [HarmonyPatch(typeof(Reptile.Player), "SaveSelectedCharacter")]
    public class PlayerSaveCharacterPatch
    {
        public static bool Prefix(Player __instance, ref Characters selectedCharacter)
        {
            bool runOriginal = true;

            bool isAI = (bool) __instance.GetField("isAI").GetValue(__instance);
            bool isNew = selectedCharacter > Characters.MAX;
            if (!isAI)
            {
                CharacterSaveSlots.CurrentSaveSlot.LastPlayedCharacter = Guid.Empty;

                if (isNew)
                {
                    if (CharacterDatabase.GetFirstOrConfigCharacterId(selectedCharacter, out Guid guid))
                    {
                        CharacterSaveSlots.CurrentSaveSlot.LastPlayedCharacter = guid;
                    }
                    runOriginal = false;
                }

                CharacterSaveSlots.SaveSlot();
            }
            else if (selectedCharacter > Characters.MAX)
            {
                runOriginal = false;
            }

            return runOriginal;
        }
    }
    */

    /*
    [HarmonyPatch(typeof(Reptile.Player), nameof(Player.PlayVoice))]
    public class PlayerVoicePatch
    {
        public static bool Prefix(AudioClipID audioClipID,
                                  VoicePriority voicePriority,
                                  bool fromPlayer,
                                  AudioManager ___audioManager,
                                  ref VoicePriority ___currentVoicePriority,
                                  Characters ___character,
                                  AudioSource ___playerGameplayVoicesAudioSource)
        {
            if (___character > Characters.MAX && CharacterDatabase.GetCharacter(___character, out CustomCharacter customCharacter))
            {
                if (fromPlayer)
                {
                    //ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("Test");
                    //log.LogMessage(___currentVoicePriority);

                    //___audioManager.InvokeMethod("PlayVoice",
                    //    new Type[] { typeof(VoicePriority).MakeByRefType(), typeof(Characters), typeof(AudioClipID), typeof(AudioSource), typeof(VoicePriority) },
                    //    ___currentVoicePriority, ___character, audioClipID, ___playerGameplayVoicesAudioSource, voicePriority);

                    //log.LogMessage(___currentVoicePriority);
                }
                else
                {
                    ___audioManager.InvokeMethod("PlaySfxGameplay",
                        new Type[] { typeof(SfxCollectionID), typeof(AudioClipID), typeof(float) },
                        customCharacter.SfxID, audioClipID, 0.0f);
                    return false;
                }

            }

            return true;
        }
    }
    */
}
