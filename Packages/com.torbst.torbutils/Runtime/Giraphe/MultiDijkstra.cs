using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TorbuTils.Giraphe
{
    public class MultiDijkstra
    {
        public event Action Done;
        private readonly Graph inputGraph;
        private readonly ICollection<(int, int)> hotspots;  // tileId, costhere
        private readonly int maxDistance;
        [field: SerializeField] public Graph ResultGraph { get; private set; }
        public MultiDijkstra(Graph inputGraph, ICollection<(int, int)> hotspots, int maxDistance = int.MaxValue)
        {
            this.inputGraph = inputGraph;
            this.maxDistance = maxDistance;
            this.hotspots = hotspots;
            ResultGraph = Graph.MakeFromSatellites(inputGraph);
        }
        public IEnumerable Solve()
        {
            Queue<int> queue = new();  // ids
            foreach (var hotspot in hotspots)
            {
                int tileId = hotspot.Item1;
                int costhere = hotspot.Item2;
                ResultGraph.SetSatellite(tileId, "costhere", costhere);
                queue.Enqueue(tileId);
            }

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                int? ch = (int?)ResultGraph.GetSatellite(current, "costhere");
                int costHere = ch == null ? 0 : ch.Value;
                foreach (int next in inputGraph.CopyEdgesFrom(current))
                {
                    yield return null;
                    int hypoCost = costHere + (int)inputGraph.GetWeight(current, next);
                    if (hypoCost > maxDistance) continue;
                    int? prevCost = (int?)ResultGraph.GetSatellite(next, "costhere");
                    if (prevCost == null || hypoCost < prevCost)
                    {
                        if (prevCost != null)
                        {
                            foreach (int backtrack in ResultGraph.CopyEdgesTo(next))
                            {
                                ResultGraph.RemoveEdge(backtrack, next);
                            }
                        }

                        ResultGraph.AddEdge(current, next);
                        ResultGraph.SetSatellite(next, "costhere", hypoCost);
                        queue.Enqueue(next);
                    }
                }
            }
            Done?.Invoke();
        }
    }
}
