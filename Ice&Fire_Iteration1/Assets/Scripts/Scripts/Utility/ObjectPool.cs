using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;

/*
https://mirror-networking.gitbook.io/docs/guides/networkbehaviour

https://gist.github.com/James-Frowen/c2ab4cdc96165298518bd2db0781bbe6

https://gist.github.com/James-Frowen/46ca5e8fd76d62527be7b958ca8dbaf1
*/

public class ObjectPool : NetworkBehaviour {
    public        int                                   maxPoolSize       = 50;
    public        Dictionary<string, Queue<GameObject>> _Pool             = new Dictionary<string, Queue<GameObject>>();
    public        Dictionary<System.Guid, string>       _PoolNames        = new Dictionary<System.Guid, string>();
    public        Dictionary<uint, GameObject>          _SpawnedObj       = new Dictionary<uint, GameObject>();
    public static string                                _SpawnableObjDir_ = "Spawnables";
    public static ObjectPool                            _Singleton;

    private void Awake() {
        _Singleton = this;
    }

    public static void RegisterPrefab(string aName, int count) {
        GameObject prefab   = Resources.Load<GameObject>($"{_SpawnableObjDir_}/{aName}");
        bool       existing = _Singleton._Pool.ContainsKey(aName);

        Queue<GameObject> prefabPool;
        if (existing) {
            prefabPool = _Singleton._Pool[aName];
        }
        else {
            prefabPool = new Queue<GameObject>();
            _Singleton._Pool.Add(aName, prefabPool);
        }
        _Singleton._PoolNames[prefab.GetComponent<NetworkIdentity>().assetId] = aName;
        for (int i = 0; i < count && prefabPool.Count < _Singleton.maxPoolSize; i++) {
            GameObject obj = Instantiate(prefab);
            obj.name = obj.name.Replace("(Clone)", "") + i.ToString();
            prefabPool.Enqueue(obj);
            obj.SetActive(false);
            obj.GetComponent<IPoolableObject>().Init();
        }
    }

    public static void Register(uint netId, GameObject obj) {
        _Singleton._SpawnedObj.Add(netId, obj);
    }

    public GameObject GetFromPool(System.Guid assetId, Vector3 position, Quaternion rotation) {
        return GetFromPool(_PoolNames[assetId], position, rotation);
    }

    public GameObject GetFromPool(string aName, Vector3 position, Quaternion rotation) {
        // 1. Get the oldest obj in que.
        GameObject obj = _Pool[aName].Dequeue();

        // 2. Setup.
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        // 3. Requeue
        _Pool[aName].Enqueue(obj);
        NetworkServer.Spawn(obj);
        return obj;
    }

    private IEnumerator Despawn(GameObject obj, float delay) {
        yield return new WaitForSeconds(delay);
        NetworkServer.UnSpawn(obj);
    }


    public static GameObject SpawnHandler(SpawnMessage msg) {
        return _Singleton.GetFromPool(msg.assetId, msg.position, msg.rotation);
    }


    public static void DespawnHandler(GameObject obj) {
        Debug.Log("Despawning");
        if (!obj) { return; }
        obj.transform.SetParent(null);
        obj.SetActive(false);
    }
}
