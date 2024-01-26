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
        Main.resetButton.onClick.AddListener(Main.FirstHitResetButton);
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
            Cell cel = GridManager.Main.cellGridA.grid[13, 10];
            GameObject butObj = Instantiate(levelButtonPrefab, cel.transform);
            butObj.GetComponent<LevelButton>().Init(2);
        }

        if (((int)ProgressTracker.Main.progress) > 1) // Level 2
        {
            Cell cel = GridManager.Main.cellGridA.grid[18, 13];
            GameObject butObj = Instantiate(levelButtonPrefab, cel.transform);
            butObj.GetComponent<LevelButton>().Init(3);
            Cell_Inventory.Instance.inventory[1].gameObject.SetActive(true); // Enable human soul
        }

    }

    public void FirstHitPlayButton()
    {
        playButton.onClick.RemoveListener(FirstHitPlayButton);
        resetButton.gameObject.SetActive(true);
        placer.disablePlacer--;
    }

    public void FirstHitResetButton()
    {
        resetButton.onClick.RemoveListener(FirstHitResetButton);
        StartCoroutine(ShowFirstTooltip());
    }

    IEnumerator ShowFirstTooltip()
    {
        Tooltip tip = Cell_Inventory.Instance.inventory[0].tooltip.gameObject.GetComponent<Tooltip>();
        tip.ShowTooltip();
        yield return new WaitForSeconds(5f);
        tip.HideTooltip();
    }
}
