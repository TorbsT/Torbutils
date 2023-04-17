using UnityEngine;

namespace TorbuTils.Anime
{
    internal class ColorController : AnimController<Color> { protected override Color GetActualValue(Color startValue, Color endValue, float relative) => Color.LerpUnclamped(startValue, endValue, relative); }
}
