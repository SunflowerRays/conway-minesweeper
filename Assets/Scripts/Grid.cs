using System.Collections.Generic;

public class Grid
{

    public int gridHeight { get; private set; }
    public int gridWidth { get; private set; }
    public (int x, int y) centre { get; private set; }




    /// <summary>
    /// Initializes a new instance of the Grid class with the specified center position, height, and width.
    /// </summary>
    /// <param name="centre">The coordinates representing the center of the grid.</param>
    /// <param name="gridHeight">The number of rows in the grid. Must be a positive integer.</param>
    /// <param name="gridWidth">The number of columns in the grid. Must be a positive integer.</param>
    public Grid((int x, int y) centre, int gridHeight, int gridWidth)
    {
        //Consider Arithmetic Overflow Check Operators    

        this.centre = centre;
        this.gridHeight = gridHeight;
        this.gridWidth = gridWidth;


    }

    public IEnumerable<(int x, int y)> GetAllCells()
    {
        for (int i = (centre.x - (gridWidth / 2)); i <= (centre.x + (gridWidth / 2)); ++i)
        {
            for (int j = (centre.y - (gridHeight / 2)); j <= (centre.y + (gridHeight / 2)); ++j)
            {
                yield return (i, j);
            }
        }
    }


}
