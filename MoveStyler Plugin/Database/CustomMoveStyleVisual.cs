using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using Reptile;
using UnityEngine;
using MoveStyler.Data;

namespace MoveStyler.Data
{
    public class CustomMoveStyleVisualParent : MonoBehaviour
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} CharMoveStyle Visual Parent");

        public List<KeyValuePair<MoveStyle, CustomMoveStyleVisual>> CustomMoveStylesList;

        public CustomMoveStyleVisualParent()
        {
            CustomMoveStylesList = new List<KeyValuePair<MoveStyle, CustomMoveStyleVisual>>();
        }

        public void SetCustomMoveStyleVisualsPropMode(Player player, MoveStyle setMoveStyle, bool forceOff = false)
        {
            //Will the player switch to on foot
            bool ToOnfoot = setMoveStyle == MoveStyle.ON_FOOT && !forceOff;

            //Read the internal property on player
            MoveStyle styleEquipt = (MoveStyle)player.GetField("moveStyleEquipped").GetValue(player);

            //Loop Through all Custom Movestyles
            foreach (KeyValuePair<MoveStyle, CustomMoveStyleVisual> prop in CustomMoveStylesList)
            {
                CharacterVisual.MoveStylePropMode mode = CharacterVisual.MoveStylePropMode.OFF;

                if ((int)styleEquipt == (int)prop.Key && ToOnfoot) // If currently equipt and going to on foot
                {
                    mode = CharacterVisual.MoveStylePropMode.ON_BACK;
                    prop.Value.SetPropsActive(false);
                }
                else if (setMoveStyle == prop.Key)
                {
                    mode = CharacterVisual.MoveStylePropMode.ACTIVE;
                    prop.Value.SetPropsActive(true);
                }
                else
                {
                    mode = CharacterVisual.MoveStylePropMode.OFF;
                    prop.Value.SetPropsActive(false);
                }
            }
        }

        public void SetupPropVisuals(CharacterVisual characterVisual)
        {
            
            Transform characterVisualTransform = characterVisual.gameObject.transform;

            foreach (KeyValuePair<MoveStyle, CustomMoveStyleVisual> style in CustomMoveStylesList )
            {
                foreach (KeyValuePair<string, GameObject> prop in style.Value.Props)
                {
                    Transform bone = characterVisualTransform.FindRecursive(prop.Key);
                    prop.Value.transform.SetParent(bone, false);
                    prop.Value.transform.SetToIdentity();
                    prop.Value.SetActive(true);
                }
            }
        }
    }

    public class CustomMoveStyleVisual
    {
        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} CharMoveStyle Visual");

        public List<KeyValuePair<string, GameObject>> Props
        {
            get
            {
                if (_Props != null)
                {
                    return _Props;
                }
                return null;
            }
        }
        private List<KeyValuePair<string, GameObject>> _Props;

        // Constructor
        public CustomMoveStyleVisual()
        {
            //DebugLog.LogMessage("Init");
            _Props = new List<KeyValuePair<string, GameObject>>();
        }

        public void AddPropObject(GameObject obj, string attachment)
        {
            //DebugLog.LogMessage("Add Prop Object");
            _Props.Add(new KeyValuePair<string, GameObject>(attachment, obj));
        }

        public void SetPropsActive(bool active)
        {
            foreach (KeyValuePair<string, GameObject> Prop in _Props)
            {
                Prop.Value.SetActive(active);   
            }     
        }

    }
}
