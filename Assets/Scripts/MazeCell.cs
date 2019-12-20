using UnityEngine;

public class MazeCell : MonoBehaviour
{
    public bool Visited = false;
    public GameObject NorthWall, SouthWall, EastWall, WestWall;
    public Color StartColor;
}
