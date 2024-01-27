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
            currentScore = 0;
        }
    }

    private void OnWaveOver(int pointsEarned) {
        StartCoroutine(HandleWaveOver(pointsEarned));
    }

    private IEnumerator HandleWaveOver(int pointsEarned) {
        yield return new WaitForSeconds(1);
        if (pointsEarned > 0) {
            SceneManager.LoadScene(inventorySceneName);
            currentState = GameState.INVENTORY;
            CurrentWave++;
            currentScore += pointsEarned;
        } else {
            // Player lost
            SceneManager.LoadScene(mainMenuSceneName);
            currentState = GameState.MAINMENU;
        }
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
}
