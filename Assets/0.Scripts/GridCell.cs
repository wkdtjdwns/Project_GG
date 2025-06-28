using System;
using System.Collections;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private int posX;
    private int posY;

    // Save a reference to gameObject that gets placed on this cell
    public GameObject objectInThisGridSpace;

    // Save if the grid space is occupied or not
    public bool isOccupied = false;

    // Set the position of this grid cell on the grid
    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    // Get the position of this grid space on the grid
    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posY);
    }

}
