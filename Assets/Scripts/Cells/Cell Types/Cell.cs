using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to a Cell GameObject
/// </summary>
public class Cell : MonoBehaviour
{
    public CellType cellType;
    [SerializeField] CellStateMachine stateMachine;

    public void ChangeType(CellType type)
    {
        cellType = type;
        stateMachine.ChangeState(type);
    }
}
