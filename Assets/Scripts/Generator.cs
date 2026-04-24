using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;


public class Generator
{

    private LiveRegistry liveRegistry;
    private Tilemap currentState;
    private Tilemap nextState;
    private Tile aliveTile;
    private Tile deadTile;
    private HashSet<Vector3Int> cellsToCheck;
    private Vector2Int centre;
    private bool isRunning = true;
    private GoL gol;
    private MineDetector mineDetector;
    private HashSet2TileMap hashSet2TileMap;

    public Generator(GoL gol, LiveRegistry liveRegistry, Tilemap currentState, Tilemap nextState, Tile aliveTile, Tile deadTile, Vector2Int centre, MineDetector mineDetector = null, HashSet2TileMap hashSet2TileMap = null)
    {
        //required
        this.gol = gol;
        this.liveRegistry = liveRegistry;
        this.currentState = currentState;
        this.nextState = nextState;
        this.aliveTile = aliveTile;
        this.deadTile = deadTile;
        this.centre = centre;

        //optional
        this.mineDetector = mineDetector;
        this.hashSet2TileMap = hashSet2TileMap;

        cellsToCheck = new HashSet<Vector3Int>();

    }

    public bool IsAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == aliveTile;

    }


    // TODO: Decrease Cognitive Complexity of this Method. The paper from the university in Bangladesh should help with this.
    // TODO: Refactor to use plain C#.
    public void UpdateState()
    {
        cellsToCheck.Clear();

        foreach (Vector3Int cell in liveRegistry.aliveCells)
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


            if (!alive && neighbours == 3 && IsInsideBounds(cell))
            {
                nextState.SetTile(cell, aliveTile);
                liveRegistry.aliveCells.Add(cell);
                liveRegistry.newAliveCells.Add((cell.x, cell.y));
            }
            else if (alive && (neighbours < 2 || neighbours > 3))
            {
                nextState.SetTile(cell, deadTile);
                liveRegistry.aliveCells.Remove(cell);
                liveRegistry.newAliveCells.Remove((cell.x, cell.y));
            }
            else if (cell.x < centre.x - gol.grid.gridWidth / 2 || cell.x > centre.x + gol.grid.gridWidth / 2 || cell.y < centre.y - gol.grid.gridHeight / 2 || cell.y > centre.y + gol.grid.gridHeight / 2)
            {
                nextState.SetTile(cell, deadTile);
                liveRegistry.aliveCells.Remove(cell);
                liveRegistry.newAliveCells.Remove((cell.x, cell.y));
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

        if (mineDetector != null)
        {
            mineDetector.detector();
        }

    }

    public void Stop()
    {
        isRunning = false;
    }

    private bool IsInsideBounds(Vector3Int cell)
    {
        return cell.x > centre.x - gol.grid.gridWidth / 2 &&
               cell.x < centre.x + gol.grid.gridWidth / 2 &&
               cell.y > centre.y - gol.grid.gridHeight / 2 &&
               cell.y < centre.y + gol.grid.gridHeight / 2;
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

    public IEnumerator Simulate()
    {




        //Does not support changing interval during simulation.
        //Other cashing methods are possible.
        //show initial state of the pattern before the game begins.
        yield return new WaitForSeconds(gol.freqInterval);

        while (isRunning)
        {
            UpdateState();
            liveRegistry.population = liveRegistry.newAliveCells.Count;
            gol.iterations++;
            gol.time += gol.freqInterval;

            yield return new WaitForSeconds(gol.freqInterval);
        }


    }
}
