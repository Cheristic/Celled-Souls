using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CellGrid
{
    internal int rows;
    internal int cols;
    internal Cell[,] grid;
    internal Vector3 pivot;
    public CellGrid(int _rows, int _cols, Vector3 _pivot)
    {
        rows = _rows;
        cols = _cols;
        grid = new Cell[rows, cols];
        pivot = _pivot;
    }

    public void Populate(string filePath)
    {
        StreamReader stream = new StreamReader(filePath);

        // File is in format "x y Type", read each line to fill up initial Grid
        while(!stream.EndOfStream)
        {
            GameObject obj = LevelAssembler.Main.GetCellObjectFromPool();
            Cell cell = obj.GetComponent<Cell>();

            var parts = stream.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            cell.ChangeType(Enum.Parse<CellType>(parts[2]));
            int row = int.Parse(parts[0]);
            int col = int.Parse(parts[1]);
            grid[row, col] = cell;

            obj.transform.position = new Vector2(row+pivot.x, col+pivot.y);
        }
        
    }
}
