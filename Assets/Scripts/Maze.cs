using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public int SizeX, SizeY;
    public float Size = 1.0f;
    public MazeCell Cell;
    public GameObject Wall;
    public float GenerationStepDelay;

    private MazeCell[,] _cells;
    private GameObject _mazeContainer;

    private MazeAlgorithm _ma;

    //VARS vor camera


    public void Start()
    {
        BeginGame();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) RestartGame();
    }

    public void BeginGame()
    {
        CreateGrid();
        _ma = new RecursiveBacktrackingAlg(_cells, GenerationStepDelay);
        //_ma = new HuntAndKillMazeAlgorithm(_cells, GenerationStepDelay);
        StartCoroutine(_ma.Generate());
        if (_ma.CourseComplete)
        {
            //Play the actual game
        }

        SetOrthCamSize();
        transform.position = new Vector3((Size * SizeX / 2) - Size/2, 0.0f, (Size * SizeY / 2) - Size / 2);
    }

    void SetOrthCamSize()
    {
        float mazeSizeRatio = (float)SizeX / (float)SizeY;

        //Sets Orthographio Camera size based on Maze Size
        if (mazeSizeRatio < Camera.main.aspect)
            Camera.main.orthographicSize = (float)SizeY/2;
        else
            Camera.main.orthographicSize = ((float)SizeX/2) / Camera.main.aspect;

        if (SizeX > SizeY)
            Camera.main.farClipPlane = (float)SizeX * 2;
        else
            Camera.main.farClipPlane = (float)SizeY * 2;
    }

    public void RestartGame()
    {
        GameObject.Destroy(_mazeContainer);
        StopAllCoroutines();
        BeginGame();
    }

    private void CreateGrid()
    {
        _mazeContainer = new GameObject() { name = "MazeContainer" };
        _cells = new MazeCell[SizeX, SizeY];
        
        for (int x = 0; x < SizeX; x++)
        {
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


}
