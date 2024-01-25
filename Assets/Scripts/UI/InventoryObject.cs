using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

[Serializable]
public class InventoryObject : MonoBehaviour
{
    public CellType cellType;
    public Sprite cellSprite;
    public int amount;
    [SerializeField] TMP_Text text;
    [SerializeField] Image soul;
    public Outline backgroundOutline;
    [SerializeField] Material remaining;
    [SerializeField] Material empty;
    private Collider2D colliderReference;

    private void Start()
    {
        text.text = amount.ToString();
        soul.sprite = cellSprite;
        colliderReference = GetComponent<Collider2D>();
    }

    public void Increment()
    {
        amount++;
        text.text = amount.ToString();
        if (amount == 1) text.canvasRenderer.SetMaterial(remaining, 0);
    }

    public void Decrement()
    {
        amount--;
        text.text = amount.ToString();
        if (amount == 0) text.canvasRenderer.SetMaterial(empty, 0);
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] rayHit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity);
            foreach (var hit in rayHit)
            {
                if (hit.collider.Equals(colliderReference))
                {
                    Color c = backgroundOutline.effectColor;
                    backgroundOutline.effectColor = new Color(c.r, c.g, c.b, 255);
                    Cell_Inventory.Instance.SelectSoul(cellType);
                }
            }
        }
    }
}
