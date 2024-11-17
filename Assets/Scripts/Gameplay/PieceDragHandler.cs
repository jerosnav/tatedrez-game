using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Data;
using AwesomeCompany.Tatedrez.GridSystem;
using AwesomeCompany.Tatedrez.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AwesomeCompany.Tatedrez.Gameplay
{
    public class PieceDragHandler : Piece, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Header("References")] 
        [SerializeField] private Image m_shape;
        
        private Vector3 m_startDragPosition;
        private Vector3 m_restartPosition;
        private bool m_isMoving;
        public bool IsPlacedOnBoard => GameManager.Instance.BoardController.IsPieceOnBoard(this);

        public Vector2Int GridPosition
        {
            get
            {
                GameManager.Instance.BoardController.TryGetGridPosition(this, out Vector2Int gridPosition);
                return gridPosition;
            }
        }

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
        
        public void SetPlayerData(PlayerData playerData)
        {
            m_playerData = playerData;
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
            return MoveTo(m_restartPosition, onFinished);
        }
        
        public bool MoveTo(BoardController boardController, Vector2Int gridPosition, Action onFinished = null)
        {
            if(boardController.CanPlacePiece(this, gridPosition))
            {
                if (boardController.TryGetWorldPosition(gridPosition, out Vector3 worldPosition))
                {
                    return MoveTo(worldPosition, () =>
                    {
                        boardController.TryToPlacePiece(this, gridPosition);
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
    }
}
