using UnityEngine;

public class GridBackgroundManager : MonoBehaviour
{
    [SerializeField] private GoL gol;
    [SerializeField] private SpriteRenderer background;

    void Start()
    {
        float width = gol.grid.gridWidth;
        float height = gol.grid.gridHeight;
        background.transform.localPosition = new Vector3(0, 0, 0);
        background.transform.localScale = new Vector3(width, height, 1);
    }
}