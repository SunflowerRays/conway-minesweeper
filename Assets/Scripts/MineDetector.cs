using System;
using System.Collections.Generic;
public class MineDetector
{
    private Grid grid;
    private LiveRegistry liveRegistry;
    public struct CellData
    {
        public int x;
        public int y;
        public int mines;
    }

    public List<CellData> cellsData;

    //To keep this class a pure C# class with no Unity elements, so it remains testable
    public event Action onDetectionComplete;

    public MineDetector(Grid grid, LiveRegistry liveRegistry)
    {
        this.grid = grid;
        this.liveRegistry = liveRegistry;
        cellsData = new List<CellData>();

        detectorOverAllCells();


    }


    public void detectorOverAllCells()
    {

        cellsData.Clear();

        foreach (var (i, j) in grid.GetAllCells())
        {
            var (x, y, mines) = detector(i, j);
            CellData cell = new CellData() { x = x, y = y, mines = mines };
            cellsData.Add(cell);
        }

        onDetectionComplete?.Invoke();


    }


    public (int x, int y, int mines) detector(int i, int j)
    {

        int mines = 0;

        if (liveRegistry.aliveCells.Contains((i, j)))
        {
            mines = -1;
        }
        else
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (liveRegistry.aliveCells.Contains((i + x, j + y)))
                    {
                        mines++;
                    }
                }
            }

        }
        return (i, j, mines);
    }


}