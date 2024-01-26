using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal3Creature : MonoBehaviour
{
    private int x, y;
    [SerializeField] Collider2D colliderReference;
    private GameObject youWinClick;

    public void Init(GameObject win)
    {
        youWinClick = win;
        x = Mathf.RoundToInt(transform.position.x);
        y = Mathf.RoundToInt(transform.position.y);
        GridManager.newGeneration.AddListener(CheckMovement);
        GridManager.reset.AddListener(CheckMovement);
    }

    private void CheckMovement()
    {
        if (Mathf.RoundToInt(transform.position.x) != x || // Button has moved
            Mathf.RoundToInt(transform.position.y) != y)
        {
            Vector3 pivot = GridManager.Main.cellGridA.pivot;
            Cell par = GridManager.Main.cellGridA.grid[x - (int)pivot.x, y - (int)pivot.y];
            transform.SetParent(par.gameObject.transform);
        }
        transform.position = new Vector2(x, y);

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
                    youWinClick.SetActive(true);
                    ProgressTracker.Main.LevelWin(3);
                }
            }
        }

    }
}
