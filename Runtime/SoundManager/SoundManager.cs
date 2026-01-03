using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace BJ
{
    /**
     * @brief SoundManager is a quick way to readily play froma  bank of sound, each sound can have custom settings as though it were it's own audio source.
     *                     Under the hood this script creates audio sources for each sound to play and provides an easy interface to these sounds.
     */
    public class SoundManager : MonoBehaviour
    {
        // TODO upgrade to Serializable Hashmap when it's ready
        // TODO Sounds is a candidate for a custom editor at some point.
        [SerializeField]
        [Tooltip("The set of sounds this object can play, each one can be tuned separately.")]
        private List<PlayableSound> sounds;
        // Internal storage of sounds after initialization.
        private Dictionary<string, PlayableSound> soundMap;

        private void Awake()
        {
            soundMap = new Dictionary<string, PlayableSound>();

            for (int i = 0, count = sounds.Count; i < count; i++)
            {
                PlayableSound sound = sounds[i];

                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.outputAudioMixerGroup = sound.mixerGroup;
                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.playOnAwake = sound.playOnAwake;
                sound.source.loop = sound.loop;

                if (!soundMap.ContainsKey(sound.name))
                {
                    soundMap.Add(sound.name, sound);
                    if (sound.playOnAwake)
                    {
                        sound.source.Play();
                    }
                }
                else
                {
                    Debug.LogError($"Duplicate sound {sound.name} found.");
                }
            }
        }

        /**
         * @brief Plays the specified sound immediately. If the sound is not found logs an error instead.
         * @param sound The assigned name of the sound to play.
         */
        public void PlaySound(string sound)
        {
            if (soundMap.TryGetValue(sound, out PlayableSound playable))
            {
                playable.source.Play();
            }
            else
            {
                Debug.LogError($"Sound {sound} not found");
            }
        }

        /**
         * @brief Stops the specified sound immediately. If the sound is not found logs an error instead.
         * @param sound The assigned name of the sound to stop.
         */
        public void StopSound(string sound)
        {
            if (soundMap.TryGetValue(sound, out PlayableSound playable))
            {
                playable.source.Stop();
            }
            else
            {
                Debug.LogError($"Sound {sound} not found");
            }
        }
    }

    /**
     * @brief PlayableSound has properties that mirror an AudioSource, they store the data that will be converted into an AudioSource on awake.
     *        Only name is new, this is what will be used internally to identify the sound.
     */
    [System.Serializable]
    public class PlayableSound
    {
        public string name;
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;

        [Range(0f, 1f)]
        public float volume = 1.0f;
        [Range(0f, 1.5f)]
        public float pitch = 1.0f;

        public bool loop;
        public float spatialBlend;

        public bool playOnAwake;

        [HideInInspector]
        public AudioSource source;
    }
}
