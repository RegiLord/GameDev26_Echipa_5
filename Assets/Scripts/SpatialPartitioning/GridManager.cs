using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int gridSize = 100;
    [SerializeField] private float cellSize = 5.0f;
    
    private CellData[,] cells;
    
    public static GridManager Instance = null;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(this);
            return;
        }
        
        initializeGrid();
    }

    private void initializeGrid()
    {
        cells = new CellData[gridSize, gridSize];
    }


    public void AddCell(CellData cellData)
    {
        if (cellData == null)
            return;
        
        cellData.next = cellData.prev = null;
        // We add a new cell to the grid
        var index = getIndexFromPosition(cellData.UnitData.transform.position);
        
        // The cell is not occupied
        if (cells[index.Item1, index.Item2] == null)
        {
            cells[index.Item1, index.Item2] = cellData;
            
            cellData.x = index.Item1;
            cellData.y = index.Item2;
        }
        // The cell is already occupied put the cell first
        else
        {
            var startCellData = cells[index.Item1, index.Item2];
            startCellData.prev = cellData;

            cellData.prev = null;
            cellData.next = startCellData;
            
            cells[index.Item1, index.Item2] = cellData;
            
            cellData.x = index.Item1;
            cellData.y = index.Item2;
        }
    }

    public void UpdateCell(CellData cellData)
    {
        if (cellData == null)
            return;

        // Check if index has changed from old index
        var index = getIndexFromPosition(cellData.UnitData.transform.position);

        if (index.Item1 != cellData.x || index.Item2 != cellData.y)
        {
            RemoveCell(cellData);
            AddCell(cellData);
        }
    }

    public void RemoveCell(CellData cellData)
    {
        if (cellData == null)
            return;
        
        // Remove Cell from grid
        
        // First check if index is valid
        if (cellData.x < 0 || cellData.y < 0 || cellData.x >= gridSize || cellData.y >= gridSize)
        {
            Debug.LogError($"Grid position {cellData} is out of range. (Remove)");
            return;
        }
        
        // If there is a previous connect the previous to the next cell
        if (cellData.prev != null)
            cellData.prev.next = cellData.next;
        else
            cells[cellData.x, cellData.y] = null;
        
        if (cellData.next != null)
        {
            cellData.next.prev = cellData.prev;

            // If it was the first in list, make the next node the new first
            if (cellData.prev == null)
                cells[cellData.x, cellData.y] = cellData.next;
        }

        cellData.next = cellData.prev = null;
        cellData.x = cellData.y = 0;
    }

    public List<UnitData> GetUnits(Vector3 position, float detectionRange)
    {
        var index = getIndexFromPosition(position);
        int gridRange = Mathf.CeilToInt(detectionRange / cellSize);
        
        List<UnitData> units = new List<UnitData>();

        for (int i = -gridRange; i <= gridRange; i++)
        {
            for (int j = -gridRange; j <= gridRange; j++)
            {
                (int, int) newIndex = (index.Item1 + i, index.Item2 + j);

                if (newIndex.Item1 < 0 || newIndex.Item2 < 0 && newIndex.Item1 >= gridSize || newIndex.Item2 >= gridSize)
                    continue;

                var cellUnits = GetUnitsInCell(cells[newIndex.Item1, newIndex.Item2]);
                units.AddRange(cellUnits);
            }
        }
        
        return units;
    }
    
    private (int, int) getIndexFromPosition(Vector3 position) => getIndexFromPosition(new Vector2(position.x, position.z));
    private (int, int) getIndexFromPosition(float x, float y) => getIndexFromPosition(new Vector2(x, y));
    private (int, int) getIndexFromPosition(Vector2 position)
    {
        int x = Mathf.FloorToInt(position.x / cellSize);
        int y = Mathf.FloorToInt(position.y / cellSize);
        
        (int, int) index = (gridSize / 2 + x, gridSize / 2 + y);

        if (index.Item1 < 0 || index.Item2 < 0 || index.Item1 >= gridSize || index.Item2 >= gridSize)
        {
            Debug.LogError($"Grid position {position} is out of range.(getIndex)");
            return (0, 0);
        }
        
        return index;
    }

    public List<UnitData> GetUnitsInCell(CellData cellData)
    {
        if (cellData == null) 
            return new List<UnitData>();
        return GetUnitsInCell(cellData.x, cellData.y);
    }
    public List<UnitData> GetUnitsInCell(int x, int y)
    {
        List<UnitData> units = new List<UnitData>();
        
        if (x < 0 || x >= gridSize || y < 0 || y >= gridSize) {
            Debug.LogError($"Grid position {x},{y} is out of bounds.");
            return null;
        }
        
        if (cells[x, y] == null)
            return units;
        
        var currentCell = cells[x, y];

        int rep = 0;
        while (currentCell != null)
        {
            units.Add(currentCell.UnitData);
            currentCell = currentCell.next;
            rep++;
        }
        
        return units;
    } 
}
