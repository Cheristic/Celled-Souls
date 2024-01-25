using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cell_Inventory : MonoBehaviour
{
    public static Cell_Inventory Instance { get; private set; }
    public InventoryObject currSoul;
    public List<InventoryObject> inventory = new(); // Consists of CellTypes
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

        currSoul = inventory[0]; // Outline selected soul
        Color c = currSoul.backgroundOutline.effectColor;
        currSoul.backgroundOutline.effectColor = new Color(c.r, c.g, c.b, 255);
    }

    private void OnDestroy()
    {
        changeCellType.RemoveAllListeners();
    }

    public void Increment(CellType type)
    {
        foreach (var item in inventory)
        {
            if (item.cellType == type)
            {
                Debug.Log(item.cellType);
                item.Increment();
                return;
            }
        }
    }

    public void SelectSoul(CellType type)
    {
        foreach (var item in inventory)
        {
            if (item.cellType == type)
            {
                Color c = currSoul.backgroundOutline.effectColor;
                currSoul.backgroundOutline.effectColor = new Color(c.r, c.g, c.b, 0);
                currSoul = item;
                changeCellType.Invoke(item);
                return;
            }
        }
    }
}


