using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TorbuTils.Giraphe
{
    public class Graph2DVisualizer : GraphVisualizer<Vector2Int>
    {
        [field: SerializeField] public float Scale { get; set; } = 1f;
        protected override (Vector3, Vector3)? GetLinePoses(Vector2Int from, Vector2Int to)
        {
            return ((Vector2)from*Scale, (Vector2)to*Scale);
        }
    }
}
