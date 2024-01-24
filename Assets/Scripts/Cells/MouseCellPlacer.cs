using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseCellPlacer : MonoBehaviour
{
    CellType cellTypeSelected;

    private Vector2 mousePos;
    SpriteRenderer selectorSprite;
    Sprite currSprite;
    public int disablePlacer = 0;

    [SerializeField] private LayerMask cellLayer;

    public int max_x, max_y;

    private void Start()
    {
        selectorSprite = GetComponent<SpriteRenderer>();
        Cell_Inventory.changeCellType.AddListener(ChangeCellType);
        ChangeCellType(Cell_Inventory.Instance.inventory[0]);
        max_x = Mathf.CeilToInt(GridManager.Main.rows / 2);
        max_y = Mathf.CeilToInt(GridManager.Main.columns / 2);
    }

    private void ChangeCellType(InventoryObject cell)
    {
        cellTypeSelected = cell.cellType;
        currSprite = cell.cellSprite;
        Color temp = selectorSprite.material.color;
        temp.a = .4f;
        selectorSprite.material.color = temp;
    }
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // If within screen bounds
        int x = Mathf.RoundToInt(mousePos.x);
        int y = Mathf.RoundToInt(mousePos.y);
        if (Convert.ToBoolean(disablePlacer) || x < -max_x || x > max_x-1 || y < -max_y || y > max_y-1)
        {
            // Mouse is out of range, disable
            selectorSprite.sprite = null;
            return;
        } else
        {
            selectorSprite.sprite = currSprite;
        }
        x = Mathf.Clamp(x, -max_x, max_x-1);
        y = Mathf.Clamp(y, -max_y, max_y-1);
        transform.position = new Vector2(x, y);

        if (Input.GetMouseButton(0)) // Left click 
        {
            RaycastHit2D rayHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, cellLayer);
            if (rayHit.collider == null || (rayHit.collider != null && rayHit.collider.gameObject.tag == "DeadCell")) // Dead cell
            {
                GridManager.Main.cellGridA.PlaceAliveCell(x, y, cellTypeSelected);
            } 
            else if (rayHit.collider != null && rayHit.collider.gameObject.tag == "AliveCell" &&
                rayHit.collider.GetComponent<Cell>().cellType != cellTypeSelected) {  // Cell exists but is of different type
                GridManager.Main.cellGridA.ChangePlacedCellType(rayHit.collider.gameObject, cellTypeSelected);
            } 

        }

        if (Input.GetMouseButton(1))
        {
            RaycastHit2D rayHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, cellLayer);
            if (rayHit.collider != null && rayHit.collider.gameObject.tag == "AliveCell")
            {
                GridManager.Main.cellGridA.PlaceDeadCell(x, y, rayHit.collider.gameObject);
            }
        }
    }
}
