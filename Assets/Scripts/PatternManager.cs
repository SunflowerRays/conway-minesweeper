using System.Collections.Generic;

public class PatternManager
{

    //TODO: Refactor

    public PatternManager()
    {

    }


    public HashSet<(int x, int y)> pattern { get; set; }

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


