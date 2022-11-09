using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TorbuTils.Giraphe
{
    [Serializable]
    public class Graph
    {
        public int NodeCount => Mathf.Max(Edges.NodeCount, NodeSatellites.Count);
        private Edgees Edges { get; set; } = new();
        private Edgees AntiEdges { get; set; } = new();  // improves search time of CopyEdgesTo
        private Dictionary<(int, int), int> Weights { get; set; } = new(); // Optional
        private Dictionary<int, Dictionary<string, object>> NodeSatellites { get; set; } = new();

        public ICollection<(int, int)> CopyEdges()
        {
            HashSet<(int, int)> edges = new();
            foreach (int from in Edges.GetNodes())
            {
                foreach (int to in Edges.GetNodeEdges(from))
                {
                    edges.Add((from, to));
                }
            }
            return edges;
        }

        public static Graph MakeFromSatellites(Graph inputGraph)
        {
            Graph result = new();
            for (int i = 0; i < inputGraph.NodeCount; i++)
            {
                if (!inputGraph.NodeSatellites.ContainsKey(i)) continue;
                foreach (string key in inputGraph.NodeSatellites[i].Keys)
                {
                    object value = inputGraph.NodeSatellites[i][key];
                    result.SetSatellite(i, key, value);
                }
            }
            return result;
        }

        public ICollection<int> CopyEdgesFrom(int id)
        {
            if (!Edges.HasNode(id))
            {
                return new HashSet<int>();
            }

            HashSet<int> result = new();
            foreach (int i in Edges.GetNodeEdges(id))
            {
                result.Add(i);
            }

            return result;
        }
        public ICollection<int> CopyEdgesTo(int id)
        {
            if (!AntiEdges.HasNode(id)) return new HashSet<int>();

            HashSet<int> result = new();
            foreach (int i in AntiEdges.GetNodeEdges(id))
            {
                result.Add(i);
            }

            return result;
        }
        public void AddEdge(int from, int to, int weight = 1)
        {
            Edges.Connect(from, to);
            AntiEdges.Connect(to, from);
            SetWeight(from, to, weight);
        }
        public void RemoveEdge(int from, int to)
        {
            Edges.Disconnect(from, to);
            AntiEdges.Disconnect(to, from);
        }
        public object GetSatellite(int id, string satelliteName)
        {
            if (!NodeSatellites.ContainsKey(id)) return null;
            if (!NodeSatellites[id].ContainsKey(satelliteName)) return null;
            return NodeSatellites[id][satelliteName];
        }
        public void SetSatellite(int id, string satelliteName, object value)
        {
            if (!NodeSatellites.ContainsKey(id)) NodeSatellites[id] = new();
            NodeSatellites[id][satelliteName] = value;
        }
        public void SetWeight(int from, int to, int weight)
        {
            Weights[(from, to)] = weight;
        }
        public int? GetWeight(int from, int to)
        {
            if (!Weights.ContainsKey((from, to))) return null;
            return Weights[(from, to)];
        }
        public int GetEdgeQuantityBetween(int a, int b)
        {
            int quantity = 0;
            if (Edges.HasEdge(a, b)) quantity++;
            if (Edges.HasEdge(b, a)) quantity++;
            return quantity;
        }


        private class Edgees
        {
            private readonly Dictionary<int, HashSet<int>> edges = new();
            internal int NodeCount { get; private set; }

            internal bool HasNode(int id) => edges.ContainsKey(id);
            internal bool HasEdge(int from, int to)
                => edges.ContainsKey(from) && edges[from].Contains(to);
            internal ICollection<int> GetNodes() => edges.Keys;
            internal ICollection<int> GetNodeEdges(int id) => edges[id];
            internal void Connect(int from, int to)
            {
                if (!edges.ContainsKey(from))
                {
                    edges[from] = new();
                    NodeCount++;
                }
                edges[from].Add(to);
            }
            internal void Disconnect(int from, int to)
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

