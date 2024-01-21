using Reptile;
using UnityEngine;
using UnityEngine.Audio;
using BepInEx.Logging;

namespace MoveStyler
{
    public static class VoiceUtility
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} AudioPatch");

        public static SfxCollectionID VoiceCollectionFromCharacter(Characters character)
        {
            switch (character)
            {
                case Characters.girl1:
                    return SfxCollectionID.VoiceGirl1;
                case Characters.frank:
                    return SfxCollectionID.VoiceFrank;
                case Characters.ringdude:
                    return SfxCollectionID.VoiceRingDude;
                case Characters.metalHead:
                    return SfxCollectionID.VoiceMetalHead;
                case Characters.blockGuy:
                    return SfxCollectionID.VoiceBlockGuy;
                case Characters.spaceGirl:
                    return SfxCollectionID.VoiceSpaceGirl;
                case Characters.angel:
                    return SfxCollectionID.VoiceAngel;
                case Characters.eightBall:
                    return SfxCollectionID.VoiceEightBall;
                case Characters.dummy:
                    return SfxCollectionID.VoiceDummy;
                case Characters.dj:
                    return SfxCollectionID.VoiceDJ;
                case Characters.medusa:
                    return SfxCollectionID.VoiceMedusa;
                case Characters.boarder:
                    return SfxCollectionID.VoiceBoarder;
                case Characters.headMan:
                    return SfxCollectionID.VoiceHeadman;
                case Characters.prince:
                    return SfxCollectionID.VoicePrince;
                case Characters.jetpackBossPlayer:
                    return SfxCollectionID.VoiceJetpackBoss;
                case Characters.legendFace:
                    return SfxCollectionID.VoiceLegendFace;
                case Characters.oldheadPlayer:
                    return SfxCollectionID.NONE;
                case Characters.robot:
                    return SfxCollectionID.VoiceRobot;
                case Characters.skate:
                    return SfxCollectionID.VoiceSkate;
                case Characters.wideKid:
                    return SfxCollectionID.VoiceWideKid;
                case Characters.futureGirl:
                    return SfxCollectionID.VoiceFutureGirl;
                case Characters.pufferGirl:
                    return SfxCollectionID.VoicePufferGirl;
                case Characters.bunGirl:
                    return SfxCollectionID.VoiceBunGirl;
                case Characters.headManNoJetpack:
                    return SfxCollectionID.VoiceHeadmanNoJetpack;
                case Characters.eightBallBoss:
                    return SfxCollectionID.VoiceEightBallBoss;
                case Characters.legendMetalHead:
                    return SfxCollectionID.VoiceMetalHead;
            }

            return SfxCollectionID.NONE;
        }
        public static Characters CharacterFromVoiceCollection(SfxCollectionID collectionID)
        {
            switch (collectionID)
            {
                case SfxCollectionID.VoiceAngel:
                    return Characters.angel;
                case SfxCollectionID.VoiceBlockGuy:
                    return Characters.blockGuy;
                case SfxCollectionID.VoiceBoarder:
                    return Characters.boarder;
                case SfxCollectionID.VoiceBunGirl:
                    return Characters.bunGirl;
                case SfxCollectionID.VoiceDJ:
                    return Characters.dj;
                case SfxCollectionID.VoiceDummy:
                    return Characters.dummy;
                case SfxCollectionID.VoiceEightBall:
                    return Characters.eightBall;
                case SfxCollectionID.VoiceEightBallBoss:
                    return Characters.eightBallBoss;
                case SfxCollectionID.VoiceFrank:
                    return Characters.frank;
                case SfxCollectionID.VoiceFutureGirl:
                    return Characters.futureGirl;
                case SfxCollectionID.VoiceGirl1:
                    return Characters.girl1;
                case SfxCollectionID.VoiceHeadman:
                    return Characters.headMan;
                case SfxCollectionID.VoiceHeadmanNoJetpack:
                    return Characters.headManNoJetpack;
                case SfxCollectionID.VoiceJetpackBoss:
                    return Characters.jetpackBossPlayer;
                case SfxCollectionID.VoiceLegendFace:
                    return Characters.legendFace;
                case SfxCollectionID.VoiceMedusa:
                    return Characters.medusa;
                case SfxCollectionID.VoiceMetalHead:
                    return Characters.metalHead;
                case SfxCollectionID.VoicePrince:
                    return Characters.prince;
                case SfxCollectionID.VoicePufferGirl:
                    return Characters.pufferGirl;
                case SfxCollectionID.VoiceRingDude:
                    return Characters.ringdude;
                case SfxCollectionID.VoiceSpaceGirl:
                    return Characters.spaceGirl;
                case SfxCollectionID.VoiceWideKid:
                    return Characters.wideKid;
                case SfxCollectionID.VoiceRobot:
                    return Characters.robot;
                case SfxCollectionID.VoiceSkate:
                    return Characters.skate;
                default:
                    return Characters.NONE;
            }
        }

        // Movestyler
        public static AudioClip GetRandomClipFromIdCustomSFX(AudioClipID audioClipID, SfxCollection SFX )
        {

            if (SFX.audioClipContainers.Length == 0)
            {
                return null;
            }

            SfxCollection.RandomAudioClipContainer audioClipContainerFromID = GetAudioClipContainerFromIDCustom(ref SFX.audioClipContainers, audioClipID);
            AudioClip result = null;
            if (audioClipContainerFromID != null && audioClipContainerFromID.clipID == audioClipID && audioClipContainerFromID.clips != null)
            {
                int num = audioClipContainerFromID.clips.Length;
                int num2 = UnityEngine.Random.Range(0, num);
                if (num2 == audioClipContainerFromID.lastRandomClip)
                {
                    num2 = (num2 + 1) % num;
                }
                audioClipContainerFromID.lastRandomClip = num2;
                result = audioClipContainerFromID.clips[num2];
            }
            return result;
        }

        public static SfxCollection.RandomAudioClipContainer GetAudioClipContainerFromIDCustom(ref SfxCollection.RandomAudioClipContainer[] audioClipContainers, AudioClipID clipID)
        {
            int num = audioClipContainers.Length;

            foreach (SfxCollection.RandomAudioClipContainer cont in audioClipContainers)
            {
                if (cont.clipID == clipID)
                {
                    return cont;
                }
            }
            return null;
        }

        public static void AddNewMovestyleSFXCollection(SfxLibrary sfxLibrary, SfxCollection collection, SfxCollectionID sfxCollectionID, string name)
        {
            DebugLog.LogMessage($"Adding new Movestyle SFX Collection | id : {sfxCollectionID}");

            if (sfxLibrary.sfxCollectionIDDictionary.ContainsKey(sfxCollectionID))
            {
                DebugLog.LogWarning($"A SFX Collection of ID {(int)sfxCollectionID} already exists");
                return;
            }

            DebugLog.LogMessage($"Adding new Movestyle SFX Collection to sfxLibrary Dictionaries");
            sfxLibrary.sfxCollectionIDDictionary.Add(sfxCollectionID, collection);
            sfxLibrary.sfxCollectionDictionary.Add(GetMovestyleSFXCollectionNameFromStyleName(name), collection);
            
        }
        public static SfxCollectionID GetMovestyleSFXCollectionID(MoveStyle moveStyle)
        {
            return  SfxCollectionID.MAX + 100 + (int)moveStyle;
        }

        public static string GetMovestyleSFXCollectionNameFromStyleName(string name)
        {
            return "customMovestyle." + name;
        }

        // Use ID's 7800+ for custom Sounds
        public enum CustomMovestyleSFXID
        {
            GrindTrick1 = 7800,
			GrindTrick2 = 7801,
            GrindTrick3 = 7802,
            GroundTrick1 = 7803,
            GroundTrick2 = 7804,
            GroundTrick3 = 7805,
            AirTrick1 = 7806,
            AirTrick2 = 7807,
            AirTrick3 = 7808,

        }

        public static void PlayLoopingSFX(AudioMixerGroup audioMixerGroup, AudioClip audioClip, AudioSource audioSource, float randomPitchVariance, float delayedSeconds)
        {
            DebugLog.LogMessage("PlayLoopingSFX Voic Utility");


            if (audioClip == null)
            {
                DebugLog.LogMessage("clip null");
                return;
            }
            if (randomPitchVariance > 0f)
            {
                randomPitchVariance = UnityEngine.Random.Range(-randomPitchVariance, randomPitchVariance);
            }
            audioSource.clip = audioClip;
            audioSource.loop = true;
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.pitch = 1f - randomPitchVariance;
            if (delayedSeconds != 0f)
            {
                audioSource.PlayDelayed(delayedSeconds);
                return;
            }
            DebugLog.LogMessage($"playing audio {audioSource.clip}, {audioSource.outputAudioMixerGroup}");

            audioSource.Play();
        }

    }
}