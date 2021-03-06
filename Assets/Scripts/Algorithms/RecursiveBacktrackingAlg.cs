﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveBacktrackingAlg : MazeAlgorithm
{
    private Stack<Direction> _lastDirections;
    private int _currX, _currY;
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
        _lastDirections = new Stack<Direction>();

        // Choose a random Cell to start.
        _currX = Random.Range(0, _mazeColumns);
        _currY = Random.Range(0, _mazeRows);

        // Set the random Cell to visited.
        _cells[_currX, _currY].Visited = true;

        // Set material color of the starting Cell gray.
        _rend = _cells[_currX, _currY].GetComponent<Renderer>();
        _rend.material.color = Color.gray;

        // Begin Maze Generation loop.
        while (!CourseComplete)
        {
            // Suspend coroutine for given amount of seconds.
            yield return StepDelay;

            //Visit neighbours until there are no neighbours left.
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
        if (_currY < _mazeRows && CellIsAvailable(_currX, _currY + 1))
            availableDirections[neighbCount++] = (int)Direction.North;
        if (_currY > 0 && CellIsAvailable(_currX, _currY - 1))
            availableDirections[neighbCount++] = (int)Direction.South;
        if (_currX > 0 && CellIsAvailable(_currX - 1, _currY))
            availableDirections[neighbCount++] = (int)Direction.West;
        if (_currX < _mazeColumns && CellIsAvailable(_currX + 1, _currY))
            availableDirections[neighbCount++] = (int)Direction.East;

        //If there were neighbours found, randomly choose one of the neighbours,
        //set the neighbour as visited, break the wall to the neighbour and make 
        //the neighbour the new current cell.
        if (neighbCount > 0)
        {
            // Randomly choose a neighbouring unvisited Cells, destroy wall between them and
            // make the new Cell the current Cell.
            int rand = Random.Range(0, neighbCount);
            Direction dir = (Direction)availableDirections[rand];
            switch (dir)
            {
                case Direction.North:
                    DestroyWallIfItExists(_cells[_currX, _currY].NorthWall);
                    _cells[_currX, _currY + 1].Visited = true;
                    _currY++;
                    break;
                case Direction.South:
                    DestroyWallIfItExists(_cells[_currX, _currY - 1].NorthWall);
                    _cells[_currX, _currY - 1].Visited = true;
                    _currY--;
                    break;
                case Direction.East:
                    DestroyWallIfItExists(_cells[_currX, _currY].EastWall);
                    _cells[_currX + 1, _currY].Visited = true;
                    _currX++;
                    break;
                case Direction.West:
                    DestroyWallIfItExists(_cells[_currX - 1, _currY].EastWall);
                    _cells[_currX - 1, _currY].Visited = true;
                    _currX--;
                    break;
            }
            // Remember the direction to the chosen neighbour, so backtracking is possible.
            _lastDirections.Push(dir);

            // Turn the new current Cell Gray. If the Cell already was gray, turn it white.
            _rend = _cells[_currX, _currY].GetComponent<Renderer>();
            _rend.material.color = _rend.material.color == Color.gray ? Color.white : Color.gray;
        }
        // If all adjacent Cells have been visited, back up to the previous Cell if there is a previous Cell.
        else if (_lastDirections.Count > 0)
        {
            // Turn current Cell white
            _cells[_currX, _currY].GetComponent<Renderer>().material.color = Color.white;

            // Back up to previous cell.
            Backtrack();
        }
        else
        {
            // Turn last Cell white.
            _cells[_currX, _currY].GetComponent<Renderer>().material.color = Color.white;
            CourseComplete = true;
        }
    }
    
    /// <summary>
    /// Makes the last visited cell the current cell again.
    /// </summary>
    private void Backtrack()
    {
        //Get the previous direction (pop from the stack) and go to the Cell opposite of that direction.
        Direction lastDirection = _lastDirections.Pop();
        if (lastDirection == Direction.North) { _currY--; }
        if (lastDirection == Direction.South) { _currY++; }
        if (lastDirection == Direction.East) { _currX--; }
        if (lastDirection == Direction.West) { _currX++; }
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
    /// <param name="x">The row the cell is in.</param>
    /// <param name="y">The column the cell is in.</param>
    /// <returns>Visitability.</returns>
    private bool CellIsAvailable(int x, int y) =>
        x >= 0
        && x < _mazeColumns
        && y >= 0
        && y < _mazeRows
        && !_cells[x, y].Visited;
}
