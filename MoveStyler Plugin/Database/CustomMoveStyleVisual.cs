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

        public bool LHandIKCurrent = false;
        public bool RHandIKCurrent = false;

        private Player player;
        private CharacterVisual characterVisual;

        public CustomMoveStyleVisualParent()
        {
            this.CustomMoveStylesList = new List<KeyValuePair<MoveStyle, CustomMoveStyleVisual>>();
            this.player = base.transform.parent.parent.GetComponentInParent<Player>();  
            this.characterVisual = base.transform.parent.GetComponentInParent<CharacterVisual>();
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
                    prop.Value.SetPropsActive(false, characterVisual);
                }
                else if (setMoveStyle == prop.Key)
                {
                    mode = CharacterVisual.MoveStylePropMode.ACTIVE;
                    prop.Value.SetPropsActive(true , characterVisual);
                }
                else
                {
                    mode = CharacterVisual.MoveStylePropMode.OFF;
                    prop.Value.SetPropsActive(false, characterVisual);
                }
            }
        }

        public void SetupPropVisuals(CharacterVisual characterVisual)
        {

            Transform characterVisualTransform = characterVisual.gameObject.transform;

            foreach (KeyValuePair<MoveStyle, CustomMoveStyleVisual> style in CustomMoveStylesList)
            {
                foreach (KeyValuePair<string, GameObject> prop in style.Value.Props)
                {
                    Transform bone = characterVisualTransform.FindRecursive(prop.Key);
                    
                    //Fix for Crewboom skateOffset
                    Transform offetR = bone.FindChild("skateOffsetR");
                    Transform offetL = bone.FindChild("skateOffsetL");
                    if (offetR !=null)
                    {
                        bone = offetR;
                    }
                    if (offetL != null)
                    {
                        bone = offetL;
                    }

                    prop.Value.transform.SetParent(bone, false);
                    prop.Value.transform.SetToIdentity();
                    prop.Value.SetActive(true);
                }
            }
        }

        static public CustomMoveStyleVisualParent GetCustomMoveStyleVisualParent(CharacterVisual Char)   
        {
            CustomMoveStyleVisualParent parent = Char.anim.GetComponentInChildren<CustomMoveStyleVisualParent>();

            return parent;
        }

        //Custom Animation Events handelers

        public void MSAnimEvent(AnimationEvent animationEvent)
        {
            DebugLog.LogMessage($"Triggering AnimEvent : {animationEvent.stringParameter}");

            string[] strings = animationEvent.stringParameter.Split('.');

            switch (strings[0])
            {
                case "layer":
                    if (strings.Length > 2)
                    {
                        int ind = -1; ind = characterVisual.anim.GetLayerIndex(strings[1]);
                        float weight = -1; weight =  float.Parse(strings[2]);

                        if (ind > 0 && weight!= -1)
                        { 
                            characterVisual.anim.SetLayerWeight(ind, weight);
                            DebugLog.LogMessage($"set layer weight : {strings[1]} = {weight} ");
                        }
                    }

                    break;

                case "parameter":
                    if (strings.Length > 3)
                    {
                        float weight = -1; weight = float.Parse(strings[3]);

                        if (strings[1] == "int")
                        {
                            if (weight != -1)
                            {
                                characterVisual.anim.SetInteger(strings[2], (int)weight);
                                DebugLog.LogMessage($"set parameter int : {strings[2]} = {characterVisual.anim.GetInteger(strings[2])} ");
                            }
                        }
                        else if (strings[1] == "float")
                        {
                            if (weight != -1)
                            {
                                characterVisual.anim.SetFloat(strings[2], weight);
                                DebugLog.LogMessage($"set parameter float : {strings[2]} = {weight} ");
                            }
                        }
                    }

                    break;

                default:

                    DebugLog.LogMessage($"AnimEvent : *{strings[0]}* not found");

                    break;          
            }

            return;
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

        public void SetPropsActive(bool active, CharacterVisual visual)
        {

            Transform characterVisualTransform = visual.gameObject.transform;

            foreach (KeyValuePair<string, GameObject> Prop in _Props)
            {
                Prop.Value.SetActive(active);
                if (active == true)
                { 
                    Transform bone = characterVisualTransform.FindRecursive(Prop.Key);

                    //Fix for Crewboom skateOffset
                    Transform offetR = bone.FindChild("skateOffsetR");
                    Transform offetL = bone.FindChild("skateOffsetL");
                    if (offetR != null)
                    {
                        bone = offetR;
                    }
                    if (offetL != null)
                    {
                        bone = offetL;
                    }

                    Prop.Value.transform.SetParent(bone, false);
                    Prop.Value.transform.SetToIdentity();
                }
            }     
        }
    }
}
