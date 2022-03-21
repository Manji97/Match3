using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Point : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent<int, int> OnClick;
    public UnityEvent OnEmptyChange;
    public  bool isEmpty;
    public bool IsEmpty
    {
        get { return isEmpty; }
        set { isEmpty = value;
        if(value==true)
            {
                OnEmptyChange.Invoke();
            }
        }
    }
    public int row;
    public int col;
    public PieceType type;
    public PieceType Type
    {
        get => type;
        set
        {
            type = value;
            int index = ((int)type) * 5 + (int)color;
            if (index <= 20)
            {
                item.sprite = sprites[index];
            }
            else
            {
                item.sprite = sprites[20];
            }

        }
    }
    public ColorType color;
    public ColorType Color
    {
        get => color;
        set
        {
            color = value;

            int index = ((int)type) * 5 + (int)color;
            if (index <= 20)
            {
                item.sprite = sprites[index];
            }
            else
            {
                item.sprite = sprites[20];
            }
        }
    }
    public int x;
    public int y;

    public Image item;

    public Sprite[] sprites;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick.Invoke(x, y);
    }
}
public enum PieceType
{
    NORMAL,
    ROW,
    COLUMN,
    BOOM,
    SUPER,
}
public enum ColorType
{
    BLUE,
    GREEN,
    ORANGE,
    PURPLE,
    RED,
    NON,
}