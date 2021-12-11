using UnityEngine;
using UnityEngine.UI;



public class PlayerGUI : MonoBehaviour {
    public Text playerName;

    public void SetPlayerInfo(PlayerInfo info) {
        playerName.text = $"Player {info.playerId}";
        playerName.color = info.ready ? Color.green : Color.red;
    }
}
