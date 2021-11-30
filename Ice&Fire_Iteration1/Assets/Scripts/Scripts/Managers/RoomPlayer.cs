using System.Reflection;
using UnityEngine;
using Mirror;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



/*https://mirror-networking.gitbook.io/docs/components/network-room-player*/

/// <summary>
/// This component works in conjunction with the NetworkRoomManager to make up the multiplayer room system.
/// The RoomPrefab object of the NetworkRoomManager must have this component on it.
/// This component holds basic room player data required for the room to function.
/// Game specific data for room players can be put in other components on the RoomPrefab or in scripts derived from NetworkRoomPlayer.
///
/// Partial of Mirror's Room-Player.
/// </summary>
public class RoomPlayer : NetworkRoomPlayer {
    [Header("Characters")]
    public Builder[] _AvailableBuilds;
    private Builder p_SelectedBuildTable;

    [Header("UI")] public GameObject   lobbyUI          = null;
    public                TMP_Text[]   playerNameTexts  = new TMP_Text[4];
    public                TMP_Text[]   playerReadyTexts = new TMP_Text[4];
    public                Button       startGameButton  = null;
    public                TMP_Dropdown characterDropdown;


    // Server Variables / Events \\
    [SyncVar(hook = nameof(HandleDisplayNameChanged))] public string PlayerAlias = "...";
    public                                                    void   HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    [SyncVar(hook = nameof(HandleReadyStatusChanged))] public bool IsReady = true;
    public                                                    void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

    private bool p_IsServerLeader;
    public bool IsLeader {
        set {
            p_IsServerLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    private RoomManager p_Scene;
    public RoomManager Scene {
        get {
            if (p_Scene != null) return p_Scene;
            return p_Scene = NetworkManager.singleton as RoomManager;
        }
    }

    // ---------------------------------------------------- Getters ---------------------------------------------------- \\
    public Builder GetSelectedBuild() {
        return p_SelectedBuildTable;
    }


    // ---------------------------------------------------- Events ---------------------------------------------------- \\
    public void HandleReadyToStart(bool readyToStart) {
        if (!p_IsServerLeader) { return; }
        startGameButton.gameObject.SetActive(readyToStart);
        startGameButton.interactable = readyToStart;
    }


    public void Replace(GameObject player) {
        player.GetComponent<Player>().Client_SetPlayerAlias(PlayerAlias);
        NetworkServer.ReplacePlayerForConnection(connectionToClient, player);
        NetworkServer.Destroy(gameObject);
    }


    public override void OnStartAuthority() {
        Command_SetPlayerAlias(Random.Range(0, 100).ToString());
        lobbyUI.SetActive(true);
        // LoadBuilds();
        UpdateDisplay();
        SelectCharacterBuild(0);
    }


    public void CloseLobbyUI() {
        lobbyUI.SetActive(false);
    }


    public override void OnStartClient() {
        foreach (RoomPlayer player in Scene.RoomPlayers) {
            player.gameObject.SetActive(false);
        }
        (Scene)?.RoomPlayers.Add(this);
    }

    public override void OnStopClient() {
        Scene.RoomPlayers.Remove(this);
        UpdateDisplay();
    }

    public void LeaveRoom() {
        Scene.StopClient();
        if (isServer) {
            NetworkServer.DisconnectAll();
        }
        SceneManager.LoadScene("MainMenu");
    }


    private void UpdateDisplay() {
        if (!hasAuthority) {
            for (int i = 0; i < Scene.RoomPlayers.Count; i++) {
                RoomPlayer player = Scene.RoomPlayers[i];
                if (player.hasAuthority) {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++) {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Scene.RoomPlayers.Count; i++) {
            playerNameTexts[i].text = Scene.RoomPlayers[i].PlayerAlias;
            playerReadyTexts[i].text = Scene.RoomPlayers[i].IsReady ? "<color=blue>Ready</color>" : "<color=red>Not Ready</color>";
        }

        if (_AvailableBuilds == null) {
            return;
        }
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < _AvailableBuilds.Length; i++) {
            options.Add(new TMP_Dropdown.OptionData(_AvailableBuilds[i]._BuildTitle));
        }
        characterDropdown.options = options;
    }

    public void SelectCharacterBuild(int pindex) {
        if (!hasAuthority)
            return;
        p_SelectedBuildTable = _AvailableBuilds[pindex];
        Command_SelectCharacterBuild(p_SelectedBuildTable.ToString());
    }



    [ClientRpc] private void Client_SelectCharacterBuild(string toolboxJson) {
        p_SelectedBuildTable = Builder.LoadDataFromString(toolboxJson);
    }




    public void ReadyUp() {
        Command_ReadyUp();
        CmdChangeReadyState(IsReady);
    }


    // ---------------------------------------------------- Remote Command Calls ---------------------------------------------------- \\
    [Command] private void Command_SelectCharacterBuild(string toolboxJson) {
        p_SelectedBuildTable = Builder.LoadDataFromString(toolboxJson);
        Client_SelectCharacterBuild(toolboxJson);
    }


    [Command] private void Command_SetPlayerAlias(string displayName) {
        PlayerAlias = displayName;
    }


    [Command] public void Command_ReadyUp() {
        Debug.Log("CmdReadyUp");
        IsReady = !IsReady;
        Debug.Log($"IsReady? {IsReady}");
        Scene.NotifyOfReadyState();
    }


    [Command] public void Command_StartGame() {
        Debug.Log("CmdStartGame");
        Scene.StartGame();
    }

    public override void OnGUI() {}
}
