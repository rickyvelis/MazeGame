using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    North,
    East,
    South,
    West
}

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

    public override IEnumerator Generate()
    {
        WaitForSeconds delay = new WaitForSeconds(_stepDelay);
        _lastDirections = new Stack<Direction>();


        //1. Choose a random cell to start.
        _currRow = Random.Range(0, _mazeRows);
        _currColumn = Random.Range(0, _mazeColumns);
        _cells[_currRow, _currColumn].visited = true;
        Debug.Log("Starting at " + _cells[_currRow, _currColumn].name);

        _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
        _rend.material.color = Color.gray;

        while (!CourseComplete)
        {
            yield return delay;
            VisitNeighbour();
        }
    }
    
    private bool VisitNeighbour()
    {
        bool success = false;
        int neighbCount = 0;
        int[] availableDirections = new int[4];

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

        if (neighbCount > 0)
        {
            success = true;

            int rand = Random.Range(0, neighbCount);
            Direction dir = (Direction)availableDirections[rand];
            _lastDirections.Push(dir);
            Debug.Log("GOING " + dir);

            if (dir == Direction.North)
            {
                _cells[_currRow - 1, _currColumn].visited = true;
                DestroyWallIfItExists(_cells[_currRow - 1, _currColumn].southWall);
                _currRow--;
            } 
            else if (dir == Direction.South)
            {
                _cells[_currRow + 1, _currColumn].visited = true;
                DestroyWallIfItExists(_cells[_currRow, _currColumn].southWall);
                _currRow++;
            }
            else if (dir == Direction.East)
            {
                _cells[_currRow, _currColumn + 1].visited = true;
                DestroyWallIfItExists(_cells[_currRow, _currColumn].eastWall);
                _currColumn++;
            }
            else if (dir == Direction.West)
            {
                _cells[_currRow, _currColumn - 1].visited = true;
                DestroyWallIfItExists(_cells[_currRow, _currColumn - 1].eastWall);
                _currColumn--;
            }
            _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
            if (_rend.material.color == Color.gray) _rend.material.color = Color.white;
            else _rend.material.color = Color.gray;
            Debug.Log("Visiting " + _cells[_currRow, _currColumn].name);

        }
        else if(_lastDirections.Count > 0)
        {
            _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
            _rend.material.color = Color.white;
            Backtrack();
        }
        else
        {
            _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
            _rend.material.color = Color.white;
            CourseComplete = true;
        }
        return success;
    }

    private void Backtrack()
    {
        Direction lastDirection = _lastDirections.Pop();
        if (lastDirection == Direction.North) { _currRow++; }
        if (lastDirection == Direction.South) { _currRow--; }
        if (lastDirection == Direction.East) { _currColumn--; }
        if (lastDirection == Direction.West) { _currColumn++; }
    }

    private void DestroyWallIfItExists(GameObject wall)
    {
        if (wall != null) GameObject.Destroy(wall);
    }

    private bool CellIsAvailable(int row, int column)
    {
        if (row >= 0 && row < _mazeRows && column >= 0 && column < _mazeColumns && !_cells[row, column].visited)
            return true;
        else
            return false;
    }
}
