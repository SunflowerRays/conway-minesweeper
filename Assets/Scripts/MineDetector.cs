using System;
using System.Collections.Generic;
public class MineDetector
{
    private Grid grid;
    private LiveRegistry liveRegistry;

    /// <summary>
    /// Public struct that contains the number of mines in a cell, and 
    /// The cell's coordinates.
    /// </summary>
    public struct CellData
    {
        public int x;
        public int y;
        public int mines;
    }

    public List<CellData> cellsData;

    //To keep this class a pure C# class with no Unity elements, so it remains testable
    public event Action onDetectionComplete = delegate { };

    public MineDetector(Grid grid, LiveRegistry liveRegistry)
    {
        this.grid = grid;
        this.liveRegistry = liveRegistry;
        cellsData = new List<CellData>();
    }

    /// <summary>
    /// Runs the detector method over all of the cells in the grid.
    /// </summary>
    public void detectorOverAllCells()
    {

        cellsData.Clear();

        foreach (var (i, j) in grid.GetAllCells())
        {
            CellData cell = detector(i, j);
            cellsData.Add(cell);
        }

        onDetectionComplete.Invoke();


    }

    /// <summary>
    /// Evaluates the cell at coordinates (i,j) for
    /// The presence of a mines,
    /// And if there is no mine on the tile,
    /// Then for the presence of mines around the tile.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns>a tuple with coordinates and an int indicating the presence or absence of a mine(s)</returns>
    public CellData detector(int i, int j)
    {
        CellData cell;
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
        cell = new CellData() { x = i, y = j, mines = mines };
        return cell;
    }
}