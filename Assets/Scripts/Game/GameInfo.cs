using System;
using UnityEngine;



/// <summary>
/// Spawning information
/// </summary>
public struct SpawnMessage {
    public Guid                 objId;
    public Vector3              position;
    public Quaternion           rotation;
    public ServerMatchOperation serverMatchOperation;
}


/// <summary>
/// 
/// </summary>
public enum SceneOperation : byte {
    Normal,
    LoadAdditive,
    UnloadAdditive
}


/// <summary>
/// Match message to be sent to the client
/// </summary>
public struct ClientMatchMessage {
    public ClientMatchOperation clientMatchOperation;
    public Guid                 matchId;
    public MatchInfo[]          matchInfos;
    public PlayerInfo[]         playerInfos;
}



/// <summary>
/// Information about a match
/// </summary>
[Serializable]
public struct MatchInfo {
    public Guid matchId;
    public byte players;
    public byte maxPlayers;
}



/// <summary>
/// Information about a player
/// </summary>
[Serializable]
public struct PlayerInfo {
    public Guid playerId;
    public bool ready;
    public Guid matchId;
}



[Serializable]
public struct ObjectInfo {
    public Guid ObjectId;
    public bool ready;
    public Guid matchId;
}



[Serializable]
public struct MatchPlayerData {
    public int playerIndex;
    public int wins;
}



/// <summary>
/// Match operation to execute on the client
/// </summary>
public enum ClientMatchOperation : byte {
    None,
    List,
    Created,
    Cancelled,
    Joined,
    Departed,
    UpdateRoom,
    Started
}



/// <summary>
/// Match operation to execute on the server
/// </summary>
public enum ServerMatchOperation : byte {
    None,
    Create,
    Cancel,
    Start,
    Join,
    Leave,
    Ready
}
