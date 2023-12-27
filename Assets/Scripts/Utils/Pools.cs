using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameObject = UnityEngine.GameObject;
using Object = UnityEngine.Object;

namespace MognomUtils {
    public class ObjectPool {
        private static Dictionary<GameObject, List<GameObject>> pools;
        private static Dictionary<GameObject, List<GameObject>> activeObjectsToPool;
        private static Transform poolParent;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() {
            // Reset the pools
            Reset();

            SceneManager.sceneLoaded += SceneLoadedCallback;
        }

        private static void Reset() {
            pools = new Dictionary<GameObject, List<GameObject>>();
            activeObjectsToPool = new Dictionary<GameObject, List<GameObject>>();
            poolParent = new GameObject("ObjectPool").transform;
        }

        private static void SceneLoadedCallback(Scene arg0, LoadSceneMode arg1) {
            Reset();
        }

        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : MonoBehaviour {
            return Spawn(prefab.GameObject(), position, rotation).GetComponent<T>();
        }

        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation) {
            GameObject newObject;
            GameObject newGameObject;
            GetMatchingPool(prefab, out List<GameObject> currentPool);

            if (currentPool.Count > 0) {
                // Get a recycled object
                newObject = currentPool[0];
                currentPool.RemoveAt(0);
                newGameObject = newObject.GameObject();
                newGameObject.transform.SetPositionAndRotation(position, rotation);
            } else {
                // Create a new object if none are available
                newObject = GameObject.Instantiate(prefab, position, rotation);
                newGameObject = newObject.GameObject();
            }

            activeObjectsToPool.Add(newGameObject, currentPool);
            newGameObject.SetActive(true);
            newGameObject.transform.SetParent(poolParent);

            return newObject;
        }

        public static void Recycle<T>(T objectToRecycle) where T : Object {
            GameObject gO = objectToRecycle.GameObject();
            activeObjectsToPool.TryGetValue(gO, out List<GameObject> currentPool);
            if (currentPool != null) {
                gO.SetActive(false);
                currentPool.Add(gO);
                activeObjectsToPool.Remove(gO);
            } else {
                GameObject.Destroy(gO);
            }
        }

        private static void GetMatchingPool<T>(T prefab, out List<GameObject> currentPool) where T : Object {
            GameObject gO = prefab.GameObject();
            if (!pools.TryGetValue(gO, out currentPool)) {
                if (currentPool == null) {
                    currentPool = new List<GameObject>();
                    pools.Add(gO, currentPool);
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