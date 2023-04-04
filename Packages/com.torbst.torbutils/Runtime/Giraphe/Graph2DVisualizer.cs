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
        protected override Vector3? GetPos(Vector2Int node)
        {
            return (Vector2)node * Scale;
        }
    }
}
