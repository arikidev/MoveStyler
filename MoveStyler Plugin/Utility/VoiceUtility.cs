using Reptile;
using UnityEngine;
using UnityEngine.Audio;
using BepInEx.Logging;

namespace MoveStyler
{
    public static class VoiceUtility
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} AudioPatch");

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

            if (sfxLibrary.sfxCollectionIDDictionary.ContainsKey(sfxCollectionID))
            {
                DebugLog.LogWarning($"A SFX Collection of ID {(int)sfxCollectionID} already exists");
                return;
            }

            sfxLibrary.sfxCollectionIDDictionary.Add(sfxCollectionID, collection);
            sfxLibrary.sfxCollectionDictionary.Add(GetMovestyleSFXCollectionNameFromStyleName(name), collection);
            
        }
        public static SfxCollectionID GetMovestyleSFXCollectionID(MoveStyle moveStyle)
        {
            return  SfxCollectionID.MAX + 500 + (int)moveStyle;
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
            //DebugLog.LogMessage("PlayLoopingSFX Voic Utility");


            if (audioClip == null)
            {
                DebugLog.LogWarning($"clip {audioClip} was null");
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
            //DebugLog.LogMessage($"playing audio {audioSource.clip}, {audioSource.outputAudioMixerGroup}");

            audioSource.Play();
        }

    }
}