using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Diagnostics;

namespace TorbuTils.Giraphe
{
    public class GraphLocalizer
    {
        public event Action Done;
        private readonly Graph inputGraph;
        public Graph ResultGraph { get; private set; }
        public GraphLocalizer(Graph inputGraph)
        {
            this.inputGraph = inputGraph;
            ResultGraph = Graph.MakeFromSatellites(inputGraph);
        }
        public IEnumerable Solve()
        {
            ICollection<int> nodes = inputGraph.CopyNodes();
            foreach (int id in nodes)
            {
                Dijkstra dijkstra = new(inputGraph, id);
                foreach (var _ in dijkstra.Solve()) yield return null;
                foreach (int i in dijkstra.ResultTree.CopyNodes())
                {
                    object sat = dijkstra.ResultTree.GetSatellite(i, Settings.CostSatellite);
                    if (sat is int costHere)
                    {
                        object resultSat = ResultGraph.GetSatellite(i, Settings.CostSatellite);
                        if (resultSat == null)
                        {
                            ResultGraph.SetSatellite(i, Settings.CostSatellite, 0);
                            resultSat = 0;
                        }
                        int newCost = costHere + (int)resultSat;
                        ResultGraph.SetSatellite(i, Settings.CostSatellite, newCost);
                    }
                    yield return null;
                }
                yield return null;
            }
            Done?.Invoke();
        }
    }
}

