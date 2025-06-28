using System;
using System.Collections;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    private int height = 15;
    private int width = 15;
    private float gridSpaceSize = 1.25f;

    [SerializeField] private GameObject gridCellPrefab;
    private GameObject[,] gameGrid;

    private void Start()
    {
        StartCoroutine(CreateGrid());
    }

    private IEnumerator CreateGrid()
    {
        gameGrid = new GameObject[height, width];

        if (gridCellPrefab == null)
        {
            Debug.LogError("Grid Cell Prefab = Null");
            yield return null;
        }

        // Create the grid
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create a new GridSpace
                gameGrid[x, y] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, y * gridSpaceSize), Quaternion.identity);
                gameGrid[x, y].GetComponent<GridCell>().SetPosition(x, y);
                gameGrid[x, y].transform.parent = transform;
                gameGrid[x, y].gameObject.name = string.Format("Grid Space ( X: {0}, Y: {1} )", x.ToString(), y.ToString());

                yield return new WaitForSeconds(0.025f);
            }
        }
    }

    // Get the grid position from wolrd position
    public Vector2Int GetGridPos(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / gridSpaceSize);
        int y = Mathf.FloorToInt(pos.y / gridSpaceSize);

        x = Mathf.Clamp(x, 0, width);
        y = Mathf.Clamp(x, 0, height);

        return new Vector2Int(x, y);
    }

    // Get the position of a grid position
    public Vector3 GetPosFromGird(Vector2Int pos)
    {
        float x = pos.x * gridSpaceSize;
        float y = pos.y * gridSpaceSize;

        return new Vector3(x, 0, y);
    }
}
