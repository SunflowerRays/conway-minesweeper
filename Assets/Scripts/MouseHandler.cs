using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MouseHandler : MonoBehaviour
{

    [SerializeField] private Tilemap greyfield;
    [SerializeField] private GoL gol;
    [SerializeField] private Tilemap minefield;
    [SerializeField] private Tile explosion;
    public bool gameOver;
    void Start()
    {

    }

    void Update()
    {
        if (gameOver == false)
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
                        gameOver = true;
                        greyfield.ClearAllTiles();
                        gol.mineHider.topCells.Clear();
                        minefield.SetTile(cellPosition, explosion);
                    }
                }
            }
        }

    }
}
