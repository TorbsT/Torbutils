using UnityEngine;

namespace TorbuTils.Anime
{
    internal class QuaternionController : AnimController<Quaternion> { protected override Quaternion GetActualValue(Quaternion startValue, Quaternion endValue, float relative) => Quaternion.LerpUnclamped(startValue, endValue, relative); }
}
