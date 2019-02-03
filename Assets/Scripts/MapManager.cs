using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int width;
    public int height;
    public float tileSize = 1.0f;
    public float tileOffset = 0.5f;

    private List<Cell> cells;

    public static MapManager Instance { get; private set; }
    public List<Cell> VisualPath { get; set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            if (Instance != this)
                Destroy(this);
    }

    void Start()
    {
        InstantiateCells();
    }

    void Update()
    {
        DrawMap();
        UpdateCells();
    }

    public void PlaceAgents(List<Agent> agents)
    {
       for (int i = 0; i < agents.Count; ++i)
       {
           int x = Random.Range(0, width);
           int y = Random.Range(0, height);
           Cell cell = CellAt(x, y);
           if (cell != null && cell.Walkable)
                agents[i].SnapTo(new Vector2Int(x, y));
       }
    }

    public bool IsPositionOnMap(Vector2Int pos)
    {
        return (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height);
    }

    public Cell CellAt(Vector2Int pos)
    {
        if (cells == null)
            return null;

        if (!IsPositionOnMap(pos))
            return null;

        return cells[pos.x + pos.y * width];
    }

    public Cell CellAt(int x, int y)
    {
        return CellAt(new Vector2Int(x, y));
    }

    private void InstantiateCells()
    {
        cells = new List<Cell>(width * height);
        for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
            {
               GameObject go = new GameObject(string.Format("Cell {0} - {1}", x, y));
               Cell cell = go.AddComponent<Cell>();
               cell.Initialize(new Vector2Int(x, y));
               cells.Add(cell);
            }
    }

    private void DrawMap()
    {
        Vector3 widthLine = Vector3.right * width;
        Vector3 heightLine = Vector3.forward * height;

        for (int z = 0; z <= width; ++z)
        {
            Vector3 start = Vector3.forward * z;
            Debug.DrawLine(start, start + widthLine);

            for (int x = 0; x <= width; ++x)
            {
                start = Vector3.right * x;
                Debug.DrawLine(start, start + heightLine);
            }
        }
    }

    private void UpdateCells()
    {
        for (int i = 0; i < cells.Count; ++i)
        {
            if (VisualPath != null && VisualPath.Contains(cells[i]))
                cells[i].Color = Color.green;
            else
                cells[i].Color = Color.yellow;
        }
    }
}
