using System.Collections.Generic;
using System;

public class PatternManager
{

    public HashSet<(int x, int y)> pattern { get; private set; }

    public LiveRegistry liveRegistry;
    public event Action onAddCell;
    public event Action onSubtractCell;

    public PatternManager(LiveRegistry liveRegistry)
    {

        this.liveRegistry = liveRegistry;
        pattern = new HashSet<(int x, int y)>();

    }


    public bool ToggleCell(int x, int y)
    {
        if (pattern.Contains((x, y)))
        {
            pattern.Remove((x, y));
            liveRegistry.newAliveCells.Remove((x, y));
            onSubtractCell?.Invoke();
            return true;
        }
        else
        {
            pattern.Add((x, y));
            liveRegistry.newAliveCells.Add((x, y));
            onAddCell?.Invoke();
            return false;
        }
    }

    public void ClearPattern()
    {
        pattern.Clear();
    }


    //public event Action onDetectionComplete;
    public void Pattern2AliveCells()
    {
        if (pattern.Count > 0)
        {
            liveRegistry.newAliveCells.Clear();

            foreach (var cell in pattern)
            {
                liveRegistry.newAliveCells.Add(cell);
            
            
            }

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


