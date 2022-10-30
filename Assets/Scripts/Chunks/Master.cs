using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chunks
{
    public abstract class Master<Loc> : MonoBehaviour
    {  // MonoBehaviour because of coroutines
        /*
        private Loc centerLoc;
        private float calculateDuration = 1f;
        private float loadDuration = 1f;
        private int outerBoxRadius;

        private Dictionary<Loc, IChunk<Loc>> dict = new();
        private HashSet<Loc> loadedChunks = new();
        private HashSet<Loc> chunksToLoad = new();
        private HashSet<Loc> chunksToUnload = new();
        private Coroutine calculateRoutine;
        private Coroutine loadRoutine;
        private Loc previousCenterLoc;
        private bool loadNeeded;
        
        public void Tick()
        {
            centerLoc = FetchCenterLoc();
            if (true)//centerLoc != previousCenterLoc && calculateRoutine == null)
            {
                //calculateRoutine = StartCoroutine(StartCalculating());
                previousCenterLoc = centerLoc;
            }

            // If done calculating and no current loading, start loading
            //if (calculateRoutine == null && loadRoutine == null) loadRoutine = StartCoroutine(StartLoading());
        }
        private IEnumerator StartCalculating(ICollection<Loc> loadedChunks, Loc centerChunk, int outerBoxRadius, float radius)
        {
            yield return null;
        }
        */
    }
}
