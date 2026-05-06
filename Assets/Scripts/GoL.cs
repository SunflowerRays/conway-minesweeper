using System.Collections;
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

    public int numberOfHighScores = 3;

    public int iterations { get; internal set; }
    public float time { get; internal set; }

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
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (!isGeneratorRunning)
            {
                mouseHandler.SetMode(MouseHandler.GameMode.Simulating);
                StartCoroutine(Simulate());
            }
            else
            {
                StopGenerator();
                mouseHandler.SetMode(MouseHandler.GameMode.Minesweeper);
            }
        }
    }

    private void OnEnable()
    {

    }


    private IEnumerator Simulate()
    {
        isGeneratorRunning = true;

        while (isGeneratorRunning)
        {
            generator.UpdateState();
            liveRegistry.population = liveRegistry.aliveCells.Count;
            iterations++;
            time += freqInterval;
            yield return new WaitForSeconds(freqInterval);
        }
    }

    public void StopGenerator()
    {
        isGeneratorRunning = false;
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }
}