using System;
using System.Collections.Generic;

public class MineHider
{

    private Grid grid;
    private LiveRegistry liveRegistry;
    public HashSet<(int x, int y)> topCells { get; set; }

    //To keep this class a pure C# class with no Unity elements, so it remains testable
    public event Action onDetectionComplete;
    public event Action onGameStart;
    public event Action onWin;
    public MineHider(Grid grid, LiveRegistry liveRegistry)
    {
        topCells = new HashSet<(int x, int y)>();
        this.grid = grid;
        this.liveRegistry = liveRegistry;

    }

    public void coverMines(Grid grid)
    {
        topCells.Clear();

        foreach (var (i, j) in grid.GetAllCells())
        {

            topCells.Add((i, j));

        }


        onDetectionComplete?.Invoke();
        onGameStart?.Invoke();

    }

    public Boolean reveal(int x, int y)
    {


        if (topCells.Remove((x, y)))
        {
            if (topCells.Count == liveRegistry.newAliveCells.Count)
            {
                onWin?.Invoke();
            }

            return true;
        }

        return false;
    }

}
