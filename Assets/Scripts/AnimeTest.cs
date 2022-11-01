using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TorbuTils.Anime;
using System;

public class AnimeTest : MonoBehaviour
{
    private Renderer rndr { get; set; }
    [field: SerializeField] private Color toColor { get; set; }
    [field: SerializeField] private Quaternion toQuat { get; set; }
    [field: SerializeField] private Vector3 toV3 { get; set; }
    [field: SerializeField, Range(0.1f, 10f)] private float duration { get; set; } = 1f;
    [field: SerializeField] private AnimationCurve curve { get; set; } = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private void Awake()
    {
        rndr = GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    public void Test()
    {
        Anim<Color> colorAnim = new()
        {
            Curve = curve,
            Duration = duration,
            Action = delegate (Color color) { rndr.material.color = color; },
            StartValue = rndr.material.color,
            EndValue = toColor
        };
        colorAnim.Start();

        Anim<Vector3> v3Anim = new()
        {
            Curve = curve,
            Duration = duration,
            Action = delegate (Vector3 pos) { transform.position = pos; },
            StartValue = transform.position,
            EndValue = toV3
        };
        v3Anim.Start();

        Anim<Quaternion> quatAnim = new()
        {
            Curve = curve,
            Duration = duration,
            Action = delegate (Quaternion quat) { transform.rotation = quat; },
            StartValue = transform.rotation,
            EndValue = toQuat
        };
        quatAnim.Start();
    }
    private void Anim_OnFinish(Anim<Color> obj)
    {
        Debug.Log("COCK");
    }
}
