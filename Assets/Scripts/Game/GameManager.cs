using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;



/*https://mirror-networking.gitbook.io/docs/components/network-room-manager*/
/*https://github.com/vis2k/Mirror/blob/master/Assets/Mirror/Runtime/NetworkManager.cs/#L1204*/
/*
 * https://doc.photonengine.com/en-us/pun/current/demos-and-tutorials/pun-basics-tutorial/gamemanager-levels
 * (Not identical. However underlying idea is similar.)
 */
/// <summary>
///  The Network Room Manager is a specialized type of Network Manager that provides
///  a multiplayer room before entering the main play scene of the game. It allows you
///  to set up a network with:
/// 
/// Partial of Mirror's Network Room-Manager.
/// </summary>
public class GameManager : MonoBehaviour {
    public        Avatar             player;
    public        List<Enemy>        Enemies       { get; } = new List<Enemy>();
    public        List<EnemySpawner> EnemySpawners { get; } = new List<EnemySpawner>();
    public static List<Transform>    spawnPositions = new List<Transform>();
    public static int                spawnPositionIndex;
    public enum PlayerSpawnMethod { Random, RoundRobin }

    private bool        gameActive = false;

    public        bool           isLoadingScene;
    public static AsyncOperation loadingSceneAsync;


    [Header("Player Object")]
    [FormerlySerializedAs("m_PlayerPrefab")]
    [Tooltip("Prefab of the player object.")]
    public GameObject playerPrefab;

    /// <summary>Enable to automatically create player objects on connect and on scene change.</summary>
    /// <remarks>https://github.com/vis2k/Mirror/blob/8b363d11e9ff2d53878838efe61c0debf8240488/Assets/Mirror/Runtime/NetworkManager.cs#L84</remarks>
    [FormerlySerializedAs("m_AutoCreatePlayer")]
    [Tooltip("Should Mirror automatically spawn the player after scene change?")]
    public bool autoCreatePlayer = true;

    [FormerlySerializedAs("m_GameScene")]
    public string GameplayScene = "EndOfTheRoad";

    [FormerlySerializedAs("m_offlineScene")]
    public string offlineScene = "OfflineScene";

    [FormerlySerializedAs("m_LoginScene")]
    public string LoginScene = "";

    [FormerlySerializedAs("m_AccountScene")]
    public string AccountScene = "";

    [FormerlySerializedAs("m_LobbyScene")]
    public string LobbyScene = "";

    [FormerlySerializedAs("m_MenuScene")]
    public string MainMenu = "MainMenu";

    public string activeSceneName;

    /// <summary>Where to spawn players.</summary>
    /// <remarks>https://github.com/vis2k/Mirror/blob/8b363d11e9ff2d53878838efe61c0debf8240488/Assets/Mirror/Runtime/NetworkManager.cs#L89</remarks>
    [Tooltip("Round Robin or Random order of Start Position selection")]
    public PlayerSpawnMethod playerSpawnMethod;

    /// <summary>The only GameManager</summary>
    public static GameManager Instance { get; private set; }

    /// <summary>The only Database Manager</summary>
    public static DatabaseNetwork.DatabaseManager dingleton { get; private set; }





    private void Awake() {
        if (!InitializeSingleton()) { return; }
        ;
        activeSceneName = MainMenu;
    }


    public static void Spawn(GameObject obj) {}


    public static void UnSpawn(GameObject obj) {}


    
    // ----------------------------------------------------Starters And Finishers---------------------------------------------------- \\
    // public void GameSpawned() {
        // Debug.Log("Game scene Started");
        // player = FindObjectOfType<Avatar>();
        // List<PlayerAvatar> players = new List<PlayerAvatar>(FindObjectsOfType<PlayerAvatar>());
    
        // Dictionary<uint, PlayerAvatar> playerDictionary = players.ToDictionary(x => x.netId, x => x);
        // foreach (Avatar avi in avatars) {
            // if (playerDictionary.ContainsKey(avi.player_ID)) {
                // if (avi.Allocated == true) { continue; }
                // Start Character
                // avatar.Initialize(playerDictionary[avi.player_ID]);
                // playerDictionary[avi.player_ID].avatar = avi;
    
                // player.toolbox.BuildToolBox(Builder.LoadDataFromString(player.playeravi.player_ID).scripts);
                // if (avi.hasAuthority) {
                    // RootUI.SetActive(true);
                    // HealthUI.Health = ((PlayerHealth)avatar.health);
                    // ToolUI.toolbox = avatar.toolbox;
                    // ToolUI.Init();
                // }
                // continue;
            // }
            // Debug.LogWarning($"Warning, avatar with connectionId: {netIdentity.connectionToClient} does not have avi gamePlayer");
        // }
    // }
    

    /// <summary>
    /// Client initiation. Starts server connection via URI.
    /// <remarks>https://github.com/vis2k/Mirror/blob/8b363d11e9ff2d53878838efe61c0debf8240488/Assets/Mirror/Runtime/NetworkManager.cs#L350</remarks>
    /// </summary>
    /// <param name="serverUri"></param>
    public void StartClient(Uri serverUri) {}


    /// <summary>
    /// Initiate the game 
    /// </summary>
    public void StartGame() {
        Debug.Log("StartGame");
        Debug.Log($"In room scene? {SceneManager.GetActiveScene().path == AccountScene}");
        if (SceneManager.GetActiveScene().path == AccountScene || SceneManager.GetActiveScene().path == MainMenu) {
            Debug.Log($"Ready to start? {IsReady()}");
            // if (!IsReady()) { return; }
            ChangeScene(GameplayScene);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool InitializeSingleton() {
        if (Instance != null && Instance == this)
            return true;

        if (Instance != null) {
            Debug.LogWarning("Multiple Managers detected in the scene.");
            Destroy(gameObject);

            // Return false to not allow collision-destroyed second instance to continue.
            return false;
        }
        Instance = this;
        if (Application.isPlaying) {
            // Force the object to scene root, in case user made it a child of something
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else {
            Instance = this;
        }

        return true;
    }




    /// <summary>
    /// called when quitting the application by closing the window / pressing
    /// <remarks>https://github.com/vis2k/Mirror/blob/8b363d11e9ff2d53878838efe61c0debf8240488/Assets/Mirror/Runtime/NetworkManager.cs#L617</remarks>
    /// </summary>
    public void OnGameQuit() {
        if (dingleton.Online) { ShutDownClient(); }
    }


    /// <summary>Stops and disconnects the client from the server.</summary>
    /// <remarks>https://github.com/vis2k/Mirror/blob/8b363d11e9ff2d53878838efe61c0debf8240488/Assets/Mirror/Runtime/NetworkManager.cs#L568</remarks>
    public void ShutDownClient() {
        if (dingleton.Online) { return; }

        // if (authenticator != null) {
        //     authenticator.OnClientAuthenticated.RemoveListener(OnClientAuthenticated);
        //     authenticator.OnStopClient();
        // }

        // Get Network Manager out of DDOL before going to offline scene
        // to avoid collision and let a fresh Network Manager be created.
        // IMPORTANT: .gameObject can be null if StopClient is called from
        //            OnApplicationQuit or from tests!
        if (gameObject != null
            && gameObject.scene.name == "DontDestroyOnLoad"
            && !string.IsNullOrEmpty(offlineScene)
            && SceneManager.GetActiveScene().path != offlineScene)
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        OnStopClient();

        //Debug.Log("NetworkManager StopClient");

        // set offline mode BEFORE changing scene so that FinishStartScene
        // doesn't think we need initialize anything.
        // set offline mode BEFORE NetworkClient.Disconnect so StopClient
        // only runs once.
        gameActive = dingleton.Offline;

        // shutdown client
        Disconnnect();
        Kill();

        // If this is the host player, StopServer will already be changing scenes.
        // Check loadingSceneAsync to ensure we don't double-invoke the scene change.
        // Check if NetworkServer.active because we can get here via Disconnect before server has started to change scenes.
        if (!string.IsNullOrEmpty(offlineScene) && !IsSceneActive(offlineScene) && loadingSceneAsync == null && !dingleton.Active) {
            ChangeScene(offlineScene, SceneOperation.Normal);
        }

        activeSceneName = "";
    }


    public void Disconnnect() {}


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool Kill() {
        if (!Instance) { return false; }

        spawnPositions.Clear();
        spawnPositionIndex = 0;
        // Instance.Stop

        // TODO: Convert data to binary, and store.

        return true;
    }



    // --------------------------------------------Getters-------------------------------------------- \\

    /// <summary>
    /// Get the next start position based on the selected spawn Method.
    /// <remarks>https://github.com/vis2k/Mirror/blob/8b363d11e9ff2d53878838efe61c0debf8240488/Assets/Mirror/Runtime/NetworkManager.cs#L1048</remarks>
    /// </summary>
    /// <returns></returns>
    public Transform GetAStartPosition() {
        // first remove any dead transforms
        spawnPositions.RemoveAll(t => t == null);

        if (spawnPositions.Count == 0)
            return null;

        if (playerSpawnMethod == PlayerSpawnMethod.Random) {
            return spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];
        }
        else {
            Transform startPosition = spawnPositions[spawnPositionIndex];
            spawnPositionIndex = (spawnPositionIndex + 1) % spawnPositions.Count;
            return startPosition;
        }
    }



    // --------------------------------------------Setters-------------------------------------------- \\

    
    // /// <summary>
    // /// Add an enemy to the scene manager's library to maintain track and control.
    // /// </summary>
    // /// <param name="enemy"></param>
    public void AddEnemy(Enemy enemy) { Enemies.Add(enemy); }
    


    // --------------------------------------------Mutilators-------------------------------------------- \\

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>https://github.com/vis2k/Mirror/blob/8b363d11e9ff2d53878838efe61c0debf8240488/Assets/Mirror/Runtime/NetworkManager.cs#L787</remarks>
    /// <param name="newSceneName"></param>
    /// <param name="sceneOperation"></param>
    /// <param name="customHandling"></param>
    public void ChangeScene(string newSceneName, SceneOperation sceneOperation = SceneOperation.Normal, bool customHandling = false) {
        if (isLoadingScene && newSceneName == activeSceneName) {
            Debug.LogError($"Scene change is already in progress for {newSceneName}");
            return;
        }

        activeSceneName = newSceneName;
        isLoadingScene = true;
    }


    public void SpawnObjects() {
        
    }
    
    






    // --------------------------------------------Checkers-------------------------------------------- \\

    /// <summary>
    /// Check if a scene is the active scene.
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    public static bool IsSceneActive(string scene) {
        Scene activeScene = SceneManager.GetActiveScene();
        return activeScene.path == scene || activeScene.name == scene;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool IsReady() {
        Debug.Log("The Scene is ready to commence.");
        return player.playerAvi.IsReady;
    }


    /// <summary>
    /// Notify the players that the lobby has filled and the game is ready to begin.
    /// </summary>
    public void NotifyOfReadyState() {
        // player.HandleReadyToStart(IsReady());

    }



    // --------------------------------------------Events-------------------------------------------- \\

    /// <summary>This is called when a client is stopped.</summary>
    public void OnStopClient() {
        // TODO: Send home data for record.
    }


    /// <summary>
    /// Response to when a temporary pause on the active game is called. 
    /// </summary>
    public void OnPauseClient() {
        // player.Clear();
    }


    /// <summary>
    /// 
    /// </summary>
    public void PrepareToSpawnSceneObjects() {
        
    }


    // support additive scene loads:
    //   NetworkScenePostProcess disables all scene objects on load, and
    //   * NetworkServer.SpawnObjects enables them again on the server when
    //     calling OnStartServer
    //   * NetworkClient.PrepareToSpawnSceneObjects enables them again on the
    //     client after the server sends ObjectSpawnStartedMessage to client
    //     in SpawnObserversForConnection. this is only called when the
    //     client joins, so we need to rebuild scene objects manually again
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode == LoadSceneMode.Additive) {
            if (dingleton.Active) {
                if (gameActive) {
                    SpawnObjects();
                    // TODO: still going to need?
                    PrepareToSpawnSceneObjects();
                    // Debug.Log($"Rebuild Client spawnableObjects after additive scene load: {scene.name}");
                }
            }
        }
    }

    
    /// <remarks>https://github.com/vis2k/Mirror/blob/8b363d11e9ff2d53878838efe61c0debf8240488/Assets/Mirror/Runtime/NetworkManager.cs#L910</remarks>
    private void FinishLoadingScene() {
        isLoadingScene = false;
    }




    /// <summary>
    /// default graphic user interface.
    /// </summary>
    public void OnGUI() {}

}