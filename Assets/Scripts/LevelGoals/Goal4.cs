using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal4 : MonoBehaviour
{
    [SerializeField] GameObject youWinClick;
    private int gens;
    void Start()
    {
        GridManager.reset.AddListener(OnReset);
        GridManager.newGeneration.AddListener(OnGeneration);
        youWinClick.GetComponentInChildren<Button>().onClick.AddListener(BackToMainMenu);
        gens = 0;
    }

    private void OnReset()
    {
        gens = 0;
    }

    private void OnGeneration()
    {
        gens++;
        if (GridManager.Main.cellGridA.grid[23, 11].cellType != CellType.Isolation)
        {
            return;
        }
        if (gens == 25)
        {
            AudioManager.Instance.Play("Victory");
            if (youWinClick.activeInHierarchy) return;
            youWinClick.SetActive(true);
            ProgressTracker.Main.LevelWin(4);
        }
        
    }

    private void BackToMainMenu()
    {
        LevelLoader.Loader.LoadScene(0);
    }
}
