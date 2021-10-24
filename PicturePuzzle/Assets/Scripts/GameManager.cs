using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject[ , ] gameCells;
    private Vector2[ , ] originalCellPositions;
    public int gridSizeX = 8;
    public int gridSizeY = 5;
    public int movingRow = 0;
    public int movingCol = 0;
    private int mouseDragDirection = 0;
    private GameObject startScrollObj;
    [SerializeField] GameObject gridHolder;
    // Start is called before the first frame update
    void Start()
    {
        gameCells = new GameObject[gridSizeX,gridSizeY];
        originalCellPositions = new Vector2[gridSizeX,gridSizeY];
        ArrangeGrid();
        ShuffleGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            mouseDragDirection = 0;
            Vector2 mousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
            Vector2 localMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            GameObject obj = GetMouseOverObject(localMousePosition);
            startScrollObj = obj;
        }

        if (Input.GetMouseButtonUp(0))
        {
            localOnMouseDragEnd();
        }

        if(Input.GetMouseButton(0))
        {
            onMouseDrag();
        }
    }

    private void ArrangeGrid()
    {
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                GameObject cell = new GameObject();
                cell.AddComponent<SpriteRenderer>();
                cell.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(((i * gridSizeY) + j + 1).ToString("D2"));
                cell.name = "Cell" + ((i * gridSizeY) + j + 1).ToString("D2");
                float width = cell.GetComponent<SpriteRenderer>().bounds.size.x;
                float height = cell.GetComponent<SpriteRenderer>().bounds.size.y;
                Vector2 origin = new Vector2(0 - ((gridSizeX * width) / 2), 0 + ((gridSizeY * height) / 2));
                cell.transform.position = origin + new Vector2((width * i), (height * -j));
                cell.transform.parent = gridHolder.transform;
                cell.AddComponent<BoxCollider2D>();
                cell.AddComponent<CellObject>();
                cell.GetComponent<CellObject>().setController(i, j);
                gameCells[i, j] = cell;
                originalCellPositions[i, j] = cell.transform.position;
            }
        }
    }
    public void ShuffleGrid()
    {
        int randomShuffleCount = Random.Range(2, 5);
        for (int i = 0; i < randomShuffleCount; i++)
            {
                int rowShuffleIndex1 = Random.Range(0, gridSizeX);
                int rowShuffleIndex2 = Random.Range(0, gridSizeX);
                int colShuffleIndex1 = Random.Range(0, gridSizeY);
                int colShuffleIndex2 = Random.Range(0, gridSizeY);
                swapCol(colShuffleIndex1, colShuffleIndex2);
                swapRow(rowShuffleIndex1, rowShuffleIndex2);
            }
    }

    public void onMouseDrag()
    {
        int rowNum = startScrollObj.GetComponent<CellObject>().getRowNumber();
        int colNum = startScrollObj.GetComponent<CellObject>().getColNumber();
        Vector2 mousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
        Vector2 localMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        GameObject startObj = gameCells[rowNum, colNum];
        GameObject obj = GetMouseOverObject(localMousePosition);
        if (mouseDragDirection == 1)
        {
            mouseDragDirection = 1;
            for (int i = 0; i < gridSizeY; i++)
            {
                Vector3 newPos = new Vector3(localMousePosition.x, gameCells[rowNum, i].transform.position.y, -1);
                gameCells[rowNum, i].transform.position = newPos;
                movingRow = rowNum;
            }
            return;
        }
        else if (mouseDragDirection == 2)
        {
            mouseDragDirection = 2;
            for (int i = 0; i < gridSizeX; i++)
            {
                Vector3 newPos = new Vector3(gameCells[i, colNum].transform.position.x, localMousePosition.y, -1);
                gameCells[i, colNum].transform.position = newPos;
                movingCol = colNum;
            }
            return;
        }
        if (!obj)
            return;
        if ((startObj.transform.position.x != obj.transform.position.x))
            mouseDragDirection = 1;
        else if((startObj.transform.position.y != obj.transform.position.y))
            mouseDragDirection = 2;
    }

    public void onMouseDragEnd(Vector2 objPos, int rowNum, int colNum) 
    {
        resetZCordinate();
    }
    public void localOnMouseDragEnd()
    {
        resetZCordinate();
        FitToPosition();
    }

    private void FitToPosition()
    {
        Vector2 mousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
        Vector2 localMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        GameObject startObj = gameCells[movingRow, movingCol];
        GameObject obj = GetMouseOverObject(localMousePosition);
        if(!obj)
        {
            swapRow(movingRow, movingRow);
            swapCol(movingCol, movingCol);
            return;
        }
        int targetRow = obj.GetComponent<CellObject>().getRowNumber();
        int targetCol = obj.GetComponent<CellObject>().getColNumber();
        if (mouseDragDirection == 1)
        {
            moveRow(movingRow, targetRow);
        }
        else if (mouseDragDirection == 2)
        {
            moveCol(movingCol, targetCol);
        }

    }
    private void moveCol(int incomingCol, int targetCol)
    {
        int moveCount = Mathf.Abs(incomingCol - targetCol);
        int mover = (incomingCol - targetCol > 0) ? 1 : -1;
        for (int i = 0; i < moveCount; i++)
        {
            swapCol(incomingCol, targetCol + (i * mover));
        }
    }
    private void moveRow(int incomingRow, int targetRow)
    {
        int moveCount = Mathf.Abs(incomingRow - targetRow);
        int mover = (incomingRow - targetRow > 0) ? 1 : -1;
        for (int i = 0; i < moveCount; i++)
        {
            swapRow(incomingRow, targetRow + (i * mover));
        }
    }
    private void swapCol(int incomingCol, int targetCol)
    {
        GameObject tempGo;
        Vector3 tempPos;
        for (int i = 0; i < gridSizeX; i++)
        {
            tempPos = originalCellPositions[i, targetCol];
            gameCells[i, targetCol].transform.position = originalCellPositions[i, incomingCol];
            gameCells[i, incomingCol].transform.position = tempPos;

            tempGo = gameCells[i, targetCol];
            gameCells[i, targetCol] = gameCells[i, incomingCol];
            gameCells[i, incomingCol] = tempGo;
            gameCells[i, targetCol].GetComponent<CellObject>().setCordinates(i, targetCol);
            gameCells[i, incomingCol].GetComponent<CellObject>().setCordinates(i, incomingCol);
        }
    }
    private void swapRow(int incomingRow, int targetRow)
    {
        GameObject tempGo;
        Vector3 tempPos;
        for (int i = 0; i < gridSizeY; i++)
        {
            tempPos = originalCellPositions[targetRow, i];
            gameCells[targetRow, i].transform.position = originalCellPositions[incomingRow, i];
            gameCells[incomingRow, i].transform.position = tempPos;

            tempGo = gameCells[targetRow, i];
            gameCells[targetRow, i] = gameCells[incomingRow, i];
            gameCells[incomingRow, i] = tempGo;
            gameCells[targetRow, i].GetComponent<CellObject>().setCordinates(targetRow, i);
            gameCells[incomingRow, i].GetComponent<CellObject>().setCordinates(incomingRow, i);
        }
    }

    private void resetZCordinate()
    {
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                Vector3 newPos = new Vector3(gameCells[i, j].transform.position.x, gameCells[i, j].transform.position.y, 0);
                gameCells[i, j].transform.position = newPos;
            }
        }
    }

    private GameObject GetMouseOverObject(Vector2 _pos)
    {
        float x = _pos.x;
        float y = _pos.y;

        Vector2 tempPos;
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                tempPos = originalCellPositions[i, j];

                float x1 = tempPos.x - 0.75f;
                float y1 = tempPos.y - 0.75f;
                float x2 = tempPos.x + 0.75f;
                float y2 = tempPos.y + 0.75f;
                if (x > x1 && x < x2 && y > y1 && y < y2)
                    return gameCells[i, j];
            }
        }
        return null;
    }

    private void getAcceptedCells()
    {

    }
}
