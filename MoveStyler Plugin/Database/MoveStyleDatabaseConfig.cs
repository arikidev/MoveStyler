using BepInEx.Configuration;
using MoveStylerMono;
using Reptile;
using System;

namespace MoveStyler
{
    public static class MoveStyleDatabaseConfig
    {
        private const string CONFIG_DESCRIPTION = "Enter a GUID of a character bundle to always load for {0} (Blank = Auto-detect, \"OFF\" = Default character for you)";

        private static ConfigEntry<string>[] _moveStyleIdOverrides;

        public static void Initialize(ConfigFile config)
        {
            var values = Enum.GetValues(typeof(MoveStyle));
            _moveStyleIdOverrides = new ConfigEntry<string>[values.Length - 1];
            foreach (MoveStyle character in values)
            {
                if (character == MoveStyle.ON_FOOT || character == MoveStyle.MAX)
                {
                    continue;
                }

                BrcMovestyle characterName = (BrcMovestyle)character;
                _moveStyleIdOverrides[(int)character] = config.Bind<string>("Replacement IDs", characterName.ToString(), null, string.Format(CONFIG_DESCRIPTION, characterName)); ;
            }
        }

        public static bool GetMovestyleOverride(MoveStyle character, out Guid id, out bool isDisabled)
        {
            id = Guid.Empty;
            isDisabled = false;

            if (character > MoveStyle.MAX)
            {
                return false;
            }

            string guidString = _moveStyleIdOverrides[(int)character].Value;
            if (guidString == string.Empty)
            {
                return false;
            }
            if (guidString == "OFF")
            {
                isDisabled = true;
                return false;
            }

            if (Guid.TryParse(guidString, out id))
            {
                return true;
            }

            return false;
        }
    }
}
