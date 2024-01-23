using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI.Table;

public class CellGrid
{
    internal int rows;
    internal int cols;
    internal Cell[,] grid;
    internal Cell[,] postMovementGrid; // Once cellCheckMovement is triggered, cells will claim their new position
    internal Queue<Cell> flewOutsideGrid;
    internal CellType[,] gridTypes; // Only used for keeping 1 state of memory
    internal Vector3 pivot;
    public CellGrid(int _rows, int _cols, Vector3 _pivot)
    {
        rows = _rows;
        cols = _cols;
        grid = new Cell[rows, cols];
        pivot = _pivot;
        postMovementGrid = new Cell[rows, cols];
        flewOutsideGrid = new();
    }

    public CellGrid(CellGrid curr)
    {
        rows = curr.rows;
        cols = curr.cols;
        grid = new Cell[rows, cols];
        gridTypes = new CellType[rows, cols];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                gridTypes[r, c] = curr.grid[r, c].cellType;
                grid[r, c] = curr.grid[r, c];
            }
        }
    }

    public void Populate(string filePath)
    {
        StreamReader stream = new StreamReader(filePath);

        // File is in format "x y Type mutable(n)", read each line to fill up initial Grid
        while(!stream.EndOfStream)
        {
            var parts = stream.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int row = int.Parse(parts[0]);
            int col = int.Parse(parts[1]);
            CellType type = Enum.Parse<CellType>(parts[2]);

            GameObject obj = GridManager.Main.CreateCell();
            obj.tag = "AliveCell";
            Cell cell = obj.GetComponent<Cell>();
            cell.mutable = parts.Length <= 3; // default is yes
            grid[row, col] = cell;
            obj.transform.position = new Vector2(row+pivot.x, col+pivot.y);
            cell.CreateCell(row, col, type);
        }
        
        // Set all remainder to dead cells
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c] == null)
                {
                    GameObject obj = GridManager.Main.CreateCell();
                    obj.tag = "DeadCell";
                    Cell cell = obj.GetComponent<Cell>();
                    cell.mutable = true;
                    grid[r, c] = cell;
                    obj.transform.position = new Vector2(r + pivot.x, c + pivot.y);
                    cell.CreateCell(r, c, CellType.Dead);
                }
            }
        }
    }

    // ### METHODS FOR INITIAL START (Mouse placement)
    public void PlaceAliveCell(int x, int y, CellType type)
    {
        Cell cell = grid[x - (int)pivot.x, y - (int)pivot.y];
        if (!cell.mutable) return;
        cell.gameObject.tag = "AliveCell";
        cell.ChangeInitialType(type);
    }

    public void ChangePlacedCellType(GameObject aliveCell, CellType type)
    {
        Cell cell = aliveCell.GetComponent<Cell>();
        if (!cell.mutable) return;
        cell.ChangeInitialType(type);
    }

    public void PlaceDeadCell(int x, int y, GameObject aliveCell)
    {
        Cell cell = grid[x - (int)pivot.x, y - (int)pivot.y];
        if (!cell.mutable) return;
        cell.gameObject.tag = "DeadCell";
        cell.ChangeInitialType(CellType.Dead);
    }


    // ### METHODS FOR GENERATIONS
    public void ClaimMovementSpot(int x, int y, Cell cell)
    {
        int row = x - (int)pivot.x;
        int col = y - (int)pivot.y;
        if (cell.cellType != CellType.Dead && row >= 0 && row < rows && col >= 0 && col < cols)
        {
            // Cell is still within grid
            postMovementGrid[row, col] = cell;
        } else
        {
            flewOutsideGrid.Enqueue(cell); // flew outside or dead cell
        }
    }
    public void AssignMovementSpots()
    {
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                if (postMovementGrid[r, c] == null)
                {
                    postMovementGrid[r, c] = flewOutsideGrid.Dequeue();
                    postMovementGrid[r, c].FlewOffScreen(); // Turn into dead cell
                    postMovementGrid[r, c].gameObject.tag = "DeadCell";
                }
                else
                {
                    postMovementGrid[r, c].gameObject.tag = "AliveCell";
                }
                postMovementGrid[r, c].transform.rotation = Quaternion.identity;
                postMovementGrid[r, c].transform.position = new Vector2(r + pivot.x, c + pivot.y);
                postMovementGrid[r, c].GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
                postMovementGrid[r, c].NewSpot(r, c);
                grid[r, c] = postMovementGrid[r, c];
            }
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                postMovementGrid[r, c] = null;
            }
        flewOutsideGrid.Clear();
    }

    public void MakeAliveCell(int row, int col, CellType type)
    {
        if (row >= 0 && row < rows && col >= 0 && col < cols)
        {
            Cell cell = grid[row, col];
            cell.gameObject.tag = "AliveCell";
            cell.ChangeType(type);
        }
    }
    public void MakeDeadCell(int row, int col)
    {
        if (row >= 0 && row < rows && col >= 0 && col < cols)
        {
            Cell cell = grid[row, col];
            cell.gameObject.tag = "DeadCell";
            cell.ChangeType(CellType.Dead);
        }
    }

    // On Reset
    public void Reset()
    {
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                grid[r, c].transform.position = new Vector2(r + pivot.x, c + pivot.y);
                grid[r, c].transform.position = new Vector2(r + pivot.x, c + pivot.y);
                grid[r, c].GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
    }

    // Counts neighbors of cell from previous generation
    public int Census(int row, int col)
    {
        int neighbors = 0;
        if (IsCellAlive(row - 1, col - 1)) neighbors++;
        if (IsCellAlive(row - 1, col)) neighbors++;
        if (IsCellAlive(row - 1, col + 1)) neighbors++;
        if (IsCellAlive(row, col - 1)) neighbors++;
        if (IsCellAlive(row, col + 1)) neighbors++;
        if (IsCellAlive(row + 1, col - 1)) neighbors++;
        if (IsCellAlive(row + 1, col)) neighbors++;
        if (IsCellAlive(row + 1, col + 1)) neighbors++;

        return neighbors;
    }

    // Returns whether the cell is dead (a Dead cell) or alive (any other type of cell)
    public bool IsCellAlive(int row, int col)
    {
        if (row >= 0 && row < rows && col >= 0 && col < cols)
        {
            return gridTypes[row,col] != CellType.Dead;
        }
        return false;
    }
}
