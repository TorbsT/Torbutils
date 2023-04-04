using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorbuTils.Giraphe
{
    /// <summary>
    /// Computer-science graph.
    /// </summary>
    /// <typeparam name="T">Node identifier type.
    /// Common types are int, Vector2Int and Vector3Int.</typeparam>
    public class Graph<T>
    {
        /// <summary>
        /// Quantity of nodes that have ever been mentioned in edges or satellite data.
        /// </summary>
        public int NodeCount => Nodes.Count;
        public string Name { get; set; }
        private Edgees Edges { get; set; } = new();
        private Edgees AntiEdges { get; set; } = new();  // improves time complexity of CopyEdgesTo
        private Dictionary<(T, T), int> Weights { get; set; } = new();
        private Dictionary<T, Dictionary<string, object>> NodeSatellites { get; set; } = new();
        private HashSet<T> Nodes { get; set; } = new();

        /// <summary>
        /// Creates a new Graph object with a
        /// copy of inputGraph's satellite data
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <returns></returns>
        public static Graph<T> MakeFromSatellites(Graph<T> inputGraph)
        {
            Graph<T> result = new();
            foreach (T node in inputGraph.NodeSatellites.Keys)
            {
                foreach (string key in inputGraph.NodeSatellites[node].Keys)
                {
                    object value = inputGraph.NodeSatellites[node][key];
                    result.SetSatellite(node, key, value);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a copy of every node in this graph.
        /// A node is defined either though edges or satellite data.
        /// </summary>
        /// <returns>A collection of node IDs</returns>
        public ICollection<T> CopyNodes()
        {
            HashSet<T> result = new();
            foreach (T node in Nodes)
            {
                result.Add(node);
            }
            return result;
        }
        /// <summary>
        /// Gets a copy of every edge in this graph.
        /// </summary>
        /// <returns>
        /// A collection of tuples (from, to)
        /// </returns>
        public ICollection<(T, T)> CopyEdges()
        {
            HashSet<(T, T)> edges = new();
            foreach (T from in Edges.GetNodes())
            {
                foreach (T to in Edges.GetNodeEdges(from))
                {
                    edges.Add((from, to));
                }
            }
            return edges;
        }
        /// <summary>
        /// Gets a copy of every node directly accessible from node
        /// </summary>
        /// <returns>
        /// A collection of node ids, empty if node is nonexistent
        /// </returns>
        public ICollection<T> CopyEdgesFrom(T from)
        {
            if (!Edges.HasNode(from)) return new HashSet<T>();

            HashSet<T> result = new();
            foreach (T node in Edges.GetNodeEdges(from))
            {
                result.Add(node);
            }

            return result;
        }
        /// <summary>
        /// Gets a copy of every node with direct access to node
        /// </summary>
        /// <returns>
        /// A collection of node ids, empty if node is nonexistent
        /// </returns>
        public ICollection<T> CopyEdgesTo(T to)
        {
            if (!AntiEdges.HasNode(to)) return new HashSet<T>();

            HashSet<T> result = new();
            foreach (T node in AntiEdges.GetNodeEdges(to))
            {
                result.Add(node);
            }

            return result;
        }
        /// <summary>
        /// Gets the quantity of nodes directly accessible from node
        /// </summary>
        /// <returns>An integer, 0 if node is nonexistent</returns>
        public int GetEdgesCountFrom(T from)
        {
            if (!Edges.HasNode(from)) return 0;
            return Edges.GetNodeEdges(from).Count;
        }
        /// <summary>
        /// Gets the quantity of nodes with direct access to node
        /// </summary>
        /// <returns>An integer, 0 if node is nonexistent</returns>
        public int GetEdgesCountTo(T to)
        {
            if (!AntiEdges.HasNode(to)) return 0;
            return AntiEdges.GetNodeEdges(to).Count;
        }
        /// <summary>
        /// Adds a one-directional edge to this graph.
        /// Can be weighted.
        /// Replaces an existing edge if necessary.
        /// </summary>
        /// <param name="from">Edge start node id.</param>
        /// <param name="to">Edge end node id.</param>
        /// <param name="weight">Edge weight. Ignore this parameter in unweighted graphs.</param>
        public void AddEdge(T from, T to, int weight = 1)
        {
            Edges.Connect(from, to);
            AntiEdges.Connect(to, from);
            SetWeight(from, to, weight);
            if (!Nodes.Contains(from)) Nodes.Add(from);
            if (!Nodes.Contains(to)) Nodes.Add(to);
        }
        /// <summary>
        /// Removes the given edge from this graph.
        /// Only removes in the given direction.
        /// If the given edge is nonexistent, nothing happens.
        /// </summary>
        /// <param name="from">Edge start node id.</param>
        /// <param name="to">Edge end node id.</param>
        public void RemoveEdge(T from, T to)
        {
            Edges.Disconnect(from, to);
            AntiEdges.Disconnect(to, from);
        }
        /// <summary>
        /// Gets satellite info of a node.
        /// Can get from nonexistent nodes.
        /// </summary>
        /// <param name="node">The node id.</param>
        /// <param name="satelliteName">Specifies where the info is stored.</param>
        /// <returns>
        /// An object, null if there is no satellite info
        /// </returns>
        public object GetSatellite(T node, string satelliteName)
        {
            if (!NodeSatellites.ContainsKey(node)) return null;
            if (!NodeSatellites[node].ContainsKey(satelliteName)) return null;
            return NodeSatellites[node][satelliteName];
        }
        /// <summary>
        /// Stores satellite info on a node.
        /// Can store on nonexistent nodes.
        /// Overwrites previous info at the given satellite
        /// </summary>
        /// <param name="node">The node id.</param>
        /// <param name="satelliteName">Specifies where the info should be stored.</param>
        /// <param name="value">Specifies the object to store</param>
        public void SetSatellite(T node, string satelliteName, object value)
        {
            if (!NodeSatellites.ContainsKey(node)) NodeSatellites[node] = new();
            if (!Nodes.Contains(node)) Nodes.Add(node);
            NodeSatellites[node][satelliteName] = value;
        }
        /// <summary>
        /// Sets the weight of an edge.
        /// Can set the weight of a nonexistent edge.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="weight"></param>
        public void SetWeight(T from, T to, int weight)
        {
            Weights[(from, to)] = weight;
        }
        /// <summary>
        /// Gets the weight of an edge.
        /// </summary>
        /// <param name="from">Edge start node id.</param>
        /// <param name="to">Edge end node id.</param>
        /// <returns>An integer, null if the edge weight is nonexistent.</returns>
        public int? GetWeight(T from, T to)
        {
            if (!Weights.ContainsKey((from, to))) return null;
            return Weights[(from, to)];
        }
        /// <summary>
        /// Gets the quantity of edges between two nodes.
        /// </summary>
        /// <param name="a">Node id A.</param>
        /// <param name="b">Node id B.</param>
        /// <returns>0, 1 or 2.</returns>
        public int GetEdgeQuantityBetween(T a, T b)
        {
            int quantity = 0;
            if (Edges.HasEdge(a, b)) quantity++;
            if (Edges.HasEdge(b, a)) quantity++;
            return quantity;
        }

        /// <summary>
        /// "Edges" was taken.
        /// </summary>
        private class Edgees
        {
            private readonly Dictionary<T, HashSet<T>> edges = new();
            internal int NodeCount { get; private set; }

            internal bool HasNode(T node) => edges.ContainsKey(node);
            internal bool HasEdge(T from, T to)
                => edges.ContainsKey(from) && edges[from].Contains(to);
            internal ICollection<T> GetNodes() => edges.Keys;
            internal ICollection<T> GetNodeEdges(T node) => edges[node];
            internal void Connect(T from, T to)
            {
                if (!edges.ContainsKey(from))
                {
                    edges[from] = new();
                    NodeCount++;
                }
                edges[from].Add(to);
            }
            internal void Disconnect(T from, T to)
            {
                if (edges.ContainsKey(from))
                {
                    edges[from].Remove(to);
                    if (edges[from].Count == 0)
                    {
                        edges.Remove(from);
                        NodeCount--;
                    }
                }
            }
        }
    }
}
