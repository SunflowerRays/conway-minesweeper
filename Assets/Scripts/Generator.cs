using System;
using System.Collections.Generic;


public class Generator
{

    private LiveRegistry liveRegistry;

    private HashSet<(int x, int y)> cellsToCheck;
    private (int x, int y) centre;

    private Grid grid;
    private MineDetector mineDetector;
    
    public event Action onGeneration;


    public Generator(Grid grid, LiveRegistry liveRegistry, (int x, int y) centre, MineDetector mineDetector = null)
    {
        //required
        this.grid = grid;
        this.liveRegistry = liveRegistry;
        this.centre = centre;

        //optional
        this.mineDetector = mineDetector;


        cellsToCheck = new HashSet<(int x, int y)>();

    }

    private bool IsAlive(int x, int y)
    {
        return liveRegistry.newAliveCells.Contains((x, y));
    }

    //remember to reduce complexity
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

                //Add alive cell
                liveRegistry.newAliveCells.Add((x, y));
            }
            else if (alive && (neighbours < 2 || neighbours > 3))
            {

                liveRegistry.newAliveCells.Remove((x, y));
            }
            else if (x < centre.x - grid.gridWidth / 2 || x > centre.x + grid.gridWidth / 2 || y < centre.y - grid.gridHeight / 2 || y > centre.y + grid.gridHeight / 2)
            {

                liveRegistry.newAliveCells.Remove((x, y));
            }

            //cells in stable arrangments remain in aliveCells but do not require additional statements for that.

        }


        //from here
        //This conditonal statement may make it possible to have a GoL only mode.
        if (mineDetector != null)
        {
            mineDetector.detectorOverAllCells();
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
