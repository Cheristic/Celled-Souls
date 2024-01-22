using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public int level;
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
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }
}
