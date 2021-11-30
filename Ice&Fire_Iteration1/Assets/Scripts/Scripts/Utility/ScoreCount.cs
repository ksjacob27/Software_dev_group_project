using Mirror;
using UnityEngine;


public class ScoreCount : NetworkBehaviour {
    [SyncVar] public int  _PlayerIndex;
    [SyncVar] public uint _PlayerScore;

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer() {
        _PlayerIndex = connectionToClient.connectionId;
    }




}
