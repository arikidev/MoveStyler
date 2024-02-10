using CommonAPI;
using Reptile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx.Logging;

namespace MoveStyler.UI
{
    public class MovestylerEmailSave : CustomSaveData
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} MoveStylerEmailManager");

        public static MovestylerEmailSave Instance { get; private set; }

        private readonly Dictionary<string, bool> messageStates;
        
        public MovestylerEmailSave() : base(PluginInfo.PLUGIN_NAME, "{0}_msg.data")
        {
            Instance = this;
            messageStates = new Dictionary<string, bool>();
        }

        public bool hasReadData = false;

        public void setMessageState(string messageName, bool state)
        {
            if (messageStates.ContainsKey(messageName))
            {
                messageStates[messageName] = state;
                return;
            }
            messageStates.Add(messageName, state);
        }

        public bool getMessageState(string messageName)
        {
            if (messageStates.ContainsKey(messageName))
            {
                return messageStates[messageName];
            }
            return false;
        }

        // Starting a new save - start from zero.
        public override void Initialize()
        {
            messageStates.Clear();
        }

        public override void Read(BinaryReader reader)
        {
            var version = reader.ReadByte();          
            var messages = reader.ReadInt32();

            for (var i = 0; i < messages; i++)
            {
                var msgID = reader.ReadString();
                var msgState = reader.ReadBoolean();

                if (!messageStates.ContainsKey(msgID))
                {
                    messageStates.Add(msgID, msgState);
                }
                
            }
            hasReadData = true;
        }

        public override void Write(BinaryWriter writer)
        {
            // Version
            writer.Write((byte)0);
            writer.Write(messageStates.Count);
            foreach (var messageState in messageStates)
            {
                writer.Write(messageState.Key);
                writer.Write(messageState.Value); 
            }
        }
    }
}

