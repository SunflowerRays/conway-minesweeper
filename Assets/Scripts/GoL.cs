using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GoL : MonoBehaviour
{
    //Local UI elements
    [SerializeField] public Tilemap currentState;
    [SerializeField] public Tile aliveTile;
    [SerializeField] public float freqInterval;
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] public UnityEngine.UI.Slider generationSlider;
    [SerializeField] private UnityEngine.UI.Button ConfirmButton;
    [SerializeField] public UnityEngine.UI.Toggle DemoSwitch;

    //Other Monobehaviour Classes
    [SerializeField] public HashSet2TileMap HashSet2TileMap;
    [SerializeField] public MouseHandler mouseHandler;
    [SerializeField] public TextHandler textHandler;

    //Games state
    public (int x, int y) centre;
    public Grid grid;
    public LiveRegistry liveRegistry;
    public Generator generator;
    public MineDetector mineDetector;
    public MineHider mineHider;
    public PatternManager patternManager;
    public ScoreKeeper scoreKeeper;
    public bool isGeneratorRunning;


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
        mineHider = new MineHider(liveRegistry);
        generator = new Generator(grid, liveRegistry);
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
            //Disable UI elements
            DemoSwitch.gameObject.SetActive(false);

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
            ConfirmButton.image.color = new Color(1f, 0.84f, 0f);
            ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().color = new Color(0.7f, 0.13f, 0.13f);

        }
        else if (mouseHandler.mode == MouseHandler.GameMode.GameOver || mouseHandler.mode == MouseHandler.GameMode.Minesweeper)
        {

            ResetGame();

        }
    }
    /// <summary>
    /// Resets the game by 
    /// Updating the game state, enabling, disabling and updating the values of UI elements, as well as
    /// Calling a complementary ResetUI method in textHandler, and
    /// Setting the game mode to PatternEdit.
    /// </summary>
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
        DemoSwitch.gameObject.SetActive(true);
        generationSlider.gameObject.SetActive(false);
        mouseHandler.playerNameInput.gameObject.SetActive(false);
        mouseHandler.SubmitScore.gameObject.SetActive(false);

        //Set local UI values
        ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Simulate";
        ConfirmButton.image.color = Color.white;
        ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().color = new Color(0.72f, 0.53f, 0.04f);
        generationSlider.value = minGenerations;
        DemoSwitch.isOn = false;
        

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
            if (DemoSwitch.isOn)
            {
            generator.UpdateState(true);
            yield return new WaitForSeconds(freqInterval);
            } 
            else
            {
                generator.UpdateState(false);
            }
            liveRegistry.population = liveRegistry.aliveCells.Count;
            patternManager.patterns.Add(new HashSet<(int x, int y)>(liveRegistry.aliveCells));
            patternManager.minesPerPattern.Add(liveRegistry.population);
            if (liveRegistry.aliveCells.Count == 0 || liveRegistry.aliveCells.SetEquals(previousCells))
            {
                generationSlider.maxValue = i;
                break;
            }
        }
        //Enable UI elements
        generationSlider.gameObject.SetActive(true);
        textHandler.GeneratorCountText.gameObject.SetActive(true);
        ConfirmButton.interactable = true;
        //Set local UI values
        ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Minesweeper";
        ConfirmButton.image.color = Color.green;
        ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().color = new Color(1f, 0.55f, 0f);
        
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