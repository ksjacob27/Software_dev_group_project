using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;



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
public class RoomManager : NetworkRoomManager {
    public List<RoomPlayer>   RoomPlayers { get; } = new List<RoomPlayer>();
    public List<Player> Players = new List<Player>();



    // private void Awake() {
    //     Instance = this;
    // }


    // ----------------------------------------------------Server Loading---------------------------------------------------- \\
    public override void OnRoomStopServer() {
        RoomPlayers.Clear();
    }


    /// <summary>
    /// Called on the server when a client disconnects.
    /// </summary>
    /// <param name="connection">The connection that disconnected.</param>
    public override void OnRoomServerDisconnect(NetworkConnection connection) {
        if (connection.identity != null) {
            RoomPlayer player = connection.identity.GetComponent<RoomPlayer>();
            RoomPlayers.Remove(player);

            NotifyOfReadyState();
        }

        // If we are in the gameplay scene, check if we can go back to the room scene if all players are gone
        if (SceneManager.GetActiveScene().path == GameplayScene && RoomPlayers.Count < 1) {
            SceneManager.LoadScene(RoomScene);
        }
        base.OnRoomServerDisconnect(connection);
    }



    // --------------------------------------------Checkers-------------------------------------------- \\
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool IsReady() {
        Debug.Log("The Scene is ready to commence.");
        Debug.Log($"{numPlayers} < {minPlayers}: {numPlayers < minPlayers}");
        if (numPlayers < minPlayers) { return false; }

        Debug.Log($"Player count: {RoomPlayers.Count}");
        foreach (RoomPlayer player in RoomPlayers) {
            Debug.Log($"Player {player.PlayerAlias} IsReady? {player.IsReady}");
            if (!player.IsReady) { return false; }
        }
        return true;
    }


    /// <summary>
    /// Initiate the game 
    /// </summary>
    public void StartGame() {
        Debug.Log("StartGame");
        Debug.Log($"In room scene? {SceneManager.GetActiveScene().path == RoomScene}");
        if (SceneManager.GetActiveScene().path == RoomScene) {
            Debug.Log($"Ready to start? {IsReady()}");
            if (!IsReady()) { return; }
            ServerChangeScene(GameplayScene);
        }
    }


    /*XXX*/
    public override void OnRoomClientSceneChanged(NetworkConnection conn) {
        Debug.Log("OnRoomClientSceneChanged");
        if (SceneManager.GetActiveScene().path == GameplayScene) {
            for (int i = 0; i < RoomPlayers.Count; i++) {
                RoomPlayers[i].gameObject.SetActive(false);
            }
        }
        base.OnRoomClientSceneChanged(conn);
    }


    /// <summary>
    /// This is called on the server when all the players in the room are ready.
    /// <para>The default implementation of this function uses ServerChangeScene() to switch to the game player scene. By implementing this callback you can customize what happens when all the players in the room are ready, such as adding a countdown or a confirmation for a group leader.</para>
    /// </summary>
    public override void OnRoomServerPlayersReady() {
        Debug.Log("OnRoomServerPlayersReady");
        base.OnRoomServerPlayersReady();

        // #if (UNITY_SERVER)
        //     base.OnRoomServerPlayersReady();
        // }
        // else {
        // }
        // #endif
    }


    // /// <summary>
    // /// Add an enemy to the scene manager's library to maintain track and control.
    // /// </summary>
    // /// <param name="enemy"></param>
    // public void AddEnemy(Enemy enemy) { Enemies.Add(enemy); }


    /// <summary>
    ///  Add active, connected players to the scene controllers library to help maintain track and control. 
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(RoomPlayer player) { RoomPlayers.Add(player); }


    /// <summary>
    /// Notify the players that the lobby has filled and the game is ready to begin.
    /// </summary>
    public void NotifyOfReadyState() {
        foreach (RoomPlayer player in RoomPlayers) {
            player.HandleReadyToStart(IsReady());
        }
    }
    

    /// <summary>
    /// Override Mirror Network's default graphic user interface.
    /// </summary>
    public override void OnGUI() {}

}
