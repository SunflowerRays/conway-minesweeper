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
    public Grid((int x, int y) centre, int gridWidth, int gridHeight)
    {
        this.centre = centre;
        this.gridHeight = gridHeight;
        this.gridWidth = gridWidth;
    }

    /// <summary>
    /// Returns an enumerable collection of all cell coordinates within the grid, relative to the current center
    /// position.
    /// </summary>
    /// <remarks>The enumeration covers all cells in the rectangular area defined by the grid's width and
    /// height, centered at the specified center point. The order of the returned coordinates is row by row, starting
    /// from the top-left corner.</remarks>
    /// <returns>An enumerable sequence of tuples, where each tuple contains the x and y coordinates of a cell within the grid.</returns>
    public IEnumerable<(int x, int y)> GetAllCells()
    {
        for (int i = centre.x - gridWidth / 2; i < centre.x + gridWidth / 2; ++i)
        {
            for (int j = centre.y - gridHeight / 2; j < centre.y + gridHeight / 2; ++j)
            {
                yield return (i, j);
            }
        }
    }

    /// <summary>
    /// Evaluates whether coordinates are inside the grid.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>boolean</returns>
    public bool IsInsideBounds(int x, int y)
    {
        if (x < centre.x - gridWidth / 2 ||
            x >= centre.x + gridWidth / 2 ||
            y < centre.y - gridHeight / 2 ||
            y >= centre.y + gridHeight / 2) return false;

        return true;
    }



}
