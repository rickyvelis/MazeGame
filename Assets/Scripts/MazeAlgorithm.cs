using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MazeAlgorithm
{
    protected MazeCell[,] _cells;
    protected int _mazeRows, _mazeColumns;
    protected float _stepDelay;
    public bool CourseComplete;

    protected MazeAlgorithm(MazeCell[,] mazeCells, float delay)
    {
        _cells = mazeCells;
        _mazeRows = mazeCells.GetLength(0);
        _mazeColumns = mazeCells.GetLength(1);
        _stepDelay = delay;
    }
    
    public abstract IEnumerator Generate();
}
