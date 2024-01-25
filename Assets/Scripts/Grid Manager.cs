using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public static GridManager Main { get; private set; }

    public int rows;
    public int columns;
    private float movSensitivity = 0.3f;


    public CellGrid cellGridA; // Grid for editing
    public CellGrid cellGridPrevious; // Keeps a copy of the previous generation for cells to reference
    public CellGrid cellGridInitial; // Keeps a copy of initial generation
    public GenerationStatus status;

    public GameObject defaultCellObject;

    [SerializeField] Button resetButton;
    [SerializeField] Button playButton;
    [SerializeField] MouseCellPlacer placer;
    [SerializeField] Sprite playButtonSprite;
    [SerializeField] Sprite pauseButtonSprite;

    public static UnityEvent cellCheckMovement;
    public static UnityEvent newGeneration;
    public static UnityEvent postGeneration;
    public static UnityEvent reset;
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

        cellCheckMovement = new UnityEvent();
        newGeneration = new UnityEvent();
        postGeneration = new UnityEvent();
        reset = new UnityEvent();

        transform.position = new Vector2(-rows/2, -columns/2); // Places Assembler at bottom left of grid

        cellGridA = new(rows, columns, transform.position);

        resetButton.onClick.AddListener(ResetToInitial);
        playButton.onClick.AddListener(PlayGenerations);
    }

    private void OnDestroy()
    {
        newGeneration.RemoveAllListeners();
        postGeneration.RemoveAllListeners();
        cellCheckMovement.RemoveAllListeners();
        reset.RemoveAllListeners();
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
            playButton.gameObject.GetComponent<Image>().sprite = pauseButtonSprite;
            placer.disablePlacer++;
            resetButton.interactable = true;
            cellGridInitial = new(cellGridA);
            status = GenerationStatus.Going;
            StartCoroutine("Generations");
        }
        else if (status == GenerationStatus.Stopped) // Iterate
        {
            playButton.gameObject.GetComponent<Image>().sprite = pauseButtonSprite;
            status = GenerationStatus.Going;
            StartCoroutine("Generations");
        }
        else if (status == GenerationStatus.Going) // Iterate
        {
            playButton.GetComponent<Image>().sprite = playButtonSprite;
            status = GenerationStatus.Stopped;
            StopCoroutine("Generations");
        }
    }

    private IEnumerator Generations()
    {
        bool inMotion;
        while (true)
        {
            inMotion = false;
            foreach (Cell cell in cellGridA.grid)
            {
                // Check for motion before moving to next generation
                if (cell.isActiveAndEnabled && cell.GetComponent<Rigidbody2D>().velocity.magnitude > movSensitivity)
                {
                    inMotion = true;
                    break;
                }
            }
            if (inMotion) yield return new WaitForSeconds(0.1f);
            else
            {
                cellCheckMovement.Invoke();
                cellGridA.AssignMovementSpots();
                yield return new WaitForSeconds(0.5f);
                cellGridPrevious = new(cellGridA);
                newGeneration.Invoke();
                postGeneration.Invoke();
                yield return new WaitForSeconds(0.5f);
            }
            
        }
    }

    public void ResetToInitial()
    {
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < columns; c++)
            {
                cellGridA.postMovementGrid[r, c] = null;
            }
        cellGridA.flewOutsideGrid.Clear();
        // change cellGridInitial to hold Cell references too

        placer.disablePlacer--;
        resetButton.interactable = false;
        playButton.GetComponent<Image>().sprite = playButtonSprite;
        status = GenerationStatus.Initial;
        StopCoroutine("Generations");
        for (int r = 0; r < rows; r++) 
            for (int c = 0; c < columns; c++)
            {
                Cell i_cell = cellGridInitial.grid[r, c];
                CellType i_type = cellGridInitial.gridTypes[r, c];
                if (i_type == CellType.Dead)
                {
                    i_cell.gameObject.tag = "DeadCell";
                    i_cell.ChangeInitialType(CellType.Dead);
                }  else
                {
                    i_cell.gameObject.tag = "AliveCell";
                    i_cell.ChangeInitialType(i_type);
                }
                cellGridA.grid[r, c] = i_cell;
            }
        cellGridA.Reset(); // Resets transforms
        reset.Invoke();
    }



    public GameObject CreateCell()
    {
        return Instantiate(defaultCellObject);
    }
}
