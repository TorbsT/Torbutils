using System.Collections;
using System.Collections.Generic;
using System;

namespace TorbuTils.Giraphe
{
    [Serializable]
    public class Dijkstra<T>
    {
        public event Action Done;
        private readonly Graph<T> inputGraph;
        private readonly T startNode;
        private readonly int maxDistance;
        public Graph<T> ResultTree { get; private set; }
        public Dijkstra(Graph<T> inputGraph, T startNode, int maxDistance = int.MaxValue)
        {
            this.inputGraph = inputGraph;
            this.startNode = startNode;
            this.maxDistance = maxDistance;
            ResultTree = Graph<T>.MakeFromSatellites(inputGraph);
        }
        public IEnumerable Solve()
        {
            Queue<T> queue = new();  // ids
            queue.Enqueue(startNode);
            ResultTree.SetSatellite(startNode, Settings.CostSatellite, 0);

            while (queue.Count > 0)
            {
                T current = queue.Dequeue();
                int? ch = (int?)ResultTree.GetSatellite(current, Settings.CostSatellite);
                int costHere = ch == null ? 0 : ch.Value;
                foreach (T next in inputGraph.CopyEdgesFrom(current))
                {
                    yield return null;
                    int hypoCost = costHere + (int)inputGraph.GetWeight(current, next);
                    if (hypoCost > maxDistance) continue;
                    int? prevCost = (int?)ResultTree.GetSatellite(next, Settings.CostSatellite);
                    if (prevCost == null || hypoCost < prevCost)
                    {
                        if (prevCost != null)
                        {
                            foreach (T backtrack in ResultTree.CopyEdgesTo(next))
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