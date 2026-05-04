using UnityEngine;
using TMPro;


public class TextHandler : MonoBehaviour
{
    [SerializeField] private GoL gol;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text mineCountText;

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


    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;
            DisplayTime(currentTime);

        }
        mineCountText.text = string.Format("Mines: {0}", mineCount);
    }
}
