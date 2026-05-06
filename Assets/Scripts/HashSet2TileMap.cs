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

    /// <summary>
    /// Read HashSet write to tilemap.
    /// </summary>
    /// <param name="hashSet"></param>
    /// <param name="tilemap"></param>
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

    public void clear(Tilemap tileMap)
    {
        tileMap.ClearAllTiles();
    }


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

    public (int width, int height) getTilemapDimensions(Tilemap tileMap)
    {
        // add comments
        BoundsInt bounds = tileMap.cellBounds;
        return (bounds.size.x, bounds.size.y);
    }



    void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gol.generator.onGeneration += () => mapper(gol.liveRegistry.aliveCells, currentState);
        gol.mineDetector.onDetectionComplete += () => mapper(gol.mineDetector.cellsData, minefield);
        gol.mineHider.onCoverageComplete += () => mapper(gol.mineHider.topCells, greyfield);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
