using System.Collections;
using UnityEngine;
using System;

namespace TorbuTils.Anime
{
    public class Main : MonoBehaviour
    {
        public static Main Instance { get; private set; }

        public void Begin<T>(Anim<T> anim)
        {
            AnimController<T> controller = FindController<T>();
            if (controller != null) controller.Begin(anim);
        }
        public void Stop<T>(Anim<T> anim, float? stopAtTime = null)
        {
            AnimController<T> controller = FindController<T>();
            if (controller != null) controller.Stop(anim, stopAtTime);
        }

        private void Awake()
        {
            Instance = this;
        }
        private AnimController<T> FindController<T>()
        {
            AnimController<T> controller = GetComponent<AnimController<T>>();
            if (controller == null)
            {
                Debug.LogError($"Could not find an AnimController for {typeof(T)}." +
                    $" Ensure that that the GameObject with the Anime.Main component" +
                    $" also has an Anime.AnimController<{typeof(T)}> component." +
                    $" Default ones like Anime.Vector3Controller and Anime.ColorController" +
                    $" are already implemented, but you can create your own if you like.");
            }
            return controller;
        }
    }
    /// <summary>
    /// Animation object for interpolating between values over time.
    /// MUST assign Action, StartValue and EndValue.
    /// The scene MUST have a GameObject with an Anime.Main component
    /// and a AnimController<T> component.
    /// </summary>
    /// <typeparam name="T">Values that will be interpolated.</typeparam>
    public class Anim<T>
    {
        // INFO
        /// <summary>
        /// Current progress of the animation.
        /// Value is in the interval [0, 1].
        /// </summary>
        public float Progress => (Time.time - StartTime) / Duration;

        // REQUIRE ASSIGNMENT
        /// <summary>
        /// The animation will start at this value.
        /// </summary>
        public T StartValue { get; set; } = default;
        /// <summary>
        /// The animation will end at this value (if not interrupted).
        /// </summary>
        public T EndValue { get; set; } = default;
        /// <summary>
        /// What to do with the interpolated values.
        /// For vector movement, this could e.g. look like
        /// (value) => { transform.position = value; }
        /// </summary>
        public Action<T> Action { get; set; }

        // OPTIONAL ASSIGNMENT
        /// <summary>
        /// Fires once the animation ends by any means.
        /// Parameter is this Anim object.
        /// (after OnFinishAction).
        /// </summary>
        public event Action<Anim<T>> JustFinished;
        /// <summary>
        /// Executes the action once the animation ends by any means.
        /// Parameter is this Anim object.
        /// (before JustFinished).
        /// </summary>
        public Action<Anim<T>> OnFinishAction { get; set; }
        /// <summary>
        /// Defines how the value between StartValue and EndValue
        /// will be interpolated based on time.
        /// X values should be in the interval [0, 1].
        /// Y can be any number, but typically around [0, 1].
        /// Defaults to a linear curve.
        /// </summary>
        public AnimationCurve Curve { get; set; }
            = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        /// <summary>
        /// How long the animation should last in seconds.
        /// Defaults to 1 second.
        /// </summary>
        public float Duration { get; set; } = 1f;
        /// <summary>
        /// How the animation should loop once finished, if at all.
        /// </summary>
        public LoopMode Loop { get; set; } = LoopMode.None;
        internal float StartTime { get; set; } = Time.time;

        /// <summary>
        /// Start the animation. It will execute Action every frame,
        /// based on StartValue, EndValue and Curve.
        /// </summary>
        public void Start()
        {
            if (Main.Instance == null)
            {
                Debug.LogError(
                "Cannot start animation."+
                " The scene does not contain a GameObject with" +
                " an attached Anime.Main component. Please create one.");
                return;
            }
            Main.Instance.Begin(this);
        }

        /// <summary>
        /// Stop the animation at the given time.
        /// </summary>
        /// <param name="stopAtTime">
        /// null: stop at current time.
        /// [0, 1]: stop between start or end, respectively.
        /// </param>
        public void Stop(float? stopAtTime = null)
        {
            if (Main.Instance == null)
            {
                Debug.LogError(
                    "Cannot stop animation."+
                    " The scene does not contain a GameObject with" +
                    " an attached Anime.Main component. Please create one.");
                return;
            }
            if (stopAtTime == null) stopAtTime = Progress;
            stopAtTime = Mathf.Clamp((float)stopAtTime, 0f, 1f);
            Main.Instance.Stop(this, (float)stopAtTime);
        }

        internal void Finished()
        {
            if (OnFinishAction != null)
                OnFinishAction(this);
            JustFinished?.Invoke(this);
        }
    }
    /// <summary>
    /// None: no loop.
    /// Reset: Goes back to 0 progress instantly.
    /// InvertSameCurve: Invert the animation, using the same curve.
    /// InvertMirrorCurve: Invert the animation, using a mirrored curve.
    /// </summary>
    public enum LoopMode
    {
        None,
        Reset,
        InvertSameCurve,
        InvertMirrorCurve
    }
}