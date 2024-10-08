﻿using MoveStylerMono;
using Reptile;
using System;
using UnityEngine;
using BepInEx.Logging;

namespace MoveStyler
{
    public static class CharUtil
    {
        public const string CHARACTER_BUNDLE = "characters";

        public const string ADD_CHARACTER_METHOD = "AddCharacterFBX";
        public const string ADD_MATERIAL_METHOD = "AddCharacterMaterial";

        public const string MATERIAL_FORMAT = "{0}Mat{1}";

        public const string SKATE_OFFSET_L = "skateOffsetL";
        public const string SKATE_OFFSET_R = "skateOffsetR";

        public const string GRAFFITI_ASSET = "charGraffiti";

        public static string GetOutfitMaterialName(Characters character, int outfitIndex)
        {
            return string.Format(MATERIAL_FORMAT, character.ToString(), outfitIndex.ToString());
        }

        private static readonly string[] PropBones = new string[]
        {
            "propl",
            "propr",
            "footl",
            "footr",
            "handl",
            "handr",
            "jetpackPos"
        };

        public static void ReparentAllProps(Transform originalRoot, Transform targetRoot)
        {
            foreach (string bone in PropBones)
            {
                Transform original = originalRoot.FindRecursive(bone);
                Transform target = targetRoot.FindRecursive(bone);
                if (original && target)
                {
                    ReparentChildren(original, target);
                }
            }
        }
        private static void ReparentChildren(Transform source, Transform target)
        {
            Transform[] children = source.GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i] == null)
                {
                    continue;
                }
                children[i].SetParent(target, false);
                children[i].SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }

        public static bool TrySetCustomOutfit(Component source, int outfit, out SkinnedMeshRenderer firstActiveRenderer)
        {
            firstActiveRenderer = null;

            //CharacterDefinition characterDefinition = source.GetComponentInChildren<CharacterDefinition>(true);
            //if (characterDefinition != null)
            //{
                //characterDefinition.SetOutfit(outfit, out firstActiveRenderer);
            //    return true;
            //}

            return false;
        }

        public static int GetSavedCharacterOutfit(Characters character)
        {
            int outfit = 0;
            /*
            if (character > Characters.MAX && CharacterDatabase.GetFirstOrConfigCharacterId(character, out Guid guid))
            {
                if (CharacterSaveSlots.GetCharacterData(guid, out CharacterProgress data))
                {
                    outfit = data.outfit;
                }
            }
            else
            {
                outfit = Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(character).outfit;
            }
            */
            return outfit;
        }

        public static bool GetGuidForDefaultCharacters(Characters character, out Guid guid)
        {
            guid = new Guid();

            if (character > Characters.MAX) { return false; }

            string charName = character.ToString("X");
            if (charName.Length > 8)
            {
                charName = charName.Remove(8);
            }
            charName = charName.PadRight(8, '0');
            charName += "-0000-0000-0000-000000000000";

            if (Guid.TryParse(charName, out guid))
            {
                return true;
            }
            return false;
        }

        public static bool GetGuidForCharacters(Characters character, out Guid guid)
        {
            guid = Guid.Empty;

            if (character < Characters.MAX)
            {
                GetGuidForDefaultCharacters( character, out guid);
            }
            else if(CrewBoomAPI.CrewBoomAPIDatabase.IsInitialized)
            {
                CrewBoomAPI.CrewBoomAPIDatabase.GetUserGuidForCharacter((int)character, out guid);
            }

            return guid != Guid.Empty;
        }

    }
}
