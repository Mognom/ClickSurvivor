using TMPro;
using UnityEngine;

// This class handles the score display on the MainMenu
public class MainMenuManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private string currentScoreString;
    [SerializeField] private string bestScoreString;

    private void Start() {
        int newScore = GameStateManager.I.GetCurrentScore();
        int bestScore = GameStateManager.I.GetBestScore();

        if (newScore == -1) {
            currentScoreText.text = "";
        } else {
            currentScoreText.text = currentScoreString + newScore;
        }
        bestScoreText.text = bestScoreString + bestScore;
    }
}
