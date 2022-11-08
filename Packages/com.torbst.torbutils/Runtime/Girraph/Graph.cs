using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TorbuTils.Girraph
{
    [Serializable]
    public class Graph
    {
        // Features and stuff
        private Edges Edges { get; set; } = new();
        private Edges AntiEdges { get; set; } = new();  // improves search time
        private EdgeWeights Weights { get; set; }  // Optional
        private Dictionary<int, NodeSatellite> NodeSatellites { get; set; } = new();
        public int NodeCount => Mathf.Max(Edges.nodeCount, NodeSatellites.Count);

        public Graph(bool weighted)
        {
            if (weighted) Weights = new EdgeWeights();
        }

        public IEnumerable<(int, int)> CopyEdges()
        {
            HashSet<(int, int)> edges = new();
            foreach (int from in Edges.edges.Keys)
            {
                foreach (int to in Edges.edges[from])
                {
                    edges.Add((from, to));
                }
            }
            return edges;
        }

        public ICollection<int> CopyEdgesFrom(int id)
        {
            //if (id < 0 || id >= NodeCount) Debug.LogWarning("Tried getting edges from id " + id);
            if (!Edges.edges.ContainsKey(id))
            {
                return new HashSet<int>();
            }

            HashSet<int> result = new();
            foreach (int i in Edges.edges[id])
            {
                result.Add(i);
            }


            return result;
        }
        public ICollection<int> CopyEdgesTo(int id)
        {
            //if (id < 0 || id >= NodeCount) Debug.LogWarning("Tried getting edges to id " + id +", max is "+NodeCount + ": "+Edges.nodeCount + " " + NodeSatellites.Count);
            if (!AntiEdges.edges.ContainsKey(id)) return new HashSet<int>();

            HashSet<int> result = new();
            foreach (int i in AntiEdges.edges[id])
            {
                result.Add(i);
            }

            return result;
        }
        public void AddEdge(int from, int to, int weight = 1)
        {
            Edges.Connect(from, to);
            AntiEdges.Connect(to, from);
        }
        public void SetWeight(int from, int to, int weight)
        {
            Weights.AddWeight(from, to, weight);
        }
        public object GetSatellite(int id, string satelliteName)
        {
            if (!NodeSatellites.ContainsKey(id)) return null;
            if (!NodeSatellites[id].data.ContainsKey(satelliteName)) return null;
            return NodeSatellites[id].data[satelliteName];
        }
        public void SetSatellite(int id, string satelliteName, object value)
        {
            if (!NodeSatellites.ContainsKey(id)) NodeSatellites.Add(id, new());
            NodeSatellites[id].Set(satelliteName, value);
        }
        public int? GetWeight(int from, int to)
        {
            if (Weights == null) return 1;
            if (!Weights.weights.ContainsKey((from, to))) return null;
            return Weights.weights[(from, to)];
        }

        public void RemoveEdge(int from, int to)
        {
            // TODO maybe all useless info gets removed?
            Edges.Disconnect(from, to);
            AntiEdges.Disconnect(to, from);
        }
    }
    internal class Edges
    {
        internal readonly Dictionary<int, HashSet<int>> edges = new();
        internal int nodeCount; 

        public HashSet<int> this[int id] { get { if (!edges.ContainsKey(id)) return new(); return edges[id]; } }

        public void Connect(int from, int to)
        {
            if (!edges.ContainsKey(from))
            {
                edges.Add(from, new());
                nodeCount++;
            }
            edges[from].Add(to);
        }
        public void Disconnect(int from, int to)
        {
            if (edges.ContainsKey(from))
            {
                edges[from].Remove(to);
                if (edges[from].Count == 0)
                {
                    edges.Remove(from);
                    nodeCount--;
                }
            }
        }
    }
    internal class EdgeWeights
    {
        internal readonly Dictionary<(int, int), int> weights = new();
        public void AddWeight(int from, int to, int weight)
        {
            weights[(from, to)] = weight;
        }
        public void RemoveWeight(int from, int to)
        {
            weights.Remove((from, to));
        }
    }
    internal class NodeSatellite
    {
        public Dictionary<string, object> data = new();

        public void Set(string name, object value)
        {
            data[name] = value;
        }
    }
}

