using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    System.Random random = new System.Random();

    private int[] size;
    private bool[,] maze;
    private List<int[]> visitedVertex = new List<int[]>();
    private List<int[]> unvisitedVertex = new List<int[]>();
    private List<int[]> currentTree = new List<int[]>();
    private int[] currentVertex;

    public bool[,] GenerateMaze(int[] _size)
    {
        visitedVertex.Clear();
        unvisitedVertex.Clear();
        currentTree.Clear();
        currentVertex = new int[2];

        size = _size;
        size[0] = size[0] * 2 + 1;
        size[1] = size[1] * 2 + 1;
        CreateMazeGrid(size);
        int[] firstVertex = ReturnRandomUnvisitedVertex();

        PutFromUnvisitedToVisited(unvisitedVertex.IndexOf(firstVertex));

        while (unvisitedVertex.Count > 0)
        {
            ReturnRandomUnvisitedVertex().CopyTo(currentVertex, 0);
            int visitedIndex = DoTreeCycle();
            PutTreeCycleToMaze(visitedIndex);
            currentTree.Clear();
        }

        return maze;
    }

    private void CreateMazeGrid(int[] size)
    {
        maze = new bool[size[0], size[1]];
        for (int i = 0; i < size[0]; i++)
            for (int k = 0; k < size[1]; k++)
            {
                if (i % 2 == 0 || k % 2 == 0)
                    maze[i, k] = true;
                else
                {
                    maze[i, k] = false;
                    unvisitedVertex.Add(new int[] { i, k });
                }
            }
    }

    private void RemoveWallBetweenTwoCells(int[] firstCell, int[] secondCell)
    {
        if (firstCell[0] == secondCell[0])
            maze[firstCell[0], (firstCell[1] < secondCell[1]) ? firstCell[1] + 1 : secondCell[1] + 1] = false;
        else maze[(firstCell[0] < secondCell[0]) ? firstCell[0] + 1 : secondCell[0] + 1, firstCell[1]] = false;
    }

    private int[] ReturnRandomUnvisitedVertex()
    {
        int i = random.Next(0, unvisitedVertex.Count - 1);
        return unvisitedVertex[i];
    }

    private int[] DoRandomStepFromCurrentVertex()
    {
        int[] newCell;
        do
        {
            int direction = random.Next(1, 5);
            newCell = DoStep(direction);
        } while (!CanDoStep(newCell));
        return newCell;
    }

    private int[] DoStep(int direction)
    {
        int[] newCell = new int[currentVertex.Length];
        currentVertex.CopyTo(newCell, 0);
        switch (direction)
        {
            case 1: //down
                newCell[0] += 2;
                break;
            case 2: //up
                newCell[0] -= 2;
                break;
            case 3: //right
                newCell[1] += 2;
                break;
            case 4: //left
                newCell[1] -= 2;
                break;
        }

        return newCell;
    }

    private bool CanDoStep(int[] step)
    {
        if (step[0] > 0 && step[0] < size[0] &&
            step[1] > 0 && step[1] < size[1])
            return true;
        else return false;
    }

    private int FindCellInList(List<int[]> list, int[] cell)
    {
        for (int i = 0; i < list.Count; i++)
            if (cell[0] == list[i][0] && cell[1] == list[i][1])
                return i;
        return -1;
    }

    private int DoTreeCycle()
    {
        int[] startVertex = new int[currentVertex.Length];
        currentVertex.CopyTo(startVertex, 0);
        currentTree.Add(startVertex);

        while (true)
        {
            int[] newMove = DoRandomStepFromCurrentVertex();

            int index = FindCellInList(currentTree, newMove);
            if (index != -1)
            {
                currentTree[index].CopyTo(currentVertex, 0);
                currentTree.RemoveRange(index + 1, currentTree.Count - (index + 1));
            }
            else
            {
                int visitedCellIndex = FindCellInList(visitedVertex, newMove);
                if (visitedCellIndex != -1)
                    return visitedCellIndex;

                currentTree.Add(newMove);
                newMove.CopyTo(currentVertex, 0);
            }
        }
    }

    private void PutTreeCycleToMaze(int visitedIndex)
    {
        for (int i = 0; i < currentTree.Count - 1; i++)
            RemoveWallBetweenTwoCells(currentTree[i], currentTree[i + 1]);
        RemoveWallBetweenTwoCells(currentTree[currentTree.Count - 1], visitedVertex[visitedIndex]);

        for (int i = 0; i < currentTree.Count; i++)
        {
            int index = FindCellInList(unvisitedVertex, currentTree[i]);
            PutFromUnvisitedToVisited(index);
        }
    }

    private void PutFromUnvisitedToVisited(int index_needToReplace)
    {
        visitedVertex.Add(unvisitedVertex[index_needToReplace]);
        unvisitedVertex.RemoveAt(index_needToReplace);
    }
}
