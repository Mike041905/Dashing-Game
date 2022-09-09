using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mike.ObjectPooling
{
    public class MikePooledObject : MonoBehaviour
    {
        public MikeObjectPooler pool;

        public Action OnSpawn;
        public Action OnReturn;

        public bool ready = true;

        private void Update()
        {
            ReturnDelay();
        }

        void ReturnDelay()
        {
            if (_timer > 0 && !ready)
            {
                _timer -= Time.deltaTime;

                if (_timer <= 0)
                {
                    Return(0);
                }
            }
        }

        float _timer = 0;
        public void Return(float delay = 0)
        {
            if(delay <= 0)
            {
                OnReturn?.Invoke();
                ready = true;
                gameObject.SetActive(false);
                transform.parent = pool.transform;
            }
            else
            {
                _timer = delay;
            }
        }

        public void Spawn(Vector3 position, Quaternion rotation)
        {
            ready = false;

            transform.parent = null;
            transform.position = position;
            transform.rotation = rotation;

            gameObject.SetActive(true);

            OnSpawn?.Invoke();
        }
    }
}
