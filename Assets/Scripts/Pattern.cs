using UnityEngine;

[CreateAssetMenu(menuName = "GoL/Pattern")]
public class Pattern : ScriptableObject
{

    //TODO: Refactor

    public Vector2Int[] cells;

    public Vector2Int GetCentre()
    {
        if (cells == null || cells.Length == 0)
        {
            return Vector2Int.zero;
        }

        Vector2Int min = Vector2Int.zero;
        Vector2Int max = Vector2Int.zero;

        for (int i = 0; i < cells.Length; i++)
        {

            Vector2Int cell = cells[i];
            min.x = Mathf.Min(cell.x, min.x);
            max.x = Mathf.Max(cell.x, max.x);
            min.y = Mathf.Min(cell.y, min.y);
            max.y = Mathf.Max(cell.y, max.y);
        }

        return (min + max) / 2;

    }

}


