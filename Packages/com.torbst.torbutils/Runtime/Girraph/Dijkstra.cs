using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TorbuTils.Girraph
{
    [Serializable]
    public class Dijkstra
    {
        // Requires Edges
        // Optional Weights

        public event Action Done;
        private readonly Graph inputGraph;
        private readonly int startId;
        private readonly int maxDistance;
        [field: SerializeField] public Graph ResultTree { get; private set; }
        public Dijkstra(Graph graph, int startId, int maxDistance = int.MaxValue)
        {
            this.inputGraph = graph;
            this.startId = startId;
            this.maxDistance = maxDistance;
        }
        public IEnumerable Solve()
        {
            ResultTree = new(false);
            Queue<int> queue = new();  // ids
            queue.Enqueue(startId);


            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                int? ch = (int?)ResultTree.GetSatellite(current, "costhere");
                int costHere = ch == null ? 0 : ch.Value;
                foreach (int next in inputGraph.CopyEdgesFrom(current))
                {
                    yield return null;
                    int hypoCost = costHere + (int)inputGraph.GetWeight(current, next);
                    if (hypoCost > maxDistance) continue;
                    int? prevCost = (int?)ResultTree.GetSatellite(next, "costhere");
                    if (prevCost == null || hypoCost < prevCost)
                    {

                        if (prevCost != null)
                        {
                            foreach (int backtrack in ResultTree.CopyEdgesTo(next))
                            {
                                ResultTree.RemoveEdge(backtrack, next);
                            }
                        }

                        ResultTree.AddEdge(current, next);
                        ResultTree.SetSatellite(next, "costhere", hypoCost);
                        queue.Enqueue(next);

                    }
                }
            }
            Done?.Invoke();
        }
    }
}