using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{
    // User Input objects
    public TMP_InputField InputX, InputY;
    public TMP_Dropdown DropdownAlg;
    public Slider DelaySlider;

    // Game objects
    public MazeCell Cell;
    public GameObject Wall;
    public GameObject Pellet;
    public Player Player;
    public CameraScript Cam;

    // Maze variables
    public int SizeX, SizeY;
    public float GenerationStepDelay = 0;
    public float Size = 1.0f;

    // Private variables
    private MazeCell[,] _cells;
    private GameObject _mazeContainer;
    private GameObject _spawnedPellet;
    private Player _spawnedPlayer;
    private MazeAlgorithm _ma;
    private bool _firstTimeGenerate = true;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        // Add OnValueChanged event listener to the DelaySlider, so its changes can be detected by this class.
        DelaySlider.onValueChanged.AddListener(delegate { SetStepDelay(DelaySlider.value); });
    }

    /// <summary>
    /// Start the game. 
    /// </summary>
    public void StartGame()
    {
        // Get the given X and Y sizes of the maze from the InputFields
        if (!int.TryParse(InputX.text, out SizeX)) SizeX = 5;
        if (!int.TryParse(InputY.text, out SizeY)) SizeY = 5;
        if (SizeX < 2) SizeX = 2;
        if (SizeY < 2) SizeY = 2;

        // If this is the first time a maze is generated, run SetupMaze().
        if (_firstTimeGenerate)SetupMaze();
        else RestartGame(false);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    public void Update()
    {
        // If the maze exists and is done generating, stop all coroutines and instantiate new Pellet and Player. 
        // And start checking if Player has won or not.
        if (_ma != null && _ma.CourseComplete)
        {
            StopAllCoroutines();
            if (_spawnedPellet == null) InstantiatePellet();
            if (_spawnedPlayer == null) InstantiatePlayer();

            //Restart game when player has won.
            if (_spawnedPlayer.Won) RestartGame(true);

        }
    }

    /// <summary>
    /// Setup the maze for the game to be played on.
    /// </summary>
    public void SetupMaze()
    {
        // A maze is going to be generated, so _firstTimeGenerate should be false.
        _firstTimeGenerate = false;

        // Create a grid of walls and cells.
        CreateGrid();

        // Pick an algorithm for generating the maze.
        AlgorithmPicker(DropdownAlg.value);

        // Generate the maze with the chosen algorithm.
        if (_ma != null) StartCoroutine(_ma.Generate());

        // Adjust the size and far clipping plane of the orthographic camera.
        Cam.ConfigureOrthCam(SizeX, SizeY);

        // Set the position of GameManager to the center of the generated maze, so the camera (its child) will be at the center as well.
        transform.position = new Vector3((Size * SizeX / 2) - Size / 2, 0.0f, (Size * SizeY / 2) - Size / 2);
    }

    /// <summary>
    /// Regenerate the maze and restart the game.
    /// </summary>
    public void RestartGame(bool increaseSize)
    {
        //Destroy the maze, the pellet and the player if they exist.
        if(_mazeContainer != null) Destroy(_mazeContainer);
        if (_spawnedPellet != null) Destroy(_spawnedPellet);
        if (_spawnedPlayer != null) Destroy(_spawnedPlayer.gameObject);

        //Stop all the running coroutines
        StopAllCoroutines();

        //increase size of the maze by 1 (happens when Player has reached Pellet).
        if (increaseSize)
        {
            SizeX += 1;
            SizeY += 1;
        }

        //Generate a new maze.
        SetupMaze();
    }

    /// <summary>
    /// Instantiates a new Pellet at a random location in the maze.
    /// </summary>
    public void InstantiatePellet()
    {
        int randX = Random.Range(0, SizeX);
        int randY = Random.Range(0, SizeY);
        _spawnedPellet = Instantiate(Pellet, new Vector3(randX, 0, randY), Quaternion.identity) as GameObject;
    }

    /// <summary>
    /// Instantiates a new Player at a random location in the maze.
    /// </summary>
    public void InstantiatePlayer()
    {
        int randX = Random.Range(0, SizeX);
        int randY = Random.Range(0, SizeY);
        _spawnedPlayer = Instantiate(Player, new Vector3(randX, 0, randY), Quaternion.identity) as Player;
    }

    /// <summary>
    /// Creates and instantiates every Cell and Wall object to create a grid.
    /// </summary>
    private void CreateGrid()
    {
        _mazeContainer = new GameObject() { name = "MazeContainer" };
        _cells = new MazeCell[SizeX, SizeY];

        //double for-loop where each Cell is being Instantiated with its Walls.
        for (int x = 0; x < SizeX; x++)
            for (int y = 0; y < SizeY; y++)
            {
                _cells[x, y] = Instantiate(Cell, new Vector3(x * Size, 0, y * Size), Quaternion.Euler(90.0f, 0.0f, 0.0f), _mazeContainer.transform) as MazeCell;
                _cells[x, y].name = "Cell " + x + "," + y;

                //only add a South Wall to the Cell if the Cell is at the first row/
                if (y == 0)
                {
                    _cells[x, y].SouthWall = Instantiate(Wall, new Vector3(x * Size, 0.5f, (y * Size) - (Size / 2f)), Quaternion.identity, _cells[x, y].transform) as GameObject;
                    _cells[x, y].SouthWall.name = "South Wall " + x + "," + y;
                }

                _cells[x, y].NorthWall = Instantiate(Wall, new Vector3(x * Size, 0.5f, (y * Size) + (Size / 2f)), Quaternion.identity, _cells[x, y].transform) as GameObject;
                _cells[x, y].NorthWall.name = "North Wall " + x + "," + y;

                //only add a West Wall to the Cell if the Cell is at the first row/
                if (x == 0)
                {
                    _cells[x, y].WestWall = Instantiate(Wall, new Vector3((x * Size) - (Size / 2f), 0.5f, y * Size), Quaternion.Euler(0.0f, 90.0f, 0.0f), _cells[x, y].transform) as GameObject;
                    _cells[x, y].WestWall.name = "West Wall " + x + "," + y;
                }

                _cells[x, y].EastWall = Instantiate(Wall, new Vector3((x * Size) + (Size / 2f), 0.5f, y * Size), Quaternion.Euler(0.0f, 90.0f, 0.0f), _cells[x, y].transform) as GameObject;
                _cells[x, y].EastWall.name = "East Wall " + x + "," + y;
            }
    }

    /// <summary>
    /// Picks the algorithm to be used to generate a maze with.
    /// </summary>
    /// <param name="index">The index given to an algorithm.</param>
    private void AlgorithmPicker(int index)
    {
        switch (index)
        {
            case 0:
                _ma = new RecursiveBacktrackingAlg(_cells, GenerationStepDelay);
                break;
            case 1:
                _ma = new HuntAndKillAlg(_cells, GenerationStepDelay);
                break;
            //case 2:
            //    return _ma = new ThirdNonexistantAlgorithm(_cells, GenerationStepDelay);
            default:
                Debug.LogError("No algorithm selected.");
                break;
        }
    }

    /// <summary>
    /// Sets the maze generation step delay in seconds.
    /// </summary>
    public void SetStepDelay(float delay)
    {
        GenerationStepDelay = delay;
        _ma.StepDelay = new WaitForSeconds(delay);
    }
}