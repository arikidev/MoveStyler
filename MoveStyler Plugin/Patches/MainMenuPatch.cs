using MoveStyler.Utility;
using HarmonyLib;
using Reptile;
using System.Drawing;
//using System;
using UnityEngine;

namespace MoveStyler.Patches
{
    [HarmonyPatch(typeof(Reptile.MainMenuManager), nameof(Reptile.MainMenuManager.Init))]
    public class MainMenuPatch
    {
        public static void Postfix(MainMenuManager __instance)
        {
            Texture2D texture = TextureUtil.GetTextureFromBitmap(Properties.Resources.logo_background);

            int padding = 15;
            int MaxWidth = (int)(Screen.width * 0.5f - padding);
            int MaxHeight = (int)(Screen.height * 0.35f - padding);

            GameObject logo = new GameObject("MoveStyler Logo");
            logo.transform.SetParent(__instance.transform, false);

            UnityEngine.UI.Image image = logo.AddComponent<UnityEngine.UI.Image>();
            image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            RectTransform rect = logo.RectTransform();
            rect.sizeDelta = new Vector2(texture.width, texture.height);
            rect.anchorMin = new Vector2(0.0f, 0.85f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = rect.anchorMin;
            rect.anchoredPosition = new Vector2(Random.Range(padding, MaxWidth), Random.Range(padding, -MaxHeight));
            rect.rotation = Quaternion.Euler( 0, 0, Random.Range(-35, 35));
        
            Vector2 randPos = new Vector2(Random.Range(0, MaxWidth), Random.Range(0, MaxHeight));
        }
    }
}
