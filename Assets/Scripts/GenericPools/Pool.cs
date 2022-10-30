using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenericPools
{
    public abstract class Pool<T> : MonoBehaviour where T : Component
    {
        [Header("POOL CONFIG")]
        [SerializeField] protected T prefab;
        [SerializeField] protected bool disableObjectsOnEnpool = true;
        [SerializeField] protected bool enableObjectsOnDepool = true;

        public static Pool<T> Instance { get { if (instance == null) Debug.LogError("NO INSTANCES OF " + typeof(Pool<T>) + " IN SCENE"); return instance; } }

        private Queue<T> enpooledObjects;
        private static Pool<T> instance;

        void Awake()
        {
            enpooledObjects = new();
            if (instance != null)
            {
                Debug.LogWarning("MULTIPLE INSTANCES OF " + this + " IN SCENE");
            }
            instance = this;
        }
        
        public T Depool()
        {
            T result;
            if (enpooledObjects.Count > 0)
            {
                result = enpooledObjects.Dequeue();
            } else
            {
                result = Instantiate(prefab);
            }

            JustDepooled(result);
            if (enableObjectsOnDepool) result.gameObject.SetActive(true);
            return result;
        }
        public void Enpool(T t)
        {
            JustEnpooled(t);
            if (disableObjectsOnEnpool) t.gameObject.SetActive(false);
            enpooledObjects.Enqueue(t);
        }

        protected virtual void JustEnpooled(T t)
        {

        }
        protected virtual void JustDepooled(T t)
        {

        }
    }
}

