using System.Collections.Generic;
using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public sealed class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 16;

        private readonly Queue<GameObject> pool = new();

        private void Awake()
        {
            for (var i = 0; i < initialSize; i++)
            {
                pool.Enqueue(CreateInstance());
            }
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            var instance = pool.Count > 0 ? pool.Dequeue() : CreateInstance();
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
            return instance;
        }

        public void Return(GameObject instance)
        {
            instance.SetActive(false);
            instance.transform.SetParent(transform);
            pool.Enqueue(instance);
        }

        private GameObject CreateInstance()
        {
            var instance = Instantiate(prefab, transform);
            instance.SetActive(false);
            return instance;
        }
    }
}
