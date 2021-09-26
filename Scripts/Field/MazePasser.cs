using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePasser : MonoBehaviour
{
    List<Vector2Int> pathToFinish = new List<Vector2Int>();
    bool[,] maze;
    Vector2Int size;
    Vector2Int finish;
    Vector2Int currentCell;

    Vector2Int currentDirection;

    #region TODO: delete
    //public Player p;
    //bool needToMove = false;
    //int i_current = 0; 
    #endregion

    public List<Vector2Int> NegrKalian(bool[,] maze, Vector2Int start, Vector2Int finish)
    {
        this.maze = maze;
        size.x = maze.GetLength(0);
        size.y = maze.GetLength(1);
        this.finish = finish;

        currentCell = start;
        DetermineStartDirection();

        do
        {
            DoStep();

            int index = FindCellInList(pathToFinish, currentCell);
            if (index != -1)
                RemoveCycle(index);

        } while (!CurrentCellIsFinish());

        return pathToFinish;
    }

    #region TODO: change to mainmenu
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.G))
    //    {
    //        needToMove = true;

    //    }
    //    if (needToMove && !p.Tractor.isMoving)
    //    {
    //        goNext();
    //        if (i_current == pathToFinish.Count) needToMove = false;
    //    }
    //}

    //private void goNext()
    //{
    //    p.Controls.SetMove(new Vector3(pathToFinish[i_current].x, 0, pathToFinish[i_current].y) - p.Controls.currentCell);
    //    i_current++;
    //} 
    #endregion

    private bool CurrentCellIsFinish()
    {
        if (StepLeft() == finish || StepForward() == finish || StepRight() == finish)
            return true;
        else return false;
    }

    private void DetermineStartDirection()
    {
        if (currentCell.x == 0)
            currentDirection = Vector2Int.up;
        else if (currentCell.x == size.x - 1)
            currentDirection = Vector2Int.down;
        else if (currentDirection.y == 0)
            currentDirection = Vector2Int.right;
        else if (currentDirection.y == size.y - 1)
            currentDirection = Vector2Int.left;
    }

    private void DoStep()
    {
        bool stepDone = false;

        do
        {
            if (CanDoStep(StepLeft()))
            {
                DoNewStep(StepLeft());
                stepDone = true;
            }
            else if (CanDoStep(StepForward()))
            {
                DoNewStep(StepForward());
                stepDone = true;
            }
            else if (CanDoStep(StepRight()))
            {
                DoNewStep(StepRight());
                stepDone = true;
            }
            else TurnRight();
        } while (!stepDone);
    }

    private Vector2Int StepLeft()
    {
        Vector2Int newCell;
        if (currentDirection == Vector2Int.up)
            newCell = currentCell + Vector2Int.left;
        else if (currentDirection == Vector2Int.down)
            newCell = currentCell + Vector2Int.right;
        else if (currentDirection == Vector2Int.right)
            newCell = currentCell + Vector2Int.up;
        else
            newCell = currentCell + Vector2Int.down;
        return newCell;
    }

    private Vector2Int StepForward()
    {
        Vector2Int newCell;
        if (currentDirection == Vector2Int.up)
            newCell = currentCell + Vector2Int.up;
        else if (currentDirection == Vector2Int.down)
            newCell = currentCell + Vector2Int.down;
        else if (currentDirection == Vector2Int.right)
            newCell = currentCell + Vector2Int.right;
        else
            newCell = currentCell + Vector2Int.left;
        return newCell;
    }

    private Vector2Int StepRight()
    {
        Vector2Int newCell;
        if (currentDirection == Vector2Int.up)
            newCell = currentCell + Vector2Int.right;
        else if (currentDirection == Vector2Int.down)
            newCell = currentCell + Vector2Int.left;
        else if (currentDirection == Vector2Int.right)
            newCell = currentCell + Vector2Int.down;
        else
            newCell = currentCell + Vector2Int.up;
        return newCell;
    }

    private void TurnRight()
    {
        Vector2Int newDirection;
        if (currentDirection == Vector2Int.up)
            newDirection = Vector2Int.right;
        else if (currentDirection == Vector2Int.down)
            newDirection = Vector2Int.left;
        else if (currentDirection == Vector2Int.right)
            newDirection = Vector2Int.down;
        else
            newDirection = Vector2Int.up;

        currentDirection = newDirection;
    }

    private void DoNewStep(Vector2Int newCell)
    {
        if (currentCell.x == newCell.x)
        {
            if (newCell.y > currentCell.y)
                currentDirection = Vector2Int.up;
            else
                currentDirection = Vector2Int.down;
        }
        else
        {
            if (newCell.x > currentCell.x)
                currentDirection = Vector2Int.right;
            else
                currentDirection = Vector2Int.left;
        }

        pathToFinish.Add(newCell);
        currentCell = newCell;
    }

    private bool CanDoStep(Vector2Int step)
    {
        if (step.x > 0 && step.x < size.x &&
            step.y > 0 && step.y < size.y &&
            maze[step.x, step.y] == false)
            return true;
        else return false;
    }

    private int FindCellInList(List<Vector2Int> list, Vector2Int cell)
    {
        for (int i = 0; i < list.Count; i++)
            if (cell == list[i])
                return i;
        return -1;
    }

    private void RemoveCycle(int i)
    {
        currentCell = pathToFinish[i];
        pathToFinish.RemoveRange(i + 1, pathToFinish.Count - (i + 1));
    }
}
