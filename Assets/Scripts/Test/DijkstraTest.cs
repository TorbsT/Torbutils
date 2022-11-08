using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TorbuTils.Girraph;
using UnityEditor;

public class DijkstraTest : MonoBehaviour
{
    [Range(0f, 1f)] public float edgeChance = 0.2f;
    public bool showExistingEdges;
    public int nodeCount = 10;
    public int maxSteps = -1;

    private Dijkstra dijkstra;
    private Graph input;
    private Graph output => dijkstra.ResultTree;
    IEnumerator cor;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (input == null) return;
        Vector2 GetPos(int id) => (Vector2)input.GetSatellite(id, "pos");

        Gizmos.color = Color.yellow;
        for (int id = 0; id < input.NodeCount; id++)
        {
            Vector2 pos = GetPos(id);
            Gizmos.DrawCube(pos, Vector3.one * 0.005f);
            Handles.Label((Vector3)pos+Vector3.down*0.01f, id.ToString());
        }
        
        if (showExistingEdges)
        foreach ((int, int) edge in input.CopyEdges())
        {
            int from = edge.Item1;
            int to = edge.Item2;

            Gizmos.DrawLine(GetPos(from), GetPos(to));
        }
        

        if (dijkstra == null) return;
        if (output == null) return;

        Gizmos.color = Color.red;
        foreach ((int, int) edge in output.CopyEdges())
        {
            int from = edge.Item1;
            int to = edge.Item2;
            Gizmos.DrawLine(GetPos(from), GetPos(to));
        }
    }

    private void OnEnable()
    {
        input = new(true);
        for (int i = 0; i < nodeCount; i++)
        {
            Vector2 iPos = new Vector2(Random.value, Random.value);
            input.SetSatellite(i, "pos", iPos);
            for (int j = 0; j < i; j++)
            {
                if (Random.value < edgeChance)
                {
                    input.AddEdge(i, j);
                    input.AddEdge(j, i);
                    Vector2 jPos = (Vector2)input.GetSatellite(j, "pos");
                    int dist = Mathf.CeilToInt((jPos - iPos).magnitude);
                    input.SetWeight(i, j, dist);
                    input.SetWeight(j, i, dist);
                }
            }
        }

        if (maxSteps == -1)
            dijkstra = new(input, 0);
        else
            dijkstra = new(input, 0, maxSteps);
        cor = dijkstra.Solve().GetEnumerator();
    }
    private void Update()
    {
        if (cor != null)
        cor.MoveNext();
    }
}
