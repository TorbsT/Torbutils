using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chunks
{
    public abstract class Calculator<Loc> : Routine
    {
        protected float ChunkSize { get; set; }
        public abstract ICollection<Loc> LocsToLoad { get; }
        public abstract ICollection<Loc> LocsToUnload { get; }
        public void StartCalculating()
        {
            TryRun();
        }
        public void StopCalculating()
        {
            TryStop();
        }
    }
}
