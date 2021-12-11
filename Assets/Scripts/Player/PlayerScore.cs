using UnityEngine;



/* Tutorial: Mirror Examples: NetworkRoom */
public class PlayerScore : MonoBehaviour {
    public int  index;
    public uint score;

    void OnGUI() {
        GUI.Box(new Rect(10f + (index * 110), 10f, 100f, 25f), $"P{index}: {score:0000000}");
    }
}
