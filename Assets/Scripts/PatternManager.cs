
using System.Collections.Generic;
using System;

public class PatternManager
{

    //public HashSet<(int x, int y)> pattern { get; private set; }

    public LiveRegistry liveRegistry;
    public event Action onAddCell;
    public event Action onSubtractCell;

    public PatternManager(LiveRegistry liveRegistry)
    {
        this.liveRegistry = liveRegistry;
    }
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


