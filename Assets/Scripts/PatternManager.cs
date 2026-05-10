
using System.Collections.Generic;
using System;

public class PatternManager
{

    //public HashSet<(int x, int y)> pattern { get; private set; }

    public LiveRegistry liveRegistry;
    public event Action onAddCell;
    public event Action onSubtractCell;
    public List<HashSet<(int x, int y)>> patterns { get; set; }
    public List<int> minesPerPattern;


    /// <summary>
    /// Initializes a new instance of the PatternManager class using the specified live registry.
    /// </summary>
    /// <param name="liveRegistry">The LiveRegistry instance that provides access to the current state or configuration required by the pattern
    /// manager. Cannot be null.</param>
    public PatternManager(LiveRegistry liveRegistry)
    {
        this.liveRegistry = liveRegistry;
        patterns = new List<HashSet<(int x, int y)>>();
        minesPerPattern = new List<int>();
    }

    /// <summary>
    /// Toggles the alive state of the cell at the specified coordinates.
    /// </summary>
    /// <remarks>Invokes a callback when a cell is added or removed. Use this method to change the state of a
    /// single cell in the grid.</remarks>
    /// <param name="x">The zero-based horizontal coordinate of the cell to toggle.</param>
    /// <param name="y">The zero-based vertical coordinate of the cell to toggle.</param>
    /// <returns>true if the cell was previously alive and is now dead; otherwise, false.</returns>
    public bool ToggleCell(int x, int y)
    {
        if (liveRegistry.aliveCells.Contains((x, y)))
        {
            liveRegistry.aliveCells.Remove((x, y));
            onSubtractCell?.Invoke();
            return true;
        }
        else
        {
            liveRegistry.aliveCells.Add((x, y));
            onAddCell?.Invoke();
            return false;
        }
    }

    /// <summary>
    /// Removes all cells from the current pattern, resetting the state to empty.
    /// </summary>
    /// <remarks>Call this method to clear any existing pattern before defining a new one. This operation
    /// affects only the current pattern and does not persist changes automatically.</remarks>
    public void ClearPattern()
    {
        liveRegistry.aliveCells.Clear();
    }
    public (int x, int y) GetCentre()
    {
        if (liveRegistry.population == 0)
        {
            return (0, 0);
        }
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        foreach (var (x, y) in liveRegistry.aliveCells)
        {
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }
        return ((minX + maxX) / 2, (minY + maxY) / 2);
    }

}


