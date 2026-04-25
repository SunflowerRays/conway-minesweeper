using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static MouseHandler;

public class MouseHandler : MonoBehaviour
{

    [SerializeField] private Tilemap greyfield;
    [SerializeField] private GoL gol;
    [SerializeField] private Tilemap minefield;
    [SerializeField] private Tile explosion;
    private Tilemap currentState;
    //private (int x, int y) gridDimensions;

    public enum GameMode { PatternEdit, Simulating, Minesweeper, GameOver }
    private GameMode mode = GameMode.PatternEdit;
    void Start()
    {
        //gridDimensions = (gol.grid.gridWidth, gol.grid.gridHeight);
        currentState = gol.currentState;
    }

    public void SetMode(GameMode newMode)
    {
        mode = newMode;
    }


    void Update()
    {

        if (mode == GameMode.PatternEdit) playPatternEditor();
        if (mode == GameMode.Simulating) playSimulator();
        if (mode == GameMode.Minesweeper) playMineSweeper();
        if (mode == GameMode.GameOver) playGameOver();
    }



    private void playPatternEditor()
    {

        if (gol.generator.isRunning) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
            Vector3Int cellPosition = currentState.WorldToCell(worldPosition);

            int x = cellPosition.x;
            int y = cellPosition.y;

            if (x < gol.grid.centre.x - gol.grid.gridWidth / 2 ||
                x > gol.grid.centre.x + gol.grid.gridWidth / 2 ||
                y < gol.grid.centre.y - gol.grid.gridHeight / 2 ||
                y > gol.grid.centre.y + gol.grid.gridHeight / 2) return;

            if (gol.patternManager.pattern.Contains((x, y)))
            {
                gol.patternManager.pattern.Remove((x, y));
                gol.liveRegistry.newAliveCells.Remove((x, y));
                gol.currentState.SetTile(cellPosition, null);
            }
            else
            {
                gol.patternManager.pattern.Add((x, y));
                gol.liveRegistry.newAliveCells.Add((x, y));
                gol.currentState.SetTile(cellPosition, gol.aliveTile);
            }
        }
    }


    private void playSimulator()
    {
        //recieve pattern

        //start simulation

        //pause simulation or pick generation from a sliding input

        //confirm generation and start playMineSweeper

    }


    private void playMineSweeper()
    {

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
            Vector3Int cellPosition = greyfield.WorldToCell(worldPosition);

            int x = cellPosition.x;
            int y = cellPosition.y;

            if (gol.mineHider.reveal(x, y))
            {
                greyfield.SetTile(cellPosition, null);

                if (gol.mineDetector.isMine(x, y))
                {
                    mode = GameMode.GameOver;
                    greyfield.ClearAllTiles();
                    gol.mineHider.topCells.Clear();
                    minefield.SetTile(cellPosition, explosion);
                }
            }
        }
    }

    private void playGameOver()
    {

    }

}
