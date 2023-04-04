using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TorbuTils.Giraphe
{
    public class MultiDijkstra<T>
    {
        public event Action Done;
        private readonly Graph<T> inputGraph;
        private readonly ICollection<(T, int)> hotspots;  // tileId, costhere
        private readonly int maxDistance;
        [field: SerializeField] public Graph<T> ResultGraph { get; private set; }
        public MultiDijkstra(Graph<T> inputGraph, ICollection<(T, int)> hotspots, int maxDistance = int.MaxValue)
        {
            this.inputGraph = inputGraph;
            this.maxDistance = maxDistance;
            this.hotspots = hotspots;
            ResultGraph = Graph<T>.MakeFromSatellites(inputGraph);
        }
        public IEnumerable Solve()
        {
            Queue<T> queue = new();  // ids
            foreach (var hotspot in hotspots)
            {
                T node = hotspot.Item1;
                int costhere = hotspot.Item2;
                ResultGraph.SetSatellite(node, Settings.CostSatellite, costhere);
                queue.Enqueue(node);
            }

            while (queue.Count > 0)
            {
                T current = queue.Dequeue();
                int? ch = (int?)ResultGraph.GetSatellite(current, Settings.CostSatellite);
                int costHere = ch == null ? 0 : ch.Value;
                foreach (T next in inputGraph.CopyEdgesFrom(current))
                {
                    yield return null;
                    int hypoCost = costHere + (int)inputGraph.GetWeight(current, next);
                    if (hypoCost > maxDistance) continue;
                    int? prevCost = (int?)ResultGraph.GetSatellite(next, Settings.CostSatellite);
                    if (prevCost == null || hypoCost < prevCost)
                    {
                        if (prevCost != null)
                        {
                            foreach (T backtrack in ResultGraph.CopyEdgesTo(next))
                            {
                                ResultGraph.RemoveEdge(backtrack, next);
                            }
                        }

                        ResultGraph.AddEdge(current, next);
                        ResultGraph.SetSatellite(next, Settings.CostSatellite, hypoCost);
                        queue.Enqueue(next);
                    }
                }
            }
            Done?.Invoke();
        }
    }
}
