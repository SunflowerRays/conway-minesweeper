using UnityEngine;
using UnityEngine.Tilemaps;

public class Generator
{

    private LiveRegistry liveRegistry;
    private Tilemap currentState;
    private Tile aliveTile;
    private Tile deadTile;

    public Generator(LiveRegistry liveRegistry, Tilemap currentState, Tile aliveTile, Tile deadTile)
    {
        this.liveRegistry = liveRegistry;
        this.currentState = currentState;
        this.aliveTile = aliveTile;
        this.deadTile = deadTile;
    }

    public bool IsAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == aliveTile;

    }

}
