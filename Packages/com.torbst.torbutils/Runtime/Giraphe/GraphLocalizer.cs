using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Diagnostics;

namespace TorbuTils.Giraphe
{
    public class GraphLocalizer<T>
    {
        public event Action Done;
        private readonly Graph<T> inputGraph;
        public Graph<T> ResultGraph { get; private set; }
        public GraphLocalizer(Graph<T> inputGraph)
        {
            this.inputGraph = inputGraph;
            ResultGraph = Graph<T>.MakeFromSatellites(inputGraph);
        }
        public IEnumerable Solve()
        {
            ICollection<T> nodes = inputGraph.CopyNodes();
            ICollection<(T, T)> edges = inputGraph.CopyEdges();
            foreach ((T, T) edge in edges)
                ResultGraph.AddEdge(edge.Item1, edge.Item2);

            foreach (T node in nodes)
            {
                Dijkstra<T> dijkstra = new(inputGraph, node);
                foreach (var _ in dijkstra.Solve()) yield return null;
                foreach (T n in dijkstra.ResultTree.CopyNodes())
                {
                    object sat = dijkstra.ResultTree.GetSatellite(n, Settings.CostSatellite);
                    if (sat is int costHere)
                    {
                        object resultSat = ResultGraph.GetSatellite(n, Settings.CostSatellite);
                        if (resultSat == null)
                        {
                            ResultGraph.SetSatellite(n, Settings.CostSatellite, 0);
                            resultSat = 0;
                        }
                        int newCost = costHere + (int)resultSat;
                        ResultGraph.SetSatellite(n, Settings.CostSatellite, newCost);
                    }
                    yield return null;
                }
                yield return null;
            }
            Done?.Invoke();
        }
    }
}

