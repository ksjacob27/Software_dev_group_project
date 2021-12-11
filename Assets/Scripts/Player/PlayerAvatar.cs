using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



/*https://mirror-networking.gitbook.io/docs/components/network-room-player*/
/*https://github.com/vis2k/Mirror/blob/master/Assets/Mirror/Runtime/NetworkManager.cs/#L1204*/

/// <summary>
/// This component works in conjunction with the NetworkRoomManager to make up the
/// multiplayer room system. The RoomPrefab object of the NetworkRoomManager must have
/// this component on it. This component holds basic room player data required for the room to function.
///
/// Partial of Mirror's RoomPlayer.
/// </summary>
public class PlayerAvatar : MonoBehaviour {
    public static readonly string _ConditionLibrary_ = "Effects/";

    [Header("Character")]
    private Avatar avatar;
    public Guid player_ID;
    
    [Header("Builders")]
    public Builder[] AvailableBuilds;
    private Builder SelectedBuildTable;
    public  string  JsonFile;

    [Header("UI")]
    public GameObject RootUI;
    public HealthBarUI  HealthUI;
    public ToolBarUI    ToolUI;
    public TMP_Dropdown characterDropdownMenu;
    public Button       startGameButton;
    public Button       closeLobbyButton;
    public Button       readyUserButton;
    public TMP_Text     playerNameTexts;
    public TMP_Text     playerReadyTexts;

    // Server Variables / Events \\
    public string PlayerAlias = "...";
    public bool   IsReady     = false;

    // public void HandleDisplayNameChanged(string oldVal, string newVal) => LobbyDisplayUpdate();
    // public void HandleReadyStatusChanged(bool   oldVal, bool   newVal) => LobbyDisplayUpdate();


    private GameManager game;
    public GameManager Game {
        get {
            if (game != null) return game;
            return game = GameManager.Instance as GameManager;
        }
    }



    // ---------------------------------------------------- Getters ---------------------------------------------------- \\
    public Builder GetSelectedBuild() {
        return SelectedBuildTable;
    }



    // ---------------------------------------------------- Events ---------------------------------------------------- \\
    public void HandleReadyToStart(bool readyToStart) {
        startGameButton.gameObject.SetActive(readyToStart);
        startGameButton.interactable = readyToStart;
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="buildIndex"></param>
    public void SelectCharacterBuild(int buildIndex) {
        Debug.Log($"SelectCharacterBuild({buildIndex}) called");
        SelectedBuildTable = AvailableBuilds[buildIndex];
    }


    /// <summary>
    /// 
    /// </summary>
    public void OnStart() {
        // SetPlayerAlias(Random.Range(0, 4).ToString());
        // Game.player = this;
        RootUI.SetActive(true);
        // LobbyDisplayUpdate();
        SelectCharacterBuild(0);
    }


    /*/// <summary>
    /// 
    /// </summary>
    public void CloseLobby() {
        Debug.Log("Event_CloseLobbyUI");
        RootUI.SetActive(false);
        Game.ChangeScene(Game.MainMenu);
    }*/


    /// <summary>
    /// 
    /// </summary>
    public void OnStop() {
        Game.ChangeScene(Game.MainMenu);
    }



    /*/*https://mirror-networking.gitbook.io/docs/components/network-room-player#1#
    private void LobbyDisplayUpdate() {
        Debug.Log("LobbyDisplayUpdate Called!");


        // playerNameTexts.text = "...";
        // playerReadyTexts.text = string.Empty;


        playerNameTexts.text = PlayerAlias;
        playerReadyTexts.text = IsReady ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";


        if (AvailableBuilds == null) { return; }
        List<TMP_Dropdown.OptionData> optionsDropDown = AvailableBuilds.Select(t => new TMP_Dropdown.OptionData(t._BuildTitle)).ToList();
        characterDropdownMenu.options = optionsDropDown;
    }*/


    
    public void PostBuff(Health   affected, ConditionInventory candi) { ObjectPool.pingleton.spawnedObjects[affected.GetComponent<ObjectInfo>().ObjectId].GetComponent<Health>().AddBuff(Resources.Load<ConditionInventory>($"{_ConditionLibrary_}{candi}"), avatar); }
    public void PostDebuff(Health affected, ConditionInventory candi) { ObjectPool.pingleton.spawnedObjects[affected.GetComponent<ObjectInfo>().ObjectId].GetComponent<Health>().AddDebuff(Resources.Load<ConditionInventory>($"{_ConditionLibrary_}{candi}"), avatar); }
    public void DealDamage(Health affected, float amount) { ObjectPool.pingleton.spawnedObjects[affected.GetComponent<ObjectInfo>().ObjectId].GetComponent<Health>().OnDamageTaken(amount, avatar); }
}
