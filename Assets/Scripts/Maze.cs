using TMPro;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public TMP_InputField InputX, InputY;
    public MazeCell Cell;
    public GameObject Wall;
    public int SizeX, SizeY;
    public float Size = 1.0f;
    public float GenerationStepDelay;

    private MazeCell[,] _cells;
    private GameObject _mazeContainer;

    private MazeAlgorithm _ma;

    private bool _firstTimeGenerate = true;
    

    public void StartGame()
    {
        if (!int.TryParse(InputX.text, out SizeX)) SizeX = 5;
        if (!int.TryParse(InputY.text, out SizeY)) SizeY = 5;
        if (SizeX == 0) SizeX = 2;
        if (SizeY == 0) SizeY = 2;

        if (_firstTimeGenerate)
        {
            BeginGame();
            _firstTimeGenerate = false;
        }
        else
            RestartGame();
    }

    public void Update()
    {
        if (!_firstTimeGenerate)
            if (_ma.CourseComplete)
            {
                //Play the actual game
            }
    }

    public void BeginGame()
    {
        CreateGrid();
        //_ma = new RecursiveBacktrackingAlg(_cells, GenerationStepDelay);
        _ma = new HuntAndKillAlg(_cells, GenerationStepDelay);
        StartCoroutine(_ma.Generate());

        ConfigureOrthCam();
        transform.position = new Vector3((Size * SizeX / 2) - Size / 2, 0.0f, (Size * SizeY / 2) - Size / 2);
    }

    public void RestartGame()
    {
        GameObject.Destroy(_mazeContainer);
        StopAllCoroutines();
        BeginGame();
    }

    /// <summary>
    /// Adjusts the Size and farClipPlane properties of the Camera, so the maze will fill the screen correctly.
    /// </summary>
    private void ConfigureOrthCam()
    {
        float mazeSizeRatio = SizeX / SizeY;

        //If mazeSizeRatio is bigger than the Camera's aspect ratio, base the Camera's Orthograpic size on the wisth (SizeX) of the Maze.
        if (mazeSizeRatio > Camera.main.aspect)
            Camera.main.orthographicSize = (float)SizeX / 2;
        else
            Camera.main.orthographicSize = (float)SizeY / 2 / Camera.main.aspect;

        //Sets Camera's Far Clipping Plane depending on the biggest Dimension of the maze
        if (SizeX > SizeY)
            Camera.main.farClipPlane = (float)SizeX * 2;
        else
            Camera.main.farClipPlane = (float)SizeY * 2;
    }

    /// <summary>
    /// Creates and instantiates every Cell and Wall object to create a grid.
    /// </summary>
    private void CreateGrid()
    {
        _mazeContainer = new GameObject() { name = "MazeContainer" };
        _cells = new MazeCell[SizeX, SizeY];

        for (int x = 0; x < SizeX; x++)
            for (int y = 0; y < SizeY; y++)
            {
                _cells[x, y] = Instantiate(Cell, new Vector3(x * Size, 0, y * Size), Quaternion.Euler(90.0f, 0.0f, 0.0f), _mazeContainer.transform) as MazeCell;
                _cells[x, y].name = "Cell " + x + "," + y;

                if (y == 0)
                {
                    _cells[x, y].westWall = Instantiate(Wall, new Vector3(x * Size, 0.5f, (y * Size) - (Size / 2f)), Quaternion.identity, _cells[x, y].transform) as GameObject;
                    _cells[x, y].westWall.name = "West Wall " + x + "," + y;
                }
                _cells[x, y].eastWall = Instantiate(Wall, new Vector3(x * Size, 0.5f, (y * Size) + (Size / 2f)), Quaternion.identity, _cells[x, y].transform) as GameObject;
                _cells[x, y].eastWall.name = "East Wall " + x + "," + y;
                if (x == 0)
                {
                    _cells[x, y].northWall = Instantiate(Wall, new Vector3((x * Size) - (Size / 2f), 0.5f, y * Size), Quaternion.Euler(0.0f, 90.0f, 0.0f), _cells[x, y].transform) as GameObject;
                    _cells[x, y].northWall.name = "North Wall " + x + "," + y;
                }
                _cells[x, y].southWall = Instantiate(Wall, new Vector3((x * Size) + (Size / 2f), 0.5f, y * Size), Quaternion.Euler(0.0f, 90.0f, 0.0f), _cells[x, y].transform) as GameObject;
                _cells[x, y].southWall.name = "South Wall " + x + "," + y;
            }
    }


}
