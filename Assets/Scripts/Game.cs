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
            state[x,y].revelaed = true;
        }
    }
}
