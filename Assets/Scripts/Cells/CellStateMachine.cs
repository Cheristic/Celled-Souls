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

    // MISC CELL VARIABLES
    internal float explosionRadius = 4.0f;
    internal float explosionForce = 12000.0f;
    internal Rigidbody2D rigidbody2D;

    private void Awake()
    {
        cellTypes = new()
        {
            new DeadCell(),
            new HumanCell(),
            new DestructionCell(),
            new IsolationCell()
        };
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // For initial placement
    public void CreateCell(int _row, int _col, CellType type)
    {
        row = _row; // Every cell is assigned a specific row+col to monitor
        col = _col;
        currentState = cellTypes[(int)type];
        currentState.OnStatePlace(this);
        GridManager.newGeneration.AddListener(Generation);
        GridManager.postGeneration.AddListener(PostGeneration);
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

    private void Update()
    {
        currentState.OnStateUpdate();
    }
    private void Generation()
    {
        currentState.OnStateGeneration();
    }
    private void PostGeneration()
    {
        currentState.OnStatePostGeneration();
    }

    // Cell went off screen, turn into dead cell
    public void FlewOffScreen()
    {
        if (currentState == cellTypes[0]) return;
        currentState = cellTypes[0];
        currentState.OnStateEnter(this);
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
    public void OnStateUpdate()
    {
        OnUpdate();
    }
    protected virtual void OnUpdate() { }
    public void OnStateGeneration()
    {
        OnGeneration();
    }
    protected virtual void OnGeneration() { }
    public void OnStatePostGeneration()
    {
        OnPostGeneration();
    }
    protected virtual void OnPostGeneration() { }
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
    Human,
    Destruction,
    Isolation
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
        if (GridManager.Main.cellGridPrevious.Census(stateMachine.row, stateMachine.col, CellType.Human) == 3)
        {
            GridManager.Main.cellGridA.MakeAliveCell(stateMachine.row, stateMachine.col, CellType.Human);
        }
            
    }

    protected override void OnExit()
    {
        stateMachine.gameObject.SetActive(true);
    }
}

public class HumanCell : CellState
{
    protected override void OnPlace()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Human Soul");
        stateMachine.gameObject.SetActive(true);
    }
    protected override void OnEnter()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Human Soul");
    }

    protected override void OnGeneration()
    {
        int neighbors = GridManager.Main.cellGridPrevious.Census(stateMachine.row, stateMachine.col, CellType.Human);
        if (neighbors < 2 || neighbors > 3)
        {
            GridManager.Main.cellGridA.MakeDeadCell(stateMachine.row, stateMachine.col);
        }
    }

    protected override void OnExit()
    {

    }
}

public class DestructionCell : CellState
{
    float explosionVelocityThreshold = .3f;
    protected override void OnPlace()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Destruction Soul");
        stateMachine.gameObject.SetActive(true);
    }
    protected override void OnEnter()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Destruction Soul");
        stateMachine.gameObject.SetActive(true);
    }

    protected override void OnUpdate()
    {
        // Repeat checks of dead cells
        if (stateMachine.rigidbody2D.velocity.magnitude > explosionVelocityThreshold)
        {
            // Create explosion https://www.reddit.com/r/Unity2D/comments/fg64lm/explosive_force_in_2d/
            Collider2D[] hits = Physics2D.OverlapCircleAll(stateMachine.transform.position, stateMachine.explosionRadius, LayerMask.GetMask("Cells"));
            foreach (var hit in hits)
            {
                Vector3 dir = hit.gameObject.transform.position - stateMachine.transform.position;
                float fallOff = 1 - (dir.magnitude / stateMachine.explosionRadius);
                hit.GetComponent<Rigidbody2D>().AddForce(dir.normalized * (fallOff <= 0 ? 0 : stateMachine.explosionForce) * fallOff);
            }
            GridManager.Main.cellGridA.MakeDeadCell(stateMachine.row, stateMachine.col);
        }
    }

    protected override void OnPostGeneration()
    {
        // Repeat checks of dead cells
        if (GridManager.Main.cellGridPrevious.Census(stateMachine.row, stateMachine.col, CellType.Human) == 3)
        {
            // Create explosion https://www.reddit.com/r/Unity2D/comments/fg64lm/explosive_force_in_2d/
            Collider2D[] hits = Physics2D.OverlapCircleAll(stateMachine.transform.position, stateMachine.explosionRadius, LayerMask.GetMask("Cells"));
            foreach (var hit in hits)
            {
                Vector3 dir = hit.gameObject.transform.position - stateMachine.transform.position;
                float fallOff = 1 - (dir.magnitude / stateMachine.explosionRadius);
                hit.GetComponent<Rigidbody2D>().AddForce(dir.normalized * (fallOff <= 0 ? 0 : stateMachine.explosionForce) * fallOff);
            }
            GridManager.Main.cellGridA.MakeDeadCell(stateMachine.row, stateMachine.col);
        }
    }

    protected override void OnExit()
    {

    }
}

public class IsolationCell : CellState
{
    protected override void OnPlace()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Isolation Soul");
        stateMachine.gameObject.SetActive(true);
    }
    protected override void OnEnter()
    {
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Isolation Soul");
    }

    protected override void OnGeneration()
    {
        int neighbors = GridManager.Main.cellGridPrevious.CensusAll(stateMachine.row, stateMachine.col);
        if (neighbors > 0)
        {
            GridManager.Main.cellGridA.MakeDeadCell(stateMachine.row, stateMachine.col);
        }
    }
}
