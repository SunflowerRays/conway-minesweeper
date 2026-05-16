using UnityEngine;

public class GridBackgroundManager : MonoBehaviour
{
    // GoL class
    [SerializeField] private GoL gol;
    // Sprite Renderer used to set the background with
    // A material and a shader graph.
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private float verticalOffset = 0f;

    /// <summary>
    /// Runs when the program starts.
    /// Camera is centred on the grid
    /// </summary>
    void Start()
    {
        Camera.main.transform.position = new Vector3(-0.5f, -0.5f + verticalOffset, Camera.main.transform.position.z);
        Camera.main.orthographicSize = gol.grid.gridHeight / 2f + 1f;
    }
}