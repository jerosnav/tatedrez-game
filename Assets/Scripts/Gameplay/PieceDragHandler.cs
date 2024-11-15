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
        
        public BoardGrid BoardGrid { get; set; }
        public Vector2Int GridPosition { get; set; }

        private Vector3 m_startDragPosition;
        private Vector3 m_restartPosition;
        private Vector2Int m_gridPosition;
        private List<Vector2Int> m_validPositions = new List<Vector2Int>();
        private bool m_isMoving;

        private void Awake()
        {
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
        
        public void UpdateVisuals()
        {
            if (!m_pieceData) return;
            if (!m_shape) m_shape = GetComponentInChildren<Image>();
            m_shape.sprite = m_pieceData.Shape;
        }
        
        public void SetColor(Color color)
        {
            m_shape.color = color;
        }
        
        public bool IsValidPosition(Vector2Int gridPosition)
        {
            return BoardGrid == null || m_validPositions.Contains(gridPosition);
        }
        
        public void SetDraggable(bool isDraggable)
        {
            m_shape.raycastTarget = isDraggable;
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
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            UpdateValidPositions();
            SetDraggable(false);
            m_startDragPosition = transform.position;
            transform.position = eventData.position;
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!eventData.used)
            {
                transform.position = m_startDragPosition;
                SetDraggable(true);
            }
        }

        private void UpdateValidPositions()
        {
            m_validPositions.Clear();
            if (BoardGrid != null)
            {
                m_pieceData.PopulateWithValidMoves(BoardGrid, GridPosition, m_validPositions);
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
