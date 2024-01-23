using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Main { get; private set; }

    [SerializeField] string titleGridPath;
    [SerializeField] Button playButton;
    [SerializeField] Button resetButton;
    [SerializeField] GameObject levelButtonPrefab;

    private void Awake()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        }
        else
        {
            Main = this;
        }
    }

    private void Start()
    {
        GridManager.Main.cellGridA.Populate(titleGridPath);
        playButton.onClick.AddListener(FirstHitPlayButton);

        Cell cell = GridManager.Main.cellGridA.grid[1, 7]; // Level 1
        GameObject buttObj = Instantiate(levelButtonPrefab, cell.transform);
        buttObj.GetComponent<LevelButton>().level = 1;

        if (((int)ProgressTracker.Main.progress) > 0) // Level 2
        {
            Cell cel = GridManager.Main.cellGridA.grid[6, 6];
            GameObject butObj = Instantiate(levelButtonPrefab, cel.transform);
            butObj.GetComponent<LevelButton>().level = 1;
        }
    }

    public void FirstHitPlayButton()
    {
        playButton.onClick.RemoveListener(FirstHitPlayButton);
        resetButton.gameObject.SetActive(true);
    }
}
