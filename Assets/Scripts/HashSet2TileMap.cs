using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
// editor-only code here

using TreeEditor;
using UnityEditor.EditorTools;
#endif

using UnityEngine;
using UnityEngine.Tilemaps;



public class HashSet2TileMap : MonoBehaviour
{

    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile bomb;
    [SerializeField] private Tile explosion;
    [SerializeField] private Tile[] nonMineTiles;
    [SerializeField] private Tile greyTile;
    [SerializeField] private GoL gol;
    [SerializeField] private Tilemap minefield;
    [SerializeField] private Tilemap greyfield;
    [SerializeField] private Tilemap currentState;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gol.generator.onGeneration += () => mapper(gol.liveRegistry.aliveCells, currentState);
        gol.mineDetector.onDetectionComplete += () => mapper(gol.mineDetector.cellsData, minefield);
        gol.mineHider.onCoverageComplete += () => mapper(gol.mineHider.topCells, greyfield);
    }

    /// <summary>
    /// Updates the specified tile map by setting tiles at the given coordinates to represent their current state.
    /// </summary>
    /// <remarks>The method clears the provided tile map before updating it. The type of tile set at each position
    /// depends on whether the tile map matches the current state or the greyfield. No action is taken if the tile map does
    /// not match either.</remarks>
    /// <param name="hashSet">A set of coordinate pairs representing the positions to update in the tile map.</param>
    /// <param name="tileMap">The tile map to update with the specified tile states. Must not be null.</param>
    public void mapper(HashSet<(int x, int y)> hashSet, Tilemap tileMap)
    {
        clear(tileMap);
        if (tileMap == currentState)
        {
            foreach (var cell in hashSet)
            {
                tileMap.SetTile(new Vector3Int(cell.x, cell.y, 0), aliveTile);
            }
        }
        else if (tileMap == greyfield)
        {
            foreach (var cell in hashSet)
            {
                tileMap.SetTile(new Vector3Int(cell.x, cell.y, 0), greyTile);
            }
        }
    }

    /// <summary>
    /// Updates the specified tilemap to reflect the state of each cell in the provided collection, displaying mines,
    /// numbers, or empty tiles as appropriate.
    /// </summary>
    /// <remarks>This method clears the tilemap before applying the new cell data. Each cell is mapped to a
    /// tile based on whether it contains a mine, a number, or is empty. The method does not validate the bounds of the
    /// cell coordinates relative to the tilemap.</remarks>
    /// <param name="cellsData">A list of cell data objects representing the state of each cell, including mine presence and adjacent mine
    /// counts. Cannot be null.</param>
    /// <param name="tileMap">The tilemap to update with the visual representation of the cell data. Cannot be null.</param>
    public void mapper(List<MineDetector.CellData> cellsData, Tilemap tileMap)
    {
        clear(tileMap);
        foreach (var CellData in cellsData)
        {
            if (CellData.mines == -1)
            {
                tileMap.SetTile(new Vector3Int(CellData.x, CellData.y, 0), bomb);
            }
            else if (CellData.mines > 0)
            {
                tileMap.SetTile(new Vector3Int(CellData.x, CellData.y, 0), nonMineTiles[CellData.mines]);
            }
            else
            {
                tileMap.SetTile(new Vector3Int(CellData.x, CellData.y, 0), nonMineTiles[0]);
            }
        }
    }



    /// <summary>
    /// Removes all tiles from the specified tilemap.
    /// </summary>
    /// <param name="tileMap">The tilemap to clear. Cannot be null.</param>
    public void clear(Tilemap tileMap)
    {
        tileMap.ClearAllTiles();
    }


    /// <summary>
    /// Removes all mines from the minefield, resetting it to an empty state.
    /// </summary>
    public void clearMinefield()
    {
        clear(minefield);
    }

    /// <summary>
    /// Clears all elements from the greyfield collection.
    /// </summary>
    public void clearGreyfield()
    {
        clear(greyfield);
    }


    /// <summary>
    /// Retrieves the coordinates of all non-empty tiles in the specified tilemap.
    /// </summary>
    /// <remarks>The returned set contains only the coordinates of tiles that have a tile assigned in the
    /// provided tilemap. Positions with no tile are excluded.</remarks>
    /// <param name="tileMap">The tilemap to search for non-empty tiles. Cannot be null.</param>
    /// <returns>A set of (x, y) coordinate pairs representing the positions of all tiles in the tilemap that are not empty. The
    /// set is empty if no non-empty tiles are found.</returns>
    public HashSet<(int x, int y)> readTileMap(Tilemap tileMap)
    {
        HashSet<(int x, int y)> temp = new HashSet<(int x, int y)>();

        foreach (var position in tileMap.cellBounds.allPositionsWithin)
        {
            if (tileMap.GetTile(position) != null)
            {
                temp.Add((position.x, position.y));
            }
        }

        return temp;
    }

    /// <summary>
    /// Gets the width and height of the specified tilemap in cells.
    /// </summary>
    /// <param name="tileMap">The tilemap from which to retrieve the dimensions. Cannot be null.</param>
    /// <returns>A tuple containing the width and height, in cells, of the tilemap.</returns>
    public (int width, int height) getTilemapDimensions(Tilemap tileMap)
    {
        BoundsInt bounds = tileMap.cellBounds;
        return (bounds.size.x, bounds.size.y);
    }

}
