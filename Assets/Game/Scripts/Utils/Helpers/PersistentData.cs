using UnityEngine;

namespace Game.Scripts.Utils.Helpers
{
    public static class PersistentData
    {
        private static int _level;
        public static int Level
        {
            get
            {
                return PlayerPrefs.GetInt("Level", 1);
            }
            set
            {
                _level = value;
                PlayerPrefs.SetInt("Level", value);
            }
        }
    }
}
