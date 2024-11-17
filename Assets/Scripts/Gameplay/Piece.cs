using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.Data;
using AwesomeCompany.Tatedrez.GridSystem;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Gameplay
{
    public class Piece : MonoBehaviour
    {
        [SerializeField] protected PieceData m_pieceData;
        [SerializeField] protected PlayerData m_playerData;
        [SerializeField] protected BoardController m_boardController;
        [SerializeField] protected Vector2Int m_gridPosition;

        public PieceData Data => m_pieceData;
        public PlayerData Owner => m_playerData;

        protected List<Vector2Int> m_reusableValidPositions = new List<Vector2Int>();

        public BoardController BoardController => m_boardController;
        public Vector2Int GridPosition => m_gridPosition;
        public bool IsPlacedOnBoard => m_boardController != null;

        public bool IsValidPosition(Vector2Int gridPosition)
        {
            // If cell grid is null, the piece is still to be placed on board, so any free position is valid
            if (BoardController == null) return true;
            
            // Populate with all the valid moves for the current piece data assigned
            m_pieceData.PopulateWithValidMoves(BoardController, GridPosition, m_reusableValidPositions);
            return m_reusableValidPositions.Count == 0 || m_reusableValidPositions.Contains(gridPosition);
        }

        public Object Value => this;
    }
}