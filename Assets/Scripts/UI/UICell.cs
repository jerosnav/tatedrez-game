using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Gameplay;
using AwesomeCompany.Tatedrez.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICell : MonoBehaviour, IDropHandler
{
    [SerializeField] private Vector2Int m_gridPosition;
    [SerializeField] private Image m_image;
    [SerializeField] private BoardWidget m_boardWidget;
    
    public void Setup(BoardWidget boardWidget, Vector2Int gridPosition, Color color)
    {
        m_boardWidget = boardWidget;
        m_gridPosition = gridPosition;
        m_image.color = color;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag 
            && eventData.pointerDrag.TryGetComponent<ICellElement>(out ICellElement cellElement))
        {
            if (m_boardWidget.BoardGrid.TryPlaceElement(cellElement, m_gridPosition))
            {
                eventData.pointerDrag.transform.position = transform.position;
                eventData.Use();
                GameManager.Instance.EndPlayerTurn();
            }
            Debug.Log(eventData.pointerDrag, eventData.pointerDrag);
        }
    }
}
