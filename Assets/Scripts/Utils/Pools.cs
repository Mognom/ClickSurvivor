using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MognomUtils {
    public class ObjectPool {
        private static Dictionary<Type, List<Object>> pools;
        private static Transform poolParent;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() {
            // Reset the pools
            pools = new Dictionary<Type, List<Object>>();
            poolParent = new GameObject("ObjectPool").transform;
        }

        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object {
            T newObject;
            GameObject newGameObject;
            GetMatchingPool(prefab, out List<Object> currentPool);


            if (currentPool.Count > 0) {
                // Get a recycled object
                newObject = (T)currentPool[0];
                currentPool.RemoveAt(0);
                newGameObject = newObject.GameObject();
                newGameObject.transform.SetPositionAndRotation(position, rotation);
            } else {
                // Create a new object if none are available
                newObject = (T)Object.Instantiate(prefab, position, rotation);
                newGameObject = newObject.GameObject();
            }

            newGameObject.SetActive(true);
            newGameObject.transform.SetParent(poolParent);

            return newObject;
        }

        public static void Recycle<T>(T objectoToRecycle) where T : Object {
            GetMatchingPool(objectoToRecycle, out List<Object> currentPool);
            objectoToRecycle.GameObject().SetActive(false);
            currentPool.Add(objectoToRecycle);
        }

        private static void GetMatchingPool<T>(T prefab, out List<Object> currentPool) where T : Object {
            if (!pools.TryGetValue(typeof(T), out currentPool)) {
                if (currentPool == null) {
                    currentPool = new List<Object>();
                    pools.Add(typeof(T), currentPool);
                }
            }
        }
    }

    public static class ObjectPoolExtensions {
        public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation) {
            return ObjectPool.Spawn(prefab, position, rotation);
        }

        public static T Spawn<T>(this T prefab, Vector3 position, Quaternion rotation) where T : MonoBehaviour {
            return ObjectPool.Spawn(prefab, position, rotation);
        }

        public static void Recycle(this GameObject gameObject) {
            ObjectPool.Recycle(gameObject);
        }
        public static void Recycle<T>(this T gameObject) where T : MonoBehaviour {
            ObjectPool.Recycle(gameObject);
        }
    }
}