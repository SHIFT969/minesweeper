using Unity.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;
    
    private bool gameOver;
    private Board board;
    private Cell[,] state;

    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 1, width * height);
    }

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        gameOver = false;
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);

        state = new Cell[width, height];

        GenerateCells();
        GenerateMines();
        GenerateNumbers();

        board.Draw(state);
    }

    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = new Cell
                {
                    position = new Vector3Int(x, y, 0),
                    type = Cell.Type.Empty,
                };
                state[x, y] = cell;
            }
        }
    }

    private void GenerateMines()
    {
        for (int i = 0; i < mineCount; i++)
        {
            var x = Random.Range(0, width);
            var y = Random.Range(0, height);

            while (state[x,y].type == Cell.Type.Mine)
            {
                x = Random.Range(0, width);
                y = Random.Range(0, height);
            }

            state[x,y].type = Cell.Type.Mine;
            //state[x,y].revealed = true;
        }
    }

    private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = state[x,y];

                if (cell.type == Cell.Type.Mine)
                    continue;

                cell.number = CountMines(x, y);
                if (cell.number > 0)
                {
                    cell.type = Cell.Type.Number;
                }

                //cell.revealed = true;
                state[x, y] = cell;
            }
        }
    }

    private int CountMines(int cellX, int cellY)
    {
        var count = 0;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                    continue;

                var x = cellX + adjacentX;
                var y = cellY + adjacentY;

                if (GetCell(x, y).type == Cell.Type.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void Update()
    {
        if (this.gameOver && Input.GetMouseButtonDown(0)) {
            NewGame();
            return;
        }

        if (Input.GetMouseButtonDown(1)) {
            Flag();
        } else if (Input.GetMouseButtonDown(0)) {
            Reveal();
        }
    }

    private void Reveal()
    {
        var cell = GetCurrentCell();

        if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged) {
            return;
        }

        switch (cell.type)
        {
            case Cell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;
            case Cell.Type.Mine:
                Explode(cell);
                break;
            default:
                cell.revealed = true;
                state[cell.position.x, cell.position.y] = cell;
                CheckWinCondition();
                break;
        }
        board.Draw(state);
    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = state[x, y];

                if (cell.type != Cell.Type.Mine && !cell.revealed) {
                    return;
                }
            }
        }

        Debug.Log("You have won!");
        gameOver = true;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    state[cell.position.x, cell.position.y] = cell;
                }
            }
        }

        board.Draw(state);
    }

    private void Explode(Cell cell)
    {
        Debug.Log("Game Over!");
        gameOver = true;

        cell.revealed = true;
        cell.exploded = true;
        state[cell.position.x, cell.position.y] = cell;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    state[cell.position.x, cell.position.y] = cell;
                }
            }
        }
        board.Draw(state);
    }

    private void Flood(Cell cell)
    {
        if (cell.revealed || cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid)
            return;

        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        if (cell.type == Cell.Type.Empty) {
            Flood(GetCell(cell.position.x, cell.position.y + 1));
            Flood(GetCell(cell.position.x + 1, cell.position.y + 1));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
            Flood(GetCell(cell.position.x - 1, cell.position.y - 1));
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x - 1, cell.position.y + 1));
        }
    }

    private void Flag()
    {
        var cell = GetCurrentCell();

        if (cell.type == Cell.Type.Invalid || cell.revealed) {
            return;
        }

        cell.flagged = !cell.flagged;
        state[cell.position.x, cell.position.y] = cell;
        board.Draw(state);
    }

    private Cell GetCurrentCell()
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        return GetCell(cellPosition.x, cellPosition.y);
    }

    private Cell GetCell(int x, int y)
    {
        if (IsValid(x, y))
            return state[x, y];
        else
            return new Cell();
    }

    private bool IsValid(int x, int y)
    {
        return !(x < 0 || x >= width || y < 0 || y >= height);
    }
}
