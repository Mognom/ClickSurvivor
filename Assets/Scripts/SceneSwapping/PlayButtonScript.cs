using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonScript : MonoBehaviour {
    [SerializeField] private string sceneName;

    public void GoToScene() {
        SceneManager.LoadScene(sceneName);
    }
}
