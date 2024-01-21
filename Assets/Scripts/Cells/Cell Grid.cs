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
    internal CellType[,] gridTypes; // Only used for keeping 1 state of memory
    internal Vector3 pivot;
    public CellGrid(int _rows, int _cols, Vector3 _pivot)
    {
        rows = _rows;
        cols = _cols;
        grid = new Cell[rows, cols];
        pivot = _pivot;
    }

    public CellGrid(CellGrid curr)
    {
        rows = curr.rows;
        cols = curr.cols;
        gridTypes = new CellType[rows, cols];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                gridTypes[r, c] = curr.grid[r, c].cellType;
            }
        }
    }

    public void Populate(string filePath)
    {
        StreamReader stream = new StreamReader(filePath);

        // File is in format "x y Type", read each line to fill up initial Grid
        while(!stream.EndOfStream)
        {
            var parts = stream.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int row = int.Parse(parts[0]);
            int col = int.Parse(parts[1]);
            CellType type = Enum.Parse<CellType>(parts[2]);

            GameObject obj = LevelManager.Main.CreateCell();
            obj.tag = "AliveCell";
            Cell cell = obj.GetComponent<Cell>();
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
                    GameObject obj = LevelManager.Main.CreateCell();
                    obj.tag = "DeadCell";
                    Cell cell = obj.GetComponent<Cell>();
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
        cell.gameObject.tag = "AliveCell";
        cell.ChangeInitialType(type);
    }

    public void ChangePlacedCellType(GameObject aliveCell, CellType type)
    {
        aliveCell.GetComponent<Cell>().ChangeInitialType(type);
    }

    public void PlaceDeadCell(int x, int y, GameObject aliveCell)
    {
        Cell cell = grid[x - (int)pivot.x, y - (int)pivot.y];
        cell.gameObject.tag = "DeadCell";
        cell.ChangeInitialType(CellType.Dead);
    }


    // ### METHODS FOR GENERATIONS

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
