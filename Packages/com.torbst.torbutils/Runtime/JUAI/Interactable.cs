using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TorbuTils.JUAI
{
    public class Interactable : MonoBehaviour
    {
        [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
        [field: SerializeField] public Image Image { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public bool Trigger { get; private set; } = false;
        public bool Hovered { get; set; }
        public bool Selected { get; set; }

        private void OnValidate()
        {
            if (Animator != null) return;
            if (SpriteRenderer == null)
                SpriteRenderer = GetComponent<SpriteRenderer>();
            if (Image == null)
                Image = GetComponent<Image>();
        }
    }
}