namespace TorbuTils.Anime
{
    internal class FloatController : AnimController<float> { protected override float GetActualValue(float startValue, float endValue, float relative) => startValue + ((endValue - startValue) * relative); }
}
