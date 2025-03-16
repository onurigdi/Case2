using System;
using UnityEngine;

namespace Game.Scripts.Managers.Audio.Models
{
    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public bool loop;
        
        [Range(0f, 1f)] public float volume;
        [Range(.1f, 3f)] public float pitch;
        [HideInInspector] public AudioSource source;
        
        
        public Sound(string name, AudioClip clip, float volume, float pitch, bool loop, AudioSource source)
        {
            this.name = name;
            this.clip = clip;
            this.volume = volume;
            this.pitch = pitch;
            this.loop = loop;
            this.source = source;
        }
    }
}