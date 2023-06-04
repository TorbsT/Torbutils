using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TorbuTils.JUAI
{
    public class LogCategoryButton : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI Name { get; private set; }
        [field: SerializeField] public Image Background { get; private set; }
        public bool Active { get; set; } = true;

        public void Clicked()
        {
            Console.Instance.CategoryButtonClicked(this);
        }
    }
}
