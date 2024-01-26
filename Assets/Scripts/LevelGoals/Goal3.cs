using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal3 : MonoBehaviour
{
    [SerializeField] GameObject youWinClick;
    [SerializeField] GameObject creature;
    [SerializeField] string gridBuildPath;

    private void Start()
    {
        GridManager.Main.cellGridA.Populate(gridBuildPath);
        youWinClick.GetComponentInChildren<Button>().onClick.AddListener(BackToMainMenu); // Clicking creature sets it to active

        Cell cell = GridManager.Main.cellGridA.grid[24, 7];
        GameObject buttObj = Instantiate(creature, cell.transform);
        buttObj.GetComponent<Goal3Creature>().Init(youWinClick);
    }

    private void BackToMainMenu()
    {
        LevelLoader.Loader.LoadScene(0);
    }
}
