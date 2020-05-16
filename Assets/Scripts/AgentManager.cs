﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviourSingleton<AgentManager>
{
    [SerializeField] private Agent agentPrefab = default;
    [SerializeField] private int agentCount = 3;

    private List<Agent> _agents { get; set; } = new List<Agent>();

    public Agent SelectedAgent { get; private set; }

    public bool HasBusyAgent
    {
        get
        {
            for (int a = 0; a < _agents.Count; ++a)
            {
                if (_agents[a].Busy)
                    return true;
            }
            return false;
        }
    }

    protected override void OnAwake()
    {
        _agents.Capacity = agentCount;
        for (int i = 0; i < agentCount; i++)
        {
            _agents.Add(Instantiate(agentPrefab));
        }

        GameEvents.CellSelected.AddListener(OnCellSelected);
    }

    private void Start()
    {
        foreach (Agent agent in _agents)
        {
            // result: (hasCell, position)
            (bool, Vector2Int) result = MapManager.Instance.ReserveRandomCell(CellType.Agent, agent);
            if (!result.Item1)
                continue;

            agent.Position = result.Item2;
            agent.transform.position= agent.Position.ToWorldPosition();
        }
    }

    private void OnCellSelected(Cell cell)
    {
        Agent agent = _agents[0];
        agent.Cast(AbilityType.Move, MapManager.Instance.CellAt(agent.Position), cell);
    }
}
