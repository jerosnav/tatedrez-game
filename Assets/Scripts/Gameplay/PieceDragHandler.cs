using System;
using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AwesomeCompany.Tatedrez.Gameplay
{
    public class PieceDragHandler : MonoBehaviour, ICellElement, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private PieceData m_pieceData;
        [Header("References")] 
        [SerializeField] private Image m_shape;
        
        public Vector2Int GridPosition { get; set; }
        public BoardGrid BoardGrid { get; set; }
        

        private Graphic m_graphic;
        private Vector3 m_startDragPosition;
        private Vector3 m_restartPosition;
        private bool m_isMoving;

        private void Awake()
        {
            m_graphic = GetComponent<Graphic>();
            UpdateVisuals();
        }
        
        private void OnValidate()
        {
            UpdateVisuals();
        }
        
        public void SetPieceData(PieceData pieceData)
        {
            m_pieceData = pieceData;
            UpdateVisuals();
        }
        
        public void SetDraggable(bool isDraggable)
        {
            m_graphic.raycastTarget = isDraggable;
        }
        
        public void SetRestartPosition()
        {
            m_restartPosition = transform.position;
        }

        public void MoveToRestartPosition(Action onFinished = null)
        {
            if (m_isMoving) return;
            m_isMoving = true;
            GridPosition = default;
            BoardGrid = null;
            StartCoroutine(MoveToResetPositionCo(onFinished));
        }

        public void UpdateVisuals()
        {
            if (!m_pieceData) return;
            if (!m_shape) m_shape = GetComponentInChildren<Image>();
            m_shape.sprite = m_pieceData.Shape;
        }

        public void PopulateWithValidMoves(BoardGrid boardGrid, Vector2Int gridPosition, List<Vector2Int> validMoves)
        {
            m_pieceData.PopulateWithValidMoves(boardGrid, gridPosition, validMoves);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            SetDraggable(false);
            m_startDragPosition = transform.position;
            transform.position = eventData.position;
            transform.SetAsLastSibling();
            transform.parent.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            SetDraggable(true);
            if (!eventData.used)
            {
                transform.position = m_startDragPosition;
            }
        }
        
        private IEnumerator MoveToResetPositionCo(Action onFinished)
        {
            float dist;
            do
            {
                transform.position = Vector3.Lerp(transform.position, m_restartPosition, Time.deltaTime);
                dist = Vector3.Distance(transform.position, m_restartPosition);
                yield return null;
            } while (!Mathf.Approximately(dist, 0f) );

            m_isMoving = false;
            onFinished?.Invoke();
        }
    }
}
