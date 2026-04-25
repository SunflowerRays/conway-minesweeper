using System;
using System.Collections.Generic;
using UnityEngine;

public class PatternManager
{

    public HashSet<(int x, int y)> pattern { get; set; }

    public LiveRegistry liveRegistry;


    public PatternManager(LiveRegistry liveRegistry)
    {

        this.liveRegistry = liveRegistry;
        pattern = new HashSet<(int x, int y)>();
        
    }


    //public event Action onDetectionComplete;
    public void SetPattern()
    {
        if (pattern.Count > 0)
        {
            liveRegistry.newAliveCells.Clear();

            foreach (var cell in pattern)
            {
                liveRegistry.newAliveCells.Add(cell);
                liveRegistry.aliveCells.Add(new Vector3Int(cell.x, cell.y, 0));
            }

            //onDetectionComplete?.Invoke();

        }
    }


    public (int x, int y) GetCentre(HashSet<(int x, int y)> pattern)
    {
        if (pattern.Count == 0)
        {
            return (0, 0);
        }

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (var (x, y) in pattern)
        {
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }

        return ((minX + maxX) / 2, (minY + maxY) / 2);
    }

}


