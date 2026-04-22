using System;
using System.Collections.Generic;

public class MineHider
{

    private Grid grid;
    public HashSet<(int x, int y)> topCells { get; set; }

    //To keep this class a pure C# class with no Unity elements, so it remains testable
    public event Action onDetectionComplete;

    public MineHider(Grid grid)
    {
        topCells = new HashSet<(int x, int y)>();
        this.grid = grid;

    }

    public void coverMines(Grid grid)
    {
        topCells.Clear();

        for (int i = (grid.centre.x - (grid.gridWidth / 2)); i <= (grid.centre.x + (grid.gridWidth / 2)); ++i)
            {

                for (int j = (grid.centre.y - (grid.gridHeight / 2)); j <= (grid.centre.y + (grid.gridHeight / 2)); ++j)
                {

                topCells.Add((i, j));

            }



        }
        onDetectionComplete?.Invoke();
    }


}
