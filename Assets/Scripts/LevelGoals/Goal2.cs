using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        foreach (Cell cell in GridManager.Main.cellGridA.grid)
        {
            if (cell.cellType != CellType.Dead) return;
        }
        // All cells are dead
        youWinClick.SetActive(true);
        ProgressTracker.Main.LevelWin(2);
    }

    private void BackToMainMenu()
    {
        LevelLoader.Loader.LoadScene(0);
    }
}
