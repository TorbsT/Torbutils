using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chunks
{
    public interface IChunk<T>
    {
        void StartLoading(T identity);
        void StartUnloading();
    }
}
