using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace TorbuTils.Giraphe
{
    public class GraphVisualizer : MonoBehaviour
    {
        public enum GizmosMode
        {
            None,
            Select,
            Always
        }
        /// <summary>
        /// MonoBehaviour instance used for visualizing graphs.
        /// </summary>
        public static GraphVisualizer Instance { get; private set; }
        /// <summary>
        /// Should the graph be displayed always, when the GameObject is selected, or never?
        /// </summary>
        [field: SerializeField] public GizmosMode Mode { get; set; } = GizmosMode.Always;
        /// <summary>
        /// Size of nodes with at least one edge.
        /// </summary>
        [field: SerializeField, Range(0f, 10f)] public float NodeSize { get; set; } = 1f;
        /// <summary>
        /// Size of isolated nodes.
        /// </summary>
        [field: SerializeField, Range(0f, 10f)] public float IsolatedNodeSize { get; set; } = 1f;
        /// <summary>
        /// Color to use for nodes with at least one edge.
        /// </summary>
        [field: SerializeField] public Color NodeColor { get; set; } = Color.yellow;
        /// <summary>
        /// Color to use for isolated nodes.
        /// </summary>
        [field: SerializeField] public Color IsolatedNodeColor { get; set; } = Color.yellow;
        /// <summary>
        /// Color to use for one-directional edges.
        /// </summary>
        [field: SerializeField] public Color EdgeColor { get; set; } = Color.yellow;
        /// <summary>
        /// Color to use for bidirectional edges.
        /// </summary>
        [field: SerializeField] public Color BiEdgeColor { get; set; } = Color.yellow;
        /// <summary>
        /// Label nodes with their id, if it has neighbours
        /// </summary>
        [field: SerializeField] public bool DisplayIds { get; set; } = true;
        /// <summary>
        /// Label isolated nodes with their id
        /// </summary>
        [field: SerializeField] public bool DisplayIsolatedIds { get; set; } = true;
        
        /// <summary>
        /// The currently displayed graph.
        /// </summary>
        private Graph graph;
        [SerializeField] private bool debug = false;

        private void Awake()
        {
            Instance = this;
        }
        private void OnDrawGizmos()
        {
            if (Mode == GizmosMode.Always) DoGizmos();
        }
        private void OnDrawGizmosSelected()
        {
            if (Mode == GizmosMode.Select) DoGizmos();
        }
        private void DoGizmos()
        {
            if (graph == null)
            {
                Handles.color = Color.red;
                if (Application.isPlaying && debug)
                {
                    string msg = "Warning: GraphVisualiser (" + gameObject +
                    ") has no graph.";
                    Handles.Label(Vector3.zero, msg);
                    Debug.LogWarning(msg);
                }
                return;
            }

            // Draw edges
            foreach ((int, int) edge in graph.CopyEdges())
            {
                int from = edge.Item1;
                int to = edge.Item2;
                if (graph.GetEdgeQuantityBetween(from, to) == 2) Gizmos.color = BiEdgeColor;
                else Gizmos.color = EdgeColor;

                object fromPosSat = graph.GetSatellite(from, "pos");
                object toPosSat = graph.GetSatellite(to, "pos");
                if (fromPosSat == null || toPosSat == null)
                {
                    Debug.LogWarning("Satellite info (pos) doesn't exist, can't visualize. "
                        + "Either from (" + fromPosSat + ") or (" + toPosSat + "). "+
                        "fromId = "+from+", toId = "+to);
                    continue;
                }
                if (fromPosSat is not Vector2 || toPosSat is not Vector2)
                {
                    Debug.LogWarning("Satellite info (pos) is not castable to Vector2, either: '" +
                        fromPosSat + "' or '" + toPosSat + "'. "+
                        "fromId = " + from + ", toId = " + to);
                }

                Vector2 fromPos = (Vector2)fromPosSat;
                Vector2 toPos = (Vector2)toPosSat;
                Gizmos.DrawLine(fromPos, toPos);
            }

            // Draw nodes
            for (int id = 0; id < graph.NodeCount; id++)
            {
                float size;
                bool isolated = graph.CopyEdgesFrom(id).Count == 0;
                if (isolated)
                {
                    Gizmos.color = IsolatedNodeColor;
                    size = IsolatedNodeSize;
                }
                else
                {
                    Gizmos.color = NodeColor;
                    size = NodeSize;
                }

                object posSat = graph.GetSatellite(id, "pos");
                if (posSat == null)
                {
                    Debug.LogWarning("Satellite info (pos) doesn't exist, can't visualize. " +
                        "id = " + id);
                    continue;
                }
                if (posSat is not Vector2)
                {
                    Debug.LogWarning("Satellite info (pos) is not castable to Vector2. '" +
                        posSat + "'. " +
                        "id = " + id);
                }
                Vector2 pos = (Vector2)posSat;
                Gizmos.DrawCube(pos, Vector3.one * size);

                // Draw the node id in text
                bool showId = (DisplayIds && !isolated) ^ (DisplayIsolatedIds && isolated); 
                if (showId)
                {
                    Handles.Label(pos+Vector2.up*size, id.ToString());
                }
            }
        }
        /// <summary>
        /// Display a graph.
        /// </summary>
        /// <param name="graph">The graph that will be displayed</param>
        public void Checkout(Graph graph)
        {
            this.graph = graph;
        }
    }
    public interface IGraphVisualizerProvider
    {
        Graph GetGraph(int graphId);
    }
}