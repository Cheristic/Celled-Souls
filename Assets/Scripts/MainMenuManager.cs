using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Main { get; private set; }

    [SerializeField] string titleGridPath;
    [SerializeField] Button playButton;
    [SerializeField] Button resetButton;
    [SerializeField] GameObject levelButtonPrefab;
    [SerializeField] MouseCellPlacer placer;

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

    [RuntimeInitializeOnLoadMethod]
    private static void OnFirstLoad()
    {
        if (Main == null) return;
        Main.resetButton.gameObject.SetActive(false);
        Main.placer.disablePlacer++;
        Main.playButton.onClick.AddListener(Main.FirstHitPlayButton);
    }

    private void Start()
    {
        GridManager.Main.cellGridA.Populate(titleGridPath);

        // Level 1
        Cell cell = GridManager.Main.cellGridA.grid[8, 12]; 
        GameObject buttObj = Instantiate(levelButtonPrefab, cell.transform);
        buttObj.GetComponent<LevelButton>().Init(1);
        Cell_Inventory.Instance.inventory[0].gameObject.SetActive(true); // Enable human soul

        if (((int)ProgressTracker.Main.progress) > 0) // Level 2
        {
            Cell cel = GridManager.Main.cellGridA.grid[6, 6];
            GameObject butObj = Instantiate(levelButtonPrefab, cel.transform);
            butObj.GetComponent<LevelButton>().Init(2);
        }
    }

    public void FirstHitPlayButton()
    {
        playButton.onClick.RemoveListener(FirstHitPlayButton);
        resetButton.gameObject.SetActive(true);
        placer.disablePlacer--;
    }
}
