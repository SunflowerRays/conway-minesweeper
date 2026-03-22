using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class GoLBoard : MonoBehaviour
{

    /// <summary>
    /// Tilemaps for current and upcoming states of play.
    /// The which state is stored in each tilemap will alternate,
    /// So they are arbitrarily named alpha and beta to distinguish them.
    /// </summary>
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;

    /// <summary>
    /// States that an organism in GoL can have.
    /// </summary>
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile deadTile;

    //Pattern class for objects like R-Pentomino
    [SerializeField] private Pattern pattern;
    //frequency of updates in seconds.
    [SerializeField] private float freqInterval = 0.20f;

    //Only tracks cells that are alive
    //for updates.
    //HashSet helps prevent duplicate entries, which is useful for optimisation,
    //and prevents the program from hanging on an area with multiple cells.
    private HashSet<Vector3Int> aliveCells;
    private HashSet<Vector3Int> cellsToCheck;

    public int population {  get; private set; }
    // public int maxPopulation { get; private set; }
    public int iterations { get; private set; }
    public float time {  get; private set; }


    private void Awake()
    {
        aliveCells = new HashSet<Vector3Int>();
        cellsToCheck = new HashSet<Vector3Int>();
    }

    public void Start()
    {
        SetPattern(pattern);
    }

    private void SetPattern(Pattern pattern)
    {

        Clear();

        //centering is optional
        Vector2Int centre = pattern.GetCentre();

        for (int i = 0; i < pattern.cells.Length; i++)
        {
            Vector3Int cell = (Vector3Int)(pattern.cells[i] - centre);
            currentState.SetTile(cell, aliveTile);
            aliveCells.Add(cell);
        }

        population = aliveCells.Count;

    }

    private void Clear()
    {
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
        aliveCells.Clear();
        cellsToCheck.Clear();
        population = 0;
        iterations = 0;
        time = 0f;
    }

    private void OnEnable()
    {
        StartCoroutine(Simulate());
    }

    private IEnumerator Simulate()
    {
        //Does not support changing interval during simulation.
        //Other cashing methods are possible.
        var interval = new WaitForSeconds(freqInterval);
        //show initial state of the pattern before the game begins.
        yield return interval;

        while (enabled)
        {
            UpdateState();

            //update metadata
            population = aliveCells.Count;
            iterations++;
            time += freqInterval;

            yield return interval;

        }


    }

    private void UpdateState()
    {
        cellsToCheck.Clear();

        foreach (Vector3Int cell in aliveCells)
        {
            for (int x = -1; x <= 1; x++)
            {

                for (int y = -1; y <= 1; y++)
                {
                    cellsToCheck.Add(cell + new Vector3Int(x, y));
                }

            }
        }

        foreach (Vector3Int cell in cellsToCheck)
        {
            int neighbours = CountNeighbours(cell);
            bool alive = IsAlive(cell);


            if (!alive && neighbours == 3)
            {
                nextState.SetTile(cell, aliveTile);
                aliveCells.Add(cell);
            } 
            else if (alive && (neighbours < 2 || neighbours > 3))
            {
                nextState.SetTile(cell, deadTile);
                aliveCells.Remove(cell);
            }
            else
            {
                nextState.SetTile(cell, currentState.GetTile(cell));
                //no change to alive cells.
            }
        }

        //swap tilemaps
        Tilemap temp = currentState;
        currentState = nextState;

        //unsure if this line is necessary.
        nextState = temp;
        
        nextState.ClearAllTiles();
    }

    private int CountNeighbours(Vector3Int cell)
    {
        int count = 0;

        for (int x = -1; x <= 1; x++)
        {

            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighbour = cell + new Vector3Int(x, y);

                //excludes the cell passed as a parameter from the count of its neighbours.
                if (x == 0 && y == 0)
                {
                    continue;
                } 
                else if (IsAlive(neighbour))
                {
                    count++;
                }
            }

        }

        return count;
    }


    private bool IsAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == aliveTile;

    }

}
