using MognomUtils;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : PersistentSingleton<GameStateManager> {

    [SerializeField] private IntEventChannel waveOverChannel;

    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private string gameSceneName;
    [SerializeField] private string lootSceneName;
    [SerializeField] private string inventorySceneName;

    [SerializeField] private GameState currentState;

    public int CurrentWave { get; private set; }
    private int currentScore;
    private int bestScore;

    private enum GameState {
        MAINMENU,
        WAVE,
        LOOT,
        INVENTORY
    }

    protected override void Awake() {
        base.Awake();
        if (GameStateManager.I == this) {
            waveOverChannel.Channel += OnWaveOver;
            CurrentWave = 1;
            bestScore = 0;
            currentScore = -1;
        }
    }

    private void OnWaveOver(int pointsEarned) {
        if (currentState == GameState.WAVE) {
            if (pointsEarned > 0) {
                currentState = GameState.INVENTORY;
                CurrentWave++;
                currentScore += pointsEarned;
                StartCoroutine(SwapScene(inventorySceneName));
            } else {
                // Player lost
                currentState = GameState.MAINMENU;
                bestScore = Mathf.Max(currentScore, bestScore);
                StartCoroutine(SwapScene(mainMenuSceneName));
            }
        }
    }

    private IEnumerator SwapScene(string newScene) {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene(newScene);
    }

    public void GoToNextScene() {
        switch (currentState) {
            case GameState.MAINMENU:
                CurrentWave = 1;
                currentScore = 0;
                SceneManager.LoadScene(gameSceneName);
                currentState = GameState.WAVE;
                break;
            case GameState.WAVE:
                SceneManager.LoadScene(lootSceneName);
                currentState = GameState.LOOT;
                break;
            case GameState.LOOT:
                SceneManager.LoadScene(inventorySceneName);
                currentState = GameState.INVENTORY;
                break;
            case GameState.INVENTORY:
                SceneManager.LoadScene(gameSceneName);
                currentState = GameState.WAVE;
                break;
        }
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public int GetBestScore() {
        return bestScore;
    }

    public int GetCurrentScore() {
        return currentScore;
    }
}
