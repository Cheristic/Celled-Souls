using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCellPlacer : MonoBehaviour
{
    CellType cellTypeSelected;

    private Vector2 mousePos;
    SpriteRenderer selectorSprite;

    [SerializeField] private LayerMask cellLayer;

    private void Start()
    {
        selectorSprite = GetComponent<SpriteRenderer>();
        Cell_Inventory.changeCellType.AddListener(ChangeCellType);
        ChangeCellType(Cell_Inventory.Instance.inventory[0]);
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
        transform.position = new Vector2(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y));
    
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Vector2 mouseRay = Camera.main.ScreenToWorldPoint(transform.position);
            RaycastHit2D rayHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, cellLayer);
            if (rayHit.collider == null ) // No cell
            {
                GameObject obj = LevelAssembler.Main.GetCellObjectFromPool();
                obj.GetComponent<Cell>().ChangeType(cellTypeSelected);
                obj.transform.position = transform.position;
            } 
            else if (rayHit.collider.gameObject.tag == "Cell" &&
                rayHit.collider.GetComponent<Cell>().cellType != cellTypeSelected) {  // Cell is of different type
                rayHit.collider.GetComponent<Cell>().ChangeType(cellTypeSelected);
            } 
            else // Cell of same type already exists
            {
                Debug.Log("exists");
            }
            
        }
    }
}
