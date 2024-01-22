using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cell_Inventory : MonoBehaviour
{
    public static Cell_Inventory Instance { get; private set; }

    public List<InventoryObject> inventory = new(); // Consists of CellTypes
    private int cellSelectedIndex = 0;
    public static UnityEvent<InventoryObject> changeCellType;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        } else
        {
            Instance = this;
        }
        changeCellType = new UnityEvent<InventoryObject>();
    }

    private void OnDestroy()
    {
        changeCellType.RemoveAllListeners();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            cellSelectedIndex = (cellSelectedIndex + 1) % inventory.Count;
            changeCellType.Invoke(inventory[cellSelectedIndex]);
        }
    }
}

[Serializable]
public class InventoryObject
{
    public CellType cellType;
    public Sprite cellSprite;
    public int amount;
}
