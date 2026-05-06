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

    //remember to reduce complexity
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

        foreach (var (x, y) in cellsToCheck)
        {
            int neighbours = CountNeighbours(x, y);
            bool alive = IsAlive(x, y);

            if (!alive && neighbours == 3 && IsInsideBounds(x, y))
            {
                liveRegistry.aliveCells.Add((x, y));
            }
            else if (alive && (neighbours < 2 || neighbours > 3))
            {
                liveRegistry.aliveCells.Remove((x, y));
            }
            else if (x < centre.x - grid.gridWidth / 2 || x > centre.x + grid.gridWidth / 2 || y < centre.y - grid.gridHeight / 2 || y > centre.y + grid.gridHeight / 2)
            {
                liveRegistry.aliveCells.Remove((x, y));
            }

            //cells in stable arrangments remain in aliveCells but do not require additional statements for that.

        }

        onGeneration?.Invoke();
    }



    private bool IsInsideBounds(int x, int y)
    {
        return x > centre.x - grid.gridWidth / 2 &&
               x < centre.x + grid.gridWidth / 2 &&
               y > centre.y - grid.gridHeight / 2 &&
               y < centre.y + grid.gridHeight / 2;
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
