using System.Collections;
using System.Collections.Generic;
using TorbuTils.EzPools;
using UnityEngine;

namespace TorbuTils.Audi
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        [SerializeField] private GameObject prefab;
        private List<AudioSource> currentlyPlaying = new();
        internal bool PoolsExist => Pools.Instance != null;
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            if (!PoolsExist)
            {
                Debug.LogWarning($"Pools do not exist at startup. Instantiating and destroying manually.");
            }
        }
        private void Update()
        {
            for (int i = currentlyPlaying.Count-1; i >= 0; i--)
            {
                var audio = currentlyPlaying[i];
                if (!audio.isPlaying)
                {
                    currentlyPlaying.RemoveAt(i);
                    if (PoolsExist) Pools.Instance.Enpool(audio.gameObject);
                    else Destroy(audio.gameObject);
                }
            }
        }

        public void Play(SoundObject sound)
        {
            if (sound == null)
            {
                Debug.LogWarning("Cannot play null sound");
                return;
            }
            if (sound.UseMultipleClips && (sound.MultipleClips == null || sound.MultipleClips.Length == 0))
            {
                Debug.LogWarning($"Sound {sound.Name}, filename {sound.name}, has UseMultipleClips enabled but MultipleClips is null or empty");
                return;
            }

            var pitch = Random.Range(sound.Pitch.x, sound.Pitch.y);
            var clip = sound.UseMultipleClips ? sound.MultipleClips[Random.Range(0, sound.MultipleClips.Length)] : sound.SingleClip;

            if (clip == null)
            {
                Debug.LogWarning($"Sound {sound.Name}, filename {sound.name}, has null clip");
                return;
            }

            var go = GetNewGO();
            var source = go.GetComponent<AudioSource>();

            source.pitch = pitch;
            source.volume = sound.Volume;
            source.clip = clip;

            source.Play();
            currentlyPlaying.Add(source);
        }
        private GameObject GetNewGO()
        {
            if (!PoolsExist) return Instantiate(prefab);
            return Pools.Instance.Depool(prefab);
        }
    }
}
