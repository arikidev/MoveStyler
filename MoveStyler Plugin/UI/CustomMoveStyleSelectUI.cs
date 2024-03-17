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

        private static List< KeyValuePair<SimplePhoneButton, MoveStyle>> styleButtons;

        private static UnityEngine.Color defaultSelectedColor;
        private static UnityEngine.Color defaultUnSelectedColor;

        private static UnityEngine.Color equiptSelectedColor;
        private static UnityEngine.Color equiptUnSelectedColor;

        static Sprite Icon = null;

        public static void Initialize()
        {
            Texture2D texture = TextureUtil.GetTextureFromBitmap(Properties.Resources.phoneAppIcon);
            Icon = TextureUtility.CreateSpriteFromTexture(texture);
            PhoneAPI.RegisterApp<MoveStylerApp>("MoveStyles", Icon);

            equiptSelectedColor = new UnityEngine.Color( 1.0f, 0.7f, 0.0f, 1.0f);
            equiptUnSelectedColor = new UnityEngine.Color(1.0f, 0.75f, 0.25f, 1.0f);

            defaultSelectedColor = new Color32(49, 90, 165, 255);
            defaultUnSelectedColor = UnityEngine.Color.white;

            styleButtons = new List<KeyValuePair<SimplePhoneButton, MoveStyle>>();
        }

        public override void OnAppInit()
        {
            base.OnAppInit();

            CreateTitleBar("MoveStylz", Icon);
            ScrollView = PhoneScrollView.Create(this);

            int customMoveStylesMax = moveStyleDatabase.NewMovestyleCount;

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
                    //Add in highlighting the currently selected movestyle

                    var player = WorldHandler.instance.GetCurrentPlayer();
                    moveStyleDatabase.setPlayerMovementStyle(player, style);
                };

                ScrollView.AddButton(button);
                styleButtons.Add(new KeyValuePair<SimplePhoneButton, MoveStyle>(button, style));
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
                styleButtons.Add(new KeyValuePair<SimplePhoneButton, MoveStyle>(button, style));
            }
        }

        public void setButtonState()
        {
            if (true)
            {
                var player = WorldHandler.instance.GetCurrentPlayer();

                foreach (KeyValuePair<SimplePhoneButton, MoveStyle> pair in styleButtons)
                {
                    MoveStyle equiptMovestyle = (MoveStyle)player.GetField("moveStyleEquipped").GetValue(player);

                    if (equiptMovestyle == pair.Value)
                    {
                        pair.Key.LabelSelectedColor = equiptSelectedColor;
                        pair.Key.LabelUnselectedColor = equiptUnSelectedColor;
                    }
                    else 
                    {
                        pair.Key.LabelSelectedColor = defaultSelectedColor;
                        pair.Key.LabelUnselectedColor = defaultUnSelectedColor;
                    }
                }
            }
        }
    }
}
