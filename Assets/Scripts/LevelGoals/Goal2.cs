using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal2 : MonoBehaviour
{
    [SerializeField] GameObject youWinClick;
    void Start()
    {
        GridManager.newGeneration.AddListener(OnGeneration);
        youWinClick.GetComponentInChildren<Button>().onClick.AddListener(BackToMainMenu);
    }

    private void OnGeneration()
    {
        if (GridManager.Main.cellGridA.grid[29, 1].cellType != CellType.Dead ||
            GridManager.Main.cellGridA.grid[29, 10].cellType != CellType.Dead)
        {
           return;
        }
        // All loners were killed
        youWinClick.SetActive(true);
        ProgressTracker.Main.LevelWin(2);
    }

    private void BackToMainMenu()
    {
        LevelLoader.Loader.LoadScene(0);
    }
}
