using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TorbuTils.Giraphe;
using UnityEditor;

public class DijkstraTest : MonoBehaviour, IGraphVisualizerProvider
{
    [Range(0f, 1f)] public float edgeChance = 0.2f;
    public bool showExistingEdges;
    public int nodeCount = 10;
    public int maxSteps = -1;

    private Dijkstra dijkstra;
    private Graph input;
    private Graph Output => dijkstra.ResultTree;
    IEnumerator cor;

    private void OnEnable()
    {
        input = new();
        for (int i = 0; i < nodeCount; i++)
        {
            Vector2 iPos = new(Random.value, Random.value);
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
        dijkstra.Done += Done;
        cor = dijkstra.Solve().GetEnumerator();
        GetComponent<GraphVisualizer>().Checkout(Output);
    }
    private void Update()
    {
        if (cor != null)
        cor.MoveNext();
    }
    public void Done()
    {
        Debug.Log("Done!");
    }
    public Graph GetGraph(int graphId)
    {
        if (graphId == 0) return input;
        return Output;
    }
}
