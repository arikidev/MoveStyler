using UnityEngine;
using Reptile;
using System.IO;
using System.Collections.Generic;
using System;
using System.Drawing;
using BepInEx.Logging;
using BepInEx;
using TMPro;
using CommonAPI;
using CommonAPI.Phone;
using MoveStyler.Utility;


namespace MoveStyler.UI
{
    public class MoveStylerApp : CustomApp
    {

        List<SimplePhoneButton> styleButtons;

        static Sprite Icon = null;

        public static void Initialize()
        {
            Texture2D texture = TextureUtil.GetTextureFromBitmap(Properties.Resources.phoneAppIcon);
            Icon = TextureUtility.CreateSpriteFromTexture(texture);
            PhoneAPI.RegisterApp<MoveStylerApp>("MoveStyles", Icon);
        }

        
        public override void OnAppInit()
        {
            base.OnAppInit();

            CreateTitleBar("MoveStylz", Icon);
            ScrollView = PhoneScrollView.Create(this);

            int customMoveStylesMax = moveStyleDatabase.NewCharacterCount;

            for (int i = 1; i < 4; i++)
            {
                MoveStyle style = (MoveStyle)i;

                string name;
                switch (i)
                {
                    case 2:
                        name = "Skateboard";
                        break;

                    case 3:
                        name = "Inline";
                        break;

                    default:
                        name = "BMX";
                        break;
                }

                var button = PhoneUIUtility.CreateSimpleButton(name);
                button.OnConfirm += () =>
                {
                    var player = WorldHandler.instance.GetCurrentPlayer();
                    moveStyleDatabase.setPlayerMovementStyle(player, style);
                };

                ScrollView.AddButton(button);
            }

            for (int i = 1; i <= customMoveStylesMax; i++)
            {
                MoveStyle style = MoveStyle.MAX + i;

                string name;
                moveStyleDatabase.GetCharacterName(style, out name);
                var button = PhoneUIUtility.CreateSimpleButton(name);
                button.OnConfirm += () =>
                {
                    var player = WorldHandler.instance.GetCurrentPlayer();
                    moveStyleDatabase.setPlayerMovementStyle(player, style);
                };

                ScrollView.AddButton(button);
            }
        }
    }

}
