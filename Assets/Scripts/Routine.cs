using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Routine : MonoBehaviour
{
    public bool Running { get => coroutine != null; }

    [SerializeField] private bool running;
    
    private Coroutine coroutine;
    
    private void Update()
    {
        running = Running;
    }

    protected void TryRun()
    {
        if (Running)
        {
            Debug.LogWarning(this + " is already running");
        }
        else
        {
            coroutine = StartCoroutine(Run());
        }
    }
    protected void TryStop()
    {
        if (!Running)
        {
            Debug.LogWarning(this + " is not running");
        }
        else
        {
            StopCoroutine(coroutine);
        }
    }
    protected abstract IEnumerator Run();
}
