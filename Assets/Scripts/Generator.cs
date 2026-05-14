using System;
using System.Collections.Generic;


public class Generator
{

    private LiveRegistry liveRegistry;

    private HashSet<(int x, int y)> cellsToCheck;
    private (int x, int y) centre;

    private Grid grid;

    public event Action onGeneration;

    public Generator(Grid grid, LiveRegistry liveRegistry, (int x, int y) centre)
    {
        //required
        this.grid = grid;
        this.liveRegistry = liveRegistry;
        this.centre = centre;


        cellsToCheck = new HashSet<(int x, int y)>();

    }

    private bool IsAlive(int x, int y)
    {
        return liveRegistry.aliveCells.Contains((x, y));
    }

    public void UpdateState()
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

            if (!alive && neighbours == 3 && IsInsideBounds(x, y))
            {
                toAdd.Add((x, y));
            }
            else if (alive && (neighbours < 2 || neighbours > 3))
            {
                toRemove.Add((x, y));
            }
            else if (alive && !IsInsideBounds(x, y))
            {
                toRemove.Add((x, y));
            }
        }

        foreach (var cell in toAdd) liveRegistry.aliveCells.Add(cell);
        foreach (var cell in toRemove) liveRegistry.aliveCells.Remove(cell);

        liveRegistry.population = liveRegistry.aliveCells.Count;
        onGeneration?.Invoke();
    }



    private bool IsInsideBounds(int x, int y)
    {
        if (x < grid.centre.x - grid.gridWidth / 2 ||
            x >= grid.centre.x + grid.gridWidth / 2 ||
            y < grid.centre.y - grid.gridHeight / 2 ||
            y >= grid.centre.y + grid.gridHeight / 2) return false;

        return true;
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


}
