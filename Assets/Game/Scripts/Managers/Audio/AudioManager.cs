using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Managers.Audio.Enums;
using Game.Scripts.Managers.Audio.Models;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Scripts.Managers.Audio
{
    public class AudioManager : IInitializable, IDisposable
    {
        private readonly DiContainer _diContainer;

        private readonly List<Sound> _sounds;
        private readonly List<SoundClip> _audioClips;
        private object _loopedSound;

        private readonly IDisposable _disposable;

        public AudioManager(List<SoundClip> audioClips, DiContainer diContainer)
        {
            _audioClips = audioClips;
            _diContainer = diContainer;
            _sounds = new List<Sound>();
        }

        public void Initialize()
        {
                foreach (var ac in _audioClips)
                {
                    var audioSource = _diContainer.InstantiateComponentOnNewGameObject<AudioSource>();
                    audioSource.volume = 0;
                    audioSource.clip = ac.clip;
                    var sound = new Sound(ac.name, audioSource.clip, ac.volume, ac.pitch, ac.loop, audioSource);
                    _sounds.Add(sound);
                }
            }
       
        public void Play(SoundType soundName)
        {
            PlaySound(soundName.ToString());
        }
        public void PlayPitchIncreased(SoundType soundName,float pitchOverride = 1f)
        {
            PlaySound(soundName.ToString(),false,false,pitchOverride);
        }

        public void PlayOnce(SoundType soundName, bool playOnce = true, bool loop = false)
        {
            PlaySound(soundName.ToString(), playOnce);
        }

        public void PlayLooped(SoundType soundName)
        {
            PlaySound(soundName.ToString(), loop:true);
        }

        private void PlaySound(string soundName, bool playOnce = false, bool loop = false,float pitchOverride = 1)
        {
            var sound = _sounds.FirstOrDefault(s => s.name == soundName);
            if (sound == null)
            {
                Debug.Log($"Sound not found : {soundName}");
                return;
            }
            sound.source.volume = sound.volume;
            
            sound.source.pitch = pitchOverride != 1f ? pitchOverride : sound.pitch;
            if (sound.source == null)
            {
                sound.source = _diContainer.InstantiateComponentOnNewGameObject<AudioSource>();
            }

            if (playOnce)
            {
                sound.source.PlayOneShot(sound.clip, 0.7f);
                return;
            }

            if (loop)
            {
                sound.source.loop = true;
            }

            sound.source.Play();
            
        }

        public void Stop(SoundType soundName)
        {
            foreach (var sound in _sounds.Where(sound => sound.name == soundName.ToString()))
            {
                sound.source.Stop();
                break;
            }

        }
        
        public void StopPlayingAllSounds(bool withGameSound = true)
        {
            foreach (var sound in _sounds)
            {
                sound.source.Stop();
            }
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        
            foreach (var sound in _sounds)
            {
                sound.source.Stop();
                sound.source.clip = null;
                sound.source = null;
            }

            _sounds?.Clear();
            
        }
    }
}