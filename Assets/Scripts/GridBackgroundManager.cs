using UnityEngine;

public class GridBackgroundManager : MonoBehaviour
{
    [SerializeField] private GoL gol;
    [SerializeField] private SpriteRenderer background;

    void Start()
    {
        background.transform.localScale = new Vector3(gol.grid.gridWidth + 1, gol.grid.gridHeight + 1, 1);
    }
}