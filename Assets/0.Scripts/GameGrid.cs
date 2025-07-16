using System;
using System.Collections;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    private int height = 15;
    private int width = 15;
    private float gridSpaceSize = 1;
    //private float gridSpaceSize = 1.25f;

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
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create a new GridSpace
                gameGrid[x, z] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, 0, z * gridSpaceSize), Quaternion.identity);
                gameGrid[x, z].GetComponent<GridCell>().SetPosition(x, z);
                gameGrid[x, z].transform.parent = transform;
                gameGrid[x, z].gameObject.name = string.Format("Grid Space ( X: {0}, Z: {1} )", x.ToString(), z.ToString());

                yield return new WaitForSeconds(0.025f);
            }
        }
    }

    // Get the grid position from wolrd position
    public Vector3Int GetGridPos(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / gridSpaceSize);
        int z = Mathf.FloorToInt(pos.z / gridSpaceSize);

        x = Mathf.Clamp(x, 0, width);
        z = Mathf.Clamp(x, 0, height);

        return new Vector3Int(x, 0, z);
    }

    // Get the position of a grid position
    public Vector3 GetPosFromGird(Vector3Int pos)
    {
        float x = pos.x * gridSpaceSize;
        float z = pos.z * gridSpaceSize;

        return new Vector3(x, 0, z);
    }
}
