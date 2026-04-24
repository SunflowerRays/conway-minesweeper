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
        detector();
    }


    public void detector()
    {
        cellsData.Clear();

        for (int i = (grid.centre.x - (grid.gridWidth / 2)); i <= (grid.centre.x + (grid.gridWidth / 2)); ++i)
        {

            for (int j = (grid.centre.y - (grid.gridHeight / 2)); j <= (grid.centre.y + (grid.gridHeight / 2)); ++j)
            {

                int mines = 0;

                if (liveRegistry.newAliveCells.Contains((i, j)))
                {
                    mines = -1;
                }
                else
                {

                    for (int x = -1; x <= 1; x++)
                    {

                        for (int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0)
                            {

                                continue;
                            }

                            if (liveRegistry.newAliveCells.Contains((i + x, j + y)))
                            {
                                mines++;
                            }


                        }

                    }

                }

                // add cell to cellsData
                CellData cell = new CellData() { x = i, y = j, mines = mines };

                cellsData.Add(cell);

            }

        }

        onDetectionComplete?.Invoke();

    }

    //Called by MouseHandler.
    public bool isMine(int x, int y)
    {
        return cellsData.Exists(cell => cell.x == x && cell.y == y && cell.mines == -1);
    }

}