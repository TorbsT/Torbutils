#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System;

namespace TorbuTils.Giraphe
{
    public class GraphVisualizer<T> : MonoBehaviour
    {
        [System.Serializable]
        public class NodeVisual
        {
            [Range(0f, 2f)] public float size = 1f;
            public bool display = true;
            public bool displayId = true;
            public Color color = Color.white;
            public Shape shape;
        }
        public enum GizmosMode
        {
            None,
            Select,
            Always
        }
        public enum Shape
        {
            Box,
            Sphere,
            WireBox,
            WireSphere
        }
        [field: Header("CONFIG")]
        /// <summary>
        /// Should the graph be displayed always, when the GameObject is selected, or never?
        /// </summary>
        [field: SerializeField] public GizmosMode Mode { get; set; } = GizmosMode.Always;
        /// <summary>
        /// Size of nodes in general.
        /// </summary>
        [field: SerializeField, Range(0f, 10f)] public float NodeSize { get; set; } = 1f;
        /// <summary>
        /// Color to use for nodes in general.
        /// </summary>
        [field: SerializeField] public Color NodeColor { get; set; } = Color.yellow;
        /// <summary>
        /// Color to use for edges in general.
        /// </summary>
        [field: SerializeField] public Color EdgeColor { get; set; } = Color.yellow;
        /// <summary>
        /// Color to use for one-directional edges.
        /// </summary>
        [field: SerializeField] public Color OneDirectionalEdgeColor { get; set; } = Color.white;
        /// <summary>
        /// Color to use for bidirectional edges.
        /// </summary>
        [field: SerializeField] public Color BiDirectionalEdgeColor { get; set; } = Color.white;
        /// <summary>
        /// Label nodes with their id, at all
        /// </summary>
        [field: SerializeField] public bool DisplayIds { get; set; } = true;
        [field: SerializeField] public bool DisplayCosts { get; set; } = true;

        [field: SerializeField] public NodeVisual IsolatedNodesVisual { get; private set; } = new();
        [field: SerializeField] public NodeVisual SourceNodesVisual { get; private set; } = new();
        [field: SerializeField] public NodeVisual SinkNodesVisual { get; private set; } = new();
        [field: SerializeField] public NodeVisual PassthroughNodesVisual { get; private set; }
            = new();
        [field: SerializeField] public int HeatmapHottestCosthere = 0;
        [field: SerializeField] public int HeatmapColdestCosthere = 10;
        [field: SerializeField] public Color HeatmapHottestColor = Color.red;
        [field: SerializeField] public Color HeatmapColdestColor = Color.green;

        
        [Header("DEBUG")]
        [SerializeField] private bool debug = false;
        /// <summary>
        /// The currently displayed graph.
        /// </summary>
        private Graph<T> graph;
        /// <summary>
        /// Displays a graph.
        /// </summary>
        /// <param name="graph">The graph that will be displayed</param>
        public void Set(Graph<T> graph)
        {
            this.graph = graph;
        }
        [Obsolete] public void Checkout(Graph<T> graph)
        {
            this.graph = graph;
        }
        #if UNITY_EDITOR
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

            DrawEdges();
            DrawNodes();
        }
        private void DrawEdges()
        {
            // Draw edges
            foreach ((T, T) edge in graph.CopyEdges())
            {
                T from = edge.Item1;
                T to = edge.Item2;
                if (graph.GetEdgeQuantityBetween(from, to) == 2)
                    Gizmos.color = EdgeColor * BiDirectionalEdgeColor;
                else
                    Gizmos.color = EdgeColor * OneDirectionalEdgeColor;

                Vector3? fromPos = GetPos(from);
                Vector3? toPos = GetPos(to);
                if (fromPos == null || toPos == null) continue;
                Gizmos.DrawLine(fromPos.Value, toPos.Value);
            }
        }
#endif
        protected virtual Vector3? GetPos(T node)
        {
            return GetPosFromSatellites(node);
        }
        protected Vector3? GetPosFromSatellites(T node)
        {
            object posSat = graph.GetSatellite(node, Settings.PositionSatellite);
            if (posSat == null)
            {
                Debug.LogWarning(
                    $"Satellite info ({Settings.PositionSatellite})" +
                    $" doesn't exist, can't visualize position." +
                    $" posSat = ({posSat})," +
                    $" node = {node}.");
                return null;
            }
            if (posSat is Vector2 pos2)
                return pos2;
            if (posSat is Vector3 pos3)
                return pos3;

            Debug.LogWarning(
                $"Satellite info ({Settings.PositionSatellite})" +
                $" is not castable to Vector2 or Vector3.'" +
                $" posSat = ({posSat})," +
                $" node = {node}.");
            return null;
        }
#if UNITY_EDITOR
        private void DrawNodes()
        {
            // Draw nodes
            foreach (T node in graph.CopyNodes())
            {
                bool isSource = graph.GetEdgesCountFrom(node) > 0;
                bool isSink = graph.GetEdgesCountTo(node) > 0;

                NodeVisual visual;
                if (!isSource && !isSink)
                    visual = IsolatedNodesVisual;
                else if (isSource && !isSink)
                    visual = SourceNodesVisual;
                else if (!isSource && isSink)
                    visual = SinkNodesVisual;
                else
                    visual = PassthroughNodesVisual;

                if (!visual.display) continue;

                Color nodeColor = visual.color * NodeColor;
                object colorSat = graph.GetSatellite(node, Settings.ColorSatellite);
                object costhereSat = graph.GetSatellite(node, Settings.CostSatellite);
                // color satellite will override color on this node.
                if (colorSat != null)
                {
                    if (colorSat is Color color) nodeColor = color;
                    else Debug.LogWarning(
                        $"Satellite info ({Settings.ColorSatellite})" +
                        $" is not castable to Color." +
                        $" sat = ({colorSat}), id = {node}");
                } else if (costhereSat != null)
                {
                    if (costhereSat is int costhere)
                        nodeColor = Color.Lerp(HeatmapHottestColor, HeatmapColdestColor,
                            (float)costhere / (HeatmapColdestCosthere-HeatmapHottestCosthere));
                    else Debug.LogWarning(
                        $"Satellite info ({Settings.CostSatellite})" +
                        $" is not castable to float." +
                        $" sat = ({costhereSat}), id = {node}");
                }
                Gizmos.color = nodeColor;

                Vector3? posN = GetPos(node);
                if (posN == null) continue;
                Vector3 pos = posN.Value;
                float size = NodeSize * visual.size;
                Vector3 sizeV3 = size * Vector3.one;
                if (visual.shape == Shape.Box)
                    Gizmos.DrawCube(pos, sizeV3);
                else if (visual.shape == Shape.WireBox)
                    Gizmos.DrawWireCube(pos, sizeV3);
                else if (visual.shape == Shape.Sphere)
                    Gizmos.DrawSphere(pos, size / 2f);
                else if (visual.shape == Shape.WireSphere)
                    Gizmos.DrawWireSphere(pos, size / 2f);

                // Draw the node id in text
                bool showId = DisplayIds && visual.displayId;
                bool showCost = DisplayCosts && costhereSat != null;
                if (showId || showCost)
                {
                    List<string> txt = new();
                    if (showId) txt.Add(node.ToString());
                    if (showCost) txt.Add(costhereSat.ToString());
                    Handles.Label(pos + Vector3.up * size, string.Join(", ", txt));
                }
            }
        }
        #endif
    }
    public interface IGraphVisualizerProvider<T>
    {
        Graph<T> GetGraph(int graphId);
    }
}