using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Ability
{
    [Tooltip("Time that it takes to move between two cells.")]
    public float movementDuration = 0.25f;

    private Agent agent;
    private int pathIndex;
    private float movementTimer;
    public List<Cell> Path { get; set; }

    private void Awake()
    {
        Type = Ability.CastType.Move;
    }

    private void Update()
    {
        if (Casting && Path != null)
            MoveToNextCell();
    }

    public override List<Cell> Range(Cell source)
    {
        if (source == null)
        {
            Debug.LogError("[source] is null");
            return null;
        }

        return PathMaker.ExpandToWalkables(source, range);
    }

    public override void Cast(Cell source, Cell target)
    {
        if (source == null)
        {
            Debug.LogError("[source] is null");
            return;
        }

        if (target == null)
        {
            Debug.LogError("[target] is null");
            return;
        }

        if (Casting)
        {
            Debug.LogError("Cannot accept new move while moving");
            return;
        }

        if (source.CurrentState != Cell.State.Agent)
        {
            Debug.LogErrorFormat("[source], {0}: state must be Agent, is {1} instead", source.Position, source.CurrentState);
            return;
        }

        if (target.CurrentState != Cell.State.Empty)
        {
            Debug.LogErrorFormat("Cannot move to cell with state '{0}'", target.CurrentState);
            return;
        }

        Debug.LogFormat("Casting Move() from {0} to {1}", source.Position, target.Position);

        if (Path == null)
        {
            Debug.Log("[Path] is null, finding fastest path with AStar");
            Path = PathMaker.AStar(source, target);
        }

        StartMoving(source);
    }

    private void StartMoving(Cell source)
    {
        if (Path == null)
        {
            Debug.LogError("[Path] is null, cannot start moving");
            return;
        }

        if (Path.Count < 1)
        {
            Debug.LogError("[Path] is empty");
            return;
        }

        Casting = true;
        agent = GameManager.Instance.AgentAt(source);
        source.CurrentState = Cell.State.Empty;
        movementTimer = 0f;
        pathIndex = 0;
    }

    private void MoveToNextCell()
    {
        if (pathIndex >= Path.Count)
        {
            FinishedMoving();
            return;
        }
        Cell current = MapManager.Instance.CellAt(agent.Position);
        Cell next = Path[pathIndex];

        transform.position = Vector3.Lerp(
            Utilities.ToWorldPosition(current.Position, transform),
            Utilities.ToWorldPosition(next.Position, transform),
            movementTimer / movementDuration);

        if (movementTimer < movementDuration)
            movementTimer += Time.deltaTime;
        else
        {
            movementTimer -= movementDuration;
            agent.Position = next.Position;
            ++pathIndex;
        }
    }

    private void FinishedMoving()
    {
        Casting = false;
        Path = null;
        pathIndex = 0;
        movementTimer = 0f;
        agent.Position = Utilities.ToMapPosition(transform.position);
        MapManager.Instance.CellAt(agent.Position).CurrentState = Cell.State.Agent;
    }
}