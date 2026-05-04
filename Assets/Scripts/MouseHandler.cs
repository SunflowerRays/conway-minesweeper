using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static MineDetector;

public class MouseHandler : MonoBehaviour
{

    [SerializeField] private Tilemap greyfield;
    [SerializeField] private GoL gol;
    [SerializeField] private Tilemap minefield;
    [SerializeField] private Tile explosion;
    [SerializeField] private Tile flag;
    [SerializeField] private Tile greyTile;


    private Tilemap currentState;

    public enum GameMode { PatternEdit, Simulating, Minesweeper, GameOver }
    private bool isGameOver;
    private GameMode mode = GameMode.PatternEdit;
    private (float time, int points, bool levelCleared, string playerName) score;

    void Start()
    {
        currentState = gol.currentState;

        gol.mineHider.onWin += () =>
        {
            //update points when scoring is updated.
            score.points = 500;
            score.levelCleared = true;
            mode = GameMode.GameOver;
        };


    }

    public void SetMode(GameMode newMode)
    {
        
        if (newMode == GameMode.PatternEdit)
        {
            if (gol.generator.isRunning) return;
            gol.patternManager.ClearPattern();
        }
        
        if (newMode == GameMode.Minesweeper)
        {
            score = (0, 0, false, null);
        }
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

            if (gol.patternManager.ToggleCell(x, y))
            {
                gol.currentState.SetTile(cellPosition, null);
            }
            else
            {
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

    //SetMode


    // https://learn.microsoft.com/en-us/dotnet/api/system.collections.queue?view=net-10.0
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

                var (_, _, mines) = gol.mineDetector.detector(x, y);


                if (mines == -1)
                {

                    greyfield.ClearAllTiles();
                    gol.mineHider.topCells.Clear();
                    minefield.SetTile(cellPosition, explosion);


                    score.points = 0;
                    score.levelCleared = false;
                    mode = GameMode.GameOver;
                }
                if (mines == 0)
                {
                    for (int dx = x-1; dx<=x+1; dx++)
                    {
                        for (int dy = y-1; dy <= y+1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;

                            cellPosition.x = dx;
                            cellPosition.y = dy;
                            greyfield.SetTile(cellPosition, null);


                        }
                    }

                }

            }
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
            Vector3Int cellPosition = greyfield.WorldToCell(worldPosition);

            if (gol.mineHider.topCells.Contains((cellPosition.x, cellPosition.y)))
            {
                if (greyfield.GetTile(cellPosition) == flag)
                {
                    greyfield.SetTile(cellPosition, greyTile);
                }
                else
                {
                    greyfield.SetTile(cellPosition, flag);
                }
            }

        }
    }

    private void playGameOver()
    {

        if (isGameOver) return;
        isGameOver = true;

        gol.textHandler.Stop();
        score.time = gol.textHandler.currentTime;
        if (score.time > 0.00f)

            // add player name input.


        {

            ScoreKeeper.LatestScore latestScore = new ScoreKeeper.LatestScore()
            {
                time = score.time,
                points = score.points,
                levelCleared = score.levelCleared,
                playerName = score.playerName
            };

            gol.scoreKeeper.saveScore(latestScore);

        }

    }
}
