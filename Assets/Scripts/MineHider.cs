using System;
using System.Collections.Generic;
using UnityEngine;

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

        foreach (var (i, j) in grid.GetAllCells())
        {

            topCells.Add((i, j));

        }
        onDetectionComplete?.Invoke();
    }

    public Boolean reveal(int x, int y)
    {


        if (topCells.Remove((x, y)))
        {
            return true;
        }

        return false;
    }

}
