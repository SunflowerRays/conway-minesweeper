using System.Collections.Generic;
using UnityEngine;

public class LiveRegistry
{
    public HashSet<Vector3Int> aliveCells { get; set; }
    public HashSet<(int x, int y)> newAliveCells { get; set; }
    // Suggestion Moore Neighbourhood HashSet for evaluation by both minesweeper and GoL.
    // *Moore Neighbourhood of each alive cell.
    public int population { get; set; }


    public LiveRegistry()
    {
        aliveCells = new HashSet<Vector3Int>();
        newAliveCells = new HashSet<(int x, int y)>();

    }

}
