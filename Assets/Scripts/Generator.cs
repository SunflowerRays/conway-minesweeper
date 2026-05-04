using System;
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
    private HashSet<(int x, int y)> cellsToCheck;
    private (int x, int y) centre;
    public bool isRunning = false;
    private GoL gol;
    private MineDetector mineDetector;
    private HashSet2TileMap hashSet2TileMap;

    public event Action onGeneration;


    public Generator(GoL gol, LiveRegistry liveRegistry, Tilemap currentState, Tilemap nextState, Tile aliveTile, Tile deadTile, (int x, int y) centre, MineDetector mineDetector = null, HashSet2TileMap hashSet2TileMap = null)
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

        cellsToCheck = new HashSet<(int x, int y)>();

    }

    private bool IsAlive(int x, int y)
    {
        return liveRegistry.newAliveCells.Contains((x, y));
    }

    // TODO: Decrease Cognitive Complexity of this Method. The paper from the university in Bangladesh should help with this.
    // TODO: Refactor to use plain C#.
    public void UpdateState()
    {
        cellsToCheck.Clear();

        foreach (var (x, y) in liveRegistry.newAliveCells)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    cellsToCheck.Add((x + dx, y + dy));
                }
            }
        }

        foreach (var (x, y) in cellsToCheck)
        {
            int neighbours = CountNeighbours(x, y);
            bool alive = IsAlive(x, y);

            if (!alive && neighbours == 3 && IsInsideBounds(x, y))
            {
                nextState.SetTile(new Vector3Int(x, y, 0), aliveTile);
                liveRegistry.newAliveCells.Add((x, y));
            }
            else if (alive && (neighbours < 2 || neighbours > 3))
            {
                nextState.SetTile(new Vector3Int(x, y, 0), deadTile);
                liveRegistry.newAliveCells.Remove((x, y));
            }
            else if (x < centre.x - gol.grid.gridWidth / 2 || x > centre.x + gol.grid.gridWidth / 2 || y < centre.y - gol.grid.gridHeight / 2 || y > centre.y + gol.grid.gridHeight / 2)
            {
                nextState.SetTile(new Vector3Int(x, y, 0), deadTile);
                liveRegistry.newAliveCells.Remove((x, y));
            }
            else
            {
                nextState.SetTile(new Vector3Int(x, y, 0), currentState.GetTile(new Vector3Int(x, y, 0)));
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
            mineDetector.detectorOverAllCells();
        }

    }

    public void Stop()
    {
        isRunning = false;
    }

    private bool IsInsideBounds(int x, int y)
    {
        return x > centre.x - gol.grid.gridWidth / 2 &&
               x < centre.x + gol.grid.gridWidth / 2 &&
               y > centre.y - gol.grid.gridHeight / 2 &&
               y < centre.y + gol.grid.gridHeight / 2;
    }


    private int CountNeighbours(int x, int y)
    {
        int count = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                if (IsAlive(x + dx, y + dy))
                {
                    count++;
                }
            }
        }

        return count;
    }

    public IEnumerator Simulate()
    {

        isRunning = true;


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

            onGeneration?.Invoke();

            yield return new WaitForSeconds(gol.freqInterval);
        }


    }
}
