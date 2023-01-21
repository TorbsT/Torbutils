using System.Collections.Generic;
using UnityEngine;

namespace TorbuTils
{
    namespace Anime
    {
        public abstract class AnimController<T> : MonoBehaviour
        {
            private readonly List<Anim<T>> anims = new();
            private int count;

            internal void Begin(Anim<T> anim)
            {
                anims.Add(anim);
                count = anims.Count;
            } 
            internal void Stop(Anim<T> anim, float stopAtRelative)
            {
                int i = anims.FindIndex(x => x == anim);
                if (i == -1)
                {
                    Debug.LogWarning("Tried stopping an unregistered animation: "+anim);
                } else
                {
                    stopAtRelative = Mathf.Clamp(stopAtRelative, 0f, 1f);
                    float curveAdjusted = GetCurveAdjusted(anim.Curve, stopAtRelative);
                    anim.Action(GetActualValue(anim.StartValue, anim.EndValue, stopAtRelative));
                    anims.RemoveAt(i);
                    count = anims.Count;
                    anim.Finished();
                }
            }
            void Update()
            {
                for (int i = anims.Count - 1; i >= 0; i--)
                {
                    Anim<T> anim = anims[i];
                    float timePassed = Time.time - anim.StartTime;
                    float relative = (timePassed / anim.Duration);
                    bool end = false;
                    if (relative >= 1f)
                    {
                        relative = 1f;
                        end = true;
                    }

                    float curveAdjusted = GetCurveAdjusted(anim.Curve, relative);

                    T startValue = anim.StartValue;
                    T endValue = anim.EndValue;
                    T actualValue = GetActualValue(startValue, endValue, curveAdjusted);

                    anim.Action(actualValue);
                    if (end)
                    {
                        anims.RemoveAt(i);
                        count = anims.Count;
                        anim.Finished();
                    }
                }
            }
            private float GetCurveAdjusted(AnimationCurve curve, float relative)
            {
                if (curve == null) return relative;
                else return curve.Evaluate(relative);
            }
            protected abstract T GetActualValue(T startValue, T endValue, float time);
        }
    }
}