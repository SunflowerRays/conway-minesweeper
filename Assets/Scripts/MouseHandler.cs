using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MouseHandler : MonoBehaviour
{

    //GoL class
    [SerializeField] private GoL gol;
    //Local UI elements
    [SerializeField] private Tilemap greyfield;
    [SerializeField] private Tilemap minefield;
    [SerializeField] private Tile explosion;
    [SerializeField] private Tile flag;
    [SerializeField] private Tile greyTile;
    [SerializeField] public TMPro.TMP_InputField playerNameInput;
    [SerializeField] public UnityEngine.UI.Button SubmitScore;
    [SerializeField] public UnityEngine.UI.Toggle CascadeRevealSwitch;

    //Games state
    public enum GameMode { PatternEdit, Simulating, Minesweeper, GameOver }
    public GameMode mode = GameMode.PatternEdit;
    public bool isGameOver;
    private (float time, int points, bool levelCleared, string playerName) score;

    void Start()
    {
        //Enable and Disable UI elements
        playerNameInput.gameObject.SetActive(false);
        SubmitScore.gameObject.SetActive(false);
        CascadeRevealSwitch.gameObject.SetActive(false);

        //Set local UI values
        playerNameInput.text = "Player_Name_Here";
        CascadeRevealSwitch.isOn = true;

        //Event Listener
        gol.mineHider.onWin += () =>
        {
            score.points = gol.liveRegistry.population;
            score.levelCleared = true;
            mode = GameMode.GameOver;
        };

    }

    //Set game mode
    public void SetMode(GameMode newMode)
    {
        if (newMode == GameMode.PatternEdit)
        {
            if (gol.isGeneratorRunning) return;
            gol.patternManager.ClearPattern();
            CascadeRevealSwitch.gameObject.SetActive(false);
        }
        if (newMode == GameMode.Minesweeper)
        {
            gol.mineHider.coverMines(gol.grid);
            score = (0, 0, false, null);
            gol.mineDetector.detectorOverAllCells();
            CascadeRevealSwitch.gameObject.SetActive(true);
        }
        mode = newMode;
    }


    void Update()
    {
        if (mode == GameMode.PatternEdit) playPatternEditor();
        // Simulating is handled by the Simulate method in GoL, which
        // Calls the UpdateState method in Generator.
        if (mode == GameMode.Minesweeper) playMineSweeper();
        if (mode == GameMode.GameOver) playGameOver();
    }


    /// <summary>
    /// Gets the Tilemap cell that corresponds to the current mouse position in world space.
    /// This method is common to both GoL and Minesweeper.
    /// </summary>
    /// <remarks>Relies on UnityEngine.InputSystem.Mouse.current and Camera.main.ScreenToWorldPoint with
    /// Camera.main.nearClipPlane; a valid main camera and an active input system are required.</remarks>
    /// <param name="tilemap">Tilemap used to convert the computed world position to cell coordinates.</param>
    /// <returns>A Vector3Int representing the cell coordinates on the provided Tilemap corresponding to the mouse position.</returns>
    private Vector3Int getCellPosition(Tilemap tilemap)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        return tilemap.WorldToCell(worldPosition);
    }

    /// <summary>
    /// Handle left mouse clicks inside the game grid to toggle the corresponding cell in the pattern manager and update
    /// the tilemap to reflect alive or dead state.
    /// </summary>
    /// <remarks>Ignores clicks outside gol.grid bounds. Determines the clicked cell with
    /// getCellPosition(gol.currentState), uses Mouse.current.leftButton.wasPressedThisFrame, invokes
    /// gol.patternManager.ToggleCell(x, y), and sets or clears the tile on gol.currentState accordingly.</remarks>
    private void playPatternEditor()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //Get Cell Position
            Vector3Int cellPosition = getCellPosition(gol.currentState);
            int x = cellPosition.x;
            int y = cellPosition.y;

            //Exclude mouse clicks outside the game grid.
            if (!gol.grid.IsInsideBounds(x, y)) return;

            //ToggleCells:
            //If cell is alive, returns true, then sets cell to dead, and
            //If cell is dead, returns false, then sets cell to alive.
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


    /// <summary>
    /// Handle player input to reveal or flag cells in the Minesweeper grid.
    /// </summary>
    /// <remarks>Left click reveals the cell under the pointer; if the cell is uncovered it is removed from
    /// the grey tilemap and enqueued for cascade processing. Cascade reveal uses a breadth-first traversal: when a
    /// revealed cell reports zero neighboring mines and CascadeRevealSwitch.isOn, adjacent covered cells are revealed
    /// and enqueued. Revealing a mined cell clears grey tiles, places an explosion tile, sets levelCleared to false,
    /// and transitions the game to GameOver. Right click toggles a flag tile on covered cells tracked in
    /// mineHider.topCells. Cell positions are obtained via getCellPosition(greyfield).</remarks>
    private void playMineSweeper()
    {

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //Get Cell Position
            Vector3Int cellPosition = getCellPosition(greyfield);
            int x = cellPosition.x;
            int y = cellPosition.y;
            //Queue data structure for handling cascade reveal operation.
            Queue<(int x, int y)> cellsToCheck = new Queue<(int x, int y)>();
            //reveal:
            //If cell is covered return true, and
            //If cell is uncovered return false.
            if (gol.mineHider.reveal(x, y))
            {
                //clicked cell is added to the queue.
                cellsToCheck.Enqueue((x, y));
                //clicked tile is deleted from tilemap
                greyfield.SetTile(cellPosition, null);
                while (cellsToCheck.Count > 0)
                {
                    //position taken from queued values
                    var (bx, by) = cellsToCheck.Dequeue();
                    int mines = gol.mineDetector.detector(bx, by).mines;
                    // mines == -1, the tile is mined.
                    if (mines == -1)
                    {
                        greyfield.ClearAllTiles();
                        gol.mineHider.topCells.Clear();
                        minefield.SetTile(cellPosition, explosion);
                        score.levelCleared = false;
                        mode = GameMode.GameOver;
                    }
                    // mines == 0, the tile is not mined and has no mined neighbours.
                    // Cascade reveal can be enabled.
                    if (mines == 0 && CascadeRevealSwitch.isOn)
                    {
                        for (int cx = -1; cx <= 1; cx++)
                        {
                            for (int cy = -1; cy <= 1; cy++)
                            {
                                if (cx == 0 && cy == 0) continue;
                                int dx = bx + cx;
                                int dy = by + cy;
                                if (gol.mineHider.reveal(dx, dy))
                                {
                                    greyfield.SetTile(new Vector3Int(dx, dy, 0), null);
                                    cellsToCheck.Enqueue((dx, dy));
                                }
                            }
                        }
                    }

                }
            }
        }
        //Red flag placed where a right click happens.
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector3Int cellPosition = getCellPosition(greyfield);
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

    /// <summary>
    /// Marks the game as over, records the elapsed time, clears the playfield, and enables player name input and score
    /// submission.
    /// </summary>
    /// <remarks>If the game is already over, the method returns immediately. It sets isGameOver and
    /// gol.textHandler.isMinesweeperRunning, assigns score.time from gol.textHandler.currentTime, clears greyfield
    /// tiles, and activates the playerNameInput and SubmitScore UI elements.</remarks>
    private void playGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        gol.textHandler.isMinesweeperRunning = false;
        score.time = gol.textHandler.currentTime;
        greyfield.ClearAllTiles();
        if (score.levelCleared)
        {
            playerNameInput.gameObject.SetActive(true);
            SubmitScore.gameObject.SetActive(true);
        }
        else
        {
            gol.textHandler.showHighScores();
        }


    }

    /// <summary>
    /// Submits the current player's score, persists it if not in demo mode, and displays the high-score list.
    /// </summary>
    /// <remarks>Reads the player name from playerNameInput and assigns it to score.playerName, creates a
    /// ScoreKeeper.LatestScore from the current score data, hides the name input and submit button, logs the confirmed
    /// name, conditionally saves the score via gol.scoreKeeper.saveScore when gol.DemoSwitch.isOn is false, and invokes
    /// gol.textHandler.showHighScores().</remarks>
    public void OnSubmitScorePressed()
    {
        score.playerName = playerNameInput.text;

        //LatestScore struct is populated
        ScoreKeeper.LatestScore latestScore = new ScoreKeeper.LatestScore()
        {
            time = score.time,
            points = score.points,
            levelCleared = score.levelCleared,
            playerName = score.playerName
        };

        playerNameInput.gameObject.SetActive(false);
        SubmitScore.gameObject.SetActive(false);

        if (!gol.DemoSwitch.isOn)
        {
            gol.scoreKeeper.saveScore(latestScore);
        }

        gol.textHandler.showHighScores();
    }



}
