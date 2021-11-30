using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;



public class Spawner : MonoBehaviour {

    /// <summary>The position in which the spawnObject is to be instantiated.</summary>
    public GameObject p_SpawnPoint;

    /// <summary>The prefabricated GameObject to be spawned.</summary>
    public GameObject p_SpawnObject;

    /// <summary>The maximum amount of objects to be instantiated each interval.</summary>
    public int p_SpawnTotal;

    /// <summary>The interval incrementer.</summary>
    public int p_Round;
    public float p_RoundMulti;

    /// <summary>Instantiation time-gate. Implemented as to not overwhelm the player or the game world.</summary>
    public float p_TimeBetweenSpawns;
    public float p_TimeBetweenRounds;



    void Start() {
        p_SpawnTotal = 20;
        p_Round = 1;
        StartCoroutine(SpawnGameObject());
    }


    void Update() {
        // GetObjectToSpawn();
    }


    // public void GetObjectToSpawn() { throw new System.NotImplementedException(); }


    private IEnumerator SpawnGameObject() {

        if (p_Round >= 5) {
            p_RoundMulti = (float)(p_Round * 0.15);
            p_SpawnTotal = (int)(24 * p_RoundMulti);
        }

        for (int x = 0; x < p_SpawnTotal; x++) {
            GameObject spawnedEnemy = Instantiate(p_SpawnObject, p_SpawnPoint.transform.position, p_SpawnPoint.transform.rotation);
            // GameManager.AddEnemy(spawnedEnemy);
            Debug.Log("Zombie Spawned!" + p_SpawnObject.transform.position);
            yield return new WaitForSeconds(p_TimeBetweenSpawns);

        }
    }
}
