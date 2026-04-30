using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.EventSystems.EventTrigger;

public class Border : MonoBehaviour
{


    [SerializeField] private GoL gol;
    [SerializeField] private Tilemap borderTileMap;
    [SerializeField] private Tile boarderTile;

    private int gH;
    private int gW;
    private (int x, int y) centre;


    private void Awake()
    {

    }

    void Start()
    {

        gH = gol.grid.gridHeight;
        gW = gol.grid.gridWidth;
        centre = gol.grid.centre;

        int xOfC = centre.x;
        int yOfC = centre.y;

        for (int i = xOfC - (gW / 2); i <= xOfC + (gW / 2); i++)
        {
            borderTileMap.SetTile(new Vector3Int(i, yOfC + (gH / 2), 0), boarderTile);
            borderTileMap.SetTile(new Vector3Int(i, yOfC - (gH / 2), 0), boarderTile);
        }

        for (int j = yOfC - (gH / 2); j <= yOfC + (gH / 2); j++)
        {
            borderTileMap.SetTile(new Vector3Int(yOfC + (gH / 2), j, 0), boarderTile);
            borderTileMap.SetTile(new Vector3Int(yOfC - (gH / 2), j, 0), boarderTile);
        }


    }


}
