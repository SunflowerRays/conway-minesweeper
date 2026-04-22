using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using TreeEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Tilemaps;


public class HashSet2TileMap : MonoBehaviour
{

    private HashSet<(int x, int y)> hashSet { get; set; }
    private Tilemap tileMap;
    [SerializeField] private Tile one;
    [SerializeField] private Tile emptytile;
    [SerializeField] private Tile bomb;
    [SerializeField] private Tile [] nonMineTiles;
    


    [SerializeField] private GoL gol;
    [SerializeField] private Tilemap minefield;
    /// <summary>
    /// Read HashSet write to tilemap.
    /// </summary>
    /// <param name="hashSet"></param>
    /// <param name="tilemap"></param>
    public void mapper(HashSet<(int x, int y)> hashSet, Tilemap tileMap)
    {

        clear(tileMap);

        //assumption. hashSet and tileMap have the smae dimensions.
        foreach (var cell in hashSet)
        {
            tileMap.SetTile(new Vector3Int(cell.x, cell.y, 0), one);
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


    //TODO: Read tilemap method.


    void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gol.mineDetector.onDetectionComplete += () => mapper(gol.mineDetector.cellsData, minefield);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
