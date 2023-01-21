using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace TorbuTils.Giraphe
{
    public class GraphVisualizer : MonoBehaviour
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

        [field: SerializeField] public NodeVisual IsolatedNodesVisual { get; private set; } = new();
        [field: SerializeField] public NodeVisual SourceNodesVisual { get; private set; } = new();
        [field: SerializeField] public NodeVisual SinkNodesVisual { get; private set; } = new();
        [field: SerializeField] public NodeVisual PassthroughNodesVisual { get; private set; }
            = new();
        [field: SerializeField] public string displayPositionSatellite
        { get; private set; } = "pos";
        [field: SerializeField] public string displayColorOverrideSatellite
        { get; private set; } = "color";
        
        [Header("DEBUG")]
        [SerializeField] private bool debug = false;
        /// <summary>
        /// The currently displayed graph.
        /// </summary>
        private Graph graph;
        /// <summary>
        /// Displays a graph.
        /// </summary>
        /// <param name="graph">The graph that will be displayed</param>
        public void Checkout(Graph graph)
        {
            this.graph = graph;
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

            DrawEdges();
            DrawNodes();
        }
        private void DrawEdges()
        {
            // Draw edges
            foreach ((int, int) edge in graph.CopyEdges())
            {
                int from = edge.Item1;
                int to = edge.Item2;
                if (graph.GetEdgeQuantityBetween(from, to) == 2)
                    Gizmos.color = EdgeColor * BiDirectionalEdgeColor;
                else
                    Gizmos.color = EdgeColor * OneDirectionalEdgeColor;

                object fromPosSat = graph.GetSatellite(from, displayPositionSatellite);
                object toPosSat = graph.GetSatellite(to, displayPositionSatellite);
                if (fromPosSat == null || toPosSat == null)
                {
                    Debug.LogWarning(
                        $"Satellite info ({displayPositionSatellite})" +
                        $" doesn't exist, can't visualize positions of edge." +
                        $" fromSat = ({fromPosSat}), toSat = ({toPosSat}). " +
                        $" fromId = {from}, toId = {to}");
                    continue;
                }
                if (fromPosSat is not Vector2 || toPosSat is not Vector2)
                {
                    Debug.LogWarning(
                        $"Satellite info ({displayPositionSatellite})" +
                        $" is not castable to Vector2.'" +
                        $" fromSat = ({fromPosSat}), toSat = ({toPosSat})." +
                        $" fromId = {from}, toId = {to}");
                    continue;
                }

                Vector2 fromPos = (Vector2)fromPosSat;
                Vector2 toPos = (Vector2)toPosSat;
                Gizmos.DrawLine(fromPos, toPos);
            }
        }
        private void DrawNodes()
        {
            // Draw nodes
            for (int id = 0; id < graph.NodeCount; id++)
            {
                bool isSource = graph.GetEdgesCountFrom(id) > 0;
                bool isSink = graph.GetEdgesCountTo(id) > 0;

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
                object colorSat = graph.GetSatellite(id, displayColorOverrideSatellite);
                // color satellite will override color on this node.
                if (colorSat != null)
                {
                    if (colorSat is Color) nodeColor = (Color)colorSat;
                    else Debug.LogWarning(
                        $"Satellite info ({displayColorOverrideSatellite})" +
                        $" is not castable to Color." +
                        $" sat = ({colorSat}), id = {id}");
                }
                Gizmos.color = nodeColor;

                object posSat = graph.GetSatellite(id, displayPositionSatellite);
                if (posSat == null)
                {
                    Debug.LogWarning(
                        $"Satellite info ({displayPositionSatellite})" +
                        $" doesn't exist, can't visualize node position." +
                        $" sat = ({posSat}), id = ({id})");
                    continue;
                }
                if (posSat is not Vector2)
                {
                    Debug.LogWarning(
                        $"Satellite info ({displayPositionSatellite})" +
                        $" is not castable to Vector2.'" +
                        $" sat = ({posSat}), id = ({id})");
                    continue;
                }

                Vector2 pos = (Vector2)posSat;
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
                if (showId)
                {
                    Handles.Label(pos + Vector2.up * size, id.ToString());
                }
            }
        }
    }
    public interface IGraphVisualizerProvider
    {
        Graph GetGraph(int graphId);
    }
}