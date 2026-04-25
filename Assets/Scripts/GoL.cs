using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static MouseHandler;
using static UnityEngine.InputSystem.HID.HID;

public class GoL : MonoBehaviour
{
    [SerializeField] public Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] public Tile aliveTile;
    [SerializeField] private Tile deadTile;

    [SerializeField] public float freqInterval = 0.20f;
    [SerializeField] private int gridWidth = 12;
    [SerializeField] private int gridHeight = 12;

    public (int x, int y) centre;
    public Grid grid;
    public LiveRegistry liveRegistry;
    public Generator generator;
    public MineDetector mineDetector;
    public MineHider mineHider;
    public PatternManager patternManager;
    [SerializeField] public HashSet2TileMap HashSet2TileMap;
    [SerializeField] public MouseHandler mouseHandler;


    public int iterations { get; internal set; }
    public float time { get; internal set; }

    private void Awake()
    {
        centre = (0, 0);
        liveRegistry = new LiveRegistry();
        patternManager = new PatternManager(liveRegistry);
        grid = new Grid(centre, gridWidth, gridHeight);
        mineDetector = new MineDetector(grid, liveRegistry);
        mineHider = new MineHider(grid);
        generator = new Generator(this, liveRegistry, currentState, nextState, aliveTile, deadTile, centre, mineDetector);
    }

    public void Start()
    {

        patternManager.SetPattern();
        liveRegistry.population = liveRegistry.newAliveCells.Count;
        //mineHider.coverMines(grid);
        HashSet2TileMap.mapper(liveRegistry.newAliveCells, currentState);

    }

    private void Clear()
    {
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();

        //TODO Refactor
        liveRegistry.aliveCells.Clear();
        liveRegistry.newAliveCells.Clear();

        liveRegistry.population = 0;
        iterations = 0;
        time = 0f;
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (!generator.isRunning)
            {
                patternManager.SetPattern();
                mouseHandler.SetMode(MouseHandler.GameMode.Simulating);
                StartCoroutine(generator.Simulate());
            }
            else
            {
                generator.Stop();
                mouseHandler.SetMode(MouseHandler.GameMode.Minesweeper);
            }
        }
    }


    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}