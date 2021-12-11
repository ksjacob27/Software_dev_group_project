using System;
using UnityEngine;


public class ScoresMenuController : MonoBehaviour {

    private Transform entryContainer;
    private Transform entryTemplate;

    private void Awake() {
        entryContainer = transform.Find("HighScoresTable");
        entryTemplate = entryContainer.Find("ScoreEntryTemplate");
        entryTemplate.gameObject.SetActive(false);
    }

    
}

