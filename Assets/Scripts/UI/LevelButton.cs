using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    private int x, y;
    public int level;

    private void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            transform.parent.GetComponent<Rigidbody2D>().AddForce(new Vector2(100, 100));
        }
        RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, gameObject.layer);
        if (rayHit.collider != null) // Hovering over button
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene(level, LoadSceneMode.Single);
            }
        }
    }
    private void OnMouseEnter()
    {
        // MAKE
    }
    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseDown()
    {
        
    }
}
