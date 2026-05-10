using System.Collections.Generic;

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
