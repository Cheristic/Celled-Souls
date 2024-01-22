using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public static GridManager Main { get; private set; }

    public int rows;
    public int columns;


    public CellGrid cellGridA; // Grid for editing
    public CellGrid cellGridPrevious; // Keeps a copy of the previous generation for cells to reference
    public CellGrid cellGridInitial; // Keeps a copy of initial generation
    public GenerationStatus status;

    public GameObject defaultCellObject;

    [SerializeField] Button resetButton;
    [SerializeField] Button playButton;
    [SerializeField] MouseCellPlacer placer;

    public static UnityEvent newGeneration;
    void Awake()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        } else
        {
            Main = this;
        }
        status = GenerationStatus.Initial;

        newGeneration = new UnityEvent();

        transform.position = new Vector2(-rows/2, -columns/2); // Places Assembler at bottom left of grid

        cellGridA = new(rows, columns, transform.position);

        resetButton.onClick.AddListener(ResetToInitial);
        playButton.onClick.AddListener(PlayGenerations);
    }

    private void OnDestroy()
    {
        newGeneration.RemoveAllListeners();
    }

    public enum GenerationStatus
    {
        Initial,
        Going,
        Stopped
    }
    
    public void PlayGenerations()
    {
        if (status == GenerationStatus.Initial) // Iterate
        {
            placer.disablePlacer++;
            resetButton.interactable = true;
            cellGridInitial = new(cellGridA);
            status = GenerationStatus.Going;
            StartCoroutine("Generations");
        }
        else if (status == GenerationStatus.Stopped) // Iterate
        {
            status = GenerationStatus.Going;
            StartCoroutine("Generations");
        }
        else if (status == GenerationStatus.Going) // Iterate
        {
            status = GenerationStatus.Stopped;
            StopCoroutine("Generations");
        }
    }

    private IEnumerator Generations()
    {
        while (true)
        {
            cellGridPrevious = new(cellGridA);
            newGeneration.Invoke();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ResetToInitial()
    {
        placer.disablePlacer--;
        resetButton.interactable = false;
        status = GenerationStatus.Initial;
        StopCoroutine("Generations");
        for (int r = 0; r < rows; r++) 
            for (int c = 0; c < columns; c++)
            {
                Cell cell = cellGridA.grid[r, c];
                CellType type = cellGridInitial.gridTypes[r, c];
                if (type == CellType.Dead)
                {
                    cell.gameObject.tag = "DeadCell";
                    cell.ChangeInitialType(CellType.Dead);
                }  else
                {
                    cell.gameObject.tag = "AliveCell";
                    cell.ChangeInitialType(type);
                }
            }
    }



    public GameObject CreateCell()
    {
        return Instantiate(defaultCellObject);
    }
}
