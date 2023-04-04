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
        protected override Vector3? GetPos(Vector3Int node)
        {
            return (Vector3)node * Scale;
        }
    }
}
