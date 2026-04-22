using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GoL : MonoBehaviour
{
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile deadTile;
    //[SerializeField] private Tile borderTile;
    [SerializeField] private Pattern pattern;
    [SerializeField] public float freqInterval = 0.20f;
    [SerializeField] private int gridWidth = 12;
    [SerializeField] private int gridHeight = 12;
    public Vector2Int centre;
    public Grid grid;
    public LiveRegistry liveRegistry;
    public Generator generator;
    public MineDetector mineDetector;

    public int iterations { get; internal set; }
    public float time { get; internal set; }

    private void Awake()
    {
        liveRegistry = new LiveRegistry();
        grid = new Grid(centre, gridWidth, gridHeight);
        mineDetector = new MineDetector(grid, liveRegistry);
        //TODO: Add a toggleable setting for whether MineDetector.detector will trigger on each generaton,
        //If not exclude the MineDetector file from the Generator constructor as shown here.
        //generator = new Generator(this, liveRegistry, currentState, nextState, aliveTile, deadTile, centre);
        generator = new Generator(this, liveRegistry, currentState, nextState, aliveTile, deadTile, centre, mineDetector);
        
    }

    public void Start()
    {
        SetPattern(pattern);
    }

    private void SetPattern(Pattern pattern)
    {
        Clear();

        Vector2Int localCentre = new Vector2Int();

        for (int i = 0; i < pattern.cells.Length; i++)
        {
            localCentre = pattern.GetCentre();
            Vector3Int cell = (Vector3Int)(pattern.cells[i] - localCentre);
            currentState.SetTile(cell, aliveTile);
            //TODO: Refactor
            liveRegistry.aliveCells.Add(cell);
            liveRegistry.newAliveCells.Add((cell.x, cell.y));
        }

        //liveRegistry.population = liveRegistry.aliveCells.Count;
        liveRegistry.population = liveRegistry.newAliveCells.Count;
        centre = localCentre;
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

    private void OnEnable()
    {
        StartCoroutine(generator.Simulate());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}