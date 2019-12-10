using System.Collections;
using UnityEngine;

public class HuntAndKillAlg : MazeAlgorithm
{
    private int _currRow, _currColumn;
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
        WaitForSeconds delay = new WaitForSeconds(_stepDelay);

        //1. Choose a random cell to start.
        _currRow = Random.Range(0, _mazeRows);
        _currColumn = Random.Range(0, _mazeColumns);
        _cells[_currRow, _currColumn].visited = true;
        _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
        _rend.material.color = Color.white;

        while (!CourseComplete)
        {
            yield return delay;
            if (!Kill()) // Will return true until it hits a dead end.
            {
                Hunt();  // Finds the next unvisited cell with an adjacent visited cell. If it can't find any, it sets courseComplete to true.
            }
        }
    }

    private bool Kill()
    {
        int neighbCount = 0;
        int[] availableDirections = new int[4];

        //This set of four if-statements checks and counts all the available neighbours.
        if (_currRow > 0 && CellIsAvailable(_currRow - 1, _currColumn, false))
        {
            availableDirections[neighbCount] = (int)Direction.North;
            neighbCount++;
        }
        if (_currRow < _mazeRows && CellIsAvailable(_currRow + 1, _currColumn, false))
        {
            availableDirections[neighbCount] = (int)Direction.South;
            neighbCount++;
        }
        if (_currColumn > 0 && CellIsAvailable(_currRow, _currColumn - 1, false))
        {
            availableDirections[neighbCount] = (int)Direction.West;
            neighbCount++;
        }
        if (_currColumn < _mazeColumns && CellIsAvailable(_currRow, _currColumn + 1, false))
        {
            availableDirections[neighbCount] = (int)Direction.East;
            neighbCount++;
        }

        //If there were neighbours found, randomly choose one of the neighbours,
        //set the neighbour as visited, break the wall to the neighbour and make 
        //the neighbour the new current cell.
        if (neighbCount > 0)
        {
            _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
            _rend.material.color = Color.white;

            int rand = Random.Range(0, neighbCount);
            Direction dir = (Direction)availableDirections[rand];

            switch (dir)
            {
                case Direction.North:
                    _cells[_currRow - 1, _currColumn].visited = true;
                    DestroyWallIfItExists(_cells[_currRow - 1, _currColumn].southWall);
                    _currRow--;
                    break;
                case Direction.South:
                    _cells[_currRow + 1, _currColumn].visited = true;
                    DestroyWallIfItExists(_cells[_currRow, _currColumn].southWall);
                    _currRow++;
                    break;
                case Direction.East:
                    _cells[_currRow, _currColumn + 1].visited = true;
                    DestroyWallIfItExists(_cells[_currRow, _currColumn].eastWall);
                    _currColumn++;
                    break;
                case Direction.West:
                    _cells[_currRow, _currColumn - 1].visited = true;
                    DestroyWallIfItExists(_cells[_currRow, _currColumn - 1].eastWall);
                    _currColumn--;
                    break;
            }
            _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
            _rend.material.color = Color.green;

            return true;
        }
        else
        {
            _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
            _rend.material.color = Color.white;
            return false;
        }
    }

    private void Hunt()
    {
        CourseComplete = true; // Set it to this, and see if we can prove otherwise below.

        for (int x = 0; x < _mazeRows; x++)
        {
            for (int y = 0; y < _mazeColumns; y++)
            {
                if (!_cells[x, y].visited)
                {
                    int neighbCount = 0;
                    Direction[] availableDirections = new Direction[4];

                    //This set of four if-statements checks and counts all the available neighbours.
                    if (x > 0 && CellIsAvailable(x - 1, y, true))
                    {
                        availableDirections[neighbCount] = Direction.North;
                        neighbCount++;
                    }
                    if (x < _mazeRows && CellIsAvailable(x + 1, y, true))
                    {
                        availableDirections[neighbCount] = Direction.South;
                        neighbCount++;
                    }
                    if (y > 0 && CellIsAvailable(x, y - 1, true))
                    {
                        availableDirections[neighbCount] = Direction.West;
                        neighbCount++;
                    }
                    if (y < _mazeColumns && CellIsAvailable(x, y + 1, true))
                    {
                        availableDirections[neighbCount] = Direction.East;
                        neighbCount++;
                    }

                    if (neighbCount > 0)
                    {
                        CourseComplete = false; // Yep, we found something so definitely do another Kill cycle.
                        _currRow = x;
                        _currColumn = y;
                        _cells[_currRow, _currColumn].visited = true;
                        _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
                        _rend.material.color = Color.white;

                        int rand = Random.Range(0, neighbCount);
                        Direction dir = availableDirections[rand];

                        if(dir == Direction.North)
                            DestroyWallIfItExists(_cells[_currRow - 1, _currColumn].southWall);
                        else if(dir == Direction.South)
                            DestroyWallIfItExists(_cells[_currRow, _currColumn].southWall);
                        else if(dir == Direction.East)
                            DestroyWallIfItExists(_cells[_currRow, _currColumn].eastWall);
                        else if(dir == Direction.West)
                            DestroyWallIfItExists(_cells[_currRow, _currColumn - 1].eastWall);

                        return;
                    }
                }
            }
        }
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
    /// <param name="row">The row the cell is in.</param>
    /// <param name="column">The column the cell is in.</param>
    /// <param name="visited">Condition if the cell must be visited or not.</param>
    /// <returns>Visitability.</returns>
    private bool CellIsAvailable(int row, int column, bool visited) =>
        row >= 0
        && row < _mazeRows
        && column >= 0
        && column < _mazeColumns
        && _cells[row, column].visited == visited;
}