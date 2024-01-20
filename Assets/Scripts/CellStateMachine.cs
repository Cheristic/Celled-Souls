using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// From https://gamedevbeginner.com/state-machines-in-unity-how-and-when-to-use-them/
public class CellStateMachine : MonoBehaviour
{
    CellState currentState;

    public List<CellState> cellTypes;

    private void Awake()
    {
        cellTypes = new()
        {
            new DeadCell(),
            new ClassicCell(),
            new YellowCell()
        };

        currentState = cellTypes[0];
        currentState.OnStateEnter(this);
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnStateUpdate();
        }  
    }

    public void ChangeState(CellType newState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }
        currentState = cellTypes[(int)newState]; // Access corresponding State from list
        currentState.OnStateEnter(this);
    }
}

public abstract class CellState
{
    protected CellStateMachine stateMachine;
    public void OnStateEnter(CellStateMachine mach) {
        stateMachine = mach;
        OnEnter();
    }
    protected virtual void OnEnter() { }
    public void OnStateUpdate()
    {
        OnUpdate();
    }
    protected virtual void OnUpdate() { }
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
    protected override void OnEnter()
    {
        stateMachine.gameObject.SetActive(false);
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnExit()
    {
        stateMachine.gameObject.SetActive(true);
    }
}

public class ClassicCell : CellState
{
    protected override void OnEnter()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Classic Cell");
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnExit()
    {

    }
}

public class YellowCell : CellState
{
    protected override void OnEnter()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Yellow Cell");
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnExit()
    {

    }
}
