using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TorbuTils.Giraphe
{
    public class Graph3DVisualizer : GraphVisualizer<Vector3Int>
    {
        [field: SerializeField] public float Scale { get; set; } = 1f;
        protected override (Vector3, Vector3)? GetLinePoses(Vector3Int from, Vector3Int to)
        {
            return ((Vector3)from * Scale, (Vector3)to * Scale);
        }
    }
}
