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
        //textHandler.GeneratorCountText.text = "Pattern Setter";
        ResetGame();
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
            //Store initial pattern
            patternManager.patterns.Add(new HashSet<(int x, int y)>(liveRegistry.aliveCells));
            patternManager.minesPerPattern.Add(liveRegistry.population);
            
            //Set game mode
            mouseHandler.SetMode(MouseHandler.GameMode.Simulating);
            
            //Start GoL generator
            StartCoroutine(Simulate());
            
        }
        else if (mouseHandler.mode == MouseHandler.GameMode.Simulating)
        {
            //Enable and Disable UI elements
            generationSlider.gameObject.SetActive(false);
            textHandler.timerText.gameObject.SetActive(true);

            //Index chosen generation
            int selectedIndex = (int)generationSlider.value;
            liveRegistry.aliveCells = new HashSet<(int x, int y)>(patternManager.patterns[selectedIndex]);
            liveRegistry.population = liveRegistry.aliveCells.Count;

            //Set game mode
            mouseHandler.SetMode(MouseHandler.GameMode.Minesweeper);



            //Set local UI values
            ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Restart Game";
            ConfirmButton.image.color = Color.gold;
            ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().color = Color.firebrick;

        }
        else if (mouseHandler.mode == MouseHandler.GameMode.GameOver || mouseHandler.mode == MouseHandler.GameMode.Minesweeper)
        {

            ResetGame();

        }
    }

    private void ResetGame()
    {
        //Games state
        mouseHandler.isGameOver = false;
        liveRegistry.aliveCells.Clear();
        liveRegistry.population = 0;
        patternManager.patterns.Clear();
        patternManager.minesPerPattern.Clear();
        currentState.ClearAllTiles();
        HashSet2TileMap.clearMinefield();
        HashSet2TileMap.clearGreyfield();
        
        //Enable and Disable UI elements
        generationSlider.gameObject.SetActive(false);

        //Set local UI values
        ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Simulate";
        ConfirmButton.image.color = Color.white;
        ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().color = Color.darkGoldenRod;
        generationSlider.value = minGenerations;

        //Call reset method in another class
        textHandler.ResetUI();

        //Set game mode
        mouseHandler.SetMode(MouseHandler.GameMode.PatternEdit);
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

        //Enable UI elements
        generationSlider.gameObject.SetActive(true);
        textHandler.GeneratorCountText.gameObject.SetActive(true);
        ConfirmButton.interactable = true;

        //Set local UI values
        ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Start Minesweeper";
        ConfirmButton.image.color = Color.green;
        ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().color = Color.darkOrange;

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