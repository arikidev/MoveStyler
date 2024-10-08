﻿using Reptile;
using HarmonyLib;
using System.Collections.Generic;
using System;
using MoveStyler.Data;
using UnityEngine;
using UnityEngine.Audio;
using BepInEx.Logging;
using MoveStyler;

namespace MoveStyler.Patches
{

    [HarmonyPatch(typeof(Reptile.AudioManager))]
    public class AudioManagerPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} AudioPatch");

        [HarmonyPostfix]
        [HarmonyPatch("InitializeSfxLibrary")]
        public static void InitializeSfxLibraryPatch(AudioManager __instance)
        {
            for (int i = 0; i < moveStyleDatabase.NewMovestyleCount; i++)
            {
                CustomMoveStyle style;
                moveStyleDatabase.GetCharacter(MoveStyle.MAX + i + 1, out style);

                if (style == null) { DebugLog.LogError("Failed to get Custom MoveStyle"); return; }

                SfxLibrary sfxLibrary = (SfxLibrary)__instance.GetField("sfxLibrary").GetValue(__instance);
                if (sfxLibrary == null) { DebugLog.LogError("Failed to get SFXLibrary"); return; }

                VoiceUtility.AddNewMovestyleSFXCollection(sfxLibrary, style.Sfx, style.SfxID, style.name);

            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("PlaySfxGameplayLooping", typeof(MoveStyle), typeof(AudioClipID), typeof(AudioSource), typeof(float), typeof(float))]
        public static bool PlaySfxGameplayLoopingPatched(AudioManager __instance, ref List<AudioSource> ___activeGameplayLoopingAudioSources, AudioMixerGroup[] ___mixerGroups, ref MoveStyle moveStyle, AudioClipID audioClipId, AudioSource audioSource, float randomPitchVariance = 0f, float delayedSeconds = 0f)
        {
            if (moveStyle > MoveStyle.MAX)
            {

                CustomMoveStyle style; moveStyleDatabase.GetCharacter(moveStyle, out style);
                moveStyle = (MoveStyle)((int)style.SfxID - (int)SfxCollectionID.MoveStyle_0_Default );
                //DebugLog.LogMessage($"movestyle SFXID:  {(int)style.SfxID} {(int)moveStyle} , id = {audioClipId}");

                //AudioClip audioClipFromCollection = VoiceUtility.GetRandomClipFromIdCustomSFX(audioClipId, style.Sfx); //_instance.sfxLibrary.GetAudioClipFromCollection(SfxCollectionID.MoveStyle_0_Default + (int)moveStyle, audioClipId);

                //DebugLog.LogMessage($"clip {style.Sfx.GetAudioClipById(audioClipId)}, ID {(int)audioClipId}, Source {audioSource}, delay {delayedSeconds}, mixer {___mixerGroups[3]}");

                
                //__instance.InvokeMethod("PlayLoopingSfx",
                //        new Type[] { typeof(AudioMixerGroup), typeof(AudioClip), typeof(AudioSource), typeof(float), typeof(float) },
                //         ___mixerGroups[3], audioClipFromCollection, audioSource, randomPitchVariance, delayedSeconds);
                
                //VoiceUtility.PlayLoopingSFX(___mixerGroups[2], audioClipFromCollection, audioSource, randomPitchVariance, delayedSeconds);

                // old _instance.PlayLoopingSfx(_instance.mixerGroups[3], audioClipFromCollection, audioSource, randomPitchVariance, delayedSeconds);

                //__instance.InvokeMethod("AddNewActiveLoopingAudioSource",
                //    new Type[] { typeof(AudioSource) },
                //        audioSource);

                //return false;
            }

            return true;

            /*
            internal void PlaySfxGameplayLooping(MoveStyle moveStyle, AudioClipID audioClipId, AudioSource audioSource, float randomPitchVariance = 0f, float delayedSeconds = 0f)
            {
                AudioClip audioClipFromCollection = this.sfxLibrary.GetAudioClipFromCollection(SfxCollectionID.MoveStyle_0_Default + (int)moveStyle, audioClipId);
                this.PlayLoopingSfx(this.mixerGroups[3], audioClipFromCollection, audioSource, randomPitchVariance, delayedSeconds);
                this.AddNewActiveLoopingAudioSource(audioSource);
            }
            */

        }

        [HarmonyPrefix]
        [HarmonyPatch ("PlaySfxGameplay", typeof(MoveStyle), typeof(AudioClipID), typeof(AudioSource), typeof(float))]
        public static bool PlaySfxGameplayPatch(Reptile.AudioManager __instance, AudioMixerGroup[] ___mixerGroups, MoveStyle moveStyle, AudioClipID audioClipId, AudioSource audioSource, float randomPitchVariance = 0f)
        {

            if (moveStyle > MoveStyle.MAX)
            {
                //DebugLog.LogMessage($"Trying to Find Custom SFX of id {(int)audioClipId}");

                //Get the correct SFXID
                SfxCollectionID id = VoiceUtility.GetMovestyleSFXCollectionID(moveStyle);
                //DebugLog.LogMessage($" Get SFX Collection id from Movestyle: {moveStyle} | ID: {id}");

                __instance.InvokeMethod("PlaySfxGameplay",
                        new Type[] { typeof(SfxCollectionID), typeof(AudioClipID), typeof(AudioSource), typeof(float) },
                         id, audioClipId, audioSource, randomPitchVariance);

            }

            return true;

        }

    }



    [HarmonyPatch(typeof(Reptile.AnimationEventRelay))]
    public class AnimationEventRelayPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} AnimationEventRelayPatch");


        [HarmonyPrefix]
        [HarmonyPatch("PlayRandomSoundMoveStyle")]
        static bool PlayRandomSoundMoveStylePatch( ref AnimationEventRelay __instance , string soundName)
        {
            Player player = (Player)__instance.GetField("player").GetValue(__instance);

            if (player == null)
            {
                return true;
            }

            MoveStyle moveStyle = (MoveStyle)player.GetField("moveStyle").GetValue(player);
            AudioSource audioSource = (AudioSource)player.GetField("playerOneShotAudioSource").GetValue(player);

            if (moveStyle > MoveStyle.MAX)
            {
                //Custom Movestyles will pass throught the SFX ID as the sound name
                int parseInt = -1;

                if (!int.TryParse(soundName, out parseInt))
                {
                    //Failed to parse Int return;
                    return true;
                }

                AudioClipID audioClipId = (AudioClipID)parseInt;

                player.AudioManager.InvokeMethod("PlaySfxGameplay",
                        new Type[] { typeof(MoveStyle), typeof(AudioClipID), typeof(AudioSource), typeof(float) },
                         moveStyle, audioClipId, audioSource, 0f);

                return false;
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(Reptile.Player))]
    public class PlayerSFXPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} AnimationEventRelayPatch");

        [HarmonyPrefix]
        [HarmonyPatch("UpdateMoveStyleMovementAudio")]
        static bool UpdateMoveStyleMovementAudio(ref Player __instance, MoveStyle ___moveStyle, Ability ___ability, Player.MovementType ___targetMovement, ref MoveStyle ___movestyleAudioCurrent, ref AudioClip ___movestyleAudioTargetClip, AudioSource ___playerMovementLoopAudioSource )
        {
            
            if (___moveStyle <= MoveStyle.MAX) // Early Out
            { return true; }

            CustomMoveStyle style; moveStyleDatabase.GetCharacter(___moveStyle, out style);

            // ability is null | onGround | targetMovement is Running or walking | Is Movestyle Correct
            if (___ability  == null && (___targetMovement == Player.MovementType.RUNNING || ___targetMovement == Player.MovementType.WALKING) && __instance.IsGrounded())
            {
                
                SfxCollectionID sfxCollectionID = style.SfxID;

                if (___moveStyle != ___movestyleAudioCurrent)
                {
                    ___movestyleAudioTargetClip = VoiceUtility.GetRandomClipFromIdCustomSFX( AudioClipID.moveLoop , style.Sfx);
                    ___movestyleAudioCurrent = ___moveStyle;
                }
                
                if (___playerMovementLoopAudioSource.clip != ___movestyleAudioTargetClip)
                {
                    //__instance.AudioManager.PlaySfxGameplayLooping(sfxCollectionID, AudioClipID.moveLoop, this.playerMovementLoopAudioSource, 0f, 0f);

                    __instance.AudioManager.InvokeMethod("PlaySfxGameplayLooping",
                            new Type[] { typeof(SfxCollectionID), typeof(AudioClipID), typeof(AudioSource), typeof(float) , typeof(float) },
                            sfxCollectionID, AudioClipID.moveLoop, ___playerMovementLoopAudioSource, 0f, 0f );

                    return false;
                }
            }
            // Is Loop clip still playing | Is Current Clip a moveLoop
            else if (!(___playerMovementLoopAudioSource.isPlaying && ___playerMovementLoopAudioSource.time == 0f) && VoiceUtility.GetRandomClipFromIdCustomSFX(AudioClipID.moveLoop, style.Sfx) == ___playerMovementLoopAudioSource.clip) // Maybe Add back clip check?
            {
                
                //__instance.AudioManager.StopLoopingSfx(this.playerMovementLoopAudioSource);
                __instance.AudioManager.InvokeMethod("StopLoopingSfx", new Type[] { typeof(AudioSource)}, ___playerMovementLoopAudioSource);

                //DebugLog.LogMessage($"Invoked StopSFXLooping | {___playerMovementLoopAudioSource.clip.GetName() }");

                return false;
            }

            return true;

        }
    }

    [HarmonyPatch(typeof(Reptile.SfxCollection))]
    public class SfxCollectionPatch
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} SfxCollectionPatch");

        [HarmonyPrefix]
        [HarmonyPatch("GetAudioClipContainerFromID")]
        public static bool GetAudioClipContainerFromIDPatch(ref SfxCollection.RandomAudioClipContainer[] audioClipContainers, AudioClipID clipID, ref string ___collectionName, ref SfxCollection.RandomAudioClipContainer __result)
        {
            //Sfx Collections for custom Movestyles have a final identifier container with ID @9999
            bool isCustomMovestyle = ( audioClipContainers[audioClipContainers.Length - 1].clipID == (AudioClipID)9999);

            if (isCustomMovestyle)
            {
                __result = VoiceUtility.GetAudioClipContainerFromIDCustom(ref audioClipContainers, clipID);
                return false;
            }
            return true;
        }

    }

}
