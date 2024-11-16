using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Data;
using AwesomeCompany.Tatedrez.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AwesomeCompany.Tatedrez.Gameplay
{
    public class PieceDragHandler : MonoBehaviour, ICellElement, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private PieceData m_pieceData;
        [Header("References")] 
        [SerializeField] private Image m_shape;

        public PieceData PieceData => m_pieceData;
        public BoardGrid BoardGrid { get; set; }
        public Vector2Int GridPosition { get; set; }
        public Object Value => m_playerData;

        private Vector3 m_startDragPosition;
        private Vector3 m_restartPosition;
        private Vector2Int m_gridPosition;
        private PlayerData m_playerData;
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
            return BoardGrid == null || m_validPositions.Count == 0 || m_validPositions.Contains(gridPosition);
        }


        public void SetDraggable(bool isDraggable)
        {
            m_shape.raycastTarget = isDraggable;
        }
        
        public void SetRestartPosition()
        {
            m_restartPosition = transform.position;
        }

        public bool MoveToRestartPosition(Action onFinished = null)
        {
            if (MoveTo(m_restartPosition, onFinished))
            {
                GridPosition = default;
                BoardGrid = null;
            }
            return false;
        }
        
        public bool MoveTo(BoardGrid boardGrid, Vector2Int gridPosition, Action onFinished = null)
        {
            if(boardGrid.CanPlaceElement(this, gridPosition))
            {
                BoardGrid = boardGrid;
                UICell uiCell = FindObjectsOfType<UICell>().FirstOrDefault(o => o.GridPositions == gridPosition);
                if (uiCell)
                {
                    return MoveTo(uiCell.transform.position, () =>
                    {
                        BoardGrid.TryPlaceElement(this, gridPosition);
                        Debug.Log(this + " " + GridPosition);
                        onFinished?.Invoke();
                    });
                }
            }

            return false;
        }

        public bool MoveTo(Vector3 position, Action onFinished)
        {
            if (m_isMoving) return false;
            StartCoroutine(MoveToPositionCo(position, onFinished));
            return true;
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
        
        private IEnumerator MoveToPositionCo(Vector3 position, Action onFinished)
        {
            float dist;
            do
            {
                transform.position = Vector3.Lerp(transform.position, position, Time.fixedDeltaTime * 5f);
                dist = Vector3.Distance(transform.position, position);
                yield return null;
            } while (dist > 0.01f);
            transform.position = position;

            m_isMoving = false;
            onFinished?.Invoke();
        }

        public void SetPlayerData(PlayerData playerData)
        {
            m_playerData = playerData;
        }
    }
}
