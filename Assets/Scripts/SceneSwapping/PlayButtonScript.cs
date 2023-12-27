using UnityEngine;

public class PlayButtonScript : MonoBehaviour {
    [SerializeField] private string sceneName;

    public void GoToNextScene() {
        GameStateManager.I.GoToNextScene();
    }
}
