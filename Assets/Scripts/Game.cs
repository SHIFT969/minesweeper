using UnityEngine;

public class Game : MonoBehaviour
{
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;

    private Board board;
    private Cell[,] state;

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
            state[x,y].revealed = true;
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

                cell.revealed = true;
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

                if (x < 0 || x >= width || y < 0 || y >= height)
                    continue;

                if (state[x, y].type == Cell.Type.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
