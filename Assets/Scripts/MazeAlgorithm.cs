using System.Collections;
using UnityEngine;

public abstract class MazeAlgorithm
{
    protected MazeCell[,] _cells;
    protected int _mazeRows, _mazeColumns;
    protected WaitForSeconds _stepDelay;
    public bool CourseComplete;

    protected MazeAlgorithm(MazeCell[,] mazeCells, float delay)
    {
        _cells = mazeCells;
        _mazeColumns = mazeCells.GetLength(0);
        _mazeRows = mazeCells.GetLength(1);
        _stepDelay = new WaitForSeconds(delay);
    }

    public abstract IEnumerator Generate();
}
