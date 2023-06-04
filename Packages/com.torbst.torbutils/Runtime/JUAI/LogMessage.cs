using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TorbuTils.JUAI
{
    public class LogMessage : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI Message { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Stack { get; private set; }
        [field: SerializeField] public Image BGColorPanel { get; private set; }
        [field: SerializeField] public Console.Category Category { get; set; }
    }
}
