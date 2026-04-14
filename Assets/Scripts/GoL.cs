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

    public int iterations { get; internal set; }
    public float time { get; internal set; }

    private void Awake()
    {
        liveRegistry = new LiveRegistry();
        grid = new Grid(centre, gridWidth, gridHeight);
        generator = new Generator(this, liveRegistry, currentState, nextState, aliveTile, deadTile, centre);
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
            liveRegistry.aliveCells.Add(cell);
        }

        liveRegistry.population = liveRegistry.aliveCells.Count;
        centre = localCentre;
    }

    private void Clear()
    {
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
        liveRegistry.aliveCells.Clear();
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