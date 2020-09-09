using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Color[] colors;

    [Range(1, 12)]
    public int numberOfColumns;
    [Range(1,12)]
    public int numberOfRows;

    [Range(0, 100)]
    public float marginX;
    [Range(0,100)]
    public float marginY;

    [Range(0,100)]
    public float cellPadding;

    public Camera cam;

    public Tile[,] grid;

    private float cellWidth;
    private float cellHeight;

    [SerializeField]
    private Tile tilePrefab;

    private Bounds cameraBounds;

    private bool rotatingGrid = false;
    private bool reversingStack = false;

    private void Awake()
    {
        RenderBoard();

        Gestures.OnSwipe += HandleSwipe;
        Gestures.SwipeEnded += EndSwipe;
    }

    private void RenderBoard() {

        if(grid != null) {
            for(int i = 0; i < grid.GetLength(0); i++) {
                for(int j = 0; j < grid.GetLength(1); j++) {
                    if(grid[i,j] != null) {
                        grid[i,j].Destroy();
                    }
                }
            }
        }
        if(cam == null) {
            Debug.LogError("Cannot calculate grid bounds because camera is missing.");
            return;
        }
        // The height of the frustum at a given distance (both in world units) can be obtained with the following formula:

        float frustumHeight = 2.0f * 1000 * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

    
        float frustumWidth = frustumHeight * cam.aspect;

        float screenHeight = frustumWidth - marginY;
        float screenWidth = frustumWidth - marginX;
        cellWidth = screenWidth/numberOfColumns - cellPadding;
        cellHeight = screenHeight/numberOfRows - cellPadding;
        cameraBounds = new Bounds(cam.transform.position, new Vector3(screenWidth, screenHeight, 0));
        grid = InitializeGrid();
        RandomizeBoard();

        List<List<Tile>> matches = CheckForMatches();
        while(matches.Count > 0) {
            List<Vector2Int> newTiles = new List<Vector2Int>();
            foreach(List<Tile> match in matches) {
                foreach(Tile tile in match) {
                    Debug.Log(" Tile: " + tile);
                    newTiles.Add(tile.Index);
                    tile.SetColor(new Color(0,0,0,0));
                }
            };

            foreach(Vector2Int index in newTiles) {
                grid[index.x, index.y].SetColor(RandomColor());
            }
            matches = CheckForMatches();
        }
    }

    private Tile[,] InitializeGrid() {
        Tile[,] grid = new Tile[numberOfColumns,numberOfRows];
        for(int col = 0; col < numberOfColumns; col++) {
            for(int row = 0; row < numberOfRows; row++) {
                grid[col,row] = RenderTile(col, row, cellWidth, cellHeight, RandomColor());
            }
        }
        return grid;
    }

    private void RandomizeBoard() {
        for(int i = 0; i < numberOfColumns; i++) {
            for(int j = 0; j < numberOfRows; j++) {
                if(grid[i,j] != null) {
                    grid[i,j].SetColor(RandomColor());
                } else {
                    grid[i,j] = RenderTile(i, j, cellWidth, cellHeight, RandomColor());
                }
            }
        }
    }


    public void HandleTileTapped(Vector2 index) {
        Tile tapped = grid[(int) index.x, (int) index.y];
        if(!reversingStack) {
            StartCoroutine(ReverseStack(tapped));
        } else {
            Debug.Log("still reversingStack");
        }
    }

    private IEnumerator ReverseStack(Tile tapped) {
        reversingStack = true;
        Vector3 center = new Vector3(tapped.Position.x, tapped.Position.y + (grid[tapped.Index.x, numberOfRows -1].Position.y - tapped.Position.y)/2, tapped.Position.z);
        Debug.Log("Finding center: " + center);

        int tappedHeight = tapped.Index.y;
        List<Tile> rotatingTiles = new List<Tile>();
        for(int row = tappedHeight; row < tappedHeight + Mathf.Ceil((numberOfRows - tappedHeight)/2f); row++) {
            Tile a = grid[tapped.Index.x, row];
            Tile b = grid[tapped.Index.x, numberOfRows - 1 - row + tappedHeight];
            SwapTiles(ref a, ref b);

            rotatingTiles.Add(a);
            if(a != b)
                rotatingTiles.Add(b);
            
        }
        // yield return null;
        yield return StartCoroutine(RotateStack(rotatingTiles, center, 180, .5f));

        List<List<Tile>> matches = CheckForMatches();
        if(matches.Count > 0) {
            yield return new WaitForSeconds(.5f);
            List<Vector2Int> newTiles = new List<Vector2Int>();
            foreach(List<Tile> match in matches) {
                foreach(Tile tile in match) {
                    Debug.Log(" Tile: " + tile);
                    newTiles.Add(tile.Index);
                    tile.SetColor(new Color(1,1,1,0));
                }
                yield return new WaitForSeconds(.3f);
            };
            yield return new WaitForSeconds(.5f);

            foreach(Vector2Int index in newTiles) {
                grid[index.x, index.y].SetColor(RandomColor());
            }
        }
        reversingStack = false;
    }

    private IEnumerator RotateStack(List<Tile> tiles, Vector3 center, float degrees, float duration) {

        foreach(Tile tile in tiles) {
            tile.transform.Translate(Vector3.forward * -100);
            yield return null;
        }
        yield return new WaitForSeconds(.1f);

        if(tiles.Count > 1) {
            float totalDegrees = 0;
            float startTime = Time.time;
            float endTime = startTime + duration;
            while(Time.time < endTime || totalDegrees < degrees) {
                foreach(Tile tile in tiles) {
                    float rotationThisFrame = Time.deltaTime * degrees / duration;
                    totalDegrees += Mathf.Abs(rotationThisFrame);
                    tile.transform.RotateAround(center, Vector3.forward, rotationThisFrame);
                    // tile.transform.rotation = Quaternion.identity;
                }
                yield return null;
            }
            yield return new WaitForSeconds(.1f);
        }
        foreach(Tile tile in tiles) {
            tile.transform.rotation = Quaternion.identity;
            tile.Position = CalculateGamePosition(tile.Index.x, tile.Index.y, cameraBounds);
        }
        yield return null;
    }

    

    private IEnumerator RotateGrid(SwipeInfo.SwipeDirection direction) {

        List<Tile> rotatingTiles = new List<Tile>();

        if(direction == SwipeInfo.SwipeDirection.LEFT) {
            for(int i = 0; i < numberOfColumns/2; i++) {
                for(int j = i; j < numberOfRows - i - 1; j++) {
                    Tile temp = grid[i,j];
                    grid[i,j] = grid[j,numberOfRows - i - 1];
                    grid[j,numberOfRows - i - 1] = grid[numberOfColumns - i - 1, numberOfRows - j - 1];
                    grid[numberOfColumns - i - 1, numberOfRows - j - 1] = grid[numberOfRows - j - 1, i];
                    grid[numberOfRows - j - 1, i] = temp;

                    grid[i,j].Index = new Vector2Int(i,j);
                    grid[j,numberOfRows - i - 1].Index = new Vector2Int(j, numberOfRows - i - 1);
                    grid[numberOfColumns - i - 1, numberOfRows - j - 1].Index = new Vector2Int(numberOfColumns - i -1, numberOfRows -j -1);
                    grid[numberOfRows - j - 1, i].Index = new Vector2Int(numberOfRows - j -1, i);

                    rotatingTiles.Add(grid[i,j]);
                    rotatingTiles.Add(grid[j,numberOfRows - i - 1]);
                    rotatingTiles.Add(grid[numberOfColumns - i - 1, numberOfRows - j - 1]);
                    rotatingTiles.Add(grid[numberOfRows - j - 1, i]);
                }
            }
        } else if(direction == SwipeInfo.SwipeDirection.RIGHT) {
            Debug.Log("rotating right");
            for(int i = 0; i < numberOfColumns/2; i++) {
                for(int j = i; j < numberOfRows - i - 1; j++) {
                    Tile temp = grid[i,j];
                    grid[i,j] = grid[numberOfRows - j - 1, i];
                    grid[numberOfRows - j - 1, i] = grid[numberOfColumns - i - 1, numberOfRows - j - 1];
                    grid[numberOfColumns - i - 1, numberOfRows - j - 1] = grid[j,numberOfRows - i - 1];
                    grid[j, numberOfRows -i -1] = temp;

                    grid[i,j].Index = new Vector2Int(i,j);
                    grid[j,numberOfRows - i - 1].Index = new Vector2Int(j, numberOfRows - i - 1);
                    grid[numberOfColumns - i - 1, numberOfRows - j - 1].Index = new Vector2Int(numberOfColumns - i -1, numberOfRows -j -1);
                    grid[numberOfRows - j - 1, i].Index = new Vector2Int(numberOfRows - j -1, i);

                    rotatingTiles.Add(grid[i,j]);
                    rotatingTiles.Add(grid[j,numberOfRows - i - 1]);
                    rotatingTiles.Add(grid[numberOfColumns - i - 1, numberOfRows - j - 1]);
                    rotatingTiles.Add(grid[numberOfRows - j - 1, i]);

                }
            }
        }
        yield return RotateStack(rotatingTiles, GetBoardCenter(), direction == SwipeInfo.SwipeDirection.LEFT ? 90 : -90, .5f);
        

        rotatingGrid = false;
        yield return null;
    }

    private List<List<Tile>> CheckForMatches() {
        List<List<Tile>> matches = new List<List<Tile>>();
        bool[,] counted = new bool[numberOfColumns,numberOfRows];

        for(int i = 0; i < numberOfColumns; i++) {
            for(int j = 0; j < numberOfRows; j++) {
                if(!counted[i,j]) {
                    List<Tile> match = ValidateMatch(BFS(counted, i, j));
                    if(match != null) {
                        matches.Add(match);
                    }
                }
            }
        }
        return matches;
    }

    private List<Tile> BFS(bool[,] counted, int column, int row) {
        List<Tile> match = new  List<Tile>();
        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(grid[column, row]);
        while(queue.Count > 0) {
            for(int i = 0; i < queue.Count; i++) {
                Tile tile = queue.Dequeue();
                counted[tile.Index.x, tile.Index.y] = true;
                match.Add(grid[tile.Index.x, tile.Index.y]);
                
                foreach(Tile neighbor in GetNeighbors(tile.Index.x, tile.Index.y)) {
                    if(!counted[neighbor.Index.x, neighbor.Index.y] && 
                        grid[neighbor.Index.x, neighbor.Index.y].GetColor() == grid[column,row].GetColor()) {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
        return match;
    }

    private List<Tile> ValidateMatch(List<Tile> match) {
        if(match.Count < 3) {
            return null;
        }
        Dictionary<int, List<Tile>> columns = new Dictionary<int, List<Tile>>();
        Dictionary<int, List<Tile>> rows = new Dictionary<int, List<Tile>>();

        foreach(Tile tile in match) {
            if(!columns.ContainsKey(tile.Index.x)) {
                columns.Add(tile.Index.x, new List<Tile>());
            }
            columns[tile.Index.x].Add(tile);

            if(!rows.ContainsKey(tile.Index.y)) {
                rows.Add(tile.Index.y, new List<Tile>());
            }
            rows[tile.Index.y].Add(tile);
        }

        List<Tile> matchTiles = new List<Tile>();

        foreach(int col in columns.Keys) {
            if(columns[col].Count >= 3) {
                Debug.Log("Found a matching column");
                foreach(Tile tile in columns[col]) {
                    Debug.Log(tile);
                }
                matchTiles.AddRange(columns[col]);
            }
        }

        foreach(int row in rows.Keys) {
            if(rows[row].Count >= 3) {
                Debug.Log("Found a matching row");
                foreach(Tile tile in rows[row]) {
                    Debug.Log(tile);
                }
                matchTiles.AddRange(rows[row]);
            }
        }

        if(matchTiles.Count > 0) {
            return matchTiles;
        }
        return null;
    }
    
    private List<Tile> GetNeighbors(int column, int row) {
        List<Tile> neighbors = new List<Tile>();

        if(column - 1 >= 0) {
            neighbors.Add(grid[column - 1, row]);
        }
        if(column + 1 < numberOfColumns) {
            neighbors.Add(grid[column + 1, row]);
        }
        if(row - 1 >= 0) {
            neighbors.Add(grid[column, row - 1]);
        }            
        if(row + 1 < numberOfRows) {
            neighbors.Add(grid[column, row + 1]);
        }
        return neighbors;
    }

    private void SwapTiles(ref Tile a, ref Tile b) {
        Vector2Int tempIndex = a.Index;
        a.Index = b.Index;
        b.Index = tempIndex;
        grid[a.Index.x, a.Index.y] = a;
        grid[b.Index.x, b.Index.y] = b;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            RenderBoard();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            rotatingGrid = true;
            StartCoroutine(RotateGrid(SwipeInfo.SwipeDirection.LEFT));
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)) {
            rotatingGrid = true;
            StartCoroutine(RotateGrid(SwipeInfo.SwipeDirection.RIGHT));
        }
    }

    private Tile RenderTile(int col, int row, float cellWidth, float cellHeight, Color color) {
        Vector3 center = CalculateGamePosition(col, row, cameraBounds);
        Tile tile = (Instantiate(tilePrefab, center, Quaternion.identity) as Tile);
        tile.Initialize(cellWidth, cellHeight, color, new Vector2Int(col, row));
        tile.transform.parent = transform;
        return tile;
    }

    private Color RandomColor() {
        return colors[Random.Range(0, colors.Length)];
    }

    public void HandleSwipe(SwipeInfo swipe)
    {
        if (rotatingGrid ||
           swipe.Direction == SwipeInfo.SwipeDirection.UP || swipe.Direction == SwipeInfo.SwipeDirection.DOWN)
        {
            return;
        }


        rotatingGrid = true;
        StartCoroutine(RotateGrid(swipe.Direction));
    }
    
    public void EndSwipe() {
        rotatingGrid = false;
    }

    public Vector3 GetPosition(int column, int row) {
        return grid[column,row].Position;
    }

    public Vector3 GetPosition(Vector2Int index) {
        return GetPosition(index.x, index.y);
    }

    public Vector3 GetScreenPosition(int column, int row) {
        return cam.WorldToScreenPoint(grid[column,row].Position);
    }

    public Vector3 GetScreenPosition(Vector2Int index) {
        return GetScreenPosition(index.x, index.y);
    }

    private Vector3 CalculateGamePosition(int column, int row, Bounds bounds) {
            float xPosition = CalculateCoordinate(bounds.min.x, bounds.max.x, numberOfColumns, column);
            float yPosition = CalculateCoordinate(bounds.min.y, bounds.max.y, numberOfRows, row);
            return new Vector3(xPosition, yPosition, 1);
    }

    private float CalculateCoordinate(float minimum, float maximum, float totalNumber, float i) {
        float dimensionLength = maximum - minimum; // total grid dimensionLength
        float cellLength = dimensionLength / totalNumber;
        float cellCenter = minimum + cellLength / 2;
        return cellCenter + cellLength * i;
    }
    
    private Vector3 GetBoardCenter() {
        return grid[0,0].Position + (grid[numberOfColumns -1, numberOfRows -1].Position - grid[0,0].Position)/2;
    }

}
