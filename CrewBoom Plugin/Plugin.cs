using System;
using System.IO;
using Reptile;
using UnityEngine;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;

namespace MoveStyler
{
    //[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInPlugin("Ariki.MoveStylers", "MoveStylers", PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(CharacterAPIGuid, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string CharacterAPIGuid = "com.Viliger.CharacterAPI";

        private void Awake()
        {
            if (Chainloader.PluginInfos.ContainsKey(CharacterAPIGuid))
            {
                Logger.LogWarning("CrewBoom is incompatible with CharacterAPI (viliger) and will not load!\nUninstall CharacterAPI and restart the game if you want to use CrewBoom.");
                return;
            }

            Logger.LogMessage($"{PluginInfo.PLUGIN_GUID} v{PluginInfo.PLUGIN_VERSION} starting...");

            MoveStyleDatabaseConfig.Initialize(Config);

            if (moveStyleDatabase.Initialize())
            {
                Harmony harmony = new Harmony("ariki.moveStyler");
                harmony.PatchAll();

                Logger.LogMessage($"Loaded all available movestyles!");
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                ChangeplayerMovestyle(false);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                ChangeplayerMovestyle(true);
            }
        }

        private void ChangeplayerMovestyle(bool reverse = false)
        {
            Player localPlayer = WorldHandler.instance.GetCurrentPlayer();
            if (localPlayer == null) { return; }

            moveStyleDatabase.advancePlayerMovementStyle(localPlayer, reverse);
        }

    }
}
