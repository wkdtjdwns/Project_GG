using System;
using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    GameGrid gameGrid;
    [SerializeField] private LayerMask gridLayer;

    private void Start()
    {
        gameGrid = FindFirstObjectByType<GameGrid>();
    }

    private void Update()
    {
        // Interaction between mouse and grid cell
        GridCell cellMouseOver = IsMouseOverGridSpace();

        if (cellMouseOver != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                print(cellMouseOver.isOccupied);
                cellMouseOver.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
    }

    // Return the grid cell if mouse is over a grid cell
    private GridCell IsMouseOverGridSpace()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, gridLayer))
        {
            return hit.transform.GetComponent<GridCell>();
        }

        else
        {
            return null;
        }
    }
}
