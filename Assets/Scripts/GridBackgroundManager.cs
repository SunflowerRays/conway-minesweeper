using UnityEngine;

public class GridBackgroundManager : MonoBehaviour
{
    [SerializeField] private GoL gol;
    [SerializeField] private SpriteRenderer background;
    

    [SerializeField] private float verticalOffset = 0f;

    void Start()
    {
        float centreX = gol.grid.centre.x + (gol.grid.gridWidth / 2f) - 0.5f;
        float centreY = gol.grid.centre.y + (gol.grid.gridHeight / 2f) - 0.5f;

        Camera.main.transform.position = new Vector3(-0.5f, -0.5f + verticalOffset, Camera.main.transform.position.z);
        Camera.main.orthographicSize = gol.grid.gridHeight / 2f + 1f;


    }

    // Old start
    //void Start()
    //{



    //    float width = gol.grid.gridWidth;
    //    float height = gol.grid.gridHeight;
    //    background.transform.localPosition = new Vector3(0, 0, 0);
    //    background.transform.localScale = new Vector3(width, height, 1);
    //}
}