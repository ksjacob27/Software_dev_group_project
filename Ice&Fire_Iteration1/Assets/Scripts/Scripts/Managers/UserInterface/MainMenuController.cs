using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;
using UnityEngine.UI;



public class MainMenuController : MonoBehaviour {
    // private static MainMenuController _MENU_;
    private static DifficultyManager _DIFFICULTYMANAGER_;
    public static  bool              IsLevelLoading;
    public static  bool              IsPaused;

    public const           string _BUILDER_PATH_     = "CharacterBuilder";
    public static readonly string SETTINGS_PATH      = "Settings.xml";
    public static readonly string SETTINGS_SAVE_PATH = "Settings.xml";
    public static readonly string GAME_SAVE_PATH     = "Settings.xml";

    public TMP_InputField serverConnection;


    public GameObject PauseMenu;
    public GameObject UserInterface;

    public Button Register_Button;
    public Button Exit_Button;
    public Button NewGame_Button;
    public Button Load_Button;
    public Button Return_Button;
    public Button Next_Button;

    public ScrollRect Channel_Scroll;


    // public static MainMenuController Menu { get { return _MENU_; } }
    public static DifficultyManager DifficultyManager { get { return _DIFFICULTYMANAGER_; } }


    public static bool Pause {
        get { return IsPaused; }
        set {
            // if (value != IsPaused) _MENU_.TogglePause();
            IsPaused = value;
        }
    }
    public void TogglePause()  { throw new NotImplementedException(); }
    public void ToggleResume() { throw new NotImplementedException(); }



    private void Start() {
        // _MENU_ = this;
        if (DBManager.LoggedIn) {
            throw new NotImplementedException();
        }
    }

    private void OnServerInitialized() {
        throw new NotImplementedException();
    }

    public void LoadBuilderForm() {
        SceneManager.LoadScene(_BUILDER_PATH_);
    }

    public void TryConnect() {
        NetworkManager.singleton.networkAddress = serverConnection.text;
        NetworkManager.singleton.StartClient();
    }
}


internal class DBManager {
    public static bool LoggedIn { get; set; }
}


public class DifficultyManager {}
