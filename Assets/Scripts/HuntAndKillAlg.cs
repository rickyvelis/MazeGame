using System.Collections;
using UnityEngine;

public class HuntAndKillAlg : MazeAlgorithm
{
    private int _currX, _currY;
    private Renderer _rend;

    public HuntAndKillAlg(MazeCell[,] mazeCells, float delay) : base(mazeCells, delay) { }
    
    // 1. Choose a starting location.
    // 2. Perform a random walk, carving passages to unvisited neighbors, 
    //    until the current cell has no unvisited neighbors.
    // 3. Enter “hunt” mode, where you scan the grid looking for an unvisited 
    //    cell that is adjacent to a visited cell. If found, carve a passage 
    //    between the two and let the formerly unvisited cell be the new 
    //    starting location.
    // 4. Repeat steps 2 and 3 until the hunt mode scans the entire grid and 
    //    finds no unvisited cells.

    public override IEnumerator Generate()
    {
        //1. Choose a random cell to start.
        _currX = Random.Range(0, _mazeColumns);
        _currY = Random.Range(0, _mazeRows);
        _cells[_currX, _currY].visited = true;
        _rend = _cells[_currX, _currY].GetComponent<Renderer>();
        _rend.material.color = Color.white;

        while (!CourseComplete)
        {
            yield return _stepDelay;
            if (!Kill()) // Will return true until it hits a dead end.
            {
                yield return Hunt();  // Finds the next unvisited cell with an adjacent visited cell. If it can't find any, it sets courseComplete to true.
            }
        }
        Debug.Log("DONE GENERATING");
    }

    private bool Kill()
    {
        int neighbCount = 0;
        int[] availableDirections = new int[4];

        //This set of four if-statements checks and counts all the available neighbours.
        if (_currY < _mazeRows && CellIsAvailable(_currX, _currY + 1, false))
        {
            availableDirections[neighbCount] = (int)Direction.North;
            neighbCount++;
        }
        if (_currY > 0 && CellIsAvailable(_currX, _currY - 1, false))
        {
            availableDirections[neighbCount] = (int)Direction.South;
            neighbCount++;
        }
        if (_currX > 0 && CellIsAvailable(_currX - 1, _currY, false))
        {
            availableDirections[neighbCount] = (int)Direction.West;
            neighbCount++;
        }
        if (_currX < _mazeColumns && CellIsAvailable(_currX + 1, _currY, false))
        {
            availableDirections[neighbCount] = (int)Direction.East;
            neighbCount++;
        }

        //If there were neighbours found, randomly choose one of the neighbours,
        //set the neighbour as visited, break the wall to the neighbour and make 
        //the neighbour the new current cell.
        if (neighbCount > 0)
        {
            _rend = _cells[_currX, _currY].GetComponent<Renderer>();
            _rend.material.color = Color.white;

            int rand = Random.Range(0, neighbCount);
            Direction dir = (Direction)availableDirections[rand];

            switch (dir)
            {
                case Direction.North:
                    _cells[_currX, _currY + 1].visited = true;
                    DestroyWallIfItExists(_cells[_currX, _currY].northWall);
                    _currY++;
                    break;
                case Direction.South:
                    _cells[_currX, _currY - 1].visited = true;
                    DestroyWallIfItExists(_cells[_currX, _currY - 1].northWall);
                    _currY--;
                    break;
                case Direction.East:
                    _cells[_currX + 1, _currY].visited = true;
                    DestroyWallIfItExists(_cells[_currX, _currY].eastWall);
                    _currX++;
                    break;
                case Direction.West:
                    _cells[_currX - 1, _currY].visited = true;
                    DestroyWallIfItExists(_cells[_currX - 1, _currY].eastWall);
                    _currX--;
                    break;
            }
            _rend = _cells[_currX, _currY].GetComponent<Renderer>();
            _rend.material.color = Color.green;
            return true;
        }
        else
        {
            _rend = _cells[_currX, _currY].GetComponent<Renderer>();
            _rend.material.color = Color.white;
            return false;
        }
    }

    private IEnumerator Hunt()
    {
        CourseComplete = true; // Set it to this, and see if we can prove otherwise below.
        int lastX = int.MaxValue;
        int lastY = int.MaxValue;

        for (int y = _mazeRows - 1; y >= 0; y--) // From top to bottom
        {
            for (int x = 0; x < _mazeColumns; x++) // From left to right
            {
                _rend = _cells[x, y].GetComponent<Renderer>();
                _rend.material.color = Color.yellow;

                if (lastX < _mazeColumns && lastY < _mazeRows)
                {
                    _rend = _cells[lastX, lastY].GetComponent<Renderer>();
                    if (_cells[lastX, lastY].visited) _rend.material.color = Color.white;
                    else _rend.material.color = _cells[lastX, lastY].startColor;
                }

                lastX = x;
                lastY = y;


                if (!_cells[x, y].visited)
                {
                    int neighbCount = 0;
                    Direction[] availableDirections = new Direction[4];

                    //This set of four if-statements checks and counts all the available neighbours.
                    if (y < _mazeRows && CellIsAvailable(x, y + 1, true))
                        availableDirections[neighbCount++] = Direction.North;
                    if (y > 0 && CellIsAvailable(x, y - 1, true))
                        availableDirections[neighbCount++] = Direction.South;
                    if (x > 0 && CellIsAvailable(x - 1, y, true))
                        availableDirections[neighbCount++] = Direction.West;
                    if (x < _mazeColumns && CellIsAvailable(x + 1, y, true))
                        availableDirections[neighbCount++] = Direction.East;

                    if (neighbCount > 0)
                    {
                        CourseComplete = false; // Yep, we found something so definitely do another Kill cycle.
                        _currX = x;
                        _currY = y;
                        _cells[_currX, _currY].visited = true;
                        _rend = _cells[_currX, _currY].GetComponent<Renderer>();
                        _rend.material.color = Color.white;

                        int rand = Random.Range(0, neighbCount);
                        Direction dir = availableDirections[rand];

                        if(dir == Direction.North)
                            DestroyWallIfItExists(_cells[_currX, _currY].northWall);
                        else if(dir == Direction.South)
                            DestroyWallIfItExists(_cells[_currX, _currY - 1].northWall);
                        else if(dir == Direction.East)
                            DestroyWallIfItExists(_cells[_currX, _currY].eastWall);
                        else if(dir == Direction.West)
                            DestroyWallIfItExists(_cells[_currX - 1, _currY].eastWall);

                        yield break;
                    }
                }
                yield return _stepDelay;
            }
        }
        _rend = _cells[_mazeColumns - 1, 0].GetComponent<Renderer>();
        _rend.material.color = Color.white;
    }

    /// <summary>
    /// Destroys the given wall, if it exists.
    /// </summary>
    /// <param name="wall">The wall GameObject to be destroyed.</param>
    private void DestroyWallIfItExists(GameObject wall)
    {
        if (wall != null) Object.Destroy(wall);
    }

    /// <summary>
    /// Checks if the cell at the given location is within the Maze and if the cell is unvisited or not.
    /// </summary>
    /// <param name="x">The row the cell is in.</param>
    /// <param name="y">The column the cell is in.</param>
    /// <param name="visited">Condition if the cell must be visited or not.</param>
    /// <returns>Visitability.</returns>
    private bool CellIsAvailable(int x, int y, bool visited) =>
        x >= 0
        && x < _mazeColumns
        && y >= 0
        && y < _mazeRows
        && _cells[x, y].visited == visited;
}