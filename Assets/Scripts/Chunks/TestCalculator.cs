using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chunks
{
    /*
    public class TestCalculator : Calculator<Vector2Int>
    {
        /*
        public override ICollection<Vector2Int> LocsToLoad => locsToLoad;
        public override ICollection<Vector2Int> LocsToUnload => locsToUnload;
        public int OuterBoxRadius { get; set; }

        [SerializeField] private Transform centerTransform;
        private ICollection<Vector2Int> locsToLoad;
        private ICollection<Vector2Int> locsToUnload;

        protected override IEnumerator Run()
        {
            Vector3 pos = centerTransform.position;
            int centerX = Mathf.FloorToInt(pos.x / ChunkSize);
            int centerZ = Mathf.FloorToInt(pos.z / ChunkSize);
            Vector2Int centerChunk = new(centerX, centerZ);

            Waiter waiter = new(1f / 512f);
            HashSet<Vector2Int> newLoadedChunks = new();
            int circleRadius = OuterBoxRadius;
            for (int z = centerChunk.y - OuterBoxRadius; z <= centerChunk.y + OuterBoxRadius; z++)
            {
                for (int x = centerChunk.x - OuterBoxRadius; x <= centerChunk.x + OuterBoxRadius; x++)
                {
                    Vector2Int current = new(x, z);
                    Vector2Int difference = (current - centerChunk);
                    if (difference.magnitude <= circleRadius)
                    {
                        newLoadedChunks.Add(current);
                    }
                    if (waiter.CheckTime()) yield return null;
                
                }
            }

            HashSet<Vector2Int> chunksToLoad = new();
            HashSet<Vector2Int> chunksToUnload = new();
            foreach (Vector2Int chunk in newLoadedChunks)
            {
                if (!loadedChunks.Contains(chunk))
                {
                    chunksToLoad.Add(chunk);
                }
                if (waiter.CheckTime()) yield return null;
            }
            foreach (Vector2Int chunk in loadedChunks)
            {
                if (!newLoadedChunks.Contains(chunk))
                {
                    chunksToUnload.Add(chunk);
                }
                if (waiter.CheckTime()) yield return null;
            }

            this.chunksToLoad = chunksToLoad;
            this.chunksToUnload = chunksToUnload;
        }
        
    }
*/
}

