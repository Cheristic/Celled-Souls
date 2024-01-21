using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Main { get; private set; }

    public int rows;
    public int columns;
    [SerializeField] string gridBuildPath;


    public CellGrid cellGridA; // Grid for editing
    public CellGrid cellGridPrevious; // Keeps a copy of the previous generation for cells to reference
    public CellGrid cellGridInitial; // Keeps a copy of initial generation
    public GenerationStatus status;

    public GameObject defaultCellObject;

    public static UnityEvent newGeneration;
    void Start()
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
        cellGridA.Populate(gridBuildPath);
    }

    public enum GenerationStatus
    {
        Initial,
        Going,
        Stopped
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && status == GenerationStatus.Initial) // Iterate
        {
            cellGridInitial = new(cellGridA);
            status = GenerationStatus.Going;
            StartCoroutine("Generations");
        }
        else if (Input.GetKeyDown(KeyCode.Space) && status == GenerationStatus.Stopped) // Iterate
        {
            status = GenerationStatus.Going;
            StartCoroutine("Generations");
        }
        else if (Input.GetKeyDown(KeyCode.Space) && status == GenerationStatus.Going) // Iterate
        {
            status = GenerationStatus.Stopped;
            StopCoroutine("Generations");
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            status = GenerationStatus.Initial;
            StopCoroutine("Generations");
            ResetToInitial();
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

    private void ResetToInitial()
    {
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
