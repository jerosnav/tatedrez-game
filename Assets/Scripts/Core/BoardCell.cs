using System;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Gameplay;
using AwesomeCompany.Tatedrez.GridSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AwesomeCompany.Tatedrez. Core
{
    public class BoardCell : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Vector2Int m_gridPosition;
        [SerializeField] private Image m_image;
        [SerializeField] private BoardController m_boardController;

        public Vector2Int GridPositions => m_gridPosition;

        public void Setup(BoardController boardController, Vector2Int gridPosition, Color color)
        {
            m_boardController = boardController;
            m_gridPosition = gridPosition;
            m_image.color = color;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag
                && eventData.pointerDrag.TryGetComponent(out PieceDragHandler pieceDragHandler))
            {
                if (m_boardController.TryToPlacePiece(pieceDragHandler, m_gridPosition))
                {
                    eventData.pointerDrag.transform.position = transform.position;
                    eventData.Use();
                    GameManager.Instance.EndPlayerTurn();
                }
            }
        }
    }
}