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
        GridManager.Main.cellGridA.grid[2, 7].AddComponent<LevelButton>();
        GridManager.Main.cellGridA.grid[2, 7].GetComponent<LevelButton>().level = 1;
        Instantiate(levelButtonPrefab, GridManager.Main.cellGridA.grid[2, 7].transform);
        
    }

    public void FirstHitPlayButton()
    {
        playButton.onClick.RemoveListener(FirstHitPlayButton);
        resetButton.gameObject.SetActive(true);
    }
}
