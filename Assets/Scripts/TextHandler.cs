using Newtonsoft.Json;
using TMPro;
using Unity.Android.Gradle;
using UnityEngine;
using static ScoreKeeper;


public class TextHandler : MonoBehaviour
{
    [SerializeField] private GoL gol;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] public TMP_Text mineCountText;
    [SerializeField] public TMP_Text GeneratorCountText;

    [SerializeField] public TMP_Text highScoreText;
    [SerializeField] public GameObject highScorePanel;

    // https://gamedevbeginner.com/how-to-make-countdown-timer-in-unity-minutes-seconds/
    private bool isMinesweeperRunning;
    public bool isGeneratorFinished;
    public float currentTime = 0;
    public int mineCount = 0;
    public int generationCount = 0;



    void Start()
    {
        mineCount = gol.liveRegistry.population;
        gol.mineHider.onGameStart += () => isMinesweeperRunning = true;
        gol.patternManager.onAddCell += () => mineCount++;
        gol.patternManager.onSubtractCell += () => mineCount--;
    }

    public void Stop()
    {
        isGeneratorFinished = false;
        isMinesweeperRunning = false;
    }


    /// <summary>
    /// Displays the list of high scores in the user interface, showing completion times, and win or loss
    /// status.
    /// </summary>
    /// <remarks>This method retrieves the most recent high scores and updates the high score panel to make it
    /// visible to the user. The display includes the rank, time in seconds, and whether the level was
    /// cleared. The method does not return a value and is typically called after a game session or when the user
    /// requests to view high scores.</remarks>
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


        if (!isMinesweeperRunning)
        {
            if (isGeneratorFinished)
            {
                int index = (int)gol.generationSlider.value;
                generationCount = index;
                mineCount = gol.patternManager.minesPerPattern[index];
            }
        }

        if (isMinesweeperRunning)
        {
            currentTime += Time.deltaTime;
            float minutes = Mathf.FloorToInt(currentTime / 60);
            float seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            timerText.text = string.Format("Time: {0:00}:{1:00}", 0, 0);
        }
        mineCountText.text = string.Format("Mines: {0}", mineCount);
        GeneratorCountText.text = string.Format("Generation: {0}", generationCount);

    }
}
