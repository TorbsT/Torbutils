using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenericPools;

namespace Chunks
{
    public class TestLoader : Loader<Vector2Int>
    {
        protected override IChunk<Vector2Int> FetchNewChunkObject()
        {
            return FlatChunkPool.Instance.Depool();
        }
    }
}
