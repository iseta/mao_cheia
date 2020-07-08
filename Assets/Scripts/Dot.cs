using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public bool isMatched = false;
    public int column;
    public int row;
    public int previousColumn, previousRow;
    public int targetX;
    public int targetY;
    public Board board;

    [Header("Swipe Stuff")]
    public float swipeAngle = 0;

    [Header("Power-ups")]
    public bool isColumnBomb = false;
    public bool isRowBomb = false;
    public GameObject rowArrow, columnArrow;

    public GameObject otherDot;
    private Vector2 firstTouchPosition, finalTouchPosition;
    private Vector2 tempPosition;
    private FindMatches findMatches;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
    }

    //Tests & debug
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = transform;
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(mySprite.color.r, mySprite.color.g, mySprite.color.b, .5f);
        }
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move towards target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .5f);
            if (board.allDots[column, row] != this.gameObject){
                board.allDots[column, row] = this.gameObject;
            };
            findMatches.FindAllMatches();

        }
        else
        {
            //Directly set
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move towards target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .5f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            };
            findMatches.FindAllMatches();

        }
        else
        {
            //Directly set
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(0.2f);
        if (otherDot != null)
        {
            Dot other = otherDot.GetComponent<Dot>();
            if (!isMatched && !other.isMatched)
            {
                other.row = row;
                other.column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }
            else
            {
                //other.previousColumn = other.column;
                //other.previousRow = other.row;
                //previousRow = row;
                //previousColumn = column;
                board.DestroyMatches();
            }
            otherDot = null;
        }
    }

    private void OnMouseDown()
    {
        if(board.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();

        }
    }

    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
        MovePieces();
        board.currentState = GameState.wait;
        board.currentDot = this;
    }
    
    void MovePieces()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width-1)
        {
            //Right Swipe
            otherDot = board.allDots[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height-1)
        {
            //Up Swipe
            otherDot = board.allDots[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135 )&& column > 0)
        {
            //Left Swipe
            otherDot = board.allDots[column -1 , row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe
            otherDot = board.allDots[column, row -1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row += 1;
            row--;
        }
        StartCoroutine(CheckMoveCo());
    }

    void FindMatches() {
        if(column>0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if (leftDot1 != null && rightDot1 != null) {
                if (leftDot1.tag == gameObject.tag && rightDot1.tag == gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject downDot1 = board.allDots[column, row - 1];
            GameObject upDot1 = board.allDots[column, row + 1];
            if (downDot1 != null && upDot1!=null)
            {
                if (downDot1.tag == gameObject.tag && upDot1.tag == gameObject.tag)
                {
                    downDot1.GetComponent<Dot>().isMatched = true;
                    upDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = transform;
    }

    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = transform;
    }
}

