using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TorbuTils.Audi
{
    [CreateAssetMenu(menuName = "SO/Sound")]
    public class SoundObject : ScriptableObject
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField, Range(0f, 1f)] public float Volume { get; set; } = 1f;
        [field: SerializeField] public Vector2 Pitch { get; set; } = new(0.9f, 1.1f);

        public bool UseMultipleClips { get; set; }

        [SerializeField, HideInInspector] private AudioClip singleClip;
        [SerializeField, HideInInspector] private AudioClip[] multipleClips;

        public AudioClip SingleClip
        {
            get => singleClip;
            set => singleClip = value;
        }

        public AudioClip[] MultipleClips
        {
            get => multipleClips;
            set => multipleClips = value;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                if (!UseMultipleClips)
                    if (SingleClip != null)
                        Name = SingleClip.name;
                else
                    if (multipleClips != null && multipleClips.Length > 0 && multipleClips[0] != null)
                        Name = multipleClips[0].name;
            }
        }
    }
}

