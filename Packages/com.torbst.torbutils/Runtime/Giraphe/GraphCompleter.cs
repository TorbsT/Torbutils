using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace TorbuTils.Giraphe
{
    public class GraphCompleter<T>
    {
        public event Action Done;
        private readonly Graph<T> inputGraph;
        public Graph<T> ResultTree { get; private set; }
        public GraphCompleter(Graph<T> inputGraph)
        {
            this.inputGraph = inputGraph;
            ResultTree = Graph<T>.MakeFromSatellites(inputGraph);
        }
        public IEnumerable Solve()
        {
            ICollection<T> nodes = inputGraph.CopyNodes();
            foreach (T nodeA in nodes)
            {
                foreach (T nodeB in nodes)
                {
                    if (nodeA.Equals(nodeB)) continue;
                    ResultTree.AddEdge(nodeA, nodeB);
                    yield return null;
                }
            }
            Done?.Invoke();
        }
    }
}
