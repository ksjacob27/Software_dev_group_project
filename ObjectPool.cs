using System.Runtime.InteropServices.ComTypes;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;



/// <remarks>Documentation: https://gist.github.com/James-Frowen/c2ab4cdc96165298518bd2db0781bbe6</remarks>
public class ObjectPool : MonoBehaviour {
    public  int  maxPoolSize = 20;
    private bool _handlersRegistered;

    public        Dictionary<string, Queue<GameObject>> pool                = new Dictionary<string, Queue<GameObject>>();
    public        Dictionary<System.Guid, string>       poolNames           = new Dictionary<System.Guid, string>();
    public        Dictionary<System.Guid, GameObject>   spawnedObjects      = new Dictionary<System.Guid, GameObject>();
    public static string                                spawnablesDirectory = "Spawnables";
    public static ObjectPool                            pingleton;

    private void Awake() {
        pingleton = this;
    }

    public static void RegisterPrefab(string name, int count) {
        GameObject prefab   = Resources.Load<GameObject>($"{spawnablesDirectory}/{name}");
        bool       existing = pingleton.pool.ContainsKey(name);

        Queue<GameObject> prefabPool;
        if (existing) {
            prefabPool = pingleton.pool[name];
        }
        else {
            prefabPool = new Queue<GameObject>();
            pingleton.pool.Add(name, prefabPool);
        }
        pingleton.poolNames[prefab.GetComponent<ObjectInfo>().ObjectId] = name;
        for (int i = 0; i < count && prefabPool.Count < pingleton.maxPoolSize; i++) {
            GameObject obj = Instantiate(prefab);
            obj.name = obj.name.Replace("(Clone)", "") + i.ToString();
            prefabPool.Enqueue(obj);
            obj.SetActive(false);
            // obj.GetComponent<IPoolableObject>().Init();
        }
    }

    public static void Register(System.Guid gId, GameObject obj) {
        pingleton.spawnedObjects.Add(gId, obj);
    }

    public GameObject GetFromPool(System.Guid assetId, Vector3 position, Quaternion rotation) {
        return GetFromPool(poolNames[assetId], position, rotation);
    }

    public GameObject GetFromPool(string name, Vector3 position, Quaternion rotation) {
        // Get the oldest gameobject on the queue
        GameObject obj = pool[name].Dequeue();
        // set up the object
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        // requeue it as the youngest queue object
        pool[name].Enqueue(obj);
        GameManager.Spawn(obj);
        return obj;
    }

    IEnumerator Despawn(GameObject gameObj, float delay) {
        yield return new WaitForSeconds(delay);
        GameManager.UnSpawn(gameObj);
    }

    public static GameObject SpawnHandler(SpawnMessage msg) {
        return pingleton.GetFromPool(msg.objId, msg.position, msg.rotation);
    }

    public static void UnspawnHandler(GameObject spawned) {
        Debug.Log("Unspawning");
        if (spawned == null)
            return;
        spawned.transform.SetParent(null);
        spawned.SetActive(false);
    }
}
