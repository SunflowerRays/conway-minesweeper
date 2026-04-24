using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GoL : MonoBehaviour
{
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile deadTile;
    //[SerializeField] private Tile borderTile;
    //[SerializeField] private Pattern pattern;
    public HashSet<(int x, int y)> pattern { get; set; }

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

    public int iterations { get; internal set; }
    public float time { get; internal set; }

    private void Awake()
    {
        pattern = new HashSet<(int x, int y)>();
        liveRegistry = new LiveRegistry();
        patternManager = new PatternManager();
        grid = new Grid(centre, gridWidth, gridHeight);
        mineDetector = new MineDetector(grid, liveRegistry);
        mineHider = new MineHider(grid);
        generator = new Generator(this, liveRegistry, currentState, nextState, aliveTile, deadTile, centre, mineDetector);
    }

    public void Start()
    {

        SetPattern(pattern);
        if (generator != null)
        {
            StartCoroutine(generator.Simulate());
        }
    }

    private void SetPattern(HashSet<(int x, int y)> pattern)
    {
        Clear();

        (int x, int y) localCentre = (0, 0);
        localCentre = patternManager.GetCentre(pattern);


        foreach (var (x, y) in pattern)
        {
            //Vector3Int cell = new Vector3Int(x - localCentre.x, y - localCentre.y, 0);


            //currentState.SetTile(cell, aliveTile);
            // liveRegistry.aliveCells.Add(cell);
            liveRegistry.newAliveCells.Add((x, y));
        }



        //liveRegistry.population = liveRegistry.aliveCells.Count;
        liveRegistry.population = liveRegistry.newAliveCells.Count;
        centre = localCentre;
        mineHider.coverMines(grid);
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
            generator.Stop();
        }
    }


    private void OnEnable()
    {

        /* reactivate if restarting the generator becomes necessary later.
        if (generator != null)
        {
            StartCoroutine(generator.Simulate());
        }
        */
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}