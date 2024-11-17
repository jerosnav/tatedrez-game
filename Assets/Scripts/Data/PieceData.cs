using System.Collections.Generic;
using AwesomeCompany.Tatedrez.Core;
using AwesomeCompany.Tatedrez.GridSystem;
using UnityEngine;

namespace AwesomeCompany.Tatedrez.Data
{
    public abstract class PieceData : ScriptableObject
    {
        [SerializeField] private Sprite m_shape;
        public Sprite Shape => m_shape;

        public abstract void PopulateWithValidMoves(BoardController boardController, Vector2Int gridPosition,
            List<Vector2Int> validMoves);
    }
}