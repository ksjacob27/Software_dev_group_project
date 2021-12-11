using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
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
    public static readonly string QUICK_PLAY_PATH    = "EndOfTheRoad";

    // private static Builder        builder;
    public TMP_InputField serverConnection;
    
    public GameObject PauseMenu;
    public GameObject UserInterface;
    public GameObject MainMenu;
    public GameObject CreditsMenu;
    public GameObject SettingsMenu;
    public GameObject ScoreRecordMenu;
    public GameObject ActiveMenu;

    public GameObject LoginButton;
    public GameObject LogoutButton;
    public GameObject UsernameInputField;
    public GameObject PasswordInputField;
    public GameObject PlayWithLoginButton;
    public GameObject HighScoresButton;
    
    public GameObject UsernameLoggedIn;



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
        ActiveMenu = MainMenu;
        SettingsMenu.SetActive(false);
        CreditsMenu.SetActive(false);
        ScoreRecordMenu.SetActive(false);
        LoginButton.SetActive(true);
        LogoutButton.SetActive(false);
        PlayWithLoginButton.SetActive(false);
        UsernameInputField.SetActive(true);
        PasswordInputField.SetActive(true);
    }


    private void Update() {
        if (ActiveMenu == MainMenu) {
            if (!DBManager.LoggedIn) {
                LoginButton.SetActive(true);
                LogoutButton.SetActive(false);
                PlayWithLoginButton.SetActive(false);
                UsernameLoggedIn.SetActive(false);
                UsernameInputField.SetActive(true);
                PasswordInputField.SetActive(true);
            }
            else {
                LoginButton.SetActive(false);
                LogoutButton.SetActive(true);
                PlayWithLoginButton.SetActive(true);
                UsernameLoggedIn.SetActive(true);
                UsernameInputField.SetActive(false);
                PasswordInputField.SetActive(false);
            }
        }
    }


    private void OnServerInitialized() {
        throw new NotImplementedException();
    }

    public void LoadBuilderForm() {
        SceneManager.LoadScene(_BUILDER_PATH_);
    }

    public void TryConnect() {

        // if (TryConnect(out indexConnectionStatus))
        // {
        //     Console.Error.WriteLine("Could not connect to {0}:\r\n{1}",
        //         settings.Host, indexConnectionStatus.Error.OriginalException.Message);
        //     Console.Read();
        //     return;
        // }
    }

    public void TryLogin() {
        
    }  
    
    public void Logout() {
        
    }
    
    public void ToggleCredits() {
        MainMenu.SetActive(false);
        CreditsMenu.SetActive(true);
        ActiveMenu = CreditsMenu;
    }
    
    
    public void ToggleSettings() {
        MainMenu.SetActive(false);
        SettingsMenu.SetActive(true);
        ActiveMenu = SettingsMenu;
    }     
    
    
    public void ToggleHighScores() {
        MainMenu.SetActive(false);
        ScoreRecordMenu.SetActive(true);
        ActiveMenu = ScoreRecordMenu;
    } 
    
    
    public void BackToMain() {
        if (ActiveMenu != MainMenu) {
            ActiveMenu.SetActive(false);
            ActiveMenu = MainMenu;
            ActiveMenu.SetActive(true);
        }
    }

    
    public void QuickPlayStart() {

        SceneManager.LoadScene(QUICK_PLAY_PATH);
    }

}


internal class DBManager {
    public static bool LoggedIn { get; set; }
}


public class DifficultyManager {}
