using System;
using UnityEngine;

namespace Game.Scripts.Managers.Audio.Models
{
    [Serializable]
    public struct SoundClip
    {
        public string name;
        public AudioClip clip;
        public float volume;
        public float pitch;
        public bool loop;
    }
}
