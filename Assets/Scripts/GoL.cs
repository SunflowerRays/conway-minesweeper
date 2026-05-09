using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class GoL : MonoBehaviour
{
    [SerializeField] public Tilemap currentState;
    [SerializeField] public Tile aliveTile;
    [SerializeField] private Tile deadTile;
    [SerializeField] public float freqInterval;
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private UnityEngine.UI.Slider generationSlider;
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

    [SerializeField] public int numberOfHighScores;
    private int minGenerations = 1;
    [SerializeField] private int maxGenerations;
    

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

    public void Start()
    {
        liveRegistry.population = liveRegistry.aliveCells.Count;
        generationSlider.minValue = minGenerations;
        generationSlider.maxValue = maxGenerations;
    }

    void Update()
    {

    }

    public void OnConfirmButtonPressed()
    {
        if (mouseHandler.mode == MouseHandler.GameMode.PatternEdit)
        {    
            mouseHandler.SetMode(MouseHandler.GameMode.Simulating);
            StartCoroutine(Simulate());
        }
        else if (mouseHandler.mode == MouseHandler.GameMode.Simulating)
        {
            int selectedIndex = (int)generationSlider.value-1;
            liveRegistry.aliveCells = new HashSet<(int x, int y)>(patternManager.patterns[selectedIndex]);
            liveRegistry.population = liveRegistry.aliveCells.Count;


            //debugging

            //Debug.Log("Selected index: " + selectedIndex);
            //Debug.Log("minesPerPattern count: " + patternManager.minesPerPattern.Count);
            //Debug.Log("patterns count: " + patternManager.patterns.Count);
            //Debug.Log("Selected mine count: " + patternManager.minesPerPattern[selectedIndex]);



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


            // Change text
            ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Confirm Pattern";

            // Change colour
            ConfirmButton.image.color = Color.white;

        }
    }

    private void OnEnable()
    {

    }

    //Test Changes.
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
            patternManager.minesPerPattern.Add(liveRegistry.population);
            if (liveRegistry.aliveCells.Count == 0) break;
            if (liveRegistry.aliveCells.SetEquals(previousCells)) break;
            yield return new WaitForSeconds(freqInterval);
        }


        ConfirmButton.interactable = true;

        // Change text
        ConfirmButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Start Minesweeper";

        // Change colour
        ConfirmButton.image.color = Color.green;

        textHandler.isGeneratorFinished = true;

    }

    //isGeneratorRunning = true;

    //while (isGeneratorRunning)
    //{
    //    generator.UpdateState();
    //    liveRegistry.population = liveRegistry.aliveCells.Count;
    //    iterations++;
    //    time += freqInterval;
    //    yield return new WaitForSeconds(freqInterval);
    //}


    public void StopGenerator()
    {
        isGeneratorRunning = false;
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }
}