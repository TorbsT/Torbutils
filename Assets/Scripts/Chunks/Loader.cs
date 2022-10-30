using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chunks
{
    public abstract class Loader<Loc> : Routine
    {
        private ICollection<Loc> locsToLoad;
        private ICollection<Loc> locsToUnload;
        private Dictionary<Loc, IChunk<Loc>> dict = new();

        public void StartLoading(ICollection<Loc> locsToLoad, ICollection<Loc> locsToUnload)
        {
            this.locsToLoad = locsToLoad;
            this.locsToUnload = locsToUnload;
            Run();
        }
        public void FullReload(ICollection<Loc> locsToLoad)
        {
            StopLoading();
            HashSet<Loc> unloads = new();
            // should probably be a coroutine but whatever
            foreach (Loc loc in dict.Keys)
            {
                unloads.Add(loc);
            }
            StartLoading(locsToLoad, unloads);
        }
        public void StopLoading()
        {
            TryStop();
        }
        protected override IEnumerator Run()
        {
            Waiter waiter = new(1f / 512f);
            foreach (Loc loc in locsToUnload)
            {
                Unload(loc);
                if (waiter.CheckTime()) yield return null;
            }
            foreach (Loc loc in locsToLoad)
            {
                Load(loc);
                if (waiter.CheckTime()) yield return null;
            }
        }
        private void Load(Loc loc)
        {
            if (dict.ContainsKey(loc))
            {
                Debug.LogWarning("already loaded");
                return;
            }
            IChunk<Loc> chunk = FetchNewChunkObject();
            chunk.StartLoading(loc);
            dict.Remove(loc);
        }
        private void Unload(Loc loc)
        {
            if (!dict.ContainsKey(loc))
            {
                Debug.LogWarning("already unloaded");
                return;
            }
            IChunk<Loc> chunk = dict[loc];
            chunk.StartUnloading();
            dict.Remove(loc);
        }
        protected abstract IChunk<Loc> FetchNewChunkObject();
    }
}
