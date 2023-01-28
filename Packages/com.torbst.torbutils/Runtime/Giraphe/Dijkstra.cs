using System.Collections;
using System.Collections.Generic;
using System;

namespace TorbuTils.Giraphe
{
    [Serializable]
    public class Dijkstra
    {
        public event Action Done;
        private readonly Graph inputGraph;
        private readonly int startId;
        private readonly int maxDistance;
        public Graph ResultTree { get; private set; }
        public Dijkstra(Graph inputGraph, int startId, int maxDistance = int.MaxValue)
        {
            this.inputGraph = inputGraph;
            this.startId = startId;
            this.maxDistance = maxDistance;
            ResultTree = Graph.MakeFromSatellites(inputGraph);
        }
        public IEnumerable Solve()
        {
            Queue<int> queue = new();  // ids
            queue.Enqueue(startId);
            ResultTree.SetSatellite(startId, Settings.CostSatellite, 0);

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                int? ch = (int?)ResultTree.GetSatellite(current, Settings.CostSatellite);
                int costHere = ch == null ? 0 : ch.Value;
                foreach (int next in inputGraph.CopyEdgesFrom(current))
                {
                    yield return null;
                    int hypoCost = costHere + (int)inputGraph.GetWeight(current, next);
                    if (hypoCost > maxDistance) continue;
                    int? prevCost = (int?)ResultTree.GetSatellite(next, Settings.CostSatellite);
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
                        ResultTree.SetSatellite(next, Settings.CostSatellite, hypoCost);
                        queue.Enqueue(next);
                    }
                }
            }
            Done?.Invoke();
        }
    }
}