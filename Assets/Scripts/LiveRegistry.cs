using System.Collections.Generic;
using UnityEngine;

public class LiveRegistry
{
    public HashSet<Vector3Int> aliveCells { get; set; }

    // Suggestion Moore Neighbourhood HashSet for evaluation by both minesweeper and GoL.
    // *Moore Neighbourhood of each alive cell.

    public LiveRegistry()
    {
        aliveCells = new HashSet<Vector3Int>();

    }

}
