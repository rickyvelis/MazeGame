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

    /// <summary>
    /// Apply the Recursive Backracking algorithm to randomly generate a perfect maze.
    /// </summary>
    public override IEnumerator Generate()
    {
        // Choose a random Cell to start.
        _currX = Random.Range(0, _mazeColumns);
        _currY = Random.Range(0, _mazeRows);

        // Set the random Cell to visited.
        _cells[_currX, _currY].Visited = true;

        // Set material color of the starting Cell to white.
        _rend = _cells[_currX, _currY].GetComponent<Renderer>();
        _rend.material.color = Color.white;

        // Begin Maze Generation loop.
        while (!CourseComplete)
        {
            // Suspend coroutine for given amount of seconds.
            yield return StepDelay;

            // Run Kill() until it hits a dead end.
            if (!Kill())
            {
                // Scan grid for next unvisited Cell with an adjecent visited Cell. If it can't find any, it sets courseComplete to true.
                yield return Hunt();
            }
        }
    }

    /// <summary>
    /// Find unvisited adjecent Cells and visit random one.
    /// </summary>
    /// <returns>False if dead end has been hit.</returns>
    private bool Kill()
    {
        int neighbCount = 0;
        int[] availableDirections = new int[4];

        // Set of four if-statements that looks for adjecent unvisited Cells.
        if (_currY < _mazeRows && CellIsAvailable(_currX, _currY + 1, false))
            availableDirections[neighbCount++] = (int)Direction.North;
        if (_currY > 0 && CellIsAvailable(_currX, _currY - 1, false))
            availableDirections[neighbCount++] = (int)Direction.South;
        if (_currX > 0 && CellIsAvailable(_currX - 1, _currY, false))
            availableDirections[neighbCount++] = (int)Direction.West;
        if (_currX < _mazeColumns && CellIsAvailable(_currX + 1, _currY, false))
            availableDirections[neighbCount++] = (int)Direction.East;

        // If there are unvisited adjecent Cells found, randomly choose one,
        // set that Cell as visited, break the wall to that Cell and make 
        // that Cell the new current cell.
        if (neighbCount > 0)
        {
            // Turn current Cell white.
            _rend = _cells[_currX, _currY].GetComponent<Renderer>();
            _rend.material.color = Color.white;

            // Randomly choose an adjecent unvisited Cell, destroy wall between them and 
            // make the new Cell the current Cell.
            int rand = Random.Range(0, neighbCount);
            Direction dir = (Direction)availableDirections[rand];
            switch (dir)
            {
                case Direction.North:
                    _cells[_currX, _currY + 1].Visited = true;
                    DestroyWallIfItExists(_cells[_currX, _currY].NorthWall);
                    _currY++;
                    break;
                case Direction.South:
                    _cells[_currX, _currY - 1].Visited = true;
                    DestroyWallIfItExists(_cells[_currX, _currY - 1].NorthWall);
                    _currY--;
                    break;
                case Direction.East:
                    _cells[_currX + 1, _currY].Visited = true;
                    DestroyWallIfItExists(_cells[_currX, _currY].EastWall);
                    _currX++;
                    break;
                case Direction.West:
                    _cells[_currX - 1, _currY].Visited = true;
                    DestroyWallIfItExists(_cells[_currX - 1, _currY].EastWall);
                    _currX--;
                    break;
            }
            // Turn the new current Cell Green
            _rend = _cells[_currX, _currY].GetComponent<Renderer>();
            _rend.material.color = Color.green;

            // Return true, because an adjecent Cell has been found.
            return true;
        }
        else
        {
            // Turn current Cell white.
            _rend = _cells[_currX, _currY].GetComponent<Renderer>();
            _rend.material.color = Color.white;

            // Return false, because we hit a dead end.
            return false;
        }
    }

    /// <summary>
    /// Scan the grid for an unvisited Cell with an adjecent visited Cell, visit that Cell and remove the Wall between them.
    /// </summary>
    private IEnumerator Hunt()
    {
        int lastX = int.MaxValue;
        int lastY = int.MaxValue;

        for (int y = _mazeRows - 1; y >= 0; y--) // From top to bottom.
        {
            for (int x = 0; x < _mazeColumns; x++) // From left to right.
            {
                // Turn Cell that is being scanned to Yellow.
                _rend = _cells[x, y].GetComponent<Renderer>();
                _rend.material.color = Color.yellow;

                // Turn last scanned Cell back to its original color.
                if (lastX < _mazeColumns && lastY < _mazeRows)
                {
                    _rend = _cells[lastX, lastY].GetComponent<Renderer>();
                    if (_cells[lastX, lastY].Visited) _rend.material.color = Color.white;
                    else _rend.material.color = _cells[lastX, lastY].StartColor;
                }

                // Remember this Cell for next iteration of for-loop.
                lastX = x;
                lastY = y;

                // If current scanned Cell is unvisited, look for visited adjecent Cells.
                if (!_cells[x, y].Visited)
                {
                    int neighbCount = 0;
                    Direction[] availableDirections = new Direction[4];

                    // Set of four if-statements that looks for adjecent unvisited Cells.
                    if (y < _mazeRows && CellIsAvailable(x, y + 1, true))
                        availableDirections[neighbCount++] = Direction.North;
                    if (y > 0 && CellIsAvailable(x, y - 1, true))
                        availableDirections[neighbCount++] = Direction.South;
                    if (x > 0 && CellIsAvailable(x - 1, y, true))
                        availableDirections[neighbCount++] = Direction.West;
                    if (x < _mazeColumns && CellIsAvailable(x + 1, y, true))
                        availableDirections[neighbCount++] = Direction.East;

                    // If there are visited adjecent Cells found, randomly choose one,
                    // set current scanned Cell as visited and make it the current Cell, 
                    // break the Wall between current Cell and randomly chosen adjecent visited Cell.
                    if (neighbCount > 0)
                    {
                        _currX = x;
                        _currY = y;
                        _cells[_currX, _currY].Visited = true;

                        // Turn current scanned Cell white.
                        _rend = _cells[_currX, _currY].GetComponent<Renderer>();
                        _rend.material.color = Color.white;

                        // Randomly choose adjecent visited Cell and break wall between them.
                        int rand = Random.Range(0, neighbCount);
                        Direction dir = availableDirections[rand];
                        if(dir == Direction.North)
                            DestroyWallIfItExists(_cells[_currX, _currY].NorthWall);
                        else if(dir == Direction.South)
                            DestroyWallIfItExists(_cells[_currX, _currY - 1].NorthWall);
                        else if(dir == Direction.East)
                            DestroyWallIfItExists(_cells[_currX, _currY].EastWall);
                        else if(dir == Direction.West)
                            DestroyWallIfItExists(_cells[_currX - 1, _currY].EastWall);

                        // Suspend coroutine.
                        yield break;
                    }
                }
                // Suspend coroutine for given amount of seconds.
                yield return StepDelay;
            }
        }
        // Turn last Cell White.
        _rend = _cells[_mazeColumns - 1, 0].GetComponent<Renderer>();
        _rend.material.color = Color.white;

        // Set CourseComplete to true, because every Cell in the grid has been scanned and no
        // unvisited Cell has been found.
        CourseComplete = true;
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
        && _cells[x, y].Visited == visited;
}