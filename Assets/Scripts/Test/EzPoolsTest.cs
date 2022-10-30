using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using EzPools;

namespace Test
{
    public class EzPoolsTest : MonoBehaviour
    {
        /*
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public int Count { get; private set; }

        public void Depool()
        {
            for (int i = 0; i < Count; i++)
            {
                Pools.Instance.Depool(Prefab).transform.position = Random.onUnitSphere*10f;
            }
        }
        public void Enpool()
        {
            int left = Count;
            foreach (PoolObject poolObject in FindObjectsOfType<PoolObject>())
            {
                if (left == 0) return;
                if (poolObject.Origin.Prefab != Prefab) continue;
                Pools.Instance.Enpool(poolObject.gameObject);
                left--;
            }
        }
        */
    }
}
