using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public UnityEvent<int> OnScoreChange;
    int score = 0;
    public Text scoreText;
    public int Score
    {
        set
        {
            score = value;

            OnScoreChange.Invoke(value);
        }
        get { return score; }
    }
    bool canClick = true;
    public int xDim;
    public int yDim;

    public DropItem dropItem;
    public Transform DropCanvas;
    public Point point;
    public int currentX, currentY;
    public Point[,] pieces;
    private void Start()
    {

        pieces = new Point[xDim, yDim];
        for (int i = 0; i < xDim; i++)
        {
            for (int j = 0; j < yDim; j++)
            {

                var index = Random.Range(0, 5);

                pieces[i, j] = Instantiate(point, this.transform);
                pieces[i, j].type = PieceType.NORMAL;
                pieces[i, j].Color = (ColorType)index;
                pieces[i, j].name = i + " " + j;
                pieces[i, j].IsEmpty = false;
                pieces[i, j].x = i;
                pieces[i, j].y = j;
                pieces[i, j].row = 0;
                pieces[i, j].col = 0;
                pieces[i, j].OnClick.AddListener(HandleClickPointClick);
                pieces[i, j].OnEmptyChange.AddListener(HandlePoinEmpty);
            }
        }
        StartCheck(false);
        UpdateScore(score);
        OnScoreChange.AddListener(UpdateScore);
    }

    private void HandlePoinEmpty()
    {
        Score += 5;
    }

    private void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }


    private void HandleClickPointClick(int x, int y)
    {
        if (!canClick) return;
        if (x == currentX && y == currentY) return;
        if (new Vector3(currentX - x, currentY - y, 0).magnitude > 1)
        {
            NewSelectPoint(x, y);
        }
        else
        {
            canClick = false;
            if (pieces[currentX, currentY].type == PieceType.SUPER || pieces[x, y].type == PieceType.SUPER)
            {
                if (pieces[currentX, currentY].type == PieceType.SUPER)
                {
                    pieces[currentX, currentY].color = pieces[x, y].color;
                }
                else if (pieces[x, y].type == PieceType.SUPER)
                {
                    pieces[x, y].color = pieces[currentX, currentY].color;
                }
                pieces[currentX, currentY].row = 1;
                pieces[currentX, currentY].col = 1;
            
            }


            StartCoroutine(Swap(currentX, currentY, x, y));



        }
    }
    IEnumerator Swap(int x, int y, int xx, int yy)
    {
        NewSelectPoint(-1, -1);
        Point temp = new Point();
        PieceType type = pieces[x, y].type;
        ColorType color = pieces[x, y].Color;
        pieces[x, y].Type = pieces[xx, yy].type;
        pieces[x, y].Color = pieces[xx, yy].Color;
        pieces[xx, yy].Color = color;
        pieces[xx, yy].Type = type;
        yield return new WaitForSeconds(0.7f);
        CheckColum();
        CheckRow();
        if (StillMatch())
        {
            CheckDestroy(true);
            FillSpace();
            StartCoroutine(nameof(CheckMatch));
        }
        else
        {
            type = pieces[x, y].type;
            color = pieces[x, y].Color;
            pieces[x, y].Type = pieces[xx, yy].type;
            pieces[x, y].Color = pieces[xx, yy].Color;
            pieces[xx, yy].Color = color;
            pieces[xx, yy].Type = type;
            canClick = true;
        }

    }
    IEnumerator CheckMatch()
    {

        CheckColum();
        CheckRow();

        yield return new WaitForSeconds(0.7f);


        if (StillMatch())
        {
            CheckDestroy(true);
            FillSpace();
            StartCoroutine(nameof(CheckMatch));
        }
        else
        {
            canClick = true;
        }
    }

    private void NewSelectPoint(int x, int y)
    {
        if (currentX >= 0 && currentY >= 0)
        {
            pieces[currentX, currentY].transform.localScale = new Vector3(1, 1, 1);
        }

        currentX = x;
        currentY = y;
        if (x >= 0 && y >= 0)
        {

            pieces[currentX, currentY].transform.localScale = new Vector3(1.2f, 1.2f, 1);
        }

    }
    private void StartCheck(bool isTrue)
    {
        CheckColum();
        CheckRow();

        do
        {
            CheckDestroy(isTrue);
            FillSpace();
            CheckRow();
            CheckColum();
        } while (StillMatch());
        Score = 0;
    }
    private bool StillMatch()
    {
        for (int i = 0; i < xDim; i++)
        {
            for (int j = 0; j < yDim; j++)
            {
                if (pieces[i, j].col >= 1 || pieces[i, j].row >= 1)
                    return true;
            }
        }
        return false;
    }
    public void CheckRow()
    {
        for (int x = 0; x < xDim; x++)
        {
            int same = 0;
            for (int y = 0; y < yDim - 1; y++)
            {

                if (pieces[x, y].Color == pieces[x, y + 1].Color)
                {
                    same++;

                    if (same >= 2)
                    {
                        for (int i = 0; i <= same; i++)
                        {
                            pieces[x, y + 1 - i].row = same;

                        }

                    }
                }
                else
                {
                    same = 0;
                }
            }
        }
    }
    public void CheckColum()
    {
        for (int y = 0; y < yDim; y++)
        {
            int same = 0;
            for (int x = 0; x < xDim - 1; x++)
            {

                if (pieces[x, y].Color == pieces[x + 1, y].Color)
                {
                    same++;

                    if (same >= 2)
                    {
                        for (int i = 0; i <= same; i++)
                        {
                            pieces[x + 1 - i, y].col = same;

                        }

                    }
                }
                else
                {
                    same = 0;
                }
            }
        }
    }
    public void CheckDestroy(bool isSpam)
    {
        for (int i = 0; i < xDim; i++)
        {
            for (int j = 0; j < yDim; j++)
            {
                bool isBoom = false;
                if (!pieces[i, j].IsEmpty)
                    if (pieces[i, j].row >= 1 || pieces[i, j].col >= 1)
                    {
                        pieces[i, j].IsEmpty = true;
                        pieces[i, j].item.enabled = false;
                        if (isSpam)
                        {
                            var drop = Instantiate(dropItem, pieces[i, j].transform.position, Quaternion.identity);
                            drop.GetComponent<Image>().sprite = pieces[i, j].item.sprite;
                            drop.transform.SetParent(DropCanvas);
                            RunEffect(i, j, pieces[i, j].type);// check if have effect then run.

                            if (pieces[i, j].row == 4)
                            {
                                Debug.Log("Match4");
                                int index = Random.RandomRange(0, 4);
                                for (int k = 0; k < 5; k++)
                                {

                                    pieces[i, j + k].row = 1;

                                }
                                Debug.Log(pieces[i, j + index]);
                                pieces[i, j + index].row = 0;
                                pieces[i, j + index].IsEmpty = false;
                                pieces[i, j + index].item.enabled = true;
                                pieces[i, j + index].Type = PieceType.SUPER;
                                int ram = Random.Range(0, 4);
                                pieces[i, j + index].Color = (ColorType)ram;
                            }
                            else if (pieces[i, j].col == 4)
                            {
                                Debug.Log("Match4");
                                int index = Random.RandomRange(0, 4);
                                for (int k = 0; k < 5; k++)
                                {

                                    pieces[i + k, j].col = 1;

                                }
                                Debug.Log(pieces[i + index, j]);
                                pieces[i + index, j].col = 0;
                                pieces[i + index, j].IsEmpty = false;
                                pieces[i + index, j].item.enabled = true;
                                pieces[i + index, j].Type = PieceType.SUPER;
                                int ram = Random.Range(0, 4);
                                pieces[i + index, j].Color = (ColorType)ram;
                            }
                            else
                            if (pieces[i, j].col >= 2)
                            {

                                for (int n = 0; n < pieces[i, j].col; n++)
                                {
                                    if (i + n < xDim)
                                    {
                                        if (pieces[i + n, j].row >= 2)
                                        {
                                            isBoom = true;
                                            int x, y; x = i + n;
                                            y = j;
                                            for (int m = -3; m < 3; m++)
                                            {
                                                if (x + m > 0 && x + m < xDim)
                                                {
                                                    pieces[x + m, y].row = 1;
                                                    pieces[x + m, y].col = 1;
                                                }
                                                if (y + m > 0 && y + m < yDim)
                                                {
                                                    pieces[x, y + m].row = 1;
                                                    pieces[x, y + m].col = 1;
                                                }
                                            }
                                            pieces[x, y].row = 0;
                                            pieces[x, y].col = 0;
                                            pieces[x, y].IsEmpty = false;
                                            pieces[x, y].item.enabled = true;
                                            pieces[x, y].Type = PieceType.BOOM;
                                        }
                                    }
                                }
                                if (!isBoom)
                                {
                                    if (pieces[i, j].col == 3)
                                    {
                                        Debug.Log("Match4");
                                        int index = Random.Range(0, 3);
                                        for (int k = 0; k < 4; k++)
                                        {
                                            if (i + k < xDim)
                                                pieces[i + k, j].col = 1;

                                        }
                                        Debug.Log(pieces[i + index, j]);
                                        pieces[i + index, j].col = 0;
                                        pieces[i + index, j].IsEmpty = false;
                                        pieces[i + index, j].item.enabled = true;
                                        pieces[i + index, j].Type = PieceType.COLUMN;

                                    }
                                }

                            }
                            else
                            if (pieces[i, j].row == 3)
                            {
                                Debug.Log("Match4");
                                int index = Random.RandomRange(0, 3);
                                for (int k = 0; k < 4; k++)
                                {
                                    if (j + k < yDim)
                                        pieces[i, j + k].row = 1;

                                }
                                Debug.Log(pieces[i, j + index]);
                                pieces[i, j + index].row = 0;
                                pieces[i, j + index].IsEmpty = false;
                                pieces[i, j + index].item.enabled = true;
                                pieces[i, j + index].Type = PieceType.ROW;

                            }




                        }
                        pieces[i, j].row = 0;
                        pieces[i, j].col = 0;
                    }


            }
        }
    }

    private void RunEffect(int i, int j, PieceType type)
    {
        pieces[i, j].type = PieceType.NORMAL;
        switch (type)
        {
            case PieceType.NORMAL:


                break;
            case PieceType.ROW:

                for (int y = 0; y < yDim; y++)
                {
                    pieces[i, y].IsEmpty = true;
                    RunEffect(i, y, pieces[i, y].type); // Check if this one is normal or not
                    var drop = Instantiate(dropItem, pieces[i, y].transform.position, Quaternion.identity);
                    drop.GetComponent<Image>().sprite = pieces[i, y].item.sprite;
                    drop.transform.SetParent(DropCanvas);
                }

                break;
            case PieceType.COLUMN:

                for (int x = 0; x < xDim; x++)
                {
                    pieces[x, j].IsEmpty = true;
                    RunEffect(x, j, pieces[x, j].type); // Check if this one is normal or not
                    var drop = Instantiate(dropItem, pieces[x, j].transform.position, Quaternion.identity);
                    drop.GetComponent<Image>().sprite = pieces[x, j].item.sprite;
                    drop.transform.SetParent(DropCanvas);
                }
                break;
            case PieceType.BOOM:

                for (int k = -1; k < 2; k++)
                {
                    if (i + k > 0 && i + k < xDim)
                    {
                        for (int l = -1; l < 2; l++)
                        {
                            if (j + l > 0 && j + l < yDim)
                            {
                                pieces[i + k, j + l].IsEmpty = true;
                                RunEffect(i + k, j + l, pieces[i + k, j + l].type); // Check if this one is normal or not
                                var drop = Instantiate(dropItem, pieces[i + k, j + l].transform.position, Quaternion.identity);
                                drop.GetComponent<Image>().sprite = pieces[i + k, j + l].item.sprite;
                                drop.transform.SetParent(DropCanvas);
                            }
                        }
                    }
                }
                break;
            case PieceType.SUPER:
                ColorType colorType = pieces[i, j].Color;
                for (int x = 0; x < xDim; x++)
                {
                    for (int y = 0; y < yDim; y++)
                    {
                        if (pieces[x, y].color == colorType)
                        {
                            pieces[x, y].IsEmpty = true;
                            RunEffect(x, y, pieces[x, y].type); // Check if this one is normal or not
                            var drop = Instantiate(dropItem, pieces[x, y].transform.position, Quaternion.identity);
                            drop.GetComponent<Image>().sprite = pieces[x, y].item.sprite;
                            drop.transform.SetParent(DropCanvas);
                        }
                    }
                }
                break;
        }



    }

    public void FillSpace()
    {
        for (int y = 0; y < yDim; y++)
        {

            for (int x = xDim - 1; x >= 0; x--)
            {
                if (pieces[x, y].IsEmpty)
                {
                    int offset = 1;
                    while (x - offset >= 0 && pieces[x - offset, y].IsEmpty)
                    {
                        offset++;
                    }
                    if (x - offset < 0)
                    {
                        var index = Random.Range(0, 5);
                        pieces[x, y].type = PieceType.NORMAL;
                        pieces[x, y].Color = (ColorType)index;
                        pieces[x, y].IsEmpty = false;
                        pieces[x, y].row = 0;
                        pieces[x, y].col = 0;
                    }
                    else
                    {
                        MovePoint(x, y, x - offset, y);
                    }
                    pieces[x, y].item.enabled = true;
                }
            }
        }
    }
    public void MovePoint(int x1, int y1, int x2, int y2)
    {

        pieces[x1, y1].Color = pieces[x2, y2].Color;
        pieces[x1, y1].Type = pieces[x2, y2].type;
        pieces[x1, y1].IsEmpty = false;
        pieces[x2, y2].IsEmpty = true;
    }
    public void Quit()
    {
        Application.Quit();
    }
}
