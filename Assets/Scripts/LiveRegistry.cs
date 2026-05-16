using System.Collections.Generic;

/// <summary>
/// Data container class that records the positions of all living cells
/// As well as the population
/// At a given point in time.
/// </summary>
public class LiveRegistry
{
    public HashSet<(int x, int y)> aliveCells { get; set; }
    // Suggestion Moore Neighbourhood HashSet for evaluation by both minesweeper and GoL.
    // *Moore Neighbourhood of each alive cell.
    public int population { get; set; }
    public LiveRegistry()
    {
        aliveCells = new HashSet<(int x, int y)>();
    }
}
