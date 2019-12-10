using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveBacktrackingAlg : MazeAlgorithm
{
    private Stack<Direction> _lastDirections;
    private int _currRow, _currColumn;
    private Renderer _rend;

    public RecursiveBacktrackingAlg(MazeCell[,] mazeCells, float delay) : base(mazeCells, delay) { }


    //1. Choose a random cell to start.
    //2. Randomly choose an unvisited neighbouring cell and remove wall between them.
    //3. If all adjacent cells have been visited, back up to the previous cell.
    //4. Repeat step 2 and 3 until all cells have been visited

    /// <summary>
    /// Apply the Recursive Backracking algorithm to randomly generate a perfect maze.
    /// </summary>
    public override IEnumerator Generate()
    {
        WaitForSeconds delay = new WaitForSeconds(_stepDelay);
        _lastDirections = new Stack<Direction>();

        //1. Choose a random cell to start.
        _currRow = Random.Range(0, _mazeRows);
        _currColumn = Random.Range(0, _mazeColumns);
        _cells[_currRow, _currColumn].visited = true;
        _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
        _rend.material.color = Color.gray;

        while (!CourseComplete)
        {
            yield return delay;
            VisitNeighbour();
        }
    }

    /// <summary>
    /// Looks for visitable Neighbours and visits one randomly.
    /// </summary>
    private void VisitNeighbour()
    {
        int neighbCount = 0;
        int[] availableDirections = new int[4];

        //This set of four if-statements checks and counts all the available neighbours.
        if (_currRow > 0 && CellIsAvailable(_currRow - 1, _currColumn))
        {
            availableDirections[neighbCount] = (int)Direction.North;
            neighbCount++;
        }
        if (_currRow < _mazeRows && CellIsAvailable(_currRow + 1, _currColumn))
        {
            availableDirections[neighbCount] = (int)Direction.South;
            neighbCount++;
        }
        if (_currColumn > 0 && CellIsAvailable(_currRow, _currColumn - 1))
        {
            availableDirections[neighbCount] = (int)Direction.West;
            neighbCount++;
        }
        if (_currColumn < _mazeColumns && CellIsAvailable(_currRow, _currColumn + 1))
        {
            availableDirections[neighbCount] = (int)Direction.East;
            neighbCount++;
        }

        //If there were neighbours found, randomly choose one of the neighbours,
        //set the neighbour as visited, break the wall to the neighbour and make 
        //the neighbour the new current cell.
        if (neighbCount > 0)
        {
            int rand = Random.Range(0, neighbCount);
            Direction dir = (Direction)availableDirections[rand];
            _lastDirections.Push(dir);

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
            _rend.material.color = _rend.material.color == Color.gray ? Color.white : Color.gray;
        }
        //3. If all adjacent cells have been visited, back up to the previous cell.
        else if (_lastDirections.Count > 0)
        {
            _cells[_currRow, _currColumn].GetComponent<Renderer>().material.color = Color.white;
            Backtrack();
        }
        else
        {
            _cells[_currRow, _currColumn].GetComponent<Renderer>().material.color = Color.white;
            CourseComplete = true;
        }
    }
    
    /// <summary>
    /// Makes the last visited cell the current cell again.
    /// </summary>
    private void Backtrack()
    {
        Direction lastDirection = _lastDirections.Pop();
        if (lastDirection == Direction.North) { _currRow++; }
        if (lastDirection == Direction.South) { _currRow--; }
        if (lastDirection == Direction.East) { _currColumn--; }
        if (lastDirection == Direction.West) { _currColumn++; }
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
    /// Checks if the cell at the given location exists within the Maze and if the cell is unvisited.
    /// </summary>
    /// <param name="row">The row the cell is in.</param>
    /// <param name="column">The column the cell is in.</param>
    /// <returns>Visitability.</returns>
    private bool CellIsAvailable(int row, int column) =>
        row >= 0
        && row < _mazeRows
        && column >= 0
        && column < _mazeColumns
        && !_cells[row, column].visited;
}
