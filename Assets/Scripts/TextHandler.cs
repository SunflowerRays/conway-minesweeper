using Newtonsoft.Json;
using TMPro;
using Unity.Android.Gradle;
using UnityEngine;
using static ScoreKeeper;


public class TextHandler : MonoBehaviour
{
    [SerializeField] private GoL gol;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text mineCountText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private GameObject highScorePanel;

    // https://gamedevbeginner.com/how-to-make-countdown-timer-in-unity-minutes-seconds/
    private bool isRunning;
    public float currentTime = 0;
    public int mineCount = 0;

    void Start()
    {
        gol.mineHider.onGameStart += () => isRunning = true;
        gol.patternManager.onAddCell += () => mineCount++;
        gol.patternManager.onSubtractCell += () => mineCount--;
        gol.generator.onGeneration += () => mineCount = gol.liveRegistry.population;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void showHighScores()
    {
        ScoreKeeper.LatestScore[] scores = gol.scoreKeeper.loadScores(gol.numberOfHighScores);
        string display = "";
        for (int i = 0; i < scores.Length; i++)
        {
            display += $"{i + 1}. {scores[i].playerName} - {scores[i].time:F2}s - {(scores[i].levelCleared ? "Win" : "Loss")}\n";
        }
        highScoreText.text = display;
        highScorePanel.SetActive(true);
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;
            float minutes = Mathf.FloorToInt(currentTime / 60);
            float seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
        mineCountText.text = string.Format("Mines: {0}", mineCount);
    }
}
