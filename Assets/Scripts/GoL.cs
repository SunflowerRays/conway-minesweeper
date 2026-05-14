using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class GoL : MonoBehaviour
{
    [SerializeField] public Tilemap currentState;
    [SerializeField] public Tile aliveTile;
    [SerializeField] public float freqInterval;
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] public UnityEngine.UI.Slider generationSlider;
    [SerializeField] private UnityEngine.UI.Button ConfirmButton;

    public (int x, int y) centre;
    public Grid grid;
    public LiveRegistry liveRegistry;
    public Generator generator;
    public MineDetector mineDetector;
    public MineHider mineHider;
    public PatternManager patternManager;
    public ScoreKeeper scoreKeeper;
    public bool isGeneratorRunning;
    [SerializeField] public HashSet2TileMap HashSet2TileMap;
    [SerializeField] public MouseHandler mouseHandler;
    [SerializeField] public TextHandler textHandler;

    // Score Display Settings
    [SerializeField] public int numberOfHighScores;

    // Generation Settings
    private int minGenerations = 1;
    [SerializeField] private int maxGenerations;



    /// <summary>
    /// Initializes component dependencies and prepares the object for use. Called by Unity when the script instance is
    /// being loaded.
    /// </summary>
    /// <remarks>This method is invoked automatically by Unity before any Start methods. It sets up required
    /// services and managers to ensure the component operates correctly during its lifecycle.</remarks>
    private void Awake()
    {
        liveRegistry = new LiveRegistry();
        patternManager = new PatternManager(liveRegistry);
        grid = new Grid(centre, gridWidth, gridHeight);
        mineDetector = new MineDetector(grid, liveRegistry);
        mineHider = new MineHider(grid, liveRegistry);
        generator = new Generator(grid, liveRegistry, centre);
        scoreKeeper = new ScoreKeeper(Application.persistentDataPath);
    }


    /// <summary>
    /// Initializes the simulation by updating the population count and configuring the generation slider to the allowed
    /// range.
    /// </summary>
    /// <remarks>Call this method before starting the simulation to ensure that the population and generation
    /// controls are set to their initial states.</remarks>
    public void Start()
    {
        liveRegistry.population = liveRegistry.aliveCells.Count;
        generationSlider.minValue = minGenerations;
        generationSlider.maxValue = maxGenerations;
        textHandler.GeneratorCountText.text = "Pattern Setter";

    }


    /// <summary>
    /// Handles the logic for the confirm button based on the current game mode.
    /// </summary>
    /// <remarks>This method transitions the game between pattern editing, simulation, and gameplay modes when
    /// the confirm button is pressed. It updates the game state, UI elements, and relevant data structures according to
    /// the current mode. Call this method in response to user interaction with the confirm button.</remarks>
    public void OnConfirmButtonPressed()
    {
        if (mouseHandler.mode == MouseHandler.GameMode.PatternEdit)
        {
            patternManager.patterns.Add(new HashSet<(int x, int y)>(liveRegistry.aliveCells));
            patternManager.minesPerPattern.Add(liveRegistry.population);
            Debug.Log("Cells in Gen: " + 0 + string.Join(", ", patternManager.patterns[0]));

            mouseHandler.SetMode(MouseHandler.GameMode.Simulating);
            StartCoroutine(Simulate());
            generationSlider.gameObject.SetActive(true);
        }
        else if (mouseHandler.mode == MouseHandler.GameMode.Simulating)
        {
            int selectedIndex = (int)generationSlider.value;
            Debug.Log("Selected Generation is " + selectedIndex);
            liveRegistry.aliveCells = new HashSet<(int x, int y)>(patternManager.patterns[selectedIndex]);
            liveRegistry.population = liveRegistry.aliveCells.Count;
            generationSlider.gameObject.SetActive(false);

            StopGenerator();

            mouseHandler.SetMode(MouseHandler.GameMode.Minesweeper);

            Debug.Log($"Grid: width={grid.gridWidth}, height={grid.gridHeight}, centre={grid.centre.x},{grid.centre.y}");
            Debug.Log($"topCells count: {mineHider.topCells.Count}");



            ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Restart Game";
            ConfirmButton.image.color = new Color(0.29f, 0f, 0.51f);
        }
        else if (mouseHandler.mode == MouseHandler.GameMode.GameOver || mouseHandler.mode == MouseHandler.GameMode.Minesweeper)
        {

            mouseHandler.isGameOver = false;
            liveRegistry.aliveCells.Clear();
            liveRegistry.population = 0;
            patternManager.patterns.Clear();
            patternManager.minesPerPattern.Clear();
            currentState.ClearAllTiles();
            HashSet2TileMap.clearMinefield();
            HashSet2TileMap.clearGreyfield();
            ConfirmButton.interactable = true;
            textHandler.highScorePanel.SetActive(false);
            mouseHandler.SetMode(MouseHandler.GameMode.PatternEdit);
            textHandler.currentTime = 0;
            textHandler.mineCount = 0;
            generationSlider.gameObject.SetActive(false);
            textHandler.GeneratorCountText.text = "Pattern Setter";



            ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Confirm Pattern";
            ConfirmButton.image.color = Color.white;

        }
    }


    /// <summary>
    /// Runs the simulation of cell generations, updating the state and UI as the simulation progresses.
    /// </summary>
    /// <remarks>This method disables the confirm button while the simulation is running and re-enables it
    /// upon completion. The simulation stops if there are no live cells or if the state does not change between
    /// generations. Intended to be used as a coroutine in Unity.</remarks>
    /// <returns>An enumerator that advances the simulation through each generation, yielding control between steps.</returns>
    private IEnumerator Simulate()
    {


        isGeneratorRunning = true;

        ConfirmButton.interactable = false;


        for (int i = minGenerations; i <= maxGenerations; i++)
        {
            HashSet<(int x, int y)> previousCells = new HashSet<(int x, int y)>(liveRegistry.aliveCells);
            generator.UpdateState();


            liveRegistry.population = liveRegistry.aliveCells.Count;
            patternManager.patterns.Add(new HashSet<(int x, int y)>(liveRegistry.aliveCells));
            Debug.Log("Cells in Gen: " + i + string.Join(", ", patternManager.patterns[i]));
            patternManager.minesPerPattern.Add(liveRegistry.population);
            if (liveRegistry.aliveCells.Count == 0 || liveRegistry.aliveCells.SetEquals(previousCells))
            {
                generationSlider.maxValue = i;
                break;
            }

            yield return new WaitForSeconds(freqInterval);
        }


        ConfirmButton.interactable = true;

        // Change text
        ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Start Minesweeper";

        // Change colour
        ConfirmButton.image.color = Color.green;

        textHandler.isGeneratorFinished = true;

    }

    /// <summary>
    /// Stops the generator if it is currently running.
    /// </summary>
    /// <remarks>Call this method to halt generator operations. After calling this method, the generator will
    /// no longer produce output until restarted.</remarks>
    public void StopGenerator()
    {
        isGeneratorRunning = false;
    }

    /// <summary>
    /// Called when the component becomes disabled or inactive.
    /// </summary>
    /// <remarks>Use this method to perform any necessary cleanup or to stop ongoing operations when the
    /// component is disabled. In Unity, this is typically used to halt coroutines or release resources associated with
    /// the component.</remarks>
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}