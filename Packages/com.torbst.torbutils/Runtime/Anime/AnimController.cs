using System.Collections.Generic;
using UnityEngine;

namespace TorbuTils
{
    namespace Anime
    {
        public abstract class AnimController<T> : MonoBehaviour
        {
            [field: SerializeField] public bool LogWarnings { get; set; } = true;
            private readonly List<Anim<T>> anims = new();
            private int count;
            private int idCounter = 0;

            internal int Begin(Anim<T> anim)
            {
                anim.Id = idCounter;
                idCounter++;
                anims.Add(anim);
                count = anims.Count;
                return anim.Id;
            } 
            internal bool Stop(int id, float? stopAtRelative = null)
            {
                int i = anims.FindIndex(x => x.Id == id);
                if (i == -1)
                {
                    if (LogWarnings)
                        Debug.LogWarning(
                            $"Tried stopping an unregistered {typeof(T)} animation with id: {id}");
                    return false;
                } else
                {
                    Anim<T> anim = anims[i];
                    float stopAtRelativeNN = (float)stopAtRelative;
                    stopAtRelative = Mathf.Clamp(stopAtRelativeNN, 0f, 1f);
                    float curveAdjusted = GetCurveAdjusted(anim.Curve, stopAtRelativeNN);
                    anim.Action(GetActualValue(anim.StartValue, anim.EndValue, stopAtRelativeNN));
                    anims.RemoveAt(i);
                    count = anims.Count;
                    anim.Finished();
                    return true;
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
                    bool mirrorCurve = false;

                    if (anim.Loop == LoopMode.Reset)
                    {
                        relative %= 1f;
                    }
                    else if (anim.Loop == LoopMode.InvertSameCurve)
                    {
                        relative %= 2f;
                        if (relative >= 1f)
                        {
                            relative = 2f - relative;
                        }
                    }
                    else if (anim.Loop == LoopMode.InvertMirrorCurve)
                    {
                        relative %= 2f;
                        if (relative >= 1f)
                        {
                            relative -= 1;
                            mirrorCurve = true;
                        }
                    }
                    else if (relative >= 1f && anim.Loop == LoopMode.None)
                    {
                        relative = 1f;
                        end = true;
                    }
                    float curveAdjusted = GetCurveAdjusted(anim.Curve, relative);
                    if (mirrorCurve)
                        curveAdjusted = 1f - curveAdjusted;

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