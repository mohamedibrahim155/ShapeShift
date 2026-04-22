using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

namespace Scripts.Utilities.Pool
{
    public class GenericPool <T>  where T : Object
    {
        public T Prefab { get; protected set; }
        public int PoolCount { get; protected set; }
        public Queue<T>  pool = new Queue<T>();

        public GenericPool(T prefab, int initialPoolSize = 10)
        {
            Prefab = prefab;
            PoolCount = initialPoolSize;
          
        }

        public virtual void Initialize()
        {
            for (int i = 0; i < PoolCount; i++)
            {
                T instance = CreateInstance();
                ReturnToPool(instance);
            }
        }



        protected virtual T CreateInstance()
        {
            return Object.Instantiate(Prefab);
        }

        public virtual void ReturnToPool(T instance)
        {
            pool.Enqueue(instance);
        }

        protected virtual T Get()
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            else
            {
                return CreateInstance();
            }
        }

        public virtual void Cleanup()
        {
            while (pool.Count > 0)
            {
                Object.Destroy(pool.Dequeue());
            }
        }

    }
}
