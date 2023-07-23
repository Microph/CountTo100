using UnityEngine;
using System.Collections.Generic;

namespace CountTo100.Utilities
{
    public class SimpleObjectPool : MonoBehaviour
    {
        public class PooledObject : MonoBehaviour
        {
            public SimpleObjectPool Pool;
        }

        [SerializeField] private GameObject _prefab;

        private Stack<GameObject> _inactiveInstances = new Stack<GameObject>();

        public GameObject GetObjectInstance()
        {
            GameObject spawnedGameObject;
            if (_inactiveInstances.Count > 0)
            {
                spawnedGameObject = _inactiveInstances.Pop();
            }
            else
            {
                spawnedGameObject = Instantiate(_prefab);
                PooledObject pooledObject = spawnedGameObject.AddComponent<PooledObject>();
                pooledObject.Pool = this;
            }

            spawnedGameObject.transform.SetParent(null);
            spawnedGameObject.SetActive(true);
            return spawnedGameObject;
        }

        public void ReturnObject(GameObject returnedObject)
        {
            PooledObject pooledObject = returnedObject.GetComponent<PooledObject>();
            if (pooledObject != null && pooledObject.Pool == this)
            {
                returnedObject.transform.SetParent(transform);
                returnedObject.SetActive(false);
                _inactiveInstances.Push(returnedObject);
            }
            else
            {
                Destroy(returnedObject);
            }
        }
    }
}