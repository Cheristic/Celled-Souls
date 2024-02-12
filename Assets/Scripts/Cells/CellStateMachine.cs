using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using static UnityEngine.ParticleSystem;
// From https://gamedevbeginner.com/state-machines-in-unity-how-and-when-to-use-them/
public class CellStateMachine : MonoBehaviour
{
    CellState currentState;

    public List<CellState> cellTypes;
    public AudioClip clip;

    public int row, col;

    // MISC CELL VARIABLES
    internal float explosionRadius = 4.0f;
    internal float explosionForce = 12000.0f;
    internal Rigidbody2D rigidbody2D;
    public AudioSource source;

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
    public void PlaySpawnSound(float height)
    {
        float f = (float)(height - GridManager.Main.cellGridA.pivot.y) / GridManager.Main.columns * 3;
        if (f == 0) f = .09f;
        source.pitch = f;
        source.Play();
    }


    // For initial placement
    public void CreateCell(int _row, int _col, CellType type)
    {
        row = _row; // Every cell is assigned a specific row+col to monitor
        col = _col;
        currentState = cellTypes[(int)type];
        currentState.OnStatePlace(this, true);
        GridManager.newGeneration.AddListener(Generation);
        GridManager.postGeneration.AddListener(PostGeneration);
    }

    public void ChangeInitialType(CellType type)
    {
        currentState = cellTypes[(int)type];
        currentState.OnStatePlace(this, false);
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
    public void OnStatePlace(CellStateMachine mach, bool first)
    {
        stateMachine = mach;
        OnPlace(first);
    }
    protected virtual void OnPlace(bool first) { }
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
    protected override void OnPlace(bool first)
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
    protected override void OnPlace(bool first)
    {
        if (!first) stateMachine.PlaySpawnSound(stateMachine.transform.position.y);
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Human Soul");
        stateMachine.gameObject.SetActive(true);
    }
    protected override void OnEnter()
    {
        stateMachine.PlaySpawnSound(stateMachine.transform.position.y);
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
    protected override void OnPlace(bool first)
    {
        if (!first) stateMachine.PlaySpawnSound(stateMachine.transform.position.y);
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Destruction Soul");
        stateMachine.gameObject.SetActive(true);
    }
    protected override void OnEnter()
    {
        stateMachine.PlaySpawnSound(stateMachine.transform.position.y);
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
            AudioManager.Instance.Play("Explosion");
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
            AudioManager.Instance.Play("Explosion");
            GridManager.Main.cellGridA.MakeDeadCell(stateMachine.row, stateMachine.col);
        }
    }

    protected override void OnExit()
    {

    }
}

public class IsolationCell : CellState
{
    protected override void OnPlace(bool first)
    {
        if (!first) stateMachine.PlaySpawnSound(stateMachine.transform.position.y);
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Isolation Soul");
        stateMachine.gameObject.SetActive(true);
    }
    protected override void OnEnter()
    {
        stateMachine.PlaySpawnSound(stateMachine.transform.position.y);
        stateMachine.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Isolation Soul");
    }

    protected override void OnGeneration()
    {
        int neighbors = GridManager.Main.cellGridPrevious.CensusAll(stateMachine.row, stateMachine.col);
        if (neighbors > 0)
        {
            AudioManager.Instance.Play("Loner Death");
            GridManager.Main.cellGridA.MakeDeadCell(stateMachine.row, stateMachine.col);
        }
    }

    protected override void OnExit()
    {
        
    }
}
