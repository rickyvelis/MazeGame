using System.Collections;
using UnityEngine;

public abstract class MazeAlgorithm
{
    protected MazeCell[,] _cells;
    protected int _mazeRows, _mazeColumns;
    public WaitForSeconds StepDelay;
    public bool CourseComplete;

    protected MazeAlgorithm(MazeCell[,] mazeCells, float delay)
    {
        _cells = mazeCells;
        _mazeColumns = mazeCells.GetLength(0);
        _mazeRows = mazeCells.GetLength(1);
        StepDelay = new WaitForSeconds(delay);
    }

    public abstract IEnumerator Generate();
}
