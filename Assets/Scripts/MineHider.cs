using System;
using System.Collections.Generic;
using System.Diagnostics;


public class MineHider
{

    private Grid grid;
    private LiveRegistry liveRegistry;
    public HashSet<(int x, int y)> topCells { get; set; }

    //To keep this class a pure C# class with no Unity elements, so it remains testable
    public event Action onCoverageComplete;
    public event Action onGameStart;
    public event Action onWin;
    public MineHider(Grid grid, LiveRegistry liveRegistry)
    {
        topCells = new HashSet<(int x, int y)>();
        this.grid = grid;
        this.liveRegistry = liveRegistry;

    }

    /// <summary>
    /// Covers all cells in the specified grid, preparing the game for play.
    /// </summary>
    /// <remarks>This method resets the internal state to cover all cells and triggers any registered coverage
    /// completion and game start events. Call this method before starting a new game to ensure the grid is properly
    /// initialized.</remarks>
    /// <param name="grid">The grid whose cells will be covered. Cannot be null.</param>
    public void coverMines(Grid grid)
    {
        topCells.Clear();
        foreach (var (i, j) in grid.GetAllCells())
        {
            topCells.Add((i, j));
        }

        onCoverageComplete?.Invoke();
        onGameStart?.Invoke();
    }

    /// <summary>
    /// Reveals the cell at the specified coordinates and updates the game state accordingly.
    /// </summary>
    /// <remarks>If revealing the cell results in all non-alive cells being revealed, a win event is
    /// triggered. This method does not reveal cells that have already been revealed.</remarks>
    /// <param name="x">The zero-based horizontal coordinate of the cell to reveal.</param>
    /// <param name="y">The zero-based vertical coordinate of the cell to reveal.</param>
    /// <returns>true if the cell was successfully revealed; otherwise, false.</returns>
    public Boolean reveal(int x, int y)
    {
        if (topCells.Remove((x, y)))
        {
            if (topCells.Count == liveRegistry.aliveCells.Count)
            {
                onWin?.Invoke();
            }
            return true;
        }
        return false;
    }
}
