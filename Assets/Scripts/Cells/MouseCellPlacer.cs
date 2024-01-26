using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseCellPlacer : MonoBehaviour
{
    InventoryObject inventoryObject;

    private Vector2 mousePos;
    SpriteRenderer selectorSprite;
    Sprite currSprite;
    public int disablePlacer = 0;

    [SerializeField] private LayerMask cellLayer;

    private GridBounds bounds;

    private void Start()
    {
        selectorSprite = GetComponent<SpriteRenderer>();
        Cell_Inventory.changeCellType.AddListener(ChangeCellType);
        ChangeCellType(Cell_Inventory.Instance.inventory[0]);
        bounds = GridManager.Main.bounds;
    }

    private void ChangeCellType(InventoryObject cell)
    {
        inventoryObject = cell;
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
        if (Convert.ToBoolean(disablePlacer) || x < bounds.min_x || x > bounds.max_x || y < bounds.min_y || y > bounds.max_y)
        {
            // Mouse is out of range, disable
            selectorSprite.sprite = null;
            return;
        } else
        {
            selectorSprite.sprite = currSprite;
        }
        x = Mathf.Clamp(x, bounds.min_x, bounds.max_x);
        y = Mathf.Clamp(y, bounds.min_y, bounds.max_y);
        transform.position = new Vector2(x, y);

        if (Input.GetMouseButton(0)) // Left click 
        {
            if (inventoryObject.amount == 0) return;
            RaycastHit2D rayHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, cellLayer);
            if (rayHit.collider == null || (rayHit.collider != null && rayHit.collider.gameObject.tag == "DeadCell")) // Dead cell
            {
                if (!GridManager.Main.cellGridA.CheckForDeadCell(x, y)) return;
                GridManager.Main.cellGridA.PlaceAliveCell(x, y, inventoryObject.cellType);
                inventoryObject.Decrement();
            } 
            else if (rayHit.collider != null && rayHit.collider.gameObject.tag == "AliveCell" &&
                rayHit.collider.GetComponent<Cell>().cellType != inventoryObject.cellType) {  // Cell exists but is of different type
                Cell_Inventory.Instance.Increment(rayHit.collider.GetComponent<Cell>().cellType);
                GridManager.Main.cellGridA.ChangePlacedCellType(rayHit.collider.gameObject, inventoryObject.cellType);
                inventoryObject.Decrement();
            } 

        }

        if (Input.GetMouseButton(1))
        {
            RaycastHit2D rayHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, cellLayer);
            if (rayHit.collider != null && rayHit.collider.gameObject.tag == "AliveCell")
            {
                if (!GridManager.Main.cellGridA.CheckForMutable(x, y)) return;
                GridManager.Main.cellGridA.PlaceDeadCell(x, y, rayHit.collider.gameObject);
                Cell_Inventory.Instance.currSoul.Increment();
            }
        }
    }
}
