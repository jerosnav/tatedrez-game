using System.Collections;
using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Data;
using UnityEngine;
using UnityEngine.UI;

namespace AwesomeCompany.Tatedrez.Gameplay
{
    public class Piece : MonoBehaviour, ICellElement
    {
        [SerializeField] private PieceData m_pieceData;
        [SerializeField] private TeamData m_teamData;

        [Header("References")] [SerializeField]
        private Image m_shape;

        public Vector2Int GridPosition { get; }
        public BoardGrid BoardGrid { get; }


        public void SetPieceData(PieceData pieceData)
        {
            m_pieceData = pieceData;
            m_shape.sprite = m_pieceData.Shape;
        }

        public void PopulateWithValidMoves(BoardGrid boardGrid, Vector2Int gridPosition, List<Vector2Int> validMoves)
        {
            m_pieceData.PopulateWithValidMoves(boardGrid, gridPosition, validMoves);
        }
    }
}
