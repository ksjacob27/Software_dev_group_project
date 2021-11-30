using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    private        GameObject _PauseMenuGUI;               // Reference to the unity pause menu prefab object.
    private static bool       _isPaused           = false; // Boolean pause menu toggle variable with static persistence. {False: Hidden | True: Displayed}
    private        float      _volumeLvlReference = 1f;    //
    private        float      _timeReference      = 1f;    //


    // Start is called before the first frame update
    // void Start() {
    //     _PauseMenuGUI.SetActive(true);
    // }


    void Update() {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (_isPaused) { Resume(); }
        else { Pause(); }
    }


    /*
     * <summary>
     * </summary>
     * <param name=""></param>
     * <returns>void</returns>
     */
    private void Pause() {
        _PauseMenuGUI.SetActive(true);
        _isPaused           = true;
        _timeReference      = Time.timeScale;
        _volumeLvlReference = AudioListener.volume;

        Time.timeScale       = 0.001f;
        AudioListener.volume = 0f;
    }


    /*
     * <summary>
     * </summary>
     * <param name=""></param>
     * <returns>void</returns>
     */
    private void Resume() {
        _PauseMenuGUI.SetActive(false);
        _isPaused            = false;
        Time.timeScale       = _timeReference;
        AudioListener.volume = _volumeLvlReference;
    }


    /*
     * <summary>
     * </summary>
     * <param name=""></param>
     * <returns>void</returns>
     */
    private void QuitGame() {
        Application.Quit();
    }
}