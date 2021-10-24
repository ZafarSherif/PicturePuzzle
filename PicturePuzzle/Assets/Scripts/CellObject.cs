using System.Collections;
using UnityEngine;

public class CellObject : MonoBehaviour
{
    // Start is called before the first frame update
    private int rowNumber = 0;
    private int colNumber = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDrag()
    {

    }

    public void setController(int rowNum, int colNum)
    {
        rowNumber = rowNum;
        colNumber = colNum;
    }
    public void setCordinates(int rowNum, int colNum)
    {
        rowNumber = rowNum;
        colNumber = colNum;
    }
    public int getRowNumber()
    {
        return rowNumber;
    }
    public int getColNumber()
    {
        return colNumber;
    }
    bool RectIntersect(GameObject _rectData, Vector2 _pos)
    {   
        float x = _pos.x;
        float y = _pos.y;
        float x1 = _rectData.transform.position.x - 0.75f;
        float y1 = _rectData.transform.position.y - 0.75f;
        float x2 = _rectData.transform.position.x + 0.75f;
        float y2 = _rectData.transform.position.y + 0.75f;

        if (x > x1 && x < x2 && y > y1 && y < y2)
            return true;

        return false;
    }
}
