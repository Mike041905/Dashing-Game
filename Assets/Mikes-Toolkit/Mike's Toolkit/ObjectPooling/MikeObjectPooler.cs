using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mike.ObjectPooling
{
    public class MikeObjectPooler : MonoBehaviour
    {
        [SerializeField] bool _spawnOnAwake = true;

        [Space(10)]
        public GameObject pooledObject;
        public int amount;

        MikePooledObject[] _pool;


        private void Awake()
        {
            if (_spawnOnAwake) { InitializePool(); }
        }

        public void InitializePool()
        {
            if(_pool != null) { Debug.LogError("Object pool already initialized!"); return; }

            bool isPooledObject = pooledObject.GetComponent<MikePooledObject>() != null;
            _pool = new MikePooledObject[amount];

            for (int i = 0; i < amount; i++)
            {
                if (isPooledObject)
                {
                    MikePooledObject obj = Instantiate(pooledObject, transform).GetComponent<MikePooledObject>();
                    obj.pool = this;
                    _pool[i] = obj;
                }
                else
                {
                    MikePooledObject obj = Instantiate(pooledObject, transform).AddComponent<MikePooledObject>();
                    obj.pool = this;
                    _pool[i] = obj;
                }
            }
        }

        public GameObject SpawnObject(Vector3 position, Quaternion rotation)
        {
            foreach (MikePooledObject obj in _pool)
            {
                if (obj.ready)
                {
                    obj.Spawn(position, rotation);
                    return obj.gameObject;
                }
            }

            Debug.LogWarning("Not enough pooled objects (all objects in pool are already in use)"); return null;
        }
    }
}
