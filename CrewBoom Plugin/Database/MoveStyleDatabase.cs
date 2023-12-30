using UnityEngine;
using Reptile;
using System.IO;
using System.Collections.Generic;
using System;
using BepInEx.Logging;
using BepInEx;
using System.Text;
using CrewBoomAPI;
using System.Linq;
using UnityEngine.TextCore.Text;
using MoveStyler.Data;
using MoveStylerMono;

namespace MoveStyler
{
    public static class moveStyleDatabase
    {
        private static readonly string ASSET_PATH = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_NAME);
        //private static readonly string NO_CYPHER_PATH = Path.Combine(ASSET_PATH, "no_cypher");

        public static int NewCharacterCount { get; private set; } = 0;

        private static Dictionary<Guid, string> _moveStyleBundlePaths;
        private static Dictionary<Guid, CustomMoveStyle> _customMoveStyle;
        private static Dictionary<MoveStyle, List<Guid>> _moveStyleIds;

        public static bool HasCharacterOverride { get; private set; }
        public static Guid CharacterOverride { get; private set; }

        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} StyleDatabase");

        public static bool Initialize()
        {
            if (!Directory.Exists(ASSET_PATH))
            {
                DebugLog.LogWarning($"Could not find character bundle directory \"{ASSET_PATH}\".\nIt was created instead.");
                Directory.CreateDirectory(ASSET_PATH);
                return false;
            }

            /*if (!Directory.Exists(NO_CYPHER_PATH))
            {
                DebugLog.LogMessage("No cypher bundle path was not found. It was created instead.");
                Directory.CreateDirectory(NO_CYPHER_PATH);
            }*/

            _moveStyleBundlePaths = new Dictionary<Guid, string>();
            _customMoveStyle = new Dictionary<Guid, CustomMoveStyle>();
            _moveStyleIds = new Dictionary<MoveStyle, List<Guid>>();

            var styleEnum = Enum.GetValues(typeof(MoveStyle));
            foreach (MoveStyle style in styleEnum)
            {
                if (style == MoveStyle.ON_FOOT || style == MoveStyle.MAX)
                {
                    _moveStyleIds.Add(style, null);
                    continue;
                }
                _moveStyleIds.Add(style, new List<Guid>());
            }

            bool foundAnyCharacters = LoadAllMoveStyleData();
            if (!foundAnyCharacters)
            {
                DebugLog.LogWarning($"There were no valid movestyles found in {ASSET_PATH}.\nMake sure your character bundles (.cbb) are in the CONFIG folder, NOT the PLUGIN folder.");
                return false;
            }

            InitializeAPI();

            return true;
        }

        private static bool LoadAllMoveStyleData()
        {
            bool foundAtLeastOneCharacter = false;

            foreach (string filePath in Directory.GetFiles(ASSET_PATH, "*.cbb"))
            {
                if (LoadCharacterBundle(filePath, true))
                {
                    foundAtLeastOneCharacter = true;
                }
            }

            return foundAtLeastOneCharacter;
        }

        private static bool LoadCharacterBundle(string filePath, bool enableCypher)
        {
            bool success = false;

            DebugLog.LogMessage("Start LoadCharacterBundle");

            if (File.Exists(filePath) && Path.GetExtension(filePath) == ".cbb")
            {
                AssetBundle bundle = null;
                try
                {
                    bundle = AssetBundle.LoadFromFile(filePath);
                }
                catch (Exception)
                {
                    DebugLog.LogWarning($"File at {filePath} is not a {PluginInfo.PLUGIN_NAME} movestyle bundle, it will not be loaded");
                }

                if (bundle != null)
                {
                    DebugLog.LogMessage("Found Bundle");
                    GameObject[] objects = bundle.LoadAllAssets<GameObject>();
                    MoveStyleDefinition moveStyleDefinition = null;
                    foreach (GameObject obj in objects)
                    {
                        moveStyleDefinition = obj.GetComponent<MoveStyleDefinition>();
                        if (moveStyleDefinition != null)
                        {
                            break;
                        }
                    }
                    if (moveStyleDefinition != null)
                    {
                        DebugLog.LogMessage("Found MovestyleDefinition");
                        string fileName = Path.GetFileName(filePath);

                        DebugLog.LogMessage($"GrindSpeed = {moveStyleDefinition.grindSpeed}");

                        /*
                         * Reimplement Json at somepoint
                        string potentialConfigPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".json");
                        if (File.Exists(potentialConfigPath))
                        {
                            string configData = File.ReadAllText(potentialConfigPath);
                            try
                            {
                                CharacterConfig config = JsonUtility.FromJson<CharacterConfig>(configData);
                                if (Enum.TryParse(config.CharacterToReplace, out BrcCharacter newCharacterReplacement))
                                {
                                    characterToReplace = newCharacterReplacement;
                                }
                                else
                                {
                                    DebugLog.LogWarning($"The configured replacement character for the bundle {fileName} (\"{config.CharacterToReplace}\") is not a valid character!");
                                }
                            }
                            catch (Exception)
                            {
                                DebugLog.LogError($"Failed to read JSON config for \"{fileName}\"");
                            }
                        } */

                        StringBuilder characterLog = new StringBuilder();
                        characterLog.Append($"Loading \"{moveStyleDefinition.Movestylename}\"");

                        characterLog.Append("...");
                        DebugLog.LogMessage(characterLog.ToString());

                        if (Guid.TryParse(moveStyleDefinition.Id, out Guid id))
                        {
                            DebugLog.LogInfo($"GUID: {id}");

                            if (_moveStyleBundlePaths.ContainsKey(id))
                            {
                                DebugLog.LogWarning($"Movestyle GUID already exists. Make sure to not have duplicate character bundles.");
                                return false;
                            }

                            success = true;

                            _moveStyleBundlePaths.Add(id, filePath);

                            SfxCollectionID sfxID = SfxCollectionID.NONE;
                            
                            
                            NewCharacterCount++;

                            MoveStyle newCharacter = MoveStyle.MAX + NewCharacterCount;
                            sfxID = SfxCollectionID.MAX + NewCharacterCount;

                            if (_moveStyleIds.ContainsKey(newCharacter))
                            {
                                _moveStyleIds[newCharacter].Add(id);
                            }
                            else
                            {
                                _moveStyleIds.Add(newCharacter, new List<Guid>()
                                {
                                    id
                                });
                            }

                            DebugLog.LogMessage("Create CustomMoveStyle");
                            //Create a new custom character instance and store it
                            CustomMoveStyle customMoveStyle = new CustomMoveStyle(moveStyleDefinition, sfxID, (int)newCharacter);
                            _customMoveStyle.Add(id, customMoveStyle);
                        }
                        else
                        {
                            DebugLog.LogError($"This character's GUID (\"{moveStyleDefinition.Id}\") is invalid! Make sure their bundle was built correctly.");
                        }
                    }
                    else
                    {
                        DebugLog.LogWarning($"The asset bundle at \"{filePath}\" does not have a MovestyleDefinition. You may be trying to load a movestyle that was made with a different version of this plugin.");
                    }

                    //bundle.Unload(false);
                }
            }

            return success;
        }

        /*
        public static void SetOutfitShader(Shader shader)
        {
            if (shader == null)
            {
                return;
            }

            foreach (CustomCharacter character in _customCharacters.Values)
            {
                character.ApplyShaderToOutfits(shader);
            }
        }*/

        private static void InitializeAPI()
        {
            Dictionary<int, Guid> userCharacters = new Dictionary<int, Guid>();

            int max = (int)MoveStyle.MAX;
            for (int i = max + 1; i <= max + NewCharacterCount; i++)
            {
                if (GetFirstOrConfigMoveStyleId((MoveStyle)i, out Guid id))
                {
                    userCharacters.Add(i, id);
                }
            }

            CrewBoomAPIDatabase.Initialize(userCharacters);
        }

        public static void InitializeMissingSfxCollections(MoveStyle character, SfxCollection collection)
        {
            if (_moveStyleIds.TryGetValue(character, out List<Guid> replacements))
            {
                if (replacements != null && replacements.Count > 0)
                {
                    foreach (Guid guid in replacements)
                    {
                        if (GetCharacter(guid, out CustomMoveStyle customCharacter))
                        {
                            //customCharacter.ApplySfxCollection(collection);
                        }
                    }
                }
            }
        }

        public static bool GetCharacterNameWithId(int localizationId, out string name)
        {
            name = string.Empty;

            switch (localizationId)
            {
                default:
                    return false;
            }
        }
        private static bool GetCharacterName(MoveStyle character, out string name)
        {
            name = string.Empty;

            if (GetCharacter(character, out CustomMoveStyle customMove))
            {
                name = customMove.Definition.Movestylename;
                return true;
            }

            return false;
        }

        public static bool GetFirstOrConfigMoveStyleId(MoveStyle character, out Guid guid)
        {
            guid = Guid.Empty;

            /*
            if (HasCharacterOverride)
            {
                DebugLog.LogInfo($"Getting override for {character} with ID {CharacterOverride}");
                if (_moveStyleBundlePaths.ContainsKey(CharacterOverride) && _customMoveStyle.ContainsKey(CharacterOverride))
                {
                    DebugLog.LogInfo("Override was found locally.");
                    guid = CharacterOverride;
                    return true;
                }
            }
            */

            if (!_moveStyleIds.TryGetValue(character, out List<Guid> replacements) ||
                replacements == null ||
                replacements.Count == 0)
            {
                return false;
            }

            /*
            //Check if the config has an override ID for this character
            if (CharacterDatabaseConfig.GetCharacterOverride(character, out Guid id, out bool isDisabled))
            {
                if (_characterBundlePaths.ContainsKey(id) && _customCharacters.ContainsKey(id))
                {
                    guid = id;
                    return true;
                }
            }
            else 
            {
                //If the override is OFF, ignore any skins for the local player
                if (isDisabled)
                {
                    return false;
                }
            }*/

            //If there's no override, just pick the first ID available
            guid = replacements[0];
            return true;
        }

        public static bool GetCharacter(Guid id, out CustomMoveStyle characterObject)
        {
            if (!_customMoveStyle.TryGetValue(id, out characterObject))
            {
                return false;
            }

            return true;
        }
        public static bool GetCharacter(MoveStyle character, out CustomMoveStyle characterObject)
        {
            characterObject = null;

            if (GetFirstOrConfigMoveStyleId(character, out Guid guid))
            {
                GetCharacter(guid, out characterObject);
            }

            return characterObject != null;
        }

        public static bool HasMovestyle(MoveStyle character)
        {
            if (!_moveStyleIds.TryGetValue(character, out List<Guid> replacements))
            {
                return false;
            }

            return replacements != null && replacements.Count > 0;
        }

        public static bool GetCharacterValueFromGuid(Guid guid, out MoveStyle character)
        {
            character = MoveStyle.ON_FOOT;

            foreach (KeyValuePair<MoveStyle, List<Guid>> pair in _moveStyleIds)
            {
                if (pair.Value != null && pair.Value.Contains(guid))
                {
                    character = pair.Key;
                    return true;
                }
            }

            return false;
        }

        public static void SetCustomMoveStylePropMode(Player player, MoveStyle setMoveStyle, bool forceOff = false)
        {
            //Will the player switch to on foot
            bool ToOnfoot = setMoveStyle == MoveStyle.ON_FOOT && !forceOff;

            //Read the internal property on player
            MoveStyle styleEquipt = (MoveStyle)player.GetField("moveStyleEquipped").GetValue(player);

            //Loop Through all Custom Movestyles
            // TODO in future just process the relevant styles eg player.equipt or setMoveStyle#
            foreach ( var style in _customMoveStyle)
            {
                
                CharacterVisual.MoveStylePropMode mode = CharacterVisual.MoveStylePropMode.OFF;

                if ( (int)styleEquipt == style.Value.Index && ToOnfoot) // If currently equipt and going to on foot
                {
                    mode = CharacterVisual.MoveStylePropMode.ON_BACK;
                }
                else if (setMoveStyle == (MoveStyle)style.Value.Index)
                {
                    mode = CharacterVisual.MoveStylePropMode.ACTIVE;
                }
                else
                {
                    mode = CharacterVisual.MoveStylePropMode.OFF;
                }

                DebugLog.LogMessage(String.Format("setProps: {0} Mode : {1} Index : {2} ", style.Value, mode, style.Value.Index));
                //setPropMode
                // Add new method to MoveStyleDefinition
                style.Value.setCustomMoveStylePropsMode(player, (int)mode);
            }

        }

        public static void advancePlayerMovementStyle(Player player, bool reverse = false)
        { 
            MoveStyle equiptMovestyle = (MoveStyle)player.GetField("moveStyleEquipped").GetValue(player);
            int numberOfMovestyles = (int)MoveStyle.MAX + NewCharacterCount;

            MoveStyle newMovestyle = equiptMovestyle;

            if ((int)equiptMovestyle >= numberOfMovestyles )
            {
                //Loop movestyles
                switch (reverse)
                {
                    case false:
                        newMovestyle = (MoveStyle)1;
                        break;
                    case true:
                        newMovestyle -= 1;
                        break;
                }
                
            }
            else if ((int)equiptMovestyle <= 1 )
            {
                switch (reverse)
                {
                    case false:
                        newMovestyle = (MoveStyle)2;
                        break;
                    case true:
                        newMovestyle = (MoveStyle)(numberOfMovestyles);
                        break;
                }
            }
            else
            {
                switch (reverse)
                {
                    case false:
                        newMovestyle += 1;
                        break;
                    case true:
                        newMovestyle -= 1;
                        break;
                }
            }

            //Clean newMovestyle
            if ( (int)newMovestyle == 5 || (int)newMovestyle == 4)
            { 
                switch(reverse)
                { 
                    case false:
                        newMovestyle = (MoveStyle)6;
                        break;
                    case true:
                        newMovestyle = (MoveStyle)3;
                        break;

                }
            }

            DebugLog.LogMessage($"New Movestyle index: {(int)newMovestyle}");

            player.SetCurrentMoveStyleEquipped(0);
            player.SetCurrentMoveStyleEquipped(newMovestyle);

        }
    }
}