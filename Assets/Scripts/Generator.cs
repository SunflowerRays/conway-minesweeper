using System;
using System.Collections.Generic;


public class Generator
{

    private LiveRegistry liveRegistry;

    
    private HashSet<(int x, int y)> cellsToCheck;

    private Grid grid;
    //Event that can be invoked by UpdateState method.
    //Includes empty delegate so the value of the event cannot be null.
    public event Action OnGeneration = delegate { };

    public Generator(Grid grid, LiveRegistry liveRegistry)
    {
        this.grid = grid;
        this.liveRegistry = liveRegistry;

        cellsToCheck = new HashSet<(int x, int y)>();

    }

    private bool IsAlive(int x, int y)
    {
        return liveRegistry.aliveCells.Contains((x, y));
    }


    /// <summary>
    /// Applies the rules of Conway's Game of Life (GoL) to create a new pattern.
    /// *The pattern can be thought of as like a new generation of individual organisms.
    /// 
    /// The LiveRegistry is updated with the new pattern, by
    /// Adding each new living cell, and
    /// Removing each dead cell.
    /// 
    /// The population in LiveRegistry is also updated to match the new pattern.
    /// 
    /// </summary>
    /// <param name="demoMode"></param>
    public void UpdateState(bool demoMode = false)
    {
        cellsToCheck.Clear();

        foreach (var (x, y) in liveRegistry.aliveCells)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    cellsToCheck.Add((x + dx, y + dy));
                }
            }
        }

        HashSet<(int x, int y)> toAdd = new HashSet<(int x, int y)>();
        HashSet<(int x, int y)> toRemove = new HashSet<(int x, int y)>();

        foreach (var (x, y) in cellsToCheck)
        {
            int neighbours = CountNeighbours(x, y);
            bool alive = IsAlive(x, y);

            if (!alive && neighbours == 3 && grid.IsInsideBounds(x, y))
            {
                toAdd.Add((x, y));
            }
            else if (alive && (neighbours < 2 || neighbours > 3))
            {
                toRemove.Add((x, y));
            }
            else if (alive && !grid.IsInsideBounds(x, y))
            {
                toRemove.Add((x, y));
            }
        }

        foreach (var cell in toAdd) liveRegistry.aliveCells.Add(cell);
        foreach (var cell in toRemove) liveRegistry.aliveCells.Remove(cell);

        liveRegistry.population = liveRegistry.aliveCells.Count;

        if (demoMode)
        {
            OnGeneration.Invoke();
        }
        
    }

    /// <summary>
    /// Finds the number of living neighbours of the cell at the coordinates (x,y).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>Returns the number of living neighbours of the cell at the given coordinates.</returns>
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


}
