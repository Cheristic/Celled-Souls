using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCellPlacer : MonoBehaviour
{
    CellType cellTypeSelected;

    private Vector2 mousePos;
    SpriteRenderer selectorSprite;

    [SerializeField] private LayerMask cellLayer;

    private int max_x, max_y;

    private void Start()
    {
        selectorSprite = GetComponent<SpriteRenderer>();
        Cell_Inventory.changeCellType.AddListener(ChangeCellType);
        ChangeCellType(Cell_Inventory.Instance.inventory[0]);
        max_x = LevelManager.Main.rows / 2;
        max_y = LevelManager.Main.columns / 2;
    }

    private void ChangeCellType(InventoryObject cell)
    {
        cellTypeSelected = cell.cellType;
        selectorSprite.sprite = cell.cellSprite;
        Color temp = selectorSprite.material.color;
        temp.a = .4f;
        selectorSprite.material.color = temp;
    }
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // If within screen bounds
        int x = Mathf.Clamp(Mathf.RoundToInt(mousePos.x), -max_x, max_x-1);
        int y = Mathf.Clamp(Mathf.RoundToInt(mousePos.y), -max_y, max_y-1);
        transform.position = new Vector2(x, y);
    
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            RaycastHit2D rayHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, cellLayer);
            if (rayHit.collider == null || (rayHit.collider != null && rayHit.collider.gameObject.tag == "DeadCell")) // Dead cell
            {
                LevelManager.Main.cellGridA.PlaceAliveCell(x, y, cellTypeSelected);
            } 
            else if (rayHit.collider != null && rayHit.collider.gameObject.tag == "AliveCell" &&
                rayHit.collider.GetComponent<Cell>().cellType != cellTypeSelected) {  // Cell exists but is of different type
                LevelManager.Main.cellGridA.ChangePlacedCellType(rayHit.collider.gameObject, cellTypeSelected);
            } 

        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D rayHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, cellLayer);
            if (rayHit.collider != null && rayHit.collider.gameObject.tag == "AliveCell")
            {
                LevelManager.Main.cellGridA.PlaceDeadCell(x, y, rayHit.collider.gameObject);
            }
        }
    }
}
