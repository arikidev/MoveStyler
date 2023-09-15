﻿using UnityEngine;
using Reptile;
using HarmonyLib;
using BepInEx.Logging;
using BrcCustomCharactersLib;
using BrcCustomCharacters.Data;

namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(Reptile.NPC), nameof(Reptile.NPC.InitSceneObject))]
    public class NPCInitPatch
    {
        public static void Prefix(NPC __instance, Characters ___character)
        {
            OutfitSwappableCharacter[] characters = __instance.GetComponentsInChildren<OutfitSwappableCharacter>(true);
            if (characters.Length > 0)
            {
                foreach (OutfitSwappableCharacter npcCharacter in characters)
                {
                    if (CharacterDatabase.GetCharacter(npcCharacter.Character, out CustomCharacter character))
                    {
                        foreach (DynamicBone dynamicBone in npcCharacter.GetComponents<DynamicBone>())
                        {
                            dynamicBone.enabled = false;
                        }

                        GameObject customCharacter = Object.Instantiate(character.Definition.gameObject, npcCharacter.transform).gameObject;

                        Animator originalAnimator = npcCharacter.GetComponentInChildren<Animator>(true);
                        Animator customAnimator = customCharacter.GetComponent<Animator>();
                        customAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;

                        customCharacter.transform.localPosition = originalAnimator.transform.localPosition;
                        customCharacter.transform.localRotation = originalAnimator.transform.localRotation;

                        SkinnedMeshRenderer customRenderer = customCharacter.GetComponentInChildren<SkinnedMeshRenderer>(true);
                        npcCharacter.SetField("mainRenderer", customRenderer);

                        customCharacter.AddComponent<LookAtIKComponent>();
                        customCharacter.AddComponent<DummyAnimationEventRelay>();
                        if (character.Definition.CanBlink)
                        {
                            StoryBlinkAnimation blinkAnimation = customCharacter.AddComponent<StoryBlinkAnimation>();
                            blinkAnimation.mainRenderer = customRenderer;
                            blinkAnimation.characterMesh = customRenderer.sharedMesh;
                        }

                        customCharacter.SetActive(originalAnimator.gameObject.activeSelf);

                        //Need to use DestroyImmediate because Destroy won't destroy it in time for the actual function running
                        //to not find the destroyed object still
                        Object.DestroyImmediate(originalAnimator.gameObject);
                    }
                }
            }
            else if (CharacterDatabase.GetCharacter(___character, out CustomCharacter character))
            {
                GameObject customCharacter = Object.Instantiate(character.Definition.gameObject, __instance.transform).gameObject;

                Animator originalAnimator = __instance.transform.GetComponentInChildren<Animator>(true);
                Animator customAnimator = customCharacter.GetComponent<Animator>();
                customAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;

                customCharacter.transform.localPosition = originalAnimator.transform.localPosition;
                customCharacter.transform.localRotation = originalAnimator.transform.localRotation;

                customCharacter.SetActive(originalAnimator.gameObject.activeSelf);

                //Need to use DestroyImmediate because Destroy won't destroy it in time for the actual function running
                //to not find the destroyed object still
                Object.DestroyImmediate(originalAnimator.gameObject);
            }
        }
    }
}
