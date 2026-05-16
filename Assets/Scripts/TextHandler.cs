using TMPro;
using UnityEngine;

public class TextHandler : MonoBehaviour
{
    //GoL class
    [SerializeField] private GoL gol;
    //Local UI elements
    [SerializeField] private TMP_Text mineCountText;
    [SerializeField] public TMP_Text timerText;
    [SerializeField] public TMP_Text GeneratorCountText;
    [SerializeField] public TMP_Text highScoreText;
    [SerializeField] public GameObject highScorePanel;

    //Games state
    public bool isMinesweeperRunning;
    public float currentTime = 0;
    public int mineCount = 0;
    public int generationCount = 0;

    void Start()
    {
        mineCount = gol.liveRegistry.population;

        //Event Listeners
        gol.mineHider.onGameStart += () => isMinesweeperRunning = true;
        gol.patternManager.onAddCell += () => mineCount++;
        gol.patternManager.onSubtractCell += () => mineCount--;
    }

    /// <summary>
    /// Resets UI elements and gameplay state to their default inactive values: hides panels and text fields, resets the
    /// timer and mine count, and marks the game as not running.
    /// </summary>
    /// <remarks>Call when starting, restarting, or ending a game session to clear runtime UI and state. This
    /// operation does not persist data or modify saved scores.</remarks>
    public void ResetUI()
    {
        //Enable and Disable UI elements
        highScorePanel.SetActive(false);
        timerText.gameObject.SetActive(false);
        GeneratorCountText.gameObject.SetActive(false);

        //Set UI values
        currentTime = 0;
        mineCount = 0;

        //Misc
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

    /// <summary>
    /// Updates runtime state and UI each frame: when Minesweeper is not running, synchronizes generationCount and
    /// mineCount from the generator and slider; when running, advances the elapsed timer; always refreshes the timer,
    /// mine count, and generation text fields.
    /// </summary>
    /// <remarks>Called once per frame (Unity Update). Accumulates elapsed time using Time.deltaTime and
    /// formats the timer as MM:SS. When not running, validates the slider index against
    /// gol.patternManager.minesPerPattern before assigning generationCount and mineCount. Updates timerText,
    /// mineCountText, and GeneratorCountText with formatted values.</remarks>
    void Update()
    {
        if (!isMinesweeperRunning)
        {
            if (!gol.isGeneratorRunning && gol.patternManager.minesPerPattern.Count > 0)
            {   
                int index = (int)gol.generationSlider.value;
                if (index >= 0 && index < gol.patternManager.minesPerPattern.Count)
                {
                    //User can see generation number and
                    //the number of mines in the at generation.
                    generationCount = index;
                    mineCount = gol.patternManager.minesPerPattern[index];
                }
            }
        }
        //Timer starts when minesweeper is and sets to 'Time: 00:00' once minesweeper stops running.
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
        //Mine Count and Generation Count are updated with variables mineCount and generationCount.
        mineCountText.text = string.Format("Mines: {0}", mineCount);
        GeneratorCountText.text = string.Format("Generation: {0}", generationCount);
    }
}
