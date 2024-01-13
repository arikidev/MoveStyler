using System;
using System.IO;
using Reptile;
using UnityEngine;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using MoveStyler.UI;

namespace MoveStyler
{
    //[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInPlugin("Ariki.MoveStylers", "MoveStylers", PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(CommonAPIGUID, BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string CharacterAPIGuid = "com.Viliger.CharacterAPI";
        private const string CommonAPIGUID = "CommonAPI";

        private void Awake()
        {

            Logger.LogMessage($"{PluginInfo.PLUGIN_GUID} v{PluginInfo.PLUGIN_VERSION} starting...");

            MoveStyleDatabaseConfig.Initialize(Config);

            if (moveStyleDatabase.Initialize())
            {
                Harmony harmony = new Harmony("ariki.moveStyler");
                harmony.PatchAll();

                Logger.LogMessage($"Loaded all available movestyles!");
            }

            Logger.LogMessage($"Init Phone App");
            MoveStylerApp.Initialize();
        }

        void Update()
        {
            /*
            if (Input.GetKeyDown(KeyCode.G))
            {
                ChangeplayerMovestyle(false);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                ChangeplayerMovestyle(true);
            }
            */
        }

        private void ChangeplayerMovestyle(bool reverse = false)
        {
            Player localPlayer = WorldHandler.instance.GetCurrentPlayer();
            if (localPlayer == null) { return; }
      
            moveStyleDatabase.advancePlayerMovementStyle(localPlayer, reverse);

        }
    }
}
