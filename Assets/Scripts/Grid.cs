using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid 
{
    private int totalRows;
    private int rows;
    private int cols;
    private float cellHeight;
    private float cellWidth;

    public Tile[,] grid;

    public Grid(int totalRows, int rows, int cols, float cellHeight, float cellWidth, Bounds bounds) {
        this.totalRows = totalRows;
        this.rows = rows;
        this.cols = cols;
        this.cellHeight = cellHeight;
        this.cellWidth = cellWidth;
        grid = InitializeGrid(bounds);

    }

    private Tile[,] InitializeGrid(Bounds bounds) {
        Tile[,] grid = new Tile[cols,totalRows];
        for(int col = 0; col < cols; col++) {
            for(int row = 0; row < totalRows; row++) {
                // grid[col,row] = Instantiate(tilePrefab, CalculateGamePosition(col, row, bounds), Quaternion.identity);
                // grid[col,row].Initialize(cellWidth, cellHeight,)
            }
        }
        return grid;
    }

    private Vector3 CalculateGamePosition(int lane, int row, Bounds bounds) {
            float xPosition = CalculateCoordinate(bounds.min.x, bounds.max.x, cols, lane);
            float yPosition = CalculateCoordinate(bounds.min.y, bounds.max.y, rows, row);
            return new Vector3(xPosition, yPosition, 1);
    }

    private float CalculateCoordinate(float minimum, float maximum, float totalNumber, float i) {
        float dimensionLength = maximum - minimum; // total grid dimensionLength
        float cellLength = dimensionLength / totalNumber;
        float cellCenter = minimum + cellLength / 2;
        return cellCenter + cellLength * i;
    }

}
