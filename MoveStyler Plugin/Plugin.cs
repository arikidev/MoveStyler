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
                Harmony harmony = new Harmony("Ariki.moveStyler");
                harmony.PatchAll();

                Logger.LogMessage($"Loaded all available movestyles!");
            }

            Logger.LogMessage($"Init Phone App");
            MoveStylerApp.Initialize();

            MoveStylerEmailManager.Initialize();

            

        }

        void Update()
        {
            /*
            if (Input.GetKeyDown(KeyCode.G))
            {
                Logger.LogMessage($"Try Push Email Test");
                //MoveStylerEmailManager.TryEmail();
            }
            
            if (Input.GetKeyDown(KeyCode.H))
            {
                Logger.LogMessage($"Try Push Email Test");
                MoveStylerEmailManager.TryEmail();
            }
            */
        }

    }
}
