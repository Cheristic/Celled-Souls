using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAssembler : MonoBehaviour
{
    public static LevelAssembler Main { get; private set; }

    [SerializeField] int rows;
    [SerializeField] int columns;
    [SerializeField] string gridBuildPath;


    public CellGrid cellGrid;

    private int poolSize;
    private List<GameObject> cellObjectPool; // Holds all dead cells
    public GameObject defaultCellObject;
    void Start()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        } else
        {
            Main = this;
        }

        poolSize = rows * columns;
        cellObjectPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject temp = Instantiate(defaultCellObject);
            temp.SetActive(false);
            cellObjectPool.Add(temp);
        }

        transform.position = new Vector2(-rows, -columns); // Places Assembler at bottom left of grid

        cellGrid = new(rows, columns, transform.position);
        cellGrid.Populate(gridBuildPath);
    }

    public GameObject GetCellObjectFromPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (!cellObjectPool[i].activeInHierarchy)
            {
                return cellObjectPool[i];
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
