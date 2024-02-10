using UnityEngine;
using System.Collections.Generic;
using Reptile;
using System;
using System.Drawing;
using BepInEx.Logging;
using BepInEx;
using TMPro;
using CommonAPI;
using CommonAPI.Phone;
using Reptile.Phone;
using MoveStyler.Utility;

namespace MoveStyler.UI
{
    public static class MoveStylerEmailManager
    {

        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} MoveStylerEmailManager");

        static int msgSenderID = 9001;
        static UnityEngine.Color msgSenderColor = new UnityEngine.Color(1f, 0.9f, 0.9f);

        public static Player player;
        public static Phone phone;
        public static AppEmail emailApp;

        public static EmailMessage msg_tutorial;
        public static Dictionary<int, CustomContact> _MSContacts { get; private set; }

        public static List<KeyValuePair<EmailMessage, bool>> _messagesDatabase;
        public static Dictionary<string, EmailMessage> _messagesIDDatabase;

        private static MovestylerEmailSave SaveState;

        public static void Initialize()
        {
            DebugLog.LogMessage($"Init Email Manager");

            //player = WorldHandler.instance.GetCurrentPlayer();
            //phone = (Phone)player.GetField("phone").GetValue(player);
            //emailApp = phone.GetAppInstance<AppEmail>();

            _messagesDatabase = new List<KeyValuePair<EmailMessage, bool>>();
            _messagesIDDatabase = new Dictionary<string, EmailMessage>();

            InitLoadEmailSave();
            InitCustomContacts();
            InitEmailMessages();
        }

        private static void InitLoadEmailSave()
        {
            new MovestylerEmailSave();

            if (MovestylerEmailSave.Instance.hasReadData)
            { 
                
            }
        }

        private static void InitCustomContacts()
        {
            _MSContacts = new Dictionary<int, CustomContact>();

            CustomContact MS_Contact = new CustomContact(msgSenderID, "Movestyler");
            Texture2D texture = TextureUtil.GetTextureFromBitmap(Properties.Resources.phoneAppIcon);
            MS_Contact.avatar = TextureUtility.CreateSpriteFromTexture(texture);

            _MSContacts.Add(MS_Contact.characterID, MS_Contact);
        }
        private static void InitEmailMessages()
        {
            msg_tutorial = ScriptableObject.CreateInstance<EmailMessage>();
            msg_tutorial.name = "msg1";
            msg_tutorial.characterNameIDOfSender = msgSenderID;
            msg_tutorial.subject = "Intro";
            msg_tutorial.profileBackground = msgSenderColor;

            List<string> messages = new List<string>();
            messages.Add( "You can access new movestyles through the new movestyler app on your phone");
            messages.Add("New movestyles can be copied into the movestyler config folder");
            msg_tutorial.message = messages.ToArray();

            _messagesDatabase.Add(new KeyValuePair<EmailMessage, bool>(msg_tutorial,true));
            _messagesIDDatabase.Add(msg_tutorial.name, msg_tutorial);
        }

        public static void EmailNotification(string MessageID, bool setSave = true )
        {
            player = WorldHandler.instance.GetCurrentPlayer();
            phone = (Phone)player.GetField("phone").GetValue(player);
            emailApp = phone.GetAppInstance<AppEmail>();

            if (emailApp != null)
            {
                if (_messagesIDDatabase.TryGetValue(MessageID, out EmailMessage message))
                {
                    if (setSave)
                    {
                        MovestylerEmailSave.Instance.setMessageState(MessageID, setSave);
                    }
                    
                    emailApp.PushNotification(message);
                }
                
            }
     
        }

    }

    public struct CustomContact
    {
        public int CompareTo(Contact other)
        {
            return this.characterID.CompareTo(other.characterID);
        }

        public CustomContact(int characterID, string characterName)
        {
            this.characterID = characterID;
            this.characterName = characterName;
            this.avatar = null;
            this.associatedCharacterGuid = Guid.Empty;
        }

        // Token: 0x04001D98 RID: 7576
        public int characterID;

        public string characterName;

        // Token: 0x04001D99 RID: 7577
        public Sprite avatar;

        public Guid associatedCharacterGuid;

    }

}
