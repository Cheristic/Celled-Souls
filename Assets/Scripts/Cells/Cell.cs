using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to a Cell GameObject
/// </summary>
public class Cell : MonoBehaviour
{
    public CellType cellType;
    public bool mutable;
    public CellStateMachine stateMachine;

    public void CreateCell(int row, int col, CellType type)
    {
        cellType = type;
        stateMachine.CreateCell(row, col, type);
        GridManager.cellCheckMovement.AddListener(CheckMotion);
    }

    public void ChangeInitialType(CellType type)
    {
        cellType = type;
        stateMachine.ChangeInitialType(type);
    }
    
    public void ChangeType(CellType type)
    {
        cellType = type;
        stateMachine.ChangeType(type);
    }

    private void CheckMotion()
    {
        GridManager.Main.cellGridA.ClaimMovementSpot(Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y), this);
    }

    public void FlewOffScreen()
    {
        cellType = CellType.Dead;
        stateMachine.FlewOffScreen();
    }
    public void NewSpot(int row, int col)
    {
        stateMachine.row = row;
        stateMachine.col = col;
    }
}
