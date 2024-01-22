using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.Rendering.DebugUI.Table;

// From https://gamedevbeginner.com/state-machines-in-unity-how-and-when-to-use-them/
public class CellStateMachine : MonoBehaviour
{
    CellState currentState;

    public List<CellState> cellTypes;

    public int row, col;

    private void Awake()
    {
        cellTypes = new()
        {
            new DeadCell(),
            new ClassicCell(),
            new YellowCell()
        };
        
    }

    // For initial placement
    public void CreateCell(int _row, int _col, CellType type)
    {
        row = _row; // Every cell is assigned a specific row+col to monitor
        col = _col;
        currentState = cellTypes[(int)type];
        currentState.OnStatePlace(this);
        GridManager.newGeneration.AddListener(Generation);
    }

    public void ChangeInitialType(CellType type)
    {
        currentState = cellTypes[(int)type];
        currentState.OnStatePlace(this);
    }

    // For generations
    public void ChangeType(CellType newType)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }
        currentState = cellTypes[(int)newType]; // Access corresponding State from list
        currentState.OnStateEnter(this);
    }
    public void Generation()
    {
        currentState.OnStateGeneration();
    }
}

public abstract class CellState
{
    protected CellStateMachine stateMachine;
    public void OnStatePlace(CellStateMachine mach)
    {
        stateMachine = mach;
        OnPlace();
    }
    protected virtual void OnPlace() { }
    public void OnStateEnter(CellStateMachine mach)
    {
        stateMachine = mach;
        OnEnter();
    }
    protected virtual void OnEnter() { }
    public void OnStateGeneration()
    {
        OnGeneration();
    }
    protected virtual void OnGeneration() { }
    public void OnStateExit()
    {
        OnExit();
    }
    protected virtual void OnExit() { }
}

// ######## CELL TYPES ########
public enum CellType
{
    Dead,
    Classic,
    Yellow
}

public class DeadCell : CellState
{
    protected override void OnPlace()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = null;
        stateMachine.gameObject.SetActive(false);
    }
    protected override void OnEnter()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = null;
        stateMachine.gameObject.SetActive(false);
    }

    protected override void OnGeneration()
    {
        if (GridManager.Main.cellGridPrevious.Census(stateMachine.row, stateMachine.col) == 3)
        {
            GridManager.Main.cellGridA.MakeAliveCell(stateMachine.row, stateMachine.col, CellType.Classic);
        }
            
    }

    protected override void OnExit()
    {
        stateMachine.gameObject.SetActive(true);
    }
}

public class ClassicCell : CellState
{
    protected override void OnPlace()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Classic Cell");
        stateMachine.gameObject.SetActive(true);
    }
    protected override void OnEnter()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Classic Cell");
    }

    protected override void OnGeneration()
    {
        int neighbors = GridManager.Main.cellGridPrevious.Census(stateMachine.row, stateMachine.col);
        if (neighbors < 2 || neighbors > 3)
        {
            GridManager.Main.cellGridA.MakeDeadCell(stateMachine.row, stateMachine.col);
        }
    }

    protected override void OnExit()
    {

    }
}

public class YellowCell : CellState
{
    protected override void OnPlace()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Yellow Cell");
        stateMachine.gameObject.SetActive(true);
    }
    protected override void OnEnter()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Yellow Cell");
    }

    protected override void OnGeneration()
    {

    }

    protected override void OnExit()
    {

    }
}
