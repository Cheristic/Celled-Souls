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
    [SerializeField] CellStateMachine stateMachine;

    public void CreateCell(int row, int col, CellType type)
    {
        cellType = type;
        stateMachine.CreateCell(row, col, type);
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
}
